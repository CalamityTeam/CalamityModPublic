using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class BloodGeyser : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood Geyser");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 420;
            projectile.penetrate = 1;
        }

        public override void AI()
        {
            if (projectile.position.Y > projectile.ai[1])
                projectile.tileCollide = true;

            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;

            int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.Blood, 0f, 0f, 100, default, 0.5f);
            Main.dust[num469].noGravity = true;
            Main.dust[num469].velocity *= 0f;

            projectile.velocity.Y += 0.06f;
            projectile.velocity.X *= 0.995f;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BurningBlood>(), 120);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(250, 50, 50, projectile.alpha);
        }
    }
}
