using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class SeafoamBubble : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Typeless/CoralBubble";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seafoam Bubble");
        }

        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 28;
            projectile.friendly = true;
            projectile.penetrate = 3;
            projectile.timeLeft = 180;
            projectile.tileCollide = false;
            projectile.alpha = 255;
            projectile.scale = 1f;
            projectile.localNPCHitCooldown = 30;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.ai[0] += 1f;
            if (projectile.alpha < 50)
            {
                projectile.alpha = 50;
            }
            else if (projectile.alpha > 50)
            {
                projectile.alpha -= 10;
            }
            projectile.scale += 0.002f;
            if (projectile.ai[0] % 60 == 0)
            {
                projectile.damage *= 2;
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item54, projectile.position);
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 60;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            int num3;
            for (int num246 = 0; num246 < 25; num246 = num3 + 1)
            {
                int num247 = Dust.NewDust(projectile.Center, projectile.width, projectile.height, 154, 0f, 0f, 0, default, 1f);
                Main.dust[num247].position = (Main.dust[num247].position + projectile.position) / 2f;
                Main.dust[num247].velocity = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                Main.dust[num247].velocity.Normalize();
                Dust dust = Main.dust[num247];
                dust.velocity *= (float)Main.rand.Next(1, 30) * 0.1f;
                Main.dust[num247].alpha = projectile.alpha;
                num3 = num246;
            }
        }
    }
}
