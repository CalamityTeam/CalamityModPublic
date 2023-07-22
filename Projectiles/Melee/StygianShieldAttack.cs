using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class StygianShieldAttack : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        // Bull Rush stats
        public const float MinChargeTime = 15f;
        public const float MaxChargeTime = 60f;
        public const float MaxChargeDistance = 960f; // 60 blocks
        public const float MaxChargeDamageMult = 4f;
        public const float PiercingDamageMult = 0.6f;
        public const float DashDuration = 18f;
        public const float IFrameRatio = 0.3f; // Amount given = Ratio * Charge, rounded down

        public Player Owner => Main.player[Projectile.owner];
        public ref float Charge => ref Projectile.ai[0];
        public ref float DashTime => ref Projectile.ai[1];
        public Vector2 DashDestination = Vector2.Zero;
        public Vector2 DashOrigin = Vector2.Zero;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void AI()
        {
            if (Owner is null || Owner.dead || Owner.ActiveItem().type != ModContent.ItemType<StygianShield>())
                Projectile.Kill();
            
            Owner.heldProj = Projectile.whoAmI;
            
            // Dashing behavior
            if (DashDestination != Vector2.Zero)
            {
                // Detach the owner from any mounts/hooks
                if (Owner.mount != null)
                    Owner.mount.Dismount(Owner);
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (!proj.active || proj.owner != Owner.whoAmI || proj.aiStyle != ProjAIStyleID.Hook)
                        continue;
                    if (proj.aiStyle == ProjAIStyleID.Hook)
                        proj.Kill();
                }

                // Set the movement of the player
                DashTime++;
                Projectile.velocity = Projectile.SafeDirectionTo(DashDestination) * MaxChargeDistance * Charge / MaxChargeTime / DashDuration;

                // Kill the projectile once the dash is over, or it clearly detects a solid tile
                if (Vector2.Distance(Projectile.Center, DashDestination) < 4f || DashTime > DashDuration || Collision.SolidCollision(Projectile.Center, 1, 1, false))
                    Projectile.Kill();

                // Actually set the owner center if there's no issues
                Owner.Center = Projectile.Center;
                Owner.direction = Projectile.direction;
                return;
            }

            // Set the center of the projectile
            Projectile.Center = Owner.MountedCenter;
            // Charge up particles
            if (Charge < MaxChargeTime)
            {
                Charge++;
                float streakThickness = Main.rand.NextFloat(0.4f, 0.7f) * Charge / MaxChargeTime;
                Vector2 spawnPoint = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(80f, 120f);
                Particle streak = new ManaDrainStreak(Owner, streakThickness, spawnPoint, Main.rand.NextFloat(30f, 44f), Color.Firebrick, Color.OrangeRed, Main.rand.Next(12, 21));
                GeneralParticleHandler.SpawnParticle(streak);
            }

            // Attack upon releasing the button
            if (!Owner.channel || Owner.noItems || Owner.CCed)
                Attack();
        }

        public void Attack()
        {
            // Perform the dash if able to
            if (Charge >= MinChargeTime && !Owner.noItems && !Owner.CCed)
            {
                // Set the destination in which the dash is supposed to be
                Vector2 intendedDestination = Projectile.Center + Projectile.SafeDirectionTo(Owner.Calamity().mouseWorld) * MaxChargeDistance * Charge / MaxChargeTime;

                // Has to be within world bounds
                if (intendedDestination.X >= 800f && intendedDestination.Y >= 800f && intendedDestination.X <= Main.maxTilesX * 16f - 800f && intendedDestination.Y <= Main.maxTilesY * 16f - 800f)
                {
                    // Give immunity frames
                    Owner.immune = true;
                    Owner.immuneNoBlink = true;
                    Owner.immuneTime = (int)(IFrameRatio * Charge);
                    for (int k = 0; k < Owner.hurtCooldowns.Length; k++)
                        Owner.hurtCooldowns[k] = Owner.immuneTime;

                    DashDestination = intendedDestination;
                    DashOrigin = Projectile.Center;
                    Projectile.damage = (int)(Projectile.damage * MaxChargeDamageMult * Charge / MaxChargeTime);
                    Projectile.ExpandHitboxBy(100);
                    return;
                }            
            }
            // Otherwise, kill the projectile
            Projectile.Kill();
        }

        // Preventing unintended collisions with the floor
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = height = 32;
            return true;
        }

        // Bull Rush damage hitboxes (slightly larger than the player)
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Owner.Center, 48f, targetHitbox);

        // Can only deal damage while dashing
        public override bool? CanDamage() => DashTime > 0 ? base.CanDamage() : false;

        // Hits have diminishing damage
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => Projectile.damage = (int)(Projectile.damage * PiercingDamageMult);

        /*public override bool PreDraw(ref Color lightColor)
        {
            // Bull Rush telegraph here

            // Bull Rush dash effect too
            return false;
        }*/
    }
}
