using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Rarities
{
	public class Turquoise : ModRarity
	{
		// Turquoise is Rarity 12
		public override Color RarityColor => new Color(0, 255, 200);

		public override int GetPrefixedRarity(int offset, float valueMult) => offset switch
		{
			-2 => ItemRarityID.Red,
			-1 => ItemRarityID.Purple,
			1 => ModContent.RarityType<PureGreen>(),
			2 => ModContent.RarityType<DarkBlue>(),
			_ => Type,
		};
	}
}
