using System.Diagnostics;
using System.IO;
using System.Net;
using EPC.Http;
using EPC.Model;
using EPC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EPC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        //this route "/get" just returns the temperature of the 192.168.1.150 temperature sensor for debugging
        [Route("get")]
        public string PrintTemperature()
        {
            return GetTemperature(0);
        }

        //This method takes the temperature of the box you ask for and return it as a string
        //this is used before the post request the the API and this is called for every box
        public string GetTemperature(int boxNo)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://192.168.1." + (150 + boxNo));
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                string json = reader.ReadToEnd();
                TemperatureSensorModel temp = JsonConvert.DeserializeObject<TemperatureSensorModel>(json);
                return temp.Temperature.ToString();
            }
        }

        [Route("post")]
        public void PostTemperaturesAsync()
        {
            string json = "{\"ListOfSensors\": [";
            int amountOfBoxes = 2;
            for (int i = 1; i < amountOfBoxes + 1; i++)
            {
                json += "{\"BoxNo\": " + i + ", \"Value\": \"" + GetTemperature(i - 1) + "\", \"SensorType\": \"Temperature\"}";
                if (i < amountOfBoxes)
                {
                    json += ",";
                }
            }
            json += "]}";

            new HttpHandler().HttpPostRequest(json, "api/epc/temperature");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
