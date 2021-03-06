﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Lib.ioc;
using Lib.net;
using Lib.extension;
using Lib.helper;
using Lib.mvc.user;

namespace Lib.mvc.auth
{
    public class AuthServerApiCaller
    {
        private static readonly HttpClient client = HttpClientManager.Instance.DefaultClient;

        private readonly AuthServerConfig _server;
        public AuthServerApiCaller(AuthServerConfig _server)
        {
            this._server = _server;
        }

        public async Task<_<string>> CreateAuthCodeByPassword(string client_id, List<string> scope, string username, string password)
        {
            var data = new _<string>();
            var response = await client.PostAsJsonAsync(this._server.CreateAuthCodeByPassword(), new
            {
                client_id = client_id,
                scope = scope?.ToJson(),
                username = username,
                password = password
            });
            using (response)
            {
                var json = await response.Content.ReadAsStringAsync();
                var code = json.JsonToEntity<_<string>>();
                if (!code.success || !ValidateHelper.IsPlumpString(code.data))
                {
                    data.SetErrorMsg(code.msg);
                    return data;
                }
                data.SetSuccessData(code.data);
            }
            return data;
        }

        public async Task<_<string>> CreateAuthCodeByOneTimeCode(string client_id, List<string> scope, string phone, string sms)
        {
            var data = new _<string>();
            var response = await client.PostAsJsonAsync(this._server.CreateCodeByOneTimeCode(), new
            {
                client_id = client_id,
                scope = scope?.ToJson(),
                phone = phone,
                sms = sms
            });
            using (response)
            {
                var json = await response.Content.ReadAsStringAsync();
                var code = json.JsonToEntity<_<string>>();
                if (!code.success || !ValidateHelper.IsPlumpString(code.data))
                {
                    data.SetErrorMsg(code.msg);
                    return data;
                }
                data.SetSuccessData(code.data);
            }
            return data;
        }

        public async Task<_<TokenModel>> AccessToken(string client_id, string client_secret, string code, string grant_type)
        {
            var data = new _<TokenModel>();
            var response = await client.PostAsJsonAsync(this._server.CreateToken(), new
            {
                client_id = client_id,
                client_secret = client_secret,
                code = code,
                grant_type = grant_type
            });
            using (response)
            {
                var json = await response.Content.ReadAsStringAsync();
                var token = json.JsonToEntity<_<TokenModel>>();
                if (!token.success)
                {
                    data.SetErrorMsg(token.msg);
                    return data;
                }
                data.SetSuccessData(token.data);
            }
            return data;
        }

        public async Task<_<LoginUserInfo>> CheckToken(string client_id, string access_token)
        {
            var data = new _<LoginUserInfo>();
            var response = await client.PostAsJsonAsync(this._server.CheckToken(), new
            {
                client_id = client_id,
                access_token = access_token
            });
            using (response)
            {
                var json = await response.Content.ReadAsStringAsync();
                var loginuser = json.JsonToEntity<_<LoginUserInfo>>();
                if (!loginuser.success)
                {
                    data.SetErrorMsg(loginuser.msg);
                    return data;
                }
                data.SetSuccessData(loginuser.data);
            }
            return data;
        }
    }
}
