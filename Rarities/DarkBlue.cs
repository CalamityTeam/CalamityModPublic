using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace CalamityMod.Rarities
{
	public class DarkBlue : ModRarity
	{
		public override Color RarityColor => new Color(43, 96, 222);

		public override int GetPrefixedRarity(int offset, float valueMult)
		{
			switch (offset)
			{
				case -2:
					return ModContent.RarityType<Turquoise>();
				case -1:
					return ModContent.RarityType<PureGreen>();
				case 1:
					return ModContent.RarityType<Violet>();
				case 2:
					return ModContent.RarityType<HotPink>();
			}
			return Type;
		}
	}
}