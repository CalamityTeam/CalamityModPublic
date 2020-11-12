using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class CosmicBolt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bolt");
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.extraUpdates = 100;
            projectile.friendly = true;
            projectile.timeLeft = 180;
            projectile.melee = true;
        }

        public override void AI()
        {
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 4f)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 spawnPos = projectile.position;
                    spawnPos -= projectile.velocity * ((float)i * 0.25f);
                    projectile.alpha = 255;
                    int d = Dust.NewDust(spawnPos, 1, 1, 242, 0f, 0f, 0, default, 1.3f);
                    Dust dust = Main.dust[d];
                    dust.position = spawnPos;
                    dust.position.X += (float)(projectile.width / 2);
                    dust.position.Y += (float)(projectile.height / 2);
                    dust.scale = Main.rand.NextFloat(0.49f, 0.763f);
                    dust.velocity *= 0.2f;
                }
            }
        }
    }
}
