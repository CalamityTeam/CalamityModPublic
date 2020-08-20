using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Melee.Yoyos;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
	public class MicrowaveAura : ModProjectile
    {
		private int radius = 100;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Microwave Radiation");
        }

        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
			projectile.width = 200;
			projectile.height = 200;
			projectile.friendly = true;
			projectile.tileCollide = false;
			projectile.penetrate = -1;
			projectile.alpha = 255;
			projectile.ignoreWater = true;
			projectile.timeLeft = 300;
        }

		public override void AI()
		{
			Projectile parent = Main.projectile[0];
			bool active = false;
			for (int i = 0; i < Main.projectile.Length; i++)
			{
				Projectile p = Main.projectile[i];
				if (p.identity == projectile.ai[0] && p.active && p.type == ModContent.ProjectileType<MicrowaveYoyo>())
				{
					parent = p;
					active = true;
				}
			}

			if (active)
			{
				projectile.Center = parent.Center;
				projectile.timeLeft = 2;
			}
			else
			{
				projectile.Kill();
			}

			if (!parent.active)
			{
				projectile.Kill();
			}
		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 180);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float dist1 = Vector2.Distance(projectile.Center, targetHitbox.TopLeft());
            float dist2 = Vector2.Distance(projectile.Center, targetHitbox.TopRight());
            float dist3 = Vector2.Distance(projectile.Center, targetHitbox.BottomLeft());
            float dist4 = Vector2.Distance(projectile.Center, targetHitbox.BottomRight());

            float minDist = dist1;
            if (dist2 < minDist)
                minDist = dist2;
            if (dist3 < minDist)
                minDist = dist3;
            if (dist4 < minDist)
                minDist = dist4;

            return minDist <= radius;
        }
    }
}
