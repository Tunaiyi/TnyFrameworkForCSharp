// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Collections.Generic;
using System.Linq;

namespace TnyFramework.Net.Clusters;

public class RemoteServeClusterSetting : IRemoteServeClusterSetting
{
    private List<RemoteServeNodeSetting> services = [];

    public List<RemoteServeNodeSetting> Services {
        get => services;
        set => services = value;
    }

    List<IRemoteServeNodeSetting> IRemoteServeClusterSetting.Services => services.Select(s => s as IRemoteServeNodeSetting).ToList();
}
