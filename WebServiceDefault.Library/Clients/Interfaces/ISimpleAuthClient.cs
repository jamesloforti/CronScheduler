using System.Threading.Tasks;

namespace WebServiceDefault.Library.Clients.Interfaces
{
    public interface ISimpleAuthClient<T> where T : class
    {
        string AccessToken { get; }
        Task AuthenticateAsync();
    }
}
