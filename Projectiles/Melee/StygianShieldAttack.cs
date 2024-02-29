using CalamityMod.Graphics.Primitives;
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
        public const float MaxChargeDistance = 800f; // 50 blocks
        public const float MaxChargeDamageMult = 4f;
        public const float PiercingDamageMult = 0.6f;
        public const float DashDuration = 21f;
        public const float IFrameRatio = 0.35f; // Amount given = Ratio * Charge, rounded down

        public Player Owner => Main.player[Projectile.owner];
        public ref float Charge => ref Projectile.ai[0];
        public ref float DashTime => ref Projectile.ai[1];
        public Vector2 DashDestination = Vector2.Zero;

        // Rawest placeholder sound
        public static readonly SoundStyle DashSound = new("CalamityMod/Sounds/Custom/ExoMechs/ApolloMissileLaunch") { Volume = 0.6f };

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
            Projectile.tileCollide = false;
            Projectile.DamageType = TrueMeleeNoSpeedDamageClass.Instance;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void AI()
        {
            if (Owner is null || Owner.dead || Owner.ActiveItem().type != ModContent.ItemType<StygianShield>())
                Projectile.Kill();
            
            Owner.heldProj = Projectile.whoAmI;

            // TODO -- Windup sound (definitely requires a custom looping sound)
            
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
                Owner.ChangeDir(Projectile.direction);
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
                if (intendedDestination.X >= 660f && intendedDestination.Y >= 660f && intendedDestination.X <= Main.maxTilesX * 16f - 680f && intendedDestination.Y <= Main.maxTilesY * 16f - 680f)
                {
                    SoundEngine.PlaySound(DashSound, Owner.Center);

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
                    Projectile.tileCollide = true;
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
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * PiercingDamageMult);

            // Spawn a violent slash through the target that is hit
            float rotation = Projectile.velocity.ToRotation() - MathHelper.Pi;
            Particle redSlash = new SlashThrough(Color.Red * 0.9f, Owner.Center, rotation, 15, target);
            GeneralParticleHandler.SpawnParticle(redSlash);
        }

        internal Color ColorFunction(float completionRatio)
        {
            float fadeOpacity = Utils.GetLerpValue(1f, 0.2f, completionRatio, true) * Projectile.Opacity;
            return Color.Lerp(Color.DarkOrange, Color.Red, completionRatio) * fadeOpacity;
        }

        internal float WidthFunction(float completionRatio) => 12f;

        public override bool PreDraw(ref Color lightColor)
        {
            // Textures and general use stuff
            Texture2D mainTex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;
            Texture2D flatTex = ModContent.Request<Texture2D>("CalamityMod/Particles/FlatShape").Value;
            Texture2D shieldTex = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/StygianShieldBloom").Value;
            Texture2D ringTex = ModContent.Request<Texture2D>("CalamityMod/Particles/HollowCircleHardEdge").Value;
            float chargeLevel = Charge / MaxChargeTime;

            // "Arrow head" effect which draws around the shield (and also the telegraph which have different draw parameters below)
            Effect ArrowEffect = Filters.Scene["CalamityMod:SpreadTelegraph"].GetShader().Shader;
            ArrowEffect.Parameters["centerOpacity"].SetValue(1f);
            ArrowEffect.Parameters["mainOpacity"].SetValue(1f);
            ArrowEffect.Parameters["edgeBlendLength"].SetValue(0.07f);
            ArrowEffect.Parameters["edgeBlendStrength"].SetValue(8f);

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
                GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/ScarletDevilStreak"));
                PrimitiveSet.Prepare(Projectile.oldPos, new(WidthFunction, ColorFunction, (_) => Projectile.Size * 0.5f + extraOffset + Main.screenPosition - direction * 80f,
                    shader: GameShaders.Misc["CalamityMod:TrailStreak"]), 10);

                ArrowEffect.Parameters["halfSpreadAngle"].SetValue(MathHelper.ToRadians(7.5f));
                ArrowEffect.Parameters["edgeColor"].SetValue(headColor.ToVector3());
                ArrowEffect.Parameters["centerColor"].SetValue(headColor.ToVector3());
                Main.spriteBatch.EnterShaderRegion(BlendState.Additive, ArrowEffect);
                // One pokes forward and two to the sides
                for (float i = -side; i <= side; i += side)
                    Main.EntitySpriteDraw(mainTex, Projectile.Center + extraOffset + (direction * 72f * scaleMult).RotatedBy(i), null, Color.White, arrowFace + i, mainTex.Size() / 2f, 300f * scaleMult, SpriteEffects.None);
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
                    Main.EntitySpriteDraw(flatTex, shieldPos, null, Color.DarkRed * 0.3f, 0f, flatTex.Size() / 2f, 0.3f + glowScale, SpriteEffects.None);
                }
                // Normal bloom which scales with charge
                Main.EntitySpriteDraw(bloomTex, shieldPos, null, Color.DarkGoldenrod * 0.4f * chargeLevel, 0f, bloomTex.Size() / 2f, 0.4f * chargeLevel, SpriteEffects.None);

                Main.spriteBatch.ExitShaderRegion();

                // Charge sight line
                if (Charge >= MinChargeTime)
                {
                    // Where the dash is supposed to take you, taken from above
                    Vector2 dashLength = Projectile.SafeDirectionTo(Owner.Calamity().mouseWorld) * MaxChargeDistance * Charge / MaxChargeTime;
                    Vector2 intendedDestination = Projectile.Center + dashLength;
                    float direction = Projectile.SafeDirectionTo(intendedDestination).ToRotation();

                    // Arrow is red if out of bounds (indicating you can't use it)
                    bool OOB = intendedDestination.X < 660f || intendedDestination.Y < 660f || intendedDestination.X > Main.maxTilesX * 16f - 680f || intendedDestination.Y > Main.maxTilesY * 16f - 680f;
                    Color telegraphColor = OOB ? Color.Red : Color.White;
                    Effect TelegraphEffect = ArrowEffect;
                    TelegraphEffect.Parameters["centerOpacity"].SetValue(0.6f);
                    TelegraphEffect.Parameters["halfSpreadAngle"].SetValue(MathHelper.ToRadians(64f));
                    TelegraphEffect.Parameters["edgeColor"].SetValue(telegraphColor.ToVector3());
                    TelegraphEffect.Parameters["centerColor"].SetValue(telegraphColor.ToVector3());

                    Main.spriteBatch.EnterShaderRegion(BlendState.Additive, TelegraphEffect);
                    Main.EntitySpriteDraw(mainTex, intendedDestination - Main.screenPosition, null, Color.White, direction - MathHelper.Pi, mainTex.Size() / 2f, 135f, SpriteEffects.None);
                    Main.spriteBatch.ExitShaderRegion();

                    for (int i = -1; i <= 1; i += 2)
                    {
                        Vector2 pointStart = Projectile.Center + Projectile.SafeDirectionTo(Owner.Calamity().mouseWorld).RotatedBy(90f * i) * 60f;
                        Vector2 pointEnd = Projectile.Center + dashLength * 0.1f;
                        Vector2 lineStart = pointStart + dashLength * 0.1f;
                        Vector2 lineEnd = pointStart + dashLength;
                        Color lineColor = telegraphColor * 0.3f;
                        float lineScale = 2f;
                        Main.spriteBatch.DrawLineBetter(lineStart, pointEnd, lineColor, lineScale);
                        Main.spriteBatch.DrawLineBetter(lineStart, lineEnd, lineColor, lineScale * 2f);
                        Main.spriteBatch.DrawLineBetter(lineEnd, intendedDestination, lineColor, lineScale);
                    }
                }
            }
            return false;
        }

        public override void OnKill(int timeLeft)
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
