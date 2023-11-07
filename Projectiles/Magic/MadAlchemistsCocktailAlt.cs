using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Magic
{
    public class MadAlchemistsCocktailAlt : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
        }

        public override void AI()
        {
            Projectile.rotation += Math.Abs(Projectile.velocity.X) * 0.04f * (float)Projectile.direction;
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= 90f)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.4f;
                Projectile.velocity.X = Projectile.velocity.X * 0.97f;
            }
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int duration = 240;
            target.AddBuff(ModContent.BuffType<CrushDepth>(), duration);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), duration);
            target.AddBuff(ModContent.BuffType<Nightwither>(), duration);
            target.AddBuff(BuffID.Electrified, duration);
            target.AddBuff(BuffID.Venom, duration);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item107, Projectile.Center);

            if (Main.netMode != NetmodeID.Server)
            {
                Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, -Projectile.oldVelocity * 0.2f, 704, 1f);
                Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, -Projectile.oldVelocity * 0.2f, 705, 1f);
            }

            // This previously did double damage. It now does half damage.
            int blastWidth = 120;
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = blastWidth;
            Projectile.Center = Projectile.position;
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.damage /= 2;
            Projectile.Damage();

            // I don't even know what this dust code does
	    // So true bestie! -CIT
            Vector2 dustRotation = (0f - 1.57079637f).ToRotationVector2();
            Vector2 dustVelocity = dustRotation * Projectile.velocity.Length() * (float)Projectile.MaxUpdates;
            int inc;
            for (int i = 0; i < 40; i = inc + 1)
            {
                int alchDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 173, 0f, 0f, 200, default, 2.5f);
                Main.dust[alchDust].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                Main.dust[alchDust].noGravity = true;
                Dust dust = Main.dust[alchDust];
                dust.velocity *= 4f;
                dust = Main.dust[alchDust];
                dust.velocity += dustVelocity * Main.rand.NextFloat();
                alchDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 174, 0f, 0f, 100, default, 1.8f);
                Main.dust[alchDust].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                dust = Main.dust[alchDust];
                dust.velocity *= 3f;
                Main.dust[alchDust].noGravity = true;
                alchDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 229, 0f, 0f, 100, default, 1.8f);
                Main.dust[alchDust].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                dust = Main.dust[alchDust];
                dust.velocity *= 2f;
                Main.dust[alchDust].noGravity = true;
                Main.dust[alchDust].fadeIn = 1f;
                Main.dust[alchDust].color = Color.Green * 0.5f;
                dust = Main.dust[alchDust];
                dust.velocity += dustVelocity * Main.rand.NextFloat();
                inc = i;
            }
            for (int j = 0; j < 20; j = inc + 1)
            {
                int alchDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 206, 0f, 0f, 0, default, 2.5f);
                Main.dust[alchDust2].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)Projectile.velocity.ToRotation(), default) * (float)Projectile.width / 3f;
                Main.dust[alchDust2].noGravity = true;
                Dust dust = Main.dust[alchDust2];
                dust.velocity *= 0.5f;
                dust = Main.dust[alchDust2];
                dust.velocity += dustVelocity * (0.6f + 0.6f * Main.rand.NextFloat());
                inc = j;
            }
        }
    }
}
