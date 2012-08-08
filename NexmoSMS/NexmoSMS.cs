using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Program
{
    public class NexmoSMS
    {
        const string NEXMOURL = "https://rest.nexmo.com/sms/json";

        public string Key { get; set; }
        public string Secret { get; set; }

        public NexmoSMS(string key, string secret)
        {
            Key = key;
            Secret = secret;
        }

        public NexmoResponse Send(string from, string to, string message)
        {
            NexmoResponse result;
            var request = WebRequest.Create(Uri.EscapeUriString(string.Format("{0}{1}", NEXMOURL, ParameterBuilder(from, to, message)))) as HttpWebRequest;
            request.Method = "GET";
            request.ContentType = "application/json";

            using (var response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new InvalidOperationException(string.Format("Request failed : {0}", response.StatusCode));
                }

                var json = new StreamReader(response.GetResponseStream()).ReadToEnd();
                using (var reader = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    var js = new DataContractJsonSerializer(typeof (NexmoResponse));
                    result = js.ReadObject(reader) as NexmoResponse;
                }
            }
            return result;
        }

        private string ParameterBuilder(string from, string to, string message)
        {
            return string.Format("?username={0}&password={1}&from={2}&to={3}&text={4}", Key, Secret, from, to, message);
        }

    }

    [DataContract]
    public class NexmoResponse
    {
        [DataMember(Name = "message-count", Order = 0)]
        public int Count { get; set; }

        [DataMember(Name = "messages", Order = 1)]
        public List<NexmoMessage> Messages { get; set; } 
    }

    [DataContract]
    public class NexmoMessage
    {
        [DataMember(Name = "status", Order = 0)]
        public int Status { get; set; }

        [DataMember(Name = "message-id", Order = 1)]
        public string MessageId { get; set; }

        [DataMember(Name = "to", Order = 2)]
        public string To { get; set; }

        [DataMember(Name = "message-price",IsRequired = false, Order = 3)]
        public decimal MessagePrice { get; set; }

        [DataMember(Name = "remaining-balance",IsRequired = false, Order = 4)]
        public decimal RemainingBalance { get; set; }

        [DataMember(Name = "client-ref", IsRequired = false, Order = 5)]
        public string ClientRef { get; set; }

        [DataMember(Name = "error-text", IsRequired = false, Order = 6)]
        public string ErrorText { get; set; }
    }
}
