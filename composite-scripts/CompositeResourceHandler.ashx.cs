using System.Web;
using System.Web.Services;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Configuration;
using System.Net;
using System;

namespace CompositeScripts
{
    public class CompositeResourceHandler : System.Web.IHttpHandler
    {
        private const bool DO_GZIP = true;

        private readonly TimeSpan CACHE_DURATION = TimeSpan.FromHours(24);

        public void ProcessRequest(HttpContext context)
        {
            HttpRequest request = context.Request;
            string[] fileNames = null;

            string fileNameString = request["f"].ToString();
            string contentType = request["t"].ToString();

            bool isCompressed = DO_GZIP & this.CanGZip(request);

            UTF8Encoding encoding = new UTF8Encoding(false);

            fileNames = fileNameString.Split(Convert.ToChar(","));

            if (fileNames != null && fileNames.Length > 0)
            {            
                MemoryStream memoryStream = new MemoryStream();

                {
                    StreamWriter writer = default(StreamWriter);
                    if (isCompressed)
                    {
                        writer = new StreamWriter(new GZipStream(memoryStream, CompressionMode.Compress), encoding);
                    }
                    else
                    {
                        writer = new StreamWriter(memoryStream, encoding);
                    }

                    foreach (string fileName in fileNames)
                    {
                        string fileContents = this.GetFileString(context, fileName.Replace("~", string.Empty).Trim(), encoding);
                        writer.WriteLine(fileContents);
                    }
                    writer.Close();                

                    byte[] responseBytes = memoryStream.ToArray();
              
                    this.WriteBytes(responseBytes, context, isCompressed, contentType);
                }            
            }
        }

        private string[] GetAllFiles(string keys, string names)
        {
            return names.Split(Convert.ToChar(","));
        }

        private string GetFileString(HttpContext context, string virtualPath, Encoding encoding)
        {
            var client = new WebClient();

            if (virtualPath.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) | virtualPath.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
            {
                return client.DownloadString(virtualPath);
            }       
            else
            {
                string physicalPath = GetASPXSiteUrl(context.Request) + virtualPath;
                return client.DownloadString(physicalPath);
            }
        }       

        private bool CanGZip(HttpRequest request)
        {
            string acceptEncoding = request.Headers["Accept-Encoding"];
            if (!string.IsNullOrEmpty(acceptEncoding) && (acceptEncoding.Contains("gzip") | acceptEncoding.Contains("deflate")))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private string GetASPXSiteUrl(System.Web.HttpRequest request)
        {
            System.Text.StringBuilder url = new System.Text.StringBuilder();
            {
                url.Append(request.Url.Scheme);
                url.Append(Uri.SchemeDelimiter);
                url.Append(request.Url.Host);
                int portNumber = request.Url.Port;
                if (portNumber != 80 && portNumber != 443)
                {
                    url.Append(":");
                    url.Append(Convert.ToString(portNumber));
                }
                url.Append(request.ApplicationPath);                
            }
            return url.ToString();
        }   

        private void WriteBytes(byte[] bytes, HttpContext context, bool isCompressed, string contentType)
        {
            HttpResponse response = context.Response;

            response.AppendHeader("Content-Length", bytes.Length.ToString());
            response.ContentType = contentType;

            if (isCompressed)
                response.AppendHeader("Content-Encoding", "gzip");

            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.Cache.SetExpires(DateTime.Now.Add(CACHE_DURATION));
            context.Response.Cache.SetMaxAge(CACHE_DURATION);
            context.Response.Cache.AppendCacheExtension("must-revalidate, proxy-revalidate");

            response.OutputStream.Write(bytes, 0, bytes.Length);
            response.Flush();
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}
