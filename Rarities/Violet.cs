using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace CalamityMod.Rarities
{
	public class Violet : ModRarity
	{
		// Violet is Rarity 15
		public override Color RarityColor => new Color(108, 45, 199);

		public override int GetPrefixedRarity(int offset, float valueMult) => offset switch
		{
			-2 => ModContent.RarityType<PureGreen>(),
			-1 => ModContent.RarityType<DarkBlue>(),
			1 => ModContent.RarityType<HotPink>(),
			2 => ModContent.RarityType<CalamityRed>(),
			_ => Type,
		};
	}
}
