using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Reso.Sdk.Core.Middlewares;

namespace Reso.Sdk.Core.Utilities
{
    public static class LogUtils
    {
        /// <summary>
        /// Send Log to ResoLog with specific store id
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="exception"></param>
        public static async Task SendLog(int storeId, string exception)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://log.reso.vn");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var url = $"api/v1/Logs";
                    var obj = JsonConvert.SerializeObject(new
                    {
                        store_id = storeId,
                        content = exception.ToString(),
                        project_name = Assembly.GetEntryAssembly()?.GetName().Name
                    }, new JsonSerializerSettings
                    {
                        ContractResolver = new DefaultContractResolver
                        {
                            NamingStrategy = new SnakeCaseNamingStrategy { ProcessDictionaryKeys = true }
                        },
                        Formatting = Formatting.Indented
                    });
                    HttpContent httpContent = new StringContent(obj, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(url, httpContent);
                }
            }
            catch (Exception ex)
            {
                //ignore
            }
        }
        
        /// <summary>
        /// Send log to ResoLog with optional store id, default storeId=136
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="storeId"></param>
        public static async Task SendLog(string exception, int storeId = 136)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://log.reso.vn");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var url = $"api/v1/Logs";
                    var obj = JsonConvert.SerializeObject(new
                    {
                        store_id = storeId,
                        content = exception.ToString(),
                        projectName = Assembly.GetEntryAssembly()?.GetName().Name
                    }, new JsonSerializerSettings
                    {
                        ContractResolver = new DefaultContractResolver
                        {
                            NamingStrategy = new SnakeCaseNamingStrategy { ProcessDictionaryKeys = true }
                        },
                        Formatting = Formatting.Indented
                    });
                    HttpContent httpContent = new StringContent(obj, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(url, httpContent);
                }
            }
            catch (Exception ex)
            {
                //ignore
            }
        }
    }
}