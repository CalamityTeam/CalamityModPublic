using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace CalamityMod.Rarities
{
	public class HotPink : ModRarity
	{
		// Hot Pink is Rarity 16
		public override Color RarityColor => new Color(255, 0, 255);

		public override int GetPrefixedRarity(int offset, float valueMult) => offset switch
		{
			-2 => ModContent.RarityType<DarkBlue>(),
			-1 => ModContent.RarityType<Violet>(),
			1 => ModContent.RarityType<CalamityRed>(),
			2 => ModContent.RarityType<CalamityRed>(),
			_ => Type,
		};
	}
}
