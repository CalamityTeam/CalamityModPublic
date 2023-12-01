using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Ranged
{
    public class DaemonsFlameArrow : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public int x;

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = 5;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Projectile.rotation += 0.15f;
            Lighting.AddLight(Projectile.Center, new Vector3(245, 124, 110) * (1.7f / 255));
            x++;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 30f)
            {
                Projectile.velocity.Y = (float)(((double)Projectile.ai[1]) * Math.Sin(x / 4));
            }
            for (int i = 0; i < 2; i++)
            {
                int dusty = Dust.NewDust(new Vector2(Projectile.Center.X, Projectile.Center.Y), Projectile.width - 28, Projectile.height - 28, 60, 0f, 0f, 100, default, 1.6f);
                Main.dust[dusty].noGravity = true;
                Main.dust[dusty].velocity *= 0.1f;
                Main.dust[dusty].velocity += Projectile.velocity * 0.5f;
            }
            if (Main.rand.NextBool(13))
            {
                int dusty2 = Dust.NewDust(new Vector2(Projectile.Center.X, Projectile.Center.Y), Projectile.width - 32, Projectile.height - 32, 60, 0f, 0f, 100, default, 1f);
                Main.dust[dusty2].velocity *= 0.25f;
                Main.dust[dusty2].velocity += Projectile.velocity * 0.5f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            // Massively inflate the projectile's hitbox
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 160;
            Projectile.position.X = Projectile.position.X - Projectile.width / 2;
            Projectile.position.Y = Projectile.position.Y - Projectile.height / 2;

            // Allow infinite piercing and completely ignoring iframes for this one last hit
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = false;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 0;
            Projectile.Damage();

            for (int i = 0; i < 10; i++)
            {
                int dust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, 0f, 0f, 100, default, 1.2f);
                Main.dust[dust].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[dust].scale = 0.5f;
                    Main.dust[dust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int j = 0; j < 20; j++)
            {
                int dust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, 0f, 0f, 100, default, 1.7f);
                Main.dust[dust2].noGravity = true;
                Main.dust[dust2].velocity *= 5f;
                dust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, 0f, 0f, 100, default, 1f);
                Main.dust[dust2].velocity *= 2f;
            }
        }
    }
}
