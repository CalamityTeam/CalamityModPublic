using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.WorldBuilding;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Collections.Generic;
using CalamityMod.DataStructures;

namespace CalamityMod.World
{
    public class AerialiteOreGen
    {
        public static void Generate(bool Convert)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int x = 5; x < Main.maxTilesX - 5; x++) 
                {
                    for (int y = 5; y < 300; y++) 
                    {
                        if (!Convert)
                        {
                            if (WorldGen.genRand.Next(365) == 0 && Main.tile[x, y].TileType == TileID.Cloud && Main.tile[x, y].HasTile)
                            {
                                ShapeData circle = new ShapeData();
                                GenAction blotchMod = new Modifiers.Blotches(2, 0.4);

                                int outerRadius = (int)(WorldGen.genRand.Next(3, 5) * WorldGen.genRand.NextFloat(0.74f, 0.82f));

                                WorldUtils.Gen(new Point(x, y), new Shapes.Circle(outerRadius), Actions.Chain(new GenAction[]
                                {
                                    blotchMod.Output(circle)
                                }));

                                WorldUtils.Gen(new Point(x, y), new ModShapes.All(circle), Actions.Chain(new GenAction[]
                                {
                                    new Actions.ClearTile(),
                                    new Actions.PlaceTile((ushort)ModContent.TileType<Tiles.Ores.AerialiteOreDisenchanted>())
                                }));
                            }
                        }

                        if (Convert)
                        {
                            if (Main.tile[x, y].TileType == ModContent.TileType<Tiles.Ores.AerialiteOreDisenchanted>())
                            {
                                Main.tile[x, y].TileType = (ushort)ModContent.TileType<Tiles.Ores.AerialiteOre>();
                            }
                        }
                    }
                }
            }
        }
    }
}