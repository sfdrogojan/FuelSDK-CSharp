using System;
using System.IO;
using System.Net;

namespace FuelSDK
{
    public class HttpWebRequestWrapper : IHttpWebRequestWrapper
    {
        private HttpWebRequest httpWebRequest;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpWebRequestWrapper"/> class.
        /// </summary>
        /// <param name="url">The URL.</param>
        public HttpWebRequestWrapper(Uri url)
        {
            this.httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
        }

        /// <summary>
        /// Gets the request stream to write data.
        /// </summary>
        /// <returns></returns>
        public Stream GetRequestStream()
        {
            return this.httpWebRequest.GetRequestStream();
        }

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        /// <value>The method.</value>
        public string Method
        {
            get { return this.httpWebRequest.Method; }
            set { this.httpWebRequest.Method = value; }
        }

        public string UserAgent
        {
            get { return this.httpWebRequest.UserAgent; }
            set { this.httpWebRequest.UserAgent = value; }
        }

        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        /// <value>The type of the content.</value>
        public string ContentType
        {
            get { return this.httpWebRequest.ContentType; }
            set { this.httpWebRequest.ContentType = value; }
        }

        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <returns></returns>
        public IHttpWebResponseWrapper GetResponse()
        {
            return new HttpWebResponseWrapper(this.httpWebRequest.GetResponse());
        }

        public WebHeaderCollection Headers
        {
            get { return this.httpWebRequest.Headers; }
            set { this.httpWebRequest.Headers = value; }
        }
    }

    public class HttpWebResponseWrapper : IHttpWebResponseWrapper
    {
        private WebResponse webResponse;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpWebResponseWrapper"/> class.
        /// </summary>
        /// <param name="webResponse">The web response.</param>
        public HttpWebResponseWrapper(WebResponse webResponse)
        {
            this.webResponse = webResponse;
        }

        /// <summary>
        /// Gets the status code.
        /// </summary>
        /// <value>The status code.</value>
        public HttpStatusCode StatusCode
        {
            get { return ((HttpWebResponse)webResponse).StatusCode; }
        }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <returns></returns>
        public string GetContent()
        {
            using (StreamReader stream = new StreamReader(webResponse.GetResponseStream()))
            {
                return stream.ReadToEnd();
            }
        }

        public override string ToString()
        {
            return ((HttpWebResponse)webResponse).ToString();
        }
    }

    public interface IHttpWebRequestWrapper
    {
        string ContentType { get; set; }
        Stream GetRequestStream();
        IHttpWebResponseWrapper GetResponse();
        WebHeaderCollection Headers { get; set; }
        string Method { get; set; }
        string UserAgent { get; set; }
    }

    public interface IHttpWebResponseWrapper
    {
        string GetContent();
        HttpStatusCode StatusCode { get; }
    }

    public interface IHttpWebRequestWrapperFactory
    {
        IHttpWebRequestWrapper Create(Uri requestUri);
    }

    public class HttpWebRequestWrapperFactory : IHttpWebRequestWrapperFactory
    {
        public IHttpWebRequestWrapper Create(Uri requestUri)
        {
            return new HttpWebRequestWrapper(requestUri);
        }
    }
}