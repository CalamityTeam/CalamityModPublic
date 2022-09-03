using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace CalamityMod.Rarities
{
	public class Violet : ModRarity
	{
		public override Color RarityColor => new Color(108, 45, 199);

		public override int GetPrefixedRarity(int offset, float valueMult)
		{
			switch (offset)
			{
				case -2:
					return ModContent.RarityType<PureGreen>();
				case -1:
					return ModContent.RarityType<DarkBlue>();
				case 2:
				case 1:
					return ModContent.RarityType<HotPink>();
			}
			return Type;
		}
	}
}