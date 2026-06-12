using System;
using System.Collections.Generic;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Particles;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class SHPV : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        private static Asset<Texture2D> Smoke = ModContent.Request<Texture2D>("CalamityMod/Particles/MediumMist");
        private static Asset<Texture2D> SoulSquare = ModContent.Request<Texture2D>("CalamityMod/Particles/Square");

        private Player Owner => Main.player[Projectile.owner];
        private SlotId VacuumSound;
        private const int VacuumStartFrames = 78;
        private const int VacuumLoopFrames = 132;
        private bool PlayedEndSound = false;
        public ref float Timer => ref Projectile.ai[0];

        // The maximum distance in pixels out from the tip of the gun in which souls can be sucked in.
        private const float Size = 576f;
        // The spread of the suction on each side. 27 degrees on each side, 54 degrees total.
        private const float Spread = MathHelper.Pi / 6.66f;
        // Maximum number of souls per visual ring drawn on the gun. Additional souls get moved into additional rings.
        private const int MaxSoulsRing = 10;
        // Manual offset used for drawing the gun.
        private static Vector2 Offset => new Vector2(27f, -10f);
        // The position of the tip of the gun. Used for most projectile logic.
        public Vector2 TipPosition => Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * 62f + Vector2.UnitY * Offset.Y;
        // A list containing the ai[0] values of each soul that is sucked in, used for determining the color to draw the soul as.
        public List<float> SoulColors = [];
        // Tracks whether or not the gun should be doing the laser attack.
        private bool FiringLasers = false;
        // Allows for current soul color to be "consumed" every other laser shot.
        private bool ConsumeSoul = false;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 70;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ContinuouslyUpdateDamageStats = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            // Stop using the holdout if you cannot
            if (Owner.CantUseHoldout(false))
            {
                Projectile.Kill();
                return;
            }
            // Start sound
            if (Timer == 0f)
                VacuumSound = SoundEngine.PlaySound(SHPC.VacuumStart, Owner.Center);
            // Move the vacuum sound on top of the player constantly
            if (SoundEngine.TryGetActiveSound(VacuumSound, out var ChargeSound) && ChargeSound.IsPlaying)
                ChargeSound.Position = Owner.Center;
            Timer++;

            // If you release right click, use the laser attack
            if (!Owner.Calamity().mouseRight)
                FiringLasers = true;
            if (FiringLasers)
            {
                // Vacuum end sound
                if (!PlayedEndSound)
                {
                    if (SoundEngine.TryGetActiveSound(VacuumSound, out var Vacuum))
                        Vacuum?.Stop();
                    VacuumSound = SoundEngine.PlaySound(SHPC.VacuumEnd, Owner.Center);
                    PlayedEndSound = true;
                }

                if (SoulColors.Count > 0)
                {
                    if (Timer % 5 == 0f)
                    {
                        if (Owner.HeldItem.ModItem is SHPC shpc)
                        {
                            SoundEngine.PlaySound(CommonCalamitySounds.ELRFireSound, Owner.Center);
                            Vector2 laserPos = TipPosition + Vector2.UnitY.RotatedBy(Projectile.rotation) * Main.rand.NextFloat(-7f, 7f);
                            Vector2 laserVel = Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld) * 20f;
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), laserPos, laserVel, ModContent.ProjectileType<SHPL>(), (int)(Projectile.damage * 1.4f), 3f, Projectile.owner, SoulColors[0]);

                            if (ConsumeSoul)
                            {
                                SoulColors.RemoveAt(0);
                                if (shpc.storedSoulpower > 0)
                                    shpc.storedSoulpower--;
                                else if (SHPC.FindSoulForAmmo(Owner) != -1)
                                {
                                    int soulType = SHPC.FindSoulForAmmo(Owner);
                                    Owner.ConsumeItem(soulType);
                                    shpc.storedSoulType = soulType;
                                    shpc.storedSoulpower = SHPC.ShotsPerSoul;
                                }
                                else
                                {
                                    SoulColors.Clear();
                                }
                            }
                            ConsumeSoul = !ConsumeSoul;
                        }
                        else
                            SoulColors.Clear();
                    }
                }
            }
            else
            {
                // Debug code used for assessing the visual and functional area of the vacuum
                /*BloomLineVFX l = new(TipPosition, Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld).RotatedBy(Spread) * Size, 0.4f, Color.Gray, 2, true);
                BloomLineVFX l2 = new(TipPosition, Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld).RotatedBy(-Spread) * Size, 0.4f, Color.Gray, 2, true);
                GeneralParticleHandler.SpawnParticle(l);
                GeneralParticleHandler.SpawnParticle(l2);*/

                // Spawn faded smoke particles idly
                if (Timer % 2 == 0f)
                {
                    Vector2 spawn = Owner.Center + Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld).RotatedByRandom(Spread) * Size * Main.rand.NextFloat(0.4f, 1f);
                    MediumMistParticle smoky = new(spawn, Utils.DirectionTo(spawn, TipPosition) * 4.5f, Color.Gray, Color.DarkGray, 1.25f, 96f);
                    GeneralParticleHandler.SpawnParticle(smoky);
                }

                // Vacuum loop sound
                if ((Timer - VacuumStartFrames) % VacuumLoopFrames == 0)
                    VacuumSound = SoundEngine.PlaySound(SHPC.VacuumLoop, Owner.Center);
            }

            // Typical held projectile shit
            Owner.heldProj = Projectile.whoAmI;
            Owner.ChangeDir(Math.Sign((Owner.Calamity().mouseWorld - Owner.Center).X));
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (Owner.Center - Owner.Calamity().mouseWorld).ToRotation() * Owner.gravDir + MathHelper.PiOver2);
            Owner.SetDummyItemTime(2);
            Projectile.rotation = Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld).ToRotation();
            Projectile.velocity = Vector2.Zero;
            Projectile.Center = Owner.Center;
            // This is used to make the projectile naturally despawn with a delay after running out of souls or if you stop firing with no souls
            if (!FiringLasers || SoulColors.Count > 0)
                Projectile.timeLeft = 48;
        }

        public override void OnKill(int timeLeft)
        {
            if (SoundEngine.TryGetActiveSound(VacuumSound, out var ChargeSound))
                ChargeSound?.Stop();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // You shouldn't be sucking in souls while firing
            if (FiringLasers)
                return false;

            bool withinAngle = Math.Abs(Utils.DirectionTo(TipPosition, Owner.Calamity().mouseWorld).ToRotation() - Utils.DirectionTo(TipPosition, targetHitbox.Center.ToVector2()).ToRotation()) <= Spread;
            // This extra safety hitbox is a square around the gun tip used so that the soul doesn't need to perfectly travel down the narrow end of the vacuum to get sucked in
            Rectangle extraSafetyHitbox = new Rectangle((int)TipPosition.X - (Projectile.width / 2), (int)TipPosition.Y - (Projectile.height / 2), Projectile.width, Projectile.height);
            return (CalamityUtils.CircularHitboxCollision(TipPosition, Size, targetHitbox) && withinAngle) || targetHitbox.Intersects(extraSafetyHitbox);
        }
        public override bool? CanDamage() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            // Draw spiraling smoke particles representing the suction radius
            Vector2 farthestPos = Owner.Center + Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld) * Size;
            float rotation = Utils.DirectionTo(TipPosition, farthestPos).ToRotation();
            if (!FiringLasers)
            {
                Texture2D tex = Smoke.Value;
                Rectangle frame = tex.Frame(1, 3, 0, Main.rand.Next(3));

                Main.spriteBatch.SetBlendState(BlendState.Additive);
                for (int i = 0; i < 6; i++)
                {
                    // There are 3 separate streams offset from one another
                    for (int j = 0; j < 3; j++)
                    {
                        float distRatio = 1f - (Main.GameUpdateCount + j * 10) % 30 / 30f;
                        Vector2 posOffset = new Vector2(MathF.Sin(MathHelper.TwoPi / 6f * i) * 25f * distRatio, MathF.Cos(Main.GameUpdateCount * MathHelper.Pi / 30f + MathHelper.TwoPi / 6f * i) * 240f * distRatio).RotatedBy(rotation);
                        float colorMult = 0.5f * Utils.GetLerpValue(1f, 0.8f, distRatio, true);
                        Main.EntitySpriteDraw(tex, Vector2.Lerp(TipPosition, farthestPos, distRatio) + posOffset - Main.screenPosition, frame, Color.Gray * colorMult, rotation + MathHelper.Pi, frame.Size() / 2f, 1.5f * distRatio, SpriteEffects.None);
                    }
                }
                Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            }

            // Manually draw the gun itself
            Texture2D shpc = TextureAssets.Item[ModContent.ItemType<SHPC>()].Value;
            Vector2 position = Owner.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(rotation) * Offset.X + Vector2.UnitY * Offset.Y;
            SpriteEffects sp = Owner.Calamity().mouseWorld.X < Owner.Center.X ? SpriteEffects.FlipVertically : SpriteEffects.None;
            Main.EntitySpriteDraw(shpc, position, null, lightColor, rotation, shpc.Size() / 2f, 1f, sp);

            // Draw souls around the gun as a sort of indicator of how many you have sucked in
            if (SoulColors.Count > 0)
            {
                Texture2D orbitingSoulTexture = SoulSquare.Value;
                for (int i = 0; i < SoulColors.Count; i++)
                {
                    int soulsInRing = SoulColors.Count > MaxSoulsRing ? (SoulColors.Count - i > SoulColors.Count % MaxSoulsRing ? MaxSoulsRing : SoulColors.Count % MaxSoulsRing) : SoulColors.Count;
                    // Souls past 10 get moved into additional rings
                    int ringsBack = i / MaxSoulsRing;
                    Vector2 soulPosition = Projectile.Center + Vector2.UnitX.RotatedBy(rotation) * (68f - (ringsBack * 9f)) + Vector2.UnitY * -12.5f;
                    Vector2 posOffset = new Vector2(MathF.Sin(Main.GameUpdateCount * MathHelper.Pi / 30f + MathHelper.TwoPi / soulsInRing * (i % MaxSoulsRing)) * 3f, MathF.Cos(Main.GameUpdateCount * MathHelper.Pi / 30f + MathHelper.TwoPi / soulsInRing * (i % MaxSoulsRing)) * 25f).RotatedBy(rotation);
                    
                    // Makes the souls disappear when they go "behind" the gun
                    bool shouldDraw = posOffset.RotatedBy(-rotation).X <= 2.7f;
                    if (shouldDraw)
                    {
                        Main.EntitySpriteDraw(orbitingSoulTexture, soulPosition + posOffset - Main.screenPosition, null, SHPB.FindColorForSoul((int)SoulColors[i]), 0f, orbitingSoulTexture.Size() * 0.5f, 1f, SpriteEffects.None);
                        Main.EntitySpriteDraw(orbitingSoulTexture, soulPosition + posOffset - Main.screenPosition, null, Color.White, 0f, orbitingSoulTexture.Size() * 0.5f, 0.5f, SpriteEffects.None);
                    }
                }
            }
            return false;
        }
    }
}
