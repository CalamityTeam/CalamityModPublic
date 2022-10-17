using System.Collections.Generic;
using System.Linq;
using CalamityMod.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class SulphuricWaterSafeZoneSystem : ModSystem
    {
        public static Dictionary<Point, float> NearbySafeTiles
        {
            get;
            set;
        } = new();

        public override void PreUpdateEntities()
        {
            // Make the potency of the current safe tiles gradually dissipate, and eventually disappear from the dictionary once completely gone.
            foreach (Point p in NearbySafeTiles.Keys)
                NearbySafeTiles[p] -= 0.06f;
            foreach (var key in NearbySafeTiles.Keys.Where(key => NearbySafeTiles[key] <= 0f))
                NearbySafeTiles.Remove(key);

            ushort safeTileID = (ushort)ModContent.TileType<AuricTeslaBar>();
            Point center = Main.LocalPlayer.Center.ToTileCoordinates();

            for (int dx = -56; dx < 56; dx++)
            {
                for (int dy = -56; dy < 56; dy++)
                {
                    Point p = new(center.X + dx, center.Y + dy);
                    Tile t = Main.tile[p];

                    if (!t.HasTile)
                        continue;

                    if (t.TileType != safeTileID)
                        continue;

                    NearbySafeTiles[p] = 1f;
                }
            }
        }
    }
}
