using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader; using CalamityMod.Dusts.Furniture;
using Terraria.ID;

namespace CalamityMod.Tiles.FurnitureAbyss
{
    public class AbyssTorch : ModTile
    {
        public override void SetDefaults()
        {
            CalamityUtils.SetUpTorch(Type, waterImmune: true);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Abyss Torch");
            AddMapEntry(new Color(253, 221, 3), name);
            disableSmartCursor = true;
            drop = ModContent.ItemType<Items.Placeables.FurnitureAbyss.AbyssTorch>();
            adjTiles = new int[] { TileID.Torches };
            torch = true;
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 1, 0f, 0f, 1, new Color(100, 130, 150), 1f);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];
            if (tile.frameX < 66)
            {
                r = 0.9f;
                g = 0.9f;
                b = 0.9f;
            }
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height)
        {
            offsetY = 0;
            if (WorldGen.SolidTile(i, j - 1))
            {
                offsetY = 2;
                if (WorldGen.SolidTile(i - 1, j + 1) || WorldGen.SolidTile(i + 1, j + 1))
                {
                    offsetY = 4;
                }
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            long coordCombo = (long)i << 32 | (long)j;
            ulong randSeed = Main.TileFrameSeed ^ (ulong)coordCombo;
            Color color = new Color(100, 100, 100, 0);
            int frameX = Main.tile[i, j].frameX;
            int frameY = Main.tile[i, j].frameY;
            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
            {
                zero = Vector2.Zero;
            }
            for (int k = 0; k < 7; k++)
            {
                float x = (float)Utils.RandomInt(ref randSeed, -10, 11) * 0.15f;
                float y = (float)Utils.RandomInt(ref randSeed, -10, 1) * 0.35f;
            }
        }
    }
}
