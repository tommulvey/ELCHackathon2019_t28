using System;
using System.Collections.Generic;
using System.Text;

namespace fakedata
{
    public class tweet_data
    {
        //[JsonProperty(PropertyName = "id")]
        public string id { get; set; }
        public string body { get; set; }
        public DateTime date { get; set; }
        public string likes { get; set; }
        public string rts { get; set; }
        public string link { get; set; }
        public string name { get; set; }
        public string profile { get; set; }
    }
}
