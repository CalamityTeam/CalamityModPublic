using CalamityMod.CalPlayer;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Projectiles.Melee
{
    public class DNA : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("DNA");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.aiStyle = 4;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.melee = true;
			projectile.ignoreWater = true;
			aiType = ProjectileID.CrystalVileShardHead;
        }

        public override void AI()
        {
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
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
                        projectile.position += projectile.velocity * 1f;
                    }
                    if (Main.myPlayer == projectile.owner)
                    {
                        int num48 = projectile.type;
                        if (projectile.ai[1] >= (float)(12 + Main.rand.Next(2)))
                        {
                            num48 = ModContent.ProjectileType<DNA2>();
                        }
                        int num49 = projectile.damage;
                        float num50 = projectile.knockBack;
                        int number = Projectile.NewProjectile(projectile.position.X + projectile.velocity.X + (float)(projectile.width / 2), projectile.position.Y + projectile.velocity.Y + (float)(projectile.height / 2), projectile.velocity.X, projectile.velocity.Y, num48, num49, num50, projectile.owner, 0f, projectile.ai[1] + 1f);
                        NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, number, 0f, 0f, 0f, 0, 0, 0);
                    }
                }
            }
            else
            {
                if (projectile.alpha < 170 && projectile.alpha + 5 >= 170)
                {
                    for (int num55 = 0; num55 < 8; num55++)
                    {
                        int num56 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 234, projectile.velocity.X * 0.005f, projectile.velocity.Y * 0.005f, 200, default, 1f);
                        Main.dust[num56].noGravity = true;
                        Main.dust[num56].velocity *= 0.5f;
                    }
                }
                projectile.alpha += 7;
                if (projectile.alpha >= 255)
                {
                    projectile.Kill();
                }
            }
            if (Main.rand.NextBool(4))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 234, projectile.velocity.X * 0.005f, projectile.velocity.Y * 0.005f);
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 234, projectile.oldVelocity.X * 0.005f, projectile.oldVelocity.Y * 0.005f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.myPlayer == projectile.owner && !CalamityPlayer.areThereAnyDamnBosses)
            {
				bool isImmune = false;
				for (int j = 0; j < Main.player[projectile.owner].hurtCooldowns.Length; j++)
				{
					if (Main.player[projectile.owner].hurtCooldowns[j] > 0)
						isImmune = true;
				}
				if (!isImmune)
                {
                    Main.player[projectile.owner].immune = true;
                    Main.player[projectile.owner].immuneTime = 5;
                }
            }
            target.immune[projectile.owner] = 5;
        }
    }
}
