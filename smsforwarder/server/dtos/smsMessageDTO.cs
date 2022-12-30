using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace smsforwarder.server.dtos
{
    internal class smsMessageDTO
    {
        [JsonProperty]
        public int id { get; set; }
        [JsonProperty]
        public string drop_phone_number { get; set; }
        [JsonProperty]
        public string service_phone_number { get; set; }
        [JsonProperty]
        public string service_name { get; set; }
        [JsonProperty]
        public string reg_time { get; set; }
        [JsonProperty]
        public string sms_text { get; set; }
        [JsonProperty]
        public int status_id { get; set; }

    }
}
