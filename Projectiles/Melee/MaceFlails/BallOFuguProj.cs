using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.MaceFlails
{
    public class BallOFuguProj : BaseMaceFlailProjectile
    {
        public override int AssociatedItemID => ModContent.ItemType<BallOFugu>();
        public override float SpinHitboxRadius => 80f;
        public override float SpinVisualRadius => 48f;
        public override float LaunchSpeed => 20f;
        public override int LaunchLifespan => 20;
        public override float MaxDropRange => 640f;
        public override float MaxRetractSpeed => 24f;
        public override float RetractAcceleration => 3.6f;

        public static float MaxSpikeTime = 180f;
        public static float SpikeRate = 10f;
        public static float SpikeDamage => 0.6f;
        public static float SpikeKnockback => 0.2f;
        public static Color SpikeColor => new Color(91, 62, 153);

        public ref float SpikeTimer => ref Projectile.ai[2];

        public static Asset<Texture2D> ChargeGlow;
        public override void Load() => ChargeGlow = ModContent.Request<Texture2D>("CalamityMod/Particles/LargeBloom");

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 38;
            Projectile.ignoreWater = true;
            base.SetDefaults();
        }

        public override void SpinAI(float launchSpeed)
        {
            SpikeTimer++;

            // Play Jellyfish idle sounds as it spins
            if (Projectile.soundDelay <= 0)
            {
                SoundEngine.PlaySound(SoundID.Zombie35 with { Pitch = -0.4f, PitchVariance = 0.2f }, Projectile.Center);
                Projectile.soundDelay = Main.rand.Next(48, 60);
            }

            // Spews spikes while spinning if timer exceeded
            // This is slightly random and a bit faster than building up normally
            if (Projectile.owner == Main.myPlayer && SpikeTimer > MaxSpikeTime + SpikeRate)
            {
                Vector2 velocity = Projectile.DirectionFrom(Owner.MountedCenter).SafeNormalize(Vector2.Zero).RotatedByRandom(MathHelper.ToRadians(15f)) * Main.rand.NextFloat(4.5f, 6.5f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<UrchinSpikeFugu>(), (int)(Projectile.damage * SpikeDamage), Projectile.knockBack * SpikeKnockback, Projectile.owner);
                SpikeTimer = MaxSpikeTime + Main.rand.Next((int)SpikeRate - 3);
            }
            base.SpinAI(launchSpeed);
        }

        public override Action<Projectile> EffectBeforePullback => (proj) =>
        {
            int SpikeCount = (int)(MathHelper.Clamp(SpikeTimer, 0f, MaxSpikeTime) / SpikeRate);
            if (SpikeCount > 0)
            {
                SoundEngine.PlaySound(BallOFugu.BlowSound, Projectile.Center);

                for (int i = -4; i < 5; i++)
                {
                    Vector2 maxScale = new Vector2(Main.rand.NextFloat(1f, 1.2f), Main.rand.NextFloat(0.6f, 0.75f)) * (i % 2 == 0 ? 1f : 1.35f);
                    float rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(30f * i);
                    CustomPulse spike = new CustomPulse(Projectile.Center, Vector2.One, SpikeColor, "CalamityMod/Particles/BlastCone", maxScale, rotation, 1f, 0.5f, 12);
                    GeneralParticleHandler.SpawnParticle(spike);
                }

                for (int i = 0; i < SpikeCount; i++)
                {
                    Vector2 velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(MathHelper.ToRadians(105f)) * Main.rand.NextFloat(6f, 7.2f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<UrchinSpikeFugu>(), (int)(Projectile.damage * SpikeDamage), Projectile.knockBack * SpikeKnockback, Projectile.owner);
                }
            }

            SpikeTimer = 0f;
            Projectile.netUpdate = true;
        };

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Poisoned, 180);

            if (CurrentFlailState == FlailState.LaunchingForward)
            {
                StateTimer = LaunchLifespan;
                Projectile.netUpdate = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (SpikeTimer > SpikeRate)
            {
                float power = Utils.GetLerpValue(0f, MaxSpikeTime, SpikeTimer, true);
                Main.EntitySpriteDraw(ChargeGlow.Value, Projectile.Center - Main.screenPosition, null, SpikeColor * (0.6f + 0.12f * power), 0f, ChargeGlow.Size() * 0.5f, 0.1f + 0.05f * power, SpriteEffects.None);
            }
            return base.PreDraw(ref lightColor);
        }
    }
}
