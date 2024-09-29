using System.Threading.Tasks;

namespace JokeStorageFunctionApp.Interfaces;

public interface IBlobService
{
    Task SavePayloadAsync(string id, string content);
    Task<string> GetPayloadAsync(string id);
}
