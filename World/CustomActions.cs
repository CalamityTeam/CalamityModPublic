
using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.WorldBuilding;

namespace CalamityMod.World
{
    public static class CustomActions
    {
        public class SolidScanner : GenAction
        {
            private int _count;

            public SolidScanner() { }

            public int GetCount()
            {
                return _count;
            }

            public override bool Apply(Point origin, int x, int y, params object[] args)
            {
                Tile tile = GenBase._tiles[x, y];
                if (tile.HasTile && Main.tileSolid[tile.TileType])
                {
                    _count++;
                }
                return base.UnitApply(origin, x, y, args);
            }
        }

        public class PlaceTree : GenAction
        {
            public PlaceTree() { }
            public override bool Apply(Point origin, int x, int y, params object[] args)
            {
                return WorldGen.GrowTree(x, y);
            }
        }

        public class SetPaint : GenAction
        {
            private readonly byte _paintID;
            public SetPaint(byte paintID) => _paintID = paintID;
            public override bool Apply(Point origin, int x, int y, params object[] args)
            {
                if (!WorldGen.InWorld(x, y))
                    return false;

                WorldGen.paintTile(x, y, _paintID);
                return UnitApply(origin, x, y, args);
            }
        }

        public class JungleGrass : GenAction
        {
            private bool _tryMushrooms;
            public JungleGrass(bool mush)
            {
                _tryMushrooms = mush;
            }
            public override bool Apply(Point origin, int x, int y, params object[] args)
            {
                if (_tiles[x, y].HasTile || _tiles[x, y - 1].HasTile)
                {
                    return false;
                }
                if (_tiles[x, y + 1].TileType == TileID.JungleGrass)
                {
                    WorldGen.PlaceTile(x, y, _random.Next(new ushort[] { TileID.JunglePlants, TileID.JunglePlants2 }), true, false, -1, 0);
                }
                else if (_tryMushrooms && _tiles[x, y + 1].TileType == TileID.MushroomGrass)
                {
                    WorldGen.PlaceTile(x, y, TileID.MushroomPlants);
                }
                return base.UnitApply(origin, x, y, args);
            }
        }

        public class RandomFrom : GenAction
        {
            private GenAction[] _actions;
            private float[] _weights;
            public RandomFrom(GenAction[] actions, float[] weights)
            {
                _actions = actions;
                _weights = new float[weights.Length];
                for (int i = 0; i < weights.Length; i++)
                {
                    if (i == 0)
                    {
                        _weights[0] = weights[0];
                        continue;
                    }
                    _weights[i] = _weights[i - 1] + weights[i];
                }
            }

            public override bool Apply(Point origin, int x, int y, params object[] args)
            {
                float number = _random.NextFloat(0f, 1f);
                int index = 0;
                while (number > _weights[index])
                {
                    index++;
                }
                return _actions[index].Apply(origin, x, y, args);
            }
        }

        public class DistanceFromOrigin : GenAction
        {
            private float _distance;
            private bool _greaterThan;

            public DistanceFromOrigin(bool greater, float distance)
            {
                _greaterThan = greater;
                _distance = distance;
            }

            public override bool Apply(Point origin, int x, int y, params object[] args)
            {
                Vector2 worldOrigin = origin.ToWorldCoordinates();
                float distance = Vector2.Distance(worldOrigin, new Vector2(x * 16 + 8, y * 16 + 8));
                if (_greaterThan && distance > _distance)
                {
                    return base.UnitApply(origin, x, y, args);
                }
                else if (distance < _distance)
                {
                    return base.UnitApply(origin, x, y, args);
                }
                return false;
            }
        }
    }
}
