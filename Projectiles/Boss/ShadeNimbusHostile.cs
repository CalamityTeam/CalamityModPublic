using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader; using CalamityMod.Dusts;
using Terraria.ModLoader; using CalamityMod.Dusts; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    public class ShadeNimbusHostile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shade Nimbus");
            Main.projFrames[projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            projectile.width = 54;
            projectile.height = 28;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 360;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 8)
            {
                projectile.frameCounter = 0;
                projectile.frame++;
                if (projectile.frame > 5)
                {
                    projectile.frame = 0;
                }
            }
            projectile.ai[1] += 1f;
            if (projectile.ai[1] >= 300f)
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
                    int num414 = (int)(projectile.position.X + 14f + (float)Main.rand.Next(projectile.width - 28));
                    int num415 = (int)(projectile.position.Y + (float)projectile.height + 4f);
                    Projectile.NewProjectile((float)num414, (float)num415, 0f, 5f, ModContent.ProjectileType<ShaderainHostile>(), projectile.damage, 0f, Main.myPlayer, 0f, 0f);
                }
            }
        }
    }
}
