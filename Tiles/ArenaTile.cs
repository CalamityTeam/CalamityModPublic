using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.NPCs;

namespace CalamityMod.Tiles
{
	public class ArenaTile : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;
			ModTranslation name = CreateMapEntryName();
 			name.SetDefault("Arena");
 			AddMapEntry(new Color(128, 0, 0), name);
		}

        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
            return false;
        }

        public override bool CanExplode(int i, int j)
		{
			return false;
		}

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = (float)Main.DiscoR / 255f;
            g = 0f;
            b = 0f;
        }
    }
}