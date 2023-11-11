using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Healing
{
    public class EssenceFlame : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Healing";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            Projectile.alpha -= 5;
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 16)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }

            Projectile.HealingProjectile((int)Projectile.ai[1], (int)Projectile.ai[0], 3.5f, 15f);
            return;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item74, Projectile.position);
            Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            for (int i = 0; i < 10; i++)
            {
                int godSlay = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 173, 0f, 0f, 100, default, 2f);
                Main.dust[godSlay].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[godSlay].scale = 0.5f;
                    Main.dust[godSlay].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int j = 0; j < 15; j++)
            {
                int godSlay2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 173, 0f, 0f, 100, default, 3f);
                Main.dust[godSlay2].noGravity = true;
                Main.dust[godSlay2].velocity *= 5f;
                godSlay2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 173, 0f, 0f, 100, default, 2f);
                Main.dust[godSlay2].velocity *= 2f;
            }
        }
    }
}
