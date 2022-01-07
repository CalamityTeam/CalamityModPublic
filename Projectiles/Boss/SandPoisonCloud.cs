using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class SandPoisonCloud : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Toxic Cloud");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
			projectile.Calamity().canBreakPlayerDefense = true;
			projectile.width = 52;
            projectile.height = 48;
            projectile.hostile = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 1800;
			projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
		}

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.5f, 0.3f, 0f);

            projectile.frameCounter++;
            if (projectile.frameCounter > 9)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
                projectile.frame = 0;

            projectile.velocity *= 0.995f;

            if (projectile.timeLeft < 180)
            {
				projectile.damage = 0;
                if (projectile.alpha < 255)
                {
                    projectile.alpha += 5;
                    if (projectile.alpha > 255)
                    {
                        projectile.alpha = 255;
                        projectile.Kill();
                    }
                }
            }
            else if (projectile.alpha > 30)
            {
                projectile.alpha -= 30;
                if (projectile.alpha < 30)
                    projectile.alpha = 30;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(projectile.Center, 20f, targetHitbox);

		public override bool CanHitPlayer(Player target) => projectile.alpha == 30;

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
			if (projectile.alpha == 30)
				target.AddBuff(ModContent.BuffType<Irradiated>(), 240, true);
		}
    }
}
