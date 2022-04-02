using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class BouncingShotgunPellet : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Ranged/RealmRavagerBullet";
        private int bounce = 2;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shotgun Pellet");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.light = 0.5f;
            projectile.alpha = 255;
            projectile.ranged = true;
            projectile.friendly = true;
            projectile.aiStyle = 1;
            aiType = ProjectileID.BulletHighVelocity;
            projectile.timeLeft = 180;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            bounce--;
            if (bounce <= 0)
                projectile.Kill();
            else
            {
                if (projectile.velocity.X != oldVelocity.X)
                    projectile.velocity.X = -oldVelocity.X;
                if (projectile.velocity.Y != oldVelocity.Y)
                    projectile.velocity.Y = -oldVelocity.Y;
                Main.PlaySound(SoundID.Item10, projectile.Center);
                projectile.damage /= 2;
            }
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => Main.PlaySound(SoundID.Item10, projectile.Center);

        public override void OnHitPvp(Player target, int damage, bool crit) => Main.PlaySound(SoundID.Item10, projectile.Center);

        public override void Kill(int timeLeft)
        {
            CalamityGlobalProjectile.ExpandHitboxBy(projectile, 32);
            for (int d = 0; d < 2; d++)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Smoke, 0f, 0f, 100, default, 1.5f);
            }
            for (int d = 0; d < 20; d++)
            {
                int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire, 0f, 0f, 0, default, 2.5f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 3f;
                idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire, 0f, 0f, 100, default, 1.5f);
                Main.dust[idx].velocity *= 2f;
                Main.dust[idx].noGravity = true;
            }
        }
    }
}
