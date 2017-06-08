using System.Text;
using Bergfall.Oculos.Utils;

namespace Bergfall.Oculos.Data
{
    public class Message
    {
        public Message(string recipientsNumber)
        {
            RecipientsNumber = recipientsNumber;
        }

        public string RecipientsNumber { get; }

        public Encoding Encoding { get; set; } = new GSMEncoding();

        public int MaxNumberOfCharacters
        {
            get
            {
                if (Equals(Encoding, new GSMEncoding()))
                {
                    return 1120 / 7;
                }
                else if (Equals(Encoding, Encoding.BigEndianUnicode))
                {
                    return 1120 / 16;
                }
                else
                {
                    return 1120 / 8;
                }
            }
            set
            {
            }
        }

        public int MessageCount { get; set; } = 1;


        public string Body { get; set; } = "";

        public byte[]
        public int Size { get; set; }
    }
}