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
            projectile.width = 4;
            projectile.height = 40;
            projectile.hostile = true;
            projectile.extraUpdates = 1;
            projectile.penetrate = -1;
            projectile.ignoreWater = true;
            projectile.timeLeft = 300;
            projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
        }

        public override void AI()
        {
            projectile.alpha = 50;
        }

        public override void Kill(int timeLeft)
        {
            int num310 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + (float)projectile.height - 2f), 2, 2, 14, 0f, 0f, 0, default, 1f);
            Dust dust = Main.dust[num310];
            dust.position.X -= 2f;
            dust.alpha = 38;
            dust.velocity *= 0.1f;
            dust.velocity += -projectile.oldVelocity * 0.25f;
            dust.scale = 0.95f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(102, 255, 102, projectile.alpha);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Shadowflame>(), 60);
        }
    }
}
