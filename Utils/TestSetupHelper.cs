using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;


namespace Bergfall.Oculos.Utils
{
    public static class TestSetupHelper
    {
        public static void WriteTestOutFile()
        {
            var outFile = Directory.GetCurrentDirectory() + @"\in2.txt";

            Random random = new Random();

            try
            {
                for (int k = 0; k < 1000; k++)
                {

                    using (FileStream fs = new FileStream(outFile, FileMode.Append, FileAccess.Write))
                    {

                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            int[] telephone = new int[8];
                            for (int i = 0; i < 8; i++)
                            {
                                telephone[i] = random.Next(0, 9);
                            }
                            string telephoneResult = "recipient:" + string.Join("", telephone);
                            sw.Write(telephoneResult);

                            // create two variables
                            sw.Write(",someVariable,");

                            for (int i = 0; i < 2; i++)
                            {
                                if (i == 0)
                                {
                                    string variable1 = GetRandomString(random, random.Next(4, 10));
                                    sw.Write(variable1);
                                }
                                else
                                {
                                    sw.Write(",someOtherVariable,");
                                    string variable2 = GetRandomString(random, random.Next(4, 10));
                                    sw.WriteLine(variable2);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                
            }
        }


        public static string GetRandomString(Random rnd, int length)
        {
            string charPool = @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvw xyz1234567890!""#¤%&/()=?+€][$£@";
            StringBuilder rs = new StringBuilder();

            while (length-- > 0)
            {
                rs.Append(charPool[(int)(rnd.NextDouble() * charPool.Length)]);
            }

            return rs.ToString();
        }

        private static string toHex(int nr)
        {
            return nr.ToString("X4");
        }
    }
}