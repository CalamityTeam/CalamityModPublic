using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class SepticSkewerBacteria : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bacteria");
        }

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void AI()
        {
            int num297 = 171;
            if (Main.rand.Next(3) == 0)
            {
                num297 = 46;
            }
            int num469 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, num297, 0f, 0f, 100, default, 2f);
            Main.dust[num469].noGravity = true;
            float num944 = 1f - (float)Projectile.alpha / 255f;
            num944 *= Projectile.scale;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= 90f)
            {
                Projectile.localAI[0] *= -1f;
            }
            if (Projectile.localAI[0] >= 0f)
            {
                Projectile.scale += 0.003f;
            }
            else
            {
                Projectile.scale -= 0.003f;
            }
            Projectile.rotation += 0.0025f * Projectile.scale;
            float num945 = 1f;
            float num946 = 1f;
            if (Projectile.identity % 6 == 0)
            {
                num946 *= -1f;
            }
            if (Projectile.identity % 6 == 1)
            {
                num945 *= -1f;
            }
            if (Projectile.identity % 6 == 2)
            {
                num946 *= -1f;
                num945 *= -1f;
            }
            if (Projectile.identity % 6 == 3)
            {
                num946 = 0f;
            }
            if (Projectile.identity % 6 == 4)
            {
                num945 = 0f;
            }
            Projectile.localAI[1] += 1f;
            if (Projectile.localAI[1] > 60f)
            {
                Projectile.localAI[1] = -180f;
            }
            if (Projectile.localAI[1] >= -60f)
            {
                Projectile.velocity.X = Projectile.velocity.X + 0.002f * num946;
                Projectile.velocity.Y = Projectile.velocity.Y + 0.002f * num945;
            }
            else
            {
                Projectile.velocity.X = Projectile.velocity.X - 0.002f * num946;
                Projectile.velocity.Y = Projectile.velocity.Y - 0.002f * num945;
            }
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 5400f)
            {
                Projectile.damage = 0;
                Projectile.ai[1] = 1f;
                if (Projectile.alpha < 255)
                {
                    Projectile.alpha += 5;
                    if (Projectile.alpha > 255)
                    {
                        Projectile.alpha = 255;
                    }
                }
                else if (Projectile.owner == Main.myPlayer)
                {
                    Projectile.Kill();
                }
            }
            else
            {
                float num947 = (Projectile.Center - Main.player[Projectile.owner].Center).Length() / 100f;
                if (num947 > 4f)
                {
                    num947 *= 1.1f;
                }
                if (num947 > 5f)
                {
                    num947 *= 1.2f;
                }
                if (num947 > 6f)
                {
                    num947 *= 1.3f;
                }
                if (num947 > 7f)
                {
                    num947 *= 1.4f;
                }
                if (num947 > 8f)
                {
                    num947 *= 1.5f;
                }
                if (num947 > 9f)
                {
                    num947 *= 1.6f;
                }
                if (num947 > 10f)
                {
                    num947 *= 1.7f;
                }
                Projectile.ai[0] += num947;
                if (Projectile.alpha > 50)
                {
                    Projectile.alpha -= 10;
                    if (Projectile.alpha < 50)
                    {
                        Projectile.alpha = 50;
                    }
                }
            }
            if ((double)Projectile.velocity.Length() > 0.2)
            {
                Projectile.velocity *= 0.98f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Venom, 120);
        }

        public override void Kill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 56;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            int num226 = 36;
            for (int num227 = 0; num227 < num226; num227++)
            {
                Vector2 vector6 = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + Projectile.Center;
                Vector2 vector7 = vector6 - Projectile.Center;
                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 171, vector7.X, vector7.Y, 100, default, 0.5f);
                Main.dust[num228].noGravity = true;
                Main.dust[num228].noLight = true;
                Main.dust[num228].velocity = vector7;
            }
        }
    }
}
