using System.Net.Http.Json;
using System.Web;

namespace ChatServer.ConsoleClient.Clients
{
    public static class HttpContentExtensions
    {
        public static async Task<T?> ReadJson<T>(this HttpResponseMessage responseMessage) where T : class
        {
            if (responseMessage.IsSuccessStatusCode)
            {

                try
                {
                    return await responseMessage.Content.ReadFromJsonAsync<T>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("HTTP request error: {0}", ex.Message);
                }
            }
            else
            {
                Console.WriteLine("HTTP request error status: {0}, body: {1}", responseMessage.StatusCode, await responseMessage.Content.ReadAsStringAsync());
            }
            return null;
        }


        public static string ToQueryString<T>(this T obj)
        {
            var properties = from p in obj?
                                        .GetType()
                                        .GetProperties()
                             where p.GetValue(obj, null) != null
                             select $"{HttpUtility.UrlEncode(p.Name)}" +
                             $"={HttpUtility.UrlEncode(p.GetValue(obj)?.ToString())}";
            return string.Join("&", properties);
        }
    }
}
