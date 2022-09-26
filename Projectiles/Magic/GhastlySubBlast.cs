using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class GhastlySubBlast : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ghast Blast");
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 420;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
        }

        public override void AI()
        {
            bool flag60 = false;
            bool flag61 = false;
            int num3 = Projectile.type;
            int num1024 = ModContent.ProjectileType<GhastlyBlast>();
            float num1025 = 420f;
            float x3 = 0.15f;
            float y3 = 0.15f;
            if (flag61)
            {
                int num1026 = (int)Projectile.ai[1];
                if (!Main.projectile[num1026].active || Main.projectile[num1026].type != num1024)
                {
                    Projectile.Kill();
                    return;
                }
                Projectile.timeLeft = 2;
            }
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] < num1025)
            {
                bool flag62 = true;
                int num1027 = (int)Projectile.ai[1];
                if (Main.projectile[num1027].active && Main.projectile[num1027].type == num1024)
                {
                    if (!flag60 && Main.projectile[num1027].oldPos[1] != Vector2.Zero)
                    {
                        Projectile.position += Main.projectile[num1027].position - Main.projectile[num1027].oldPos[1];
                    }
                    if (Projectile.Center.HasNaNs())
                    {
                        Projectile.Kill();
                        return;
                    }
                }
                else
                {
                    Projectile.ai[0] = num1025;
                    flag62 = false;
                    Projectile.Kill();
                }
                if (flag62 && !flag60)
                {
                    Projectile.velocity += new Vector2((float)Math.Sign(Main.projectile[num1027].Center.X - Projectile.Center.X), (float)Math.Sign(Main.projectile[num1027].Center.Y - Projectile.Center.Y)) * new Vector2(x3, y3);
                    if (Projectile.velocity.Length() > 6f)
                    {
                        Projectile.velocity *= 6f / Projectile.velocity.Length();
                    }
                }
                if (Main.rand.NextBool(2))
                {
                    int num1028 = Dust.NewDust(Projectile.Center, 8, 8, 60, 0f, 0f, 0, default, 1f);
                    Main.dust[num1028].position = Projectile.Center;
                    Main.dust[num1028].velocity = Projectile.velocity;
                    Main.dust[num1028].noGravity = true;
                    Main.dust[num1028].scale = 1.5f;
                    if (flag62)
                    {
                        Main.dust[num1028].customData = Main.projectile[(int)Projectile.ai[1]];
                    }
                }
                Projectile.alpha = 255;
                return;
            }
        }

        public override void Kill(int timeLeft)
        {
            Projectile.ai[0] = 60f;
            for (int num114 = 0; num114 < 10; num114++)
            {
                int num115 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, (int)Projectile.ai[0], Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 0, default, 0.5f);
                Dust dust;
                Main.dust[num115].scale = 1f + (float)Main.rand.Next(-10, 11) * 0.01f;
                Main.dust[num115].noGravity = true;
                dust = Main.dust[num115];
                dust.velocity *= 1.25f;
                dust = Main.dust[num115];
                dust.velocity -= Projectile.oldVelocity / 10f;
            }
        }
    }
}
