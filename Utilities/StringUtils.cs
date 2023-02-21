using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityMod
{
    public static partial class CalamityUtils
    {
        internal static readonly string AlphanumericCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        public static string GenerateRandomAlphanumericString(int length) => new string(Enumerable.Repeat(AlphanumericCharacters, length).Select(s => s[Main.rand.Next(s.Length)]).ToArray());

        public static string ColorMessage(this Color color, string text, bool newline = false)
        {
            int bufferSize = text.Length < 200 ? 256 : text.Length + 32;
            StringBuilder sb = new(bufferSize);
            sb.Append("[c/");
            sb.Append(color.Hex3());
            sb.Append(':');
            sb.Append(text);
            sb.Append(']');
            if (newline)
                sb.Append('\n');
            return sb.ToString();
        }
    }
}
