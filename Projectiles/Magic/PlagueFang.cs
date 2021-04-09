using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Projectiles.Magic
{
    public class PlagueFang : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plague Fang");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.alpha = 255;
            projectile.penetrate = 6;
            projectile.timeLeft = 300;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 50;
            }
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
            if (projectile.alpha == 0)
            {
                int num159 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 163, projectile.velocity.X, projectile.velocity.Y, 100, default, 1f);
                Main.dust[num159].noGravity = true;
                Main.dust[num159].velocity *= 0.6f;
                Main.dust[num159].velocity -= projectile.velocity * 0.4f;

                int num160 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 205, projectile.velocity.X, projectile.velocity.Y, 100, default, 1f);
                Main.dust[num160].noGravity = true;
                Main.dust[num160].velocity *= 0.2f;
                Main.dust[num160].velocity -= projectile.velocity * 0.4f;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.alpha > 0)
            {
                return Color.Transparent;
            }
            return null;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Dig, (int)projectile.position.X, (int)projectile.position.Y, 27);
            for (int num301 = 0; num301 < 7; num301++)
            {
                int num302 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 163, 0f, 0f, 100, default, 1f);
                Main.dust[num302].noGravity = true;
                Main.dust[num302].velocity *= 1.2f;
                Main.dust[num302].velocity -= projectile.oldVelocity * 0.3f;

                int num402 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 205, 0f, 0f, 100, default, 1f);
                Dust dust = Main.dust[num402];
                dust.noGravity = true;
                dust.velocity *= 1.2f;
                dust.velocity -= projectile.oldVelocity * 0.3f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 300);
        }
    }
}
