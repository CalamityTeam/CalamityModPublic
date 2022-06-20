using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using System;
using CalamityMod.Systems;
using CalamityMod.Items.Tools;
using Terraria.GameContent;

namespace CalamityMod.Tiles
{
    public class WulfrumPipes : ModTile, ISpecialTempTileDraw
    {
        public static int PlaceTimeMax = 10;
        //public float PlaceProgress(Point pos) => Math.Clamp((PlaceTimeStart - Main.GameUpdateCount) / (pos.GetTileRNG() * 5f + 5f), 0, 1);
        //public float PlaceProgress(Point pos) => Math.Clamp((PlaceTimeStart - Main.GameUpdateCount) / (pos.GetTileRNG() * 5f + 5f), 0, 1);

        public static Vector2 DisplaceStart(Point pos) => new Vector2(0, -(pos.GetTileRNG() * 10f + 7f));
        public static float RotationStart(Point pos) => - MathHelper.PiOver4 * 0.8f + MathHelper.PiOver4 * 1.6f * pos.GetTileRNG(1);


        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            HitSound = SoundID.Item52;
            DustType = 83;
            AddMapEntry(new Color(128, 90, 77));
            Main.tileLighted[Type] = true;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.00f;
            g = 0.6f;
            b = 0.3f;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = Main.rand.NextBool(3) ? 0 : 1;
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            SoundEngine.PlaySound(SoundID.Item52 with { Volume = SoundID.Item52.Volume * 0.75f, Pitch = SoundID.Item52.Pitch - 0.5f }, new Vector2( i * 16, j * 16 ));
        }
        
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            return false;
        }

        public void CoolDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Point pos = new Point(i, j);
            Tile tile = Main.tile[pos];

            float timeToGo = pos.GetTileRNG(2) * 22;
            float animProgress = (float)Math.Pow(MathHelper.Clamp((TempTilesManagerSystem.GetTemporaryTileTime(pos) - (WulfrumScaffoldKit.TileTime - timeToGo)) / timeToGo, 0, 1), 2);

            Vector2 position = pos.ToWorldCoordinates() + DisplaceStart(pos) * animProgress - Main.screenPosition;
            Rectangle frame = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);

            Color tileColor = Lighting.GetColor(pos) * (1 - animProgress);

            Main.spriteBatch.Draw(TextureAssets.Tile[Type].Value, position, frame, tileColor, RotationStart(pos) * animProgress, frame.Size() / 2f, 1f, 0, 0);
        }
    }
}
