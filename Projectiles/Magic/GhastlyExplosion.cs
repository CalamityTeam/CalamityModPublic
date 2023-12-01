using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class GhastlyExplosion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            int inc;
            int dustType = (int)Projectile.ai[0];
            for (int i = 0; i < 3; i = inc + 1)
            {
                int ghostlyRed = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, Projectile.velocity.X, Projectile.velocity.Y, dustType, default, 1.2f);
                Main.dust[ghostlyRed].position = (Main.dust[ghostlyRed].position + Projectile.Center) / 2f;
                Main.dust[ghostlyRed].noGravity = true;
                Dust dust = Main.dust[ghostlyRed];
                dust.velocity *= 0.5f;
                inc = i;
            }
            for (int j = 0; j < 2; j = inc + 1)
            {
                int ghostlyRed = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, Projectile.velocity.X, Projectile.velocity.Y, dustType, default, 0.4f);
                if (j == 0)
                {
                    Main.dust[ghostlyRed].position = (Main.dust[ghostlyRed].position + Projectile.Center * 5f) / 6f;
                }
                else if (j == 1)
                {
                    Main.dust[ghostlyRed].position = (Main.dust[ghostlyRed].position + (Projectile.Center + Projectile.velocity / 2f) * 5f) / 6f;
                }
                Dust dust = Main.dust[ghostlyRed];
                dust.velocity *= 0.1f;
                Main.dust[ghostlyRed].noGravity = true;
                Main.dust[ghostlyRed].fadeIn = 1f;
                inc = j;
            }
            return;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item50, Projectile.position);
            int inc;
            for (int i = 0; i < 20; i = inc + 1)
            {
                int killDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, (int)Projectile.ai[0], Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 0, default, 0.5f);
                Dust dust;
                Main.dust[killDust].scale = 1.2f + (float)Main.rand.Next(-10, 11) * 0.01f;
                Main.dust[killDust].noGravity = true;
                dust = Main.dust[killDust];
                dust.velocity *= 2.5f;
                dust = Main.dust[killDust];
                dust.velocity -= Projectile.oldVelocity / 10f;
                inc = i;
            }
            if (Main.myPlayer == Projectile.owner)
            {
                for (int j = 0; j < 3; j = inc + 1)
                {
                    Vector2 randShardRotation = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    while (randShardRotation.X == 0f && randShardRotation.Y == 0f)
                    {
                        randShardRotation = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    }
                    randShardRotation.Normalize();
                    randShardRotation *= (float)Main.rand.Next(70, 101) * 0.1f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.oldPosition.X + (float)(Projectile.width / 2), Projectile.oldPosition.Y + (float)(Projectile.height / 2), randShardRotation.X, randShardRotation.Y, ModContent.ProjectileType<GhastlyExplosionShard>(), (int)((double)Projectile.damage * 0.8), Projectile.knockBack * 0.8f, Projectile.owner, Projectile.ai[0], 0f);
                    inc = j;
                }
            }
        }
    }
}
