using CalamityMod.Buffs.DamageOverTime;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class ShadeNimbus : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/ShadeNimbusHostile";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nimbus");
            Main.projFrames[projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            projectile.width = 54;
            projectile.height = 24;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 8)
            {
                projectile.frameCounter = 0;
                projectile.frame++;
                if (projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame = 0;
                }
            }
            projectile.ai[1] += 1f;
            if (projectile.ai[1] >= 7200f)
            {
                projectile.alpha += 5;
                if (projectile.alpha > 255)
                {
                    projectile.alpha = 255;
                    projectile.Kill();
                }
            }
            else
            {
                projectile.ai[0] += 1f;
                if (projectile.ai[0] > 8f)
                {
                    projectile.ai[0] = 0f;
                    if (projectile.owner == Main.myPlayer)
                    {
                        int num414 = (int)(projectile.position.X + 14f + (float)Main.rand.Next(projectile.width - 28));
                        int num415 = (int)(projectile.position.Y + (float)projectile.height + 4f);
                        Projectile.NewProjectile((float)num414, (float)num415, 0f, 5f, ModContent.ProjectileType<Shaderain>(), projectile.damage, 0f, projectile.owner, 0f, 0f);
                    }
                }
            }
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] >= 10f)
            {
                projectile.localAI[0] = 0f;
                int projCount = 0;
                int oldestCloud = 0;
                float cloudAge = 0f;
                int projType = projectile.type;
                for (int projIndex = 0; projIndex < Main.maxProjectiles; projIndex++)
                {
                    Projectile proj = Main.projectile[projIndex];
                    if (proj.active && proj.owner == projectile.owner && proj.type == projType && proj.ai[1] < 3600f)
                    {
                        projCount++;
                        if (proj.ai[1] > cloudAge)
                        {
                            oldestCloud = projIndex;
                            cloudAge = proj.ai[1];
                        }
                    }
                }
                if (projCount > 2)
                {
                    Main.projectile[oldestCloud].netUpdate = true;
                    Main.projectile[oldestCloud].ai[1] = 36000f;
                    return;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Shadowflame>(), 60);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Shadowflame>(), 60);
        }
    }
}
