using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Rarities
{
	public class Turquoise : ModRarity
	{
		public override Color RarityColor => new Color(0, 255, 200);

		public override int GetPrefixedRarity(int offset, float valueMult)
		{
			switch (offset)
			{
				case -2:
					return ItemRarityID.Red;
				case -1:
					return ItemRarityID.Purple;
				case 1:
					return ModContent.RarityType<PureGreen>();
				case 2:
					return ModContent.RarityType<DarkBlue>();
			}
			return Type;
		}
	}
}