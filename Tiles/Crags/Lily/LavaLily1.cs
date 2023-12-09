using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Crags.Lily
{
    public class LavaLily1 : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileAxe[Type] = true;
            TileObjectData.newTile.Width = 5;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Origin = new Point16(3, 2);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.StyleWrapLimit = 36;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.DrawYOffset = 3;
            TileObjectData.addTile(Type);
            MineResist = 3f;
            AddMapEntry(new Color(153, 100, 176));
            DustType = DustID.PurpleMoss;
            HitSound = SoundID.Grass;
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            type = WorldGen.genRand.NextBool(2) ? DustID.PurpleMoss : DustID.YellowStarDust;
            return true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 10;
        }

        internal static void DrawLilyTop(int i, int j, Texture2D tex, Rectangle? source, Vector2? offset = null, Vector2? origin = null, bool Glow = false)
        {
            Vector2 drawPos = new Vector2(i, j).ToWorldCoordinates() - Main.screenPosition + (offset ?? new Vector2(0, -2));
            Color color = Lighting.GetColor(i, j);

            Main.spriteBatch.Draw(tex, drawPos, source, Glow ? Color.White : color, 0, origin ?? source.Value.Size() / 3f, 1f, SpriteEffects.None, 0f);
        }

        public static Vector2 TileOffset => Lighting.LegacyEngine.Mode > 1 && Main.GameZoomTarget == 1 ? Vector2.Zero : Vector2.One * 12;

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            //draw the glowmask on the lily base
            Tile tile = Framing.GetTileSafely(i, j);
            Texture2D tex = ModContent.Request<Texture2D>("CalamityMod/Tiles/Crags/Lily/LavaLily1Glow").Value;
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

            spriteBatch.Draw(tex, new Vector2(i * 16, j * 16 + 2) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White);

            Texture2D lilyTex = ModContent.Request<Texture2D>("CalamityMod/Tiles/Crags/Lily/LavaLily1Top").Value;
            Texture2D glowTex = ModContent.Request<Texture2D>("CalamityMod/Tiles/Crags/Lily/LavaLily1TopGlow").Value;

            //draw in the middle of the tile so it doesnt draw more than once
            if (Framing.GetTileSafely(i, j).TileFrameX == 36 && Framing.GetTileSafely(i, j).TileFrameY == 18)
            {
                DrawLilyTop(i, j, lilyTex, new Rectangle(0, 0, 178, 184), TileOffset.ToWorldCoordinates(), new Vector2(98, 191), false);
                DrawLilyTop(i, j, glowTex, new Rectangle(0, 0, 178, 184), TileOffset.ToWorldCoordinates(), new Vector2(98, 191), true);
            }
        }
    }
}
