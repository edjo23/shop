using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Service.Client
{
    public class HttpRequestException : Exception
    {
        public HttpRequestException(HttpStatusCode responseCode, string responseContent = null)
        {
            ResponseCode = responseCode;
            ResponseContent = responseContent;
        }

        public HttpStatusCode ResponseCode { get; private set; }

        public string ResponseContent { get; set; }
    }

    public abstract class HttpClient
    {
        protected TResult Invoke<TResult>(Func<System.Net.Http.HttpClient, Task<TResult>> call)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                var task = call(client);
                try
                {
                    task.Wait();
                }
                catch (AggregateException ex)
                {
                    throw ex.InnerException;
                }
                return task.Result;
            }
        }

        protected string BuildUri(string baseUrl, Dictionary<string, string> parameters)
        {
            if (parameters == null)
                return baseUrl;

            // TODO - URL encode ... or use a proper uri builder

            var sb = new StringBuilder(baseUrl);
            var seperator = "?";

            foreach (var item in parameters)
            {
                sb.Append($"{seperator}{item.Key}={item.Value}"); 
                seperator = "&";
            }

            return sb.ToString();
        }

        protected TResult Get<TResult>(string url, Dictionary<string,string> queryParams = null)
        {
            var uri = BuildUri(url, queryParams);

            var task = Task.Run(() =>
            {
                return Invoke(async client =>
                {
                    using (var response = await client.GetAsync(uri))
                    using (var responseHttpContent = response.Content)
                    {
                        var responseContent = await responseHttpContent.ReadAsStringAsync();

                        if (!response.IsSuccessStatusCode)
                            throw new HttpRequestException(response.StatusCode, responseContent);

                        return JsonConvert.DeserializeObject<TResult>(responseContent);
                    }
                });
            });

            try
            {
                task.Wait();
            }
            catch (AggregateException ex)
            {
                throw ex.InnerException;
            }

            return task.Result;
        }

        protected HttpStatusCode Post<TBody>(string url, TBody content)
        {
            var task = Task.Run(() =>
            {
                return Invoke<HttpStatusCode>(async client =>
                {
                    var requestContent = JsonConvert.SerializeObject(content);
                    using (var response = await client.PostAsync(url, new StringContent(requestContent, Encoding.UTF8, "application/json")))
                    using (var responseHttpContent = response.Content)
                    {
                        var responseContent = await responseHttpContent.ReadAsStringAsync();

                        if (!response.IsSuccessStatusCode)
                            throw new HttpRequestException(response.StatusCode, responseContent);

                        return response.StatusCode;
                    }
                });
            });

            try
            {
                task.Wait();
            }
            catch (AggregateException ex)
            {
                throw ex.InnerException;
            }

            return task.Result;
        }

        protected TResult Post<TBody, TResult>(string url, TBody content)
        {
            var task = Task.Run(() =>
            {
                return Invoke<TResult>(async client =>
                {
                    var requestContent = JsonConvert.SerializeObject(content);
                    using (var response = await client.PostAsync(url, new StringContent(requestContent, Encoding.UTF8, "application/json")))
                    using (var responseHttpContent = response.Content)
                    {
                        var responseContent = await responseHttpContent.ReadAsStringAsync();

                        if (!response.IsSuccessStatusCode)
                            throw new Exception(response.StatusCode.ToString());

                        return JsonConvert.DeserializeObject<TResult>(responseContent);
                    }
                });
            });

            try
            {
                task.Wait();
            }
            catch (AggregateException ex)
            {
                throw ex.InnerException;
            }

            return task.Result;
        }

        protected HttpStatusCode Put<TBody>(string url, TBody content)
        {
            var task = Task.Run(() =>
            {
                return Invoke<HttpStatusCode>(async client =>
                {
                    var requestContent = JsonConvert.SerializeObject(content);
                    using (var response = await client.PutAsync(url, new StringContent(requestContent, Encoding.UTF8, "application/json")))
                    using (var responseHttpContent = response.Content)
                    {
                        var responseContent = await responseHttpContent.ReadAsStringAsync();

                        if (!response.IsSuccessStatusCode)
                            throw new Exception(response.StatusCode.ToString());

                        return response.StatusCode;
                    }
                });
            });

            try
            {
                task.Wait();
            }
            catch (AggregateException ex)
            {
                throw ex.InnerException;
            }

            return task.Result;
        }

        protected TResult Put<TBody, TResult>(string url, TBody content)
        {
            var task = Task.Run(() =>
            {
                return Invoke<TResult>(async client =>
                {
                    var requestContent = JsonConvert.SerializeObject(content);
                    using (var response = await client.PutAsync(url, new StringContent(requestContent, Encoding.UTF8, "application/json")))
                    using (var responseHttpContent = response.Content)
                    {
                        var responseContent = await responseHttpContent.ReadAsStringAsync();

                        if (!response.IsSuccessStatusCode)
                            throw new Exception(response.StatusCode.ToString());

                        return JsonConvert.DeserializeObject<TResult>(responseContent);
                    }
                });
            });

            try
            {
                task.Wait();
            }
            catch (AggregateException ex)
            {
                throw ex.InnerException;
            }

            return task.Result;
        }
    }
}
