using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class ShortCircuitShot : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public int time = 0;
        public Vector2 position;
        public Vector2 oldPos;
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.timeLeft = 180;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 2;
            Projectile.extraUpdates = 5;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ArmorPenetration = 10;
        }
        public override void AI()
        {
            if (time == 0)
            {
                for (int i = 0; i < 7; i++)
                {
                    bool is278 = Main.rand.NextBool(4);
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX) * 30, is278 ? Effects.ArsenalEffects.ArsenalDust : Effects.ArsenalEffects.ArsenalElectricDust);
                    dust.scale = is278 ? 0.7f : Main.rand.NextBool(7) ? 1.2f : 0.8f;
                    dust.noGravity = true;
                    if (!is278)
                        dust.fadeIn = 2;
                    dust.color = Effects.ArsenalEffects.ArsenalElectricColor;
                    dust.velocity = Projectile.velocity.RotatedByRandom(0.7f) * Main.rand.NextFloat(0.6f, 2.3f) * (is278 ? 0.3f : 1);
                }
            }
            if (time > 4)
            {
                if (Projectile.timeLeft % 2 == 0)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, Effects.ArsenalEffects.ArsenalElectricDust);
                    dust.scale = Main.rand.NextBool(7) ? 1f : 0.55f;
                    dust.noGravity = true;
                    dust.fadeIn = 2;
                    dust.color = Effects.ArsenalEffects.ArsenalElectricColor;
                    dust.velocity = Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.2f, 1.5f);
                }

                if (Projectile.timeLeft % 2 == 0)
                {
                    SparkParticle tip = new SparkParticle(Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX) * 5, Projectile.velocity, false, (Projectile.timeLeft < 50 ? 20 : 6), 0.6f, Effects.ArsenalEffects.ArsenalElectricColor);
                    GeneralParticleHandler.SpawnParticle(tip);
                }
                if (Projectile.timeLeft % 3 == 0)
                {
                    float squash = Utils.GetLerpValue(1, 3, Projectile.velocity.Length(), true);
                    Particle trail = new BoltParticle(Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 10, Projectile.velocity * 0.01f, false, 7, 0.2f, Effects.ArsenalEffects.ArsenalElectricColor * 0.8f * squash, new Vector2(1 - 0.15f * squash, 1f), true, false, shrinkSpeed: 0.5f * squash);
                    GeneralParticleHandler.SpawnParticle(trail);
                }
            }

            Projectile.velocity *= 0.987f;
            time++;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player Owner = Main.player[Projectile.owner];
            target.AddBuff(ModContent.BuffType<StaticDischarge>(), 40);

            Vector2 launchVel = Utils.DirectionTo(Owner.Center, target.Center);
            target.MoveNPC(launchVel, 8, false, Owner);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.numHits > 0)
                Projectile.damage = (int)(Projectile.damage * 0.7f);
            if (Projectile.damage < 1)
                Projectile.damage = 1;
        }
    }
}
