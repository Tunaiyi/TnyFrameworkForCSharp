// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using FreeRedis;

namespace TnyFramework.FreeRedis.Hosting.Configurations;

public class FreeRedisParamSetting
{
    private RedisProtocol? protocol;
    private string user = "";
    private string password = "";
    private int? database;
    private string prefix = "";
    private string clientName = "";
    private string encoding = "";
    private long? idleTimeout;
    private long? connectTimeout;
    private long? receiveTimeout;
    private long? sendTimeout;
    private int? maxPoolSize;
    private int? minPoolSize;
    private int? retry;

    public bool Update { get; private set; }

    public RedisProtocol? Protocol {
        get => protocol;
        set {
            protocol = value;
            Update = true;
        }
    }

    public string User {
        get => user;
        set {
            user = value;
            Update = true;
        }
    }

    public string Password {
        get => password;
        set {
            password = value;
            Update = true;
        }
    }

    public int? Database {
        get => database;
        set {
            database = value;
            Update = true;
        }
    }

    public string Prefix {
        get => prefix;
        set {
            prefix = value;
            Update = true;
        }
    }

    public string ClientName {
        get => clientName;
        set {
            clientName = value;
            Update = true;
        }
    }

    public string Encoding {
        get => encoding;
        set {
            encoding = value;
            Update = true;
        }
    }

    public long? IdleTimeout {
        get => idleTimeout;
        set {
            idleTimeout = value;
            Update = true;
        }
    }

    public long? ConnectTimeout {
        get => connectTimeout;
        set {
            connectTimeout = value;
            Update = true;
        }
    }

    public long? ReceiveTimeout {
        get => receiveTimeout;
        set {
            receiveTimeout = value;
            Update = true;
        }
    }

    public long? SendTimeout {
        get => sendTimeout;
        set {
            sendTimeout = value;
            Update = true;
        }
    }

    public int? MaxPoolSize {
        get => maxPoolSize;
        set {
            maxPoolSize = value;
            Update = true;
        }
    }

    public int? MinPoolSize {
        get => minPoolSize;
        set {
            minPoolSize = value;
            Update = true;
        }
    }

    public int? Retry {
        get => retry;
        set {
            retry = value;
            Update = true;
        }
    }
}
