// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using SkyApm.Common;
using SkyApm.Config;
using SkyApm.Tracing;
using SkyApm.Tracing.Segments;
using TnyFramework.Common.Attribute;
using TnyFramework.Common.Extensions;
using TnyFramework.Common.Logger;
using TnyFramework.Net.Apm.Skywalking.Hosting.Setting;
using TnyFramework.Net.Command.Dispatcher;
using TnyFramework.Net.Command.Dispatcher.Monitor;
using TnyFramework.Net.Message;
using TnyFramework.Net.Rpc;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Apm.Skywalking.Hosting.Handler
{

    public class SkywalkingRpcMonitorHandler : IRpcMonitorReceiveHandler,
        IRpcMonitorBeforeInvokeHandler, IRpcMonitorAfterInvokeHandler, IRpcMonitorResumeExecuteHandler, IRpcMonitorSuspendExecuteHandler
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<SkywalkingRpcMonitorHandler>();

        private static readonly StringOrIntValue TNY_RPC_SERVER = new(165, "tny-rpc-dotnet-server");
        private static readonly StringOrIntValue TNY_RPC_CLIENT = new(165, "tny-rpc-dotnet-client");

        private const string ARGUMENTS = "tny-rpc.arguments";
        private const string CONTACT = "tny-rpc.contact";
        private const string TARGET = "tny-rpc.target";
        private const string FORWARD = "tny-rpc.forward";
        private const string RPC_MODE = "tny-rpc.mode";
        private const string TRACE_ID = "tny-rpc.trace-id";
        private const string SPAN_ID = "tny-rpc.span-id";
        private const string SEGMENT_ID = "tny-rpc.segment-id";
        private const string RPC_PROTOCOL = "tny-rpc.protocol";

        private readonly ITracingContext tracingContext;
        private readonly IEntrySegmentContextAccessor enterSegmentContextAccessor;

        private readonly IExitSegmentContextAccessor exitSegmentContextAccessor;

        // private readonly ILocalSegmentContextAccessor localSegmentContextAccessor;
        private readonly TracingConfig tracingConfig;
        private readonly InstrumentConfig instrumentConfig;
        private readonly SkywalkingRpcMonitorProperties setting;

        private static readonly AttrKey<SegmentContext> TRACING_RPC_SPAN = AttrKeys.Key<SegmentContext>("TraceRpcSpan");

        private static readonly AttrKey<SegmentContext> TRACING_INVOKE_SPAN = AttrKeys.Key<SegmentContext>("TraceInvokeSpan");

        public SkywalkingRpcMonitorHandler(
            ITracingContext tracingContext,
            IEntrySegmentContextAccessor enterSegmentContextAccessor,
            IExitSegmentContextAccessor exitSegmentContextAccessor,
            // ILocalSegmentContextAccessor localSegmentContextAccessor,
            SkywalkingRpcMonitorProperties setting,
            IConfigAccessor configAccessor)
        {
            this.tracingContext = tracingContext;
            this.enterSegmentContextAccessor = enterSegmentContextAccessor;
            this.exitSegmentContextAccessor = exitSegmentContextAccessor;
            // this.localSegmentContextAccessor = localSegmentContextAccessor;
            this.setting = setting;
            instrumentConfig = configAccessor.Get<InstrumentConfig>();
            var config = configAccessor.Get<TracingConfig>();
            if (config == null)
            {
                tracingConfig = new TracingConfig {
                    ExceptionMaxDepth = 50
                };
            } else
            {
                tracingConfig = config;
            }
        }

        public void OnReceive(IRpcMessageEnterContext rpcMessageContext)
        {
            var message = rpcMessageContext.MessageSubject;
            var contextCarrier = LoadCarrier(message);
            var rpcSegmentContext = tracingContext.CreateEntrySegmentContext(RpcOperationName(message), contextCarrier);
            var contact = rpcMessageContext.Connector;
            TagSpanService(rpcSegmentContext, contact, message);
            var attributes = rpcMessageContext.Attributes;
            attributes.Set(TRACING_RPC_SPAN, rpcSegmentContext);
            var span = rpcSegmentContext.Span;
            LOGGER.LogInformation("OnReceive span {mode} {opName}  | {span}",
                rpcMessageContext.Mode, span.OperationName, Debug(rpcSegmentContext));
        }

        public void OnBeforeInvoke(IRpcTransactionContext rpcMessageContext)
        {
            var segmentContext = TraceOnBefore(rpcMessageContext);
            var span = segmentContext.Span;
            LOGGER.LogInformation("invoke span {mode} {op} {span}", rpcMessageContext.Mode, span.OperationName, Debug(segmentContext));
            // if (!rpcContext.Async)
            //     return;
            if (span.SpanType == SpanType.Local)
            {
                tracingContext.Release(segmentContext);
            } else
            {
                rpcMessageContext.Attributes.Set(TRACING_INVOKE_SPAN, segmentContext);
            }
        }

        private SegmentContext TraceOnBefore(IRpcTransactionContext rpcMessageContext)
        {
            SegmentContext segmentContext;
            string operationName;
            RestoreAll(rpcMessageContext);
            if (rpcMessageContext.Mode == RpcTransactionMode.Exit)
            {
                var tracingHeader = new RpcTracingHeader();
                var message = rpcMessageContext.MessageSubject;
                operationName = RemoteOperationName(rpcMessageContext);
                var carrierHeader = new TextCarrierHeaderCollection(new Dictionary<string, string>());
                segmentContext = tracingContext.CreateExitSegmentContext(operationName, Peer(rpcMessageContext.Connector), carrierHeader);
                tracingHeader.Attributes.AddRang(carrierHeader);
                message.PutHeader(tracingHeader);
                TagSpanRemote(segmentContext, rpcMessageContext.Connector, message);
                var span = segmentContext.Span;
                LOGGER.LogInformation("OnBeforeInvoke EXIT Start span {op} {span} | {header}", span.OperationName, Debug(segmentContext),
                    tracingHeader);
            } else
            {
                var message = rpcMessageContext.MessageSubject;
                operationName = LocalOperationName(rpcMessageContext);
                segmentContext = tracingContext.CreateLocalSegmentContext(operationName);
                TagSpanLocal(segmentContext, rpcMessageContext.Connector, message);
                var span = segmentContext.Span;
                LOGGER.LogInformation("OnBeforeInvoke ENTER Start span {op} {span}", span.OperationName, Debug(segmentContext));
            }
            return segmentContext;
        }

        private string Debug(SegmentContext context)
        {
            var span = context.Span;
            if (span != null)
            {
                return
                    $"Span : [SegmentId : {context.SegmentId}| TraceId : {context.TraceId} | SpanId : {span.SpanId} | ParentSpan : {span.ParentSpanId}]";
            } else
            {
                return "Span : [null]";
            }
        }

        public void OnAfterInvoke(IRpcTransactionContext rpcMessageContext, IMessageSubject? result, Exception? exception)
        {
            if (setting.Disable)
            {
                return;
            }
            // RestoreAll(rpcContext);
            StopAsyncSpans(rpcMessageContext, exception, TRACING_INVOKE_SPAN, TRACING_RPC_SPAN);
            LOGGER.LogInformation("OnAfterInvoke invoke end span {op}", rpcMessageContext.OperationName);
        }

        public void OnResume(IRpcMessageEnterContext rpcMessageContext)
        {
        }

        public void OnSuspend(IRpcMessageEnterContext rpcMessageContext)
        {
        }

        private string CreateOperationName(string action, IRpcTransactionContext context)
        {
            var operationName = context.OperationName;
            if (string.IsNullOrEmpty(operationName))
            {
                return RpcOperationName(context.MessageSubject);
            }
            return action + context.OperationName;
        }

        private string RpcOperationName(IMessageSchema message)
        {
            return $"tny://{instrumentConfig.ServiceName}/{message.ProtocolId}/{message.Mode.Mark()}";
        }

        private string RemoteOperationName(IRpcTransactionContext context)
        {
            return CreateOperationName("remote:", context);
        }

        private string LocalOperationName(IRpcTransactionContext rpcMessageContext)
        {
            return CreateOperationName("local:", rpcMessageContext);
        }

        private static string GetContactName(IContact contact)
        {
            return $"{contact.ContactType.Group}[{contact.ContactId}]";
        }

        private void TagSpanService(SegmentContext segmentContext, IConnector contact, IMessageSubject message)
        {
            TagSpanCommon(segmentContext, contact.AccessMode, message);
            TagSpanContact(segmentContext, CONTACT, contact);
        }

        private void TagSpanRemote(SegmentContext segmentContext, IConnector contact, IMessageSubject message)
        {
            TagSpanCommon(segmentContext, NetAccessMode.Client, message);
            TagSpanContact(segmentContext, TARGET, contact);
            TagSpanArguments(segmentContext, message);
            TagSpanForward(segmentContext, message);
        }

        private void TagSpanLocal(SegmentContext span, IContact connector, IMessageSubject message)
        {
            TagSpanCommon(span, NetAccessMode.Server, message);
            TagSpanArguments(span, message);
            TagSpanForward(span, message);
            TagSpanContact(span, CONTACT, connector);
        }

        private void TagSpanForward(SegmentContext segmentContext, IMessageSubject message)
        {
            var header = message.GetHeader(MessageHeaderKeys.RPC_FORWARD_HEADER);
            var forward = header?.To;
            if (forward == null)
            {
                return;
            }
            TagSpanContact(segmentContext, FORWARD, forward);
        }

        private void TagSpanArguments(SegmentContext span, IMessageSubject message)
        {
            if (setting.EnableCollectArguments)
            {
                CollectArguments(span, message, setting.CollectArgumentsMaxLength);
            }
        }

        private void CollectArguments(SegmentContext segmentContext, IMessageSubject message, int argumentsLengthThreshold)
        {
            var body = message.Body;
            if (message.Mode != MessageMode.Request || body is not ICollection parameters)
                return;
            var stringBuilder = new StringBuilder();
            var first = true;
            foreach (var parameter in parameters)
            {
                if (!first)
                {
                    stringBuilder.Append(',');
                }
                stringBuilder.Append(parameter);
                if (stringBuilder.Length > argumentsLengthThreshold)
                {
                    stringBuilder.Append("...");
                    break;
                }
                first = false;
            }
            segmentContext.Span.AddTag(ARGUMENTS, stringBuilder.ToString());
        }

        private void TagSpanContact(SegmentContext segmentContext, string key, IContact? contact)
        {
            if (contact == null)
            {
                return;
            }
            var span = segmentContext.Span;
            span.AddTag(key, GetContactName(contact));
        }

        private void TagSpanCommon(SegmentContext segmentContext, NetAccessMode accessMode, IMessageSchema message)
        {
            var span = segmentContext.Span;
            span.Component = accessMode == NetAccessMode.Server ? TNY_RPC_SERVER : TNY_RPC_CLIENT;
            span.SpanLayer = SpanLayer.RPC_FRAMEWORK;

            var mode = message.Mode.ToString();
            var protocolId = message.ProtocolId.ToString();
            span.AddTag(RPC_MODE, mode);
            span.AddTag(RPC_PROTOCOL, protocolId);
            span.AddTag(TRACE_ID, segmentContext.TraceId);
            span.AddTag(SEGMENT_ID, segmentContext.SegmentId);
            span.AddTag(SPAN_ID, span.SpanId);
        }

        private ICarrierHeaderCollection LoadCarrier(IMessageSubject message)
        {
            var headers = new Dictionary<string, string>();
            var header = message.GetHeader(MessageHeaderKeys.RPC_TRACING_HEADER);
            if (header == null)
                return new TextCarrierHeaderCollection(headers);
            LOGGER.LogInformation("LoadCarrier {protocol} {mode} | {header}", message.ProtocolId, message.Mode, header);
            headers.AddRang(header.Attributes);
            return new TextCarrierHeaderCollection(headers);
        }

        private void RestoreAll(IRpcTransactionContext rpcMessageContext)
        {
            if (rpcMessageContext is not {Valid: true})
            {
                return;
            }
            Restore(rpcMessageContext, rpcMessageContext.Attributes.Get(TRACING_INVOKE_SPAN));
            Restore(rpcMessageContext, rpcMessageContext.Attributes.Get(TRACING_RPC_SPAN));
        }

        private void Restore(IRpcTransactionContext rpcMessageContext, SegmentContext segmentContext)
        {
            if (segmentContext.IsNull())
            {
                return;
            }
            var span = segmentContext.Span;
            switch (span.SpanType)
            {
                case SpanType.Entry:
                    enterSegmentContextAccessor.Context = segmentContext;
                    break;
                case SpanType.Exit:
                    exitSegmentContextAccessor.Context = segmentContext;
                    break;
                // case SpanType.Local:
                //     localSegmentContextAccessor.Context = segmentContext;
                // break;
                default:
                    return;
            }
            LOGGER.LogInformation("restore span {mode} {name} {span}", rpcMessageContext.Mode, span.OperationName, Debug(segmentContext));
        }

        private string Peer(IAddressPeer? addressPeer)
        {
            if (addressPeer == null)
            {
                return "NA:NA";
            }
            var address = addressPeer.RemoteAddress;
            return address?.ToString() ?? "NA:NA";
        }

        private void StopAsyncSpans(IRpcTransactionContext rpcMessageContext, Exception? cause, params AttrKey<SegmentContext>[] keys)
        {
            foreach (var key in keys)
            {
                StopAsyncSpan(rpcMessageContext, cause, key);
            }
        }

        private void StopAsyncSpan(IRpcTransactionContext rpcMessageContext, Exception? cause, AttrKey<SegmentContext> key)
        {
            var segmentContext = rpcMessageContext.Attributes.Remove(key);
            if (segmentContext.IsNull())
                return;
            var span = segmentContext.Span;
            if (cause != null)
            {
                span.ErrorOccurred(cause, tracingConfig);
            }

            LOGGER.LogInformation("OnAfterInvoke - StopAsyncSpan {mode} Stop {key} span {op} {span}", rpcMessageContext.Mode, key.Name,
                span.OperationName,
                Debug(segmentContext));
            tracingContext.Release(segmentContext);
        }
    }

}
