using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.WorldBuilding;

namespace CalamityMod.World.Planets
{
    public class HeartPlanet : Planetoid
    {
        private ushort[] mossTypes = new ushort[]
        {
            TileID.BlueMoss,
            TileID.GreenMoss,
            TileID.PurpleMoss,
            TileID.RedMoss
        };

        public override bool Place(Point origin, StructureMap structures)
        {
            int radius = _random.Next(6, 10);

            if (!CheckIfPlaceable(origin, radius, structures))
            {
                return false;
            }

            PlacePlanet(origin, radius);

            return base.Place(origin, structures);
        }

        public void PlacePlanet(Point origin, int radius)
        {
            ShapeData mainArea = new ShapeData();

            //Create main shape
            WorldUtils.Gen(origin, new Shapes.Circle(radius), Actions.Chain(new GenAction[]
            {
                new Modifiers.Blotches(2, 0.3),
                new Actions.ClearTile(true),
                new Actions.PlaceWall(WallID.Cave3Unsafe),
                new Actions.PlaceTile(TileID.Stone).Output(mainArea)
            }));

            //Place gems
            ushort gemType = _random.Next(new ushort[] { TileID.Amethyst, TileID.Emerald, TileID.Topaz, TileID.Ruby, TileID.Sapphire, TileID.Diamond });
            //Using Tile Runner as it has a wider spread
            WorldGen.TileRunner(origin.X, origin.Y, _random.NextFloat(6f, 9f), _random.Next(8, 18), gemType);

            //Create Smoothness and moss
            ushort mossType = _random.Next(mossTypes);
            WorldUtils.Gen(origin, new ModShapes.OuterOutline(mainArea), Actions.Chain(new GenAction[]
            {
                new Actions.SetTile(mossType, true, false),
                new Modifiers.Conditions(new CustomConditions.RandomChance(2)),
                new Actions.Smooth(true),
                new Actions.SetFrames(true)
            }));

            //Place Heart Crystal in center
            //##
            //##
            // X
            //Place heart crystal at X, it'll appear at #

            ShapeData room = new ShapeData();

            int width = _random.Next(3, 5) * 2;
            int height = _random.Next(5, 8);
            Point roomTopLeft = new Point(origin.X - width / 2, origin.Y - height / 2);
            bool gold = _random.NextBool();
            ushort tile = gold ? TileID.GoldBrick : TileID.PlatinumBrick;
            ushort wall = gold ? WallID.GoldBrick : WallID.PlatinumBrick;
            WorldUtils.Gen(roomTopLeft, new Shapes.Rectangle(width, height), Actions.Chain(new GenAction[]
            {
                new Actions.ClearTile(true),
                new Actions.PlaceWall(wall).Output(room)
            }));
            WorldUtils.Gen(roomTopLeft, new ModShapes.InnerOutline(room), Actions.Chain(new GenAction[]
            {
                new Actions.PlaceTile(tile)
            }));

            //Add LC
            WorldGen.AddLifeCrystal(origin.X, origin.Y + 2);
        }
    }
}
