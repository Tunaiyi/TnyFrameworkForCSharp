// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Codec;
using TnyFramework.Common.Util;

namespace TnyFramework.Namespace.Etcd;

public abstract class EtcdHashing<TValue>
{
    internal INamespaceExplorer Explorer { get; }

    public ObjectMimeType<TValue> MineType { get; }

    public string Path { get; }

    public long MaxSlots { get; }

    protected EtcdHashing(string path, long maxSlots, ObjectMimeType<TValue> mineType, INamespaceExplorer explorer)
    {
        Explorer = explorer;
        MaxSlots = maxSlots;
        Path = path;
        MineType = mineType;
    }

    internal string SubPath(long slot)
    {
        return NamespacePathNames.NodePath(Path, SlotName(slot));
    }

    internal string SlotName(long hashCode)
    {
        if (hashCode >= MaxSlots)
        {
            hashCode %= MaxSlots;
        }
        return NumberFormatAide.AlignDigits(hashCode, MaxSlots);
    }
}
