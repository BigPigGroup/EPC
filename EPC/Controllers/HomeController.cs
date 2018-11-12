using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EPC.Models;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Runtime.Serialization.Json;
using EPC.Model;
using System.Net.Http;
using System.Text;

namespace EPC.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _baseUrl = "http://localhost:6000/";
        private WebClient client = new WebClient();

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
        public async Task<string> PostTemperaturesAsync()
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

            //return json;

            string result = "";
            byte[] response;
            var content = Encoding.Default.GetBytes(json);

            lock (client)
            {
                response = client.UploadData(_baseUrl + "/api/epc/temperature", "POST", content);
            }

            result = Encoding.Default.GetString(response);
            Console.WriteLine(result);

            return result;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
