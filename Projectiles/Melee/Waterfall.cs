using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class Waterfall : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beam");
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.extraUpdates = 2;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 8;
            projectile.penetrate = 3;
            projectile.timeLeft /= 2;
            projectile.melee = true;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            for (int num92 = 0; num92 < 2; num92++)
            {
                float num93 = projectile.velocity.X / 3f * (float)num92;
                float num94 = projectile.velocity.Y / 3f * (float)num92;
                int num95 = 4;
                int num96 = Dust.NewDust(new Vector2(projectile.position.X + (float)num95, projectile.position.Y + (float)num95), projectile.width - num95 * 2, projectile.height - num95 * 2, 56, 0f, 0f, 100, default, 1.2f);
                Main.dust[num96].noGravity = true;
                Main.dust[num96].velocity *= 0.25f;
                Main.dust[num96].velocity += projectile.velocity * 0.1f;
                Dust expr_47FA_cp_0 = Main.dust[num96];
                expr_47FA_cp_0.position.X -= num93;
                Dust expr_4815_cp_0 = Main.dust[num96];
                expr_4815_cp_0.position.Y -= num94;
            }
            for (int num105 = 0; num105 < 2; num105++)
            {
                float num99 = projectile.velocity.X / 3f * (float)num105;
                float num100 = projectile.velocity.Y / 3f * (float)num105;
                int num101 = 4;
                int num102 = Dust.NewDust(new Vector2(projectile.position.X + (float)num101, projectile.position.Y + (float)num101), projectile.width - num101 * 2, projectile.height - num101 * 2, 245, 0f, 0f, 100, default, 1.2f);
                Main.dust[num102].noGravity = true;
                Main.dust[num102].velocity *= 0.1f;
                Main.dust[num102].velocity += projectile.velocity * 0.25f;
                Dust expr_47FA_cp_0 = Main.dust[num102];
                expr_47FA_cp_0.position.X -= num99;
                Dust expr_4815_cp_0 = Main.dust[num102];
                expr_4815_cp_0.position.Y -= num100;
            }

            projectile.ai[1] += 1f;
            if (projectile.ai[1] >= 60f)
                projectile.tileCollide = true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.penetrate--;
            if (projectile.penetrate <= 0)
            {
                projectile.Kill();
            }
            else
            {
                if (projectile.velocity.X != oldVelocity.X)
                {
                    projectile.velocity.X = -oldVelocity.X;
                }
                if (projectile.velocity.Y != oldVelocity.Y)
                {
                    projectile.velocity.Y = -oldVelocity.Y;
                }
                Main.PlaySound(SoundID.Item10, projectile.Center);
            }
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item27, projectile.Center);
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 56, projectile.oldVelocity.X * 0.25f, projectile.oldVelocity.Y * 0.25f);
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 245, projectile.oldVelocity.X * 0.25f, projectile.oldVelocity.Y * 0.25f);
            }
        }
    }
}
