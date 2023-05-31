using CalamityMod.Items.Placeables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Rubblemaker.Abyss
{
    public class SpiderCoral1Echo : ModTile
    {
        public override string Texture => "CalamityMod/Tiles/Abyss/AbyssAmbient/SpiderCoral1";
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(82, 49, 27));
            DustType = 32;

            RegisterItemDrop(ModContent.ItemType<PyreMantle>());
            FlexibleTileWand.RubblePlacementLarge.AddVariations(ModContent.ItemType<PyreMantle>(), Type, 0);

            base.SetStaticDefaults();
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 190f / 400f;
            g = 48f / 400f;
            b = 30f / 400f;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
			Texture2D tex = ModContent.Request<Texture2D>("CalamityMod/Tiles/Abyss/AbyssAmbient/SpiderCoral1Glow").Value;
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

			spriteBatch.Draw(tex, new Vector2(i * 16, j * 16 + 2) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White);
        }
    }

    public class SpiderCoral2Echo : SpiderCoral1Echo
    {
        public override string Texture => "CalamityMod/Tiles/Abyss/AbyssAmbient/SpiderCoral2";
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
			Texture2D tex = ModContent.Request<Texture2D>("CalamityMod/Tiles/Abyss/AbyssAmbient/SpiderCoral2Glow").Value;
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

			spriteBatch.Draw(tex, new Vector2(i * 16, j * 16 + 2) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White);
        }
    }

    public class SpiderCoral3Echo : SpiderCoral1Echo
    {
        public override string Texture => "CalamityMod/Tiles/Abyss/AbyssAmbient/SpiderCoral3";
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
			Texture2D tex = ModContent.Request<Texture2D>("CalamityMod/Tiles/Abyss/AbyssAmbient/SpiderCoral3Glow").Value;
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

			spriteBatch.Draw(tex, new Vector2(i * 16, j * 16 + 2) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White);
        }
    }

    public class SpiderCoral4Echo : SpiderCoral1Echo
    {
        public override string Texture => "CalamityMod/Tiles/Abyss/AbyssAmbient/SpiderCoral4";
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
			Texture2D tex = ModContent.Request<Texture2D>("CalamityMod/Tiles/Abyss/AbyssAmbient/SpiderCoral4Glow").Value;
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

			spriteBatch.Draw(tex, new Vector2(i * 16, j * 16 + 2) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White);
        }
    }

    public class SpiderCoral5Echo : SpiderCoral1Echo
    {
        public override string Texture => "CalamityMod/Tiles/Abyss/AbyssAmbient/SpiderCoral5";
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
			Texture2D tex = ModContent.Request<Texture2D>("CalamityMod/Tiles/Abyss/AbyssAmbient/SpiderCoral5Glow").Value;
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

			spriteBatch.Draw(tex, new Vector2(i * 16, j * 16 + 2) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White);
        }
    }
}
