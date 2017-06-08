using System;
using System.IO;
using Bergfall.Oculos.Utils;

namespace Bergfall.Oculos
{
    public class Program
    {
        private string consoleMsg =
                "Insert filename with template as line 1, {keywords} look like this; on the other lines, state phonenumber, and a comma seperated key-value list (in.txt)";

        public static void Main(string[] args) => new Program();

        private Program()
        {
            try
            {
                var dir = Directory.GetCurrentDirectory();

                Log.Debug(consoleMsg + "\n" + dir);

                string filename = Console.ReadLine();

                if(String.IsNullOrEmpty(filename))
                {
                    filename = dir + @"\log.txt";
                }
                else
                {
                    filename = dir + @"\" + filename;
                }
                var attributes = File.GetAttributes(filename);
                if((attributes & FileAttributes.Normal) != 0)
                {
                    string b = "";
                }
            }
            catch(Exception exp)
            {
                Log.Debug(exp.Message);
            }

            // Singleton way of initiating IO class
            IO io = IO.GetInstance;
        }
    }
}