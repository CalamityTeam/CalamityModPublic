using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class GlaiveProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Glaive";

        private static int Lifetime = 180;
        private static int ReboundTime = 45;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Glaive");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.tileCollide = true;
            projectile.penetrate = 3;
            projectile.timeLeft = Lifetime;
            drawOffsetX = -10;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            // ai[1] = 1 means that the projectile is a stealth strike, in which case it pierces infinitely.
            if (projectile.ai[1] == 1f)
                projectile.penetrate = projectile.maxPenetrate = -1;

            // Boomerang rotation
            projectile.rotation += 0.4f * (float)projectile.direction;

            // Boomerang sound
            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = 8;
                Main.PlaySound(SoundID.Item7, projectile.position);
            }

            // Returns after some number of frames in the air
            if (projectile.timeLeft < Lifetime - ReboundTime)
                projectile.ai[0] = 1f;

            if (projectile.ai[0] != 0f)
            {
                projectile.tileCollide = false;

                float returnSpeed = Glaive.Speed * 1.6f;
                float acceleration = 1.4f;

                if (projectile.ai[1] == 1f)
                {
                    returnSpeed *= Glaive.StealthSpeedMult;
                    acceleration *= Glaive.StealthSpeedMult;
                }

                Player owner = Main.player[projectile.owner];

                // Delete the projectile if it's excessively far away.
                Vector2 playerCenter = owner.Center;
                float xDist = playerCenter.X - projectile.Center.X;
                float yDist = playerCenter.Y - projectile.Center.Y;
                float dist = (float)Math.Sqrt((double)(xDist * xDist + yDist * yDist));
                if (dist > 3000f)
                    projectile.Kill();

                dist = returnSpeed / dist;
                xDist *= dist;
                yDist *= dist;

                // Home back in on the player.
                if (projectile.velocity.X < xDist)
                {
                    projectile.velocity.X += acceleration;
                    if (projectile.velocity.X < 0f && xDist > 0f)
                        projectile.velocity.X += acceleration;
                }
                else if (projectile.velocity.X > xDist)
                {
                    projectile.velocity.X -= acceleration;
                    if (projectile.velocity.X > 0f && xDist < 0f)
                        projectile.velocity.X -= acceleration;
                }
                if (projectile.velocity.Y < yDist)
                {
                    projectile.velocity.Y += acceleration;
                    if (projectile.velocity.Y < 0f && yDist > 0f)
                        projectile.velocity.Y += acceleration;
                }
                else if (projectile.velocity.Y > yDist)
                {
                    projectile.velocity.Y -= acceleration;
                    if (projectile.velocity.Y > 0f && yDist < 0f)
                        projectile.velocity.Y -= acceleration;
                }

                // Delete the projectile if it touches its owner.
                if (Main.myPlayer == projectile.owner)
                    if (projectile.Hitbox.Intersects(owner.Hitbox))
                        projectile.Kill();
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // After its last hit, starts returning instead of vanishing. Can pierce infinitely on the way back.
            if (projectile.penetrate == 1)
            {
                projectile.penetrate = -1;
                projectile.ai[0] = 1f;
            }
        }

        // Make it bounce on tiles.
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Impacts the terrain even though it bounces off.
            Main.PlaySound(SoundID.Dig, projectile.Center);
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);

            if (projectile.velocity.X != oldVelocity.X)
            {
                projectile.velocity.X = -oldVelocity.X;
            }
            if (projectile.velocity.Y != oldVelocity.Y)
            {
                projectile.velocity.Y = -oldVelocity.Y;
            }
            projectile.ai[0] = 1f;
            return false;
        }
    }
}
