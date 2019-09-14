using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Projectiles.Pets
{
    public class Fox : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fox");
            Main.projFrames[projectile.type] = 11;
            Main.projPet[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 24;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.aiStyle = 26;
            aiType = 334;
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
            CalamityPlayer modPlayer = player.GetCalamityPlayer();
            if (player.dead)
            {
                modPlayer.fox = false;
            }
            if (modPlayer.fox)
            {
                projectile.timeLeft = 2;
            }
            projectile.spriteDirection = projectile.direction;
        }
    }
}
