using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication
{
    class Program
    {
        private delegate void testDelegate(string value);

        static void Main(string[] args)
        {
            Document doc = new Document();
            doc.Text = "Text of document";

            var blogPoster = new BlogPoster();
            var blogDelegate = new Document.SendDoc(blogPoster.PostToBlog);
            doc.ReportSendingResult(blogDelegate);

            var emailSender = new EmailSender();
            var emailDelegate = new Document.SendDoc(emailSender.SendEmail);
            doc.ReportSendingResult(emailDelegate);

            testDelegate testDelegateInstance = value => Console.WriteLine(value);
            testDelegateInstance.Invoke("This is Test");
            
            Console.ReadKey();
        }
    }
}
