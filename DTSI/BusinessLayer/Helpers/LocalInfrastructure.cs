

using Microsoft.AspNetCore.Http;

namespace BusinessLayer.Helpers
{
    public class LocalInfrastructure
    {
      public  static IEnumerable<string> Split(string str, int chunkSize)
        {
            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(i => str.Substring(i * chunkSize, chunkSize));
        }

        public async  Task<string> LocalImageStore(string path, IFormFile file)
        {
            string imgValue = null;

            var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);

            imgValue = file.FileName.Replace(" ", "_");

            stream.Close();

            return imgValue;
        }
    }
}
