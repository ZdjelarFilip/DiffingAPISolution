using DiffingAPI.Models;

namespace DiffingAPI.Services
{
    public interface IDiffService
    {
        void StoreLeft(string id, string data);
        void StoreRight(string id, string data);
        object? GetDiff(string id);
    }
}