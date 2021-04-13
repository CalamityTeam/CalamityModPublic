using CalamityMod.Tiles.Ores;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;

namespace CalamityMod.World.Planets
{
    public class LuminitePlanet : Planetoid
    {
        public override bool Place(Point origin, StructureMap structures)
        {
            int radius = _random.Next(18, 21);

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
                NetMessage.SendTileRange(-1, origin.X - radius - 16, origin.Y - radius - 16, radius * 2 + 16, radius * 2 + 16);
        }
	}
}
