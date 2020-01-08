using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Dusts;
namespace CalamityMod.Projectiles.Typeless
{
    public class SabatonSlam : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 50;
            projectile.friendly = true;
            projectile.timeLeft = 300;
            projectile.tileCollide = true;
        }

        public override void AI()
        {
            Player player = Main.player[Main.myPlayer];
            if (player.Calamity().gSabatonFall == 0)
            {
                projectile.Kill();
            }
            //Makes the projectiles follow the player
            projectile.velocity.X = player.velocity.X;
            projectile.velocity.Y = player.velocity.Y;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			//Spawns the shockwave
			Projectile.NewProjectile(projectile.position.X + 25, projectile.position.Y + 25, 0f, 0f, ModContent.ProjectileType<SabatonBoom>(), 300, 12, projectile.owner);
			Main.PlaySound(2, projectile.position, 14);
            Player player = Main.player[Main.myPlayer];
            player.Calamity().gSabatonFall = 0;
			projectile.Kill();
			//Pretty things
			for (int dustexplode = 0; dustexplode < 360; dustexplode++)
			{
				Vector2 dustd = new Vector2(17f, 17f).RotatedBy(MathHelper.ToRadians(dustexplode));
				int d = Dust.NewDust(projectile.Center, projectile.width, projectile.height, Main.rand.NextBool(2) ? ModContent.DustType<AstralBlue>() : ModContent.DustType<AstralOrange>(), dustd.X, dustd.Y, 100, default, 3f);
				Main.dust[d].noGravity = true;
				Main.dust[d].position = projectile.Center;
			}
			return false;
        }
    }
}
