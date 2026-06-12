using System;
using CalamityMod.Dusts;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class FireImplosion : ModProjectile, ILocalizedModType // Used by both Flare bolt and Frigidflash Bolt
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public Vector2 pushVelocity;
        public float customKnockback = 0;
        public int time = 0;
        public int boomTime = 0;
        public bool frigidFlash => Projectile.ai[2] == 5;
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 55;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
            Projectile.ArmorPenetration = 20;
        }
        public override void AI()
        {
            if (customKnockback == 0) // On spawn effects
            {
                Projectile.scale = 0;
                Projectile.alpha = 0;
                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/OpalChargedFire") with { Volume = 0.35f, Pitch = 0.7f }, Projectile.Center);
                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/FireImplosion") with { Volume = 0.45f, Pitch = Main.rand.NextFloat(0.2f, 0.35f) }, Projectile.Center);
                customKnockback = Math.Abs(Projectile.knockBack);
                Projectile.knockBack = 0;

                for (int i = 0; i < 3; i++)
                {
                    Particle orb1 = new CustomPulse(Projectile.Center, Vector2.Zero, Color.OrangeRed, "CalamityMod/Particles/BloomRing", new Vector2(1, 1), Main.rand.NextFloat(-10, 10), 0, 1.1f + i * 0.1f, 18);
                    GeneralParticleHandler.SpawnParticle(orb1);
                }
                float rot = Main.rand.NextFloat(-2, 2);
                for (int i = 0; i < 8; i++)
                {
                    Vector2 velocity = (MathHelper.TwoPi * i / 8f).ToRotationVector2().RotatedBy(rot) * 13f;
                    Particle trail = new CustomSpark(Projectile.Center + velocity * 3, velocity * 0.5f, "CalamityMod/Particles/FireTypeParticle", false, 35, 1.5f, Color.OrangeRed, new Vector2(1, 1.3f), true, false, shrinkSpeed: 0.15f);
                    GeneralParticleHandler.SpawnParticle(trail);
                }
            }
            if (boomTime >= 15)
            {
                Projectile.scale = 0.95f * Utils.GetLerpValue(42, 0, time, true) * Utils.GetLerpValue(40, 25, time, true);
                time++;
            }
            else // When the vortex is expanding out before it starts shrinking
            {
                Projectile.scale = 0.95f * Utils.GetLerpValue(0, 15, boomTime, true);
                boomTime++;
            }
            if (Projectile.timeLeft <= 20) // Fizzles out at the end
            {
                if ((frigidFlash && Projectile.timeLeft % 3 == 0) || !frigidFlash)
                {
                    Particle fx = new CustomSpark(Projectile.Center, Vector2.One.RotatedByRandom(100) * Main.rand.NextFloat(0.3f, 6), "CalamityMod/Particles/FireTypeParticle", false, 22, Main.rand.NextFloat(0.7f, 1.2f), Color.OrangeRed, new Vector2(0.8f, 1f), true, false);
                    GeneralParticleHandler.SpawnParticle(fx);
                }
                if (frigidFlash)
                {
                    Particle fx2 = new CustomSpark(Projectile.Center, Vector2.One.RotatedByRandom(100) * Main.rand.NextFloat(0.3f, 6), "CalamityMod/Particles/IceTypeParticle", false, 22, Main.rand.NextFloat(0.7f, 1.2f), Color.DeepSkyBlue, new Vector2(0.8f, 1f), true, false, shrinkSpeed: -0.1f);
                    GeneralParticleHandler.SpawnParticle(fx2);
                }
            }
            else if (boomTime > 5)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 vel = Vector2.One.RotatedByRandom(100) * Main.rand.NextFloat(7f, 14) * Projectile.scale;
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + vel * 3, ModContent.DustType<LightDust>(), vel);
                    dust.noGravity = true;
                    dust.scale = Main.rand.NextFloat(0.75f, 1.1f);
                    dust.color = Color.OrangeRed;
                    dust.noLightEmittence = true;
                }
            }
            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3() * 2 * Projectile.scale);
            Projectile.rotation += 0.4f;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Player Owner = Main.player[Projectile.owner];
            if (frigidFlash)
            {
                target.AddBuff(BuffID.OnFire3, 30);
                target.AddBuff(BuffID.Frostburn2, 30);
            }
            else
                target.AddBuff(BuffID.OnFire, 180);
            pushVelocity = Utils.DirectionTo(Projectile.Center, target.Center) * customKnockback;
            float minMult = 0.4f;
            int hitsToMinMult = 15;
            float damageMult = Utils.Remap(Projectile.numHits, 0, hitsToMinMult, 1, minMult, true);
            modifiers.SourceDamage *= damageMult * (frigidFlash ? 0.5f : 1); // Frigidflash explosion are spawned at full power, so their damage is reduced here

            // The vortex sucks in enemies
            target.MoveNPC(-pushVelocity, customKnockback, frigidFlash, Owner);
        }
        public override void OnKill(int timeLeft)
        {
            if (frigidFlash)
            {
                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/ProfanedGuardians/GuardianDash") with { Volume = 1, Pitch = -0.2f }, Projectile.Center);
                SoundEngine.PlaySound(SoundID.DeerclopsIceAttack with { Volume = 0.85f, Pitch = 0.1f }, Projectile.Center);

                // Create Blast
                float blastSize = 160;
                float minMultiplier = 0.25f;
                int hitsToMinMult = 8;
                int debuff1 = BuffID.Frostburn2;
                int debuff2 = BuffID.OnFire3;
                int debuffTime = 230;
                Projectile blast = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BasicBurst>(), (int)(Projectile.damage * 4f), customKnockback * 2, Projectile.owner, blastSize, minMultiplier, hitsToMinMult);
                blast.localAI[0] = debuff1;
                blast.localAI[2] = debuff2;
                blast.localAI[1] = debuffTime;
                blast.DamageType = DamageClass.Magic;

                // BIGGER "Snowflake" visual effect
                float rot = Main.rand.NextFloat(-2, 2);
                for (int i = 0; i < 8; i++)
                {
                    Vector2 velocity = (MathHelper.TwoPi * i / 8f).ToRotationVector2().RotatedBy(rot) * 14f;
                    Particle trail = new CustomSpark(Projectile.Center + velocity * 3, velocity * 0.5f, "CalamityMod/Particles/IceTypeParticle", false, 32, 1.3f, Color.Lerp(Color.DeepSkyBlue, Color.White, 0.5f), new Vector2(1.2f, 1.8f), true, false, shrinkSpeed: -0.27f);
                    GeneralParticleHandler.SpawnParticle(trail);

                    Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LightDust>(), velocity.RotatedBy(MathHelper.ToRadians(22.5f)) * 1.2f);
                    dust.noGravity = true;
                    dust.scale = 1.75f;
                    dust.color = Color.OrangeRed;
                    dust.noLightEmittence = true;

                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LightDust>(), velocity * 1.5f);
                    dust2.noGravity = true;
                    dust2.scale = 1.4f;
                    dust2.color = Color.DeepSkyBlue;
                    dust2.noLightEmittence = true;
                }
                for (int i = 0; i < 5; i++)
                {
                    Particle orb = new CustomPulse(Projectile.Center, Vector2.Zero, Color.OrangeRed, "CalamityMod/Particles/BloomRing", new Vector2(1, 1), Main.rand.NextFloat(-10, 10), 0.3f, 1.1f, 25);
                    GeneralParticleHandler.SpawnParticle(orb);
                    Particle orb1 = new CustomPulse(Projectile.Center, Vector2.Zero, Color.Lerp(Color.DeepSkyBlue, Color.White, i * 0.15f), "CalamityMod/Particles/BloomCircle", new Vector2(1, 1), Main.rand.NextFloat(-10, 10), 0.6f, 0.8f - i * 0.07f, 20);
                    GeneralParticleHandler.SpawnParticle(orb1);
                }
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CalamityUtils.CircularHitboxCollision(Projectile.Center, (110 * Projectile.scale) + 15, targetHitbox);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (boomTime < 1)
                return false;
            float fade = Utils.GetLerpValue(0, 20, Projectile.timeLeft, true);
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomRing").Value;
            Color useColor = frigidFlash ? Color.Lerp(Color.DeepSkyBlue, Color.OrangeRed, fade) : Color.OrangeRed;

            for (int i = 0; i < 15; i++)
            {
                Color auraColor = useColor with { A = 0 } * 0.4f;
                Vector2 drawOffset = (MathHelper.TwoPi * i / 15f).ToRotationVector2().RotatedBy(Main.GlobalTimeWrappedHourly * 28 - fade * 10) * 7 * fade;
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + drawOffset, null, auraColor, Projectile.rotation + i * 2.5f, texture.Size() * 0.5f, new Vector2(1 * fade, 1 + (1 - fade)) * MathHelper.Clamp((Projectile.scale - (i * 0.02f)), 0, 10), SpriteEffects.None);
            }

            return false;
        }
    }
}
