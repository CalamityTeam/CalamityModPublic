using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Walls
{
    public class NavystoneWall : ModWall
    {
		private Rectangle _area;

		public override void SetDefaults()
        {
            dustType = 96;
        }

		public override void RandomUpdate(int i, int j)
		{
			int myRadius = 5;
			int diameter = myRadius * 2;
			_area = new Rectangle(i - myRadius, j - myRadius, diameter, diameter);
			if (CalamityWorld.SunkenSeaLocation.Contains(_area))
			{
				for (int x = _area.Left; x < _area.Width; x++)
				{
					for (int y = _area.Top; y < _area.Height; y++)
					{
						if (Main.tile[x, y].liquid <= 0)
						{
							Main.tile[i, j].liquid = 255;
							Main.tile[i, j].lava(false);
						}
					}
				}
			}
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}