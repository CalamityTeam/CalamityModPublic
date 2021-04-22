using Microsoft.Xna.Framework;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Enemy
{
    public class NuclearBulletMedium : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nuclear Bullet");
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 12;
            projectile.hostile = true;
            projectile.timeLeft = 360;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
        }
        public override void AI()
        {
            Lighting.AddLight(projectile.Center, Color.White.ToVector3() * 1.25f);
            projectile.rotation = projectile.velocity.ToRotation();
            projectile.ai[0]++;
            if (projectile.ai[0] < 85f)
            {
                projectile.velocity.Y += Math.Sign(projectile.localAI[0]) * 0.1f;
            }
            if (projectile.ai[0] == 160f)
            {
                projectile.ai[1] = Player.FindClosest(projectile.Center, 1, 1);
            }
            if (projectile.ai[0] >= 160f && projectile.ai[0] <= 200f)
            {
                Player player = Main.player[(int)projectile.ai[1]];
                float inertia = 10f;
                if (projectile.Distance(player.Center) > 70f)
                    projectile.velocity = (projectile.velocity * inertia + projectile.SafeDirectionTo(player.Center) * 18.5f) / (inertia + 1f);

                projectile.tileCollide = true;
            }
            else
            {
                projectile.tileCollide = false;
            }
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 240);
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i <= 2; i++)
            {
                int idx = Dust.NewDust(projectile.position, 8, 8, (int)CalamityDusts.SulfurousSeaAcid, 0, 0, 0, default, 0.75f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 3f;
                idx = Dust.NewDust(projectile.position, 8, 8, (int)CalamityDusts.SulfurousSeaAcid, 0, 0, 0, default, 0.75f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 3f;
            }
        }
    }
}
