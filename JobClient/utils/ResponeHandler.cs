using System.IO;
using System.Net;

namespace JobClient
{
    public static class ResponeHandler
    {
        /// <summary>
        ///   输出Html
        /// </summary>
        /// <param name="response"> </param>
        /// <param name="responseString"> </param>
        public static void ResponseHtml(HttpListenerResponse response, string responseString)
        {
            ResponseStr(response, responseString, "text/html; charset=UTF-8");
        }

        /// <summary>
        ///   输出图片
        /// </summary>
        /// <param name="response"> </param>
        /// <param name="stream"> 图片流 </param>
        public static void ResponseImage(HttpListenerResponse response, Stream stream)
        {

            //using (response)
            //{
            using (stream)
            {
                response.ContentLength64
                    = stream.Length;
                response.ContentType = "image/png";


                var buff = new byte[4096];

                stream.Seek(0, SeekOrigin.Begin);

                var count = 0;
                while ((count = stream.Read(buff, 0, 4096)) != 0)
                {
                    response.OutputStream.Write(buff, 0, count);
                }

                response.OutputStream.Flush();
                stream.Close();
                response.Close();
            }
            //}
        }

        /// <summary>
        ///   输出TxtPlan
        /// </summary>
        /// <param name="response"> </param>
        /// <param name="responseString"> </param>
        public static void ResponsePlan(HttpListenerResponse response, string responseString)
        {
            ResponseStr(response, responseString, "text/plain; charset=UTF-8");
        }

        /// <summary>
        ///   输出Html文本
        /// </summary>
        /// <param name="response"> </param>
        /// <param name="responseString"> </param>
        /// <param name="contentType"> </param>
        public static void ResponseStr(HttpListenerResponse response, string responseString, string contentType)
        {
            // 设置回应头部内容，长度，编码
            response.ContentLength64
                = System.Text.Encoding.UTF8.GetByteCount(responseString);
            response.ContentType = contentType;
            // 输出回应内容
            var output = response.OutputStream;
            var writer = new System.IO.StreamWriter(output);
            writer.Write(responseString);
            // 必须关闭输出流
            writer.Close();
        }
    }
}
