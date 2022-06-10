using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Pets
{
    public class ThirdSage : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Third Sage");
            Main.projPet[Projectile.type] = true;
            Main.projFrames[Projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active)
            {
                Projectile.active = false;
                return;
            }
            CalamityPlayer modPlayer = player.Calamity();
            if (player.dead)
            {
                modPlayer.thirdSage = false;
            }
            if (modPlayer.thirdSage)
            {
                Projectile.timeLeft = 2;
            }
            Projectile.FloatingPetAI(true, 0.1f);
            //Animation
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 4 && Projectile.ai[1] < 45)
            {
                Projectile.frame = 0;
                Projectile.ai[1]++;
            }
            else if (Projectile.frame == 4 && Projectile.ai[1] >= 45)
            {
                SoundEngine.PlaySound(SoundID.Zombie32, Projectile.Center);
            }
            else if (Projectile.frame > 6)
            {
                Projectile.frame = 0;
                Projectile.ai[1] = 0;
            }
        }
    }
}
