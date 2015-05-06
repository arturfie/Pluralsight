using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveNation.Services.Interfaces;

namespace LiveNation.Services
{
    class ConsoleDisplay : IDisplay
    {
        public void Write(string message)
        {
            Console.Write(message);
        }
    }
}
