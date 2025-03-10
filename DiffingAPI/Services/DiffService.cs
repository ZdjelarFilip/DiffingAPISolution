using DiffingAPI.Models;
using System.Collections.Concurrent;

namespace DiffingAPI.Services
{
    public class DiffService : IDiffService
    {
        private static readonly ConcurrentDictionary<string, DiffData> _storage = new();

        public void StoreLeft(string id, string data)
        {
            _storage.AddOrUpdate(id, new DiffData { Left = data }, (_, d) => { d.Left = data; return d; });
        }

        public void StoreRight(string id, string data)
        {
            _storage.AddOrUpdate(id, new DiffData { Right = data }, (_, d) => { d.Right = data; return d; });
        }

        public object? GetDiff(string id)
        {
            if (!_storage.TryGetValue(id, out var data) ||
                string.IsNullOrEmpty(data.Left) ||
                string.IsNullOrEmpty(data.Right))
                return null;

            var leftBytes = Convert.FromBase64String(data.Left);
            var rightBytes = Convert.FromBase64String(data.Right);

            if (leftBytes.Length != rightBytes.Length)
                return new { diffResultType = "SizeDoNotMatch" };

            var diffs = GetDifferences(leftBytes, rightBytes);
            return diffs.Count == 0 ? new { diffResultType = "Equals" } : new { diffResultType = "ContentDoNotMatch", diffs };
        }

        // Removed 'static' so it matches instance method behavior
        private List<DiffResult> GetDifferences(byte[] left, byte[] right)
        {
            var diffs = new List<DiffResult>();
            int? offset = null;
            int length = 0;

            for (int i = 0; i < left.Length; i++)
            {
                if (left[i] != right[i])
                {
                    if (offset == null) offset = i;
                    length++;
                }
                else if (offset != null)
                {
                    diffs.Add(new DiffResult(offset.Value, length));
                    offset = null;
                    length = 0;
                }
            }

            if (offset != null)
            {
                diffs.Add(new DiffResult(offset.Value, length));
            }

            return diffs;
        }
    }
}