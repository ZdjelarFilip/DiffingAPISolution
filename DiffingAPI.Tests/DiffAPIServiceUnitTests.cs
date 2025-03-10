using DiffingAPI.Services;

namespace DiffingAPI.Tests
{
    public class DiffAPIServiceUnitTests
    {
        private readonly DiffService _diffService;

        public DiffAPIServiceUnitTests()
        {
            _diffService = new DiffService();
        }

        [Fact]
        public void StoreLeft_ShouldStoreData()
        {
            string id = Guid.NewGuid().ToString();
            string data = Convert.ToBase64String(new byte[] { 1, 2, 3 });

            _diffService.StoreLeft(id, data);
            var result = _diffService.GetDiff(id);

            Assert.Null(result);
        }

        [Fact]
        public void StoreRight_ShouldStoreData()
        {
            string id = Guid.NewGuid().ToString();
            string data = Convert.ToBase64String(new byte[] { 1, 2, 3 });

            _diffService.StoreRight(id, data);
            var result = _diffService.GetDiff(id);

            Assert.Null(result);
        }

        [Fact]
        public void GetDiff_ShouldReturnEquals_WhenDataIsIdentical()
        {
            string id = Guid.NewGuid().ToString();
            string data = Convert.ToBase64String(new byte[] { 1, 2, 3 });

            _diffService.StoreLeft(id, data);
            _diffService.StoreRight(id, data);

            var result = _diffService.GetDiff(id);

            Assert.NotNull(result);
            Assert.Contains("Equals", result.ToString());
        }

        [Fact]
        public void GetDiff_ShouldReturnSizeDoNotMatch_WhenDataLengthsAreDifferent()
        {
            string id = Guid.NewGuid().ToString();
            string leftData = Convert.ToBase64String(new byte[] { 1, 2, 3 });
            string rightData = Convert.ToBase64String(new byte[] { 1, 2 });

            _diffService.StoreLeft(id, leftData);
            _diffService.StoreRight(id, rightData);

            var result = _diffService.GetDiff(id);

            Assert.NotNull(result);
            Assert.Contains("SizeDoNotMatch", result.ToString());
        }

        [Fact]
        public void GetDiff_ShouldReturnContentDoNotMatch_WhenDataDiffers()
        {
            string id = Guid.NewGuid().ToString();
            string leftData = Convert.ToBase64String(new byte[] { 1, 2, 3 });
            string rightData = Convert.ToBase64String(new byte[] { 1, 3, 3 });

            _diffService.StoreLeft(id, leftData);
            _diffService.StoreRight(id, rightData);

            var result = _diffService.GetDiff(id);

            Assert.NotNull(result);
            Assert.Contains("ContentDoNotMatch", result.ToString());
        }
    }
}