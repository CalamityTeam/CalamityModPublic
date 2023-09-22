using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class BlackHawkBullet : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.light = 0.5f;
            Projectile.alpha = 0;
            Projectile.extraUpdates = 4;
            Projectile.scale = 1.18f;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            AIType = ProjectileID.BulletHighVelocity;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, 200);
    }
}
