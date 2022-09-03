using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace CalamityMod.Rarities
{
	public class HotPink : ModRarity
	{
		public override Color RarityColor => new Color(255, 0, 255);

		public override int GetPrefixedRarity(int offset, float valueMult)
		{
			switch (offset)
			{
				case -2:
					return ModContent.RarityType<DarkBlue>();
				case -1:
					return ModContent.RarityType<Violet>();
			}
			return Type;
		}
	}
}