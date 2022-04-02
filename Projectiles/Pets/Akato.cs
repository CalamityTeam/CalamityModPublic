using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Pets
{
    public class Akato : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Akato");
            Main.projFrames[projectile.type] = 6;
            Main.projPet[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 60;
            projectile.height = 60;
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
                modPlayer.akato = false;
            }
            if (modPlayer.akato)
            {
                projectile.timeLeft = 2;
            }
            projectile.FloatingPetAI(true, 0.02f);
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 5)
            {
                projectile.frame = 0;
            }
        }
    }
}
