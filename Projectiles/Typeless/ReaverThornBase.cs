using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Typeless
{
    public class ReaverThornBase : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/ThornBase";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Thorn");
        }

        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 28;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.alpha = 255;
            projectile.ignoreWater = true;
            projectile.aiStyle = 4;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 10;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (projectile.ai[0] == 0f)
            {
                projectile.alpha -= 50;
                if (projectile.alpha > 0)
                    return;
                projectile.alpha = 0;
                projectile.ai[0] = 1f;
                if (projectile.ai[1] == 0f)
                {
                    projectile.ai[1] += 1f;
                    projectile.position += projectile.velocity * 1f;
                }
                if (Main.myPlayer == projectile.owner)
                {
                    int type = projectile.type;
                    if (projectile.ai[1] >= 6f)
                        type = ModContent.ProjectileType<ReaverThornTip>();
                    int thorn = Projectile.NewProjectile(projectile.Center + projectile.velocity, projectile.velocity, type, projectile.damage, projectile.knockBack, projectile.owner);
                    Main.projectile[thorn].ai[1] = projectile.ai[1] + 1f;
                }
            }
            else
            {
                if (projectile.alpha < 170 && projectile.alpha + 5 >= 170)
                {
                    for (int index = 0; index < 3; ++index)
                    {
                        Dust.NewDust(projectile.position, projectile.width, projectile.height, 18, projectile.velocity.X * 0.025f, projectile.velocity.Y * 0.025f, 170, new Color(), 1.2f);
                    }
                    Dust.NewDust(projectile.position, projectile.width, projectile.height, 14, 0f, 0f, 170, new Color(), 1.1f);
                }
                projectile.alpha += 3;
                if (projectile.alpha < 255)
                    return;
                projectile.Kill();
            }
        }
    }
}
