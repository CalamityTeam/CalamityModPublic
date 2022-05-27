using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class ShaderainHostile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shade Rain");
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 40;
            Projectile.hostile = true;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Projectile.alpha = 50;
        }

        public override void Kill(int timeLeft)
        {
            int num310 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y + (float)Projectile.height - 2f), 2, 2, 14, 0f, 0f, 0, default, 1f);
            Dust dust = Main.dust[num310];
            dust.position.X -= 2f;
            dust.alpha = 38;
            dust.velocity *= 0.1f;
            dust.velocity += -Projectile.oldVelocity * 0.25f;
            dust.scale = 0.95f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(102, 255, 102, Projectile.alpha);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Shadowflame>(), 120);
        }
    }
}
