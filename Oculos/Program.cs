using System;
using System.IO;
using Bergfall.Oculos.Utils;

namespace Bergfall.Oculos
{
    public class Program
    {
        private static string consoleMsg =
            "Insert filename with template, recipients and variables." + Environment.NewLine + "Line 1 : SMS Template, {keywords} in this form" + Environment.NewLine + "Line 2+ : Phonenumber of recipient, and a comma seperated key-value list."
            + Environment.NewLine + "Press Enter for (in.txt)";
                  

        private static string workingDirectory = Directory.GetCurrentDirectory();

        public static void Main(string[] args)
        {
           
            Console.WriteLine(consoleMsg);

            Program program = new Program();

            var exit = false;
            while(!exit)
            {
                string input = Console.ReadLine();
                Console.WriteLine(input);
                if(input.Equals(""))
                {
                    Run(Path.Combine(workingDirectory, "in.txt"));
                    exit = true;
                }
                else if(input.Equals("q"))
                {
                    exit = true;
                }
                else
                {
                    Run(Path.Combine(workingDirectory, Console.ReadLine()));
                    exit = true;
                }

            }
        }

        private static void Run(string inFileName)
        {
            try
            {


                if (!File.Exists(inFileName))
                {
                    Log.Error("Infilename " + inFileName + " does not exist!");
                }
                else
                {
                    IO.GetInstance(inFileName);
                }
            }
            catch (Exception exp)
            {
                Log.Debug(exp.Message);
            }
        }
    }
}