using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Rarities
{
	public class Rainbow : ModRarity
	{
		public override Color RarityColor => new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
	}
}