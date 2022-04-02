using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class TinyCrystal : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal");
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = 1;
            projectile.alpha = 255;
            projectile.timeLeft = 90;
        }

        public override bool? CanHitNPC(NPC target) => projectile.timeLeft < 75 && target.CanBeChasedBy(projectile);

        public override void AI()
        {
            int dustType = projectile.ai[0] == 0f ? 56 : 73;
            for (int num92 = 0; num92 < 2; num92++)
            {
                float num93 = projectile.velocity.X / 3f * (float)num92;
                float num94 = projectile.velocity.Y / 3f * (float)num92;
                int num95 = 4;
                int num96 = Dust.NewDust(new Vector2(projectile.position.X + (float)num95, projectile.position.Y + (float)num95), projectile.width - num95 * 2, projectile.height - num95 * 2, dustType, 0f, 0f, 100, default, 1.2f);
                Main.dust[num96].noGravity = true;
                Main.dust[num96].velocity *= 0.1f;
                Main.dust[num96].velocity += projectile.velocity * 0.1f;
                Dust expr_47FA_cp_0 = Main.dust[num96];
                expr_47FA_cp_0.position.X -= num93;
                Dust expr_4815_cp_0 = Main.dust[num96];
                expr_4815_cp_0.position.Y -= num94;
            }

            if (Main.rand.NextBool(10))
            {
                int num97 = 4;
                int num98 = Dust.NewDust(new Vector2(projectile.position.X + (float)num97, projectile.position.Y + (float)num97), projectile.width - num97 * 2, projectile.height - num97 * 2, dustType, 0f, 0f, 100, default, 0.6f);
                Main.dust[num98].velocity *= 0.25f;
                Main.dust[num98].velocity += projectile.velocity * 0.5f;
            }

            if (projectile.timeLeft < 75)
                CalamityGlobalProjectile.HomeInOnNPC(projectile, !projectile.tileCollide, 450f, 9f, 20f);
        }

        public override void Kill(int timeLeft)
        {
            int dustType = projectile.ai[0] == 0f ? 56 : 73;
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, dustType, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
            Main.PlaySound(SoundID.Item10, projectile.position);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            int buffType = projectile.ai[0] == 0f ? BuffID.Frostburn : BuffID.OnFire;
            target.AddBuff(buffType, 90);
        }
    }
}
