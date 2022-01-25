using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class ToxicCloud : ModProjectile
    {
		public override string Texture => "CalamityMod/Projectiles/Enemy/ToxicMinnowCloud";

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
            projectile.timeLeft = 900;
			projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
		}

        public override void AI()
        {
			Lighting.AddLight(projectile.Center, 0.1f, 0.7f, 0f);

			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			if (projectile.velocity.Length() < (malice ? 6.25f : 5f))
				projectile.velocity *= 1.01f;

			projectile.frameCounter++;
            if (projectile.frameCounter > 9)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
                projectile.frame = 0;

            projectile.ai[0] += 1f;
            if (projectile.ai[0] >= 860f)
            {
                if (projectile.alpha < 255)
                {
                    projectile.alpha += 5;
                    if (projectile.alpha > 255)
                        projectile.alpha = 255;
                }
                else if (projectile.owner == Main.myPlayer)
                    projectile.Kill();
            }
            else if (projectile.alpha > 80)
            {
                projectile.alpha -= 30;
                if (projectile.alpha < 80)
                    projectile.alpha = 80;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(projectile.Center, 20f, targetHitbox);

		public override bool CanHitPlayer(Player target) => projectile.alpha == 80;

		public override void OnHitPlayer(Player target, int damage, bool crit)
        {
			if (projectile.alpha == 80)
			{
				target.AddBuff(BuffID.Poisoned, 120);
				target.AddBuff(ModContent.BuffType<Irradiated>(), 120);
			}
        }
    }
}
