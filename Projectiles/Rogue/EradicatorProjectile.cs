using CalamityMod.Buffs.DamageOverTime;
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
    public class EradicatorProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Eradicator";
        private const float RotationIncrement = 0.09f;
        private const int Lifetime = 350;
        public const int StealthExtraLifetime = 240; // 1 extra update means this is double what you'd expect for 2 seconds
        private const float ReboundTime = 40f;

        private float randomLaserCharge = 0f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 58;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = 2;
            Projectile.timeLeft = Lifetime;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 18;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            //
            // Boomerang AI copied from Nanoblack Reaper
            //

            // On the frame the disc begins returning, send a net update.
            if (Projectile.timeLeft == Lifetime - ReboundTime)
                Projectile.netUpdate = true;

            // The disc runs its returning AI if it has existed longer than ReboundTime frames.
            if (Projectile.timeLeft <= Lifetime - ReboundTime)
            {
                float returnSpeed = Eradicator.Speed * 1.3f;
                float acceleration = 0.25f;
                Player owner = Main.player[Projectile.owner];

                // Delete the disc if it's excessively far away.
                Vector2 playerCenter = owner.Center;
                float xDist = playerCenter.X - Projectile.Center.X;
                float yDist = playerCenter.Y - Projectile.Center.Y;
                float dist = (float)Math.Sqrt(xDist * xDist + yDist * yDist);
                if (dist > 3000f)
                    Projectile.Kill();

                dist = returnSpeed / dist;
                xDist *= dist;
                yDist *= dist;

                // Home back in on the player.
                if (Projectile.velocity.X < xDist)
                {
                    Projectile.velocity.X = Projectile.velocity.X + acceleration;
                    if (Projectile.velocity.X < 0f && xDist > 0f)
                        Projectile.velocity.X += acceleration;
                }
                else if (Projectile.velocity.X > xDist)
                {
                    Projectile.velocity.X = Projectile.velocity.X - acceleration;
                    if (Projectile.velocity.X > 0f && xDist < 0f)
                        Projectile.velocity.X -= acceleration;
                }
                if (Projectile.velocity.Y < yDist)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + acceleration;
                    if (Projectile.velocity.Y < 0f && yDist > 0f)
                        Projectile.velocity.Y += acceleration;
                }
                else if (Projectile.velocity.Y > yDist)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - acceleration;
                    if (Projectile.velocity.Y > 0f && yDist < 0f)
                        Projectile.velocity.Y -= acceleration;
                }

                // Delete the projectile if it touches its owner.
                if (Main.myPlayer == Projectile.owner)
                    if (Projectile.Hitbox.Intersects(owner.Hitbox))
                        Projectile.Kill();
            }

            // Lighting.
            Lighting.AddLight(Projectile.Center, 0.35f, 0f, 0.25f);

            // Rotate the disc as it flies.
            float spin = Projectile.direction <= 0 ? -1f : 1f;
            Projectile.rotation += spin * RotationIncrement;

            // If attached to something (this only occurs for stealth strikes), do the buzzsaw grind and spam lasers everywhere.
            if (Projectile.ai[0] == 1f)
                StealthStrikeGrind(spin);
            else
            {
                // Fire lasers at up to 2 nearby targets every 8 frames for 40% damage.
                // Stealth strike lasers have an intentionally lower ratio of 12%.
                double laserDamageRatio = Projectile.Calamity().stealthStrike ? 0.12D : 0.4D;
                float laserFrames = Projectile.MaxUpdates * 8f;
                CalamityUtils.MagnetSphereHitscan(Projectile, 300f, 6f, laserFrames, 2, ModContent.ProjectileType<NebulaShot>(), laserDamageRatio, true);
            }
        }

        private void StealthStrikeGrind(float spinDir)
        {
            // Spin extra fast to visually shred the enemy.
            Projectile.rotation += spinDir * RotationIncrement * 0.8f;

            // Randomly fire lasers while grinding. Each laser only does 12% damage.
            randomLaserCharge += Main.rand.NextFloat(0.09f, 0.14f);
            if (randomLaserCharge >= 1f)
            {
                randomLaserCharge -= 1f;
                Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);

                int laserDamage = (int)(Projectile.damage * 0.12D);
                Projectile laser = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<NebulaShot>(), laserDamage, 0f, Projectile.owner);
                if (laser.whoAmI.WithinBounds(Main.maxProjectiles))
                {
                    laser.DamageType = RogueDamageClass.Instance;
                    laser.aiStyle = Main.rand.NextBool() ? ProjAIStyleID.Arrow : -1;
                    laser.penetrate = -1;
                    laser.usesLocalNPCImmunity = true;

                    // This projectile has a hefty amount of extra updates, which will influence the hit cooldown.
                    laser.localNPCHitCooldown = 120;
                }
            }

            // Stay stuck to the target.
            Projectile.StickyProjAI(6, true);

            // If still attached to a target, do nothing.
            if (Projectile.ai[0] != 0f)
                return;

            // If the target died, look for a new one.
            const float turnSpeed = 30f;
            const float speedMult = 5f;
            const float homingRange = 600f;
            NPC uDie = Projectile.Center.ClosestNPCAt(homingRange, true, true);
            if (uDie != null)
            {
                Vector2 distNorm = (uDie.Center - Projectile.Center).SafeNormalize(Vector2.UnitX);
                Projectile.velocity = (Projectile.velocity * (turnSpeed - 1f) + distNorm * speedMult) / turnSpeed;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 180);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 180);

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => OnHit();

        private void OnHit()
        {
            // Non-stealth strikes do nothing special on hit.
            if (!Projectile.Calamity().stealthStrike)
                return;

            // On the first frame of impact, slow down massively so it'll effectively stay stuck to an enemy.
            if (Projectile.ai[0] == 0f && Projectile.ai[1] == 0f)
            {
                Projectile.velocity *= 0.1f;

                // Provide a fixed amount of grind time so that DPS can't vary wildly.
                Projectile.timeLeft = 90;
            }

            // Apply sticky AI.
            Projectile.ModifyHitNPCSticky(3);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Vector2 origin = new Vector2(31f, 29f);
            Main.EntitySpriteDraw(ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Rogue/EradicatorGlow").Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin, 1f, SpriteEffects.None, 0);
        }
    }
}
