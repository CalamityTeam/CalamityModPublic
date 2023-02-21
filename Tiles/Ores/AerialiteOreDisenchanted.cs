using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Tiles.Ores
{
    public class AerialiteOreDisenchanted : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileOreFinderPriority[Type] = 445;
            Main.tileBlockLight[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
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
            ItemDrop = ModContent.ItemType<Items.Placeables.Ores.AerialiteOreDisenchanted>();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Disenchanted Aerialite");
            AddMapEntry(new Color(204, 170, 81), name);
            MineResist = 2f;
            MinPick = 110;
            HitSound = SoundID.Tink;
            Main.tileSpelunker[Type] = true;
        }
        public override void PostSetDefaults()
        {
        Main.tileNoSunLight[Type] = false;
        }

        int animationFrameWidth = 288;

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.CustomMergeFrame(i, j, Type, TileID.Cloud, false, false, false);
            return false;
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
            frameXOffset = uniqueAnimationFrameX * animationFrameWidth;
        }

        private Color GetDrawColour(int i, int j, Color colour)
        {
            int colType = Main.tile[i, j].TileColor;
            Color paintCol = WorldGen.paintColor(colType);
            if (colType >= 13 && colType <= 24)
            {
                colour.R = (byte)(paintCol.R / 255f * colour.R);
                colour.G = (byte)(paintCol.G / 255f * colour.G);
                colour.B = (byte)(paintCol.B / 255f * colour.B);
            }
            return colour;
        }
    }
}
