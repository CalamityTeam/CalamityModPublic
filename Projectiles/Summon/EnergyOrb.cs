using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
	public class EnergyOrb : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Magic/BlueBubble";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Energy Orb");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.alpha = 100;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.timeLeft = 200;
			projectile.minion = true;
        }

        public override void AI()
        {
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 4f)
            {
				int num469 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 132, 0f, 0f, 100, default, 1f);
				if (Main.rand.Next(6) != 0)
					Main.dust[num469].noGravity = true;
				Main.dust[num469].velocity *= 0f;
            }
            NPC potentialTarget = projectile.Center.MinionHoming(400f, Main.player[projectile.owner]);
            if (potentialTarget != null)
                projectile.velocity = (projectile.velocity * 20f + projectile.SafeDirectionTo(potentialTarget.Center) * 10f) / 21f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Electrified, 120);
        }
    }
}
