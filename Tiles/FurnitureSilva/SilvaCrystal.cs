using CalamityMod.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureSilva
{
    public class SilvaCrystal : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;
            soundType = 21;
            mineResist = 2f;
            minPick = 275;
            drop = mod.ItemType("SilvaCrystal");
            AddMapEntry(new Color(49, 100, 99));
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, mod.DustType("SilvaTileGold"), 0f, 0f, 1, new Color(255, 255, 255), 1f);
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 157, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            return CustomTileFraming.BetterGemsparkFraming(i, j, resetFrame);
        }
    }
}
