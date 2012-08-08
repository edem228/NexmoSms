using System;

namespace Program
{
    class Program
    {
        static void Main(string[] args)
        {
            var sms = new NexmoSMS("{Key}", "{Secret}");
            var response = sms.Send("{Sender}", "{Reciver}", "{Text}");
            Console.ReadKey();
        }
    }
}
