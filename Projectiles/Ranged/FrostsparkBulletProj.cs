using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class FrostsparkBulletProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frostspark Bullet");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.aiStyle = 1;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.extraUpdates = 1;
            aiType = ProjectileID.Bullet;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.15f, 0.05f, 0.3f);

			projectile.localAI[0] += 1f;
			if (projectile.localAI[0] > 4f)
			{
				if (Main.rand.NextBool())
				{
					int dustType = 15;
					float spacing = Main.rand.NextFloat(-0.2f, 0.8f);
					int dust = Dust.NewDust(projectile.Center - spacing * projectile.velocity, 1, 1, dustType); ;
					Main.dust[dust].position = projectile.Center;
					Main.dust[dust].velocity *= 0.4f;
					Main.dust[dust].velocity += projectile.velocity * 0.7f;
					Main.dust[dust].noGravity = true;
				}
			}
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesFromEdge(projectile, 0, lightColor);
            return false;
        }

        // This projectile is always fullbright.
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(1f, 1f, 1f, 0f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 240);
            target.AddBuff(BuffID.Frostburn, 240);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 120);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item27, projectile.position);

            projectile.damage /= 3;
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 24;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();

            int num212 = Main.rand.Next(10, 20);
            for (int num213 = 0; num213 < num212; num213++)
            {
                int dustType = Main.rand.Next(2);
                if (dustType == 0)
                {
                    dustType = 67;
                }
                else
                {
                    dustType = 6;
                }
                int num214 = Dust.NewDust(projectile.Center - projectile.velocity / 2f, 0, 0, dustType, 0f, 0f, 100, default, 2f);
                Main.dust[num214].velocity *= 2f;
                Main.dust[num214].noGravity = true;
            }
        }
    }
}
