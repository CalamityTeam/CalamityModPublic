using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class CosmicSpiritBomb3 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bomb");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.timeLeft = 150;
            projectile.melee = true;
        }

        public override void AI()
        {
            float num395 = (float)Main.mouseTextColor / 200f - 0.35f;
            num395 *= 0.2f;
            projectile.scale = num395 + 0.95f;

            float num947 = (projectile.Center - Main.player[projectile.owner].Center).Length() / 100f;
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
            projectile.velocity = Vector2.Normalize(Main.player[projectile.owner].Center - projectile.Center) * num947;
            projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.01f * (float)projectile.direction;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, projectile.alpha);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Ichor, 60);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, (int)projectile.position.X, (int)projectile.position.Y);
            projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
            projectile.width = 200;
            projectile.height = 200;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            for (int k = 0; k < 10; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 244, projectile.oldVelocity.X * 2.5f, projectile.oldVelocity.Y * 2.5f);
            }
        }
    }
}
