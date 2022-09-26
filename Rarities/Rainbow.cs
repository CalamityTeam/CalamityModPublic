using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Rarities
{
	// Heart of the Elements, The Community and Elemental Excalibur are Rainbow for decoration
	// TODO -- this should be changed to be a color override
	// it's only used for a few items and is purely decorative
	public class Rainbow : ModRarity
	{
		public override Color RarityColor => new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
	}
}
