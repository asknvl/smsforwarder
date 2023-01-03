using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using smsforwarder.server.dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace smsforwarder.server
{
    internal class ServerApi : IServerApi
    {
        #region const
        #region const
        string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6MSwicm9sZXMiOlt7ImlkIjoyfV0sImlhdCI6MTY3MjE4NDQ0NH0.ONCAZr_r8Cdu1cePZz4FRP75ytLrDGtul2qzgkoqnCc";
        #endregion
        #endregion

        #region vars
        string url;
        ServiceCollection serviceCollection;
        IHttpClientFactory httpClientFactory;
        #endregion

        public ServerApi(string url) {

            this.url = url;

            serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var services = serviceCollection.BuildServiceProvider();
            httpClientFactory = services.GetRequiredService<IHttpClientFactory>();
        }

        #region private
        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddHttpClient();
        }
        #endregion

        #region public
        public async Task<List<smsMessageDTO>> GetMessages()
        {
            List<smsMessageDTO> messages = new();
            var addr = $"{url}/v1/sms?sort_by=+id&status_id=[1,2]";
            var httpClient = httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await httpClient.GetAsync(addr);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();

                var json = JObject.Parse(result);
                bool res = json["success"].ToObject<bool>();
                if (res)
                {
                    var data = json["data"];
                    if (data != null)
                    {
                        messages = data.ToObject<List<smsMessageDTO>>();                       
                    }

                }
                else
                {
                    throw new ServerException($"success=false");
                }

            }
            catch (Exception ex)
            {
                throw new Exception($"GetMessages {ex.Message}");
            }
            
            return messages;
        }

        public async Task MarkMessageRead(int id)
        {
            var addr = $"{url}/v1/sms/{id}";
            var httpClient = httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await httpClient.PatchAsync(addr, null);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();

                var json = JObject.Parse(result);
                bool res = json["success"].ToObject<bool>();
                if (res)
                {              
                }
                else
                {
                    throw new ServerException($"success=false");
                }

            }
            catch (Exception ex)
            {
                throw new Exception($"MarkMessageRead {ex.Message}");
            }
        }
        #endregion
    }
}
