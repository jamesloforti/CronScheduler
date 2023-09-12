using System.Threading.Tasks;

namespace WebServiceDefault.Library.Clients.Interfaces
{
    public interface IGenericHttpClient
    {
        Task<T> GetAsync<T>(string url);
        Task<string> GetAsync(string url);
        Task<T> PostAsync<T, U>(string url, U data);
        Task<string> PostAsync(string url, string request = null);
    }
}
