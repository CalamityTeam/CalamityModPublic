using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Abyss.AbyssAmbient
{
    public class BulbTree1 : ModTile
    {
        protected virtual string GlowAsset => "CalamityMod/Tiles/Abyss/AbyssAmbient/BulbTree1Glow";
        internal static FramedGlowMask GlowMask; 

        public override void SetStaticDefaults()
        {
            if (!string.IsNullOrEmpty(GlowAsset))
            {
                GlowMask = new(GlowAsset, 18, 18);
            }

            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Origin = new Point16(1, 2);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Table | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.WaterDeath = false;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(52, 124, 153));
            DustType = 33;

            base.SetStaticDefaults();
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.1f;
            g = 0.32f;
            b = 0.5f;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 2;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

            if (GlowMask is not null && GlowMask.HasContentInFramePos(tile.TileFrameX, tile.TileFrameY))
            {
                Vector2 pos = new Vector2(i * 16, j * 16 + 2) - Main.screenPosition + zero;
                Rectangle frame = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);
                spriteBatch.Draw(GlowMask.Texture, pos, frame, Color.White);
            }
        }
    }

    public class BulbTree2 : BulbTree1
    {
        protected override string GlowAsset => "CalamityMod/Tiles/Abyss/AbyssAmbient/BulbTree3Glow";
    }

    public class BulbTree3 : BulbTree1
    {
        protected override string GlowAsset => "CalamityMod/Tiles/Abyss/AbyssAmbient/BulbTree3Glow";
    }
}
