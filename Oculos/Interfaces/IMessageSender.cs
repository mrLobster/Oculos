using System;
using System.Collections.Generic;
using System.Text;
using Bergfall.Oculos.Data.Interfaces;

namespace Bergfall.Oculos.Interfaces
{
    public interface IMessageSender
    {
        bool Send(IMessage iMessage);
    }
    
}
