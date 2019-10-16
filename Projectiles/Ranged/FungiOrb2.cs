using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    public class FungiOrb2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Orb");
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.aiStyle = 1;
            aiType = ProjectileID.Bullet;
        }

        public override void AI()
        {
            projectile.velocity.Y += 0.1f;
            projectile.velocity.X *= 0.95f;
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 4f)
            {
                for (int num468 = 0; num468 < 3; num468++)
                {
                    int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 56, 0f, 0f, 100, default, 1f);
                    Main.dust[num469].noGravity = true;
                    Main.dust[num469].velocity *= 0f;
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 56, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
        }
    }
}
