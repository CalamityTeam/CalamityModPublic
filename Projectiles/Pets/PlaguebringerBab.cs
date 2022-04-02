using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Pets
{
    public class PlaguebringerBab : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plaguebringer Bab");
            Main.projFrames[projectile.type] = 4;
            Main.projPet[projectile.type] = true;
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
            CalamityPlayer modPlayer = player.Calamity();
            if (!player.active)
            {
                projectile.active = false;
                return;
            }
            if (player.dead)
            {
                modPlayer.plaguebringerBab = false;
            }
            if (modPlayer.plaguebringerBab)
            {
                projectile.timeLeft = 2;
            }
            projectile.FloatingPetAI(false, 0.05f);
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= 4)
            {
                projectile.frame = 2;
            }
        }
    }
}
