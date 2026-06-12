using System;
using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using CalamityMod.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class NovaChargedShot : ModProjectile, ILocalizedModType
    {
        public static readonly SoundStyle ChargeImpact = new("CalamityMod/Sounds/Item/ArcNovaDiffuserChargeImpact") { Volume = 0.3f };
        public int Time = 0;
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 36;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 20;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 1200;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.extraUpdates = 24;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Time++;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(-3, 3), ModContent.DustType<SquashDust>());
            dust.noGravity = true;
            dust.scale = Main.rand.NextFloat(0.9f, 1.3f);
            dust.color = ArcNovaDiffuser.mainColor;
            dust.fadeIn = -0.4f;
            if (Time < 120)
            {
                if (Main.rand.NextBool())
                {
                    Vector2 trailPos = Projectile.Center + Main.rand.NextVector2Circular(10, 10);
                    float trailScale = Main.rand.NextFloat(0.6f, 0.75f);
                    Color trailColor = Main.rand.NextBool(3) ? Color.Chartreuse : ArcNovaDiffuser.mainColor;
                    Particle Trail = new CustomSpark(trailPos, Projectile.velocity * 0.2f, "CalamityMod/Particles/BloomCircle", false, 50, trailScale, trailColor, new Vector2(0.2f, 1.4f), true, true, shrinkSpeed: 0.1f);
                    GeneralParticleHandler.SpawnParticle(Trail);
                }
                if (Time % 2 == 0)
                {
                    for (int i = -1; i <= 1; i += 2)
                    {
                        float sine = (float)Math.Sin(Projectile.timeLeft * 0.45f / MathHelper.Pi);
                        Dust dust2 = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2) * sine * 28 * i, ModContent.DustType<SquashDust>(), Projectile.velocity * Main.rand.NextFloat(0.6f, 0.8f), 0, default, Main.rand.NextFloat(0.8f, 0.85f));
                        dust2.noGravity = true;
                        dust2.color = Main.rand.NextBool(3) ? Color.Lime : ArcNovaDiffuser.mainColor;
                        dust2.fadeIn = -0.55f;
                    }
                }
            }


        }
        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return ArcNovaDiffuser.mainColor with { A = 0 };
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i <= 19; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SquashDust>(), new Vector2(0, -18).RotatedByRandom(MathHelper.ToRadians(35f)) * Main.rand.NextFloat(0.1f, 1.9f));
                dust.noGravity = false;
                dust.scale = Main.rand.NextFloat(0.8f, 2.3f);
                dust.color = ArcNovaDiffuser.mainColor;
                Dust dust2 = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SquashDust>(), new Vector2(0, -7).RotatedByRandom(MathHelper.ToRadians(35f)) * Main.rand.NextFloat(0.1f, 1.9f));
                dust2.noGravity = false;
                dust2.scale = Main.rand.NextFloat(0.8f, 2.3f);  
                dust2.color = Color.Lime;
            }
            SoundEngine.PlaySound(ChargeImpact, Projectile.Center);
            Particle pulse = new CustomPulse(Projectile.Center, Vector2.Zero, ArcNovaDiffuser.mainColor, "CalamityMod/Particles/BloomRing", new Vector2(1f, 1f), 0, 0.2f, 1.3f, 16);
            GeneralParticleHandler.SpawnParticle(pulse);
            Particle pulse2 = new CustomPulse(Projectile.Center, Vector2.Zero, Color.Lime, "CalamityMod/Particles/BloomCircle", new Vector2(1f, 1f), 0, 0.8f, 0.1f, 20);
            GeneralParticleHandler.SpawnParticle(pulse2);
            for (int i = 0; i < 2; i++)
            {
                Particle spark2 = new CustomSpark(Projectile.Center, Vector2.Zero, "CalamityMod/Particles/BloomCircle", false, 16, 0.95f, ArcNovaDiffuser.mainColor, new Vector2(1f, 1f), true, true, glowOpacity: 0.9f);
                GeneralParticleHandler.SpawnParticle(spark2);
            }

        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
            => modifiers.ApplyScalingForcedCrit(Projectile);
        public override bool? CanDamage() => base.CanDamage();
    }
}
