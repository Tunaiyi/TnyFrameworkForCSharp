// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using FreeRedis;

namespace TnyFramework.FreeRedis.Hosting.Client;

public static class CommandPacketExtensions
{
    public static CommandPacket SubCommand(this string cmd, string subcmd) => new CommandPacket(cmd, subcmd);

    public static CommandPacket InputKey(this string cmd, string key) => new CommandPacket(cmd).InputKey(key);

    public static CommandPacket InputKey(this string cmd, string key, string arg1) => new CommandPacket(cmd).InputKey(key).InputRaw(arg1);

    public static CommandPacket InputKey(this string cmd, string key, string arg1, long arg2) =>
        new CommandPacket(cmd).InputKey(key).InputRaw(arg1).InputRaw(arg2);

    public static CommandPacket InputKey(this string cmd, string key, string arg1, decimal arg2) =>
        new CommandPacket(cmd).InputKey(key).InputRaw(arg1).InputRaw(arg2);

    public static CommandPacket InputKey(this string cmd, string key, string arg1, string arg2) =>
        new CommandPacket(cmd).InputKey(key).InputRaw(arg1).InputRaw(arg2);

    public static CommandPacket InputKey(this string cmd, string key, decimal arg1, string arg2) =>
        new CommandPacket(cmd).InputKey(key).InputRaw(arg1).InputRaw(arg2);

    public static CommandPacket InputKey(this string cmd, string key, string[] arg1) => new CommandPacket(cmd).InputKey(key).Input(arg1);

    public static CommandPacket InputKey(this string cmd, string key, long arg1) => new CommandPacket(cmd).InputKey(key).InputRaw(arg1);

    public static CommandPacket InputKey(this string cmd, string key, long arg1, long arg2) =>
        new CommandPacket(cmd).InputKey(key).InputRaw(arg1).InputRaw(arg2);

    public static CommandPacket InputKey(this string cmd, string key, int arg1) => new CommandPacket(cmd).InputKey(key).InputRaw(arg1);

    public static CommandPacket InputKey(this string cmd, string key, decimal arg1) => new CommandPacket(cmd).InputKey(key).InputRaw(arg1);

    public static CommandPacket InputKey(this string cmd, string key, decimal arg1, decimal arg2) =>
        new CommandPacket(cmd).InputKey(key).InputRaw(arg1).InputRaw(arg2);

    public static CommandPacket InputKey(this string cmd, string[] keys) => new CommandPacket(cmd).InputKey(keys);

    public static CommandPacket InputKey(this string cmd, string[] keys, int arg1) => new CommandPacket(cmd).InputKey(keys).InputRaw(arg1);

    public static CommandPacket InputRaw(this string cmd, object arg) => new CommandPacket(cmd).InputRaw(arg);

    public static CommandPacket Input(this string cmd, string arg) => new CommandPacket(cmd).InputRaw(arg);

    public static CommandPacket Input(this string cmd, long arg) => new CommandPacket(cmd).InputRaw(arg);

    public static CommandPacket Input(this string cmd, string arg1, string arg2) => new CommandPacket(cmd).InputRaw(arg1).InputRaw(arg2);

    public static CommandPacket Input(this string cmd, string arg1, int arg2) => new CommandPacket(cmd).InputRaw(arg1).InputRaw(arg2);

    public static CommandPacket Input(this string cmd, int arg1, int arg2) => new CommandPacket(cmd).InputRaw(arg1).InputRaw(arg2);

    public static CommandPacket Input(this string cmd, long arg1, long arg2) => new CommandPacket(cmd).InputRaw(arg1).InputRaw(arg2);

    public static CommandPacket Input(this string cmd, string arg1, string arg2, string arg3) =>
        new CommandPacket(cmd).InputRaw(arg1).InputRaw(arg2).InputRaw(arg3);

    public static CommandPacket Input(this string cmd, string[] args) => new CommandPacket(cmd).Input(args);
}
