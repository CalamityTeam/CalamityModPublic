using System;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class LeviAmberDash : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public Player Owner => Main.player[Projectile.owner];
        private static float ExplosionRadius = 85f;
        public static readonly SoundStyle Slap = new("CalamityMod/Sounds/Custom/WetSlap", 4) { Volume = 0.8f, PitchVariance = 0.3f};

        public override void SetDefaults()
        {
            //These shouldn't matter because its circular
            Projectile.width = 85;
            Projectile.height = 85;
            Projectile.friendly = true;
            Projectile.DamageType = AverageDamageClass.Instance;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 4;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!target.boss && target.IsAnEnemy(true, true))
            {
                target.velocity.Y += -1.8f;
                target.velocity.X += 4.2f * Owner.direction;
            }
            target.AddBuff(BuffID.Wet, 300);
            target.AddBuff(ModContent.BuffType<Buffs.DamageOverTime.RiptideDebuff>(), 180);
            SoundEngine.PlaySound(SoundID.Item85 with { Volume = 0.4f, PitchVariance = 0.4f }, Projectile.Center);
            for (int i = 0; i < 3; ++i)
            {
                int bloodLifetime = Main.rand.Next(20, 26);
                float bloodScale = Main.rand.NextFloat(0.6f, 0.8f);
                Color bloodColor = Color.Lerp(Color.DodgerBlue, Color.DarkTurquoise, Main.rand.NextFloat());

                if (Main.rand.NextBool(20))
                    bloodScale *= 2f;

                float randomSpeedMultiplier = Main.rand.NextFloat(1.25f, 2.25f);
                Vector2 bloodVelocity = Main.rand.NextVector2Unit() * 5 * randomSpeedMultiplier;
                bloodVelocity.Y -= 5f;
                BloodParticle blood = new BloodParticle(Projectile.Center, bloodVelocity, bloodLifetime, bloodScale, bloodColor);
                GeneralParticleHandler.SpawnParticle(blood);
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, ExplosionRadius, targetHitbox);
        public override bool? CanDamage() => base.CanDamage();
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = Math.Sign(Owner.direction);
        }

        public override bool? CanCutTiles() => false;
    }
}
