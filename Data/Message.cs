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

        public string RecipientsNumber { get; private set; }

        public Encoding Encoding { get; set; } = new GSMEncoding();


        public int MaxNumberOfCharacters
        {
            get
            {
                if (Equals(Encoding, new GSMEncoding()))
                {
                    if (Body?.Length > 160)
                    {
                        return 153;
                    }
                    else
                    {
                        return 160;
                    }
                    //return 1120 / 7;
                }
                else if (Equals(Encoding, Encoding.BigEndianUnicode))
                {
                    return 80;
                }
                else // UTF-8 encoding
                {
                    return 140;
                }
            }
        }

        public int MessageCount { get; set; } = 1;


        public string Body { get; set; }

       
        public int SizeInBytes { get; set; }

        public int NumberOfCharacters => this.Body.Length;
    }

    public static class MessageExtensions
    {
        public static bool IsGSM(this string text)
        {
            return true;
        }
    }
}