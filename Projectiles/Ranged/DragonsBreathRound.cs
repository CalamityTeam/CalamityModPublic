using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class DragonsBreathRound : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dragon's Breath Round");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.extraUpdates = 10;
            projectile.penetrate = 1;
            projectile.timeLeft = 180;
            projectile.ranged = true;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void AI()
        {
            //Rotation
            projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi) +MathHelper.ToRadians(90) * projectile.direction;

            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 4f)
            {
                float num93 = projectile.velocity.X / 3f;
                float num94 = projectile.velocity.Y / 3f;
                int num95 = 4;
                int idx = Dust.NewDust(new Vector2(projectile.position.X + (float)num95, projectile.position.Y + (float)num95), projectile.width - num95 * 2, projectile.height - num95 * 2, 127, 0f, 0f, 100, default, 2f);
                Dust dust = Main.dust[idx];
                dust.noGravity = true;
                dust.velocity *= 0.1f;
                dust.velocity += projectile.velocity * 0.1f;
                dust.position.X -= num93;
                dust.position.Y -= num94;

                if (Main.rand.NextBool(20))
                {
                    int num97 = 4;
                    int num98 = Dust.NewDust(new Vector2(projectile.position.X + (float)num97, projectile.position.Y + (float)num97), projectile.width - num97 * 2, projectile.height - num97 * 2, 127, 0f, 0f, 100, default, 0.6f);
                    Main.dust[num98].velocity *= 0.25f;
                    Main.dust[num98].velocity += projectile.velocity * 0.5f;
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            if (projectile.owner == Main.myPlayer)
            {
                int proj = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<FuckYou>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
                if (proj.WithinBounds(Main.maxProjectiles))
                    Main.projectile[proj].Calamity().forceRanged = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 300);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor);
            return false;
        }
    }
}
