using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureAshen
{
    public class AshenChandelier : ModTile
    {
        int animationFrame = 0;

        public override void SetStaticDefaults()
        {
            this.SetUpChandelier(ModContent.ItemType<Items.Placeables.FurnitureAshen.AshenChandelier>(), true);
            AnimationFrameHeight = 54;
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 60, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 1, 0f, 0f, 1, new Color(100, 100, 100), 1f);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter >= 6)
            {
                frame = (frame + 1) % 6;
                animationFrame = frame;
                frameCounter = 0;
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            if (Main.tile[i, j].TileFrameX < 18)
            {
                r = 1f;
                g = 0.5f;
                b = 0.5f;
            }
            else
            {
                r = 0f;
                g = 0f;
                b = 0f;
            }
        }

        public override void HitWire(int i, int j)
        {
            CalamityUtils.LightHitWire(Type, i, j, 3, 3);
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            CalamityUtils.DrawStaticFlameEffect(ModContent.Request<Texture2D>("CalamityMod/Tiles/FurnitureAshen/AshenChandelierFlame").Value, i, j, offsetY: animationFrame * AnimationFrameHeight);
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameY == 18 && tile.TileFrameX < 54)
            {
                CalamityUtils.DrawFlameSparks(60, 5, i, j);
            }
        }
    }
}
