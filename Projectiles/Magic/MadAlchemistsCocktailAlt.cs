using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class MadAlchemistsCocktailAlt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mad Alchemist's Prismatic Cocktail");
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
        }

        public override void AI()
        {
            projectile.rotation += Math.Abs(projectile.velocity.X) * 0.04f * (float)projectile.direction;
            projectile.ai[0] += 1f;
            if (projectile.ai[0] >= 90f)
            {
                projectile.velocity.Y = projectile.velocity.Y + 0.4f;
                projectile.velocity.X = projectile.velocity.X * 0.97f;
            }
            if (projectile.velocity.Y > 16f)
            {
                projectile.velocity.Y = 16f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            int duration = 120;
            target.AddBuff(ModContent.BuffType<CrushDepth>(), duration);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), duration);
            target.AddBuff(ModContent.BuffType<Nightwither>(), duration);
            target.AddBuff(BuffID.Poisoned, duration);
            target.AddBuff(BuffID.OnFire, duration);
            target.AddBuff(BuffID.CursedInferno, duration / 2);
            target.AddBuff(BuffID.Frostburn, duration);
            target.AddBuff(BuffID.Venom, duration);
            target.AddBuff(BuffID.ShadowFlame, duration);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item20, projectile.Center);
            Main.PlaySound(SoundID.Item107, projectile.Center);
            Gore.NewGore(projectile.Center, -projectile.oldVelocity * 0.2f, 704, 1f);
            Gore.NewGore(projectile.Center, -projectile.oldVelocity * 0.2f, 705, 1f);

            // This previously did double damage. It now does half damage.
            int blastWidth = 120;
            projectile.position = projectile.Center;
            projectile.width = projectile.height = blastWidth;
            projectile.Center = projectile.position;
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
            projectile.damage /= 2;
            projectile.Damage();

            // I don't even know what this dust code does
            float num51 = 1.8f;
            float num52 = 2.5f;
            Vector2 value3 = (0f - 1.57079637f).ToRotationVector2();
            Vector2 value4 = value3 * projectile.velocity.Length() * (float)projectile.MaxUpdates;
            int num3;
            for (int num53 = 0; num53 < 40; num53 = num3 + 1)
            {
                int num54 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 173, 0f, 0f, 200, default, num52);
                Main.dust[num54].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)projectile.width / 2f;
                Main.dust[num54].noGravity = true;
                Dust dust = Main.dust[num54];
                dust.velocity *= 4f;
                dust = Main.dust[num54];
                dust.velocity += value4 * Main.rand.NextFloat();
                num54 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 174, 0f, 0f, 100, default, num51);
                Main.dust[num54].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)projectile.width / 2f;
                dust = Main.dust[num54];
                dust.velocity *= 3f;
                Main.dust[num54].noGravity = true;
                num54 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 229, 0f, 0f, 100, default, num51);
                Main.dust[num54].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)projectile.width / 2f;
                dust = Main.dust[num54];
                dust.velocity *= 2f;
                Main.dust[num54].noGravity = true;
                Main.dust[num54].fadeIn = 1f;
                Main.dust[num54].color = Color.Green * 0.5f;
                dust = Main.dust[num54];
                dust.velocity += value4 * Main.rand.NextFloat();
                num3 = num53;
            }
            for (int num55 = 0; num55 < 20; num55 = num3 + 1)
            {
                int num56 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 206, 0f, 0f, 0, default, num52);
                Main.dust[num56].position = projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)projectile.velocity.ToRotation(), default) * (float)projectile.width / 3f;
                Main.dust[num56].noGravity = true;
                Dust dust = Main.dust[num56];
                dust.velocity *= 0.5f;
                dust = Main.dust[num56];
                dust.velocity += value4 * (0.6f + 0.6f * Main.rand.NextFloat());
                num3 = num55;
            }
        }
    }
}
