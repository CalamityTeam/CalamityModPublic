using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class CosmicBolt : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bolt");
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.extraUpdates = 100;
            Projectile.friendly = true;
            Projectile.timeLeft = 180;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
        }

        public override void AI()
        {
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 4f)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 spawnPos = Projectile.position;
                    spawnPos -= Projectile.velocity * ((float)i * 0.25f);
                    Projectile.alpha = 255;
                    int d = Dust.NewDust(spawnPos, 1, 1, 242, 0f, 0f, 0, default, 1.3f);
                    Dust dust = Main.dust[d];
                    dust.position = spawnPos;
                    dust.position.X += (float)(Projectile.width / 2);
                    dust.position.Y += (float)(Projectile.height / 2);
                    dust.scale = Main.rand.NextFloat(0.49f, 0.763f);
                    dust.velocity *= 0.2f;
                    dust.noGravity = true;
                }
            }
        }
    }
}
