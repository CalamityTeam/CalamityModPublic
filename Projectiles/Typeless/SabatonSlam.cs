using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class SabatonSlam : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 35;
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
            Player player = Main.player[projectile.owner];
            //Spawns the shockwave
            if (Main.myPlayer == projectile.owner)
            {
                Projectile.NewProjectile(projectile.position.X + 25, projectile.position.Y + 25, 0f, 0f, ModContent.ProjectileType<SabatonBoom>(), (int)(300 * player.AverageDamage()), 12, projectile.owner);
                Main.PlaySound(SoundID.Item14, projectile.position);
                player.Calamity().gSabatonFall = 0;
                projectile.Kill();
            }
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
