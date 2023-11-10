using CalamityMod.Schematics;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Tiles.Astral;
using CalamityMod.Tiles.AstralDesert;
using CalamityMod.Tiles.AstralSnow;
using CalamityMod.Tiles.Ores;
using CalamityMod.Walls;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace CalamityMod.World
{
    public class AstralBiome
    {
        public static int YStart { get; set; } = (int)Main.worldSurface;

        public static readonly SoundStyle MeteorSound = new("CalamityMod/Sounds/Custom/AstralStarFall");
        public static bool CanAstralMeteorSpawn()
        {
            int astralOreCount = 0;
            float worldSizeFactor = Main.maxTilesX / 4200f; // Small = 4200, Medium = 6400, Large = 8400
            int astralOreAllowed = (int)(200f * worldSizeFactor); // Small = 201 Medium = 305 Large = 401
            for (int x = 5; x < Main.maxTilesX - 5; x++)
            {
                int y = 5;
                while (y < Main.worldSurface)
                {
                    if (Main.tile[x, y].HasTile && Main.tile[x, y].TileType == ModContent.TileType<AstralOre>())
                    {
                        astralOreCount++;
                        if (astralOreCount > astralOreAllowed)
                            return false;
                    }
                    y++;
                }
            }
            return true;
        }

        public static bool CanAstralBiomeSpawn()
        {
            int astralTileCount = 0;
            float worldSizeFactor = Main.maxTilesX / 4200f; // Small = 4200, Medium = 6400, Large = 8400
            int astralTilesAllowed = (int)(400f * worldSizeFactor); // Small = 401 Medium = 605 Large = 801
            for (int x = 5; x < Main.maxTilesX - 5; x++)
            {
                int y = 5;
                while (y < Main.worldSurface)
                {
                    if (Main.tile[x, y].HasTile &&
                    (Main.tile[x, y].TileType == ModContent.TileType<AstralSand>() || Main.tile[x, y].TileType == ModContent.TileType<AstralSandstone>() ||
                    Main.tile[x, y].TileType == ModContent.TileType<HardenedAstralSand>() || Main.tile[x, y].TileType == ModContent.TileType<AstralIce>() ||
                    Main.tile[x, y].TileType == ModContent.TileType<AstralDirt>() || Main.tile[x, y].TileType == ModContent.TileType<AstralStone>() ||
                    Main.tile[x, y].TileType == ModContent.TileType<AstralGrass>() || Main.tile[x, y].TileType == ModContent.TileType<NovaeSlag>() ||
                    Main.tile[x, y].TileType == ModContent.TileType<CelestialRemains>() || Main.tile[x, y].TileType == ModContent.TileType<AstralSnow>() ||
                    Main.tile[x, y].TileType == ModContent.TileType<AstralClay>() || Main.tile[x, y].TileType == ModContent.TileType<AstralStone>()))
                    {
                        astralTileCount++;
                        if (astralTileCount > astralTilesAllowed)
                            return false;
                    }
                    y++;
                }
            }
            return true;
        }

        public static void PlaceAstralMeteor()
        {
            Mod ancientsAwakened = CalamityMod.Instance.ancientsAwakened;

            // This flag is also used to determine whether players are nearby.
            bool meteorDropped = true;

            // Clients in multiplayer don't drop meteors.
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            for (int i = 0; i < 255; i++)
            {
                if (Main.player[i].active)
                {
                    meteorDropped = false;
                    break;
                }
            }

            // Check whether there is already too much ore.
            if (!CanAstralMeteorSpawn())
                return;

            UnifiedRandom rand = WorldGen.genRand;
            float solidTileRequirement = 600f;
            bool localAbyssSide = GenVars.dungeonX < Main.maxTilesX / 2;

            // Pre-cache a list of Ancients Awakened tiles to avoid, for performance reasons
            IList<ushort> aaTilesToAvoid = new List<ushort>(16);
            if (ancientsAwakened is not null)
            {
                string[] aaTileNames = new string[]
                {
                    "InfernoGrass",
                    "Torchstone",
                    "Torchsand",
                    "Torchsandstone",
                    "Torchsandhardened",
                    "Torchice",
                    "Depthstone",
                    "Depthsand",
                    "Depthsandstone",
                    "Depthsandhardened",
                    "Depthice",
                };
                foreach (string tileName in aaTileNames)
                    aaTilesToAvoid.Add(ancientsAwakened.Find<ModTile>(tileName).Type);
            }

            while (!meteorDropped)
            {
                float worldEdgeMargin = (float)Main.maxTilesX * 0.08f;
                int xLimit = Main.maxTilesX / 2;

                int realX = Abyss.AtLeftSideOfWorld ? rand.Next(SulphurousSea.BiomeWidth + 400, xLimit - 400) : rand.Next(xLimit + 400, Main.maxTilesX - SulphurousSea.BiomeWidth - 400);

                //clamp so it doesnt crash hopefully
                int x = Utils.Clamp(realX, SulphurousSea.BiomeWidth + 400, Main.maxTilesX - SulphurousSea.BiomeWidth - 400);

                //world surface = 920 large 740 medium 560 small
                int y = (int)(Main.worldSurface * 0.5); //Large = 522, Medium = 444, Small = 336
                while (y < Main.maxTilesY)
                {
                    //check to place the astral meteor on valid tiles, place automatically on ebonstone walls, and avoid platforms
                    if (((Main.tile[x, y].HasTile && Main.tileSolid[(int)Main.tile[x, y].TileType]) || Main.tile[x, y].WallType == 3) && !TileID.Sets.Platforms[Main.tile[x, y].TileType])
                    {
                        int suitableTiles = 0;
                        int checkRadius = 15;
                        for (int l = x - checkRadius; l < x + checkRadius; l++)
                        {
                            for (int m = y - checkRadius; m < y + checkRadius; m++)
                            {
                                if (WorldGen.SolidTile(l, m))
                                {
                                    suitableTiles++;

                                    //Avoid floating islands: Clouds and Sunplate both harshly punish attempted meteor spawns
                                    if (Main.tile[l, m].TileType == TileID.Cloud || Main.tile[l, m].TileType == TileID.Sunplate)
                                    {
                                        suitableTiles -= 100;
                                    }
                                    //Avoid the hallowed so it doesnt get demolished by the astral biome
                                    else if (Main.tile[l, m].TileType == TileID.HallowedGrass || Main.tile[l, m].TileType == TileID.Pearlstone)
                                    {
                                        suitableTiles -= 100;
                                    }
                                    //Avoid living trees, doesnt land on them too often but its better to prevent it
                                    else if (Main.tile[l, m].TileType == TileID.LivingWood || Main.tile[l, m].TileType == TileID.LeafBlock)
                                    {
                                        suitableTiles -= 100;
                                    }
                                    //Avoid Sulphurous Sea beach: Cannot be converted by astral
                                    else if (Main.tile[l, m].TileType == ModContent.TileType<SulphurousSand>() || Main.tile[l, m].TileType == ModContent.TileType<SulphurousSandstone>())
                                    {
                                        suitableTiles -= 100;
                                    }
                                    //Prevent the Astral biome from overriding or interfering with an AA biome
                                    else if (ancientsAwakened is not null && aaTilesToAvoid.Contains(Main.tile[l, m].TileType))
                                    {
                                        suitableTiles -= 100;
                                    }
                                }

                                // Liquid aversion makes meteors less likely to fall in lakes
                                else if (Main.tile[l, m].LiquidAmount > 0)
                                {
                                    suitableTiles--;
                                }
                            }
                        }

                        if ((float)suitableTiles < solidTileRequirement)
                        {
                            solidTileRequirement -= 0.5f;
                            break;
                        }
                        meteorDropped = GenerateAstralMeteor(x, y);

                        // If the meteor actually dropped, post the message stating as such.
                        if (meteorDropped)
                        {
                            string key = "Mods.CalamityMod.Status.Progression.AstralText";
                            Color messageColor = Color.Gold;

                            CalamityUtils.DisplayLocalizedText(key, messageColor);
                            break;
                        }
                        break;
                    }
                    else
                    {
                        y++;
                    }
                }
                if (solidTileRequirement < 100f)
                {
                    return;
                }
            }
        }

        public static bool GenerateAstralMeteor(int i, int j)
        {
            WorldGen.gen = true;

            // Pre-cache a list of Magic Storage tiles to avoid, for performance reasons
            // It is plausible that only StorageComponent and StorageConnector are needed, but I aint gonna risk corrupting worlds
            // or crashes as containers can do some serious shit as seen with the Abyss chests - Shade
            Mod magicStorage = CalamityMod.Instance.magicStorage;
            IList<ushort> MSTilesToAvoid = new List<ushort>(16);
            if (magicStorage is not null)
            {
                string[] MSTileNames = new string[]
                {
                    "CraftingAccess",
                    "CreativeStorageUnit",
                    "EnvironmentAccess",
                    "RemoteAccess",
                    "StorageAccess",
                    "StorageComponent",
                    "StorageConnector",
                    "StorageHeart",
                    "StorageUnit",
                };
                foreach (string tileName in MSTileNames)
                    MSTilesToAvoid.Add(magicStorage.Find<ModTile>(tileName).Type);
            }

            UnifiedRandom rand = WorldGen.genRand;
            if (i < 50 || i > Main.maxTilesX - 50)
            {
                return false;
            }
            // Avoid the dungeon so that the beacon doesn't eat it.
            if (Math.Abs(i - GenVars.dungeonX) < 65)
            {
                return false;
            }
            if (j < 50 || j > Main.maxTilesY - 50)
            {
                return false;
            }
            int avoidRectangleSize = 35;
            Rectangle rectangle = new Rectangle((i - avoidRectangleSize) * 16, (j - avoidRectangleSize) * 16, avoidRectangleSize * 2 * 16, avoidRectangleSize * 2 * 16);
            for (int k = 0; k < Main.maxPlayers; k++)
            {
                if (Main.player[k].active)
                {
                    Rectangle value = new Rectangle((int)(Main.player[k].position.X + (float)(Main.player[k].width / 2) - (float)(NPC.sWidth / 2) - (float)NPC.safeRangeX), (int)(Main.player[k].position.Y + (float)(Main.player[k].height / 2) - (float)(NPC.sHeight / 2) - (float)NPC.safeRangeY), NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
                    if (rectangle.Intersects(value))
                    {
                        return false;
                    }
                }
            }
            for (int l = 0; l < Main.maxNPCs; l++)
            {
                if (Main.npc[l].active)
                {
                    Rectangle value2 = new Rectangle((int)Main.npc[l].position.X, (int)Main.npc[l].position.Y, Main.npc[l].width, Main.npc[l].height);
                    if (rectangle.Intersects(value2))
                    {
                        return false;
                    }
                }
            }
            for (int m = i - avoidRectangleSize; m < i + avoidRectangleSize; m++)
            {
                for (int n = j - avoidRectangleSize; n < j + avoidRectangleSize; n++)
                {
                    if (Main.tile[m, n].HasTile && Main.tile[m, n].TileType == 21)
                    {
                        return false;
                    }
                }
            }
            avoidRectangleSize = rand.Next(17, 23);
            for (int inc = i - avoidRectangleSize; inc < i + avoidRectangleSize; inc++)
            {
                for (int doubleinc = j - avoidRectangleSize; doubleinc < j + avoidRectangleSize; doubleinc++)
                {
                    if (doubleinc > j + rand.Next(-2, 3) - 5)
                    {
                        float tileXDist = (float)Math.Abs(i - inc);
                        float tileYDist = (float)Math.Abs(j - doubleinc);
                        float tileDistance = (float)Math.Sqrt((double)(tileXDist * tileXDist + tileYDist * tileYDist));
                        if ((double)tileDistance < (double)avoidRectangleSize * 0.9 + (double)rand.Next(-4, 5))
                        {
                            if (Main.tile[inc, doubleinc] != null)
                            {
                                if (!Main.tileSolid[(int)Main.tile[inc, doubleinc].TileType])
                                {
                                    Main.tile[inc, doubleinc].Get<TileWallWireStateData>().HasTile = false;
                                }
                                Main.tile[inc, doubleinc].TileType = (ushort)ModContent.TileType<AstralOre>();
                            }
                        }
                    }
                }
            }
            avoidRectangleSize = WorldGen.genRand.Next(8, 14);
            for (int inc2 = i - avoidRectangleSize; inc2 < i + avoidRectangleSize; inc2++)
            {
                for (int doubleinc2 = j - avoidRectangleSize; doubleinc2 < j + avoidRectangleSize; doubleinc2++)
                {
                    if (doubleinc2 > j + rand.Next(-2, 3) - 4)
                    {
                        float tileXDist2 = (float)Math.Abs(i - inc2);
                        float tileYDist2 = (float)Math.Abs(j - doubleinc2);
                        float tileDistance2 = (float)Math.Sqrt((double)(tileXDist2 * tileXDist2 + tileYDist2 * tileYDist2));
                        if ((double)tileDistance2 < (double)avoidRectangleSize * 0.8 + (double)rand.Next(-3, 4))
                        {
                            if (Main.tile[inc2, doubleinc2] != null)
                                Main.tile[inc2, doubleinc2].Get<TileWallWireStateData>().HasTile = false;
                        }
                    }
                }
            }
            avoidRectangleSize = WorldGen.genRand.Next(25, 35);
            for (int inc3 = i - avoidRectangleSize; inc3 < i + avoidRectangleSize; inc3++)
            {
                for (int doubleinc3 = j - avoidRectangleSize; doubleinc3 < j + avoidRectangleSize; doubleinc3++)
                {
                    float tileXDist3 = (float)Math.Abs(i - inc3);
                    float tileYDist3 = (float)Math.Abs(j - doubleinc3);
                    float tileDistance3 = (float)Math.Sqrt((double)(tileXDist3 * tileXDist3 + tileYDist3 * tileYDist3));
                    if (Main.tile[inc3, doubleinc3] != null)
                    {
                        if ((double)tileDistance3 < (double)avoidRectangleSize * 0.7)
                        {
                            if (Main.tile[inc3, doubleinc3].TileType == 5 || Main.tile[inc3, doubleinc3].TileType == 32 || Main.tile[inc3, doubleinc3].TileType == 352)
                            {
                                try
                                { WorldGen.KillTile(inc3, doubleinc3, false, false, true); }
                                catch (NullReferenceException)
                                { }
                            }
                            Main.tile[inc3, doubleinc3].LiquidAmount = 0;
                        }
                        if (Main.tile[inc3, doubleinc3].TileType == (ushort)ModContent.TileType<AstralOre>())
                        {
                            if (!WorldGen.SolidTile(inc3 - 1, doubleinc3) && !WorldGen.SolidTile(inc3 + 1, doubleinc3) && !WorldGen.SolidTile(inc3, doubleinc3 - 1) && !WorldGen.SolidTile(inc3, doubleinc3 + 1))
                            {
                                Main.tile[inc3, doubleinc3].Get<TileWallWireStateData>().HasTile = false;
                            }
                            else if ((Main.tile[inc3, doubleinc3].IsHalfBlock || Main.tile[inc3 - 1, doubleinc3].TopSlope) && !WorldGen.SolidTile(inc3, doubleinc3 + 1))
                            {
                                Main.tile[inc3, doubleinc3].Get<TileWallWireStateData>().HasTile = false;
                            }
                        }
                        WorldGen.SquareTileFrame(inc3, doubleinc3, true);
                        WorldGen.SquareWallFrame(inc3, doubleinc3, true);
                    }
                }
            }
            avoidRectangleSize = WorldGen.genRand.Next(23, 32);
            for (int inc4 = i - avoidRectangleSize; inc4 < i + avoidRectangleSize; inc4++)
            {
                for (int doubleinc4 = j - avoidRectangleSize; doubleinc4 < j + avoidRectangleSize; doubleinc4++)
                {
                    if (doubleinc4 > j + WorldGen.genRand.Next(-3, 4) - 3 && Main.tile[inc4, doubleinc4].HasTile && rand.NextBool(10))
                    {
                        float tileXDist4 = (float)Math.Abs(i - inc4);
                        float tileYDist4 = (float)Math.Abs(j - doubleinc4);
                        float tileDistance4 = (float)Math.Sqrt((double)(tileXDist4 * tileXDist4 + tileYDist4 * tileYDist4));
                        if ((double)tileDistance4 < (double)avoidRectangleSize * 0.8)
                        {
                            if (Main.tile[inc4, doubleinc4] != null)
                            {
                                if (Main.tile[inc4, doubleinc4].TileType == 5 || Main.tile[inc4, doubleinc4].TileType == 32 || Main.tile[inc4, doubleinc4].TileType == 352)
                                {
                                    WorldGen.KillTile(inc4, doubleinc4, false, false, false);
                                }
                                Main.tile[inc4, doubleinc4].TileType = (ushort)ModContent.TileType<AstralOre>();
                                WorldGen.SquareTileFrame(inc4, doubleinc4, true);
                            }
                        }
                    }
                }
            }
            avoidRectangleSize = WorldGen.genRand.Next(30, 38);
            for (int inc5 = i - avoidRectangleSize; inc5 < i + avoidRectangleSize; inc5++)
            {
                for (int doubleinc5 = j - avoidRectangleSize; doubleinc5 < j + avoidRectangleSize; doubleinc5++)
                {
                    if (doubleinc5 > j + WorldGen.genRand.Next(-2, 3) && Main.tile[inc5, doubleinc5].HasTile && rand.NextBool(20))
                    {
                        float tileXDist5 = (float)Math.Abs(i - inc5);
                        float tileYDist5 = (float)Math.Abs(j - doubleinc5);
                        float tileDistance5 = (float)Math.Sqrt((double)(tileXDist5 * tileXDist5 + tileYDist5 * tileYDist5));
                        if ((double)tileDistance5 < (double)avoidRectangleSize * 0.85)
                        {
                            if (Main.tile[inc5, doubleinc5] != null)
                            {
                                if (Main.tile[inc5, doubleinc5].TileType == 5 || Main.tile[inc5, doubleinc5].TileType == 32 || Main.tile[inc5, doubleinc5].TileType == 352)
                                {
                                    WorldGen.KillTile(inc5, doubleinc5, false, false, false);
                                }
                                Main.tile[inc5, doubleinc5].TileType = (ushort)ModContent.TileType<AstralOre>();
                                WorldGen.SquareTileFrame(inc5, doubleinc5, true);
                            }
                        }
                    }
                }
            }

            WorldGen.gen = false;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(-1, i, j, 40, TileChangeType.None);
                if (CanAstralBiomeSpawn())
                {
                    if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active)
                        SoundEngine.PlaySound(MeteorSound, Main.player[Main.myPlayer].position);

                    // Immediately prior to mass-converting blocks to Astral, write down the Y position of this event.
                    YStart = j;
                    DoAstralConversion(new Point(i, j));

                    // Upward checks go up 180 tiles. If for whatever reason the placement Y position
                    // would cause this upward movement to go outside of the world, clamp it to prevent index problems.
                    if (j < 181)
                        j = 181;

                    int xOffset = GenVars.dungeonX < Main.maxTilesX / 2 ? WorldGen.genRand.Next(-80, -40) : WorldGen.genRand.Next(40, 80);

                    bool altarPlaced = false;
                    while (!altarPlaced)
                    {
                        WorldGen.gen = true;

                        int x = i + xOffset;
                        int y = j - 100;

                        while (!WorldGen.SolidTile(x, y) && y <= Main.worldSurface)
                        {
                            y += 5;
                        }

                        if (Main.tile[x, y].HasTile || Main.tile[x, y].WallType > 0 && (!CalamityUtils.TileActiveAndOfType(x, y, TileID.Torches) ||
                            !CalamityUtils.TileActiveAndOfType(x, y, TileID.Containers) || !CalamityUtils.TileActiveAndOfType(x, y, TileID.Containers2)
                            || (magicStorage is not null && MSTilesToAvoid.Contains(Main.tile[x, y].TileType)))) //AVOID HOUSES
                        {
                            bool place = true;
                            SchematicManager.PlaceSchematic<Action<Chest>>(SchematicManager.AstralBeaconKey, new Point(x, y - 5), SchematicAnchor.Center, ref place);

                            WorldGen.gen = false;

                            altarPlaced = true;
                        }
                    }
                }
            }

            return true;
        }

        public static void DoAstralConversion(object obj)
        {
            //Pre-calculate all variables necessary for elliptical area checking
            Point origin = (Point)obj;
            Vector2 center = origin.ToVector2() * 16f + new Vector2(8f);

            float angle = MathHelper.Pi * 0.15f;
            float otherAngle = MathHelper.PiOver2 - angle;

            int distanceInTiles = 150 + (Main.maxTilesX - 4200) / 4200 * 200;
            float distance = distanceInTiles * 16f;
            float constant = distance * 2f / (float)Math.Sin(angle);

            float fociSpacing = distance * (float)Math.Sin(otherAngle) / (float)Math.Sin(angle);
            int verticalRadius = (int)(constant / 16f);

            Vector2 fociOffset = Vector2.UnitY * fociSpacing;
            Vector2 topFoci = center - fociOffset;
            Vector2 bottomFoci = center + fociOffset;

            UnifiedRandom rand = WorldGen.genRand;
            for (int x = origin.X - distanceInTiles - 2; x <= origin.X + distanceInTiles + 2; x++)
            {
                for (int y = (int)(origin.Y - verticalRadius * 0.4f) - 3; y <= origin.Y + verticalRadius + 3; y++)
                {
                    if (CheckInEllipse(new Point(x, y), topFoci, bottomFoci, constant, center, out float dist, y < origin.Y))
                    {
                        //If we're in the outer blurPercent% of the ellipse
                        float percent = dist / constant;
                        float blurPercent = 0.98f;
                        if (percent > blurPercent)
                        {
                            float outerEdgePercent = (percent - blurPercent) / (1f - blurPercent);
                            if (rand.NextFloat(1f) > outerEdgePercent)
                            {
                                ConvertToAstral(x, y, true);
                            }
                        }
                        else
                        {
                            ConvertToAstral(x, y, true);
                        }
                    }
                }
            }
        }

        public static void ConvertToAstral(int startX, int endX, int startY, int endY, bool convertOre = false)
        {
            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    ConvertToAstral(x, y, convertOre);
                }
            }
        }

        public static void ConvertToAstral(int x, int y, bool convertOre = false)
        {
            if (WorldGen.InWorld(x, y, 1))
            {
                int type = Main.tile[x, y].TileType;
                int wallType = Main.tile[x, y].WallType;

                if (Main.tile[x, y] != null)
                {
                    if (wallType != WallID.None && wallType != ModContent.WallType<AstralGrassWall>() && wallType != ModContent.WallType<HardenedAstralSandWall>() &&
                    wallType != ModContent.WallType<AstralSandstoneWall>() && wallType != ModContent.WallType<AstralStoneWall>() &&
                    wallType != ModContent.WallType<AstralDirtWall>() && wallType != ModContent.WallType<AstralSnowWall>() &&
                    wallType != ModContent.WallType<CelestialRemainsWall>() && wallType != ModContent.WallType<AstralIceWall>() &&
                    wallType != ModContent.WallType<AstralMonolithWall>())
                    {
                        if (WallID.Sets.Conversion.Grass[wallType])
                        {
                            Main.tile[x, y].WallType = (ushort)ModContent.WallType<AstralGrassWall>();
                            WorldGen.SquareWallFrame(x, y, true);
                            NetMessage.SendTileSquare(-1, x, y, 1);
                        }
                        else if (WallID.Sets.Conversion.HardenedSand[wallType])
                        {
                            Main.tile[x, y].WallType = (ushort)ModContent.WallType<HardenedAstralSandWall>();
                            WorldGen.SquareWallFrame(x, y, true);
                            NetMessage.SendTileSquare(-1, x, y, 1);
                        }
                        else if (WallID.Sets.Conversion.Sandstone[wallType])
                        {
                            Main.tile[x, y].WallType = (ushort)ModContent.WallType<AstralSandstoneWall>();
                            WorldGen.SquareWallFrame(x, y, true);
                            NetMessage.SendTileSquare(-1, x, y, 1);
                        }
                        else if (WallID.Sets.Conversion.Stone[wallType])
                        {
                            Main.tile[x, y].WallType = (ushort)ModContent.WallType<AstralStoneWall>();
                            WorldGen.SquareWallFrame(x, y, true);
                            NetMessage.SendTileSquare(-1, x, y, 1);
                        }
                        else
                        {
                            switch (wallType)
                            {
                                case WallID.DirtUnsafe:
                                case WallID.DirtUnsafe1:
                                case WallID.DirtUnsafe2:
                                case WallID.DirtUnsafe3:
                                case WallID.DirtUnsafe4:
                                case WallID.Cave6Unsafe:
                                case WallID.Dirt:
                                    Main.tile[x, y].WallType = (ushort)ModContent.WallType<AstralDirtWall>();
                                    WorldGen.SquareWallFrame(x, y, true);
                                    NetMessage.SendTileSquare(-1, x, y, 1);
                                    break;
                                case WallID.SnowWallUnsafe:
                                    Main.tile[x, y].WallType = (ushort)ModContent.WallType<AstralSnowWall>();
                                    WorldGen.SquareWallFrame(x, y, true);
                                    NetMessage.SendTileSquare(-1, x, y, 1);
                                    break;
                                case WallID.DesertFossil:
                                    Main.tile[x, y].WallType = (ushort)ModContent.WallType<CelestialRemainsWall>();
                                    WorldGen.SquareWallFrame(x, y, true);
                                    NetMessage.SendTileSquare(-1, x, y, 1);
                                    break;
                                case WallID.IceUnsafe:
                                    Main.tile[x, y].WallType = (ushort)ModContent.WallType<AstralIceWall>();
                                    WorldGen.SquareWallFrame(x, y, true);
                                    NetMessage.SendTileSquare(-1, x, y, 1);
                                    break;
                                case WallID.LivingWoodUnsafe:
                                    Main.tile[x, y].WallType = (ushort)ModContent.WallType<AstralMonolithWall>();
                                    WorldGen.SquareWallFrame(x, y, true);
                                    NetMessage.SendTileSquare(-1, x, y, 1);
                                    break;
                            }
                        }
                    }

                    if (type >= TileID.Dirt && type != ModContent.TileType<AstralGrass>() && type != ModContent.TileType<AstralStone>() &&
                    type != ModContent.TileType<AstralSand>() && type != ModContent.TileType<HardenedAstralSand>() &&
                    type != ModContent.TileType<AstralSandstone>() && type != ModContent.TileType<AstralIce>() &&
                    type != ModContent.TileType<AstralDirt>() && type != ModContent.TileType<AstralSnow>() &&
                    type != ModContent.TileType<NovaeSlag>() && type != ModContent.TileType<CelestialRemains>() &&
                    type != ModContent.TileType<AstralClay>() && type != ModContent.TileType<AstralVines>() &&
                    type != ModContent.TileType<AstralMonolith>() && type != ModContent.TileType<AstralOre>() &&
                    type != ModContent.TileType<AstralNormalLargePiles>() && type != ModContent.TileType<AstralIceLargePiles>() &&
                    type != ModContent.TileType<AstralDesertLargePiles>() && type != ModContent.TileType<AstralDesertMediumPiles>() &&
                    type != ModContent.TileType<AstralNormalMediumPiles>() && type != ModContent.TileType<AstralIceMediumPiles>() &&
                    type != ModContent.TileType<AstralDesertSmallPiles>() && type != ModContent.TileType<AstralNormalSmallPiles>() &&
                    type != ModContent.TileType<AstralIceSmallPiles>() && type != ModContent.TileType<AstralDesertStalactite>() &&
                    type != ModContent.TileType<AstralNormalStalactite>() && type != ModContent.TileType<AstralIceStalactite>() &&
                    type != ModContent.TileType<AstralDesertStalactite>())
                    {
                        if (TileID.Sets.Conversion.Grass[type] && !TileID.Sets.GrassSpecial[type])
                        {
                            Main.tile[x, y].TileType = (ushort)ModContent.TileType<AstralGrass>();
                            WorldGen.SquareTileFrame(x, y, true);
                            NetMessage.SendTileSquare(-1, x, y, 1);
                        }
                        else if (TileID.Sets.Conversion.Stone[type] || Main.tileMoss[type])
                        {
                            Main.tile[x, y].TileType = (ushort)ModContent.TileType<AstralStone>();
                            WorldGen.SquareTileFrame(x, y, true);
                            NetMessage.SendTileSquare(-1, x, y, 1);
                        }
                        else if (TileID.Sets.Conversion.Sand[type])
                        {
                            Main.tile[x, y].TileType = (ushort)ModContent.TileType<AstralSand>();
                            WorldGen.SquareTileFrame(x, y, true);
                            NetMessage.SendTileSquare(-1, x, y, 1);
                        }
                        else if (TileID.Sets.Conversion.HardenedSand[type])
                        {
                            Main.tile[x, y].TileType = (ushort)ModContent.TileType<HardenedAstralSand>();
                            WorldGen.SquareTileFrame(x, y, true);
                            NetMessage.SendTileSquare(-1, x, y, 1);
                        }
                        else if (TileID.Sets.Conversion.Sandstone[type])
                        {
                            Main.tile[x, y].TileType = (ushort)ModContent.TileType<AstralSandstone>();
                            WorldGen.SquareTileFrame(x, y, true);
                            NetMessage.SendTileSquare(-1, x, y, 1);
                        }
                        else if (TileID.Sets.Conversion.Ice[type])
                        {
                            Main.tile[x, y].TileType = (ushort)ModContent.TileType<AstralIce>();
                            WorldGen.SquareTileFrame(x, y, true);
                            NetMessage.SendTileSquare(-1, x, y, 1);
                        }
                        else
                        {
                            Tile tile = Main.tile[x, y];
                            switch (type)
                            {
                                case TileID.Dirt:
                                    Main.tile[x, y].TileType = (ushort)ModContent.TileType<AstralDirt>();
                                    WorldGen.SquareTileFrame(x, y, true);
                                    NetMessage.SendTileSquare(-1, x, y, 1);
                                    break;
                                case TileID.SnowBlock:
                                    Main.tile[x, y].TileType = (ushort)ModContent.TileType<AstralSnow>();
                                    WorldGen.SquareTileFrame(x, y, true);
                                    NetMessage.SendTileSquare(-1, x, y, 1);
                                    break;
                                case TileID.Silt:
                                case TileID.Slush:
                                    Main.tile[x, y].TileType = (ushort)ModContent.TileType<NovaeSlag>();
                                    WorldGen.SquareTileFrame(x, y, true);
                                    NetMessage.SendTileSquare(-1, x, y, 1);
                                    break;
                                case TileID.DesertFossil:
                                    Main.tile[x, y].TileType = (ushort)ModContent.TileType<CelestialRemains>();
                                    WorldGen.SquareTileFrame(x, y, true);
                                    NetMessage.SendTileSquare(-1, x, y, 1);
                                    break;
                                case TileID.ClayBlock:
                                    Main.tile[x, y].TileType = (ushort)ModContent.TileType<AstralClay>();
                                    WorldGen.SquareTileFrame(x, y, true);
                                    NetMessage.SendTileSquare(-1, x, y, 1);
                                    break;
                                case TileID.Vines:
                                    Main.tile[x, y].TileType = (ushort)ModContent.TileType<AstralVines>();
                                    WorldGen.SquareTileFrame(x, y, true);
                                    NetMessage.SendTileSquare(-1, x, y, 1);
                                    break;
                                case TileID.LivingWood:
                                    Main.tile[x, y].TileType = (ushort)ModContent.TileType<AstralMonolith>();
                                    WorldGen.SquareTileFrame(x, y, true);
                                    NetMessage.SendTileSquare(-1, x, y, 1);
                                    break;
                                case TileID.Copper:
                                case TileID.Iron:
                                case TileID.Silver:
                                case TileID.Gold:
                                case TileID.Tin:
                                case TileID.Lead:
                                case TileID.Tungsten:
                                case TileID.Platinum:
                                    if (convertOre)
                                    {
                                        Main.tile[x, y].TileType = (ushort)ModContent.TileType<AstralOre>();
                                        WorldGen.SquareTileFrame(x, y, true);
                                        NetMessage.SendTileSquare(-1, x, y, 1);
                                    }
                                    break;
                                case TileID.LeafBlock:
                                case TileID.Sunflower:
                                    WorldGen.KillTile(x, y);
                                    if (Main.netMode == NetmodeID.MultiplayerClient)
                                    {
                                        NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, x, y);
                                    }
                                    break;
                                case TileID.LargePiles:
                                    if (tile.TileFrameX <= 1170)
                                    {
                                        RecursiveReplaceToAstral(TileID.LargePiles, (ushort)ModContent.TileType<AstralNormalLargePiles>(), x, y, 324, 0, 1170, 0, 18);
                                    }
                                    if (tile.TileFrameX >= 1728)
                                    {
                                        RecursiveReplaceToAstral(TileID.LargePiles, (ushort)ModContent.TileType<AstralNormalLargePiles>(), x, y, 324, 1728, 1872, 0, 18);
                                    }
                                    if (tile.TileFrameX >= 1404 && tile.TileFrameX <= 1710)
                                    {
                                        RecursiveReplaceToAstral(TileID.LargePiles, (ushort)ModContent.TileType<AstralIceLargePiles>(), x, y, 324, 1404, 1710, 0, 18);
                                    }
                                    break;
                                case TileID.LargePiles2:
                                    if (tile.TileFrameX >= 1566 && tile.TileFrameY < 36)
                                    {
                                        RecursiveReplaceToAstral(TileID.LargePiles2, (ushort)ModContent.TileType<AstralDesertLargePiles>(), x, y, 324, 1566, 1872, 0, 18);
                                    }
                                    if (tile.TileFrameX >= 756 && tile.TileFrameX <= 900)
                                    {
                                        RecursiveReplaceToAstral(TileID.LargePiles2, (ushort)ModContent.TileType<AstralNormalLargePiles>(), x, y, 324, 756, 900, 0, 18);
                                    }
                                    break;
                                case TileID.SmallPiles:
                                    if (tile.TileFrameY == 18)
                                    {
                                        ushort newType;
                                        if (tile.TileFrameX >= 1476 && tile.TileFrameX <= 1674)
                                        {
                                            newType = (ushort)ModContent.TileType<AstralDesertMediumPiles>();
                                        }
                                        else if (tile.TileFrameX <= 558 || (tile.TileFrameX >= 1368 && tile.TileFrameX <= 1458))
                                        {
                                            newType = (ushort)ModContent.TileType<AstralNormalMediumPiles>();
                                        }
                                        else if (tile.TileFrameX >= 900 && tile.TileFrameX <= 1098)
                                        {
                                            newType = (ushort)ModContent.TileType<AstralIceMediumPiles>();
                                        }
                                        else
                                        {
                                            break;
                                        }
                                        int leftMost = x;
                                        if (tile.TileFrameX % 36 != 0) //this means it's the right tile of the two
                                        {
                                            leftMost--;
                                        }
                                        if (Main.tile[leftMost, y] != null)
                                        {
                                            Main.tile[leftMost, y].TileType = newType;
                                            WorldGen.SquareTileFrame(leftMost, y, true);
                                            NetMessage.SendTileSquare(-1, leftMost, y, 1);
                                        }
                                        if (Main.tile[leftMost + 1, y] != null)
                                        {
                                            Main.tile[leftMost + 1, y].TileType = newType;
                                            WorldGen.SquareTileFrame(leftMost + 1, y, true);
                                            NetMessage.SendTileSquare(-1, leftMost + 1, y, 1);
                                        }
                                        while (Main.tile[leftMost, y].TileFrameX >= 216)
                                        {
                                            if (Main.tile[leftMost, y] != null)
                                                Main.tile[leftMost, y].TileFrameX -= 216;
                                            if (Main.tile[leftMost + 1, y] != null)
                                                Main.tile[leftMost + 1, y].TileFrameX -= 216;
                                        }
                                    }
                                    else if (tile.TileFrameY == 0)
                                    {
                                        ushort newType3;
                                        if (tile.TileFrameX >= 972 && tile.TileFrameX <= 1062)
                                        {
                                            newType3 = (ushort)ModContent.TileType<AstralDesertSmallPiles>();
                                        }
                                        else if (tile.TileFrameX <= 486)
                                        {
                                            newType3 = (ushort)ModContent.TileType<AstralNormalSmallPiles>();
                                        }
                                        else if (tile.TileFrameX >= 648 && tile.TileFrameX <= 846)
                                        {
                                            newType3 = (ushort)ModContent.TileType<AstralIceSmallPiles>();
                                        }
                                        else
                                        {
                                            break;
                                        }
                                        Main.tile[x, y].TileType = newType3;
                                        while (Main.tile[x, y].TileFrameX >= 108) //REFRAME IT
                                        {
                                            Main.tile[x, y].TileFrameX -= 108;
                                        }
                                        WorldGen.SquareTileFrame(x, y, true);
                                        NetMessage.SendTileSquare(-1, x, y, 1);
                                    }
                                    break;
                                case TileID.Stalactite:
                                    int topMost = tile.TileFrameY <= 54 ? (tile.TileFrameY % 36 == 0 ? y : y - 1) : y;
                                    bool twoTall = tile.TileFrameY <= 54;
                                    bool hanging = tile.TileFrameY <= 18 || tile.TileFrameY == 72;
                                    ushort newType2;
                                    if (tile.TileFrameX >= 378 && tile.TileFrameX <= 414) //DESERT
                                    {
                                        newType2 = (ushort)ModContent.TileType<AstralDesertStalactite>();
                                    }
                                    else if ((tile.TileFrameX >= 54 && tile.TileFrameX <= 90) || (tile.TileFrameX >= 216 && tile.TileFrameX <= 360))
                                    {
                                        newType2 = (ushort)ModContent.TileType<AstralNormalStalactite>();
                                    }
                                    else if (tile.TileFrameX <= 36)
                                    {
                                        newType2 = (ushort)ModContent.TileType<AstralIceStalactite>();
                                    }
                                    else
                                    {
                                        break;
                                    }

                                    //Set types
                                    if (Main.tile[x, topMost] != null)
                                    {
                                        Main.tile[x, topMost].TileType = newType2;
                                    }
                                    if (twoTall)
                                    {
                                        if (Main.tile[x, topMost + 1] != null)
                                            Main.tile[x, topMost + 1].TileType = newType2;
                                    }

                                    //Fix frames
                                    while (Main.tile[x, topMost].TileFrameX >= 54)
                                    {
                                        if (Main.tile[x, topMost] != null)
                                            Main.tile[x, topMost].TileFrameX -= 54;
                                        if (twoTall)
                                        {
                                            if (Main.tile[x, topMost + 1] != null)
                                                Main.tile[x, topMost + 1].TileFrameX -= 54;
                                        }
                                    }

                                    if (Main.tile[x, topMost] != null)
                                    {
                                        WorldGen.SquareTileFrame(x, topMost, true);
                                        NetMessage.SendTileSquare(-1, x, topMost, 1);
                                    }

                                    if (Main.tile[x, topMost + 1] != null)
                                    {
                                        WorldGen.SquareTileFrame(x, topMost + 1, true);
                                        NetMessage.SendTileSquare(-1, x, topMost + 1, 1);
                                    }

                                    if (hanging)
                                    {
                                        ConvertToAstral(x, topMost - 1);
                                        break;
                                    }
                                    else
                                    {
                                        if (twoTall)
                                        {
                                            ConvertToAstral(x, topMost + 2);
                                            break;
                                        }
                                        ConvertToAstral(x, topMost + 1);
                                        break;
                                    }
                                case TileID.OasisPlants:
                                    Main.tile[x, y].Get<TileWallWireStateData>().HasTile = false;
                                    NetMessage.SendTileSquare(-1, x, y, 1);
                                    break;
                            }
                        }
                    }
                }
            }
        }

        public static void ConvertFromAstral(int startX, int endX, int startY, int endY, ConvertType convert)
        {
            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    ConvertFromAstral(x, y, convert);
                }
            }
        }

        public static void ConvertFromAstral(int x, int y, ConvertType convert, bool tileframe = true)
        {
            Tile tile = Main.tile[x, y];
            int type = tile.TileType;
            int wallType = tile.WallType;

            if (WorldGen.InWorld(x, y, 1))
            {
                #region WALL
                if (Main.tile[x, y] != null)
                {
                    if (wallType == ModContent.WallType<AstralDirtWall>())
                    {
                        Main.tile[x, y].WallType = WallID.DirtUnsafe;
                    }
                    else if (wallType == ModContent.WallType<AstralSnowWall>() || wallType == ModContent.WallType<AstralSnowWallSafe>())
                    {
                        Main.tile[x, y].WallType = WallID.SnowWallUnsafe;
                    }
                    else if (wallType == ModContent.WallType<CelestialRemainsWall>())
                    {
                        Main.tile[x, y].WallType = WallID.DesertFossil;
                    }
                    else if (wallType == ModContent.WallType<AstralGrassWall>())
                    {
                        switch (convert)
                        {
                            case ConvertType.Corrupt:
                                Main.tile[x, y].WallType = WallID.CorruptGrassUnsafe;
                                break;
                            case ConvertType.Crimson:
                                Main.tile[x, y].WallType = WallID.CrimsonGrassUnsafe;
                                break;
                            case ConvertType.Hallow:
                                Main.tile[x, y].WallType = WallID.HallowedGrassUnsafe;
                                break;
                            case ConvertType.Pure:
                                Main.tile[x, y].WallType = WallID.GrassUnsafe;
                                break;
                        }
                    }
                    else if (wallType == ModContent.WallType<AstralIceWall>())
                    {
                        Main.tile[x, y].WallType = WallID.IceUnsafe;
                    }
                    else if (wallType == ModContent.WallType<AstralMonolithWall>())
                    {
                        Main.tile[x, y].WallType = WallID.LivingWood;
                    }
                    else if (wallType == ModContent.WallType<AstralStoneWall>())
                    {
                        switch (convert)
                        {
                            case ConvertType.Corrupt:
                                Main.tile[x, y].WallType = WallID.EbonstoneUnsafe;
                                break;
                            case ConvertType.Crimson:
                                Main.tile[x, y].WallType = WallID.CrimstoneUnsafe;
                                break;
                            case ConvertType.Hallow:
                                Main.tile[x, y].WallType = WallID.PearlstoneBrickUnsafe;
                                break;
                            case ConvertType.Pure:
                                Main.tile[x, y].WallType = WallID.Stone;
                                break;
                        }
                    }
                }
                #endregion

                #region TILE
                if (Main.tile[x, y] != null)
                {
                    if (type == ModContent.TileType<AstralDirt>())
                    {
                        tile.TileType = TileID.Dirt;
                    }
                    else if (type == ModContent.TileType<AstralSnow>())
                    {
                        tile.TileType = TileID.SnowBlock;
                    }
                    else if (type == ModContent.TileType<NovaeSlag>())
                    {
                        tile.TileType = TileID.Silt;
                    }
                    else if (type == ModContent.TileType<CelestialRemains>())
                    {
                        tile.TileType = TileID.DesertFossil;
                    }
                    else if (type == ModContent.TileType<AstralClay>())
                    {
                        tile.TileType = TileID.ClayBlock;
                    }
                    else if (type == ModContent.TileType<AstralGrass>())
                    {
                        SetTileFromConvert(x, y, convert, TileID.CorruptGrass, TileID.CrimsonGrass, TileID.HallowedGrass, TileID.Grass);
                    }
                    else if (type == ModContent.TileType<AstralStone>())
                    {
                        SetTileFromConvert(x, y, convert, TileID.Ebonstone, TileID.Crimstone, TileID.Pearlstone, TileID.Stone);
                    }
                    else if (type == ModContent.TileType<AstralMonolith>())
                    {
                        tile.TileType = TileID.LivingWood;
                    }
                    else if (type == ModContent.TileType<AstralSand>())
                    {
                        SetTileFromConvert(x, y, convert, TileID.Ebonsand, TileID.Crimsand, TileID.Pearlsand, TileID.Sand);
                    }
                    else if (type == ModContent.TileType<AstralSandstone>())
                    {
                        SetTileFromConvert(x, y, convert, TileID.CorruptSandstone, TileID.CrimsonSandstone, TileID.HallowSandstone, TileID.Sandstone);
                    }
                    else if (type == ModContent.TileType<HardenedAstralSand>())
                    {
                        SetTileFromConvert(x, y, convert, TileID.CorruptHardenedSand, TileID.CrimsonHardenedSand, TileID.HallowHardenedSand, TileID.HardenedSand);
                    }
                    else if (type == ModContent.TileType<AstralIce>())
                    {
                        SetTileFromConvert(x, y, convert, TileID.CorruptIce, TileID.FleshIce, TileID.HallowedIce, TileID.IceBlock);
                    }
                    else if (type == ModContent.TileType<AstralVines>())
                    {
                        SetTileFromConvert(x, y, convert, ushort.MaxValue, TileID.CrimsonVines, TileID.HallowedVines, TileID.Vines);
                    }
                    else if (type == ModContent.TileType<AstralShortPlants>())
                    {
                        SetTileFromConvert(x, y, convert, TileID.CorruptPlants, ushort.MaxValue, TileID.HallowedPlants, TileID.Plants);
                    }
                    else if (type == ModContent.TileType<AstralTallPlants>())
                    {
                        SetTileFromConvert(x, y, convert, ushort.MaxValue, ushort.MaxValue, TileID.HallowedPlants2, TileID.Plants2);
                    }
                    else if (type == ModContent.TileType<AstralNormalLargePiles>())
                    {
                        RecursiveReplaceFromAstral((ushort)type, TileID.LargePiles, x, y, 378, 0);
                    }
                    else if (type == ModContent.TileType<AstralNormalMediumPiles>())
                    {
                        RecursiveReplaceFromAstral((ushort)type, TileID.SmallPiles, x, y, 0, 18);
                    }
                    else if (type == ModContent.TileType<AstralNormalSmallPiles>())
                    {
                        RecursiveReplaceFromAstral((ushort)type, TileID.SmallPiles, x, y, 0, 0);
                    }
                    else if (type == ModContent.TileType<AstralDesertLargePiles>())
                    {
                        RecursiveReplaceFromAstral((ushort)type, TileID.LargePiles2, x, y, 1566, 0);
                    }
                    else if (type == ModContent.TileType<AstralDesertMediumPiles>())
                    {
                        RecursiveReplaceFromAstral((ushort)type, TileID.SmallPiles, x, y, 1476, 18);
                    }
                    else if (type == ModContent.TileType<AstralDesertSmallPiles>())
                    {
                        RecursiveReplaceFromAstral((ushort)type, TileID.SmallPiles, x, y, 972, 0);
                    }
                    else if (type == ModContent.TileType<AstralIceLargePiles>())
                    {
                        RecursiveReplaceFromAstral((ushort)type, TileID.LargePiles, x, y, 1404, 0);
                    }
                    else if (type == ModContent.TileType<AstralIceMediumPiles>())
                    {
                        RecursiveReplaceFromAstral((ushort)type, TileID.SmallPiles, x, y, 900, 18);
                    }
                    else if (type == ModContent.TileType<AstralIceSmallPiles>())
                    {
                        RecursiveReplaceFromAstral((ushort)type, TileID.SmallPiles, x, y, 648, 0);
                    }
                    else if (type == ModContent.TileType<AstralNormalStalactite>())
                    {
                        ushort originType = TileID.Stone;
                        switch (convert)
                        {
                            case ConvertType.Corrupt:
                                originType = TileID.Ebonstone;
                                break;
                            case ConvertType.Crimson:
                                originType = TileID.Crimstone;
                                break;
                            case ConvertType.Hallow:
                                originType = TileID.Pearlstone;
                                break;
                        }
                        ReplaceAstralStalactite(TileID.Stalactite, originType, x, y);
                    }
                    else if (type == ModContent.TileType<AstralDesertStalactite>())
                    {
                        ushort originType = TileID.Sandstone;
                        switch (convert)
                        {
                            case ConvertType.Corrupt:
                                originType = TileID.CorruptSandstone;
                                break;
                            case ConvertType.Crimson:
                                originType = TileID.CrimsonSandstone;
                                break;
                            case ConvertType.Hallow:
                                originType = TileID.HallowSandstone;
                                break;
                        }
                        ReplaceAstralStalactite(TileID.Stalactite, originType, x, y);
                    }
                    else if (type == ModContent.TileType<AstralIceStalactite>())
                    {
                        ReplaceAstralStalactite(TileID.Stalactite, TileID.IceBlock, x, y);
                    }

                    if (TileID.Sets.Conversion.Grass[type] || type == TileID.Dirt)
                    {
                        WorldGen.SquareTileFrame(x, y);
                    }
                }
                #endregion

                if (tileframe)
                {
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        WorldGen.SquareTileFrame(x, y, true);
                    }
                    else if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, x, y, 1);
                    }
                }
            }
        }

        public static void SetTileFromConvert(int x, int y, ConvertType convert, ushort corrupt, ushort crimson, ushort hallow, ushort pure)
        {
            switch (convert)
            {
                case ConvertType.Corrupt:
                    if (corrupt != ushort.MaxValue)
                    {
                        Main.tile[x, y].TileType = corrupt;
                        WorldGen.SquareTileFrame(x, y);
                    }
                    break;
                case ConvertType.Crimson:
                    if (crimson != ushort.MaxValue)
                    {
                        Main.tile[x, y].TileType = crimson;
                        WorldGen.SquareTileFrame(x, y);
                    }
                    break;
                case ConvertType.Hallow:
                    if (hallow != ushort.MaxValue)
                    {
                        Main.tile[x, y].TileType = hallow;
                        WorldGen.SquareTileFrame(x, y);
                    }
                    break;
                case ConvertType.Pure:
                    if (pure != ushort.MaxValue)
                    {
                        Main.tile[x, y].TileType = pure;
                        WorldGen.SquareTileFrame(x, y);
                    }
                    break;
            }
        }

        public static void RecursiveReplaceToAstral(ushort checkType, ushort replaceType, int x, int y, int replaceTextureWidth, int minFrameX = 0, int maxFrameX = int.MaxValue, int minFrameY = 0, int maxFrameY = int.MaxValue)
        {
            Tile tile = Main.tile[x, y];
            if (tile == null || !tile.HasTile || tile.TileType != checkType || tile.TileFrameX < minFrameX || tile.TileFrameX > maxFrameX || tile.TileFrameY < minFrameY || tile.TileFrameY > maxFrameY)
                return;

            Main.tile[x, y].TileType = replaceType;
            while (Main.tile[x, y].TileFrameX >= replaceTextureWidth)
            {
                Main.tile[x, y].TileFrameX -= (short)replaceTextureWidth;
            }

            if (Main.tile[x - 1, y] != null)
                RecursiveReplaceToAstral(checkType, replaceType, x - 1, y, replaceTextureWidth, minFrameX, maxFrameX, minFrameY, maxFrameY);
            if (Main.tile[x + 1, y] != null)
                RecursiveReplaceToAstral(checkType, replaceType, x + 1, y, replaceTextureWidth, minFrameX, maxFrameX, minFrameY, maxFrameY);
            if (Main.tile[x, y - 1] != null)
                RecursiveReplaceToAstral(checkType, replaceType, x, y - 1, replaceTextureWidth, minFrameX, maxFrameX, minFrameY, maxFrameY);
            if (Main.tile[x, y + 1] != null)
                RecursiveReplaceToAstral(checkType, replaceType, x, y + 1, replaceTextureWidth, minFrameX, maxFrameX, minFrameY, maxFrameY);
        }

        public static void RecursiveReplaceFromAstral(ushort checkType, ushort replaceType, int x, int y, int addFrameX, int addFrameY)
        {
            Tile tile = Main.tile[x, y];
            if (tile == null || !tile.HasTile || tile.TileType != checkType)
                return;

            Main.tile[x, y].TileType = replaceType;
            Main.tile[x, y].TileFrameX += (short)addFrameX;
            Main.tile[x, y].TileFrameY += (short)addFrameY;

            if (Main.tile[x - 1, y] != null)
                RecursiveReplaceFromAstral(checkType, replaceType, x - 1, y, addFrameX, addFrameY);
            if (Main.tile[x + 1, y] != null)
                RecursiveReplaceFromAstral(checkType, replaceType, x + 1, y, addFrameX, addFrameY);
            if (Main.tile[x, y - 1] != null)
                RecursiveReplaceFromAstral(checkType, replaceType, x, y - 1, addFrameX, addFrameY);
            if (Main.tile[x, y + 1] != null)
                RecursiveReplaceFromAstral(checkType, replaceType, x, y + 1, addFrameX, addFrameY);
        }

        public static void ReplaceAstralStalactite(ushort replaceType, ushort replaceOriginTile, int x, int y)
        {
            Tile tile = Main.tile[x, y];

            int topMost = tile.TileFrameY <= 54 ? (tile.TileFrameY % 36 == 0 ? y : y - 1) : y;
            bool twoTall = tile.TileFrameY <= 54;
            bool hanging = tile.TileFrameY <= 18 || tile.TileFrameY == 72;

            int yOriginTile = hanging ? topMost - 1 : (twoTall ? topMost + 2 : y + 1);

            if (Main.tile[x, topMost++] != null)
                Main.tile[x, topMost++].TileType = replaceType;
            if (twoTall)
            {
                if (Main.tile[x, topMost] != null)
                    Main.tile[x, topMost].TileType = replaceType;
            }
            if (Main.tile[x, yOriginTile] != null)
                Main.tile[x, yOriginTile].TileType = replaceOriginTile;
        }

        public static bool CheckInEllipse(Point tile, Vector2 focus1, Vector2 focus2, float distanceConstant, Vector2 center, out float distance, bool collapse = false)
        {
            Vector2 point = tile.ToWorldCoordinates();
            if (collapse) //Collapse ensures the ellipse is shrunk down a lot in terms of distance.
            {
                float distY = center.Y - point.Y;
                point.Y -= distY * 3f;
            }
            float distance1 = Vector2.Distance(point, focus1);
            float distance2 = Vector2.Distance(point, focus2);
            distance = distance1 + distance2;
            return distance <= distanceConstant;
        }
    }
}
