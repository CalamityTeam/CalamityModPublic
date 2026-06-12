using CalamityMod.Particles;
using CalamityMod.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class VenusianBolt : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        public int time = 0;
        public bool explode = true;
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 7;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];
            float targetDist = Vector2.Distance(Owner.Center, Projectile.Center);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45);
            Lighting.AddLight(Projectile.Center, 0.25f, 0.2f, 0f);

            if (Projectile.timeLeft == 1)
                explode = false;
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item73, Projectile.Center);
                Projectile.localAI[0] += 1f;
            }
            if (time == 5)
            {
                for (int i = 0; i < 12; i++)
                {
                    Dust chargefull = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB);
                    chargefull.velocity = Projectile.velocity.RotatedByRandom(0.25f) * Main.rand.NextFloat(0.1f, 1);
                    chargefull.scale = Main.rand.NextFloat(0.45f, 0.8f);
                    chargefull.noGravity = true;
                    chargefull.color = Color.Lerp(Color.White, Main.rand.NextBool(4) ? Color.Orange : Color.Coral, 0.7f);
                }
            }
            if (targetDist < 1400)
            {
                if (time > 8 && time % 3 == 0)
                {
                    SparkParticle spark = new SparkParticle(Projectile.Center + Main.rand.NextVector2Circular(15, 15) - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 10, -Projectile.velocity * Main.rand.NextFloat(0.2f, 0.8f), false, 35, 0.8f, Color.Lerp(Color.OrangeRed, Color.Coral, Main.rand.NextFloat(0, 1)));
                    GeneralParticleHandler.SpawnParticle(spark);
                }
                if (time > 6 && time % 2 == 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Particle spark = new GlowSparkParticle(Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 10, -Projectile.velocity, false, 8, 0.08f * (i == 0 ? 0.5f : 1), Color.Lerp(Color.Orange, Color.Coral, 0.25f) * 0.5f, new Vector2(0.3f, 1f), false, false, 0.8f);
                        GeneralParticleHandler.SpawnParticle(spark);
                    }
                }
            }
            time++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Daybreak, 300);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.ApplyScalingForcedCrit(Projectile);
        }

        public override void OnKill(int timeLeft)
        {
            if (explode)
            {
                Main.player[Projectile.owner].SetScreenshake(3.5f);
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode with { Volume = 1f, Pitch = -0.5f }, Projectile.Center);
                SoundEngine.PlaySound(SoundID.Item74 with { Volume = 0.7f, Pitch = 1f }, Projectile.Center);
                for (int i = 0; i < 6; i++)
                {
                    Particle explosion = new CustomPulse(Projectile.Center, Vector2.Zero, Color.Lerp(Color.Red, Color.Coral, Utils.GetLerpValue(-2, 6, i, true)), i == 5 ? "CalamityMod/Particles/FlameExplosion" : "CalamityMod/Particles/SoftRoundExplosion", Vector2.One, Main.rand.NextFloat(-5, 5), 0f, 0.1f + 0.03f * i, (int)(30 - i * 1.3f));
                    GeneralParticleHandler.SpawnParticle(explosion);
                }
                for (int i = 0; i < 2; i++)
                {
                    Particle blastRing = new CustomPulse(Projectile.Center, Vector2.Zero, Color.OrangeRed, "CalamityMod/Particles/BloomCircle", Vector2.One, Main.rand.NextFloat(-10, 10), 1f, 3f, 25, true);
                    GeneralParticleHandler.SpawnParticle(blastRing);
                    Particle blastRing2 = new CustomPulse(Projectile.Center, Vector2.Zero, Color.White, "CalamityMod/Particles/BloomCircle", Vector2.One, Main.rand.NextFloat(-10, 10), 0.5f, 1.5f, 25, true);
                    GeneralParticleHandler.SpawnParticle(blastRing2);
                }
                for (int i = 0; i < 20; i++)
                {
                    Dust chargefull = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB);
                    chargefull.velocity = new Vector2(9, 9).RotatedByRandom(100) * Main.rand.NextFloat(0.2f, 2f);
                    chargefull.scale = Main.rand.NextFloat(0.65f, 1.25f);
                    chargefull.noGravity = true;
                    chargefull.color = Color.Lerp(Color.White, Main.rand.NextBool(4) ? Color.Orange : Color.Coral, 0.7f);
                }

                if (Main.myPlayer == Projectile.owner)
                {
                    int explosionDamage = Projectile.damage / 4;
                    float explosionKB = 6f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<VenusianExplosion>(), explosionDamage, explosionKB, Projectile.owner);

                    int cinderDamage = (int)(Projectile.damage * 0.02);
                    float cinderKB = 0f;
                    Vector2 cinderPos = Projectile.Center;
                    int numCinders = 10;
                    for (int i = 0; i < numCinders; i++)
                    {
                        Vector2 cinderVel = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                        while (cinderVel.X == 0f && cinderVel.Y == 0f)
                        {
                            cinderVel = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                        }
                        cinderVel.Normalize();
                        cinderVel *= Main.rand.Next(70, 101) * 0.1f;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), cinderPos, cinderVel + new Vector2(0, -3), ModContent.ProjectileType<VenusianFlame>(), cinderDamage, cinderKB, Projectile.owner);
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], Color.White, 1);
            return true;
        }
    }
}
