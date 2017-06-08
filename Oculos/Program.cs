using System;
using System.IO;
using Bergfall.Oculos.Utils;

namespace Bergfall.Oculos
{
    public class Program
    {
        private string consoleMsg =
            @"Insert filename with template, recipients and variables.
                  Line 1 : SMS Template, {keywords} in this form
                  Line 1<: Phonenumber of recipient, and a comma seperated key-value list.
                  Press Enter for (in.txt)
                  Press 1 for GSMEncodeAchar";

        public static void Main(string[] args) => new Program();

        private Program()
        {
            try
            {
                var dir = Directory.GetCurrentDirectory();

                Log.Debug(consoleMsg + "\n" + dir);

                string filename = Console.ReadLine();
                if(filename.Length > 0)
                {
                    Log.Write("Enter character :");
                    var c = Console.ReadKey(true).KeyChar;
                    Log.Write(c + " is " + EncodeStuff.CharTo7Bit(c));
                }
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
                Log.Error(exp.Message);
            }
            //catch(FileNotFoundException exp)
            //{
            //    Log.Error(exp.Message);
            //}

            // Singleton way of initiating IO class
            IO io = IO.GetInstance;
        }
    }
}