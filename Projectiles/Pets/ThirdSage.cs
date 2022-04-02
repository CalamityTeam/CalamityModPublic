using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Pets
{
    public class ThirdSage : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Third Sage");
            Main.projPet[projectile.type] = true;
            Main.projFrames[projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 42;
            projectile.height = 42;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.tileCollide = false;
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
                modPlayer.thirdSage = false;
            }
            if (modPlayer.thirdSage)
            {
                projectile.timeLeft = 2;
            }
            projectile.FloatingPetAI(true, 0.1f);
            //Animation
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 4 && projectile.ai[1] < 45)
            {
                projectile.frame = 0;
                projectile.ai[1]++;
            }
            else if (projectile.frame == 4 && projectile.ai[1] >= 45)
            {
                Main.PlaySound(SoundID.Zombie, projectile.Center, 32);
            }
            else if (projectile.frame > 6)
            {
                projectile.frame = 0;
                projectile.ai[1] = 0;
            }
        }
    }
}
