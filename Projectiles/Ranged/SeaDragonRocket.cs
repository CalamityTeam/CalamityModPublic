using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Ranged
{
    public class SeaDragonRocket : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 95;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void AI()
        {
            if (Projectile.ai[1] == 0f)
            {
                for (int i = 0; i < 5; i++)
                {
                    int fire = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 2f);
                    Main.dust[fire].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[fire].scale = 0.5f;
                        Main.dust[fire].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                Projectile.ai[1] = 1f;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 3)
            {
                Projectile.tileCollide = false;
                Projectile.ai[1] = 0f;
                Projectile.alpha = 255;
                Projectile.position.X = Projectile.Center.X;
                Projectile.position.Y = Projectile.Center.Y;
                Projectile.width = 200;
                Projectile.height = 200;
                Projectile.position.X -= (float)(Projectile.width / 2);
                Projectile.position.Y -= (float)(Projectile.height / 2);
                Projectile.knockBack = 10f;
            }
            else
            {
                if (Math.Abs(Projectile.velocity.X) >= 8f || Math.Abs(Projectile.velocity.Y) >= 8f)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        float halfX = 0f;
                        float halfY = 0f;
                        if (j == 1)
                        {
                            halfX = Projectile.velocity.X * 0.5f;
                            halfY = Projectile.velocity.Y * 0.5f;
                        }
                        int explosion = Dust.NewDust(new Vector2(Projectile.position.X + 3f + halfX, Projectile.position.Y + 3f + halfY) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, 6, 0f, 0f, 100, default, 1f);
                        Main.dust[explosion].scale *= 2f + (float)Main.rand.Next(10) * 0.1f;
                        Main.dust[explosion].velocity *= 0.2f;
                        Main.dust[explosion].noGravity = true;
                        explosion = Dust.NewDust(new Vector2(Projectile.position.X + 3f + halfX, Projectile.position.Y + 3f + halfY) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, 33, 0f, 0f, 100, default, 0.5f);
                        Main.dust[explosion].fadeIn = 1f + (float)Main.rand.Next(5) * 0.1f;
                        Main.dust[explosion].velocity *= 0.05f;
                    }
                }
            }
            CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 200f, 12f, 20f);
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.ExpandHitboxBy(192);
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
            SoundEngine.PlaySound(SoundID.Item110, Projectile.position);
            for (int i = 0; i < 15; i++)
            {
                int smoke = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 33, 0f, 0f, 100, default, 2f);
                Main.dust[smoke].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[smoke].scale = 0.5f;
                    Main.dust[smoke].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int j = 0; j < 25; j++)
            {
                int fire = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 3f);
                Main.dust[fire].noGravity = true;
                Main.dust[fire].velocity *= 5f;
                fire = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 2f);
                Main.dust[fire].velocity *= 2f;
            }
            int projAmt = Main.rand.Next(2, 4);
            if (Projectile.owner == Main.myPlayer)
            {
                for (int k = 0; k < projAmt; k++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<RocketFire>(), (int)(Projectile.damage * 0.33), 0f, Projectile.owner);
                }
            }
        }
    }
}
