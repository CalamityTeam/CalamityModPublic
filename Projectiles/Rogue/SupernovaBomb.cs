using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class SupernovaBomb : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Supernova";
        public Color variedColor = Color.White;
        public Color mainColor = Color.LawnGreen;
        public Color randomColor = Color.White;
        public int colorTimer = 0;
        public int time = 0;
        public bool homing = false;
        public bool returning = false;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 106;
            Projectile.height = 112;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            randomColor = Main.rand.Next(4) switch
            {
                0 => Color.Red,
                1 => Color.MediumTurquoise,
                2 => Color.Orange,
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
                    0 => Color.Red,
                    1 => Color.MediumTurquoise,
                    2 => Color.Orange,
                    _ => Color.LawnGreen,
                };
                colorTimer++;
                if (colorTimer >= 4)
                    colorTimer = 0;
            }

            Vector2 visualDirection = new Vector2(17, -17).RotatedBy(Projectile.rotation);
            Vector2 rotatedVisualDirection = new Vector2(17, -17).RotatedByRandom(0.5).RotatedBy(Projectile.rotation);
            PointParticle orb = new PointParticle(Projectile.Center + visualDirection, visualDirection * Main.rand.NextFloat(0.01f, 0.3f), false, 5, Main.rand.NextFloat(0.8f, 1f), Color.White * 0.7f * Utils.GetLerpValue(10, 45, time, true));
            GeneralParticleHandler.SpawnParticle(orb);
            if (Main.rand.NextBool())
            {
                GlowOrbParticle orb2 = new GlowOrbParticle(Projectile.Center + rotatedVisualDirection * 0.7f, rotatedVisualDirection * Main.rand.NextFloat(0.01f, 0.15f), false, 13, Main.rand.NextFloat(0.55f, 0.9f), randomColor * Utils.GetLerpValue(10, 45, time, true));
                GeneralParticleHandler.SpawnParticle(orb2);

                SparkParticle orb3 = new SparkParticle(Projectile.Center + rotatedVisualDirection * 0.7f, rotatedVisualDirection * Main.rand.NextFloat(0.01f, 0.5f), false, 17, Main.rand.NextFloat(0.2f, 0.6f), Color.Lerp(Color.White, randomColor, 0.5f) * Utils.GetLerpValue(10, 45, time, true));
                GeneralParticleHandler.SpawnParticle(orb3);
            }

            mainColor = Color.Lerp(mainColor, variedColor, 0.07f);

            Projectile.scale = 0.4f;

            //dust and lighting

            Lighting.AddLight(Projectile.Center, Color.Lerp(Color.White, randomColor, 0.5f).ToVector3());

            if (!homing && !returning)
                Projectile.velocity *= 0.95f;
            else
            {
                Projectile.rotation = Projectile.rotation.AngleLerp(Projectile.velocity.ToRotation() - MathHelper.PiOver4 * 3, 0.2f);
                if (homing)
                    CalamityUtils.HomeInOnNPC(Projectile, true, 2000f, MathHelper.Clamp(5 + time * 0.1f, 10, 17), 5);
                if (Main.rand.NextBool(3))
                {
                    SparkParticle orb3 = new SparkParticle(Projectile.Center + Main.rand.NextVector2Circular(9, 9) - Projectile.velocity.SafeNormalize(Vector2.UnitY) * 12, visualDirection * Main.rand.NextFloat(0.2f, 0.8f), false, 20, 1.5f, Color.Lerp(Color.White, randomColor, 0.5f) * Utils.GetLerpValue(100, 170, time, true));
                    GeneralParticleHandler.SpawnParticle(orb3);
                }
                for (int i = 0; i < 2; i++)
                {
                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(12, 12) - Projectile.velocity.SafeNormalize(Vector2.UnitY) * 12, 303, visualDirection * Main.rand.NextFloat(0.05f, 0.7f));
                    dust2.scale = Main.rand.NextFloat(0.75f, 2.25f);
                    dust2.noGravity = true;
                    dust2.color = Main.rand.NextBool() ? Color.White : Color.Lerp(Color.White, randomColor, 0.1f);
                    dust2.alpha = 170;
                }
            }

            if (time >= 60 && !homing && !returning)
            {
                NPC target = Projectile.Center.ClosestNPCAt(2000);

                if (target == null)
                {
                    returning = true;
                    SoundStyle lockon = new("CalamityMod/Sounds/Custom/ExoMechs/ApolloArtemisTargetSelection");
                    SoundEngine.PlaySound(lockon with { Pitch = -0.7f }, Projectile.Center);
                    return;
                }

                if (time >= 60 && time < 75 && Projectile.Calamity().stealthStrike)
                {
                    SoundStyle lockon = new("CalamityMod/Sounds/Custom/ExoMechs/ApolloArtemisTargetSelection");
                    if (time == 60)
                    {
                        SoundEngine.PlaySound(lockon with { Pitch = -0.6f }, Projectile.Center);

                        Particle pulse = new CustomPulse(Projectile.Center, Vector2.Zero, Color.White * 0.7f, "CalamityMod/Particles/BloomRing", new Vector2(1f, 1f), 0f, 0.95f, 0.2f, 10);
                        GeneralParticleHandler.SpawnParticle(pulse);

                        Particle pulse2 = new CustomPulse(Projectile.Center, Vector2.Zero, randomColor * 0.7f, "CalamityMod/Particles/BloomRing", new Vector2(1f, 1f), 0f, 0.8f, 0.2f, 10);
                        GeneralParticleHandler.SpawnParticle(pulse2);
                    }
                    if (time == 67)
                    {
                        SoundEngine.PlaySound(lockon with { Pitch = -0.4f }, Projectile.Center);

                        Particle pulse = new CustomPulse(Projectile.Center, Vector2.Zero, Color.White * 0.7f, "CalamityMod/Particles/BloomRing", new Vector2(1f, 1f), 0f, 0.75f, 0.2f, 10);
                        GeneralParticleHandler.SpawnParticle(pulse);

                        Particle pulse2 = new CustomPulse(Projectile.Center, Vector2.Zero, randomColor * 0.7f, "CalamityMod/Particles/BloomRing", new Vector2(1f, 1f), 0f, 0.6f, 0.2f, 10);
                        GeneralParticleHandler.SpawnParticle(pulse2);
                    }
                    if (time == 74)
                    {
                        SoundEngine.PlaySound(lockon with { Pitch = -0.1f }, Projectile.Center);

                        Particle pulse = new CustomPulse(Projectile.Center, Vector2.Zero, Color.White * 0.7f, "CalamityMod/Particles/BloomRing", new Vector2(1f, 1f), 0f, 0.65f, 0.2f, 10);
                        GeneralParticleHandler.SpawnParticle(pulse);

                        Particle pulse2 = new CustomPulse(Projectile.Center, Vector2.Zero, randomColor * 0.7f, "CalamityMod/Particles/BloomRing", new Vector2(1f, 1f), 0f, 0.5f, 0.2f, 10);
                        GeneralParticleHandler.SpawnParticle(pulse2);
                    }
                }
                if (time == 60)
                {
                    if (!Projectile.Calamity().stealthStrike)
                    {
                        SoundStyle lockon = new("CalamityMod/Sounds/Custom/ExoMechs/ApolloArtemisTargetSelection");
                        SoundEngine.PlaySound(lockon with { Pitch = -0.3f }, Projectile.Center);

                        Particle pulse = new CustomPulse(Projectile.Center, Vector2.Zero, Color.White * 0.7f, "CalamityMod/Particles/BloomRing", new Vector2(1f, 1f), 0f, 0.75f, 0.2f, 10);
                        GeneralParticleHandler.SpawnParticle(pulse);

                        Particle pulse2 = new CustomPulse(Projectile.Center, Vector2.Zero, randomColor * 0.7f, "CalamityMod/Particles/BloomRing", new Vector2(1f, 1f), 0f, 0.6f, 0.2f, 10);
                        GeneralParticleHandler.SpawnParticle(pulse2);
                    }
                }
                if (time > (Projectile.Calamity().stealthStrike ? 75 : 62))
                {
                    Projectile.penetrate = 1;
                    homing = true;
                    Projectile.extraUpdates = 3;
                }
            }

            if (returning)
            {
                Projectile.penetrate = -1;
                float returnSpeed = MathHelper.Clamp(3 + time * 0.15f, 7, 28);
                float acceleration = 1.1f;
                Player owner = Main.player[Projectile.owner];
                Vector2 playerCenter = owner.Center;
                float xDist = playerCenter.X - Projectile.Center.X;
                float yDist = playerCenter.Y - Projectile.Center.Y;
                float dist = (float)Math.Sqrt(xDist * xDist + yDist * yDist);
                dist = returnSpeed / dist;
                xDist *= dist;
                yDist *= dist;

                if (Projectile.velocity.X < xDist)
                {
                    Projectile.velocity.X = Projectile.velocity.X + acceleration;
                    if (Projectile.velocity.X < 0f && xDist > 0f)
                        Projectile.velocity.X += acceleration;
                }
                else if (Projectile.velocity.X > xDist)
                {
                    Projectile.velocity.X = Projectile.velocity.X - acceleration;
                    if (Projectile.velocity.X > 0f && xDist < 0f)
                        Projectile.velocity.X -= acceleration;
                }
                if (Projectile.velocity.Y < yDist)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + acceleration;
                    if (Projectile.velocity.Y < 0f && yDist > 0f)
                        Projectile.velocity.Y += acceleration;
                }
                else if (Projectile.velocity.Y > yDist)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - acceleration;
                    if (Projectile.velocity.Y > 0f && yDist < 0f)
                        Projectile.velocity.Y -= acceleration;
                }

                if (Projectile.Hitbox.Intersects(owner.Hitbox))
                {
                    Projectile.Kill();
                }
            }

            time++;
        }

        public override void OnKill(int timeLeft)
        {
            if (!returning)
            {
                Projectile.position = Projectile.Center;
                Projectile.width = Projectile.height = 128;
                Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
                Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);

                //spawn explosion
                if (Projectile.owner == Main.myPlayer)
                {
                    if (Projectile.Calamity().stealthStrike)
                    {
                        float pitchRange = Main.rand.NextFloat(-0.1f, 0.1f);
                        SoundEngine.PlaySound(Supernova.StealthChargeSound with { Pitch = pitchRange }, Projectile.Center);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SupernovaStealthBoom>(), Projectile.damage, 0, Projectile.owner, 0f, 0f, pitchRange);
                    }
                    else
                    {
                        SoundEngine.PlaySound(Supernova.ExplosionSound, Projectile.Center);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SupernovaBoom>(), Projectile.damage, 0, Projectile.owner, 0f, 0f, 0f);
                    }
                }
            }
            else
            {
                for (int i = 0; i < 15; i++)
                {
                    randomColor = Main.rand.Next(4) switch
                    {
                        0 => Color.Red,
                        1 => Color.MediumTurquoise,
                        2 => Color.Orange,
                        _ => Color.LawnGreen,
                    };
                    Vector2 vel = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 14 * Main.rand.NextFloat(0.05f, 1.2f);
                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(20, 20), 278, vel);
                    dust2.scale = Main.rand.NextFloat(0.45f, 1.15f);
                    dust2.noGravity = true;
                    dust2.color = Color.Lerp(Color.White, randomColor, 0.3f);
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<MiracleBlight>(), 60);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) // Add to regular plz
        {
            if (!homing)
                modifiers.SourceDamage *= 0.01f;
            else
                modifiers.SourceDamage *= 0.05f;

            if (Projectile.damage < 1)
                Projectile.damage = 1;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<MiracleBlight>(), 300);

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 5, targetHitbox);
        public override bool PreDraw(ref Color lightColor)
        {
            Color auraColor = Projectile.GetAlpha(Color.Lerp(Color.White, randomColor, 0.3f)) * 0.25f;
            for (int i = 0; i < 7; i++)
            {
                Texture2D centerTexture = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Rogue/Supernova").Value;
                Vector2 rotationalDrawOffset = (MathHelper.TwoPi * i / 7f + Main.GlobalTimeWrappedHourly * 8f).ToRotationVector2();
                rotationalDrawOffset *= MathHelper.Lerp(3f, 5.25f, (float)Math.Cos(Main.GlobalTimeWrappedHourly * 4f) * 0.5f + 0.5f);
                Main.EntitySpriteDraw(centerTexture, Projectile.Center - Main.screenPosition + rotationalDrawOffset, null, auraColor, Projectile.rotation, centerTexture.Size() * 0.5f, Projectile.scale * 1.1f, SpriteEffects.None, 0f);
            }

            if (!homing)
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], Color.Lerp(Color.White, randomColor, 0.3f), 1);
            return true;
        }
    }
}
