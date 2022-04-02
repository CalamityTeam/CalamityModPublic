using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class Flarefrost : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flarefrost");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.timeLeft = 180;
            projectile.melee = true;
        }

        public override void AI()
        {
            Vector2 value7 = new Vector2(5f, 10f);
            Lighting.AddLight(projectile.Center, 0.25f, 0f, 0.25f);
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] == 48f)
            {
                projectile.localAI[0] = 0f;
            }
            else
            {
                for (int num41 = 0; num41 < 2; num41++)
                {
                    int dustType = num41 == 0 ? 67 : 174;
                    Vector2 value8 = Vector2.UnitX * -12f;
                    value8 = -Vector2.UnitY.RotatedBy((double)(projectile.localAI[0] * 0.1308997f + (float)num41 * 3.14159274f), default) * value7;
                    int num42 = Dust.NewDust(projectile.Center, 0, 0, dustType, 0f, 0f, 160, default, 1f);
                    Main.dust[num42].scale = dustType == 67 ? 1.5f : 1f;
                    Main.dust[num42].noGravity = true;
                    Main.dust[num42].position = projectile.Center + value8;
                    Main.dust[num42].velocity = projectile.velocity;
                    int num458 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, 0f, 0f, 100, default, 0.8f);
                    Main.dust[num458].noGravity = true;
                    Main.dust[num458].velocity *= 0f;
                }
            }

            CalamityGlobalProjectile.HomeInOnNPC(projectile, !projectile.tileCollide, 250f, 12f, 20f);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item27, projectile.Center);
            for (int k = 0; k < 2; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 67, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 174, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 120);
            target.AddBuff(BuffID.Frostburn, 120);
        }
    }
}
