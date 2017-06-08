using System.Text;
using Bergfall.Oculos.Utils;

namespace Bergfall.Oculos.Data
{
    public class Message
    {
        public string RecipientsNumber
        {
            get; set;
        }

        public Encoding Encoding { get; set; } = new GSMEncoding();
        public int MaxNumberOfCharacters { get; set; } = 160;

        public int MessageCount
        {
            get; set;
        }

        public string Body
        {
            get; set;
        }

        public int Size
        {
            get; set;
        }
    }
}