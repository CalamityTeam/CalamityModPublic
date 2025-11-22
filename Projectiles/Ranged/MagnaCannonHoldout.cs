using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class MagnaCannonHoldout : BaseGunHoldoutProjectile
    {
        public override int AssociatedItemID => ModContent.ItemType<MagnaCannon>();
        public override float MaxOffsetLengthFromArm => 20f;
        public override float OffsetXUpwards => -5f;
        public override float BaseOffsetY => -5f;

        private ref float CurrentChargingFrames => ref Projectile.ai[0];
        private ref float ShotsLoaded => ref Projectile.ai[1];
        private ref float ShootTimer => ref Projectile.ai[2];
        private bool FullyCharged => CurrentChargingFrames >= MagnaCannon.FullChargeFrames;
        public SlotId MagnaChargeSlot;
        public static int FramesPerLoad = 9;
        public static int MaxLoadableShots = 20;
        public static float BulletSpeed = 12f;

        public override void KillHoldoutLogic()
        {
            if (Owner.CantUseHoldout(false) || HeldItem.type != Owner.ActiveItem().type)
                Projectile.Kill();
        }

        public override void HoldoutAI()
        {
            if (SoundEngine.TryGetActiveSound(MagnaChargeSlot, out var ChargeSound) && ChargeSound.IsPlaying)
                ChargeSound.Position = Projectile.Center;

            // Fire if the owner stops channeling or otherwise cannot use the weapon.
            if (Owner.CantUseHoldout())
            {
                KeepRefreshingLifetime = false;

                if (ShotsLoaded > 0)
                {
                    // While bullets are remaining, refresh the lifespan; it will not refresh again after bullets run out
                    Projectile.timeLeft = MagnaCannon.AftershotCooldownFrames;
                    ShootTimer--;
                }

                if (ShootTimer <= 0f)
                {
                    ChargeSound?.Stop();
                    SoundEngine.PlaySound(MagnaCannon.Fire, Projectile.position);

                    Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * BulletSpeed;
                    if (Main.myPlayer == Projectile.owner)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, shootVelocity.RotatedByRandom(MathHelper.ToRadians(9f)), ModContent.ProjectileType<MagnaShot>(), Projectile.damage, Projectile.knockBack * (FullyCharged ? 3 : 1), Projectile.owner);
                    for (int i = 0; i <= 3; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(GunTipPosition, 187, shootVelocity.RotatedByRandom(MathHelper.ToRadians(15f)) * Main.rand.NextFloat(0.9f, 1.2f), 0, default, Main.rand.NextFloat(1.5f, 2.3f));
                        dust.noGravity = true;
                    }

                    ShotsLoaded--;
                    ShootTimer = FullyCharged ? 4f : 5f;
                }
            }
            else
            {
                // Loads shots until maxed out
                if (ShotsLoaded < MaxLoadableShots && CurrentChargingFrames % FramesPerLoad == 0)
                    ShotsLoaded++;

                CurrentChargingFrames++;

                // Sounds
                if (FullyCharged)
                {
                    ShotsLoaded = MaxLoadableShots;
                    if (CurrentChargingFrames == MagnaCannon.FullChargeFrames)
                        MagnaChargeSlot = SoundEngine.PlaySound(MagnaCannon.ChargeFull, Projectile.Center);
                    else if ((CurrentChargingFrames - MagnaCannon.FullChargeFrames - MagnaCannon.ChargeFullSoundFrames) % MagnaCannon.ChargeLoopSoundFrames == 0)
                        MagnaChargeSlot = SoundEngine.PlaySound(MagnaCannon.ChargeLoop, Projectile.Center);
                }
                else if (CurrentChargingFrames == 10)
                    MagnaChargeSlot = SoundEngine.PlaySound(MagnaCannon.ChargeStart, Projectile.Center);

                // Charge-up visuals
                if (CurrentChargingFrames >= 10)
                {
                    if (!FullyCharged)
                    {
                        Particle streak = new ManaDrainStreak(Owner, Main.rand.NextFloat(0.06f + (CurrentChargingFrames / 180), 0.08f + (CurrentChargingFrames / 180)), Main.rand.NextVector2CircularEdge(2f, 2f) * Main.rand.NextFloat(0.3f * CurrentChargingFrames, 0.3f * CurrentChargingFrames), 0f, Color.White, Color.Aqua, 7, GunTipPosition);
                        GeneralParticleHandler.SpawnParticle(streak);
                    }
                    float orbScale = MathHelper.Clamp(CurrentChargingFrames, 0f, MagnaCannon.FullChargeFrames);
                    Particle orb = new GenericBloom(GunTipPosition, Projectile.velocity, Color.DarkBlue, orbScale / 135f, 2);
                    GeneralParticleHandler.SpawnParticle(orb);
                    Particle orb2 = new GenericBloom(GunTipPosition, Projectile.velocity, Color.Aqua, orbScale / 200f, 2);
                    GeneralParticleHandler.SpawnParticle(orb2);
                }

                // Full charge dusts
                if (CurrentChargingFrames == MagnaCannon.FullChargeFrames)
                {
                    for (int i = 0; i < 36; i++)
                    {
                        Dust chargefull = Dust.NewDustPerfect(GunTipPosition, 160);
                        chargefull.velocity = (MathHelper.TwoPi * i / 36f).ToRotationVector2() * 18f + Owner.velocity;
                        chargefull.scale = Main.rand.NextFloat(1f, 1.5f);
                        chargefull.noGravity = true;
                    }
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (SoundEngine.TryGetActiveSound(MagnaChargeSlot, out var ChargeSound))
                ChargeSound?.Stop();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f);
            Vector2 rotationPoint = texture.Size() * 0.5f;
            SpriteEffects flipSprite = (Projectile.spriteDirection * Owner.gravDir == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            if (!Owner.CantUseHoldout())
            {
                float rumble = MathHelper.Clamp(CurrentChargingFrames, 0f, MagnaCannon.FullChargeFrames);
                drawPosition += Main.rand.NextVector2Circular(rumble / 43f, rumble / 43f);
            }

            Main.EntitySpriteDraw(texture, drawPosition, null, Projectile.GetAlpha(lightColor), drawRotation, rotationPoint, Projectile.scale * Owner.gravDir, flipSprite);

            return false;
        }
    }
}
