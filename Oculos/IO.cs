using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Bergfall.Oculos
{
    public class IO
    {
        private const int DefaultBufferSize = 4096;

        private const FileOptions DefaultOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;

        public IList<string> ReadFileToList(string fileName)
        {
            string[] text = ReadAllLines(fileName);
            return new List<string>(text);
        }
            /// <summary>
            /// Reads all lines of parameter path async, returning a string[] with all the lines of the file
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            private static string[] ReadAllLines(string path)
        {
            return ReadAllLines(path, Encoding.UTF8);
        }

        /// <summary>
        /// The meat of the ReadAllLines code
        /// </summary>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        private static string[] ReadAllLines(string path, Encoding encoding)
        {
            var lines = new List<string>();

            // Open the FileStream with the same FileMode, FileAccess
            // and FileShare as a call to File.OpenText would've done.
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read,
                DefaultBufferSize, DefaultOptions))
            {
                using (StreamReader reader = new StreamReader(stream, encoding))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        lines.Add(line);
                    }
                }
            }

            return lines.ToArray();
        }
    }
}
