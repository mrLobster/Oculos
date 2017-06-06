using System.Linq;
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
        public int MaxNumberOfCharacters { get; set; }
        public string MessageCount
        {
            get; set;
        }

        public string Body
        {
            set
            {
                byte[] bytes = Encoding.GetBytes(value);
                foreach (byte bit in bytes)
                {
                    if(bit > 0x7F)
                    {
                        // Cheat by using standard Unicode if any sign is beyond the 128 limit, instead of USC2, which I think is just Unicode BigEndian?!
                        Encoding = Encoding.BigEndianUnicode;
                        MaxNumberOfCharacters = 70;
                    }
                }
                //byte[] bigBits = EncodeStuff.EncodeUCS2(bytes.ToString());
                string encoded = EncodeStuff.IntToHex(bytes);
                Encoding = new GSMEncoding();
                MaxNumberOfCharacters = 160;


            }
        }
       
        public int Size { get; set; }
    }
}