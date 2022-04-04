using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class RoxSlam : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 35;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 400;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            //Kills the projectile if the alt attack ended
            if (player.itemAnimation == 0)
            {
                Projectile.Kill();
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
            Projectile.Center = new Vector2(posX, posY - (player.height / 10f));
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //This will only happen if the cooldown was full
            if (Projectile.ai[0] == 1 && Main.myPlayer == Projectile.owner)
            {
                Player player = Main.player[Projectile.owner];
                //Bounce
                player.velocity.Y = -18f;
                //reset player fall damage
                player.fallStart = (int)(player.position.Y / 16f);
                //Spawns the shockwave
                Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<RoxShockwave>(), 300, 12, Projectile.owner);
                SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
                Projectile.Kill();
                //Pretty things
                for (int dustexplode = 0; dustexplode < 360; dustexplode++)
                {
                    Vector2 dustd = new Vector2(17f, 17f).RotatedBy(MathHelper.ToRadians(dustexplode));
                    int d = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 51, dustd.X, dustd.Y, 100, default, 3f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].position = Projectile.Center;
                }
                return false;
            }
            //If the cooldown wasnt full, just bounce
            else if (Main.myPlayer == Projectile.owner)
            {
                Player player = Main.player[Projectile.owner];
                player.velocity.Y = -14f;
                //reset player fall damage
                player.fallStart = (int)(player.position.Y / 16f);
                SoundEngine.PlaySound(SoundID.Tink, Projectile.position);
                Projectile.Kill();

                return false;
            }
            return false;
        }
    }
}
