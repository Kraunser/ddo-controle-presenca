using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Components.Forms;

namespace DDO.Web.Models
{
    public class BrowserFileAsFormFile : IFormFile
    {
        private readonly IBrowserFile _browserFile;

        public BrowserFileAsFormFile(IBrowserFile browserFile)
        {
            _browserFile = browserFile;
        }

        public string ContentType => _browserFile.ContentType;

        public string ContentDisposition => "form-data; name=\"file\"; filename=\"" + _browserFile.Name + "\"";

        public IHeaderDictionary Headers => new HeaderDictionary();

        public long Length => _browserFile.Size;

        public string Name => _browserFile.Name;

        public string FileName => _browserFile.Name;

        public Stream OpenReadStream()
        {
            return _browserFile.OpenReadStream();
        }

        public void CopyTo(Stream targetStream)
        {
            _browserFile.OpenReadStream().CopyTo(targetStream);
        }

        public async Task CopyToAsync(Stream targetStream, CancellationToken cancellationToken = default)
        {
            await _browserFile.OpenReadStream().CopyToAsync(targetStream, cancellationToken);
        }
    }
}
