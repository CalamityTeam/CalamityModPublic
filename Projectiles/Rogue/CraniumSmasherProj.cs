using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class CraniumSmasherProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/CraniumSmasher";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cranium Smasher");
        }

        public override void SetDefaults()
        {
            projectile.width = 50;
            projectile.height = 50;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 5;
            projectile.timeLeft = 300;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 15;
            projectile.tileCollide = false;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.ai[0] += 1f;
            if (projectile.ai[0] >= 3f)
            {
                projectile.tileCollide = true;
            }
            projectile.rotation += projectile.velocity.X * 0.02f;
            projectile.velocity.Y = projectile.velocity.Y + 0.085f;
            projectile.velocity.X = projectile.velocity.X * 0.99f;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 5, projectile.oldVelocity.X / 2, projectile.oldVelocity.Y / 2, 0, default, 2f);
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 6, projectile.oldVelocity.X / 2, projectile.oldVelocity.Y / 2, 0, default, 1f);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;
    }
}
