using System;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class VoidBlast : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/Melee/GalaxiaBolt";
        public int time = 0;

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.extraUpdates = 2;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi) + MathHelper.ToRadians(90) * Projectile.direction;
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.1f / 255f, (255 - Projectile.alpha) * 0.7f / 255f, (255 - Projectile.alpha) * 0.15f / 255f);

            if (Projectile.timeLeft <= 597f)
            {
                Particle smoke = new HeavySmokeParticle(Projectile.Center, -Projectile.velocity * 0.5f, Color.Lerp(Color.MidnightBlue, Color.Indigo, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6f)), 20, Main.rand.NextFloat(0.4f, 0.7f) * Projectile.scale, 0.8f, 0, false, 0, true);
                GeneralParticleHandler.SpawnParticle(smoke);
                if (Main.rand.NextBool(2))
                {
                    Particle smokeGlow = new HeavySmokeParticle(Projectile.Center, -Projectile.velocity * 0.5f, Color.DarkOrchid, 20, Main.rand.NextFloat(0.4f, 0.8f) * Projectile.scale, 0.3f, 0, true, 0.005f, true);
                    GeneralParticleHandler.SpawnParticle(smokeGlow);
                }
                Particle spark2 = new CustomSpark(Projectile.Center, -Projectile.velocity * 0.05f, "CalamityMod/Particles/GlowSpark2", false, 5, 0.052f, Color.Black, new Vector2(0.6f, 1.3f), false);
                GeneralParticleHandler.SpawnParticle(spark2);
                SparkParticle spark = new SparkParticle(Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.UnitY) * 3.5f, Projectile.velocity * 0.01f, false, 5, 1.2f * Projectile.scale, Main.rand.NextBool() ? Color.Indigo : Color.BlueViolet);
                GeneralParticleHandler.SpawnParticle(spark);
                spark.DrawLayer = Enums.GeneralDrawLayer.AfterEverything;
            }
            time++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<WhisperingDeath>(), 180);
            if (Projectile.numHits == 0)
            {
                Player Owner = Main.player[Projectile.owner];
                Owner.Calamity().sharkGunDamageScaling++;
            }
        }

        public override void OnKill(int timeLeft)
        {
            float blastSize = 170;
            float minMultiplier = 0.25f;
            int hitsToMinMult = -1;
            int debuff1 = ModContent.BuffType<WhisperingDeath>();
            int debuffTime = 180;
            Projectile blast = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BasicBurst>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack, Projectile.owner, blastSize, minMultiplier, hitsToMinMult);
            blast.localAI[0] = debuff1;
            blast.localAI[1] = debuffTime;
            blast.timeLeft = 2;
            blast.DamageType = Projectile.DamageType;
            #region Visuals and Sounds
            SoundStyle fire = new("CalamityMod/Sounds/Item/OmicronBeam");
            SoundEngine.PlaySound(fire with { Volume = 0.9f }, Projectile.Center);

            for (int i = 0; i < 3; i++)
            {
                Particle blastRing = new CustomPulse(Projectile.Center, Vector2.Zero, Color.Black, "CalamityMod/Particles/SmallBloom", Vector2.One, Main.rand.NextFloat(-10, 10), 1f, 1.2f, 15, false);
                GeneralParticleHandler.SpawnParticle(blastRing);
            }

            for (float k = 0; k < 3; k++)
            {
                float colorRando = Main.rand.NextFloat(0, 1);
                int partLifetime = Main.rand.Next(13, 15 + 1);
                float scale = Main.rand.NextFloat(0.10f, 0.16f);
                Vector2 spawnPos = Projectile.Center + (Main.rand.NextVector2Circular(20, 20) * (k + 1));
                Particle blastRing = new CustomPulse(spawnPos, Vector2.Zero, Color.Lerp(Color.DarkOrchid, Color.Indigo, colorRando) * 0.6f, "CalamityMod/Particles/FlameExplosion", Vector2.One, Main.rand.NextFloat(-10, 10), 0.07f, scale, partLifetime);
                GeneralParticleHandler.SpawnParticle(blastRing);
                blastRing.DrawLayer = Enums.GeneralDrawLayer.AfterEverything;
            }

            Particle innerGlow = new CustomPulse(Projectile.Center, Vector2.Zero, Color.Indigo, "CalamityMod/Particles/BloomCircle", Vector2.One, 0f, 0.03f, 1f, 24, true);
            GeneralParticleHandler.SpawnParticle(innerGlow);
            innerGlow.DrawLayer = Enums.GeneralDrawLayer.AfterEverything;

            float offset = Main.rand.NextFloat(MathHelper.TwoPi);
            for (int i = 0; i < 4; i++)
            {
                Vector2 velocity = (MathHelper.TwoPi * i / 4f + offset).ToRotationVector2();
                Particle cross = new GlowSparkParticle(Projectile.Center, velocity, false, 12, 0.4f, Color.BlueViolet * 0.7f, new Vector2(0.07f, 0.08f), true, false);
                GeneralParticleHandler.SpawnParticle(cross);
                cross.DrawLayer = Enums.GeneralDrawLayer.AfterEverything;
            }
            for (float k = 0; k < 10; k++)
            {
                Particle spark = new CustomSpark(Projectile.Center, new Vector2(3, 3).RotatedByRandom(100) * Main.rand.NextFloat(2f, 4f), "CalamityMod/Particles/ProvidenceMarkParticle", false, 27, Main.rand.NextFloat(1.1f, 1.3f), Color.Lerp(Color.MediumPurple, Color.Indigo, Main.rand.NextFloat(0.5f, 0.7f)), new Vector2(0.6f, 0.5f), true, false, 0, false, false, Main.rand.NextFloat(0.35f, 0.4f));
                GeneralParticleHandler.SpawnParticle(spark);
                spark.DrawLayer = Enums.GeneralDrawLayer.AfterEverything;
            }

            #endregion
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Particles/GlowSpark").Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Color drawColor = Projectile.GetAlpha(lightColor);
            float drawRotation = Projectile.rotation;
            Vector2 rotationPoint = texture.Size() * 0.5f;

            Main.EntitySpriteDraw(texture, drawPosition, null, Color.Indigo with { A = 0 }, drawRotation, rotationPoint, new Vector2(0.5f, 1.4f) * 0.025f * Projectile.scale, SpriteEffects.None);
            return false;
        }
    }
}
