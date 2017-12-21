using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.ThirdParty.Models.WeSure;

namespace Tuhu.Service.ThirdParty.Server.Util
{
    class HttpHelper
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(HttpHelper));
        public static async Task<string> Post(string url, string parameters, int timeout = 5000)
        {
            string result = string.Empty;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);
                    var content = new StringContent(parameters, Encoding.UTF8, "application/json");
                    var postResult = await client.PostAsync(url, content);
                    result = await postResult.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Post请求出错，url：" + url + ",parameters:" + parameters + "", ex);
                throw ex;
            }

            return result;
        }
    }
}
