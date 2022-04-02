using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
	public class MangroveChakramFlower : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Magic/BeamingBolt";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flower");
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 30;
            projectile.friendly = true;
            projectile.timeLeft = 60;
            projectile.penetrate = -1;
            projectile.Calamity().rogue = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 30;
        }

        public override void AI()
        {
            projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.01f * (float)projectile.direction;
            projectile.velocity.X *= 0.95f;
            projectile.velocity.Y *= 0.985f;
            for (int dust = 0; dust < 2; dust++)
            {
				int randomDust = Utils.SelectRandom(Main.rand, new int[]
				{
					164,
					58,
					204
				});
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, randomDust, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 6; k++)
            {
				int randomDust = Utils.SelectRandom(Main.rand, new int[]
				{
					164,
					58,
					204
				});
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, randomDust, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
            Main.PlaySound(SoundID.Item105, projectile.position);
        }
    }
}
