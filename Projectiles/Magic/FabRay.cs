using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class FabRay : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ray");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = 20;
            projectile.extraUpdates = 100;
            projectile.timeLeft = 600;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.2f, 0.01f, 0.1f);
            projectile.ai[0] += 1f;
            if (projectile.ai[0] % 6f == 0f && projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<FabOrb>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
            }

            if (projectile.ai[0] > 16f && projectile.ai[0] % 2f == 0f)
            {
				Vector2 source = projectile.position;
				int pink = Dust.NewDust(source, 1, 1, 234, 0f, 0f, 0, default, 1.25f);
				Main.dust[pink].noGravity = true;
				Main.dust[pink].noLight = true;
				Main.dust[pink].position = source;
				Main.dust[pink].scale = Main.rand.Next(70, 110) * 0.013f;
				Main.dust[pink].velocity *= 0.1f;
            }
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
            }
            return false;
        }
    }
}
