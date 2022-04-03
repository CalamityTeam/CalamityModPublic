using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace CalamityMod.World.Planets
{
    public class Planetoid : MicroBiome
    {
        private Rectangle _area;

        public static void GenerateAllBasePlanetoids(GenerationProgress progress)
        {
            progress.Message = "Creating a new solar system";

            int GrassPlanetoidCount = Main.maxTilesX / 1100;
            int LCPlanetoidCount = Main.maxTilesX / 800;
            int MudPlanetoidCount = Main.maxTilesX / 1100;

            const int MainPlanetoidAttempts = 3000;
            int i = 0;
            while (i < MainPlanetoidAttempts)
            {
                if (Biomes<MainPlanet>.Place(new Point(WorldGen.genRand.Next(Main.maxTilesX / 2 - 300, Main.maxTilesX / 2 + 300), WorldGen.genRand.Next(128, 134)), WorldGen.structures))
                {
                    break;
                }
                i++;
            }

            const int CrystalHeartPlanetoidAttempts = 15000;
            i = 0;
            while (LCPlanetoidCount > 0 && i < CrystalHeartPlanetoidAttempts)
            {
                int x = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.2), (int)(Main.maxTilesX * 0.8));
                int y = WorldGen.genRand.Next(70, 101);

                bool placed = Biomes<HeartPlanet>.Place(x, y, WorldGen.structures);

                if (placed)
                    LCPlanetoidCount--;
                i++;
            }

            const int GrassPlanetoidAttempts = 12000;
            i = 0;
            while (GrassPlanetoidCount > 0 && i < GrassPlanetoidAttempts)
            {
                int x = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.333), (int)(Main.maxTilesX * 0.666));
                int y = WorldGen.genRand.Next(100, 131);


                bool placed = Biomes<GrassPlanet>.Place(x, y, WorldGen.structures);

                if (placed)
                    GrassPlanetoidCount--;
                i++;
            }

            const int MudPlanetoidAttempts = 12000;
            i = 0;
            while (MudPlanetoidCount > 0 && i < MudPlanetoidAttempts)
            {
                int x = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.3f), (int)(Main.maxTilesX * 0.7f));
                int y = WorldGen.genRand.Next(100, 131);

                bool placed = Biomes<MudPlanet>.Place(x, y, WorldGen.structures);

                if (placed)
                    MudPlanetoidCount--;
                i++;
            }
        }

        public static bool InvalidSkyPlacementArea(Rectangle area)
        {
            Mod varia = CalamityMod.Instance.varia;
            for (int i = area.Left; i < area.Right; i++)
            {
                for (int j = area.Top; j < area.Bottom; j++)
                {
                    if (Main.tile[i, j].TileType == TileID.Cloud || Main.tile[i, j].TileType == TileID.RainCloud || Main.tile[i, j].TileType == TileID.Sunplate)
                        return false;

                    if (varia != null &&
                        (Main.tile[i, j].TileType == varia.Find<ModTile>("StarplateBrick") .Type|| Main.tile[i, j].TileType == varia.Find<ModTile>("ForgottenCloud").Type))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public bool CheckIfPlaceable(Point origin, int radius, StructureMap structures)
        {
            //Fluff is used to create padding between the planets. this is the minimum distance between planets (they can't be within "fluff" blocks)
            int fluff = 10;
            int myRadius = radius + fluff;
            int diameter = myRadius * 2;
            _area = new Rectangle(origin.X - myRadius, origin.Y - myRadius, diameter, diameter);

            if (!InvalidSkyPlacementArea(_area))
                return false;

            if (!structures.CanPlace(_area))
            {
                return false;
            }

            Dictionary<ushort, int> dict = new Dictionary<ushort, int>();
            CustomActions.SolidScanner scanner = new CustomActions.SolidScanner();
            WorldUtils.Gen(_area.Location, new Shapes.Rectangle(_area.Width, _area.Height), scanner);
            if (scanner.GetCount() > 2)
            {
                return false;
            }
            return true;
        }

        public override bool Place(Point origin, StructureMap structures)
        {
            structures.AddStructure(_area);

            //Optional, add the planetoid's center to a list, do something else etc.
            //Just have this option available

            return true;
        }
    }
}
