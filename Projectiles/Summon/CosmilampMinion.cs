using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class CosmilampMinion : ModProjectile
    {
        public override string Texture => "CalamityMod/NPCs/Signus/CosmicLantern";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmilamp");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.minionSlots = 2f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            projectile.tileCollide = false;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 24;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                int num226 = 36;
                for (int num227 = 0; num227 < num226; num227++)
                {
                    Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                    vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + projectile.Center;
                    Vector2 vector7 = vector6 - projectile.Center;
                    int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 204, vector7.X * 1.5f, vector7.Y * 1.5f, 100, default, 1.4f);
                    Main.dust[num228].noGravity = true;
                    Main.dust[num228].noLight = true;
                    Main.dust[num228].velocity = vector7;
                }
                projectile.localAI[0] += 1f;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = damage2;
            }

            projectile.frameCounter++;
            if (projectile.frameCounter >= 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }

            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.75f / 255f, (255 - projectile.alpha) * 0f / 255f, (255 - projectile.alpha) * 0.75f / 255f);
            float num395 = (float)Main.mouseTextColor / 200f - 0.35f;
            num395 *= 0.2f;
            projectile.scale = num395 + 0.95f;
            int num1262 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 204, 0f, 0f, 0, default, 1f);
            Main.dust[num1262].velocity *= 0.1f;
            Main.dust[num1262].scale = 0.7f;
            Main.dust[num1262].noGravity = true;
            bool flag64 = projectile.type == ModContent.ProjectileType<CosmilampMinion>();
            player.AddBuff(ModContent.BuffType<CosmilampBuff>(), 3600);
            if (flag64)
            {
                if (player.dead)
                {
                    modPlayer.cLamp = false;
                }
                if (modPlayer.cLamp)
                {
                    projectile.timeLeft = 2;
                }
            }
            projectile.MinionAntiClump();
            float num535 = projectile.position.X;
            float num536 = projectile.position.Y;
            float num537 = 3000f;
            bool flag19 = false;
            int num538 = 2500;
            if (projectile.ai[1] != 0f)
            {
                num538 = 4000;
            }
            if (Math.Abs(projectile.Center.X - Main.player[projectile.owner].Center.X) + Math.Abs(projectile.Center.Y - Main.player[projectile.owner].Center.Y) > (float)num538)
            {
                projectile.ai[0] = 1f;
            }
            if (projectile.ai[0] == 0f)
            {
                if (player.HasMinionAttackTargetNPC)
                {
                    NPC npc = Main.npc[player.MinionAttackTargetNPC];
                    if (npc.CanBeChasedBy(projectile, false))
                    {
                        float num539 = npc.position.X + (float)(npc.width / 2);
                        float num540 = npc.position.Y + (float)(npc.height / 2);
                        float num541 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num539) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num540);
                        if (num541 < num537 && Collision.CanHit(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height))
                        {
                            num535 = num539;
                            num536 = num540;
                            flag19 = true;
                        }
                    }
                }
                if (!flag19)
                {
                    for (int num542 = 0; num542 < Main.maxNPCs; num542++)
                    {
                        if (Main.npc[num542].CanBeChasedBy(projectile, false))
                        {
                            float num543 = Main.npc[num542].position.X + (float)(Main.npc[num542].width / 2);
                            float num544 = Main.npc[num542].position.Y + (float)(Main.npc[num542].height / 2);
                            float num545 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num543) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num544);
                            if (num545 < num537)
                            {
                                num537 = num545;
                                num535 = num543;
                                num536 = num544;
                                flag19 = true;
                            }
                        }
                    }
                }
            }
            if (!flag19)
            {
                float num546 = 12f;
                if (projectile.ai[0] == 1f)
                {
                    num546 = 18f;
                }
                Vector2 vector42 = projectile.Center;
                float num547 = player.Center.X - vector42.X;
                float num548 = player.Center.Y - vector42.Y - 60f;
                float num549 = (float)Math.Sqrt((double)(num547 * num547 + num548 * num548));
                if (num549 < 400f && projectile.ai[0] == 1f && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                {
                    projectile.ai[0] = 0f;
                }
                if (num549 > 2000f)
                {
                    projectile.position.X = Main.player[projectile.owner].Center.X - (float)(projectile.width / 2);
                    projectile.position.Y = Main.player[projectile.owner].Center.Y - (float)(projectile.width / 2);
                }
                if (num549 > 70f)
                {
                    num549 = num546 / num549;
                    num547 *= num549;
                    num548 *= num549;
                    projectile.velocity.X = (projectile.velocity.X * 20f + num547) / 21f;
                    projectile.velocity.Y = (projectile.velocity.Y * 20f + num548) / 21f;
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
                projectile.rotation = projectile.velocity.X * 0.05f;
                if ((double)Math.Abs(projectile.velocity.X) > 0.2)
                {
                    projectile.spriteDirection = -projectile.direction;
                    return;
                }
            }
            else
            {
                if (projectile.ai[1] == -1f)
                {
                    projectile.ai[1] = 17f;
                }
                if (projectile.ai[1] > 0f)
                {
                    projectile.ai[1] -= 1f;
                }
                if (projectile.ai[1] == 0f)
                {
                    float num550 = 24f; //12
                    Vector2 vector43 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                    float num551 = num535 - vector43.X;
                    float num552 = num536 - vector43.Y;
                    float num553 = (float)Math.Sqrt((double)(num551 * num551 + num552 * num552));
                    if (num553 < 100f)
                    {
                        num550 = 28f; //14
                    }
                    num553 = num550 / num553;
                    num551 *= num553;
                    num552 *= num553;
                    projectile.velocity.X = (projectile.velocity.X * 14f + num551) / 15f;
                    projectile.velocity.Y = (projectile.velocity.Y * 14f + num552) / 15f;
                }
                else
                {
                    if (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y) < 10f)
                    {
                        projectile.velocity *= 1.05f;
                    }
                }
                projectile.rotation = projectile.velocity.X * 0.05f;
                if (Math.Abs(projectile.velocity.X) > 0.2f)
                {
                    projectile.spriteDirection = -projectile.direction;
                    return;
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Rectangle myRect = new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height);
            if (projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < 200; i++)
                {
                    if (Main.npc[i].active && !Main.npc[i].dontTakeDamage &&
                        ((projectile.friendly && (!Main.npc[i].friendly || projectile.type == 318 || (Main.npc[i].type == NPCID.Guide && projectile.owner < 255 && Main.player[projectile.owner].killGuide) || (Main.npc[i].type == NPCID.Clothier && projectile.owner < 255 && Main.player[projectile.owner].killClothier))) ||
                        (projectile.hostile && Main.npc[i].friendly && !Main.npc[i].dontTakeDamageFromHostiles)) && (projectile.owner < 0 || Main.npc[i].immune[projectile.owner] == 0 || projectile.maxPenetrate == 1))
                    {
                        if (Main.npc[i].noTileCollide || !projectile.ownerHitCheck || projectile.CanHit(Main.npc[i]))
                        {
                            bool flag3;
                            if (Main.npc[i].type == NPCID.SolarCrawltipedeTail)
                            {
                                Rectangle rect = Main.npc[i].getRect();
                                int num5 = 8;
                                rect.X -= num5;
                                rect.Y -= num5;
                                rect.Width += num5 * 2;
                                rect.Height += num5 * 2;
                                flag3 = projectile.Colliding(myRect, rect);
                            }
                            else
                            {
                                flag3 = projectile.Colliding(myRect, Main.npc[i].getRect());
                            }
                            if (flag3)
                            {
                                if (Main.npc[i].reflectingProjectiles && projectile.CanReflect())
                                {
                                    Main.npc[i].ReflectProjectile(projectile.whoAmI);
                                    return;
                                }
                                projectile.ai[1] = -1f;
                                projectile.netUpdate = true;
                            }
                        }
                    }
                }
            }
        }
    }
}
