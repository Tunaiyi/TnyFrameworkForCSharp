using System;
namespace TnyFramework.Net.Dispatcher
{
    /// <summary>
    /// 开始执行
    /// </summary>
    /// <param name="command">command </param>
    public delegate void CommandExecute(MessageCommand command);


    /// <summary>
    /// 执行Command任务完成
    /// </summary>
    /// <param name="command">command</param>
    /// <param name="cause">异常</param>
    public delegate void CommandDone(MessageCommand command, Exception cause);

    // public interface IMessageCommandListener
    // {
    //     /// <summary>
    //     /// 开始执行
    //     /// </summary>
    //     /// <param name="command">command </param>
    //     void OnExecute(MessageCommand command);
    //
    //
    //     /// <summary>
    //     /// 执行Command任务完成
    //     /// </summary>
    //     /// <param name="command">command</param>
    //     /// <param name="cause">异常</param>
    //     void OnDone(MessageCommand command, Exception cause);
    // }
}
