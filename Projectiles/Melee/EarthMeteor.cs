using CalamityMod.Buffs.DamageOverTime;
using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Utilities;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.Particles;
using ReLogic.Content;

namespace CalamityMod.Projectiles.Melee
{
    public class EarthMeteor : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public ref float time => ref Projectile.ai[0];
        public SlotId AudSlot;
        public Color mainColor = Color.White;
        public Color randomColor = Color.White;
        public Color variedColor = Color.White;
        public int colorTimer = 0;
        public int fallTime = 180;

        public override void SetStaticDefaults() => ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 84;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];
            float targetDist = Vector2.Distance(Owner.Center, Projectile.Center);
            NPC targeted = Projectile.ai[1] == 0 ? Owner.ClampedMouseWorld().ClosestNPCAt(1000) : Main.npc[(int)Projectile.ai[1]];

            randomColor = Main.rand.Next(3) switch
            {
                0 => Color.OrangeRed,
                1 => Color.MediumTurquoise,
                _ => Color.LawnGreen,
            };
            if (time == 0)
            {
                mainColor = randomColor;
            }

            if (time % 20 == 0)
            {
                variedColor = colorTimer switch
                {
                    0 => Color.OrangeRed,
                    1 => Color.MediumTurquoise,
                    _ => Color.LawnGreen,
                };
                colorTimer++;
                if (colorTimer >= 3)
                    colorTimer = 0;
            }
            mainColor = Color.Lerp(mainColor, variedColor, 0.07f);

            if (time == 0 && Projectile.ai[2] == 2)
            {
                SoundStyle fire2 = new("CalamityMod/Sounds/Item/WeldingShoot");
                AudSlot = SoundEngine.PlaySound(fire2 with { Volume = 0.01f, Pitch = 0.01f, IsLooped = true }, Projectile.Center);
            }
            if (SoundEngine.TryGetActiveSound(AudSlot, out var ChargeSound) && ChargeSound.IsPlaying && Projectile.ai[2] == 2)
            {
                ChargeSound.Position = Projectile.Center;
                ChargeSound.Pitch = Utils.Remap(time, 0, fallTime, 0.4f, -0.8f) * 100;
                ChargeSound.Volume = Utils.Remap(time, fallTime * 0.2f, fallTime, 0f, 0.9f) * 100;
            }
            if (time == (int)(fallTime * 0.2f) && Projectile.ai[2] > 0)
            {
                Vector2 spawnSpot;
                if (targeted == null || !targeted.active || targeted.life <= 0)
                    spawnSpot = Owner.Center + new Vector2(Main.rand.NextFloat(-450, 450), Main.rand.NextFloat(-450, -650));
                else
                    spawnSpot = targeted.Center + new Vector2(Main.rand.NextFloat(-450, 450), Main.rand.NextFloat(-450, -650));

                Projectile meteor = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), spawnSpot, Vector2.Zero, ModContent.ProjectileType<EarthMeteor>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, Projectile.ai[1], Projectile.ai[2] - 1);
                meteor.scale = Projectile.scale;
            }
            if (time == fallTime)
            {
                for (int i = 0; i < 2; i++)
                {
                    Particle bloom = new CustomPulse(Projectile.Center, Vector2.Zero, mainColor, "CalamityMod/Particles/LargeBloom", new Vector2(1, 1), 0, 0.8f, 0f, 27);
                    GeneralParticleHandler.SpawnParticle(bloom);
                }
                Particle bloom3 = new CustomPulse(Projectile.Center, Vector2.Zero, Color.White, "CalamityMod/Particles/LargeBloom", new Vector2(1, 1), 0, 0.65f, 0f, 27);
                GeneralParticleHandler.SpawnParticle(bloom3);

                Projectile.extraUpdates = 30;

                Vector2 mouse = Owner.ClampedMouseWorld();
                NPC target = mouse.ClosestNPCAt(1000);
                if (target != null)
                    Projectile.velocity = (target.Center - Projectile.Center + target.velocity * 8).SafeNormalize(Vector2.UnitX) * 8;
                else
                {
                    // 14NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
                    Projectile.velocity = (Owner.Calamity().mouseWorld - Projectile.Center).SafeNormalize(Vector2.UnitX) * 8;
                }
            }
            if (time >= fallTime)
            {
                float fadeIn = Utils.GetLerpValue(fallTime * 1.7f, fallTime, time, true);
                // Spawn in a helix-style pattern
                float sine = (float)Math.Sin(Projectile.timeLeft * 0.475f / MathHelper.Pi);

                Vector2 offset = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2) * sine * (16f + 34 * fadeIn) * Projectile.scale;
                if (targetDist < 1400f && time % 2 == 0)
                {
                    CustomSpark orb = new(Projectile.Center + offset, -Projectile.velocity * 0.5f, "CalamityMod/Particles/BloomCircle", false, 10, (0.3f + (0.4f * fadeIn)) * Projectile.scale, mainColor, new Vector2(0.5f, 1f), true, false);
                    GeneralParticleHandler.SpawnParticle(orb);

                    CustomSpark orb2 = new(Projectile.Center - offset, -Projectile.velocity * 0.5f, "CalamityMod/Particles/BloomCircle", false, 10, (0.3f + (0.4f * fadeIn)) * Projectile.scale, mainColor, new Vector2(0.5f, 1f), true, false);
                    GeneralParticleHandler.SpawnParticle(orb2);
                }
            }
            else
            {
                float randSize = Main.rand.NextFloat(0.8f, 1.2f) * Projectile.scale;
                if (!CalamityClientConfig.Instance.Photosensitivity)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Particle bloom = new CustomPulse(Projectile.Center, Vector2.Zero, mainColor * (float)Math.Pow(Utils.Remap(time, 0, fallTime, 0f, 1f), 3), "CalamityMod/Particles/LargeBloom", new Vector2(Utils.Remap(time, 0, fallTime, 0.3f, 4), Utils.Remap(time, fallTime * 0.6f, fallTime, 1, 4)), 0, 0.8f * randSize, 0f, 3);
                        GeneralParticleHandler.SpawnParticle(bloom);
                    }
                    Particle bloom3 = new CustomPulse(Projectile.Center, Vector2.Zero, Color.White * (float)Math.Pow(Utils.Remap(time, 0, fallTime, 0f, 1f), 3), "CalamityMod/Particles/LargeBloom", new Vector2(Utils.Remap(time, 0, fallTime, 0.3f, 4), Utils.Remap(time, fallTime * 0.6f, fallTime, 1, 4)), 0, 0.65f * randSize, 0f, 3);
                    GeneralParticleHandler.SpawnParticle(bloom3);
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            time++;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> tex = ModContent.Request<Texture2D>("CalamityMod/Particles/VerticalSmearRagged");
            if (time <= fallTime)
                return false;
            if (!CalamityClientConfig.Instance.Photosensitivity)
            {
                for (int i = 0; i < 10; i++)
                {
                    Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition + Projectile.velocity.SafeNormalize(Vector2.UnitX) * (i - 110), null, mainColor with { A = 0 }, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale * new Vector2(0.4f - i * 0.05f, 1.3f + i * 0.1f) * 0.4f, i % 2 == 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
                }
            }

            return false;
        }
        public override void OnKill(int timeLeft)
        {
            if (SoundEngine.TryGetActiveSound(AudSlot, out var ChargeSound) && Projectile.ai[2] == 2)
                ChargeSound?.Stop();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<MiracleBlight>(), 300);
            if (Projectile.numHits <= 0)
            {
                Player Owner = Main.player[Projectile.owner];
                Owner.SetScreenshake(4.5f);
                Projectile boom = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<EarthBoom>(), (int)(Projectile.damage * 0.75f), Projectile.knockBack, Projectile.owner);
                boom.scale = Projectile.scale;
                for (int i = 0; i < 15; i++)
                {
                    randomColor = Main.rand.Next(3) switch
                    {
                        0 => Color.OrangeRed,
                        1 => Color.MediumTurquoise,
                        _ => Color.LawnGreen,
                    };
                    Dust dust2 = Dust.NewDustPerfect(target.Center, DustID.FireworksRGB, Vector2.One.RotatedByRandom(100) * Main.rand.NextFloat(5.5f, 20));
                    dust2.scale = Main.rand.NextFloat(0.85f, 1.15f);
                    dust2.noGravity = false;
                    dust2.color = Color.Lerp(Color.White, randomColor, 0.5f);

                    Particle sparker = new CustomSpark(target.Center, Vector2.One.RotatedByRandom(100) * Main.rand.NextFloat(5.5f, 20), "CalamityMod/Particles/Sparkle", false, 38, Main.rand.NextFloat(2.2f, 4.8f) * Projectile.scale, randomColor, new Vector2(0.4f, Main.rand.NextFloat(0.9f, 1.4f)), true, true);
                    GeneralParticleHandler.SpawnParticle(sparker);
                }

                SoundStyle fire2 = new("CalamityMod/Sounds/Item/EarthMeteor");
                SoundEngine.PlaySound(fire2 with { Volume = 0.9f }, target.Center);

                if (!CalamityClientConfig.Instance.Photosensitivity)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        string tex = "CalamityMod/Particles/ShatteredExplosion";
                        Particle bolt1 = new CustomSpark(target.Center, Vector2.Zero, tex, false, 12, (0.5f - i * 0.08f) * Projectile.scale, Color.OrangeRed * 0.8f, Vector2.One * 0.65f, extraRotation: Main.rand.NextFloat(-10f, 10f), shrinkSpeed: (i == 0 ? 0.6f : 0.8f));
                        GeneralParticleHandler.SpawnParticle(bolt1);
                        Particle bolt2 = new CustomSpark(target.Center, Vector2.Zero, tex, false, 10, (0.4f - i * 0.08f) * Projectile.scale, Color.MediumTurquoise * 0.8f, Vector2.One * 0.65f, extraRotation: Main.rand.NextFloat(-10f, 10f), shrinkSpeed: (i == 0 ? 0.6f : 0.8f));
                        GeneralParticleHandler.SpawnParticle(bolt2);
                        Particle bolt3 = new CustomSpark(target.Center, Vector2.Zero, tex, false, 8, (0.35f - i * 0.08f) * Projectile.scale, Color.LawnGreen * 0.8f, Vector2.One * 0.65f, extraRotation: Main.rand.NextFloat(-10f, 10f), shrinkSpeed: (i == 0 ? 0.6f : 0.8f));
                        GeneralParticleHandler.SpawnParticle(bolt3);

                    }

                    Particle blastRing = new CustomPulse(target.Center, Vector2.Zero, mainColor, "CalamityMod/Particles/BloomCircle", Vector2.One, Main.rand.NextFloat(-10, 10), 4f * Projectile.scale, 3f * Projectile.scale, 18, true);
                    GeneralParticleHandler.SpawnParticle(blastRing);
                    Particle blastRing2 = new CustomPulse(target.Center, Vector2.Zero, Color.White, "CalamityMod/Particles/BloomCircle", Vector2.One, Main.rand.NextFloat(-10, 10), 3f * Projectile.scale, 2f * Projectile.scale, 18, true);
                    GeneralParticleHandler.SpawnParticle(blastRing2);
                }
            }
        }
        public override bool? CanDamage() => time < fallTime ? false : null;
        public override bool? CanCutTiles() => false;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 80, targetHitbox);
    }
}
