using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Healing
{
    public class PhoenixHeal : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heal");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 420;
            projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            projectile.velocity.X *= 0.95f;
            projectile.velocity.Y *= 0.95f;
            projectile.HealingProjectile(10, (int)projectile.ai[0], 6f, 15f, false, 360);
            float num498 = projectile.velocity.X * 0.2f;
            float num499 = -(projectile.velocity.Y * 0.2f);
            int num500 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 100, default, 1f);
            Dust dust = Main.dust[num500];
            dust.noGravity = true;
            dust.position.X -= num498;
            dust.position.Y -= num499;
        }
    }
}
