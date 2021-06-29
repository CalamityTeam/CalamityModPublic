using CalamityMod.Buffs.DamageOverTime;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class VehemenceSkull : ModProjectile
    {
        public ref float Time => ref projectile.ai[0];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimstone Skull");
            Main.projFrames[projectile.type] = 10;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 45;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 6;
            projectile.magic = true;
        }

        public override void AI()
        {
            Time++;
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }

			if (projectile.ai[0] < 125) // 155 - frameCounter tick * number of disippation frames.
			{
				if (projectile.frame >= 4)
					projectile.frame = 0;
			}

			else if (projectile.owner == Main.myPlayer && projectile.frame >= Main.projFrames[projectile.type])
				projectile.Kill();

            projectile.velocity *= 0.98f;
            if (projectile.alpha > 110)
            {
                projectile.alpha -= 30;
                if (projectile.alpha < 70)
                    projectile.alpha = 70;
            }

            if (Math.Abs(projectile.velocity.X) > 0.1f)
                projectile.spriteDirection = -projectile.direction;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.AddBuff(ModContent.BuffType<DemonFlames>(), 240);
    }
}
