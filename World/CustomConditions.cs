
using Terraria;
using Terraria.ID;
using Terraria.WorldBuilding;

namespace CalamityMod.World
{
    public static class CustomConditions
    {
        public class RandomChance : GenCondition
        {
            private float oneInThisValue;

            public RandomChance(float chance)
            {
                oneInThisValue = chance;
            }

            protected override bool CheckValidity(int x, int y)
            {
                return _random.NextFloat(oneInThisValue) <= 1f;
            }
        }

        public class IsWater : GenCondition
        {
            protected override bool CheckValidity(int x, int y)
            {
                Tile tile = CalamityUtils.ParanoidTileRetrieval(x, y);
                return tile.LiquidAmount >= 200 && !tile.honey() && !tile.lava();
            }
        }

        public class SolidOrPlatform : GenCondition
        {
            protected override bool CheckValidity(int x, int y) => TileID.Sets.Platforms[CalamityUtils.ParanoidTileRetrieval(x, y).TileType] || WorldGen.SolidTile(x, y);
        }

        public class IsNotTouchingAir : GenCondition
        {
            private bool _useDiagonals;
            public IsNotTouchingAir(bool diagonals)
            {
                _useDiagonals = diagonals;
            }
            protected override bool CheckValidity(int x, int y)
            {
                for (int i = x - 1; i <= x + 1; i++)
                {
                    for (int j = y - 1; j <= y + 1; j++)
                    {
                        if (j != y && i != x && !_useDiagonals)
                            continue;
                        if (!WorldGen.InWorld(i, j))
                            continue;
                        if (!_tiles[i, j].HasTile)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }
    }
}
