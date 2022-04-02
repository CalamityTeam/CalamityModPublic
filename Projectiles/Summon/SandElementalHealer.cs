using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Healing;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class SandElementalHealer : ModProjectile
    {
        public int dust = 3;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sand Healer");
            Main.projFrames[projectile.type] = 6;
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 42;
            projectile.height = 98;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 0f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            if (!modPlayer.sandBoobWaifu && !modPlayer.allWaifus)
            {
                projectile.active = false;
                return;
            }

            bool correctMinion = projectile.type == ModContent.ProjectileType<SandElementalHealer>();
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.dWaifu = false;
                }
                if (modPlayer.dWaifu)
                {
                    projectile.timeLeft = 2;
                }
            }

            dust--;
            if (dust >= 0)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                int dustAmt = 50;
                for (int d = 0; d < dustAmt; d++)
                {
                    int sand = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 16f), projectile.width, projectile.height - 16, 32, 0f, 0f, 0, default, 1f);
                    Main.dust[sand].velocity *= 2f;
                    Main.dust[sand].scale *= 1.15f;
                }
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = damage2;
            }

            projectile.frameCounter++;
            if (projectile.frameCounter > 16)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }

            if (Math.Abs(projectile.velocity.X) > 0.2f)
            {
                projectile.spriteDirection = -projectile.direction;
            }

            float lightScalar = (float)Main.rand.Next(90, 111) * 0.01f;
            lightScalar *= Main.essScale;
            Lighting.AddLight(projectile.Center, 0.7f * lightScalar, 0.6f * lightScalar, 0f * lightScalar);

            projectile.MinionAntiClump();

            if (Vector2.Distance(player.Center, projectile.Center) > 400f)
            {
                projectile.ai[0] = 1f;
                projectile.tileCollide = false;
                projectile.netUpdate = true;
            }

            float safeDist = 100f; //150
            bool returning = false;
            if (!returning)
            {
                returning = projectile.ai[0] == 1f;
            }
            float returnSpeed = 7f; //6
            if (returning)
            {
                returnSpeed = 18f; //15
            }
            Vector2 playerVec = player.Center - projectile.Center + new Vector2(-250f, -60f); //-60
            float playerDist = playerVec.Length();
            if (playerDist > 200f && returnSpeed < 10f) //200 and 8
            {
                returnSpeed = 10f; //8
            }
            if (playerDist < safeDist && returning && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
            {
                projectile.ai[0] = 0f;
                projectile.netUpdate = true;
            }
            if (playerDist > 2000f)
            {
                projectile.position.X = player.Center.X - (float)(projectile.width / 2);
                projectile.position.Y = player.Center.Y - (float)(projectile.height / 2);
                projectile.netUpdate = true;
            }
            if (playerDist > 70f)
            {
                playerVec.Normalize();
                playerVec *= returnSpeed;
                projectile.velocity = (projectile.velocity * 40f + playerVec) / 41f;
            }
            else if (projectile.velocity.X == 0f && projectile.velocity.Y == 0f)
            {
                projectile.velocity.X = -0.22f;
                projectile.velocity.Y = -0.12f;
            }

            if (projectile.ai[1] > 0f)
            {
                projectile.ai[1] += (float)Main.rand.Next(1, 4);
            }
            if (projectile.ai[1] > 220f)
            {
                projectile.ai[1] = 0f;
                projectile.netUpdate = true;
            }
            if (projectile.localAI[0] < 120f)
            {
                projectile.localAI[0] += 1f;
            }
            if (projectile.ai[0] == 0f)
            {
                int healProj = ModContent.ProjectileType<CactusHealOrb>();
                if (projectile.ai[1] == 0f && projectile.localAI[0] >= 120f)
                {
                    projectile.ai[1] += 1f;
                    if (Main.myPlayer == projectile.owner && player.statLife < player.statLifeMax2)
                    {
                        Main.PlaySound(SoundID.Dig, (int)projectile.position.X, (int)projectile.position.Y, 6);
                        int dustAmt = 36;
                        for (int d = 0; d < dustAmt; d++)
                        {
                            Vector2 source = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                            source = source.RotatedBy((double)((float)(d - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + projectile.Center;
                            Vector2 dustVel = source - projectile.Center;
                            int green = Dust.NewDust(source + dustVel, 0, 0, 107, dustVel.X * 1.5f, dustVel.Y * 1.5f, 100, new Color(0, 200, 0), 1f);
                            Main.dust[green].noGravity = true;
                            Main.dust[green].noLight = true;
                            Main.dust[green].velocity = dustVel;
                        }
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, -6f, healProj, 0, 0f, Main.myPlayer, 0f, 0f);
                    }
                }
            }
        }
    }
}
