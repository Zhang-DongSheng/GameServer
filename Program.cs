using System;

namespace Game.Network
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("服务器启动...");

            string input = string.Empty;

            Server server = new Server("127.0.0.1", 88);

            while (input != "Exit")
            {
                input = Console.ReadLine();

                if (string.IsNullOrEmpty(input))
                    continue;

                switch (input.ToLower())
                {
                    case Command.HELP:
                        {
                            for (int i = 0; i < Command.information.Count; i++)
                            {
                                Console.WriteLine(Command.information[i]);
                            }
                        }
                        break;
                    case Command.CLOSE:
                        {
                            server.Close();
                        }
                        break;
                    default:
                        server.Send(input);
                        break;
                }
            }
            Console.WriteLine("按任意键退出...");

            Console.ReadKey();
        }
    }
}