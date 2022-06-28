using System.Text;

namespace Game.Network
{
    public static class Convert
    {
        static readonly Encoding encoding = new UTF8Encoding(false);

        public static string ToString(byte[] buffer)
        {
            return encoding.GetString(buffer);
        }

        public static string ToString(byte[] buffer, int index, int count)
        {
            return encoding.GetString(buffer, index, count);
        }

        public static byte[] ToBytes(string value)
        {
            return encoding.GetBytes(value);
        }
    }
}