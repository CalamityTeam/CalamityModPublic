using CalamityMod.Items.Placeables.Furniture;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Furniture
{
    public class TwinklerInABottleTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpLantern();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Twinkler in a Bottle");
            AddMapEntry(new Color(255, 99, 71), name);
            AnimationFrameHeight = 36;

            TileID.Sets.DisableSmartCursor[Type] = true;
            AdjTiles = new int[] { TileID.HangingLanterns, TileID.FireflyinaBottle, TileID.LightningBuginaBottle };
        }

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

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 32, ModContent.ItemType<TwinklerInABottle>());
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 1f;
            g = 0.39f;
            b = 0.28f;
        }
    }
}
