using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Ores
{
    public class AerialiteOreDisenchanted : ModTile
    {
        private const int AnimationFrameWidth = 234;

        public byte[,] tileAdjacency;
        public byte[,] secondTileAdjacency;
        public byte[,] thirdTileAdjacency;
        public byte[,] fourthTileAdjacency;

        public override void SetStaticDefaults()
        {
            Main.tileOreFinderPriority[Type] = 445;
            Main.tileBlockLight[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileLighted[Type] = true;
            //Main.tileNoSunLight[Type] = false;

            TileID.Sets.Ore[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.SetMerge(Type, ModContent.TileType<AerialiteOre>());
            CalamityUtils.SetMerge(Type, TileID.Cloud);
            CalamityUtils.SetMerge(Type, TileID.RainCloud);
            CalamityUtils.SetMerge(Type, TileID.SnowCloud);
            //Main.tileMerge[TileID.Cloud][ModContent.TileType<AerialiteOre>()] = true;

            //Main.tileShine[Type] = 3500;
            Main.tileShine2[Type] = false;

            TileID.Sets.ChecksForMerge[Type] = true;
            DustType = 33;
            AddMapEntry(new Color(204, 170, 81), CreateMapEntryName());
            MinPick = 110;
            HitSound = SoundID.Tink;
            Main.tileSpelunker[Type] = true;

            TileFraming.SetUpUniversalMerge(Type, TileID.Cloud, out tileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, TileID.RainCloud, out secondTileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, TileID.SnowCloud, out thirdTileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, TileID.Dirt, out fourthTileAdjacency);
        }
        public override void PostSetDefaults()
        {
            Main.tileNoSunLight[Type] = false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.GetAdjacencyData(i, j, TileID.Cloud, out tileAdjacency[i, j]);
            TileFraming.GetAdjacencyData(i, j, TileID.RainCloud, out secondTileAdjacency[i, j]);
            TileFraming.GetAdjacencyData(i, j, TileID.SnowCloud, out thirdTileAdjacency[i, j]);
            TileFraming.GetAdjacencyData(i, j, TileID.Dirt, out fourthTileAdjacency[i, j]);
            return true;
        }
        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            int uniqueAnimationFrameX = 0;
            int xPos = i % 4;
            int yPos = j % 4;
            switch (xPos)
            {
                case 0:
                    switch (yPos)
                    {
                        case 0:
                            uniqueAnimationFrameX = 0;
                            break;
                        case 1:
                            uniqueAnimationFrameX = 2;
                            break;
                        case 2:
                            uniqueAnimationFrameX = 1;
                            break;
                        case 3:
                            uniqueAnimationFrameX = 2;
                            break;
                        default:
                            uniqueAnimationFrameX = 2;
                            break;
                    }
                    break;
                case 1:
                    switch (yPos)
                    {
                        case 0:
                            uniqueAnimationFrameX = 2;
                            break;
                        case 1:
                            uniqueAnimationFrameX = 0;
                            break;
                        case 2:
                            uniqueAnimationFrameX = 2;
                            break;
                        case 3:
                            uniqueAnimationFrameX = 2;
                            break;
                        default:
                            uniqueAnimationFrameX = 2;
                            break;
                    }
                    break;
                case 2:
                    switch (yPos)
                    {
                        case 0:
                            uniqueAnimationFrameX = 2;
                            break;
                        case 1:
                            uniqueAnimationFrameX = 0;
                            break;
                        case 2:
                            uniqueAnimationFrameX = 1;
                            break;
                        case 3:
                            uniqueAnimationFrameX = 2;
                            break;
                        default:
                            uniqueAnimationFrameX = 2;
                            break;
                    }
                    break;
                case 3:
                    switch (yPos)
                    {
                        case 0:
                            uniqueAnimationFrameX = 1;
                            break;
                        case 1:
                            uniqueAnimationFrameX = 2;
                            break;
                        case 2:
                            uniqueAnimationFrameX = 0;
                            break;
                        case 3:
                            uniqueAnimationFrameX = 2;
                            break;
                        default:
                            uniqueAnimationFrameX = 2;
                            break;
                    }
                    break;
            }
            frameXOffset = uniqueAnimationFrameX * AnimationFrameWidth;
        }
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            TileFraming.DrawUniversalMergeFrames(i, j, tileAdjacency, "CalamityMod/Tiles/Merges/CloudMerge");
            TileFraming.DrawUniversalMergeFrames(i, j, secondTileAdjacency, "CalamityMod/Tiles/Merges/RainCloudMerge");
            TileFraming.DrawUniversalMergeFrames(i, j, thirdTileAdjacency, "CalamityMod/Tiles/Merges/SnowCloudMerge");
            TileFraming.DrawUniversalMergeFrames(i, j, fourthTileAdjacency, "CalamityMod/Tiles/Merges/DirtMerge");
        }
    }
}
