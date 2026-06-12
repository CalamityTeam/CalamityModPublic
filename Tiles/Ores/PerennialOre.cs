
using System;
using CalamityMod.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace CalamityMod.Tiles.Ores
{
    public class PerennialOre : GlowMaskTile
    {
        public const int AnimationFrameWidth = 234;

        public override void SetupStatic()
        {
            Main.tileLighted[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileOreFinderPriority[Type] = 710;
            Main.tileShine[Type] = 2500;
            Main.tileShine2[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);

            TileID.Sets.Ore[Type] = true;
            TileID.Sets.OreMergesWithMud[Type] = true;

            AddMapEntry(new Color(64, 207, 97), CreateMapEntryName());
            MineResist = 2f;
            MinPick = 200;
            HitSound = SoundID.Tink;
            Main.tileSpelunker[Type] = true;

            this.RegisterBlendMergeWith(TileID.Dirt);
            this.RegisterBlendMergeWith(TileID.Stone);
            this.RegisterBlendMergeWith(TileID.Mud);
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void PostTileFrame(int i, int j, int up, int down, int left, int right, int upLeft, int upRight, int downLeft, int downRight)
        {
            var tile = Main.tile[i, j];
            var frameX = tile.TileFrameX;
            var frameY = tile.TileFrameY;

            bool hasFlowerInFrame = false;
            switch (frameX)
            {
                case 0 when frameY == 0:
                    hasFlowerInFrame = true;
                    break;

                case 18 when frameY == 18:
                    hasFlowerInFrame = true;
                    break;

                case 36 when frameY == 0 || frameY == 36:
                    hasFlowerInFrame = true;
                    break;

                case 54 when frameY == 18:
                    hasFlowerInFrame = true;
                    break;
            }

            tile.Get<TileSpecialDrawData>().Flag0 = hasFlowerInFrame;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            // The base green color glow
            r = 0.08f;
            g = 0.2f;
            b = 0.04f;

            // Flower color glow
            if (Main.tile[i, j].Get<TileSpecialDrawData>().Flag0)
            {
                float timeScalar = Main.GameUpdateCount * 0.017f;
                float jDiv14 = j / 14f;
                float iDiv14 = i / 14f;
                float brightness = 0.7f;
                brightness *= (float)MathF.Sin(jDiv14 + timeScalar);
                brightness *= (float)MathF.Sin(iDiv14 + timeScalar);
                brightness += 0.3f;
                float flowerPosBrightnessR = 0.83f * brightness;
                float flowerPosBrightnessG = 0.16f * brightness;
                float flowerPosBrightnessB = 0.31f * brightness;

                // Adjust brightness for flowers
                r = flowerPosBrightnessR;
                g = flowerPosBrightnessG;
                b = flowerPosBrightnessB;
            }
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            frameXOffset = AnimationFrameWidth * TileFramingSystem.GetVariation4x4_012_Low0(i, j);
        }

        public override Color GetGlowMaskColor(int i, int j, TileDrawInfo drawData)
        {
            return Color.White * 0.686f;
        }
    }
}
