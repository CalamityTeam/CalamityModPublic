using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Typeless
{
    public class GodSlayerShrapnelRound : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Round");
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0f)
            {
                Main.PlaySound(SoundID.Item73, projectile.position);
                projectile.localAI[0] += 1f;
            }
            for (int d = 0; d < 3; d++)
            {
                int cosmilite = Dust.NewDust(projectile.position, projectile.width, projectile.height, 173, 0f, 0f, 100, default, 1.2f);
                Main.dust[cosmilite].noGravity = true;
                Main.dust[cosmilite].velocity *= 0.5f;
                Main.dust[cosmilite].velocity += projectile.velocity * 0.1f;
            }
        }

        public override void Kill(int timeLeft)
        {
            if (projectile.owner == Main.myPlayer)
            {
                int shrapnelAmt = Main.rand.Next(4, 7);
                for (int s = 0; s < shrapnelAmt; s++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<GodSlayerShrapnel>(), (int)(projectile.damage * 0.3), 0f, projectile.owner, 0f, 0f);
                }
            }
        }
    }
}
