using System;
using CalamityMod.Dusts;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class NidhoggRailgunBigShot : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public int time = 0;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 900;
            Projectile.extraUpdates = 80;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];
            float targetDist = Vector2.Distance(Owner.Center, Projectile.Center);

            if (targetDist < 1400 && time > 7)
            {
                if (time % 2 == 0)
                {
                    Particle trail = new CustomSpark(Projectile.Center, Projectile.velocity * 0.2f, "CalamityMod/Particles/BloomCircle", false, 48, 0.375f, Effects.ArsenalEffects.ArsenalGaussColor, new Vector2(1f, 2f), true, true, glowCenterScale: 0.7f, shrinkSpeed: 0.05f);
                    GeneralParticleHandler.SpawnParticle(trail);
                }
                if (time % 3 == 0)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(4, 4), ModContent.DustType<SquashDust>());
                    dust.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(0.1f) * Main.rand.NextFloat(10f, 30f);
                    dust.scale = Main.rand.NextFloat(1.3f, 1.7f);
                    dust.noGravity = true;
                    dust.color = Effects.ArsenalEffects.ArsenalGaussColor;
                    dust.noLightEmittence = time % 6 != 0;
                    dust.fadeIn = -0.6f;
                }

                if (time % 12 == 0)
                {
                    Particle trail2 = new VelChangingSpark(Projectile.Center, -Projectile.velocity * 0.1f, -Projectile.velocity * 0.1f, "CalamityMod/Particles/GlowSquareFading", 35, 0.33f, Effects.ArsenalEffects.ArsenalGaussColor, new Vector2(1f, 0.8f), shrinkSpeed: 0f, lerpRate: 0.1f);
                    GeneralParticleHandler.SpawnParticle(trail2);
                }
            }

            for (int x = 0; x < Main.maxProjectiles; x++)
            {
                Projectile projectile = Main.projectile[x];
                if (Vector2.Distance(Projectile.Center, projectile.Center) <= 20 && projectile.active && projectile.type == ModContent.ProjectileType<RicoshotCoin>())
                {
                    projectile.Kill();
                    Projectile.velocity = Projectile.Center.DirectionTo(Owner.Center) * 6;
                    Projectile.hostile = true;
                    Projectile.friendly = false;
                }
            }

            time++;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            impactDust();

            bool onKill = (target.life <= 0 && target.realLife == -1);
            if (onKill && Projectile.numHits > 0)
                Projectile.numHits--;

            modifiers.ApplyScalingForcedCrit(Projectile);

            Player Owner = Main.player[Projectile.owner];
            float minMult = 0.25f;
            int hitsToMinMult = 7;
            float damageMult = Utils.Remap(Projectile.numHits, 0, hitsToMinMult, 1, minMult, true);
            modifiers.SourceDamage *= damageMult;

            Vector2 launchVel = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            float launchPower = 20;
            target.MoveNPC(launchVel, launchPower, true, Owner);
        }

        public override void OnKill(int timeLeft)
        {
            impactDust();
        }
        public void impactDust()
        {
            for (int i = 0; i < MathHelper.Clamp(12 - Projectile.numHits * 2, 1, 12); i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(3) ? ModContent.DustType<SquashDust>() : Effects.ArsenalEffects.ArsenalGaussDust);
                dust.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(0.3f) * Main.rand.NextFloat(14f, 37f);
                dust.scale = Main.rand.NextFloat(0.8f, 1.5f);
                dust.noGravity = true;
                dust.color = Effects.ArsenalEffects.ArsenalGaussColor;
                dust.noLightEmittence = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
