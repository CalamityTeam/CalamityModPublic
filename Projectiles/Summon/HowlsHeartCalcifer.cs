using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class HowlsHeartCalcifer : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Calcifer");
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
            CalamityPlayer modPlayer = player.Calamity();
            bool correctMinion = projectile.type == ModContent.ProjectileType<HowlsHeartCalcifer>();
            if (!modPlayer.howlsHeart && !modPlayer.howlsHeartVanity || !player.active)
            {
                projectile.active = false;
                return;
            }
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.howlTrio = false;
                }
                if (modPlayer.howlTrio)
                {
                    projectile.timeLeft = 2;
                }
            }
            if (!modPlayer.howlsHeartVanity)
                Lighting.AddLight(projectile.Center, 0.75f, 0.485f, 0f);
            projectile.FloatingPetAI(false, 0.04f, true);
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }
        }

        public override bool CanDamage() => false;
    }
}
