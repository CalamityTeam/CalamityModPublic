using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CalamityMod.Tiles;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Tiles.Astral;
using CalamityMod.Tiles.AstralDesert;
using CalamityMod.Tiles.AstralSnow;
using CalamityMod.Tiles.Crags;
using CalamityMod.Tiles.FurnitureAbyss;
using CalamityMod.Tiles.FurnitureAshen;
using CalamityMod.Tiles.FurnitureEutrophic;
using CalamityMod.Tiles.FurnitureOtherworldly;
using CalamityMod.Tiles.FurnitureProfaned;
using CalamityMod.Tiles.FurnitureVoid;
using CalamityMod.Tiles.Ores;
using CalamityMod.Tiles.SunkenSea;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod
{
    public static partial class CalamityUtils
    {
        public static string GetMapChestName(string baseName, int x, int y)
        {
            // Bounds check.
            if (!WorldGen.InWorld(x, y, 2))
                return baseName;

            Tile tile = Main.tile[x, y];
            int left = x;
            int top = y;
            if (tile.TileFrameX % 36 != 0)
                left--;
            if (tile.TileFrameY != 0)
                top--;

            int chest = Chest.FindChest(left, top);

            // Valid chest index check.
            if (chest < 0)
                return baseName;

            string name = baseName;

            // Concatenate the chest's custom name if it has one.
            if (!string.IsNullOrEmpty(Main.chest[chest].name))
                name += $": {Main.chest[chest].name}";

            return name;
        }

        public static void SafeSquareTileFrame(int x, int y, bool resetFrame = true)
        {
            for (int xIter = x - 1; xIter <= x + 1; ++xIter)
            {
                if (xIter < 0 || xIter >= Main.maxTilesX)
                    continue;

                for (int yIter = y - 1; yIter <= y + 1; yIter++)
                {
                    if (yIter < 0 || yIter >= Main.maxTilesY)
                        continue;

                    if (xIter == x && yIter == y)
                        WorldGen.TileFrame(x, y, resetFrame, false);
                    else
                        WorldGen.TileFrame(xIter, yIter, false, false);
                }
            }
        }

        public static void LightHitWire(int type, int i, int j, int tileX, int tileY)
        {
            int x = i - Main.tile[i, j].TileFrameX / 18 % tileX;
            int y = j - Main.tile[i, j].TileFrameY / 18 % tileY;
            int tileXX18 = 18 * tileX;
            for (int l = x; l < x + tileX; l++)
            {
                for (int m = y; m < y + tileY; m++)
                {
                    if (Main.tile[l, m].HasTile && Main.tile[l, m].TileType == type)
                    {
                        if (Main.tile[l, m].TileFrameX < tileXX18)
                            Main.tile[l, m].TileFrameX += (short)(tileXX18);
                        else
                            Main.tile[l, m].TileFrameX -= (short)(tileXX18);
                    }
                }
            }

            if (Wiring.running)
            {
                for (int k = 0; k < tileX; k++)
                {
                    for (int l = 0; l < tileY; l++)
                        Wiring.SkipWire(x + k, y + l);
                }
            }
        }

        public static void DrawFlameEffect(Texture2D flameTexture, int i, int j, int offsetX = 0, int offsetY = 0)
        {
            Tile tile = Main.tile[i, j];
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

            int width = 16;
            int height = 16;
            int yOffset = TileObjectData.GetTileData(tile).DrawYOffset;

            ulong randShakeEffect = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (long)(uint)i);
            float drawPositionX = i * 16 - (int)Main.screenPosition.X - (width - 16f) / 2f;
            float drawPositionY = j * 16 - (int)Main.screenPosition.Y;
            for (int c = 0; c < 7; c++)
            {
                float shakeX = Utils.RandomInt(ref randShakeEffect, -10, 11) * 0.15f;
                float shakeY = Utils.RandomInt(ref randShakeEffect, -10, 1) * 0.35f;
                Main.spriteBatch.Draw(flameTexture, new Vector2(drawPositionX + shakeX, drawPositionY + shakeY + yOffset) + zero, new Rectangle(tile.TileFrameX + offsetX, tile.TileFrameY + offsetY, width, height), new Color(100, 100, 100, 0), 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
            }
        }

        public static void DrawStaticFlameEffect(Texture2D flameTexture, int i, int j, int offsetX = 0, int offsetY = 0)
        {
            int xPos = Main.tile[i, j].TileFrameX;
            int yPos = Main.tile[i, j].TileFrameY;
            Color drawColour = new Color(100, 100, 100, 0);
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + zero;
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    Vector2 flameOffset = new Vector2(x, y).SafeNormalize(Vector2.Zero);
                    flameOffset *= 1.5f;
                    Main.spriteBatch.Draw(flameTexture, drawOffset + flameOffset, new Rectangle?(new Rectangle(xPos + offsetX, yPos + offsetY, 18, 18)), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
                }
            }
        }

        public static void DrawFlameSparks(int dustType, int rarity, int i, int j)
        {
            if (!Main.gamePaused && Main.instance.IsActive && (!Lighting.UpdateEveryFrame || Main.rand.NextBool(4)))
            {
                if (Main.rand.NextBool(rarity))
                {
                    int dust = Dust.NewDust(new Vector2(i * 16 + 4, j * 16 + 2), 4, 4, dustType, 0f, 0f, 100, default, 1f);
                    if (!Main.rand.NextBool(3))
                        Main.dust[dust].noGravity = true;

                    // Prevent lag.
                    Main.dust[dust].noLightEmittence = true;

                    Main.dust[dust].velocity *= 0.3f;
                    Main.dust[dust].velocity.Y = Main.dust[dust].velocity.Y - 1.5f;
                }
            }
        }

        public static void DrawItemFlame(Texture2D flameTexture, Item item)
        {
            int width = flameTexture.Width;
            int height = flameTexture.Height;
            float drawPositionX = item.position.X - Main.screenPosition.X + item.width * 0.5f;
            float drawPositionY = item.position.Y - Main.screenPosition.Y + item.height - flameTexture.Height * 0.5f + 2f;
            for (int c = 0; c < 7; c++)
            {
                float shakeX = Main.rand.Next(-10, 11) * 0.15f;
                float shakeY = Main.rand.Next(-10, 1) * 0.35f;
                Main.spriteBatch.Draw(flameTexture, new Vector2(drawPositionX + shakeX, drawPositionY + shakeY), new Rectangle(0, 0, width, height), new Color(100, 100, 100, 0), 0f, default, 1f, SpriteEffects.None, 0f);
            }
        }

        /// <summary>
        /// Generates an framing offset for individual tiles to be animated off-sync.  DOES NOT WORK WITHOUT EVEN ANIMATIONS!!! (e.g. Flak Hermit Cages don't work with this.)
        /// </summary>
        /// <param name="mt">The ModTile which is being initialized.</param>
        /// <param name="i">X position of the tile.</param>
        /// <param name="j">Y position of the tile.</param>
        /// <param name="frameAmt">The number of frames the tile has.</param>
        /// <param name="xLength">The number of pixels of one tile of the furniture in the X direction (ImageHeight / frameAmt / xTiles).</param>
        /// <param name="yLength">The number of pixels of one tile of the furniture in the Y direction (ImageWidth / frameAmt / yTiles).</param>
        /// <param name="xTiles">The number of tiles wide the furniture is.</param>
        /// <param name="yTiles">The number of tiles tall the furniture is.</param>
        /// <param name="animationFrameLength">This is animationFrameHeight in vertical animated tiles and animationFrameWidth in horizontal animated tiles.</param>
        /// <returns>The offset for the animation. This is set to frameXOffset in horizontal animations and frameYOffset in vertical animations in the AnimateIndividualTile() function.</returns>
        internal static int GetAnimationOffset(this ModTile mt, int i, int j, int frameAmt, int xLength, int yLength, int xTiles, int yTiles, int animationFrameLength)
        {
            int frameX = Main.tile[i, j].TileFrameX;
            int frameY = Main.tile[i, j].TileFrameY;

            // Tweak the frame drawn so tiles next to each other are off-sync and look much more interesting.
            frameX %= (xLength * xTiles);
            i -= frameX / xLength;

            frameY %= (yLength * yTiles);
            j -= frameY / yLength;

            int uniqueAnimationFrame = Main.tileFrame[mt.Type] + j;
            if (i % 2 == 0)
                uniqueAnimationFrame += 3;
            if (i % 3 == 0)
                uniqueAnimationFrame += 3;
            if (i % 4 == 0)
                uniqueAnimationFrame += 3;
            if (j % 2 == 0)
                uniqueAnimationFrame += 3;
            if (j % 3 == 0)
                uniqueAnimationFrame += 3;
            if (j % 4 == 0)
                uniqueAnimationFrame += 3;

            uniqueAnimationFrame %= frameAmt;

            return uniqueAnimationFrame * animationFrameLength;
        }

        public static Tile ParanoidTileRetrieval(int x, int y)
        {
            if (!WorldGen.InWorld(x, y))
                return new Tile();

            return Main.tile[x, y];
        }

        public static bool AnySolidTileInSelection(int x, int y, int width, int height)
        {
            for (int i = x; i != x + width; i += Math.Sign(width))
            {
                for (int j = y; j != y + height; j += Math.Sign(height))
                {
                    if (WorldGen.InWorld(i, j))
                        continue;

                    if (WorldGen.SolidTile(Framing.GetTileSafely(i, j)))
                        return true;
                }
            }
            return false;
        }

        public static bool TileSelectionSolid(int x, int y, int width, int height)
        {
            for (int i = x; i != x + width; i += Math.Sign(width))
            {
                for (int j = y; y != y + height; j += Math.Sign(height))
                {
                    if (!WorldGen.InWorld(i, j))
                        return false;

                    if (!WorldGen.SolidTile(Framing.GetTileSafely(i, j)))
                        return false;
                }
            }
            return true;
        }

        public static bool TileSelectionSolidSquare(int x, int y, int width, int height)
        {
            for (int i = x - width; i != x + width; i += Math.Sign(width))
            {
                for (int j = y - height; y != y + height; j += Math.Sign(height))
                {
                    if (!WorldGen.InWorld(i, j))
                        return false;

                    if (!WorldGen.SolidTile(Framing.GetTileSafely(i, j)))
                        return false;
                }
            }
            return true;
        }

        public static bool TileActiveAndOfType(int x, int y, int type)
        {
            return ParanoidTileRetrieval(x, y).HasTile && ParanoidTileRetrieval(x, y).TileType == type;
        }

        /// <summary>
        /// Sets the mergeability state of two tiles. By default, enables tile merging.
        /// </summary>
        /// <param name="type1">The first tile type which should merge (or not).</param>
        /// <param name="type2">The second tile type which should merge (or not).</param>
        /// <param name="merge">The mergeability state of the tiles. Defaults to true if omitted.</param>
        public static void SetMerge(int type1, int type2, bool merge = true)
        {
            if (type1 != type2)
            {
                Main.tileMerge[type1][type2] = merge;
                Main.tileMerge[type2][type1] = merge;
            }
        }

        /// <summary>
        /// Makes the first tile type argument merge with all the other tile type arguments. Also accepts arrays.
        /// </summary>
        /// <param name="myType">The tile whose merging properties will be set.</param>
        /// <param name="otherTypes">Every tile that should be merged with.</param>
        public static void MergeWithSet(int myType, params int[] otherTypes)
        {
            for (int i = 0; i < otherTypes.Length; ++i)
                SetMerge(myType, otherTypes[i]);
        }

        /// <summary>
        /// Makes the specified tile merge with the most common types of tiles found in world generation.<br></br>
        /// Notably excludes Ice.
        /// </summary>
        /// <param name="type">The tile whose merging properties will be set.</param>
        public static void MergeWithGeneral(int type) => MergeWithSet(type, new int[] {
            // Soils
            TileID.Dirt,
            TileID.Mud,
            TileID.ClayBlock,
            // Stones
            TileID.Stone,
            TileID.Ebonstone,
            TileID.Crimstone,
            TileID.Pearlstone,
            // Sands
            TileID.Sand,
            TileID.Ebonsand,
            TileID.Crimsand,
            TileID.Pearlsand,
            // Snows
            TileID.SnowBlock,
            // Calamity Tiles
            TileType<AstralDirt>(),
            TileType<AstralClay>(),
            TileType<AstralStone>(),
            TileType<AstralSand>(),
            TileType<AstralSnow>(),
            TileType<Navystone>(),
            TileType<EutrophicSand>(),
            TileType<SulphurousShale>(),
            TileType<AbyssGravel>(),
            TileType<Voidstone>(),
        });

        /// <summary>
        /// Makes the specified tile merge with all ores, vanilla and Calamity. Particularly useful for stone blocks.
        /// </summary>
        /// <param name="type">The tile whose merging properties will be set.</param>
        public static void MergeWithOres(int type) => MergeWithSet(type, new int[] {
            // Vanilla Ores
            TileID.Copper,
            TileID.Tin,
            TileID.Iron,
            TileID.Lead,
            TileID.Silver,
            TileID.Tungsten,
            TileID.Gold,
            TileID.Platinum,
            TileID.Demonite,
            TileID.Crimtane,
            TileID.Cobalt,
            TileID.Palladium,
            TileID.Mythril,
            TileID.Orichalcum,
            TileID.Adamantite,
            TileID.Titanium,
            TileID.LunarOre,
            // Calamity Ores
            TileType<AerialiteOre>(),
            TileType<CryonicOre>(),
            TileType<PerennialOre>(),
            TileType<InfernalSuevite>(),
            TileType<ScoriaOre>(),
            TileType<AstralOre>(),
            TileType<UelibloomOre>(),
            TileType<AuricOre>(),
        });

        /// <summary>
        /// Makes the specified tile merge with all types of desert tiles, including the Calamity Sunken Sea.
        /// </summary>
        /// <param name="type">The tile whose merging properties will be set.</param>
        public static void MergeWithDesert(int type) => MergeWithSet(type, new int[] {
            // Sands
            TileID.Sand,
            TileID.Ebonsand,
            TileID.Crimsand,
            TileID.Pearlsand,
            // Hardened Sands
            TileID.HardenedSand,
            TileID.CorruptHardenedSand,
            TileID.CrimsonHardenedSand,
            TileID.HallowHardenedSand,
            // Sandstones
            TileID.Sandstone,
            TileID.CorruptSandstone,
            TileID.CrimsonSandstone,
            TileID.HallowSandstone,
            // Miscellaneous Desert Tiles
            TileID.FossilOre,
            TileID.DesertFossil,
            // Astral Desert
            TileType<AstralSand>(),
            TileType<HardenedAstralSand>(),
            TileType<AstralSandstone>(),
            TileType<CelestialRemains>(),
            // Sunken Sea
            TileType<EutrophicSand>(),
            TileType<Navystone>(),
            TileType<SeaPrism>(),
        });

        /// <summary>
        /// Makes the specified tile merge with all types of snow and ice tiles.
        /// </summary>
        /// <param name="type">The tile whose merging properties will be set.</param>
        public static void MergeWithSnow(int type) => MergeWithSet(type, new int[] {
            // Snows
            TileID.SnowBlock,
            // Ices
            TileID.IceBlock,
            TileID.CorruptIce,
            TileID.FleshIce,
            TileID.HallowedIce,
            // Astral Snow
            TileType<AstralIce>(),
            TileType<AstralSnow>(),
            TileType<NovaeSlag>(),
        });

        /// <summary>
        /// Makes the specified tile merge with all tiles which generate in hell. Does not include Infernal Suevite.
        /// </summary>
        /// <param name="type">The tile whose merging properties will be set.</param>
        public static void MergeWithHell(int type) => MergeWithSet(type, new int[] {
            TileID.Ash,
            TileID.Hellstone,
            TileID.ObsidianBrick,
            TileID.HellstoneBrick,
            TileType<BrimstoneSlag>(),
            TileType<BrimstoneSlab>(),
            TileType<ScorchedRemains>(),
            TileType<ScorchedRemainsGrass>(),
        });

        /// <summary>
        /// Makes the specified tile merge with all tiles which generate in the Abyss or the Sulphurous Sea. Includes Chaotic Ore.
        /// </summary>
        /// <param name="type">The tile whose merging properties will be set.</param>
        public static void MergeWithAbyss(int type) => MergeWithSet(type, new int[] {
            // Sulphurous Sea
            TileType<SulphurousSand>(),
            TileType<SulphurousSandstone>(),
            TileType<SulphurousShale>(),
            // Abyss
            TileType<AbyssGravel>(),
            TileType<PyreMantle>(),
            TileType<PyreMantleMolten>(),
            TileType<Voidstone>(),
            TileType<PlantyMush>(),
            TileType<ScoriaOre>(),
        });

        /// <summary>
        /// Makes the tile merge with all the tile types that generate within various types of astral tiles
        /// </summary>
        /// <param name="type"></param>
        public static void MergeAstralTiles(int type)
        {
            //Astral
            SetMerge(type, TileType<AstralDirt>());
            SetMerge(type, TileType<AstralStone>());
            SetMerge(type, TileType<AstralMonolith>());
            SetMerge(type, TileType<AstralClay>());
            //Astral Desert
            SetMerge(type, TileType<AstralSand>());
            SetMerge(type, TileType<HardenedAstralSand>());
            SetMerge(type, TileType<AstralSandstone>());
            SetMerge(type, TileType<CelestialRemains>());
            //Astral Snow
            SetMerge(type, TileType<AstralIce>());
            SetMerge(type, TileType<AstralSnow>());
        }

        /// <summary>
        /// Makes the tile merge with all the decorative 'smooth' tiles
        /// </summary>
        /// <param name="type"></param>
        public static void MergeSmoothTiles(int type)
        {
            //Vanilla
            SetMerge(type, TileID.MarbleBlock);
            SetMerge(type, TileID.GraniteBlock);
            //Calam
            SetMerge(type, TileType<SmoothNavystone>());
            SetMerge(type, TileType<SmoothBrimstoneSlag>());
            SetMerge(type, TileType<SmoothAbyssGravel>());
            SetMerge(type, TileType<SmoothVoidstone>());
        }

        /// <summary>
        /// Makes the tile merge with other mergable decorative tiles
        /// </summary>
        /// <param name="type"></param>
        public static void MergeDecorativeTiles(int type)
        {
            //Vanilla decor
            Main.tileBrick[type] = true;
            //Calam
            SetMerge(type, TileType<CryonicBrick>());
            SetMerge(type, TileType<PerennialBrick>());
            SetMerge(type, TileType<UelibloomBrick>());
            SetMerge(type, TileType<OtherworldlyStone>());
            SetMerge(type, TileType<ProfanedSlab>());
            SetMerge(type, TileType<RunicProfanedBrick>());
            SetMerge(type, TileType<AshenSlab>());
            SetMerge(type, TileType<VoidstoneSlab>());
        }

        /// <summary>
        /// The X position of the Tile
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        public static int X(this Tile tile)
        {
            TilePos(tile, out int x, out _);
            return x;
        }

        /// <summary>
        /// The Y position of the Tile
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        public static int Y(this Tile tile)
        {
            TilePos(tile, out _, out int y);
            return y;
        }

        /// <summary>
        /// Gets the Position of the tile, the same values that would be inputted in Main.tile to get this Tile
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="x">The outputted X value, if you want the X by itself use Tile.X</param>
        /// <param name="y">The outputted Y value, if you want the Y by itself use Tile.Y</param>
        public static void TilePos(this Tile tile, out int x, out int y)
        {
            uint tileId = Unsafe.BitCast<Tile, uint>(tile);
            x = Math.DivRem((int)tileId, Main.tile.Height, out y); //Thanks to FoxXD_ for the help with this
        }

        
        /// <summary>
        /// Determines if a tile is solid ground based on whether it's active and not actuated or if the tile is solid in any way, including just the top.
        /// </summary>
        /// <param name="tile">The tile to check.</param>
        public static bool IsTileSolidGround(this Tile tile) => tile != null && tile.HasUnactuatedTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType]);


        /// <summary>
        /// Determines if a tile is solid based on whether it's active and not actuated or if the tile is solid. This will not count platforms and other non-solid ground tiles
        /// </summary>
        /// <param name="tile">The tile to check.</param>
        public static bool IsTileSolid(this Tile tile) => tile != null && tile.HasUnactuatedTile && Main.tileSolid[tile.TileType] && !TileID.Sets.Platforms[tile.TileType];

        /// <summary>
        /// Determines if a tile is "full" based on if the tile is solid. This will count platforms and actuated tiles but no other non-solid ground tiles.
        /// </summary>
        /// <param name="tile">The tile to check.</param>
        public static bool IsTileFull(this Tile tile) => tile != null && tile.HasTile && Main.tileSolid[tile.TileType];

        /// <summary>
        /// Returns a random number between 0 and 1 that always remains the same based on the tile's coordinates.
        /// </summary>
        /// <param name="tilePos">The tile position to grab the rng from</param>
        /// <param name="shift">An extra offset. Useful if you need multiple counts of rng for the same time</param>
        public static float GetTileRNG(this Point tilePos, int shift = 0) => (float)(Math.Sin(tilePos.X * 17.07947 + shift * 36) + Math.Sin(tilePos.Y * 25.13274)) * 0.25f + 0.5f;

        /// <summary>
        /// Grabs the nearest tile point to the origin, in the specified direction
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Point GetNearestPointInDirection(this Point origin, float direction)
        {
            return origin + new Point((int)Math.Round(Math.Cos(direction)), (int)Math.Round(Math.Sin(direction)));
        }

        /// <summary>
        /// Just like Vector2.ToTileCoordinates, but also clamps the position to the tile grid.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns>The tile coordinates</returns>
        public static Point ToSafeTileCoordinates(this Vector2 vec)
        {
            return new Point((int)MathHelper.Clamp((int)vec.X >> 4, 0, Main.maxTilesX), (int)MathHelper.Clamp((int)vec.Y >> 4, 0, Main.maxTilesY));
        }

        /// <summary>
        /// Is a tile valid to be grappled onto
        /// A straight rip of the private method Projectile.AI_007_GrapplingHooks_CanTileBeLatchedOnTo()
        /// </summary>
        /// <param name="theTile"></param>
        /// <returns>Wether or not the tile may be grappled onto</returns>
        public static bool CanTileBeLatchedOnTo(this Tile theTile, bool grappleOnTrees = false) => Main.tileSolid[theTile.TileType] | (theTile.TileType == 314) | (grappleOnTrees && TileID.Sets.IsATreeTrunk[theTile.TileType]) | (grappleOnTrees && theTile.TileType == 323);

        /// <summary>
        /// Gets the required pickaxe power of a tile, accounting for both the ModTile and the vanilla tile pick requirements
        /// </summary>
        /// <param name="tile"></param>
        /// <returns>The pickaxe power required to break a tile</returns>
        public static int GetRequiredPickPower(this Tile tile, int i, int j)
        {
            int pickReq = 0;

            if (Main.tileNoFail[tile.TileType])
                return pickReq;

            ModTile moddedTile = TileLoader.GetTile(tile.TileType);

            //Getting the pickaxe requirement of a modded tile is shrimple.
            if (moddedTile != null)
                pickReq = moddedTile.MinPick;

            //Getting the pickaxe requirement of a vanilla tile is quite clamplicated
            //This was lifted from code in onyx excavator, which likely was lifted from vanilla. It might need 1.4 updating.
            else
            {
                switch (tile.TileType)
                {
                    case TileID.Chlorophyte:
                        pickReq = 200;
                        break;
                    case TileID.Ebonstone:
                    case TileID.Crimstone:
                    case TileID.Pearlstone:
                    case TileID.Hellstone:
                        pickReq = 65;
                        break;
                    case TileID.Obsidian:
                        pickReq = 55;
                        break;
                    case TileID.Meteorite:
                        pickReq = 50;
                        break;
                    case TileID.Demonite:
                    case TileID.Crimtane:
                        if (j > Main.worldSurface)
                            pickReq = 55;
                        break;
                    case TileID.LihzahrdBrick:
                    case TileID.LihzahrdAltar:
                        pickReq = 210;
                        break;
                    case TileID.Cobalt:
                    case TileID.Palladium:
                        pickReq = 100;
                        break;
                    case TileID.Mythril:
                    case TileID.Orichalcum:
                        pickReq = 110;
                        break;
                    case TileID.Adamantite:
                    case TileID.Titanium:
                        pickReq = 150;
                        break;
                    default:
                        break;
                }
            }

            if (Main.tileDungeon[tile.TileType] && j > Main.worldSurface)
                pickReq = 100;

            return pickReq;
        }

        /// <summary>
        /// Returns if a tile is safe to be mined in terms of it being "important"
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="ignoreAbyss">If abyss terrain blocks should be considered unsafe to mine</param>
        /// <returns></returns>
        public static bool ShouldBeMined(this Tile tile, bool ignoreAbyss = true)
        {
            List<int> tileExcludeList = new List<int>()
            {
                TileID.DemonAltar, TileID.ElderCrystalStand, TileID.LihzahrdAltar, TileID.Dressers, TileID.Containers
            };

            if (ignoreAbyss)
            {
                tileExcludeList.Add(ModContent.TileType<AbyssGravel>());
                tileExcludeList.Add(ModContent.TileType<PyreMantle>());
                tileExcludeList.Add(ModContent.TileType<PyreMantleMolten>());
                tileExcludeList.Add(ModContent.TileType<Voidstone>());
            }

            return !Main.tileContainer[tile.TileType] && !tileExcludeList.Contains(tile.TileType);
        }
    }
}
