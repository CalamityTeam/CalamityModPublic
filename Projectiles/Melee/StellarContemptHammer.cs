using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class StellarContemptHammer : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Melee/StellarContempt";

        private static float RotationIncrement = 0.22f;
        private static int Lifetime = 240;
        private static float ReboundTime = 26f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stellar Contempt");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 44;
            projectile.height = 44;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.extraUpdates = 3;
            projectile.timeLeft = Lifetime;
        }

        public override void AI()
        {
            drawOffsetX = -11;
            drawOriginOffsetY = -10;
            drawOriginOffsetX = 0;

            Lighting.AddLight(projectile.Center, 0.7f, 0.3f, 0.6f);

            // The hammer makes sound while flying.
            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = 8;
                Main.PlaySound(SoundID.Item7, (int)projectile.position.X, (int)projectile.position.Y);
            }

            // ai[0] stores whether the hammer is returning. If 0, it isn't. If 1, it is.
            if (projectile.ai[0] == 0f)
            {
                projectile.ai[1] += 1f;
                if (projectile.ai[1] >= ReboundTime)
                {
                    projectile.ai[0] = 1f;
                    projectile.ai[1] = 0f;
                    projectile.netUpdate = true;
                }
            }
            else
            {
                projectile.tileCollide = false;
                float returnSpeed = StellarContemptMelee.Speed;
                float acceleration = 3.2f;
                Player owner = Main.player[projectile.owner];

                // Delete the hammer if it's excessively far away.
                Vector2 playerCenter = owner.Center;
                float xDist = playerCenter.X - projectile.Center.X;
                float yDist = playerCenter.Y - projectile.Center.Y;
                float dist = (float)Math.Sqrt(xDist * xDist + yDist * yDist);
                if (dist > 3000f)
                    projectile.Kill();

                dist = returnSpeed / dist;
                xDist *= dist;
                yDist *= dist;

                // Home back in on the player.
                if (projectile.velocity.X < xDist)
                {
                    projectile.velocity.X = projectile.velocity.X + acceleration;
                    if (projectile.velocity.X < 0f && xDist > 0f)
                        projectile.velocity.X += acceleration;
                }
                else if (projectile.velocity.X > xDist)
                {
                    projectile.velocity.X = projectile.velocity.X - acceleration;
                    if (projectile.velocity.X > 0f && xDist < 0f)
                        projectile.velocity.X -= acceleration;
                }
                if (projectile.velocity.Y < yDist)
                {
                    projectile.velocity.Y = projectile.velocity.Y + acceleration;
                    if (projectile.velocity.Y < 0f && yDist > 0f)
                        projectile.velocity.Y += acceleration;
                }
                else if (projectile.velocity.Y > yDist)
                {
                    projectile.velocity.Y = projectile.velocity.Y - acceleration;
                    if (projectile.velocity.Y > 0f && yDist < 0f)
                        projectile.velocity.Y -= acceleration;
                }

                // Delete the projectile if it touches its owner.
                if (Main.myPlayer == projectile.owner)
                    if (projectile.Hitbox.Intersects(owner.Hitbox))
                        projectile.Kill();
            }

            // Rotate the hammer as it flies.
            projectile.rotation += RotationIncrement;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // Some dust gets produced on impact.
            int dustCount = Main.rand.Next(20, 24);
            int dustRadius = 6;
            Vector2 corner = new Vector2(target.Center.X - dustRadius, target.Center.Y - dustRadius);
            for (int i = 0; i < dustCount; ++i)
            {
                int dustType = 229;
                float scale = 0.8f + Main.rand.NextFloat(1.1f);
                int idx = Dust.NewDust(corner, 2 * dustRadius, 2 * dustRadius, dustType);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 3f;
                Main.dust[idx].scale = scale;
            }

            // Applies Nightwither on contact at night.
            if (!Main.dayTime)
                target.AddBuff(ModContent.BuffType<Nightwither>(), 240);

            SpawnFlares(target.Center, target.width, target.height);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            // Some dust gets produced on impact.
            int dustCount = Main.rand.Next(20, 24);
            int dustRadius = 6;
            Vector2 corner = new Vector2(target.Center.X - dustRadius, target.Center.Y - dustRadius);
            for (int i = 0; i < dustCount; ++i)
            {
                int dustType = 229;
                float scale = 0.8f + Main.rand.NextFloat(1.1f);
                int idx = Dust.NewDust(corner, 2 * dustRadius, 2 * dustRadius, dustType);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 3f;
                Main.dust[idx].scale = scale;
            }

            // Applies Nightwither on contact at night.
            if (!Main.dayTime)
                target.AddBuff(ModContent.BuffType<Nightwither>(), 240);

            SpawnFlares(target.Center, target.width, target.height);
        }

        private void SpawnFlares(Vector2 targetPos, int width, int height)
        {
            // Play the Lunar Flare sound centered on the user, not the target (consistent with Lunar Flare and Stellar Striker)
            Player user = Main.player[projectile.owner];
            Main.PlaySound(SoundID.Item88, projectile.position);
            projectile.netUpdate = true;

            int numFlares = 2;
            int flareDamage = (int)(0.3f * projectile.damage);
            float flareKB = 4f;
            for (int i = 0; i < numFlares; ++i)
            {
                float flareSpeed = Main.rand.NextFloat(8f, 11f);

                // Flares never come from straight up, there is always at least an 80 pixel horizontal offset
                float xDist = Main.rand.NextFloat(80f, 320f) * (Main.rand.NextBool() ? -1f : 1f);
                float yDist = Main.rand.NextFloat(1200f, 1440f);
                Vector2 startPoint = targetPos + new Vector2(xDist, -yDist);

                // The flare is somewhat inaccurate based on the size of the target.
                float xVariance = width / 4f;
                if (xVariance < 8f)
                    xVariance = 8f;
                float yVariance = height / 4f;
                if (yVariance < 8f)
                    yVariance = 8f;
                float xOffset = Main.rand.NextFloat(-xVariance, xVariance);
                float yOffset = Main.rand.NextFloat(-yVariance, yVariance);
                Vector2 offsetTarget = targetPos + new Vector2(xOffset, yOffset);

                // Finalize the velocity vector and make sure it's going at the right speed.
                Vector2 velocity = offsetTarget - startPoint;
                velocity.Normalize();
                velocity *= flareSpeed;

                float AI1 = Main.rand.Next(3);
                if (projectile.owner == Main.myPlayer)
                {
                    int proj = Projectile.NewProjectile(startPoint, velocity, ProjectileID.LunarFlare, flareDamage, flareKB, Main.myPlayer, 0f, AI1);
                    if (proj.WithinBounds(Main.maxProjectiles))
                    {
                        CalamityGlobalProjectile cgp = Main.projectile[proj].Calamity();
                        cgp.forceMelee = true;
                    }
                }
            }
        }
    }
}
