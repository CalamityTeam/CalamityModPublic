using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class JudgementProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        int whiteLightTimer = 5;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Judgement");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 60;
            projectile.melee = true;
        }

        public override void AI()
        {
            whiteLightTimer--;
            if (whiteLightTimer == 0)
            {
                float spread = 180f * 0.0174f;
                double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
                double deltaAngle = spread / 8f;
                double offsetAngle;
                int i;
                if (projectile.owner == Main.myPlayer)
                {
                    for (i = 0; i < 1; i++)
                    {
                        offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                        int projectile1 = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<WhiteOrb>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                        int projectile2 = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<WhiteOrb>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                        Main.projectile[projectile1].velocity.X *= 0.1f;
                        Main.projectile[projectile1].velocity.Y *= 0.1f;
                        Main.projectile[projectile2].velocity.X *= 0.1f;
                        Main.projectile[projectile2].velocity.Y *= 0.1f;
                    }
                }
                whiteLightTimer = 5;
            }

            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.5f / 255f, (255 - projectile.alpha) * 0.5f / 255f, (255 - projectile.alpha) * 0.5f / 255f);

            for (int num457 = 0; num457 < 2; num457++)
            {
                int num458 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 91, 0f, 0f, 100, default, 1.25f);
                Main.dust[num458].noGravity = true;
                Main.dust[num458].velocity *= 0.5f;
                Main.dust[num458].velocity += projectile.velocity * 0.1f;
            }

            CalamityGlobalProjectile.HomeInOnNPC(projectile, !projectile.tileCollide, 200f, 9f, 20f);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item122, projectile.position);
            if (projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y - 100, 0f, 0f, ModContent.ProjectileType<WhiteBoltAura>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
            }
        }
    }
}
