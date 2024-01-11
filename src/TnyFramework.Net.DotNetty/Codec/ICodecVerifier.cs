// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace TnyFramework.Net.DotNetty.Codec;

/// <summary>
/// 编码校验器
/// </summary>
public interface ICodecVerifier
{
    /// <summary>
    /// 校验码长度(字节)
    /// </summary>
    /// <returns>长度</returns>
    int CodeLength { get; }

    /// <summary>
    /// 生成校验码
    /// </summary>
    /// <param name="packager">包上下文</param>
    /// <param name="body">字节</param>
    /// <param name="offset">开始位置</param>
    /// <param name="length">长度</param>
    /// <returns>返回校验码</returns>
    byte[] Generate(DataPackageContext packager, byte[] body, int offset, int length);

    /// <summary>
    /// 校验校验码
    /// </summary>
    /// <param name="packager">包上下文</param>
    /// <param name="body">字节</param>
    /// <param name="offset">开始位置</param>
    /// <param name="length">长度</param>
    /// <param name="verifyCode">校验的校验码</param>
    /// <returns>结果, 成功返回 true, 否则返回 false</returns>
    bool Verify(DataPackageContext packager, byte[] body, int offset, int length, byte[] verifyCode);
}

public class NoopCodecVerifier : ICodecVerifier
{
    public int CodeLength => 0;

    public byte[] Generate(DataPackageContext packager, byte[] body, int offset, int length)
    {
        return body;
    }

    public bool Verify(DataPackageContext packager, byte[] body, int offset, int length, byte[] verifyCode)
    {
        return true;
    }
}
