using System.Linq;
using Terraria;

namespace CalamityMod
{
    public static partial class CalamityUtils
    {
        internal static readonly string AlphanumericCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        public static string GenerateRandomAlphanumericString(int length) => new string(Enumerable.Repeat(AlphanumericCharacters, length).Select(s => s[Main.rand.Next(s.Length)]).ToArray());
    }
}
