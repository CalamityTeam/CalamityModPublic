using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class Waterfall : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
            Projectile.penetrate = 3;
            Projectile.timeLeft /= 2;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            for (int i = 0; i < 2; i++)
            {
                float shortXVel = Projectile.velocity.X / 3f * (float)i;
                float shortYVel = Projectile.velocity.Y / 3f * (float)i;
                int fourConst = 4;
                int watery = Dust.NewDust(new Vector2(Projectile.position.X + (float)fourConst, Projectile.position.Y + (float)fourConst), Projectile.width - fourConst * 2, Projectile.height - fourConst * 2, 56, 0f, 0f, 100, default, 1.2f);
                Main.dust[watery].noGravity = true;
                Main.dust[watery].velocity *= 0.25f;
                Main.dust[watery].velocity += Projectile.velocity * 0.1f;
                Dust expr_47FA_cp_0 = Main.dust[watery];
                expr_47FA_cp_0.position.X -= shortXVel;
                Dust expr_4815_cp_0 = Main.dust[watery];
                expr_4815_cp_0.position.Y -= shortYVel;
            }
            for (int j = 0; j < 2; j++)
            {
                float shortXVel2 = Projectile.velocity.X / 3f * (float)j;
                float shortYVel2 = Projectile.velocity.Y / 3f * (float)j;
                int otherFourConst = 4;
                int superWet = Dust.NewDust(new Vector2(Projectile.position.X + (float)otherFourConst, Projectile.position.Y + (float)otherFourConst), Projectile.width - otherFourConst * 2, Projectile.height - otherFourConst * 2, 245, 0f, 0f, 100, default, 1.2f);
                Main.dust[superWet].noGravity = true;
                Main.dust[superWet].velocity *= 0.1f;
                Main.dust[superWet].velocity += Projectile.velocity * 0.25f;
                Dust expr_47FA_cp_0 = Main.dust[superWet];
                expr_47FA_cp_0.position.X -= shortXVel2;
                Dust expr_4815_cp_0 = Main.dust[superWet];
                expr_4815_cp_0.position.Y -= shortYVel2;
            }

            Projectile.ai[1] += 1f;
            if (Projectile.ai[1] >= 60f)
                Projectile.tileCollide = true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
            {
                Projectile.Kill();
            }
            else
            {
                if (Projectile.velocity.X != oldVelocity.X)
                {
                    Projectile.velocity.X = -oldVelocity.X;
                }
                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.velocity.Y = -oldVelocity.Y;
                }
                SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            }
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 56, Projectile.oldVelocity.X * 0.25f, Projectile.oldVelocity.Y * 0.25f);
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 245, Projectile.oldVelocity.X * 0.25f, Projectile.oldVelocity.Y * 0.25f);
            }
        }
    }
}
