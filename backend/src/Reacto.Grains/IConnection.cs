using System.Threading.Tasks;
using Orleans;

namespace Reacto.Grains
{
    public interface IConnection : IGrainWithStringKey
    {
        Task SetStageName(string stageName);
        
        Task<string?> GetStageName();
    }
}
