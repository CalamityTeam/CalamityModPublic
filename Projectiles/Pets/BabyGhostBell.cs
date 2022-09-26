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
            Main.projFrames[Projectile.type] = 4;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.LightPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
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
                modPlayer.babyGhostBell = false;
            }
            if (modPlayer.babyGhostBell)
            {
                Projectile.timeLeft = 2;
            }
            underwater = Collision.DrownCollision(player.position, player.width, player.height, player.gravDir);
            if (underwater)
            {
                Lighting.AddLight(Projectile.Center, 0.3f, 0.9f, 1.5f);
            }
            else
            {
                Lighting.AddLight(Projectile.Center, 0.1f, 0.3f, 0.5f);
            }
            Projectile.FloatingPetAI(false, 0.05f, true);
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
        }
    }
}
