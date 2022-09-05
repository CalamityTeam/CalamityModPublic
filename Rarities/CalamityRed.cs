using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace CalamityMod.Rarities
{
	public class CalamityRed : ModRarity
	{
		// Calamity Red is Rarity 17
		// This rarity should never be assigned to any items.
		// It is the equivalent of vanilla's Purple rarity (11): only used for positive reforges on top-rarity items.
		public override Color RarityColor => new Color(163, 25, 26); // #A3191A
		// (139, 0, 0) is the classic donator item color

		public override int GetPrefixedRarity(int offset, float valueMult) => offset switch
		{
			-2 => ModContent.RarityType<Violet>(),
			-1 => ModContent.RarityType<HotPink>(),
			_ => Type,
		};
	}
}
