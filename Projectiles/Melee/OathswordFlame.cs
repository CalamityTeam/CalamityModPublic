using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class OathswordFlame : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flame");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.tileCollide = true;
            projectile.timeLeft = 240;
            projectile.Opacity = 0f;
            projectile.melee = true;
        }

        public override void AI()
        {
            projectile.Opacity = MathHelper.Clamp(projectile.Opacity + 0.08f, 0f, 1f);
            CalamityGlobalProjectile.HomeInOnNPC(projectile, false, 700f, 15f, 10f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, projectile.alpha) * projectile.Opacity;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 180);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, projectile.Center);

            CalamityGlobalProjectile.ExpandHitboxBy(projectile, 100);
            for (int k = 0; k < 10; k++)
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 73, projectile.oldVelocity.X * 2.5f, projectile.oldVelocity.Y * 2.5f);
        }
    }
}
