using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;
using Terraria.ObjectData;

namespace CalamityMod.Tiles
{
    public class AstralBar : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileShine[Type] = 1000;
            Main.tileSolid[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            dustType = ModContent.DustType<AstralBlue>();
            drop = ModContent.ItemType<AstralBar>();

            AddMapEntry(new Color(47, 66, 90));
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            if (Main.rand.NextBool(2))
            {
                type = ModContent.DustType<AstralOrange>();
            }
            else
            {
                type = ModContent.DustType<AstralBlue>();
            }
            return true;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            WorldGen.Check1x1(i, j, Type);
            return true;
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            Main.tile[i, j].frameX = 0;
            Main.tile[i, j].frameY = 0;
        }
    }
}
