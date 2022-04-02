using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class FlareBomb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flare Bomb");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.Calamity().canBreakPlayerDefense = true;
            projectile.width = 64;
            projectile.height = 66;
            projectile.hostile = true;
            projectile.scale = 1.5f;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.alpha = 50;
            projectile.timeLeft = 180;
            cooldownSlot = 1;
        }

        public override void AI()
        {
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;

            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
                projectile.frame = 0;

            Lighting.AddLight(projectile.Center, 0.5f, 0.25f, 0f);

            if (projectile.timeLeft > 30 && projectile.alpha > 0)
                projectile.alpha -= 25;
            if (projectile.timeLeft > 30 && projectile.alpha < 128 && Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                projectile.alpha = 128;
            if (projectile.alpha < 0)
                projectile.alpha = 0;

            if (projectile.ai[0] == -1f || (projectile.timeLeft > 135 && projectile.ai[1] == 1f))
                return;

            float inertia = revenge ? 70f : 77f;
            float num954 = 40f;
            float scaleFactor12 = revenge ? 35f : 28f;
            int num959 = (int)projectile.ai[0];
            if (num959 >= 0 && Main.player[num959].active && !Main.player[num959].dead)
            {
                if (projectile.Distance(Main.player[num959].Center) > num954)
                {
                    Vector2 moveDirection = projectile.SafeDirectionTo(Main.player[num959].Center, Vector2.UnitY);
                    projectile.velocity = (projectile.velocity * (inertia - 1f) + moveDirection * scaleFactor12) / inertia;
                }
            }
            else
            {
                if (projectile.ai[0] != -1f)
                {
                    projectile.ai[0] = -1f;
                    projectile.netUpdate = true;
                }
            }

            if (projectile.timeLeft < 60)
                return;

            float num1247 = 0.5f;
            for (int num1248 = 0; num1248 < Main.maxProjectiles; num1248++)
            {
                if (Main.projectile[num1248].active)
                {
                    if (num1248 != projectile.whoAmI && Main.projectile[num1248].type == projectile.type)
                    {
                        if (Vector2.Distance(projectile.Center, Main.projectile[num1248].Center) < 24f)
                        {
                            if (projectile.position.X < Main.projectile[num1248].position.X)
                                projectile.velocity.X -= num1247;
                            else
                                projectile.velocity.X += num1247;

                            if (projectile.position.Y < Main.projectile[num1248].position.Y)
                                projectile.velocity.Y -= num1247;
                            else
                                projectile.velocity.Y += num1247;
                        }
                    }
                }
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, projectile.alpha);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y6 = num214 * projectile.frame;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Rectangle(0, y6, texture2D13.Width, num214), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2(texture2D13.Width / 2f, num214 / 2f), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, projectile.Center);
            CalamityGlobalProjectile.ExpandHitboxBy(projectile, 48);
            for (int d = 0; d < 2; d++)
            {
                int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 100, default, 1f);
                Main.dust[idx].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[idx].scale = 0.5f;
                    Main.dust[idx].fadeIn = 1f + Main.rand.NextFloat(0.1f, 1f);
                }
            }
            for (int d = 0; d < 4; d++)
            {
                int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 100, default, 2f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 5f;
                idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 100, default, 1f);
                Main.dust[idx].velocity *= 2f;
            }
            CalamityUtils.ExplosionGores(projectile.Center, 3);
            projectile.Damage();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(projectile.Center, 16f * projectile.scale, targetHitbox);

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<LethalLavaBurn>(), 180);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)    
        {
            target.Calamity().lastProjectileHit = projectile;
        }
    }
}
