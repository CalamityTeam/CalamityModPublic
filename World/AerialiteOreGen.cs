using CalamityMod.Tiles.Ores;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace CalamityMod.World
{
    public class AerialiteOreGen
    {
        /// <summary>
        /// The chance for a cloud tile the be randomly changed into a blotch of disenchanted aerialite ore during world generation.
        /// </summary>
        public const int CloudOreConversionChance = 365;

        public static void Generate()
        {
            // Don't attempt to generate anything client-side. Only the server is allowed to do that.
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            // Loop through every tile above the world's surface, searching for potential candidates for aerialite generation.
            for (int x = 5; x < Main.maxTilesX - 5; x++)
            {
                for (int y = 5; y < Main.worldSurface; y++)
                {
                    Tile tile = Main.tile[x, y];

                    // The following conditions must happen in order for aerialite to generate:
                    // 1. The original tile ID must be that of a cloud.
                    // 2. The original tile must not be empty air.
                    // 3. A random dice-roll must land correctly, to ensure that patches of ore are occasional.
                    // If any of these conditions are not met, this loop iteration is skipped.
                    if (tile.TileType != TileID.Cloud || !tile.HasTile || !WorldGen.genRand.NextBool(CloudOreConversionChance))
                        continue;

                    int radius = (int)(WorldGen.genRand.Next(3, 5) * WorldGen.genRand.NextFloat(0.74f, 0.82f));
                    ShapeData circle = new ShapeData();
                    ShapeData biggerCircle = new ShapeData();
                    GenAction blotchMod = new Modifiers.Blotches(2, 0.4);

                    // Big cloud circle.
                    WorldUtils.Gen(new Point(x, y), new Shapes.Circle(radius + 1), Actions.Chain(new GenAction[]
                    {
                        blotchMod.Output(biggerCircle)
                    }));

                    WorldUtils.Gen(new Point(x, y), new ModShapes.All(biggerCircle), Actions.Chain(new GenAction[]
                    {
                        new Actions.ClearTile(),
                        new Actions.PlaceTile((ushort)TileID.Cloud)
                    }));

                    // Circle of ore.
                    WorldUtils.Gen(new Point(x, y), new Shapes.Circle(radius), Actions.Chain(new GenAction[]
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
        }

        public static void Enchant()
        {
            // Don't attempt to change tile states client-side.
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            // Loop through every tile above the world's surface, searching for aerialite to generate.
            ushort disenchantedOreID = (ushort)ModContent.TileType<AerialiteOreDisenchanted>();
            ushort enchantedOreID = (ushort)ModContent.TileType<AerialiteOre>();
            for (int x = 5; x < Main.maxTilesX - 5; x++)
            {
                for (int y = 5; y < Main.worldSurface; y++)
                {
                    if (Main.tile[x, y].TileType != disenchantedOreID)
                        continue;

                    // Enchant the ore and re-evaluate nearby framing.
                    Main.tile[x, y].TileType = enchantedOreID;
                    WorldGen.SquareTileFrame(x, y);

                    // Inform all clients of the tile change.
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendTileSquare(-1, x, y);
                }
            }
        }
    }
}
