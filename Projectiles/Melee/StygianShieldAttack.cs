using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
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
        public const float DashDuration = 21f;
        public const float IFrameRatio = 0.3f; // Amount given = Ratio * Charge, rounded down

        public Player Owner => Main.player[Projectile.owner];
        public ref float Charge => ref Projectile.ai[0];
        public ref float DashTime => ref Projectile.ai[1];
        public Vector2 DashDestination = Vector2.Zero;

        internal PrimitiveTrail TrailDrawer;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

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

            // TODO -- Windup sound and rushing sound (definitely requires a custom looping sound)
            
            // Dashing behavior
            Projectile.Opacity = Utils.GetLerpValue(0f, DashDuration * 0.7f, DashTime, true);
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

                    // Reset the trail
                    for (int i = 0; i < Projectile.oldPos.Length; i++)
                    {
                        Projectile.oldPos[i] = Vector2.Zero;
                        Projectile.oldRot[i] = 0f;
                        Projectile.oldSpriteDirection[i] = 0;
                    }

                    // Spawn flat gradients signalling the start of the dash
                    for (int i = 0; i < 3; i++)
                    {
                        float scale = 1 / (float)Math.Pow(1.6D, i);
                        float rot = Owner.miscCounter / MathHelper.TwoPi + MathHelper.ToRadians(120f * i);
                        Particle glow = new FlatGlow(Projectile.Center, Vector2.Zero, Color.DarkOrange * 0.3f, rot, Vector2.One * scale, Vector2.One * 8f * scale, 12);
                        GeneralParticleHandler.SpawnParticle(glow);
                    }

                    DashDestination = intendedDestination;
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

        internal Color ColorFunction(float completionRatio)
        {
            float fadeOpacity = Utils.GetLerpValue(1f, 0.2f, completionRatio, true) * Projectile.Opacity;
            return Color.Lerp(Color.DarkOrange, Color.Red, completionRatio) * fadeOpacity;
        }

        internal float WidthFunction(float completionRatio) => 12f;

        public override bool PreDraw(ref Color lightColor)
        {
            // Textures and general use stuff
            Texture2D bloomTex = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;
            Texture2D flatTex = ModContent.Request<Texture2D>("CalamityMod/Particles/FlatShape").Value;
            Texture2D shieldTex = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/StygianShieldBloom").Value;
            Texture2D ringTex = ModContent.Request<Texture2D>("CalamityMod/Particles/HollowCircleHardEdge").Value;
            float chargeLevel = Charge / MaxChargeTime;

            // Bull Rush dash effects
            if (DashTime > 0 && DashTime < DashDuration - 1f && DashDestination != Vector2.Zero && Projectile.velocity.Length() > 0f)
            {
                // General uses
                float durationRatio = DashTime / DashDuration;
                float scaleMult = MathHelper.Lerp(1.8f, 1f, durationRatio);
                Vector2 direction = Projectile.SafeDirectionTo(DashDestination);
                Vector2 extraOffset = (direction * 800f / Projectile.velocity.Length()) - Main.screenPosition;
                // Arrows and stuff
                float arrowFace = Projectile.velocity.ToRotation() - MathHelper.Pi;
                float side = MathHelper.ToRadians(135f);
                Color headColor = Color.Lerp(Color.Orange, Color.OrangeRed, durationRatio);
                Color shieldColor = Color.LightSalmon;

                // Main trail
                if (TrailDrawer is null)
                TrailDrawer = new PrimitiveTrail(WidthFunction, ColorFunction, specialShader: GameShaders.Misc["CalamityMod:TrailStreak"]);

                GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/ScarletDevilStreak"));
                TrailDrawer.Draw(Projectile.oldPos, Projectile.Size * 0.5f + extraOffset - (direction * 80f), 10);

                // "Arrow heads" making up the shield tip
                Effect ArrowEffect = Filters.Scene["CalamityMod:SpreadTelegraph"].GetShader().Shader;
                ArrowEffect.Parameters["centerOpacity"].SetValue(1f);
                ArrowEffect.Parameters["mainOpacity"].SetValue(1f);
                ArrowEffect.Parameters["halfSpreadAngle"].SetValue(MathHelper.ToRadians(7.5f));
                ArrowEffect.Parameters["edgeColor"].SetValue(headColor.ToVector3());
                ArrowEffect.Parameters["centerColor"].SetValue(headColor.ToVector3());
                ArrowEffect.Parameters["edgeBlendLength"].SetValue(0.07f);
                ArrowEffect.Parameters["edgeBlendStrength"].SetValue(8f);

                Main.spriteBatch.EnterShaderRegion(BlendState.Additive, ArrowEffect);
                Texture2D headTex = ModContent.Request<Texture2D>(Texture).Value;
                // One pokes forward and two to the sides
                for (float i = -side; i <= side; i += side)
                    Main.EntitySpriteDraw(headTex, Projectile.Center + extraOffset + (direction * 72f * scaleMult).RotatedBy(i), null, Color.White, arrowFace + i, headTex.Size() / 2f, 300f * scaleMult, SpriteEffects.None);
                Main.spriteBatch.ExitShaderRegion();

                // Blooming shield and rings
                Main.spriteBatch.EnterShaderRegion(BlendState.Additive);
                
                float shieldRot = arrowFace - MathHelper.Pi;
                Main.EntitySpriteDraw(shieldTex, Projectile.Center + extraOffset, null, shieldColor, shieldRot, shieldTex.Size() / 2f, 1.25f * scaleMult, SpriteEffects.None);
                Main.EntitySpriteDraw(bloomTex, Projectile.Center + extraOffset, null, shieldColor * 0.75f, 0f, bloomTex.Size() / 2f, 0.5f * scaleMult, SpriteEffects.None);

                Vector2 ringScale = new Vector2(0.033f, 2.25f * scaleMult * shieldTex.Height / (float)ringTex.Height);
                // Inner to outer in order: largest -> smallest -> middlest
                Main.EntitySpriteDraw(ringTex, Projectile.Center + extraOffset - (direction * 36f * scaleMult), null, shieldColor, shieldRot, ringTex.Size() / 2f, ringScale * 1.2f, SpriteEffects.None);
                Main.EntitySpriteDraw(ringTex, Projectile.Center + extraOffset - (direction * 30f * scaleMult), null, shieldColor, shieldRot, ringTex.Size() / 2f, ringScale * 0.8f, SpriteEffects.None);
                Main.EntitySpriteDraw(ringTex, Projectile.Center + extraOffset - (direction * 24f * scaleMult), null, shieldColor, shieldRot, ringTex.Size() / 2f, ringScale, SpriteEffects.None);
                Main.spriteBatch.ExitShaderRegion();
            }
            // Bull Rush telegraph
            else if (DashTime <= 0f)
            {
                // Move according to the player direction & frame
                Vector2 shieldPos = (Vector2.UnitX * Owner.direction * Owner.width * 0.4f) + Owner.Center - Main.screenPosition;
                if (Owner.bodyFrame.Y == 280)
                    shieldPos -= Vector2.UnitY * Owner.height * 0.35f;

                Main.spriteBatch.EnterShaderRegion(BlendState.Additive);

                // Glow at the shield when charge is full
                if (Charge >= MaxChargeTime)
                {
                    float glowScale = 0.5f * CalamityUtils.Convert01To010((Owner.miscCounter % 40) / 40f);
                    Main.EntitySpriteDraw(flatTex, shieldPos, null, Color.DarkOrange * 0.2f, 0f, flatTex.Size() / 2f, 0.3f + glowScale, SpriteEffects.None);
                }
                // Normal bloom which scales with charge
                Main.EntitySpriteDraw(bloomTex, shieldPos, null, Color.DarkGoldenrod * 0.3f, 0f, bloomTex.Size() / 2f, 0.5f * chargeLevel, SpriteEffects.None);

                Main.spriteBatch.ExitShaderRegion();
            }
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (DashTime <= 0f)
                return;

            // More flat gradients signalling the end of the dash
            for (int i = 0; i < 3; i++)
            {
                float scale = 1f / (float)Math.Pow(1.6D, i);
                float rot = Owner.miscCounter / MathHelper.TwoPi + MathHelper.ToRadians(120f * i);
                Particle glow = new FlatGlow(Projectile.Center, Vector2.Zero, Color.DarkGoldenrod * 0.3f, rot, Vector2.One * scale, Vector2.One * 6f * scale, 9);
                GeneralParticleHandler.SpawnParticle(glow);
            }
        }
    }
}
