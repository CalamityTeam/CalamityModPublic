using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class RotomBeam : ModProjectile
    {
		private int counter = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beam");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.penetrate = 1;
            projectile.extraUpdates = 100;
            projectile.timeLeft = 200;
        }

        public override void AI()
        {
			if (counter > 3)
			{
				for (int num447 = 0; num447 < 4; num447++)
				{
					Vector2 vector33 = projectile.position;
					vector33 -= projectile.velocity * ((float)num447 * 0.25f);
					projectile.alpha = 255;
					int num448 = Dust.NewDust(vector33, 1, 1, 132, 0f, 0f, 0, default, 1f);
					Main.dust[num448].position = vector33;
					Main.dust[num448].scale = (float)Main.rand.Next(70, 110) * 0.013f;
					Main.dust[num448].velocity *= 0.2f;
					Main.dust[num448].noGravity = true;
				}
			}
			else
				counter++;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.rand.NextBool(3))
            {
				target.AddBuff(BuffID.Electrified, 60);
            }
        }
    }
}
