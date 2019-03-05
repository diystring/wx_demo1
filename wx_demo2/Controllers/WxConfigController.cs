using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Deepleo.Weixin.SDK.Helpers;
using Deepleo.Weixin.SDK.JSSDK;
using NewLife.Caching;
using NewLife.Log;
using Deepleo.Weixin.SDK;
namespace wx_demo2.Controllers
{
    public class WxConfigController : ApiController
    {
        // GET: api/WxConfig
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/WxConfig/5
        public WxConfig Get(int id)
        {
            string _appid = "wx6aae9a42791c0cdf";
            string _appsecret = "71201c6d9ca47d1375ca2de8dbdf1c93";
            long _timestamp = Util.CreateTimestamp();
            string _nonceStr = Util.CreateNonce_str();
            string _signature = "";
            string string1 = "";
            var ic = Redis.Create("127.0.0.1",7);
            ic.Log = XTrace.Log;

            var access_token = ic.Get<string>("access_token");
            var jsapi_ticket = ic.Get<string>("jsapi_ticket");
            

            if (access_token != null&& jsapi_ticket!=null)
            {
                _signature = JSAPI.GetSignature(jsapi_ticket, _nonceStr, _timestamp, "", out string1);
            }
            else {

                var access_token_json = BasicAPI.GetAccessToken(_appid, _appsecret);

                ic.Set("access_token", access_token_json.access_token,7200);

                var jsapi_ticket_json= JSAPI.GetTickect(access_token_json.access_token);

                ic.Set("jsapi_ticket", jsapi_ticket_json.ticket,7200);

                _signature = JSAPI.GetSignature(jsapi_ticket_json.ticket, _nonceStr, _timestamp, "", out string1);
            }

            return new WxConfig
            {
                appId = _appid,
                timestamp = _timestamp,
                nonceStr = _nonceStr,
                signature = _signature
            };

        }
    }

    public class WxConfig
    {
        public string appId { get; set; }
        public long timestamp { get; set; }
        public string nonceStr { get; set; }
        public string signature { get; set; }
    }
}
