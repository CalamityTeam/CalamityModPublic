using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    // TODO -- Abstract away most of the direct vector math away in here into things like SafeDirectionTo calls.
    public class AtaraxiaHoming : ModProjectile
    {
        private static int NumAnimationFrames = 5;
        private static int AnimationFrameTime = 9;
        private static float HomingStartRange = 300f;
        private static float HomingBreakRange = 1000f;

        private static int Lifespan = 180;
        private static int IntangibleFrames = 12;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Definitely Not Exoblade");
            Main.projFrames[projectile.type] = NumAnimationFrames;
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = 1;
            projectile.extraUpdates = 1;
            projectile.timeLeft = Lifespan;
            projectile.ignoreWater = true;
        }

        private bool HomingAI()
        {
            // Save current speed to AI variables
            float speed = projectile.velocity.Length();
            if (projectile.ai[0] == 0f)
                projectile.ai[0] = speed;

            // Variables for choosing an NPC to home in on
            float posX = projectile.position.X;
            float posY = projectile.position.Y;
            float homingRange = HomingStartRange;
            bool foundTarget = false;
            int targetIndex = 0;

            // ai[1] stores the index of the current target plus one. 0 is none.
            if (projectile.ai[1] == 0f)
            {
                // Find the closest target. The range decreases each time you find a closer target.
                for (int i = 0; i < Main.maxNPCs; ++i)
                {
                    if (Main.npc[i].CanBeChasedBy(projectile, false))
                    {
                        // Chlorophyte Bullets actually home in a diamond instead of a circle because they don't perform a square root here.
                        float d = Math.Abs(projectile.Center.X - Main.npc[i].Center.X) + Math.Abs(projectile.Center.Y - Main.npc[i].Center.Y);
                        if (d < homingRange && Collision.CanHit(projectile.Center, 1, 1, Main.npc[i].position, Main.npc[i].width, Main.npc[i].height))
                        {
                            homingRange = d;
                            posX = Main.npc[i].Center.X;
                            posY = Main.npc[i].Center.Y;
                            foundTarget = true;
                            targetIndex = i;
                        }
                    }
                }

                // If at least one valid target was found, set ai[1] to store its index plus one.
                if (foundTarget)
                    projectile.ai[1] = (float)(targetIndex + 1);
            }

            // This is always the case after that loop for some reason.
            foundTarget = false;

            // Ensure the current target is still valid
            if (projectile.ai[1] > 0f)
            {
                targetIndex = (int)(projectile.ai[1] - 1f);

                // Chlorophyte Bullets will continue homing in unless their target is invincible.
                if (Main.npc[targetIndex].active && Main.npc[targetIndex].CanBeChasedBy(projectile, false))
                {
                    float targetX = Main.npc[targetIndex].Center.X;
                    float targetY = Main.npc[targetIndex].Center.Y;
                    float d = Math.Abs(projectile.Center.X - targetX) + Math.Abs(projectile.Center.Y - targetY);
                    if (d < HomingBreakRange)
                    {
                        foundTarget = true;
                        posX = targetX;
                        posY = targetY;
                    }
                }
                else
                    projectile.ai[1] = 0f;
            }

            // Non-friendly Chlorophyte Bullets will never actually home in, despite maintaining a target.
            if (foundTarget && projectile.friendly)
            {
                float dx = posX - projectile.Center.X;
                float dy = posY - projectile.Center.Y;
                float distance = (float)Math.Sqrt(dx * dx + dy * dy);
                float homingFactor = speed / distance;
                dx *= homingFactor;
                dy *= homingFactor;

                // The projectile loses 1/5th of its current velocity and replaces it with homing power.
                float homingStrength = 5f;
                projectile.velocity.X = (projectile.velocity.X * (homingStrength - 1f) + dx) / homingStrength;
                projectile.velocity.Y = (projectile.velocity.Y * (homingStrength - 1f) + dy) / homingStrength;

                // And then approaches the homing velocity by an increment of 0.3.
                projectile.velocity = projectile.velocity.MoveTowards(new Vector2(dx, dy), 0.3f);

                // Re-scale resulting velocity to the projectile's set maximum speed
                projectile.velocity.Normalize();
                projectile.velocity *= speed;
                return true;
            }
            return false;
        }

        public override void AI()
        {
            drawOffsetX = -28;
            drawOriginOffsetY = -2;
            drawOriginOffsetX = 12;
            projectile.rotation = projectile.velocity.ToRotation();

            // Run homing code
            bool homing = HomingAI();

            // Light
            Lighting.AddLight(projectile.Center, 0.3f, 0.1f, 0.45f);

            // Spawn dust with a 3/4 chance if currently homing in
            if (homing && Main.rand.Next(4) != 3)
            {
                int idx = Dust.NewDust(projectile.Center, 1, 1, 70);
                Main.dust[idx].position = projectile.Center;
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 0.25f;
            }

            // Update animation
            projectile.frameCounter++;
            if (projectile.frameCounter > AnimationFrameTime)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= NumAnimationFrames)
                projectile.frame = 0;
        }

        // Cannot deal damage for the first several frames of existence.
        public override bool CanDamage()
        {
            return projectile.timeLeft < Lifespan - IntangibleFrames;
        }
    }
}
