using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class NettleLeft : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Thorn");
        }

        public override void SetDefaults()
        {
			projectile.width = 28;
			projectile.height = 28;
			projectile.aiStyle = 4;
			projectile.friendly = true;
			projectile.penetrate = 1;
			projectile.tileCollide = false;
			projectile.alpha = 255;
			projectile.ignoreWater = true;
			projectile.magic = true;
        }

		public override void AI()
		{
			if (projectile.ai[0] == 0f)
			{
				if (Main.myPlayer == projectile.owner)
				{
					int Type = ModContent.ProjectileType<NettleRight>();
					if (projectile.ai[1] >= 10f)
						Type = ModContent.ProjectileType<NettleTip>();
					int number = Projectile.NewProjectile(projectile.position.X + projectile.velocity.X + (float)(projectile.width / 2), projectile.position.Y + projectile.velocity.Y + (float)(projectile.height / 2), projectile.velocity.X, projectile.velocity.Y, Type, projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
					Main.projectile[number].ai[1] = projectile.ai[1] + 1f;
					projectile.ai[0]++;
				}
			}
		}
    }
}
