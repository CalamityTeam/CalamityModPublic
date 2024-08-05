using System;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Abyss.AbyssAmbient
{
    public class SpiderCoral1 : ModTile
    {
        protected virtual string GlowAsset => "CalamityMod/Tiles/Abyss/AbyssAmbient/SpiderCoral1Glow";
        internal static FramedGlowMask GlowMask;

        public override void SetStaticDefaults()
        {
            if (!string.IsNullOrEmpty(GlowAsset))
            {
                GlowMask = new(GlowAsset, 18, 18);
            }

            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(82, 49, 27));
            DustType = 32;

            base.SetStaticDefaults();
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.48f;
            g = 0.12f;
            b = 0.08f;
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

    public class SpiderCoral2 : SpiderCoral1
    {
        protected override string GlowAsset => "CalamityMod/Tiles/Abyss/AbyssAmbient/SpiderCoral2Glow";
    }

    public class SpiderCoral3 : SpiderCoral1
    {
        protected override string GlowAsset => "CalamityMod/Tiles/Abyss/AbyssAmbient/SpiderCoral3Glow";
    }

    public class SpiderCoral4 : SpiderCoral1
    {
        protected override string GlowAsset => "CalamityMod/Tiles/Abyss/AbyssAmbient/SpiderCoral4Glow";
    }

    public class SpiderCoral5 : SpiderCoral1
    {
        protected override string GlowAsset => "CalamityMod/Tiles/Abyss/AbyssAmbient/SpiderCoral5Glow";
    }
}
