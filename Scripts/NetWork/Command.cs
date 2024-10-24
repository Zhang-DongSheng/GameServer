using System.Collections.Generic;

namespace Game.Network
{
    public static class Command
    {
        public const string HELP = ">.help";

        public const string CLOSE = ">.close";

        public static readonly List<string> information = new List<string>()
        {
            ">.help             帮助",
            ">.close            关闭",
        };
    }
}
