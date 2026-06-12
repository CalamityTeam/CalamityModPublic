using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.DataStructures;
using CalamityMod.Dusts;
using CalamityMod.Effects;
using CalamityMod.Enums;
using CalamityMod.Graphics.Metaballs;
using CalamityMod.Graphics.Primitives;
using CalamityMod.NPCs;
using CalamityMod.Packets.Entities;
using CalamityMod.Particles;
using CalamityMod.Utilities.Daybreak;
using CalamityMod.Utilities.Daybreak.Buffers;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace CalamityMod.Projectiles.Ranged
{
    [PierceResistException]
    public class AbyssalFire : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";

        public Vector2[] ControlPoints;

        public static int MaxLaserControlPoints => 32;

        public static int MaxLaserLength => 3330;

        public static int MaxLaserWidth => 90;

        public Player Owner => Main.player[Projectile.owner];

        public Projectile VoidragonHoldout => Main.projectile[(int)Projectile.ai[0]];

        public ref float LaserLength => ref Projectile.ai[1];

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";


        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = MaxLaserWidth;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 2;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            LaserLength = 0.05f;
        }

        public override void AI()
        {
            // If the owner is no longer able to cast the beam, kill it.
            if (Owner.CantUseHoldout() || VoidragonHoldout is null)
            {
                Projectile.Kill();
                return;
            }

            // Decide where to position the laserbeam.
            Vector2 circlePointDirection = Projectile.velocity.SafeNormalize(Vector2.UnitX * Owner.direction);
            Projectile.Center = VoidragonHoldout.ModProjectile<VoidragonHoldout>().GunTipPosition - ((Projectile.velocity * 12f).RotatedBy(-0.02f * Projectile.direction));

            int beamTimer = VoidragonHoldout.ModProjectile<VoidragonHoldout>().beamTimer;
            float beamWidthInterpolant = Utils.GetLerpValue(0, 25, beamTimer, true);

            // Set the control points for the primitive drawing.
            ControlPoints ??= new Vector2[MaxLaserControlPoints];
            for (int i = 0; i < MaxLaserControlPoints; i++)
                ControlPoints[i] = Projectile.Center + Projectile.velocity * i / (ControlPoints.Length - 1f) * MaxLaserLength;

            // Grow and shrink depending on how long left the laser has to remain active.
            Projectile.scale = MathHelper.Lerp(0f, 1f, CalamityUtils.ExpOutEasing(beamWidthInterpolant, 2));
            LaserLength = MathHelper.Lerp(LaserLength, 1f, 0.032f);
            
            // Update aim.
            UpdateAim();

            // Spawn a bunch of particles along the length of the laser.
            if (Projectile.scale >= 0.25f)
            {
                BezierCurve curve = new(ControlPoints);
                float tipRatio = 0.042f;
                for (int i = 0; i < 10; i++)
                {
                    Vector2 tipSpawnPosition = curve.Evaluate(Main.rand.NextFloat(0f, tipRatio));
                    Vector2 bodySpawnPosition = curve.Evaluate(Main.rand.NextFloat(tipRatio, 1f));
                    Vector2 fireVelocity = Owner.SafeDirectionTo(Main.MouseWorld).RotatedByRandom(MathHelper.ToRadians(20f)) * Main.rand.NextFloat(25f, 30f);

                    Color fireColorBackground = Color.Lerp(Color.DarkViolet, Color.Black, 0.25f);

                    int fireLifetime = Main.rand.Next(45, 60);
                    float fireScale = Main.rand.NextFloat(1.75f, 2.25f) * Projectile.scale;
                    float fireOpacity = Main.rand.NextFloat(0.65f, 0.95f);

                    HeavySmokeParticle abyssalFlamesBodyBackground = new(bodySpawnPosition, fireVelocity, fireColorBackground, fireLifetime, fireScale, 1f, Main.rand.NextFloat(0.02f, 0.1f) * Main.rand.NextBool().ToDirectionInt(), false);
                    GeneralParticleHandler.SpawnParticle(abyssalFlamesBodyBackground, true, GeneralDrawLayer.BeforeProjectiles);

                    HeavySmokeParticle abyssalFlamesBodyForeground = new(bodySpawnPosition, fireVelocity, Main.rand.NextBool(4) ? Color.White : Color.Purple, fireLifetime, fireScale * 0.6f, 0.8f, Main.rand.NextFloat(0.02f, 0.1f) * Main.rand.NextBool().ToDirectionInt(), true);
                    GeneralParticleHandler.SpawnParticle(abyssalFlamesBodyForeground, true, GeneralDrawLayer.AfterProjectiles);

                    HeavySmokeParticle abyssFlamesTip = new(tipSpawnPosition, fireVelocity, Color.White, fireLifetime, fireScale * 0.52f, 0.3f, Main.rand.NextFloat(0.02f, 0.1f) * Main.rand.NextBool().ToDirectionInt(), true);
                    GeneralParticleHandler.SpawnParticle(abyssFlamesTip, true, GeneralDrawLayer.AfterProjectiles);
                }

                for (int i = 0; i < 3; i++)
                {
                    Vector2 fireVelocity = Owner.SafeDirectionTo(Main.MouseWorld).RotatedByRandom(MathHelper.ToRadians(20f)) * Main.rand.NextFloat(15f, 20f);
                    HeavySmokeParticle whiteFlames = new(Projectile.Center - Projectile.velocity * 8f, fireVelocity, Color.White, Main.rand.Next(15, 20), 0.64f, 0.7f, Main.rand.NextFloat(0.02f, 0.1f) * Main.rand.NextBool().ToDirectionInt(), true);
                    GeneralParticleHandler.SpawnParticle(whiteFlames, true, GeneralDrawLayer.AfterDusts);
                }
            }

            // Shake the screen slightly.
            if (Owner.Calamity().GeneralScreenShakePower < 1.15f)
                Owner.Calamity().GeneralScreenShakePower = MathHelper.Lerp(0f, 1.15f, Projectile.scale / 2f);

            // Make the beam cast light along its length. The brightness of the light is reliant on the scale of the beam.
            DelegateMethods.v3_1 = Color.DarkViolet.ToVector3() * Projectile.scale * 0.4f;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * MaxLaserLength, Projectile.width * Projectile.scale, DelegateMethods.CastLight);
        }

        public void UpdateAim()
        {
            // Only execute the aiming code for the owner.
            if (Main.myPlayer != Projectile.owner)
                return;

            Vector2 newAimDirection = VoidragonHoldout.velocity.SafeNormalize(Vector2.UnitY);

            // Sync if the direction is different from the old one.
            // Spam caps are ignored due to the frequency of this happening.
            if (newAimDirection != Projectile.velocity)
            {
                Projectile.netUpdate = true;
                Projectile.netSpam = 0;
            }

            Projectile.velocity = newAimDirection;
        }
        private float PrimitiveWidthFunction(float completionRatio, Vector2 vertexPos)
        {
            float width;
            float maxBodyWidth = Projectile.scale * MaxLaserWidth * (1 + MathF.Pow(Utils.GetLerpValue(450, 500, VoidragonHoldout.ModProjectile<VoidragonHoldout>().beamTimer, true), 3.5f) * 4);
            float shrinkRatio = 0.018f;

            //if (completionRatio < shrinkRatio)
            //    width = MathF.Sin(completionRatio / shrinkRatio * MathHelper.PiOver2) * maxBodyWidth + shrinkRatio;
            //else
            //    width = Utils.Remap(completionRatio, shrinkRatio, 1f, maxBodyWidth, 0f);

            return maxBodyWidth * Utils.GetLerpValue(0f, 0.05f, completionRatio, true) * Utils.GetLerpValue(LaserLength, LaserLength - 0.1f, completionRatio, true);
        }

        private Color PrimitiveColorFunction(float completionRatio, Vector2 vertexPos)
        {

            return Color.White * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> mainStreakTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/HarshNoise");
            int beamTimer = VoidragonHoldout.ModProjectile<VoidragonHoldout>().beamTimer;
            float beamWidthInterpolant = Utils.GetLerpValue(500, 460, beamTimer, true);

            if (ControlPoints is null)
                return false;

            Main.spriteBatch.End(out var snapshot);

            var device = Main.instance.GraphicsDevice;
            using var lease = RenderTargetPool.Shared.Rent(
                device,
                Main.screenWidth / 2,
                Main.screenHeight / 2,
                RenderTargetDescriptor.Default
            );

            using (lease.Scope(clearColor: Color.Transparent))
            {
                MiscShaderData shader = GameShaders.Misc["CalamityMod:AbyssalFire"];
                shader.Shader.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly);
                shader.Shader.Parameters["glowPower"].SetValue(0.8f);
                shader.Shader.Parameters["overallColorStrength"].SetValue(1f - beamWidthInterpolant);
                shader.Shader.Parameters["edgeFadeoutThreshold"].SetValue(0.46f);
                shader.Shader.Parameters["noiseScale"].SetValue(new Vector2(4f, 0.5f));
                shader.Shader.Parameters["innerColor"].SetValue(Color.DarkViolet.ToVector3());
                shader.Shader.Parameters["outerColor"].SetValue(Color.Black.ToVector3());
                shader.Shader.Parameters["overallColor"].SetValue(Color.White.ToVector3());
                shader.Shader.Parameters["tipColor"].SetValue(Color.White.ToVector3());

                device.Textures[1] = mainStreakTexture.Value;

                PrimitiveRenderer.RenderTrail(ControlPoints, new(PrimitiveWidthFunction, PrimitiveColorFunction, shader: shader, useUnscaledMatrices: true, capStyle: PrimitiveCapStyle.Flat), ControlPoints.Length);
            }

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(lease.Target, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
            Main.spriteBatch.End();

            Main.spriteBatch.Begin(snapshot);

            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * MaxLaserLength);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player Owner = Main.player[Projectile.owner];
            target.AddBuff(ModContent.BuffType<WhisperingDeath>(), 180);
            int bonusDamage = 200 * Owner.Calamity().sharkGunDamageScaling;
            if (target.Calamity().demonicFlamesBonusDamage <= bonusDamage)
            {
                target.Calamity().demonicFlamesBonusDamage = bonusDamage;
                target.AddBuff(ModContent.BuffType<DemonicFlames>(), 180);
                // Demonic Flames damage must be synced, because OnHitNPC is only run for the client that hit the NPC
                if (Main.netMode != NetmodeID.SinglePlayer)
                    DemonicFlamesSyncPacket.Send(target);
            }
        }

        public override bool ShouldUpdatePosition() => false;
    }
}
