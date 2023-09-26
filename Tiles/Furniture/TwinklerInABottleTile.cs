using CalamityMod.Items.Placeables.Furniture;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Furniture
{
    public class TwinklerInABottleTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpLantern(ModContent.ItemType<TwinklerInABottle>(), false, false);
            AddMapEntry(new Color(255, 99, 71), CalamityUtils.GetItemName<TwinklerInABottle>());
            AnimationFrameHeight = 36;
            AdjTiles = new int[] { TileID.HangingLanterns, TileID.FireflyinaBottle, TileID.LightningBuginaBottle };
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) => CalamityUtils.PlatformHangOffset(i, j, ref offsetY);

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            int frameAmt = 15;
            int timeNeeded = 6;
            frameCounter++;
            if (frameCounter >= timeNeeded)
            {
                frame++;
                frameCounter = 0;
            }
            if (frame >= frameAmt)
            {
                frame = 0;
            }
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            frameYOffset = this.GetAnimationOffset(i, j, 15, 16, 18, 1, 2, AnimationFrameHeight);
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 13, 0f, 0f, 0, new Color(), 1f);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 1f;
            g = 0.39f;
            b = 0.28f;
        }
    }
}
