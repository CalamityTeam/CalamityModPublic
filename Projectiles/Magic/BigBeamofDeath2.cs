using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class BigBeamofDeath2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Big Beam of Death");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 100;
            projectile.penetrate = -1;
            projectile.extraUpdates = 100;
            projectile.timeLeft = 80;
        }

        public override void AI()
        {
			projectile.localAI[0] += 1f;
			if (projectile.localAI[0] > 9f)
			{
				Dust.NewDust(projectile.position, 1, 1, 206, 0f, 0f, 0, default, 3f);
			}
		}
    }
}
