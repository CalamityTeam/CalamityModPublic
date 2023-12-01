using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class MadAlchemistsCocktailRed : ModProjectile, ILocalizedModType
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

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item107, Projectile.Center);

            if (Main.netMode != NetmodeID.Server)
            {
                Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, -Projectile.oldVelocity * 0.2f, 704, 1f);
                Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, -Projectile.oldVelocity * 0.2f, 705, 1f);
            }

            // This previously had a width of 480 pixels and did triple damage.
            // It now has a width of 110 pixels and deals 1/2 damage.
            int blastWidth = 110;
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
            float dustAI = 2.4f;
            Vector2 dustRotation = (0f - 1.57079637f).ToRotationVector2();
            Vector2 dustVelocity = dustRotation * Projectile.velocity.Length() * (float)Projectile.MaxUpdates;
            int inc;
            for (int i = 0; i < 60; i = inc + 1)
            {
                int fiery = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 174, 0f, 0f, 200, default, 3f);
                Main.dust[fiery].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                Main.dust[fiery].noGravity = true;
                Dust dust = Main.dust[fiery];
                dust.velocity *= 8f;
                dust = Main.dust[fiery];
                dust.velocity += dustVelocity * Main.rand.NextFloat();
                fiery = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 174, 0f, 0f, 100, default, dustAI);
                Main.dust[fiery].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                dust = Main.dust[fiery];
                dust.velocity *= 6f;
                Main.dust[fiery].noGravity = true;
                Main.dust[fiery].fadeIn = 1f;
                Main.dust[fiery].color = Color.Crimson * 0.5f;
                dust = Main.dust[fiery];
                dust.velocity += dustVelocity * Main.rand.NextFloat();
                fiery = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 174, 0f, 0f, 100, default, dustAI);
                Main.dust[fiery].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                dust = Main.dust[fiery];
                dust.velocity *= 4f;
                Main.dust[fiery].noGravity = true;
                Main.dust[fiery].fadeIn = 1f;
                Main.dust[fiery].color = Color.Crimson * 0.5f;
                dust = Main.dust[fiery];
                dust.velocity += dustVelocity * Main.rand.NextFloat();
                fiery = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 174, 0f, 0f, 100, default, dustAI);
                Main.dust[fiery].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                dust = Main.dust[fiery];
                dust.velocity *= 2f;
                Main.dust[fiery].noGravity = true;
                Main.dust[fiery].fadeIn = 1f;
                Main.dust[fiery].color = Color.Crimson * 0.5f;
                dust = Main.dust[fiery];
                dust.velocity += dustVelocity * Main.rand.NextFloat();
                inc = i;
            }
            for (int j = 0; j < 30; j = inc + 1)
            {
                int fiery2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 174, 0f, 0f, 0, default, 3.8f);
                Main.dust[fiery2].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)Projectile.velocity.ToRotation(), default) * (float)Projectile.width / 3f;
                Main.dust[fiery2].noGravity = true;
                Dust dust = Main.dust[fiery2];
                dust.velocity *= 0.5f;
                dust = Main.dust[fiery2];
                dust.velocity += dustVelocity * (0.6f + 0.6f * Main.rand.NextFloat());
                inc = j;
            }
        }
    }
}
