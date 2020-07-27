using CalamityMod.Projectiles.Enemy;
using CalamityMod.Tiles;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace CalamityMod.TileEntities
{
    public class TEDraedonTurret : TEBaseTurret
    {
        public override int ShootWaitTime => 15;
        public override int ShootRate => 90;
        public override int ProjectileDamage => 20;
        public override float ShootSpeed => 5f;
        public override float ShootSpawnOffset => 32f;
        public override float ProjectileKnockback => 3f;
        public override int TileType => ModContent.TileType<DraedonTurretTile>();
        public override int ProjectileShot => ModContent.ProjectileType<DraedonLaser>();
        public override Vector2 ShootCoordsOffset => new Vector2(22f + 4f * Direction, -2f);
        public override float TargetDistanceCheck => 600f;
    }
}
