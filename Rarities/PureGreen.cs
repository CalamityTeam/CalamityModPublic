using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Rarities
{
	public class PureGreen : ModRarity
	{
		public override Color RarityColor => new Color(0, 255, 0);

		public override int GetPrefixedRarity(int offset, float valueMult)
		{
			switch (offset)
			{
				case -2:
					return ItemRarityID.Purple;
				case -1:
					return ModContent.RarityType<Turquoise>();
				case 1:
					return ModContent.RarityType<DarkBlue>();
				case 2:
					return ModContent.RarityType<Violet>();
			}
			return Type;
		}
	}
}