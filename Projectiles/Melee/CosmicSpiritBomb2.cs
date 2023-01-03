using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class CosmicSpiritBomb2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bomb");
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 150;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            float num395 = (float)Main.mouseTextColor / 200f - 0.35f;
            num395 *= 0.2f;
            Projectile.scale = num395 + 0.95f;

            float num947 = (Projectile.Center - Main.player[Projectile.owner].Center).Length() / 100f;
            if (num947 <= 2f)
            {
                num947 = 1f;
            }
            else
            {
                if (num947 > 8f)
                {
                    num947 = 12f;
                }
                else if (num947 > 6f)
                {
                    num947 = 9f;
                }
                else if (num947 > 5f)
                {
                    num947 = 7f;
                }
                else if (num947 > 4f)
                {
                    num947 = 5f;
                }
                else if (num947 > 3f)
                {
                    num947 = 4f;
                }
                else if (num947 > 2.5f)
                {
                    num947 = 3f;
                }
                else
                {
                    num947 = 2f;
                }
            }
            Projectile.velocity = Vector2.Normalize(Main.player[Projectile.owner].Center - Projectile.Center) * num947;
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, Projectile.alpha);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 180);
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
            Projectile.width = 200;
            Projectile.height = 200;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            for (int k = 0; k < 10; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 73, Projectile.oldVelocity.X * 2.5f, Projectile.oldVelocity.Y * 2.5f);
            }
        }
    }
}
