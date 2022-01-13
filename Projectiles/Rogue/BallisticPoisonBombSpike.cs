using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class BallisticPoisonBombSpike : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spike");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.alpha = 255;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 3;
            projectile.alpha = 255;
            projectile.extraUpdates = 2;
            projectile.aiStyle = 93;
            aiType = ProjectileID.NailFriendly;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.alpha -= 10;
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
            projectile.localAI[1] += 1f;
            if (projectile.localAI[1] > 4f)
            {
                int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 14, 0f, 0f, 100, default, 0.75f);
                Main.dust[num469].noGravity = true;
                Main.dust[num469].velocity *= 0f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Venom, 120);
            target.immune[projectile.owner] = 1;
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Venom, 120);
        }

        public override void Kill(int timeLeft)
        {
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 32;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            for (int num621 = 0; num621 < 2; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 14, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 1.2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 2; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 6, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 1.7f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 6, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 1f);
                Main.dust[num624].velocity *= 2f;
            }
        }
    }
}
