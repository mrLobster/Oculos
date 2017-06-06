using System;
using System.IO;
using System.Linq;
using System.Text;
using Bergfall.Oculos.Utils;

namespace Bergfall.Oculos
{
    public class Program
    {
        private string consoleMsg =
                "Insert filename with template as line 1, {keywords} look like this; on the other lines, state phonenumber, and a comma seperated key-value list (in.txt)"
            ;

        public static void Main(string[] args) => new Program();

        public Program()
        {
            Log.Debug(consoleMsg);
            string filename = Console.ReadLine();
            string dir = Directory.GetCurrentDirectory();

            if (String.IsNullOrEmpty(filename))
            {
                filename = dir + @"\log.txt";
            }
            else
            {
                filename = dir + @"\" + filename;
            }


            IO io = new IO().GetInstance();
        }
    }
}
