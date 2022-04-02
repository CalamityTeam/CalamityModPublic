using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class EnhancedNanoRoundProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Enhanced Nano Round");
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
            projectile.extraUpdates = 3;
            aiType = ProjectileID.Bullet;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0f, 0.25f, 0.25f);

            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 4f)
            {
                if (Main.rand.NextBool(3))
                {
                    int num137 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), 1, 1, 229, 0f, 0f, 0, default, 0.5f);
                    Main.dust[num137].alpha = projectile.alpha;
                    Main.dust[num137].velocity *= 0f;
                    Main.dust[num137].noGravity = true;
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
            target.AddBuff(BuffID.Confused, 300);
            if (target.life <= 0)
            {
                if (projectile.owner == Main.myPlayer)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                        Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<Nanomachine>(), (int)(projectile.damage * 0.3), 0f, projectile.owner, 0f, 0f);
                    }
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item93, (int)projectile.position.X, (int)projectile.position.Y);
            int num212 = Main.rand.Next(5, 10);
            for (int num213 = 0; num213 < num212; num213++)
            {
                int num214 = Dust.NewDust(projectile.Center - projectile.velocity / 2f, 0, 0, 229, 0f, 0f, 100, default, 2f);
                Main.dust[num214].velocity *= 2f;
                Main.dust[num214].noGravity = true;
            }
        }
    }
}
