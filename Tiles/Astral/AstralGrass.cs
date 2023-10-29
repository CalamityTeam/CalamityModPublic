using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Astral
{
    public class AstralGrass : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBrick[Type] = true;
			TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Grass"]);

            CalamityUtils.SetMerge(Type, ModContent.TileType<AstralDirt>());
            CalamityUtils.SetMerge(Type, TileID.Grass);
            CalamityUtils.SetMerge(Type, TileID.CorruptGrass);
            CalamityUtils.SetMerge(Type, TileID.HallowedGrass);
            CalamityUtils.SetMerge(Type, TileID.CrimsonGrass);

            DustType = ModContent.DustType<AstralBasic>();
            RegisterItemDrop(ModContent.ItemType<Items.Placeables.AstralDirt>());

            AddMapEntry(new Color(133, 109, 140));

            TileID.Sets.Grass[Type] = true;
            TileID.Sets.Conversion.Grass[Type] = true;

            //Grass framing (<3 terraria devs)
            TileID.Sets.NeedsGrassFraming[Type] = true;
            TileID.Sets.NeedsGrassFramingDirt[Type] = ModContent.TileType<AstralDirt>();
            TileID.Sets.CanBeDugByShovel[Type] = true;
        }
        int animationFrameWidth = 288;

        public override void NumDust(int i, int j, bool fail, ref int Type)
        {
            Type = fail ? 1 : 3;
        }
        public override void RandomUpdate(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            Tile up = Main.tile[i, j - 1];
            Tile up2 = Main.tile[i, j - 2];
            //place Astral Wild Grass
            if (WorldGen.genRand.Next(10) == 0 && !up.HasTile && !up2.HasTile && !(up.LiquidAmount > 0 && up2.LiquidAmount > 0) && !tile.LeftSlope && !tile.RightSlope && !tile.IsHalfBlock)
            {
                up.TileType = (ushort)ModContent.TileType<AstralTallPlants>();
                up.HasTile = true;
                up.TileFrameY = 0;

                //20 different frames, choose a random one
                up.TileFrameX = (short)(WorldGen.genRand.Next(20) * 18);
                WorldGen.SquareTileFrame(i, j - 1, true);

                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendTileSquare(-1, i, j - 1, 3, TileChangeType.None);
                }
            }
            //place Astral Short Grass
            if (WorldGen.genRand.Next(10) == 0 && !up.HasTile && !up2.HasTile && !(up.LiquidAmount > 0 && up2.LiquidAmount > 0) && !tile.LeftSlope && !tile.RightSlope && !tile.IsHalfBlock)
            {
                up.TileType = (ushort)ModContent.TileType<AstralShortPlants>();
                up.HasTile = true;
                up.TileFrameY = 0;

                //23 different frames, choose a random one
                up.TileFrameX = (short)(WorldGen.genRand.Next(23) * 18);
                WorldGen.SquareTileFrame(i, j - 1, true);

                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendTileSquare(-1, i, j - 1, 3, TileChangeType.None);
                }
            }
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
                            uniqueAnimationFrameX = 0;
                            break;
                        case 2:
                            uniqueAnimationFrameX = 1;
                            break;
                        case 3:
                            uniqueAnimationFrameX = 1;
                            break;
                        default:
                            uniqueAnimationFrameX = 0;
                            break;
                    }
                    break;
                case 1:
                    switch (yPos)
                    {
                        case 0:
                            uniqueAnimationFrameX = 1;
                            break;
                        case 1:
                            uniqueAnimationFrameX = 0;
                            break;
                        case 2:
                            uniqueAnimationFrameX = 1;
                            break;
                        case 3:
                            uniqueAnimationFrameX = 1;
                            break;
                        default:
                            uniqueAnimationFrameX = 0;
                            break;
                    }
                    break;
                case 2:
                    switch (yPos)
                    {
                        case 0:
                            uniqueAnimationFrameX = 1;
                            break;
                        case 1:
                            uniqueAnimationFrameX = 0;
                            break;
                        case 2:
                            uniqueAnimationFrameX = 0;
                            break;
                        case 3:
                            uniqueAnimationFrameX = 1;
                            break;
                        default:
                            uniqueAnimationFrameX = 0;
                            break;
                    }
                    break;
                case 3:
                    switch (yPos)
                    {
                        case 0:
                            uniqueAnimationFrameX = 0;
                            break;
                        case 1:
                            uniqueAnimationFrameX = 1;
                            break;
                        case 2:
                            uniqueAnimationFrameX = 0;
                            break;
                        case 3:
                            uniqueAnimationFrameX = 1;
                            break;
                        default:
                            uniqueAnimationFrameX = 0;
                            break;
                    }
                    break;
            }
            frameXOffset = uniqueAnimationFrameX * animationFrameWidth;
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (fail && !effectOnly)
            {
                Main.tile[i, j].TileType = (ushort)ModContent.TileType<AstralDirt>();
            }
        }

        public override bool IsTileBiomeSightable(int i, int j, ref Color sightColor)
        {
            sightColor = Color.Cyan;
            return true;
        }
    }
}
