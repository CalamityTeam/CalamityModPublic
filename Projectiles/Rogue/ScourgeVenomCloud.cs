using CalamityMod.Buffs.DamageOverTime;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class ScourgeVenomCloud : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Venom Cloud");
            Main.projFrames[projectile.type] = 10;
        }

        public override void SetDefaults()
        {
            projectile.width = 45;
            projectile.height = 45;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 3600;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 30;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.ai[0] += 1f;
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.ai[0] < 219f) //255 - frameCounter tick * number of disippation frames
            {
                if (projectile.frame >= 4)
                {
                    projectile.frame = 0;
                }
            }
            else if (projectile.owner == Main.myPlayer && projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.Kill();
            }
            projectile.velocity *= 0.98f;
            if (projectile.alpha > 110)
            {
                projectile.alpha -= 30;
                if (projectile.alpha < 110)
                {
                    projectile.alpha = 110;
                }
            }
            if (Math.Abs(projectile.velocity.X) > 0.1f)
            {
                projectile.spriteDirection = -projectile.direction;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Venom, 120);
            if (projectile.ai[1] == 1f && projectile.owner == Main.myPlayer) //stealth strike attack
            {
                target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 120);
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Venom, 120);
            if (projectile.ai[1] == 1f && projectile.owner == Main.myPlayer) //stealth strike attack
            {
                target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 120);
            }
        }
    }
}
