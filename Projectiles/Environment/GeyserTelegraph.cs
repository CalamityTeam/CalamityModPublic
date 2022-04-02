using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Environment
{
    public class GeyserTelegraph : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private bool initialized = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Geyser");
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 12;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 100;
            projectile.trap = true;
        }

        public override void AI()
        {
            if (!initialized && Main.myPlayer != projectile.owner)
            {
                int projectileType = ModContent.ProjectileType<SmokeTelegraph>();
                float randomVelocity = Main.rand.NextFloat() + 0.5f;
                int proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, -8f * randomVelocity, projectileType, 0, 0f, projectile.owner, 0f, 0f);
                Main.projectile[proj].netUpdate = true;
                initialized = true;
            }
        }

        public override void Kill(int timeLeft)
        {
            if (Main.myPlayer != projectile.owner)
                return;

            int projectileType = ProjectileID.GeyserTrap;
            if (projectile.ai[0] == 1f)
            {
                projectileType = ModContent.ProjectileType<BrimstoneGeyser>();
            }
            float randomVelocity = Main.rand.NextFloat() + 0.5f;
            int proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, -8f * randomVelocity, projectileType, 20, 2f, projectile.owner, 0f, 0f);
            Main.projectile[proj].friendly = false;
            Main.projectile[proj].netUpdate = true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;
    }
}
