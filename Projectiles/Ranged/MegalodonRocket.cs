using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.NPCs.SunkenSea;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class MegalodonRocket : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public static int Lifetime = 600;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 38;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = 600;
            AIType = ProjectileID.Bullet;
        }

        public override void AI()
        {
            //Rotation
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi) * Projectile.direction;

            Lighting.AddLight(Projectile.Center, 0.3f, 0.5f, 0.1f);
            if (Projectile.timeLeft <= Lifetime - 6)
            {
                AltSparkParticle spark = new AltSparkParticle(Projectile.Center, -Projectile.velocity * 0.05f, false, 15, 1f, Color.Gold * 0.1f);
                GeneralParticleHandler.SpawnParticle(spark);
                if (Main.rand.NextBool(3))
                {
                    SparkParticle spark2 = new SparkParticle(Projectile.Center + Main.rand.NextVector2Circular(15, 15) - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 10, -Projectile.velocity * Main.rand.NextFloat(0.5f, 1.5f), false, Main.rand.Next(9, 12 + 1), 0.4f, Color.OrangeRed);
                    GeneralParticleHandler.SpawnParticle(spark2);
                }
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, Projectile.velocity, 500, default, 0.5f);
                dust.scale = Main.rand.NextFloat(0.7f, 1.5f);
                dust.velocity = Projectile.velocity * Main.rand.NextFloat(-3, 3);
                dust.noGravity = true;
                Dust dust2 = Dust.NewDustPerfect(Projectile.Center, DustID.Smoke, Projectile.velocity, 500, default, 0.5f);
                dust2.scale = Main.rand.NextFloat(0.7f, 1.5f);
                dust2.velocity = Projectile.velocity * Main.rand.NextFloat(-3, 3);
                dust2.noGravity = true;
            }
            CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 450f, 12f, 20f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 240);
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.zenithWorld)
            {
                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/MegalodonMissileMax") with { Volume = 1.5f }, Projectile.position);
            }
            else
            {
                SoundEngine.PlaySound(AbyssGravel.MineSound, Projectile.position);
                SoundEngine.PlaySound(GiantClam.SlamSound, Projectile.position);
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode.WithPitchOffset(0.5f), Projectile.position);
                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/FlakKrakenShoot") with { Volume = 0.6f, Pitch = 0.8f }, Projectile.position);
            }

            GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, Color.Blue, "CalamityMod/Particles/LargeBloom", Vector2.One, 0f, 1f, 0f, 25));
            GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, Color.White, "CalamityMod/Particles/LargeBloom", Vector2.One, 0f, 0.5f, 0f, 15));

            for (int i = 0; i < 8; i++) GeneralParticleHandler.SpawnParticle(new BloodParticle2(Projectile.Center, new Vector2(Main.rand.NextFloat(6, 12), 0).RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)), 12, Main.rand.NextFloat(0.3f, 0.4f), Color.DarkGoldenrod * 0.8f));
            Particle pulse = new CustomPulse(Projectile.Center, Vector2.Zero, Color.RoyalBlue * 0.5f, "CalamityMod/Particles/FlameExplosion", Vector2.One, Main.rand.NextFloat(-10f, 10f), 0f, 0.1f, 15);
            GeneralParticleHandler.SpawnParticle(pulse);
            Particle pulse2 = new CustomPulse(Projectile.Center, Vector2.Zero, Color.CornflowerBlue, "CalamityMod/Particles/FlameExplosion2", Vector2.One, Main.rand.NextFloat(-10f, 10f), 0f, 0.07f, 15);
            GeneralParticleHandler.SpawnParticle(pulse2);
            for (int i = 0; i < 10; i++)
            {
                Vector2 randVel = new Vector2(15, 15).RotatedByRandom(100) * Main.rand.NextFloat(0.3f, 0.5f);
                Particle smoke = new HeavySmokeParticle(Projectile.Center + randVel, randVel, new Color(57, 46, 115) * 0.9f, Main.rand.Next(25, 35 + 1), Main.rand.NextFloat(0.6f, 1.5f), 0.4f);
                GeneralParticleHandler.SpawnParticle(smoke);
            }
        }
    }
}
