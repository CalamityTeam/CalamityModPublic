using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class BigBeamofDeath : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Big Beam of Death");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 100;
            projectile.penetrate = -1;
            projectile.extraUpdates = 100;
            projectile.timeLeft = 80;
        }

        public override void Kill(int timeLeft)
        {
            int numProj = 2;
            float rotation = MathHelper.ToRadians(20);
            if (projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < numProj; i++)
                {
                    Vector2 perturbedSpeed = projectile.velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
                    Projectile.NewProjectile(projectile.Center, perturbedSpeed, ModContent.ProjectileType<BigBeamofDeath2>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                }
            }
        }

        public override void AI()
        {
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 9f)
            {
                Vector2 vector33 = projectile.position;
                vector33 -= projectile.velocity * 0.25f;
                int num448 = Dust.NewDust(vector33, 1, 1, 206, 0f, 0f, 0, default, 3f);
                Main.dust[num448].position = vector33;
                Main.dust[num448].velocity *= 0.1f;
            }
        }
    }
}
