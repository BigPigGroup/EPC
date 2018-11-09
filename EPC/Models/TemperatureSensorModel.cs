using System.Runtime.Serialization;

namespace EPC.Model
{

    [DataContract]
    public class TemperatureSensorModel
    {
        [DataMember(Name = "Temperature")]
        public float Temperature { get; set; }

        public TemperatureSensorModel(float temperature = 0)
        {
            Temperature = temperature;
        }
    }
}