using CalamityMod.Tiles.Ores;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace CalamityMod.World.Planets
{
    public class LuminitePlanet2 : Planetoid
    {
        public override bool Place(Point origin, StructureMap structures)
        {
            int radius = _random.Next(16, 21);

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
                    new Actions.PlaceTile((ushort)ModContent.TileType<ExodiumOre>()),
                    new Actions.SetFrames()
                }));

                // Create walls.
                WorldUtils.Gen(offsetedOrigin, new Shapes.Circle(radius - 4), Actions.Chain(new GenAction[]
                {
                    new Actions.PlaceWall(WallID.Stone)
                }));

                // Place an inner luminite core.
                WorldUtils.Gen(origin, new Shapes.Circle(radius - 2), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Blotches(2, 0.3),
                    new Actions.SetTile(TileID.LunarOre, true),
                    new Actions.SetFrames()
                }));
            }

            // Generate a bunch of exodium tendrils that appear in an arced formation.
            float outwardTentacleReach = radius + 18f;
            float initialRotationalOffset = WorldGen.genRand.NextFloat(MathHelper.TwoPi);
            Vector2[] initialTendrilDirections = new Vector2[5];
            for (int i = 0; i < initialTendrilDirections.Length; i++)
            {
                // Determine the base control points for the tendril.
                // They are all determined relative to the previous point.
                Vector2[] veinDirections = new Vector2[4];

                veinDirections[0] = (MathHelper.TwoPi * i / initialTendrilDirections.Length + initialRotationalOffset).ToRotationVector2();

                // RotatedByRandom uses Main.rand, not WorldGen.genRand.
                veinDirections[0] = veinDirections[0].RotatedBy(WorldGen.genRand.NextFloat(-0.19f, 0.19f));

                for (int j = 1; j < veinDirections.Length; j++)
                    veinDirections[j] = veinDirections[j - 1].RotatedBy(WorldGen.genRand.NextFloat(-0.87f, 0.87f));

                for (int j = 0; j <= 18; j++)
                {
                    float completionRatio = j / 18f;

                    // Smoothen the control points based on a catmull-rom spline.
                    Vector2 offset = Vector2.CatmullRom(veinDirections[0], veinDirections[1], veinDirections[2], veinDirections[3], j / 18f);
                    offset *= completionRatio * outwardTentacleReach;

                    Point spawnPosition = new Point((int)(origin.X + offset.X), (int)(origin.Y + offset.Y));
                    int strength = (int)(4f - completionRatio * 4f) + 1;

                    // And generate the current tendril segment.
                    WorldUtils.Gen(spawnPosition, new Shapes.Circle(strength), Actions.Chain(new GenAction[]
                    {
                        new Actions.SetTile((ushort)ModContent.TileType<ExodiumOre>(), true)
                    }));
                }
            }

            // Smoothen the structure.
            WorldUtils.Gen(origin, new Shapes.Circle(radius + 36), Actions.Chain(new GenAction[]
            {
                new Actions.Smooth()
            }));

            // And sync the entire thing.
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendTileSquare(-1, origin.X - radius - 36, origin.Y - radius - 36, radius * 2 + 36, radius * 2 + 36);
        }
    }
}
