using Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = string.Empty;

            Server server = new Server("127.0.0.1", 88);

            while (input != "Exit")
            {
                input = Console.ReadLine();

                if (input == "Close")
                {
                    server.Close();
                }
                else
                {
                    server.Send(input);
                }
            }


            Console.WriteLine("按任意键退出...");
            Console.ReadKey();
        }
    }
}