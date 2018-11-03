using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
	public class CosmiliteBrick : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;
			drop = mod.ItemType("CosmiliteBrick");
			AddMapEntry(new Color(60, 50, 50));
		}
	}
}