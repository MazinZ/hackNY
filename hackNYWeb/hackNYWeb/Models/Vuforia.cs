using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace hackNYWeb.Models
{
    class Vuforia
    {
        private string accessKey = "180a785dae6c17f46ad1e79801a616fe6b8dbdc0";
        private string secretKey = "c2474730fd69a65e6c00d442d3d7a8ab15841a89";

        private const string url = "https://vws.vuforia.com";
        private const string requestPath = "/targets";
        private const string contentType = "application/json";
        private string httpAction;
        private string targetName;
        private string imageLocation;

        public Vuforia(string targetName, string imageLocation)
        {
            if (File.Exists(imageLocation))
            {
                this.targetName = targetName;
                this.imageLocation = imageLocation;
            }
            else
            {
                throw new Exception("Image cannot be found.");
            }
        }

        public string postTarget()
        {
            try
            {
                WebResponse response;
                string serviceURI = url + requestPath;
                string requestBody = setUpRequestBody();
                httpAction = "POST";
                string date = string.Format("{0:r}", DateTime.Now.ToUniversalTime());


                HttpWebRequest httpWReq = (HttpWebRequest)HttpWebRequest.Create(serviceURI);
                httpWReq.Method = httpAction;
                MethodInfo priMethod = httpWReq.Headers.GetType().GetMethod("AddWithoutValidate", BindingFlags.Instance | BindingFlags.NonPublic);
                priMethod.Invoke(httpWReq.Headers, new[] { "Date", date });
                httpWReq.ContentType = contentType;

                MD5 md5 = MD5.Create();
                var contentMD5bytes = md5.ComputeHash(System.Text.Encoding.ASCII.GetBytes(requestBody));
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                for (int i = 0; i < contentMD5bytes.Length; i++)
                {
                    sb.Append(contentMD5bytes[i].ToString("x2"));
                }

                string contentMD5 = sb.ToString();

                string stringToSign = string.Format("{0}\n{1}\n{2}\n{3}\n{4}", httpAction, contentMD5, contentType, date, requestPath);

                httpWReq.Headers.Add("Authorization", string.Format("VWS {0}:{1}", accessKey, buildSignature(stringToSign)));

                var streamWriter = httpWReq.GetRequestStream();
                byte[] buffer = System.Text.Encoding.ASCII.GetBytes(requestBody);
                streamWriter.Write(buffer, 0, buffer.Length);
                streamWriter.Flush();
                streamWriter.Close();

                response = httpWReq.GetResponse();

                Stream receiveStream = response.GetResponseStream();
                StreamReader sr = new StreamReader(receiveStream, System.Text.Encoding.UTF8);
                string responseData = sr.ReadToEnd();
                response.Close();
                sr.Close();
                Console.WriteLine(responseData);
                return responseData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string buildSignature(string stringToSign)
        {
            HMACSHA1 sha1 = new HMACSHA1(System.Text.Encoding.ASCII.GetBytes(secretKey));
            byte[] sha1Bytes = Encoding.ASCII.GetBytes(stringToSign);
            MemoryStream stream = new MemoryStream(sha1Bytes);
            byte[] sha1Hash = sha1.ComputeHash(stream);
            string signature = System.Convert.ToBase64String(sha1Hash);
            return signature;
        }

        public string getTargetData(string targetID)
        {
            try
            {
                WebResponse response;
                string serviceURI = url + requestPath + "/" + targetID;
                string requestBody = "";
                httpAction = "GET";

                string date = string.Format("{0:r}", DateTime.Now.ToUniversalTime());
                MD5 md5 = MD5.Create();
                var contentMD5bytes = md5.ComputeHash(System.Text.Encoding.ASCII.GetBytes(requestBody));
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                for (int i = 0; i < contentMD5bytes.Length; i++)
                {
                    sb.Append(contentMD5bytes[i].ToString("x2"));
                }

                string contentMD5 = sb.ToString();
                string stringToSign = string.Format("{0}\n{1}\n{2}\n{3}\n{4}", httpAction, contentMD5, "", date, requestPath + "/" + targetID);

                HttpWebRequest httpWReq = (HttpWebRequest)HttpWebRequest.Create(serviceURI);
                MethodInfo priMethod = httpWReq.Headers.GetType().GetMethod("AddWithoutValidate", BindingFlags.Instance | BindingFlags.NonPublic);
                priMethod.Invoke(httpWReq.Headers, new[] { "Date", date });
                httpWReq.Method = httpAction;
                httpWReq.Headers.Add("Authorization", string.Format("VWS {0}:{1}", accessKey, buildSignature(stringToSign)));

                response = httpWReq.GetResponse();

                Stream receiveStream = response.GetResponseStream();
                StreamReader sr = new StreamReader(receiveStream, System.Text.Encoding.UTF8);
                string responseData = sr.ReadToEnd();
                response.Close();
                sr.Close();
                Console.WriteLine(responseData);
                return responseData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getTargetStatus(string targetID)
        {
            string response;
            string status = "";
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            while (status.ToLower() != "success")
            {
                response = getTargetData(targetID);
                int milliseconds = 10000;
                Thread.Sleep(milliseconds);
                dynamic r = JsonConvert.DeserializeObject(response);

                status = r.status;
            }
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            Console.WriteLine(elapsedTime);
            Console.Read();
        }

        private string setUpRequestBody()
        {
            double width;
            string image;
            //get image string
            byte[] imageBytes = File.ReadAllBytes(imageLocation);
            image = Convert.ToBase64String(imageBytes);
            //get width
            Image img = Image.FromFile(imageLocation);
            width = img.Width;
            //return the json
            RequestBody requestBody = new RequestBody(targetName, width, image);
            return requestBody.ToString();
        }
    }
    struct RequestBody
    {
        private string name;
        private double width;
        private string image;

        public RequestBody(string name, double width, string image)
        {
            this.name = name;
            this.width = width;
            this.image = image;
        }

        public override string ToString()
        {
            return "{" + string.Format("\"name\" : \"{0}\", \"width\" : {1}, \"image\" : \"{2}\"", name, width, image) + "}";
        }
    }
}
