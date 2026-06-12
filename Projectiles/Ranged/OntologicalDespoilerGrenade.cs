using System;
using System.Collections.Generic;
using CalamityMod.Dusts;
using CalamityMod.Particles;
using CalamityMod.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class OntologicalDespoilerGrenade : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/Ranged/OntologicalDespoilerGrenade";
        public ref float time => ref Projectile.ai[0];
        public bool explode = true;
        public Color baseColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
        public Player Owner => Main.player[Projectile.owner];
        public Color color1;
        public Color color2;
        public Color color3;
        public Color color4;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 25;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.extraUpdates = 3;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            color1 = Owner.shirtColor;
            color2 = Color.Lerp(Owner.shirtColor, Color.Black, 0.3f);
            color3 = Color.Lerp(Owner.shirtColor, Color.White, 0.2f);
            color4 = Color.Lerp(Owner.shirtColor, Color.White, 0.4f);

            Projectile.rotation = Projectile.velocity.ToRotation();

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 12)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 5)
            {
                Projectile.frame = 0;
            }

            if (time < 60)
                Projectile.velocity *= 0.982f;

            if (Owner.shirtColor != Color.White)
            {
                float rate = (Main.GlobalTimeWrappedHourly * 15);
                List<Color> eColors = new List<Color>()
                {
                    color1,
                    color2,
                    color3,
                    color4
                };
                int colorIndex = (int)(rate / 2 % eColors.Count);
                Color currentColor = eColors[colorIndex];
                Color nextColor = eColors[(colorIndex + 1) % eColors.Count];
                baseColor = Color.Lerp(currentColor, nextColor, rate % 2f > 1f ? 1f : rate % 1f);
            }
            else
                baseColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);

            if (time > 20)
            {
                // Spawn in a helix-style pattern
                float sine = (float)Math.Sin(Projectile.timeLeft * 0.575f / MathHelper.Pi);

                Projectile.scale = MathHelper.Clamp(1 + sine, 0.7f, 1.3f);

                Vector2 offset = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2) * sine * 12f;
                float scale = Main.rand.NextFloat(1.3f, 1.4f);
                if (Main.rand.NextBool(3))
                {
                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center + offset, ModContent.DustType<VoidDust>(), -Projectile.velocity * Main.rand.NextFloat(0.3f, 0.8f));
                    dust2.noGravity = true;
                    dust2.scale = scale;
                    dust2.color = baseColor;
                }
                if (Main.rand.NextBool(3))
                {
                    Dust dust3 = Dust.NewDustPerfect(Projectile.Center - offset, ModContent.DustType<VoidDustInverted>(), -Projectile.velocity * Main.rand.NextFloat(0.3f, 0.8f));
                    dust3.noGravity = true;
                    dust3.scale = scale;
                    dust3.color = baseColor;
                }
            }

            if (Projectile.timeLeft == 1)
                explode = false;

            NPC targetedNPC = Projectile.Center.ClosestNPCAt(1200);
            if (targetedNPC != null && time > 30 && Projectile.numHits < 1 && Vector2.Distance(targetedNPC.Center, Projectile.Center) < 1200)
            {
                float moveSpeed = Utils.GetLerpValue(570, 120, Projectile.timeLeft, true);
                CalamityUtils.HomeInOnSelectedNPC(Projectile, targetedNPC, true, moveSpeed, 7, 0.98f, accelerate: true);
                explode = true;
            }
            time++;
        }
        public override void OnKill(int timeLeft)
        {
            Player Owner = Main.player[Projectile.owner];

            if (explode)
            {
                Owner.SetScreenshake(8.5f);
                float power = 1.5f;

                for (int i = 0; i < 55; i++)
                {
                    Color useColor = GetRandomColor();
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, (Main.rand.NextBool(6) ? ModContent.DustType<VoidDustInverted>() : ModContent.DustType<VoidDust>()), (Projectile.velocity * 6 * power).RotatedByRandom(100) * Main.rand.NextFloat(0.2f, 1f));
                    dust.noGravity = true;
                    dust.scale = Main.rand.NextFloat(1.75f, 2.25f) * power;
                    dust.color = useColor;
                    if (Owner.shirtColor == Color.White)
                        useColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);

                    if (i % 2 == 0)
                    {
                        Particle orb2 = new CustomSpark(Projectile.Center, new Vector2(0, -40 * power).RotatedByRandom(100) * Main.rand.NextFloat(0.1f, 1f), "CalamityMod/Particles/Sparkle", false, 40, Main.rand.NextFloat(1.4f, 2.4f) * power, useColor, new Vector2(0.4f, 1.1f));
                        GeneralParticleHandler.SpawnParticle(orb2);
                    }
                }
                for (int i = 0; i < 3; i++)
                {
                    Color useColor = GetRandomColor();
                    Particle orb4 = new CustomPulse(Projectile.Center, Vector2.Zero, useColor, "CalamityMod/Particles/SoftRoundExplosion", new Vector2(1, 1), Main.rand.NextFloat(-10, 10), 0, 0.4f - i * 0.03f * power, 13);
                    GeneralParticleHandler.SpawnParticle(orb4);
                }
                Particle orb = new CustomPulse(Projectile.Center, Vector2.Zero, baseColor, "CalamityMod/Particles/BloomRing", new Vector2(1, 1), Main.rand.NextFloat(-10, 10), 0.15f * power, 2.5f * power, 38);
                GeneralParticleHandler.SpawnParticle(orb);
                int parts = 8;
                float rot = Main.rand.NextFloat(-9, 9);
                for (int i = 0; i < parts; i++)
                {
                    Color useColor = GetRandomColor();
                    Particle orb2 = new CustomSpark(Projectile.Center, new Vector2(0, -15 * (i % 2 == 0 ? 1.8f : 1f) * power).RotatedBy(i * (MathHelper.ToRadians(360f) / parts)).RotatedBy(rot), "CalamityMod/Particles/VerticalSmear", false, 19, 3 * power, useColor, new Vector2(0.2f, 1));
                    GeneralParticleHandler.SpawnParticle(orb2);
                }
                Particle orb3 = new CustomPulse(Projectile.Center, Vector2.Zero, Color.Black, "CalamityMod/Particles/SmallBloom", new Vector2(1, 1), Main.rand.NextFloat(-10, 10), 0, 1.2f * power, 39, false);
                GeneralParticleHandler.SpawnParticle(orb3);

                for (int i = 0; i < 3; i++)
                {
                    SoundStyle fire = new("CalamityMod/Sounds/Item/EarthMeteor");
                    SoundEngine.PlaySound(fire with { Volume = 0.6f, Pitch = -0.1f * (i + 1), MaxInstances = 3 }, Projectile.Center);
                }
                SoundStyle fire2 = new("CalamityMod/Sounds/Item/ShadowboltReflect");
                SoundEngine.PlaySound(fire2 with { Volume = 0.9f, Pitch = -0.4f}, Projectile.Center);

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<OntoligicalDespoilerBurst>(), (int)(Projectile.damage * 12f), 0, Projectile.owner, 0, 0, 0);
            }
            else
            {
                for (int i = 0; i < 20; i++)
                {
                    Color useColor = GetRandomColor();
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, (Main.rand.NextBool() ? ModContent.DustType<VoidDustInverted>() : ModContent.DustType<VoidDust>()), (Projectile.velocity * 3).RotatedByRandom(100) * Main.rand.NextFloat(0.2f, 1f));
                    dust.noGravity = true;
                    dust.scale = Main.rand.NextFloat(1.85f, 2.45f);
                    dust.color = useColor;
                }
            }
        }
        public Color GetRandomColor()
        {
            Color useColor = Main.rand.Next(4) switch
            {
                0 => color1,
                1 => color2,
                2 => color3,
                _ => color4,
            };
            if (Owner.shirtColor == Color.White)
                useColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
            return useColor;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
            => modifiers.ApplyScalingForcedCrit(Projectile);
        public override bool PreDraw(ref Color lightColor)
        {
            if (time < 2)
                return false;

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation;

            Asset<Texture2D> tex = ModContent.Request<Texture2D>("CalamityMod/Particles/DrainLineBloom2");
            Asset<Texture2D> tex2 = ModContent.Request<Texture2D>("CalamityMod/Particles/DrainLine2");
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], baseColor with { A = 0 } * 0.8f, 1, tex.Value);
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], Color.Black, 1, tex2.Value, true, true);

            Asset<Texture2D> tex3 = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Ranged/OntologicalDespoilerGrenade");
            Rectangle frame = tex3.Frame(1, 6, 0, Projectile.frame);
            Vector2 rotationPoint = frame.Size() * 0.5f;
            for (int i = 0; i < 4; i++)
                Main.EntitySpriteDraw(tex3.Value, drawPosition + Main.rand.NextVector2Circular(7, 7), frame, baseColor with { A = 0 }, drawRotation, rotationPoint, 1, SpriteEffects.None);
            Main.EntitySpriteDraw(tex3.Value, drawPosition, frame, baseColor, drawRotation, rotationPoint, 1, SpriteEffects.None);

            return false;
        }
    }
}
