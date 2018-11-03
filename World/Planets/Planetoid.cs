using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Terraria;
using Terraria.ModLoader;
using Terraria.World.Generation;
using Terraria.GameContent.Generation;
using Microsoft.Xna.Framework;

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
            int diameter = myRadius * 2 ;
            _area = new Rectangle(origin.X - myRadius, origin.Y - myRadius, diameter, diameter);

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
