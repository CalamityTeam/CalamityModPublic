using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Pets
{
    public class BabyGhostBell : ModProjectile
    {
        private bool underwater = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Baby Ghost Bell");
            Main.projFrames[projectile.type] = 4;
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.LightPet[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (!player.active)
            {
                projectile.active = false;
                return;
            }
            CalamityPlayer modPlayer = player.Calamity();
            if (player.dead)
            {
                modPlayer.babyGhostBell = false;
            }
            if (modPlayer.babyGhostBell)
            {
                projectile.timeLeft = 2;
            }
            underwater = Collision.DrownCollision(player.position, player.width, player.height, player.gravDir);
            if (underwater)
            {
                Lighting.AddLight(projectile.Center, 0.3f, 0.9f, 1.5f);
            }
            else
            {
                Lighting.AddLight(projectile.Center, 0.1f, 0.3f, 0.5f);
            }
            projectile.FloatingPetAI(false, 0.05f, true);
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
            {
                projectile.frame = 0;
            }
        }
    }
}
