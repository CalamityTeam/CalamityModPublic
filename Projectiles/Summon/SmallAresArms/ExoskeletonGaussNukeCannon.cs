using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.Sounds;
using CalamityMod.Items.Weapons.Summon;
using System;

namespace CalamityMod.Projectiles.Summon.SmallAresArms
{
    public class ExoskeletonGaussNukeCannon : ExoskeletonCannon
    {
        public int NukeRebuildTime => ShootRate - 45;

        public ref float RebuildTimer => ref Projectile.localAI[0];

        public override int ShootRate => AresExoskeleton.GaussNukeShootRate;

        public override float ShootSpeed => 16f;

        public override Vector2 OwnerRestingOffset => HoverOffsetTable[HoverOffsetIndex];

        public override void ClampFirstLimbRotation(ref double limbRotation) => limbRotation = RotationalClampTable[HoverOffsetIndex];

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 12;
        }

        public override void ShootAtTarget(NPC target, Vector2 shootDirection)
        {
            // Play the large gun fire sound.
            SoundEngine.PlaySound(CommonCalamitySounds.LargeWeaponFireSound with { Volume = 0.4f }, Projectile.Center);

            // Start the rebuild timer.
            RebuildTimer = 1f;

            // Shoot the fireball. This only happens for the owner client.
            if (Main.myPlayer != Projectile.owner)
                return;

            int nukeID = ModContent.ProjectileType<MinionGaussNuke>();
            Vector2 laserVelocity = shootDirection * ShootSpeed;
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, laserVelocity, nukeID, (int)(Projectile.damage * AresExoskeleton.NukeDamageFactor), 0f, Projectile.owner);
        }

        public override void PostAI()
        {
            Projectile.frameCounter++;
            Projectile.frame = Projectile.frameCounter / 5 % 6;
            if (RebuildTimer >= 1f)
            {
                RebuildTimer++;
                Projectile.frame += 6;
            }

            if (RebuildTimer >= NukeRebuildTime)
            {
                RebuildTimer = 0f;
                Projectile.frameCounter = 0;
                Projectile.frame = 0;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glowmask = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/SmallAresArms/ExoskeletonGaussNukeCannonGlowmask").Value;

            // The two stabilizers have two different shades of lighting, thus necessitating two different textures.
            Texture2D stabilizer = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/SmallAresArms/ExoskeletonGaussNukeStabilizer").Value;
            Texture2D stabilizer2 = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/SmallAresArms/ExoskeletonGaussNukeStabilizerBottom").Value;
            Texture2D crystal = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/SmallAresArms/ExoskeletonGaussNukeCrystal").Value;
            Texture2D core = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/SmallAresArms/ExoskeletonGaussNukeCore").Value;
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);

            float stabilizerRotationInterpolant = (float)Math.Pow(Utils.GetLerpValue(NukeRebuildTime * 0.75f, NukeRebuildTime * 0.92f, RebuildTimer, true), 1.7);
            Vector2 origin = frame.Size() * 0.5f;
            Vector2 crystalOrigin = crystal.Size() * new Vector2(Projectile.spriteDirection == 1 ? 1f : 0f, 0.5f);
            Vector2 stabilizerOrigin = stabilizer.Size() * new Vector2(0.5f, 1f);
            Vector2 coreOrigin = core.Size() * new Vector2(Projectile.spriteDirection == 1 ? 0f : 1f, 0.5f);
            Vector2 perpendicularStabilizerOffset = (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2() * Projectile.scale * MathHelper.Lerp(18f, 10f, stabilizerRotationInterpolant);

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            SpriteEffects direction = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float rotation = Projectile.rotation;
            if (Projectile.spriteDirection == -1)
                rotation += MathHelper.Pi;

            DrawLimbs();

            // Draw the nuke pieces when rebuilding. This is done before the cannon itself is drawn, to make it look like they're attached to it.
            if (RebuildTimer >= 1f)
            {
                float componentDirection = rotation + MathHelper.Pi;
                float stabilizerDirection = componentDirection;
                if (Projectile.spriteDirection == -1)
                    stabilizerDirection += MathHelper.Pi;

                Vector2 aimDirection = Projectile.rotation.ToRotationVector2();

                // Draw the crystal.
                float crystalOffsetInterpolant = Utils.GetLerpValue(0f, NukeRebuildTime * 0.5f, RebuildTimer, true);
                Vector2 crystalOffset = aimDirection * crystalOffsetInterpolant * Projectile.scale * (crystal.Width - 15f);
                Main.EntitySpriteDraw(crystal, drawPosition + crystalOffset, null, Projectile.GetAlpha(lightColor), rotation, crystalOrigin, Projectile.scale, direction, 0);

                // Draw the stabilizers.
                float stabilizerOffsetInterpolant = Utils.GetLerpValue(NukeRebuildTime * 0.5f, NukeRebuildTime * 0.75f, RebuildTimer, true);
                float stabilizerOffsetMagnitude = MathHelper.Lerp(-stabilizer.Width * 0.32f, stabilizer.Width * 0.4f - 8f, stabilizerOffsetInterpolant) * Projectile.scale;
                float leftStabilizerRotation = stabilizerDirection + MathHelper.Pi - (1f - stabilizerRotationInterpolant) * 0.42f;
                float rightStabilizerRotation = stabilizerDirection + (1f - stabilizerRotationInterpolant) * 0.42f;
                Vector2 stabilizerOffset = aimDirection * stabilizerOffsetMagnitude;
                Color stabilizerColor = Projectile.GetAlpha(lightColor) * (float)Math.Pow(stabilizerOffsetInterpolant, 1.61);
                Vector2 leftStabilizerPosition = drawPosition + stabilizerOffset - perpendicularStabilizerOffset * 0.9f;
                Vector2 rightStabilizerPosition = drawPosition + stabilizerOffset + perpendicularStabilizerOffset;
                SpriteEffects leftStabilizerDirection = direction;
                SpriteEffects rightStabilizerDirection = direction ^ SpriteEffects.FlipHorizontally;
                if (Projectile.spriteDirection == -1)
                {
                    leftStabilizerDirection ^= SpriteEffects.FlipHorizontally;
                    rightStabilizerDirection ^= SpriteEffects.FlipHorizontally;
                    Utils.Swap(ref stabilizer, ref stabilizer2);
                }
                Main.EntitySpriteDraw(stabilizer, leftStabilizerPosition, null, stabilizerColor, leftStabilizerRotation, stabilizerOrigin, Projectile.scale, leftStabilizerDirection, 0);
                Main.EntitySpriteDraw(stabilizer2, rightStabilizerPosition, null, stabilizerColor, rightStabilizerRotation, stabilizerOrigin, Projectile.scale, rightStabilizerDirection, 0);

                // Draw the core.
                Vector2 coreDrawPosition = drawPosition + aimDirection * Projectile.scale * -12f;
                Main.EntitySpriteDraw(core, coreDrawPosition, null, Projectile.GetAlpha(lightColor) * stabilizerRotationInterpolant, rotation, coreOrigin, Projectile.scale, direction, 0);
            }

            Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), rotation, origin, Projectile.scale, direction, 0);
            Main.EntitySpriteDraw(glowmask, drawPosition, frame, Projectile.GetAlpha(Color.White), rotation, origin, Projectile.scale, direction, 0);

            return false;
        }
    }
}
