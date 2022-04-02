using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class EradicatorProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Eradicator";
        private const float RotationIncrement = 0.09f;
        private const int Lifetime = 350;
        public const int StealthExtraLifetime = 240; // 1 extra update means this is double what you'd expect for 2 seconds
        private const float ReboundTime = 40f;

        private float randomLaserCharge = 0f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eradicator");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 58;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.MaxUpdates = 2;
            projectile.timeLeft = Lifetime;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 18;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            //
            // Boomerang AI copied from Nanoblack Reaper
            //

            // On the frame the disc begins returning, send a net update.
            if (projectile.timeLeft == Lifetime - ReboundTime)
                projectile.netUpdate = true;

            // The disc runs its returning AI if it has existed longer than ReboundTime frames.
            if (projectile.timeLeft <= Lifetime - ReboundTime)
            {
                float returnSpeed = Eradicator.Speed * 1.3f;
                float acceleration = 0.25f;
                Player owner = Main.player[projectile.owner];

                // Delete the disc if it's excessively far away.
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

            // Lighting.
            Lighting.AddLight(projectile.Center, 0.35f, 0f, 0.25f);

            // Rotate the disc as it flies.
            float spin = projectile.direction <= 0 ? -1f : 1f;
            projectile.rotation += spin * RotationIncrement;

            // If attached to something (this only occurs for stealth strikes), do the buzzsaw grind and spam lasers everywhere.
            if (projectile.ai[0] == 1f)
                StealthStrikeGrind(spin);
            else
            {
                // Fire lasers at up to 2 nearby targets every 8 frames for 40% damage.
                // Stealth strike lasers have an intentionally lower ratio of 12%.
                double laserDamageRatio = projectile.Calamity().stealthStrike ? 0.12D : 0.4D;
                float laserFrames = projectile.MaxUpdates * 8f;
                CalamityGlobalProjectile.MagnetSphereHitscan(projectile, 300f, 6f, laserFrames, 2, ModContent.ProjectileType<NebulaShot>(), laserDamageRatio, true);
            }
        }

        private void StealthStrikeGrind(float spinDir)
        {
            // Spin extra fast to visually shred the enemy.
            projectile.rotation += spinDir * RotationIncrement * 0.8f;

            // Randomly fire lasers while grinding. Each laser only does 12% damage.
            randomLaserCharge += Main.rand.NextFloat(0.09f, 0.14f);
            if (randomLaserCharge >= 1f)
            {
                randomLaserCharge -= 1f;
                Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);

                int laserDamage = (int)(projectile.damage * 0.12D);
                Projectile laser = Projectile.NewProjectileDirect(projectile.Center, velocity, ModContent.ProjectileType<NebulaShot>(), laserDamage, 0f, projectile.owner);
                if (laser.whoAmI.WithinBounds(Main.maxProjectiles))
                {
                    laser.Calamity().forceRogue = true;
                    laser.aiStyle = Main.rand.NextBool() ? 1 : -1;
                    laser.penetrate = -1;
                    laser.usesLocalNPCImmunity = true;

                    // This projectile has a hefty amount of extra updates, which will influence the hit cooldown.
                    laser.localNPCHitCooldown = 120;
                }
            }

            // Stay stuck to the target.
            projectile.StickyProjAI(6, true);

            // If still attached to a target, do nothing.
            if (projectile.ai[0] != 0f)
                return;

            // If the target died, look for a new one.
            const float turnSpeed = 30f;
            const float speedMult = 5f;
            const float homingRange = 600f;
            NPC uDie = projectile.Center.ClosestNPCAt(homingRange, true, true);
            if (uDie != null)
            {
                Vector2 distNorm = (uDie.Center - projectile.Center).SafeNormalize(Vector2.UnitX);
                projectile.velocity = (projectile.velocity * (turnSpeed - 1f) + distNorm * speedMult) / turnSpeed;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 180);
            target.AddBuff(BuffID.Frostburn, 90);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 180);
            target.AddBuff(BuffID.Frostburn, 90);
        }

        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit) => OnHit();
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) => OnHit();

        private void OnHit()
        {
            // Non-stealth strikes do nothing special on hit.
            if (!projectile.Calamity().stealthStrike)
                return;

            // On the first frame of impact, slow down massively so it'll effectively stay stuck to an enemy.
            if (projectile.ai[0] == 0f && projectile.ai[1] == 0f)
            {
                projectile.velocity *= 0.1f;

                // Provide a fixed amount of grind time so that DPS can't vary wildly.
                projectile.timeLeft = 90;
            }

            // Apply sticky AI.
            projectile.ModifyHitNPCSticky(3, true);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor);
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 origin = new Vector2(31f, 29f);
            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Items/Weapons/Rogue/EradicatorGlow"), projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation, origin, 1f, SpriteEffects.None, 0f);
        }
    }
}
