using CalamityMod.Dusts;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class NidhoggRailgunBlast : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public int time = 0;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 80;
            Projectile.penetrate = 1;
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
                if (time % 3 == 0)
                {
                    Particle trail = new CustomSpark(Projectile.Center, Projectile.velocity * 0.2f, "CalamityMod/Particles/BloomCircle", false, 8, 0.15f, Effects.ArsenalEffects.ArsenalGaussColor, new Vector2(1f, 2.5f), true, true, glowCenterScale: 0.7f, shrinkSpeed: 0.4f);
                    GeneralParticleHandler.SpawnParticle(trail);
                }
                if (time % 3 == 0)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(4, 4), Effects.ArsenalEffects.ArsenalGaussDust);
                    dust.velocity = Vector2.One.RotatedByRandom(MathHelper.Pi) * Main.rand.NextFloat(0.1f, 0.2f);
                    dust.scale = Main.rand.NextFloat(0.7f, 0.85f);
                    dust.noGravity = true;
                    dust.color = Effects.ArsenalEffects.ArsenalGaussColor;
                    dust.noLightEmittence = time % 9 != 0;
                    dust.fadeIn = 0.3f;
                }
            }

            time++;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Player Owner = Main.player[Projectile.owner];
            Particle spark = new GlowSparkParticle(Projectile.Center, Projectile.velocity, false, 8, MathHelper.Clamp(0.06f - Projectile.numHits * 0.01f, 0f, 0.06f), Effects.ArsenalEffects.ArsenalGaussColor * 0.9f, new Vector2(1f, 0.5f), true, true, 0.9f);
            GeneralParticleHandler.SpawnParticle(spark);

            if (Projectile.numHits == 0)
            {
                Particle bolt2 = new CustomSpark(Projectile.Center, Vector2.Zero, "CalamityMod/Particles/GlowSquareParticleThick", false, 8, 1f, Effects.ArsenalEffects.ArsenalGaussColor * 0.6f, Vector2.One, true, true, glowOpacity: 0.5f, glowCenterScale: 1.15f, extraRotation: MathHelper.PiOver4);
                GeneralParticleHandler.SpawnParticle(bolt2);

                foreach (Projectile p in Main.ActiveProjectiles)
                {
                    if (p.type == ModContent.ProjectileType<NidhoggExplosion>() && p.owner == Projectile.owner)
                    {
                        if (p.timeLeft > 10)
                            p.timeLeft = 10;
                    }
                }
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<NidhoggExplosion>(), (int)(Projectile.damage * 0.5f), 0, Projectile.owner, 0f, 0f);
            }
            impactDust();

            Vector2 launchVel = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            float launchPower = 6;
            target.MoveNPC(launchVel, launchPower, true, Owner);
        }

        public override void OnKill(int timeLeft)
        {
            impactDust();
        }
        public void impactDust()
        {
            for (int i = 0; i < MathHelper.Clamp(5 - Projectile.numHits, 1, 5); i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SquashDust>());
                dust.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(0.3f) * Main.rand.NextFloat(8f, 13f);
                dust.scale = Main.rand.NextFloat(0.8f, 1.35f);
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
