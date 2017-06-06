namespace Bergfall.Oculos.Interfaces
{
    internal interface ISMSencoding
    {
        string Name
        {
            get; set;
        }

        bool IsStringInEncoding(string message);

        byte[] EncodeString(string message);

        int CharactersPerSingleSMS
        {
            get; set;
        }
    }
}