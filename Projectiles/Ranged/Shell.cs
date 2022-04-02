using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class Shell : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shell");
        }

        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 18;
            projectile.ignoreWater = true;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 5;
            projectile.aiStyle = 1;
            projectile.arrow = true;
            projectile.timeLeft = 600;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void AI()
        {
            projectile.velocity *= 0.9995f;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item10, projectile.position);
            projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
            projectile.width = 34;
            projectile.height = 18;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            for (int num621 = 0; num621 < 10; num621++)
            {
                int num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 14, projectile.oldVelocity.X / 4, projectile.oldVelocity.Y / 4, 0, new Color(0, 255, 255), 1.5f);
                Main.dust[num195].noGravity = true;
                Main.dust[num195].velocity *= 3f;
            }
        }
    }
}
