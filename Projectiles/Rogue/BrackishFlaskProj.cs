using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class BrackishFlaskProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brackish Flask");
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.aiStyle = 2;
            projectile.timeLeft = 180;
            aiType = 48;
            projectile.Calamity().rogue = true;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 107);
            int randomDust = Main.rand.Next(2);
            if (randomDust == 0)
            {
                randomDust = 33;
            }
            else
            {
                randomDust = 89;
            }
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, randomDust, projectile.oldVelocity.X, projectile.oldVelocity.Y);
            }
            float spread = 45f * 0.0174f;
            double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
            double deltaAngle = spread / 8f;
            double offsetAngle;
            int i;
            if (projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<BrackishWaterBlast>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                for (i = 0; i < 4; i++)
                {
                    offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<BrackishWater>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<BrackishWater>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Venom, 300);
            target.AddBuff(BuffID.Poisoned, 300);
        }
    }
}
