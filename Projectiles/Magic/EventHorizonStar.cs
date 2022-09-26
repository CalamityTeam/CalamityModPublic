using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class EventHorizonStar : ModProjectile
    {
        private bool initialized = false;
        Vector2 initialPosition;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Event Horizon Star");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.alpha = 180;
        }

        public override void AI()
        {
            //rotation
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;

            //sound effects
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 20 + Main.rand.Next(40);
                if (Main.rand.NextBool(5))
                {
                    SoundEngine.PlaySound(SoundID.Item9, Projectile.position);
                }
            }

            //dust effects
            if (Main.rand.NextBool(10))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 262, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 0, default, 0.75f);
            }

            Projectile.localAI[0]++;

            Vector2 playerCenter = Main.player[Projectile.owner].Center;
            float centerX = Projectile.Center.X;
            float centerY = Projectile.Center.Y;

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

            if (Projectile.localAI[0] > 10 && Projectile.localAI[0] < 100)
            {
                Projectile.ai[1] += 1f / 60f;

                if (Projectile.ai[1] > 0)
                {
                    Projectile.ai[0] += MathHelper.ToRadians(5f) / Projectile.ai[1];
                    Projectile.Center = playerCenter + Projectile.ai[0].ToRotationVector2() * radius;
                }
            }

            //homing
            if (Projectile.localAI[0] >= 100)
            {
                Vector2 center = Projectile.Center;
                float homingRange = 325f;
                bool homeIn = false;
                float inertia = 25f;
                float homingSpeed = 23f;

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].CanBeChasedBy(Projectile, false))
                    {
                        float extraDistance = (float)(Main.npc[i].width / 2) + (float)(Main.npc[i].height / 2);

                        if (Vector2.Distance(Main.npc[i].Center, Projectile.Center) < (homingRange + extraDistance))
                        {
                            center = Main.npc[i].Center;
                            homeIn = true;
                            break;
                        }
                    }
                }

                if (homeIn)
                {
                    Projectile.extraUpdates = 1;
                    Vector2 homeInVector = Projectile.SafeDirectionTo(center, Vector2.UnitY);

                    Projectile.velocity = (Projectile.velocity * inertia + homeInVector * homingSpeed) / (inertia + 1f);
                }
                else
                    Projectile.extraUpdates = 0;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Daybreak, 120);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<EventHorizonBlackhole>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack * 0.5f, Projectile.owner, 0f, 0f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, 0, texture.Width, texture.Height)), new Color(255, 255, 255, 127), Projectile.rotation, texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
        }
    }
}
