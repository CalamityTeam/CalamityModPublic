using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
	public class AstralOre : ModTile
	{
		public override void SetDefaults()
		{
            Main.tileLighted[Type] = true;
            Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;
            Main.tileValue[Type] = 700;
			minPick = 199;
			dustType = 173;
			drop = mod.ItemType("AstralOre");
			ModTranslation name = CreateMapEntryName();
 			name.SetDefault("Astral Ore");
 			AddMapEntry(new Color(255, 153, 255), name);
			mineResist = 5f;
			soundType = 21;
			Main.tileSpelunker[Type] = true;
		}
		
		public override bool CanKillTile(int i, int j, ref bool blockDamaged)
		{
			return CalamityWorld.downedStarGod;
		}
		
		public override bool CanExplode(int i, int j)
		{
			return CalamityWorld.downedStarGod;
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.08f;
            g = 0.04f;
            b = 0.10f;
        }
    }
}