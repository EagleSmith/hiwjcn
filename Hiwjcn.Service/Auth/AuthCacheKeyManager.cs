﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib.cache;

namespace Hiwjcn.Bll.Auth
{
    /// <summary>
    /// auth缓存key管理
    /// </summary>
    public static class AuthCacheKeyManager
    {
        public static string TokenKey(string token_uid) => $"auth.token.uid={token_uid}".WithCacheKeyPrefix();

        public static string ClientKey(string client_uid) => $"auth.client.uid={client_uid}".WithCacheKeyPrefix();

        public static string ScopeKey(string scope_uid) => $"auth.scope.uid={scope_uid}".WithCacheKeyPrefix();

        public static string ScopeAllKey() => "auth.scope.all".WithCacheKeyPrefix();

        public static string UserInfoKey(string user_uid) => $"auth.user.uid={user_uid}".WithCacheKeyPrefix();
    }
}
