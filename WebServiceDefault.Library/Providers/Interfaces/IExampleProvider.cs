using System.Threading.Tasks;
using WebServiceDefault.Common.Models;

namespace WebServiceDefault.Library.Providers.Interfaces
{
    public interface IExampleProvider
    {
        Task<string> SendAsync(ExampleRequest request);
    }
}
