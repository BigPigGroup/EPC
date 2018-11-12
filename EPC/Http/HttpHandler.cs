using System;
using System.Net;
using System.Text;

namespace EPC.Http
{

    public class HttpHandler
    {
        private readonly string _baseUrl = "http://localhost:6000/";
        private WebClient client = new WebClient();

        public HttpHandler()
        {
            // This fixes the error: error: (415) Unsupported Media Type.
            client.Headers["Content-Type"] = "application/json";
            client.BaseAddress = _baseUrl;
        }

        public string HttpPostRequest(string jsonString, string relativePath)
        {
            string result = "";
            byte[] response;
            var content = Encoding.Default.GetBytes(jsonString);

            lock (client)
            {
                response = client.UploadData(relativePath, "POST", content);
            }

            result = Encoding.Default.GetString(response);
            Console.WriteLine(result);

            return result;
        }
    }

}