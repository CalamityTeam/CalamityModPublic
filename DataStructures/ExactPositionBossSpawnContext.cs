using Microsoft.Xna.Framework;

namespace CalamityMod.DataStructures
{
    public class ExactPositionBossSpawnContext : BaseBossSpawnContext
    {
        public override Vector2 DetermineSpawnPosition(Vector2 relativePosition) => relativePosition;
    }
}
