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
            Main.projFrames[Projectile.type] = NumAnimationFrames;
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = Lifespan;
            Projectile.ignoreWater = true;
        }

        private bool HomingAI()
        {
            // Save current speed to AI variables
            float speed = Projectile.velocity.Length();
            if (Projectile.ai[0] == 0f)
                Projectile.ai[0] = speed;

            // Variables for choosing an NPC to home in on
            float posX = Projectile.position.X;
            float posY = Projectile.position.Y;
            float homingRange = HomingStartRange;
            bool foundTarget = false;
            int targetIndex = 0;

            // ai[1] stores the index of the current target plus one. 0 is none.
            if (Projectile.ai[1] == 0f)
            {
                // Find the closest target. The range decreases each time you find a closer target.
                for (int i = 0; i < Main.maxNPCs; ++i)
                {
                    if (Main.npc[i].CanBeChasedBy(Projectile, false))
                    {
                        // Chlorophyte Bullets actually home in a diamond instead of a circle because they don't perform a square root here.
                        float d = Math.Abs(Projectile.Center.X - Main.npc[i].Center.X) + Math.Abs(Projectile.Center.Y - Main.npc[i].Center.Y);
                        if (d < homingRange && Collision.CanHit(Projectile.Center, 1, 1, Main.npc[i].position, Main.npc[i].width, Main.npc[i].height))
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
                    Projectile.ai[1] = (float)(targetIndex + 1);
            }

            // This is always the case after that loop for some reason.
            foundTarget = false;

            // Ensure the current target is still valid
            if (Projectile.ai[1] > 0f)
            {
                targetIndex = (int)(Projectile.ai[1] - 1f);

                // Chlorophyte Bullets will continue homing in unless their target is invincible.
                if (Main.npc[targetIndex].active && Main.npc[targetIndex].CanBeChasedBy(Projectile, false))
                {
                    float targetX = Main.npc[targetIndex].Center.X;
                    float targetY = Main.npc[targetIndex].Center.Y;
                    float d = Math.Abs(Projectile.Center.X - targetX) + Math.Abs(Projectile.Center.Y - targetY);
                    if (d < HomingBreakRange)
                    {
                        foundTarget = true;
                        posX = targetX;
                        posY = targetY;
                    }
                }
                else
                    Projectile.ai[1] = 0f;
            }

            // Non-friendly Chlorophyte Bullets will never actually home in, despite maintaining a target.
            if (foundTarget && Projectile.friendly)
            {
                float dx = posX - Projectile.Center.X;
                float dy = posY - Projectile.Center.Y;
                float distance = (float)Math.Sqrt(dx * dx + dy * dy);
                float homingFactor = speed / distance;
                dx *= homingFactor;
                dy *= homingFactor;

                // The projectile loses 1/5th of its current velocity and replaces it with homing power.
                float homingStrength = 5f;
                Projectile.velocity.X = (Projectile.velocity.X * (homingStrength - 1f) + dx) / homingStrength;
                Projectile.velocity.Y = (Projectile.velocity.Y * (homingStrength - 1f) + dy) / homingStrength;

                // And then approaches the homing velocity by an increment of 0.3.
                Projectile.velocity = Projectile.velocity.MoveTowards(new Vector2(dx, dy), 0.3f);

                // Re-scale resulting velocity to the projectile's set maximum speed
                Projectile.velocity.Normalize();
                Projectile.velocity *= speed;
                return true;
            }
            return false;
        }

        public override void AI()
        {
            drawOffsetX = -28;
            drawOriginOffsetY = -2;
            drawOriginOffsetX = 12;
            Projectile.rotation = Projectile.velocity.ToRotation();

            // Run homing code
            bool homing = HomingAI();

            // Light
            Lighting.AddLight(Projectile.Center, 0.3f, 0.1f, 0.45f);

            // Spawn dust with a 3/4 chance if currently homing in
            if (homing && Main.rand.Next(4) != 3)
            {
                int idx = Dust.NewDust(Projectile.Center, 1, 1, 70);
                Main.dust[idx].position = Projectile.Center;
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 0.25f;
            }

            // Update animation
            Projectile.frameCounter++;
            if (Projectile.frameCounter > AnimationFrameTime)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= NumAnimationFrames)
                Projectile.frame = 0;
        }

        // Cannot deal damage for the first several frames of existence.
        public override bool CanDamage()
        {
            return Projectile.timeLeft < Lifespan - IntangibleFrames;
        }
    }
}
