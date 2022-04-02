using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class SandnadoMinion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sandnado");
            Main.projFrames[projectile.type] = 6;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 40;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 1f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (projectile.localAI[1] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                int dustAmt = 36;
                for (int d = 0; d < dustAmt; d++)
                {
                    Vector2 source = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                    source = source.RotatedBy((double)((float)(d - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + projectile.Center;
                    Vector2 dustVel = source - projectile.Center;
                    int sand = Dust.NewDust(source + dustVel, 0, 0, 85, dustVel.X * 1.5f, dustVel.Y * 1.5f, 100, default, 1.4f);
                    Main.dust[sand].noGravity = true;
                    Main.dust[sand].noLight = true;
                    Main.dust[sand].velocity = dustVel;
                }
                projectile.localAI[1] += 1f;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = damage2;
            }
            bool correctMinion = projectile.type == ModContent.ProjectileType<SandnadoMinion>();
            player.AddBuff(ModContent.BuffType<Sandnado>(), 3600);
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.sandnado = false;
                }
                if (modPlayer.sandnado)
                {
                    projectile.timeLeft = 2;
                }
            }
            projectile.MinionAntiClump(0.1f);
            if (Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
            {
                projectile.alpha += 20;
                if (projectile.alpha > 150)
                {
                    projectile.alpha = 150;
                }
            }
            else
            {
                projectile.alpha -= 50;
                if (projectile.alpha < 60)
                {
                    projectile.alpha = 60;
                }
            }
            Vector2 targetPos = projectile.position;
            float range = 1800f;
            bool foundTarget = false;
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(projectile, false))
                {
                    float npcDist = Vector2.Distance(npc.Center, projectile.Center);
                    if ((!foundTarget && npcDist < range) && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height))
                    {
                        range = npcDist;
                        targetPos = npc.Center;
                        foundTarget = true;
                        int num11 = npc.whoAmI;
                    }
                }
            }
            if (!foundTarget)
            {
                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    NPC npc = Main.npc[k];
                    if (npc.CanBeChasedBy(projectile, false))
                    {
                        float npcDist = Vector2.Distance(npc.Center, projectile.Center);
                        if ((!foundTarget && npcDist < range) && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height))
                        {
                            range = npcDist;
                            targetPos = npc.Center;
                            foundTarget = true;
                        }
                    }
                }
            }
            float sepAnxietyDist = 1200f;
            if (foundTarget)
            {
                sepAnxietyDist = 3000f;
            }
            if (Vector2.Distance(player.Center, projectile.Center) > sepAnxietyDist)
            {
                projectile.ai[0] = 1f;
                projectile.netUpdate = true;
            }
            if (foundTarget && projectile.ai[0] == 0f)
            {
                Vector2 targetDir = targetPos - projectile.Center;
                float targetDist = targetDir.Length();
                targetDir.Normalize();
                if (targetDist > 400f)
                {
                    float speedMult = 6f;
                    targetDir *= speedMult;
                    projectile.velocity = (projectile.velocity * 20f + targetDir) / 21f;
                }
                else
                {
                    projectile.velocity *= 0.96f;
                }
                if (targetDist > 200f)
                {
                    float speedMult = 12f;
                    targetDir *= speedMult;
                    projectile.velocity = (projectile.velocity * 40f + targetDir) / 41f;
                }
                if (projectile.velocity.Y > -1f)
                {
                    projectile.velocity.Y -= 0.1f;
                }
            }
            else
            {
                if (!Collision.CanHitLine(projectile.Center, 1, 1, player.Center, 1, 1))
                {
                    projectile.ai[0] = 1f;
                }
                float returnSpeed = 12f;
                Vector2 playerVec = player.Center - projectile.Center + new Vector2(0f, -20f);
                float playerDist = playerVec.Length();
                if (playerDist > 200f && returnSpeed < 12f)
                {
                    returnSpeed = 12f;
                }
                if (playerDist < 100f && projectile.ai[0] == 1f && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                {
                    projectile.ai[0] = 0f;
                    projectile.netUpdate = true;
                }
                if (playerDist > 2000f)
                {
                    projectile.position = player.position;
                    projectile.netUpdate = true;
                }
                if (Math.Abs(playerVec.X) > 40f || Math.Abs(playerVec.Y) > 10f)
                {
                    playerVec.Normalize();
                    playerVec *= returnSpeed;
                    playerVec *= new Vector2(1.25f, 0.65f);
                    projectile.velocity = (projectile.velocity * 20f + playerVec) / 21f;
                }
                else
                {
                    if (projectile.velocity.X == 0f && projectile.velocity.Y == 0f)
                    {
                        projectile.velocity.X = -0.15f;
                        projectile.velocity.Y = -0.05f;
                    }
                    projectile.velocity *= 1.01f;
                }
            }
            projectile.rotation = projectile.velocity.X * 0.05f;
            projectile.frameCounter++;
            int two = 2;
            if (projectile.frameCounter >= 6 * two)
            {
                projectile.frameCounter = 0;
            }
            projectile.frame = projectile.frameCounter / two;
            if (Main.rand.NextBool(5))
            {
                int sand = Dust.NewDust(projectile.position, projectile.width, projectile.height, 85, 0f, 0f, 100, default, 2f);
                Main.dust[sand].velocity *= 0.3f;
                Main.dust[sand].noGravity = true;
                Main.dust[sand].noLight = true;
            }
            if (projectile.velocity.X > 0f)
            {
                projectile.spriteDirection = projectile.direction = -1;
            }
            else if (projectile.velocity.X < 0f)
            {
                projectile.spriteDirection = projectile.direction = 1;
            }
            if (projectile.ai[1] > 0f)
            {
                projectile.ai[1] += 1f;
                if (Main.rand.Next(3) != 0)
                {
                    projectile.ai[1] += 1f;
                }
            }
            if (projectile.ai[1] > 75f)
            {
                projectile.ai[1] = 0f;
                projectile.netUpdate = true;
            }
            if (projectile.ai[0] == 0f)
            {
                float speed = 14f;
                int projType = ModContent.ProjectileType<MiniSandShark>();
                if (foundTarget)
                {
                    if (!Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                    {
                        if (projectile.ai[1] == 0f)
                        {
                            projectile.ai[1] += 1f;
                            if (Main.myPlayer == projectile.owner)
                            {
                                Vector2 velocity = targetPos - projectile.Center;
                                velocity.Normalize();
                                velocity *= speed;
                                int shark = Projectile.NewProjectile(projectile.Center, velocity, projType, projectile.damage, projectile.knockBack, projectile.owner);
                                Main.projectile[shark].netUpdate = true;
                                projectile.netUpdate = true;
                            }
                        }
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            int frameHeight = texture.Height / Main.projFrames[projectile.type];
            int frame = frameHeight * projectile.frame;
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, frame, texture.Width, frameHeight)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture.Width / 2f, (float)frameHeight / 2f), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool CanDamage() => false;
    }
}
