using CalamityMod.Tiles.Ores;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace CalamityMod.World.Planets
{
    public class LuminitePlanet : Planetoid
    {
        public static void GenerateLuminitePlanetoids()
        {
            // Don't attempt to generate these things client-side.
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            // The worldgen structure map only gets created/regenerated when you generate a world, for some reason.
            // This means if you simply enter a world and generate this planetoid, the map will not exist yet, and errors will arise.
            // As a result, a new one is generated as necessary.
            if (GenVars.structures is null)
                GenVars.structures = new StructureMap();
            var config = WorldGenConfiguration.FromEmbeddedPath("Terraria.GameContent.WorldBuilding.Configuration.json");

            int totalPlanetoidsToGenerate = Main.maxTilesX / 1200 + 2;
            for (int i = 0; i < totalPlanetoidsToGenerate; i++)
            {
                for (int tries = 0; tries < 15000; tries++)
                {
                    Point planetoidOrigin = new Point(WorldGen.genRand.Next((int)(Main.maxTilesX*0.15), (int)(Main.maxTilesX*0.85)), WorldGen.genRand.Next(75, 125));
                    if (WorldGen.genRand.NextBool(2))
                    {
                        if (config.CreateBiome<LuminitePlanet>().Place(planetoidOrigin, GenVars.structures))
                            break;
                    }
                    else
                    {
                        if (config.CreateBiome<LuminitePlanet2>().Place(planetoidOrigin, GenVars.structures))
                            break;
                    }
                }
            }
        }

        public override bool Place(Point origin, StructureMap structures)
        {
            int radius = _random.Next(14, 18);

            if (!CheckIfPlaceable(origin, radius, structures))
                return false;

            PlacePlanet(origin, radius);
            return base.Place(origin, structures);
        }

        public void PlacePlanet(Point origin, int radius)
        {
            // Place outer shell that's somewhat randomized in shape.
            for (int i = 0; i < 6; i++)
            {
                Vector2 offset = WorldGen.genRand.NextVector2Circular(7f, 7f);
                Point offsetedOrigin = new Point((int)(origin.X + offset.X), (int)(origin.Y + offset.Y));

                // Create an outer exodium shell.
                WorldUtils.Gen(offsetedOrigin, new Shapes.Circle(radius), Actions.Chain(new GenAction[]
                {
                    new Actions.PlaceTile((ushort)ModContent.TileType<ExodiumOre>())
                }));

                // Create walls.
                WorldUtils.Gen(offsetedOrigin, new Shapes.Circle(radius - 4), Actions.Chain(new GenAction[]
                {
                    new Actions.PlaceWall(WallID.Stone)
                }));

                // Place an inner luminite core.
                WorldUtils.Gen(origin, new Shapes.Circle(radius - 3), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Blotches(2, 0.3),
                    new Actions.SetTile(TileID.LunarOre, true)
                }));
            }

            // Smoothen the structure.
            WorldUtils.Gen(origin, new Shapes.Circle(radius + 16), Actions.Chain(new GenAction[]
            {
                new Actions.Smooth()
            }));

            // And sync the entire thing.
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendTileSquare(-1, origin.X - radius - 16, origin.Y - radius - 16, radius * 2 + 16, radius * 2 + 16);
        }
    }
}
