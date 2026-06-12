using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class HolyColliderHolyFire : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/Boss/HolyFire2";

        public bool isLaunched = false;
        public bool setStats = true;
        public static int statMax = 8;
        public int setStatTimer = statMax;
        public ref float scale => ref Projectile.localAI[0];
        public ref float time => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10 * Projectile.MaxUpdates;
        }

        public override bool? CanDamage() => (Projectile.ai[0] == 10 && Projectile.numHits > 5) ? false : ((time > 10 && !isLaunched) || (isLaunched && !setStats)) ? null : false;

        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];
            int hitTime = 13; // Time to pass before it can be launched

            if (Projectile.ai[2] == 5 && time >= hitTime && Projectile.ai[0] != 10)
                isLaunched = true;
            else
                Projectile.ai[2] = 0;
            if (Projectile.ai[0] == 10)
                Projectile.localNPCHitCooldown = 60 * Projectile.MaxUpdates;

            Projectile.scale = Utils.GetLerpValue(0, 40, Projectile.timeLeft, true);

            if (isLaunched)
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
                if (setStats)
                {
                    if (setStatTimer == statMax)
                    {
                        Projectile.timeLeft = 300;
                        for (int i = 0; i < 2; i++)
                        {
                            SoundStyle sound5 = new("CalamityMod/Sounds/Item/HeliumFlashCoreImpact");
                            SoundEngine.PlaySound(sound5 with { Volume = 0.55f, Pitch = 0.6f, MaxInstances = 2 }, Projectile.Center);
                        }
                        float starAngle = Main.rand.NextFloat(-0.9f, 0.9f);
                        for (int i = 0; i < 4; i++)
                        {
                            Dust chargefull = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB);
                            Vector2 vel = (MathHelper.TwoPi * i / 4f).ToRotationVector2().RotatedBy(starAngle) * 8f;

                            Particle pulse = new GlowSparkParticle(Projectile.Center, vel, false, 10, 0.08f * scale, Color.Orange, new Vector2(3.2f, 0.9f), true, true, 0.9f);
                            GeneralParticleHandler.SpawnParticle(pulse);
                        }
                    }
                    if (setStatTimer == 0)
                    {
                        SoundStyle fire = new("CalamityMod/Sounds/Custom/Providence/ProvidenceHolyBlastShoot");
                        SoundEngine.PlaySound(fire with { Volume = 1f, Pitch = 0.4f }, Projectile.Center);

                        Projectile.extraUpdates = 5;
                        // 14NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
                        Vector2 vel = (Projectile.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX) * -12;
                        Projectile.velocity = vel;
                        Projectile.penetrate = 1;
                        // This has reduced damage on spawn so this isn't as high as it seems
                        Projectile.damage *= 15;
                        time = 0;
                        setStats = false;
                    }
                    else
                        setStatTimer -= 2;
                }
                else
                {
                    if (Main.rand.NextBool())
                    {
                        Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(80, 80) * scale, ModContent.DustType<SquashDust>(), (Projectile.velocity * 2) * Main.rand.NextFloat(0.1f, 1f));
                        dust.noGravity = true;
                        dust.scale = Main.rand.NextFloat(1.85f, 2.45f) * scale;
                        dust.color = Main.rand.NextBool() ? Color.OrangeRed : Color.Goldenrod;
                        dust.noLightEmittence = true;
                        dust.fadeIn = scale - 1;
                    }
                    if (Main.rand.NextBool())
                    {
                        Particle spark = new SparkParticle(Projectile.Center + Main.rand.NextVector2Circular(80, 80) * scale, -Projectile.velocity * Main.rand.NextFloat(0.1f, 1f), false, 11, 0.9f * scale, Main.rand.NextBool() ? Color.Goldenrod : Color.Orange);
                        GeneralParticleHandler.SpawnParticle(spark);
                    }
                    else
                    {
                        Particle spark = new CustomSpark(Projectile.Center + Main.rand.NextVector2Circular(80, 80) * scale, -Projectile.velocity * Main.rand.NextFloat(0.1f, 1f), "CalamityMod/Particles/ProvidenceMarkParticle", false, 27, Main.rand.NextFloat(1.15f, 1.3f) * scale, Main.rand.NextBool(4) ? Color.Khaki : Color.Orange, new Vector2(1.3f, 0.5f), true, false, 0, false, false, Main.rand.NextFloat(0.1f, 0.2f));
                        GeneralParticleHandler.SpawnParticle(spark);
                    }
                    
                }
            }
            else
            {
                if (Main.rand.NextBool(5))
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SquashDust>(), (new Vector2(0, -7)).RotatedByRandom(0.2) * Main.rand.NextFloat(0.2f, 1f));
                    dust.noGravity = true;
                    dust.scale = Main.rand.NextFloat(0.85f, 1.45f) * Projectile.scale;
                    dust.color = Color.Goldenrod;
                    dust.noLightEmittence = true;
                    dust.fadeIn = scale - 1;
                }
                
                if (Projectile.velocity.Length() > 8)
                    Projectile.velocity *= 0.88f;
                else
                    Projectile.velocity *= 0.965f;
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
                Projectile.frame = 0;

            time++;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, (isLaunched ? 100 : 20) * scale, targetHitbox);
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
        }
        public override void OnKill(int timeLeft)
        {
            // boom
            Player Owner = Main.player[Projectile.owner];
            if (isLaunched && Projectile.scale > 0.2f)
            {
                Owner.SetScreenshake(9f);

                SoundStyle sound = new("CalamityMod/Sounds/Item/HolyColliderProjectileHit");
                SoundEngine.PlaySound(sound with { Volume = 1f }, Projectile.Center);

                if (!CalamityClientConfig.Instance.Photosensitivity)
                {
                    for (int g = 0; g < 3; g++)
                    {
                        Particle blastRing = new CustomPulse(Projectile.Center, Vector2.Zero, Color.OrangeRed, "CalamityMod/Particles/BloomCircle", Vector2.One, Main.rand.NextFloat(-10, 10), 2.8f * (g + 1) * scale, 1.7f * scale, 18, true);
                        GeneralParticleHandler.SpawnParticle(blastRing);
                        Particle blastRing2 = new CustomPulse(Projectile.Center, Vector2.Zero, Color.White, "CalamityMod/Particles/BloomCircle", Vector2.One, Main.rand.NextFloat(-10, 10), 2.2f * (g + 1) * scale, 1.3f * scale, 18, true);
                        GeneralParticleHandler.SpawnParticle(blastRing2);
                    }
                    for (int i = 0; i < 5; i++)
                    {
                        Particle orb1 = new CustomPulse(Projectile.Center, Vector2.Zero, Color.Lerp(Color.OrangeRed, Color.Orange, i * 0.2f), i == 4 ? "CalamityMod/Particles/ShatteredExplosion" : "CalamityMod/Particles/FlameExplosion", new Vector2(1, 1), Main.rand.NextFloat(-10, 10), 0, (0.11f + i * 0.05f) * scale, 18);
                        GeneralParticleHandler.SpawnParticle(orb1);
                    }
                }
                for (int i = 0; i < 25; i++)
                {
                    Particle spark = new SparkParticle(Projectile.Center, new Vector2(21, 21).RotatedByRandom(100) * Main.rand.NextFloat(0.4f, 1f), true, 55, 0.85f * scale, Main.rand.NextBool() ? Color.Goldenrod : Color.OrangeRed);
                    GeneralParticleHandler.SpawnParticle(spark);
                }

                Projectile blast = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BurningHolyBlast>(), (int)(Projectile.damage * 0.47), Projectile.knockBack, Projectile.owner, 1.8f);
                blast.scale = scale;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D bloomTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;

            Texture2D smallTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/HolyColliderHolyFire").Value;
            Texture2D bigTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/HolyColliderHolyFire2").Value;

            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            // The back glow
            float power = !setStats ? 2.4f : 0.6f;
            float randSize = Main.rand.NextFloat(0.8f, 1.1f);
            Main.EntitySpriteDraw(bloomTexture, drawPos, null, Color.Goldenrod with { A = 0 }, Projectile.rotation, bloomTexture.Size() * 0.5f, 0.65f * randSize * power * Projectile.scale * scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(bloomTexture, drawPos, null, Color.White with { A = 0 } * 0.65f, Projectile.rotation, bloomTexture.Size() * 0.5f, 0.45f * randSize * power * Projectile.scale * scale, SpriteEffects.None, 0);

            Texture2D usedTex = (!setStats ? bigTexture : smallTexture);
            Rectangle frame = usedTex.Frame(1, 4, 0, Projectile.frame);
            Vector2 rotationPoint = frame.Size() * 0.5f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation;

            if (setStatTimer == statMax || !setStats)
                Main.EntitySpriteDraw(usedTex, drawPosition, frame, Color.White, drawRotation, rotationPoint, Projectile.scale * scale, SpriteEffects.None);
            return false;
        }
    }
}
