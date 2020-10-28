using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;

namespace CalamityMod.World.Planets
{
    public class Planetoid : MicroBiome
    {
        private Rectangle _area;

        public static bool InvalidSkyPlacementArea(Rectangle area)
		{
            Mod varia = ModLoader.GetMod("Varia");
            for (int i = area.Left; i < area.Right; i++)
            {
                for (int j = area.Top; j < area.Bottom; j++)
                {
                    if (Main.tile[i, j].type == TileID.Cloud || Main.tile[i, j].type == TileID.RainCloud || Main.tile[i, j].type == TileID.Sunplate)
                        return false;

                    if (varia != null &&
                        (Main.tile[i, j].type == varia.TileType("StarplateBrick") || Main.tile[i, j].type == varia.TileType("ForgottenCloud")))
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
