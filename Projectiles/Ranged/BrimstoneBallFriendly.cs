using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class BrimstoneBallFriendly : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ball");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.scale = 1.2f;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 6;
            projectile.aiStyle = 14;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
            projectile.rotation += 0.12f * projectile.direction;
            Lighting.AddLight(projectile.Center, 0.25f, 0f, 0f);
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 4f)
            {
                int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, (int)CalamityDusts.Brimstone, 0f, 0f, 150, default, 1.5f);
                Main.dust[num469].noGravity = true;
                Main.dust[num469].velocity *= 0f;

            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.penetrate--;
            if (projectile.penetrate <= 0)
            {
                projectile.Kill();
            }
            else
            {
                if (projectile.velocity.X != oldVelocity.X)
                {
                    projectile.velocity.X = -oldVelocity.X;
                }
                if (projectile.velocity.Y != oldVelocity.Y)
                {
                    projectile.velocity.Y = -oldVelocity.Y;
                }
                projectile.velocity *= 0.98f;
            }
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 5;
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 180);
        }
    }
}
