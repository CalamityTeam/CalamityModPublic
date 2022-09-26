using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Rarities
{
	public class PureGreen : ModRarity
	{
		// Pure Green is Rarity 13
		public override Color RarityColor => new Color(0, 255, 0);

		public override int GetPrefixedRarity(int offset, float valueMult) => offset switch
		{
			-2 => ItemRarityID.Purple,
			-1 => ModContent.RarityType<Turquoise>(),
			1 => ModContent.RarityType<DarkBlue>(),
			2 => ModContent.RarityType<Violet>(),
			_ => Type,
		};
	}
}
