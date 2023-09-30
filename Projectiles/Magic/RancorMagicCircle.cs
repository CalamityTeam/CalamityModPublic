using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class RancorMagicCircle : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public Player Owner => Main.player[Projectile.owner];
        public ref float Time => ref Projectile.ai[0];
        private SlotId PulseLoopSoundSlot;
        public ActiveSound PulseLoopSound
        {
            get
            {
                ActiveSound sound;
                if (SoundEngine.TryGetActiveSound(PulseLoopSoundSlot, out sound))
                    return sound;
                return null;
            }
        }
        public float ChargeupCompletion => MathHelper.Clamp(Time / ChargeupTime, 0f, 1f);
        public const int ChargeupTime = 240;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 114;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 90000;
        }

        public override void AI()
        {
            // Always update before the laserbeam, so that it doesn't recieve strange offsets.
            Projectile.Calamity().UpdatePriority = 1f;

            // If the owner is no longer able to cast the circle, kill it.
            if (!Owner.channel || Owner.noItems || Owner.CCed)
            {
                Projectile.Kill();
                return;
            }

            if (Time >= 1f && Owner.ownedProjectileCounts[ModContent.ProjectileType<RancorHoldout>()] <= 0)
            {
                Projectile.Kill();
                return;
            }

            // Adjust visual values such as scale and opacity when charging.
            AdjustVisualValues();

            // Update aim.
            UpdateAim();

            // Decide where to position the magic circle.
            Vector2 circlePointDirection = Projectile.velocity.SafeNormalize(Vector2.UnitX * Owner.direction);
            Projectile.Center = Owner.Center + circlePointDirection * Projectile.scale * 56f;

            // Adjust the owner's direction.
            Owner.ChangeDir(Projectile.direction);

            // Do animation stuff.
            DoPrettyDustEffects();


            ActiveSound soundOut;
            // Handle charge stuff.
            if (Time < ChargeupTime)
                HandleChargeEffects();

            // Create an idle ominous sound once the laser has appeared.
            else if (!SoundEngine.TryGetActiveSound(PulseLoopSoundSlot, out soundOut) || !soundOut.IsPlaying)
                PulseLoopSoundSlot = SoundEngine.PlaySound(SoundID.DD2_EtherianPortalIdleLoop with { IsLooped = true }, Projectile.Center);

            // Make a cast sound effect soon after the circle appears.
            if (Time == 15f)
                SoundEngine.PlaySound(SoundID.Item117, Projectile.Center);

            Time++;
        }

        public void AdjustVisualValues()
        {
            Projectile.scale = Utils.GetLerpValue(0f, 35f, Time, true) * 1.4f;
            Projectile.Opacity = (float)Math.Pow(Projectile.scale / 1.4f, 2D);
            Projectile.rotation -= MathHelper.ToRadians(Projectile.scale * 4f);
        }

        public void UpdateAim()
        {
            // Only execute the aiming code for the owner since Main.MouseWorld is a client-side variable.
            if (Main.myPlayer != Projectile.owner)
                return;

            Vector2 idealDirection = Owner.SafeDirectionTo(Main.MouseWorld, Vector2.UnitX * Owner.direction);
            Vector2 newAimDirection = Projectile.velocity.MoveTowards(idealDirection, 0.05f);

            // Sync if the direction is different from the old one.
            // Spam caps are ignored due to the frequency of this happening.
            if (newAimDirection != Projectile.velocity)
            {
                Projectile.netUpdate = true;
                Projectile.netSpam = 0;
            }

            Projectile.velocity = newAimDirection;
            Projectile.direction = (Projectile.velocity.X > 0f).ToDirectionInt();
        }

        public void DoPrettyDustEffects()
        {
            int dustSpawnChance = (int)MathHelper.SmoothStep(16f, 4f, ChargeupCompletion);
            for (int i = 0; i < 2; i++)
            {
                if (!Main.rand.NextBool(dustSpawnChance))
                    continue;

                float dustSpawnOffsetFactor = Main.rand.NextFloat(Projectile.width * 0.375f, Projectile.width * 0.485f);
                Vector2 dustSpawnOffsetDirection = Main.rand.NextVector2CircularEdge(0.5f, 1f).RotatedBy(Projectile.velocity.ToRotation());
                Vector2 dustSpawnOffset = dustSpawnOffsetDirection * dustSpawnOffsetFactor;
                Vector2 dustVelocity = (-dustSpawnOffset.SafeNormalize(Vector2.UnitY)).RotatedBy(MathHelper.PiOver2 * Main.rand.NextFloatDirection());
                dustVelocity *= Main.rand.NextFloat(2f, 6f);

                Dust magic = Dust.NewDustPerfect(Projectile.Center + dustSpawnOffset, 264);
                magic.color = Color.Lerp(Color.Red, Color.Blue, Main.rand.NextFloat());
                magic.velocity = dustVelocity;
                magic.scale *= Main.rand.NextFloat(1f, 1.4f);
                magic.noLight = true;
                magic.noGravity = true;
            }
        }

        public void HandleChargeEffects()
        {
            // Create dust that fires parallel to the direction of the circle.
            if (Main.rand.NextBool(3))
            {
                float dustSpeed = MathHelper.Lerp(3.5f, 8f, ChargeupCompletion) * Main.rand.NextFloat(0.65f, 1f);
                float dustSpawnOffsetFactor = Main.rand.NextFloat(Projectile.width * 0.375f, Projectile.width * 0.485f);
                Vector2 dustVelocity = Projectile.velocity * dustSpeed;
                Vector2 dustSpawnOffsetDirection = Main.rand.NextVector2CircularEdge(0.5f, 1f).RotatedBy(Projectile.velocity.ToRotation());
                Vector2 dustSpawnOffset = dustSpawnOffsetDirection * dustSpawnOffsetFactor;

                Dust magic = Dust.NewDustPerfect(Projectile.Center + dustSpawnOffset, 264);
                magic.color = Color.Lerp(Color.Red, Color.Blue, Main.rand.NextFloat());
                magic.velocity = dustVelocity;
                magic.scale *= Main.rand.NextFloat(1f, 1.05f + ChargeupCompletion * 0.55f);
                magic.noLight = true;
                magic.noGravity = true;
            }

            // Create the laser once the charge animation is complete.
            if (Time == ChargeupTime - 1f)
            {
                // Play a laserbeam deathray sound. Should probably be replaced some day
                SoundEngine.PlaySound(SoundID.Zombie104, Projectile.Center);

                if (Main.myPlayer == Projectile.owner)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<RancorLaserbeam>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.identity);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D outerCircleTexture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D outerCircleGlowmask = ModContent.Request<Texture2D>(Texture + "Glowmask").Value;
            Texture2D innerCircleTexture = ModContent.Request<Texture2D>(Texture + "Inner").Value;
            Texture2D innerCircleGlowmask = ModContent.Request<Texture2D>(Texture + "InnerGlowmask").Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            float directionRotation = Projectile.velocity.ToRotation();
            Color startingColor = Color.Red;
            Color endingColor = Color.Blue;

            void restartShader(Texture2D texture, float opacity, float circularRotation, BlendState blendMode)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, blendMode, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                CalamityUtils.CalculatePerspectiveMatricies(out Matrix viewMatrix, out Matrix projectionMatrix);

                GameShaders.Misc["CalamityMod:RancorMagicCircle"].UseColor(startingColor);
                GameShaders.Misc["CalamityMod:RancorMagicCircle"].UseSecondaryColor(endingColor);
                GameShaders.Misc["CalamityMod:RancorMagicCircle"].UseSaturation(directionRotation);
                GameShaders.Misc["CalamityMod:RancorMagicCircle"].UseOpacity(opacity);
                GameShaders.Misc["CalamityMod:RancorMagicCircle"].Shader.Parameters["uDirection"].SetValue((float)Projectile.direction);
                GameShaders.Misc["CalamityMod:RancorMagicCircle"].Shader.Parameters["uCircularRotation"].SetValue(circularRotation);
                GameShaders.Misc["CalamityMod:RancorMagicCircle"].Shader.Parameters["uImageSize0"].SetValue(texture.Size());
                GameShaders.Misc["CalamityMod:RancorMagicCircle"].Shader.Parameters["overallImageSize"].SetValue(outerCircleTexture.Size());
                GameShaders.Misc["CalamityMod:RancorMagicCircle"].Shader.Parameters["uWorldViewProjection"].SetValue(viewMatrix * projectionMatrix);
                GameShaders.Misc["CalamityMod:RancorMagicCircle"].Apply();
            }

            restartShader(outerCircleGlowmask, Projectile.Opacity, Projectile.rotation, BlendState.Additive);
            Main.EntitySpriteDraw(outerCircleGlowmask, drawPosition, null, Color.White, 0f, outerCircleGlowmask.Size() * 0.5f, Projectile.scale * 1.075f, SpriteEffects.None, 0);

            restartShader(outerCircleTexture, Projectile.Opacity * 0.7f, Projectile.rotation, BlendState.AlphaBlend);
            Main.EntitySpriteDraw(outerCircleTexture, drawPosition, null, Color.White, 0f, outerCircleTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            restartShader(innerCircleGlowmask, Projectile.Opacity * 0.5f, 0f, BlendState.Additive);
            Main.EntitySpriteDraw(innerCircleGlowmask, drawPosition, null, Color.White, 0f, innerCircleGlowmask.Size() * 0.5f, Projectile.scale * 1.075f, SpriteEffects.None, 0);

            restartShader(innerCircleTexture, Projectile.Opacity * 0.7f, 0f, BlendState.AlphaBlend);
            Main.EntitySpriteDraw(innerCircleTexture, drawPosition, null, Color.White, 0f, innerCircleTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.ExitShaderRegion();

            return false;
        }

        public override void OnKill(int timeLeft) => PulseLoopSound?.Stop();

        public override bool ShouldUpdatePosition() => false;

        public override bool? CanDamage() => false;
    }
}
