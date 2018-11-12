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

namespace EPC.Controllers
{
    public class HomeController : Controller
    {
        private static readonly HttpClient client = new HttpClient();

        public IActionResult Index()
        {
            return View();
        }

        [Route("get")]
        public string PrintTemperature()
        {
            return GetTemperature(0);
        }

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
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://192.168.1.126:6000/api/epc/temperature");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            string json = "{\"ListOfSensors\": [";
            int amountOfBoxes = 2;
            for (int i = 0; i < amountOfBoxes; i++)
            {
                json += "{\"BoxNo\": " + i + ", \"Value\": \"" + GetTemperature(i) + "\", \"SensorType\": \"Temperature\"}";
                if (i < amountOfBoxes - 1)
                {
                    json += ",";
                }
            }
            json += "]}";

            //return json;

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                return result;
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
