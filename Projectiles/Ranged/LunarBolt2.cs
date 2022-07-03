using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Ranged
{
    public class LunarBolt2 : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Ranged/LunarBolt";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bolt");
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 180;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
        }

        public override void AI()
        {
            //Rotation
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);

            if (Projectile.alpha < 170)
            {
                for (int d = 0; d < 5; d++)
                {
                    Vector2 dspeed = -Projectile.velocity * 0.5f;
                    int index = Dust.NewDust(Projectile.Center, 1, 1, 206, 0f, 0f, 0, default, 1.2f);
                    Main.dust[index].alpha = Projectile.alpha;
                    Main.dust[index].velocity = dspeed;
                    Main.dust[index].noGravity = true;
                }
                for (int d = 0; d < 5; d++)
                {
                    Vector2 dspeed2 = -Projectile.velocity * 0.5f;
                    int index = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 107, 0f, 0f, 0, default, 0.7f);
                    Main.dust[index].alpha = Projectile.alpha;
                    Main.dust[index].velocity = dspeed2;
                    Main.dust[index].noGravity = true;
                }
            }
            if (Projectile.alpha > 50)
            {
                Projectile.alpha -= 25;
            }
            if (Projectile.alpha < 50)
            {
                Projectile.alpha = 50;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.ai[0] == 0f)
            {
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
                Projectile.ai[0]++;
            }
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            Color color = new Color(168, 247, 193);
            return color;
        }
    }
}
