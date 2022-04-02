using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class RoxSlam : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 35;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.timeLeft = 400;
            projectile.tileCollide = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            //Kills the projectile if the alt attack ended
            if (player.itemAnimation == 0)
            {
                projectile.Kill();
            }
            //Makes the projectiles track the player
            float posX;
            float posY = player.Center.Y + 63;
            // Makes the projectile follow the proper position on the X axis
            if (player.direction == 1)
            {
                posX = player.Center.X + 6;
            }
            else
            {
                posX = player.Center.X - 8;
            }
            projectile.Center = new Vector2(posX, posY - (player.height / 10f));
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //This will only happen if the cooldown was full
            if (projectile.ai[0] == 1 && Main.myPlayer == projectile.owner)
            {
                Player player = Main.player[projectile.owner];
                //Bounce
                player.velocity.Y = -18f;
                //reset player fall damage
                player.fallStart = (int)(player.position.Y / 16f);
                //Spawns the shockwave
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<RoxShockwave>(), 300, 12, projectile.owner);
                Main.PlaySound(SoundID.Item14, projectile.position);
                projectile.Kill();
                //Pretty things
                for (int dustexplode = 0; dustexplode < 360; dustexplode++)
                {
                    Vector2 dustd = new Vector2(17f, 17f).RotatedBy(MathHelper.ToRadians(dustexplode));
                    int d = Dust.NewDust(projectile.Center, projectile.width, projectile.height, 51, dustd.X, dustd.Y, 100, default, 3f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].position = projectile.Center;
                }
                return false;
            }
            //If the cooldown wasnt full, just bounce
            else if (Main.myPlayer == projectile.owner)
            {
                Player player = Main.player[projectile.owner];
                player.velocity.Y = -14f;
                //reset player fall damage
                player.fallStart = (int)(player.position.Y / 16f);
                Main.PlaySound(SoundID.Tink, projectile.position);
                projectile.Kill();

                return false;
            }
            return false;
        }
    }
}
