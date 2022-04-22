using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Typeless
{
    public class SabatonSlam : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 35;
            Projectile.friendly = true;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            Player player = Main.player[Main.myPlayer];
            if (player.Calamity().gSabatonFall == 0)
            {
                Projectile.Kill();
            }
            //Makes the projectiles follow the player
            Projectile.velocity.X = player.velocity.X;
            Projectile.velocity.Y = player.velocity.Y;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Player player = Main.player[Projectile.owner];
            //Spawns the shockwave
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + 25, Projectile.position.Y + 25, 0f, 0f, ModContent.ProjectileType<SabatonBoom>(), (int)(300 * player.AverageDamage()), 12, Projectile.owner);
                SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
                player.Calamity().gSabatonFall = 0;
                Projectile.Kill();
            }
            //Pretty things
            for (int dustexplode = 0; dustexplode < 360; dustexplode++)
            {
                Vector2 dustd = new Vector2(17f, 17f).RotatedBy(MathHelper.ToRadians(dustexplode));
                int d = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, Main.rand.NextBool(2) ? ModContent.DustType<AstralBlue>() : ModContent.DustType<AstralOrange>(), dustd.X, dustd.Y, 100, default, 3f);
                Main.dust[d].noGravity = true;
                Main.dust[d].position = Projectile.Center;
            }
            return false;
        }
    }
}
