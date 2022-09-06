using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace CalamityMod.Rarities
{
	public class DarkBlue : ModRarity
	{
		// Dark Blue is Rarity 14
		public override Color RarityColor => new Color(43, 96, 222);

		public override int GetPrefixedRarity(int offset, float valueMult) => offset switch
		{
			-2 => ModContent.RarityType<Turquoise>(),
			-1 => ModContent.RarityType<PureGreen>(),
			1 => ModContent.RarityType<Violet>(),
			2 => ModContent.RarityType<HotPink>(),
			_ => Type,
		};
	}
}
