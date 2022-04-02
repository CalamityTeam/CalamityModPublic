using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class EventHorizonStar : ModProjectile
    {
        private bool initialized = false;
        Vector2 initialPosition;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Event Horizon Star");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 300;
            projectile.alpha = 180;
        }

        public override void AI()
        {
            //rotation
            projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.01f * (float)projectile.direction;

            //sound effects
            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = 20 + Main.rand.Next(40);
                if (Main.rand.NextBool(5))
                {
                    Main.PlaySound(SoundID.Item9, (int)projectile.position.X, (int)projectile.position.Y);
                }
            }

            //dust effects
            if (Main.rand.NextBool(10))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 262, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 0, default, 0.75f);
            }

            projectile.localAI[0]++;

            Vector2 playerCenter = Main.player[projectile.owner].Center;
            float centerX = projectile.Center.X;
            float centerY = projectile.Center.Y;

            if (!initialized)
            {
                initialPosition = playerCenter;
                initialized = true;
            }
            else if (playerCenter != initialPosition)
            {
                playerCenter = initialPosition;
            }

            float xDist = playerCenter.X - centerX;
            float yDist = playerCenter.Y - centerY;
            float radius = (float)Math.Sqrt((double)(xDist * xDist + yDist * yDist));

            if (projectile.localAI[0] > 10 && projectile.localAI[0] < 100)
            {
                projectile.ai[1] += 1f / 60f;

                if (projectile.ai[1] > 0)
                {
                    projectile.ai[0] += MathHelper.ToRadians(5f) / projectile.ai[1];
                    projectile.Center = playerCenter + projectile.ai[0].ToRotationVector2() * radius;
                }
            }

            //homing
            if (projectile.localAI[0] >= 100)
            {
                Vector2 center = projectile.Center;
                float homingRange = 325f;
                bool homeIn = false;
                float inertia = 25f;
                float homingSpeed = 23f;

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].CanBeChasedBy(projectile, false))
                    {
                        float extraDistance = (float)(Main.npc[i].width / 2) + (float)(Main.npc[i].height / 2);

                        if (Vector2.Distance(Main.npc[i].Center, projectile.Center) < (homingRange + extraDistance))
                        {
                            center = Main.npc[i].Center;
                            homeIn = true;
                            break;
                        }
                    }
                }

                if (homeIn)
                {
                    projectile.extraUpdates = 1;
                    Vector2 homeInVector = projectile.SafeDirectionTo(center, Vector2.UnitY);

                    projectile.velocity = (projectile.velocity * inertia + homeInVector * homingSpeed) / (inertia + 1f);
                }
                else
                    projectile.extraUpdates = 0;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Daybreak, 120);
            Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<EventHorizonBlackhole>(), (int)(projectile.damage * 0.5f), projectile.knockBack * 0.5f, projectile.owner, 0f, 0f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, 0, texture.Width, texture.Height)), new Color(255, 255, 255, 127), projectile.rotation, texture.Size() / 2f, projectile.scale, spriteEffects, 0f);
        }
    }
}
