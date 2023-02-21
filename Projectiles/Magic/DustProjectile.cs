using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.DataStructures;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class DustProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dust");
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 4;
            Projectile.MaxUpdates = 4;
            Projectile.timeLeft = 30 * Projectile.MaxUpdates; // 30 effectively, 120 total
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10 * Projectile.MaxUpdates; // 10 effective, 40 total
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.5f, 0.4f, 0.01f);
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;
            if (Projectile.ai[0] > 7f)
            {
                int dustType = 22;
                int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, default, 1f);
                Dust dust = Main.dust[idx];
                if (Main.rand.NextBool(3))
                {
                    dust.noGravity = true;
                    dust.scale *= 2f;
                    dust.velocity.X *= 3f;
                    dust.velocity.Y *= 3f;
                }
                dust.velocity.X *= 1.5f;
                dust.velocity.Y *= 1.5f;
            }
            Projectile.ai[0] += 1f;
            Projectile.rotation += 0.3f * (float)Projectile.direction;
            if (Projectile.timeLeft < 5)
                Projectile.Opacity = MathHelper.Lerp(0f, 1f, (float)Projectile.timeLeft / 5f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 180);

        public override void OnHitPvp(Player target, int damage, bool crit) => target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 180);

        public override bool? CanDamage() => Projectile.timeLeft > 5;

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);

            // Dust effects
            Circle dustCircle = new Circle(Projectile.Center, Projectile.width / 2);

            for (int i = 0; i < 20; i++)
            {
                // Dust
                Vector2 dustPos = dustCircle.RandomPointInCircle();
                if ((dustPos - Projectile.Center).Length() > 48)
                {
                    int dustIndex = Dust.NewDust(dustPos, 1, 1, 22);
                    Main.dust[dustIndex].noGravity = true;
                    Main.dust[dustIndex].fadeIn = 1f;
                    Vector2 dustVelocity = Projectile.Center - Main.dust[dustIndex].position;
                    float distToCenter = dustVelocity.Length();
                    dustVelocity.Normalize();
                    dustVelocity = dustVelocity.RotatedBy(MathHelper.ToRadians(-90f));
                    dustVelocity *= distToCenter * 0.04f;
                    Main.dust[dustIndex].velocity = dustVelocity;
                }
            }
            return false;
        }
    }
}
