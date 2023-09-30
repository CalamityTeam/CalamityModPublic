using System;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class PressurizedBubbleStream : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public static int FireTime = 60;
        public static int ChargeupTime = 12;
        public float Progress => 1 - Projectile.timeLeft / (float)FireTime;
        public bool ChargingUp => Projectile.timeLeft > FireTime;
        public float ChargeupProgress => (ChargeupTime - (Projectile.timeLeft - FireTime)) / (float)ChargeupTime;

        public Particle chargeBubble;
        public Player Owner => Main.player[Projectile.owner];
        public ref float PulseTimer => ref Projectile.localAI[0];
        public ref float PrevPulseTimer => ref Projectile.localAI[1];

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 4;
            Projectile.timeLeft = FireTime + ChargeupTime;
        }

        public override bool ShouldUpdatePosition()
        {
            return !ChargingUp;
        }

        public override void AI()
        {
            ChargeupTime = 60;

            if (ChargingUp)
            {
                float bubbleRotation = Owner.itemRotation - MathHelper.PiOver4 / 3f * Owner.direction * Owner.gravDir;
                if (Owner.direction < 0)
                    bubbleRotation += MathHelper.Pi;


                Vector2 bubblePosition = ReedBlowgun.getPlayerMouth(Owner) + (bubbleRotation + MathHelper.PiOver4 / 15f * Owner.direction * Owner.gravDir).ToRotationVector2() * 48f;

                Projectile.Center = bubblePosition;
                Projectile.velocity = Vector2.UnitX.RotatedBy(bubbleRotation) * Projectile.velocity.Length();

                if (chargeBubble == null)
                {
                    chargeBubble = new GenericBubbleParticle(bubblePosition, Vector2.Zero, 0.4f, Owner.itemRotation, ChargeupTime / Projectile.extraUpdates);
                    GeneralParticleHandler.SpawnParticle(chargeBubble);
                }    
                else
                {
                    chargeBubble.Position = bubblePosition;
                    chargeBubble.Rotation = Owner.itemRotation;
                    chargeBubble.Scale = MathHelper.Lerp(0.4f, 1.4f, (float)Math.Sqrt(ChargeupProgress));
                }
                return;
            }

            if (Projectile.timeLeft == FireTime)
            {
                float bubbleRotation = Owner.itemRotation - (MathHelper.PiOver4 / 3f * Owner.direction);
                if (Owner.direction < 0)
                    bubbleRotation += MathHelper.Pi;

                Vector2 bubblePosition = Owner.MountedCenter - Vector2.UnitY * 4f + bubbleRotation.ToRotationVector2() * 55f;

                SoundEngine.PlaySound(ReedBlowgun.BubbleBurstSound, bubblePosition);
            }

            Projectile.velocity *= 0.985f;
            PulseTimer--;
            Lighting.AddLight(Projectile.Center, 0f, 0f, 0.5f);

            //Pulse
            if (PulseTimer <= 0 && Progress < 0.7f)
            {
                if (PrevPulseTimer == 0)
                    PrevPulseTimer = 2;
                else
                    PrevPulseTimer *= 2;

                PulseTimer = PrevPulseTimer * 2f;

                Color pulseColor = Main.rand.NextBool() ? (Main.rand.NextBool() ? Color.SkyBlue : Color.LightSkyBlue) : (Main.rand.NextBool() ? Color.LightBlue : Color.DeepSkyBlue);
                Particle pulse = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, pulseColor, new Vector2(0.5f, 1f), Projectile.velocity.ToRotation(), 0.05f, 0.2f + 0.3f * (1 - Progress), 30);
                GeneralParticleHandler.SpawnParticle(pulse);

                int numDust = 18;

                for (int i = 0; i < numDust; i++)
                {
                    Vector2 ringSpeed = new Vector2((float)Math.Cos(i / (float)numDust * MathHelper.TwoPi), (float)Math.Sin(i / (float)numDust * MathHelper.TwoPi) * 0.5f).RotatedBy(Projectile.velocity.ToRotation() + MathHelper.PiOver2) * (3.5f * (1 - Progress) + 3f) ;
                    Dust ringDust = Dust.NewDustPerfect(Projectile.position, 211, ringSpeed, 100, default, 1.25f);
                    ringDust.noGravity = true;

                }
            }

            //Bubbles
            if (!Main.rand.NextBool(3))
            {
                Gore bubble = Gore.NewGorePerfect(Projectile.GetSource_FromAI(), Projectile.position, Projectile.velocity * 0.2f + Main.rand.NextVector2Circular(1f, 1f) * Progress, 411);
                bubble.timeLeft = 8 + Main.rand.Next(6);
                bubble.scale = Main.rand.NextFloat(0.6f, 1f) * (1 + Progress * 0.4f);
                bubble.type = Main.rand.NextBool(3) ? 412 : 411;
            }

            //Water trail
            for (int i = 0; i < 6; i++)
            {
                Dust waterDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 33, 0f, 0f, 100, default, 0.9f);
                waterDust.noGravity = true;
                waterDust.velocity *= 0.5f;
                waterDust.velocity -= Projectile.velocity * 0.1f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item21, Projectile.position);
            int bubbleCount = 25 + Main.rand.Next(15);

            for (int i = 0; i <= bubbleCount; i++)
            {
                Gore bubble = Gore.NewGorePerfect(Projectile.GetSource_Death(), Projectile.position, Projectile.velocity * 0.3f + Main.rand.NextVector2Circular(4f, 4f), 411);
                bubble.timeLeft = 8 + Main.rand.Next(6);
                bubble.scale = Main.rand.NextFloat(0.6f, 1f) * (1 + Progress * 0.4f);
                bubble.type = Main.rand.NextBool(3) ? 412 : 411;
            }
        }
    }
}
