using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//SensorTag
//9Q14vDW1ZKjRW/epnJorGGtv3lZf0En9Wu2A13fybok=
namespace SensorTagWindows.Code
{
    public class IoTMessage
    {
        [JsonProperty("humidity")]
        public string humidity;

        [JsonProperty("pressure")]
        public string pressure;

        [JsonProperty("temperature")]
        public string temperature;

        [JsonProperty("time")]
        public string time;
    }
}
