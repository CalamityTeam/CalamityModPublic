using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace CalamityMod.DataStructures
{
    public class OffscreenBossSpawnContext : BaseBossSpawnContext
    {
        public override Vector2 DetermineSpawnPosition(Vector2 relativePosition)
        {
            Vector2 exactPosition = relativePosition;

            // Determine the position based on a circlular offset that is always 10% away from the screen points.
            exactPosition += Main.rand.NextVector2CircularEdge(Main.screenWidth, Main.screenHeight) * (float)Math.Sqrt(2f) * 1.1f * 0.5f;
            return exactPosition;
        }
    }
}
