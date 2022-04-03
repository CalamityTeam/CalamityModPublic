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
            CalamityPlayer modPlayer = player.Calamity();
            bool correctMinion = Projectile.type == ModContent.ProjectileType<HowlsHeartCalcifer>();
            if (!modPlayer.howlsHeart && !modPlayer.howlsHeartVanity || !player.active)
            {
                Projectile.active = false;
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
                    Projectile.timeLeft = 2;
                }
            }
            if (!modPlayer.howlsHeartVanity)
                Lighting.AddLight(Projectile.Center, 0.75f, 0.485f, 0f);
            Projectile.FloatingPetAI(false, 0.04f, true);
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }
        }

        public override bool? CanDamage() => false;
    }
}
