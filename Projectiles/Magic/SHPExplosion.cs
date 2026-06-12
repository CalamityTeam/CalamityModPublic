using System;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class SHPExplosion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public const int Lifetime = 20;
        public ref float Timer => ref Projectile.ai[2];

        public override void SetDefaults()
        {
            Projectile.width = 500;
            Projectile.height = 500;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = Lifetime;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            float lights = Main.rand.NextFloat(0.9f, 1.1f);
            lights *= Main.essScale;
            Lighting.AddLight(Projectile.Center, 5f * lights, 1f * lights, 4f * lights);
            bool lightSoul = SHPB.GetSoulEffects((int)Projectile.ai[0]) == SHPB.SoulType.Light;
            bool nightSoul = SHPB.GetSoulEffects((int)Projectile.ai[0]) == SHPB.SoulType.Night;

            // Increase explosion size with time
            int explosionSize = 200 + (int)MathHelper.Clamp(Timer, 0, Lifetime) * (int)Math.Ceiling(15 * (lightSoul ? SHPC.LightExplosionSizeMult : 1f));
            Projectile.ExpandHitboxBy(explosionSize);

            if (Projectile.ai[1] == 0f)
            {
                Color particleColor = SHPB.FindColorForSoul((int)Projectile.ai[0]);
                // Light makes the explosion larger
                float sizeScalar = lightSoul ? SHPC.LightExplosionSizeMult : 1f;

                CustomPulse inner = new(Projectile.Center, Vector2.Zero, particleColor, "CalamityMod/Particles/BloomCircle", Vector2.One, 0f, 0.4f, 1.5f * sizeScalar, Lifetime);
                CustomPulse outer = new(Projectile.Center, Vector2.Zero, particleColor, "CalamityMod/Particles/BloomRing", Vector2.One, 0f, 0.4f, 2.5f * sizeScalar, Lifetime);
                CustomPulse explode = new(Projectile.Center, Vector2.Zero, particleColor, "CalamityMod/Particles/PlasmaExplosion", Vector2.One, Main.rand.NextFloat(MathHelper.TwoPi), 0.026f, 0.26f * sizeScalar, Lifetime);

                GeneralParticleHandler.SpawnParticle(inner);
                GeneralParticleHandler.SpawnParticle(outer);
                GeneralParticleHandler.SpawnParticle(explode);

                // Night makes the explosion last longer
                if (nightSoul)
                {
                    Projectile.timeLeft = (int)(Lifetime * SHPC.NightExplosionTimeMult);

                    CustomPulse nightInner = new(Projectile.Center, Vector2.Zero, particleColor, "CalamityMod/Particles/BloomCircle", Vector2.One, 0f, 0.4f, 1.5f, (int)(Lifetime * SHPC.NightExplosionTimeMult));
                    CustomPulse nightOuter = new(Projectile.Center, Vector2.Zero, particleColor, "CalamityMod/Particles/BloomRing", Vector2.One, 0f, 0.4f, 2.5f, (int)(Lifetime * SHPC.NightExplosionTimeMult));
                    CustomPulse nightExplode = new(Projectile.Center, Vector2.Zero, particleColor, "CalamityMod/Particles/PlasmaExplosion", Vector2.One, Main.rand.NextFloat(MathHelper.TwoPi), 0.026f, 0.26f, (int)(Lifetime * SHPC.NightExplosionTimeMult));

                    GeneralParticleHandler.SpawnParticle(nightInner);
                    GeneralParticleHandler.SpawnParticle(nightOuter);
                    GeneralParticleHandler.SpawnParticle(nightExplode);
                }
                Projectile.ai[1] = 1f;
            }

            Timer++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Might launches enemies
            if (SHPB.GetSoulEffects((int)Projectile.ai[0]) == SHPB.SoulType.Might && target.CanBeMoved())
            {
                // 14NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
                Player owner = Main.player[Projectile.owner];
                Vector2 launchVel = Utils.DirectionTo(owner.Center, owner.Calamity().mouseWorld) - Vector2.UnitY * 5f;

                target.MoveNPC(launchVel, SHPC.MightKnockbackStrength, false, owner);
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // Fright deals extra flat damage
            if (SHPB.GetSoulEffects((int)Projectile.ai[0]) == SHPB.SoulType.Fright)
                modifiers.SourceDamage.Flat += SHPC.FrightFlatDamage;
        }
    }
}
