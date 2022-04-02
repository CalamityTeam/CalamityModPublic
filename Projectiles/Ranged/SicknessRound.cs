using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class SicknessRound : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Round");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
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
            projectile.light = 0.25f;
            projectile.extraUpdates = 4;
            aiType = ProjectileID.Bullet;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override bool PreAI()
        {
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 4f)
            {
                Vector2 dspeed = -projectile.velocity * 0.5f;
                float x2 = projectile.Center.X - projectile.velocity.X / 10f;
                float y2 = projectile.Center.Y - projectile.velocity.Y / 10f;
                int num137 = Dust.NewDust(new Vector2(x2, y2), 1, 1, 107, dspeed.X, dspeed.Y, 0, default, 1f);
                Main.dust[num137].alpha = projectile.alpha;
                Main.dust[num137].position.X = x2;
                Main.dust[num137].position.Y = y2;
                Main.dust[num137].velocity = dspeed;
                Main.dust[num137].noGravity = true;
            }

            //Rotation
            projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = (projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi)) + MathHelper.ToRadians(90) * projectile.direction;

            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 180);
        }

        public override void Kill(int timeLeft)
        {
            if (projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<Sickness>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                for (int k = 0; k < 3; k++)
                {
                    Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 107, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, Main.rand.Next(-35, 36) * 0.2f, Main.rand.Next(-35, 36) * 0.2f, ModContent.ProjectileType<SicknessRound2>(),
                    (int)(projectile.damage * 0.5), projectile.knockBack * 0.5f, Main.myPlayer);
                }
            }
        }
    }
}
