using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Walls
{
	public class ProfanedRockWall : ModWall
	{
		public override void SetDefaults()
		{
			Main.wallHouse[Type] = true;
			dustType = mod.DustType("Sparkle");
			drop = mod.ItemType("ProfanedRockWall");
			AddMapEntry(new Color(5, 5, 5));
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
	}
}