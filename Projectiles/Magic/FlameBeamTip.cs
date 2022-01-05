using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class FlameBeamTip : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beam");
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.aiStyle = 4;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.magic = true;
        }

        public override bool PreAI()
        {
            if (projectile.ai[0] != 0f)
                if (projectile.alpha < 170 && projectile.alpha + 5 >= 170)
                    projectile.alpha += 5;

            return true;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 0f)
            {
                projectile.alpha -= 50;
                if (projectile.alpha <= 0)
                {
                    projectile.alpha = 0;
                    projectile.ai[0] = 1f;
                    if (projectile.ai[1] == 0f)
                    {
                        projectile.ai[1] += 1f;
                        projectile.position += projectile.velocity;
                    }
                    if (Main.myPlayer == projectile.owner)
                    {
                        int num48 = projectile.type;
                        if (projectile.ai[1] >= (float)(15 + Main.rand.Next(3)))
                        {
                            num48 = ModContent.ProjectileType<FlameBeamTip2>();
                        }

                        int number = Projectile.NewProjectile(projectile.position.X + projectile.velocity.X + (float)(projectile.width / 2), projectile.position.Y + projectile.velocity.Y + (float)(projectile.height / 2),
                            projectile.velocity.X, projectile.velocity.Y, num48, projectile.damage, projectile.knockBack, projectile.owner, 0f, projectile.ai[1] + 1f);
                        NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, number, 0f, 0f, 0f, 0, 0, 0);
                    }
                }
            }
            else
            {
                if (projectile.alpha == 150)
                {
                    for (int num55 = 0; num55 < 10; num55++)
                    {
                        int num56 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 60, projectile.velocity.X * 0.01f, projectile.velocity.Y * 0.01f, 200, default, 2f);
                        Main.dust[num56].noGravity = true;
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 8;
            target.AddBuff(BuffID.OnFire, 240);
        }
    }
}
