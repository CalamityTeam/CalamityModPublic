using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class GhostlyBolt : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bolt");
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.extraUpdates = 1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 6;
            projectile.timeLeft = 900;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 6f)
            {
                Main.PlaySound(SoundID.Item8, projectile.position);
                for (int num151 = 0; num151 < 40; num151++)
                {
                    int num152 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 181, 0f, 0f, 100, default, 1f);
                    Main.dust[num152].velocity *= 3f;
                    Main.dust[num152].velocity += projectile.velocity * 0.75f;
                    Main.dust[num152].scale *= 1.2f;
                    Main.dust[num152].noGravity = true;
                }
            }
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 6f)
            {
                for (int num153 = 0; num153 < 3; num153++)
                {
                    int num154 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 181, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default, 1f);
                    Main.dust[num154].velocity *= 0.6f;
                    Main.dust[num154].scale *= 1.4f;
                    Main.dust[num154].noGravity = true;
                }
            }
        }
    }
}
