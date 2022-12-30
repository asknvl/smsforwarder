using smsforwarder.bot;

namespace smsforwarder
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bot_forwarder bot = new bot_forwarder();
            bot.Start();
            Console.ReadLine();
        }
    }
}