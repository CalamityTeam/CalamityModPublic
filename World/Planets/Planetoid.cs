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

        public bool CheckIfPlaceable(Point origin, int radius, StructureMap structures)
        {
            //Fluff is used to create padding between the planets. this is the minimum distance between planets (they can't be within "fluff" blocks)
            int fluff = 10;
            int myRadius = radius + fluff;
            int diameter = myRadius * 2;
            _area = new Rectangle(origin.X - myRadius, origin.Y - myRadius, diameter, diameter);

            Mod varia = ModLoader.GetMod("Varia");
            for (int i = _area.Left; i < _area.Right; i++)
            {
                for (int j = _area.Top; j < _area.Bottom; j++)
                {
                    if (Main.tile[i, j].type == TileID.Cloud || Main.tile[i, j].type == TileID.RainCloud || Main.tile[i, j].type == TileID.Sunplate)
                    {
                        return false;
                    }
                    if (varia != null)
                    {
                        if (Main.tile[i, j].type == varia.TileType("StarplateBrick") || Main.tile[i, j].type == varia.TileType("ForgottenCloud"))
                        {
                            return false;
                        }
                    }
                }
            }

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
