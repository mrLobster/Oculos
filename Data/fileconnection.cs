using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Bergfall.Oculos.Data
{
    public class FileConnection
    {
        public string FileName
        {
            get; private set;
        }

        public IList<int> SomeNumbers { get; } = new List<int>();

        public FileConnection(string fileName)
        {
            if(File.Exists(fileName))
            {
                FileName = fileName;
            }
        }

        public async Task WriteTextAsync(string filePath, string text)
        {
            byte[] encodedText = Encoding.Unicode.GetBytes(text);

            using(FileStream sourceStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
            {
                await sourceStream.WriteAsync(encodedText, 0, encodedText.Length).ConfigureAwait(false);
            };
        }
    }
}