using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication
{
    public class EmailSender
    {
        private int sendResult;

        public int SendEmail()
        {
            Console.WriteLine("Simulating sending email");
            sendResult = 0;
            return sendResult;
        }
    }
}
