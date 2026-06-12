using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static CalamityMod.Projectiles.Ranged.TauCannonHoldout;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Projectiles.Ranged
{
    public class TauCannonBeam : BaseLaserbeamProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";

        public override float Lifetime => IsStage3Laser ? 90 : 25;

        public override float MaxScale => IsStage3Laser ? 4f : 1f;

        public override float MaxLaserLength => 2000f;

        private const string LaserTexturePath = "CalamityMod/ExtraTextures/Lasers/TauCannonBeam";
        public override Texture2D LaserBeginTexture => Request<Texture2D>(LaserTexturePath + "Start").Value;
        public override Texture2D LaserMiddleTexture => Request<Texture2D>(LaserTexturePath + "Middle").Value;
        public override Texture2D LaserEndTexture => Request<Texture2D>(LaserTexturePath + "End").Value;
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private Projectile Holdout => Main.projectile[(int)Projectile.ai[1]];

        private bool IsStage3Laser => Projectile.ai[2] == 1f;
        public Color color1 = Color.MediumTurquoise;
        public Color color2 = Color.Coral;

        private Player Owner { get; set; }

        private SlotId BigBeamSoundSlot;
        public int time = 0;

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ContinuouslyUpdateDamageStats = true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 90);

            if (Projectile.numHits > 1)
                Projectile.damage = (int)(Projectile.damage * (IsStage3Laser ? 1 : 0.75f));
            if (Projectile.damage < 1)
                Projectile.damage = 1;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * LaserLength, IsStage3Laser ? 70f : 10f, ref _);
        }

        public override void UpdateLaserMotion()
        {
            Projectile.velocity = Holdout.velocity;
            Projectile.rotation = Holdout.velocity.ToRotation() - MathHelper.PiOver2;
        }

        public override void AttachToSomething()
        {
            if (Owner == null || !Owner.active || Owner.dead || Owner.CCed || Owner.noItems || Owner.ownedProjectileCounts[ProjectileType<TauCannonHoldout>()] == 0 || Holdout.ModProjectile<TauCannonHoldout>() == null) 
                return;
            Projectile.Center = Holdout.ModProjectile<TauCannonHoldout>().GunTipPosition + Holdout.velocity * (IsStage3Laser ? 20f : 5f);
        }

        public override void ExtraBehavior()
        {
            Owner ??= Main.player[Projectile.owner];
            Projectile.localNPCHitCooldown = IsStage3Laser ? 4 : -1;

            float Lifet = Lifetime - time;
            if (!SoundEngine.TryGetActiveSound(BigBeamSoundSlot, out var sound))
                BigBeamSoundSlot = SoundEngine.PlaySound(BigBeamSound with { Volume = 0.01f, IsLooped = true }, Projectile.Center);
            else
            {
                sound.Position = Projectile.Center;
                sound.Volume = Utils.GetLerpValue(0, 20, Lifet, true) * 100;
                sound.Pitch = (1 - 1 * Utils.GetLerpValue(0, 20, Lifet, true)) * -1;
            }

            Vector2 effectsPosition = Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.velocity * LaserLength, Main.rand.NextFloat());
            Vector2 randomLineEffectPosition = effectsPosition + Main.rand.NextVector2Circular(40f * (IsStage3Laser ? 1.2f : 0.8f), 40f * (IsStage3Laser ? 1.2f : 0.8f));

            Dust laserDust = Dust.NewDustPerfect(randomLineEffectPosition, DustID.FireworksRGB, Projectile.velocity * Main.rand.NextFloat(5f, 40f), Scale: Main.rand.NextFloat(0.8f, 1.1f));
            laserDust.noGravity = true;
            laserDust.color = Main.rand.NextBool(3) ? color2 : color1;

            for (int i = 0; i < 2; i++)
            {
                randomLineEffectPosition = effectsPosition + Main.rand.NextVector2Circular(50f * (IsStage3Laser ? 1.2f : 0.8f), 50f * (IsStage3Laser ? 1.2f : 0.8f));
                Particle spark2 = new LineParticle(randomLineEffectPosition, Projectile.velocity * Main.rand.NextFloat(20f, 90f), false, 18, 0.8f, Main.rand.NextBool(3) ? color2 : color1);
                GeneralParticleHandler.SpawnParticle(spark2);
            }

            time++;
        }

        public override void OnKill(int timeLeft)
        {
            if (SoundEngine.TryGetActiveSound(BigBeamSoundSlot, out var ChargeSound))
                ChargeSound?.Stop();
        }
    }
}
