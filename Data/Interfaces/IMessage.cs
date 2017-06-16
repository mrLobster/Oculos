using System;
using System.Collections.Generic;
using System.Text;

namespace Bergfall.Oculos.Data.Interfaces
{
    public interface IMessage
    {
        string RecipientsNumber { get; }
        string Body { get; set; }
        int PartsCount { get; set; }
        int NumberOfCharacters { get; }
        int SizeInBytes { get; }
        int MaxNumberOfCharacters { get; }
        Encoding Encoding { get; }
    }
}
