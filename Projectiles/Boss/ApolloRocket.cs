using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class ApolloRocket : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("High Explosive Plasma Rocket");
            Main.projFrames[projectile.type] = 5;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.Calamity().canBreakPlayerDefense = true;
            projectile.width = 28;
            projectile.height = 28;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            cooldownSlot = 1;
            projectile.timeLeft = 600;
            projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
        }

        public override void AI()
        {
            if (projectile.position.Y > projectile.ai[1])
                projectile.tileCollide = true;

            // Animation
            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 4)
                projectile.frame = 0;

            // Rotation
            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;

            // Spawn effects
            if (projectile.localAI[0] == 0f)
            {
                projectile.localAI[0] = 1f;

                float speed1 = 1.8f;
                float speed2 = 2.8f;
                float angleRandom = 0.35f;

                for (int num53 = 0; num53 < 20; num53++)
                {
                    float dustSpeed = Main.rand.NextFloat(speed1, speed2);
                    Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(projectile.velocity.ToRotation());
                    dustVel = dustVel.RotatedBy(-angleRandom);
                    dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);
                    int randomDustType = Main.rand.NextBool(2) ? 107 : 110;

                    int num54 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, randomDustType, dustVel.X, dustVel.Y, 200, default, 1.7f);
                    Main.dust[num54].position = projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * projectile.width / 2f;
                    Main.dust[num54].noGravity = true;

                    Dust dust = Main.dust[num54];
                    dust.velocity *= 3f;
                    dust = Main.dust[num54];

                    num54 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, randomDustType, dustVel.X, dustVel.Y, 100, default, 0.8f);
                    Main.dust[num54].position = projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * projectile.width / 2f;

                    dust = Main.dust[num54];
                    dust.velocity *= 2f;

                    Main.dust[num54].noGravity = true;
                    Main.dust[num54].fadeIn = 1f;
                    Main.dust[num54].color = Color.Green * 0.5f;

                    dust = Main.dust[num54];
                }
                for (int num55 = 0; num55 < 10; num55++)
                {
                    float dustSpeed = Main.rand.NextFloat(speed1, speed2);
                    Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(projectile.velocity.ToRotation());
                    dustVel = dustVel.RotatedBy(-angleRandom);
                    dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);
                    int randomDustType = Main.rand.NextBool(2) ? 107 : 110;

                    int num56 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, randomDustType, dustVel.X, dustVel.Y, 0, default, 2f);
                    Main.dust[num56].position = projectile.Center + Vector2.UnitX.RotatedByRandom(MathHelper.Pi).RotatedBy(projectile.velocity.ToRotation()) * projectile.width / 3f;
                    Main.dust[num56].noGravity = true;

                    Dust dust = Main.dust[num56];
                    dust.velocity *= 0.5f;
                    dust = Main.dust[num56];
                }
            }

            // Difficulty modes
            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;

            // Light
            Lighting.AddLight(projectile.Center, 0f, 0.6f, 0f);

            // Get a target and calculate distance from it
            int target = Player.FindClosest(projectile.Center, 1, 1);
            Vector2 distanceFromTarget = Main.player[target].Center - projectile.Center;

            // Set AI to stop homing, start accelerating
            float stopHomingDistance = malice ? 120f : death ? 160f : revenge ? 180f : expertMode ? 200f : 240f;
            if (distanceFromTarget.Length() < stopHomingDistance || projectile.ai[0] == 1f || projectile.timeLeft < 480)
            {
                projectile.ai[0] = 1f;

                if (projectile.velocity.Length() < 24f)
                    projectile.velocity *= 1.05f;

                return;
            }

            // Home in on target
            float scaleFactor = projectile.velocity.Length();
            float inertia = malice ? 6f : death ? 8f : revenge ? 9f : expertMode ? 10f : 12f;
            distanceFromTarget.Normalize();
            distanceFromTarget *= scaleFactor;
            projectile.velocity = (projectile.velocity * inertia + distanceFromTarget) / (inertia + 1f);
            projectile.velocity.Normalize();
            projectile.velocity *= scaleFactor;

            // Fly away from other rockets
            float pushForce = malice ? 0.07f : death ? 0.06f : revenge ? 0.055f : expertMode ? 0.05f : 0.04f;
            float pushDistance = malice ? 120f : death ? 100f : revenge ? 90f : expertMode ? 80f : 60f;
            for (int k = 0; k < Main.maxProjectiles; k++)
            {
                Projectile otherProj = Main.projectile[k];
                // Short circuits to make the loop as fast as possible
                if (!otherProj.active || k == projectile.whoAmI)
                    continue;

                // If the other projectile is indeed the same owned by the same player and they're too close, nudge them away.
                bool sameProjType = otherProj.type == projectile.type;
                float taxicabDist = Vector2.Distance(projectile.Center, otherProj.Center);
                if (sameProjType && taxicabDist < pushDistance)
                {
                    if (projectile.position.X < otherProj.position.X)
                        projectile.velocity.X -= pushForce;
                    else
                        projectile.velocity.X += pushForce;

                    if (projectile.position.Y < otherProj.position.Y)
                        projectile.velocity.Y -= pushForce;
                    else
                        projectile.velocity.Y += pushForce;
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 360);
            target.AddBuff(BuffID.CursedInferno, 180);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            int height = texture.Height / Main.projFrames[projectile.type];
            int drawStart = height * projectile.frame;
            Vector2 origin = projectile.Size / 2;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Projectiles/Boss/ApolloRocketGlow"), projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, drawStart, texture.Width, height)), Color.White, projectile.rotation, origin, projectile.scale, spriteEffects, 0f);
        }

        public override void Kill(int timeLeft)
        {
            // Rocket explosion
            int height = 90;
            projectile.position = projectile.Center;
            projectile.width = projectile.height = height;
            projectile.Center = projectile.position;
            projectile.Damage();

            Main.PlaySound(SoundID.Item14, projectile.Center);

            for (int num621 = 0; num621 < 12; num621++)
            {
                int randomDustType = Main.rand.NextBool(2) ? 107 : 110;
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, randomDustType, 0f, 0f, 100, default, 2f);
                Main.dust[dust].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[dust].scale = 0.5f;
                    Main.dust[dust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 15; num623++)
            {
                int randomDustType = Main.rand.NextBool(2) ? 107 : 110;
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, randomDustType, 0f, 0f, 100, default, 3f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 5f;
                dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, randomDustType, 0f, 0f, 100, default, 2f);
                Main.dust[dust].velocity *= 2f;
            }
            CalamityUtils.ExplosionGores(projectile.Center, 3);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            target.Calamity().lastProjectileHit = projectile;
        }
    }
}
