using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class Shellfish : ModProjectile
    {
        private int playerStill = 0;
        private bool fly = false;
        private bool spawnDust = true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shellfish");
            Main.projFrames[projectile.type] = 2;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 24;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.minionSlots = 2;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 120;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = false;
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            CalamityGlobalProjectile modProj = projectile.Calamity();
            if (spawnDust)
            {
                modProj.spawnedPlayerMinionDamageValue = player.MinionDamage();
                modProj.spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                int dustAmt = 20;
                for (int d = 0; d < dustAmt; d++)
                {
                    int water = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 16f), projectile.width, projectile.height - 16, 33, 0f, 0f, 0, default, 1f);
                    Main.dust[water].velocity *= 2f;
                    Main.dust[water].scale *= 1.15f;
                }
                spawnDust = false;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)modProj.spawnedPlayerMinionProjectileDamageValue /
                    modProj.spawnedPlayerMinionDamageValue * player.MinionDamage());
                projectile.damage = damage2;
            }

            bool correctMinion = projectile.type == ModContent.ProjectileType<Shellfish>();
            player.AddBuff(ModContent.BuffType<ShellfishBuff>(), 3600);
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.shellfish = false;
                }
                if (modPlayer.shellfish)
                {
                    projectile.timeLeft = 2;
                }
            }

            projectile.frameCounter++;
            if (projectile.frameCounter > 3)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 1)
            {
                projectile.frame = 0;
            }

            if (projectile.ai[0] == 0f)
            {
                projectile.ai[1] += 1f;
                if (!fly)
                {
                    projectile.tileCollide = true;
                    Vector2 playerVec = player.Center - projectile.Center;
                    float playerDistance = playerVec.Length();
                    if (projectile.velocity.Y == 0f && (projectile.velocity.X != 0f || playerDistance > 200f))
                    {
                        float jumpHeight = Utils.SelectRandom(Main.rand, new float[]
                        {
                            5f,
                            7.5f,
                            10f
                        });
                        projectile.velocity.Y -= jumpHeight;
                    }
                    projectile.velocity.Y += 0.3f;
                    float maxDistance = 1000f;
                    bool chaseNPC = false;
                    float npcPositionX = 0f;
                    if (player.HasMinionAttackTargetNPC)
                    {
                        NPC npc = Main.npc[player.MinionAttackTargetNPC];
                        if (npc.CanBeChasedBy(projectile, false))
                        {
                            float npcDist = Vector2.Distance(npc.Center, projectile.Center);
                            if (!chaseNPC && npcDist < maxDistance)
                            {
                                npcPositionX = npc.Center.X;
                                chaseNPC = true;
                            }
                        }
                    }
                    if (!chaseNPC)
                    {
                        for (int index = 0; index < Main.maxNPCs; index++)
                        {
                            NPC npc = Main.npc[index];
                            if (npc.CanBeChasedBy(projectile, false))
                            {
                                float npcDist = Vector2.Distance(npc.Center, projectile.Center);
                                if (!chaseNPC && npcDist < maxDistance)
                                {
                                    npcPositionX = npc.Center.X;
                                    chaseNPC = true;
                                }
                            }
                        }
                    }
                    if (chaseNPC)
                    {
                        if (npcPositionX - projectile.position.X > 0f)
                        {
                            float rightDist = Utils.SelectRandom(Main.rand, new float[]
                            {
                                0.15f,
                                0.2f
                            });
                            projectile.velocity.X += rightDist;

                            if (projectile.velocity.X > 8f)
                            {
                                projectile.velocity.X = 8f;
                            }
                        }
                        else
                        {
                            float leftDist = Utils.SelectRandom(Main.rand, new float[]
                            {
                                0.15f,
                                0.2f
                            });
                            projectile.velocity.X -= leftDist;

                            if (projectile.velocity.X < -8f)
                            {
                                projectile.velocity.X = -8f;
                            }
                        }
                    }
                    else
                    {
                        if (playerDistance > 800f)
                        {
                            fly = true;
                            projectile.velocity.X = 0f;
                            projectile.velocity.Y = 0f;
                            projectile.tileCollide = false;
                        }
                        if (playerDistance > 200f)
                        {
                            if (player.position.X - projectile.position.X > 0f)
                            {
                                float rightDist = Utils.SelectRandom(Main.rand, new float[]
                                {
                                    0.05f,
                                    0.1f,
                                    0.15f
                                });
                                projectile.velocity.X += rightDist;

                                if (projectile.velocity.X > 6f)
                                {
                                    projectile.velocity.X = 6f;
                                }
                            }
                            else
                            {
                                float leftDist = Utils.SelectRandom(Main.rand, new float[]
                                {
                                    0.05f,
                                    0.1f,
                                    0.15f
                                });
                                projectile.velocity.X -= leftDist;

                                if (projectile.velocity.X < -6f)
                                {
                                    projectile.velocity.X = -6f;
                                }
                            }
                        }
                        if (playerDistance < 200f)
                        {
                            if (projectile.velocity.X != 0f)
                            {
                                if (projectile.velocity.X > 0.5f)
                                {
                                    float leftDist = Utils.SelectRandom(Main.rand, new float[]
                                    {
                                        0.05f,
                                        0.1f,
                                        0.15f
                                    });
                                    projectile.velocity.X -= leftDist;
                                }
                                else if (projectile.velocity.X < -0.5f)
                                {
                                    float rightDist = Utils.SelectRandom(Main.rand, new float[]
                                    {
                                        0.05f,
                                        0.1f,
                                        0.15f
                                    });
                                    projectile.velocity.X += rightDist;
                                }
                                else if (Math.Abs(projectile.velocity.X) < 0.5f)
                                {
                                    projectile.velocity.X = 0f;
                                }
                            }
                        }
                    }
                }
                else if (fly)
                {
                    Vector2 playerVec = player.Center - projectile.Center + new Vector2(0f, 0f);
                    float playerDistance = playerVec.Length();
                    playerVec.Normalize();
                    playerVec *= 14f;
                    projectile.velocity = (projectile.velocity * 40f + playerVec) / 41f;

                    projectile.rotation = projectile.velocity.X * 0.03f;
                    if (playerDistance > 1500f)
                    {
                        projectile.Center = player.Center;
                        projectile.netUpdate = true;
                    }
                    if (playerDistance < 100f)
                    {
                        if (player.velocity.Y == 0f)
                        {
                            ++playerStill;
                        }
                        else
                        {
                            playerStill = 0;
                        }
                        if (playerStill > 30 && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                        {
                            fly = false;
                            projectile.tileCollide = true;
                            projectile.rotation = 0;
                            projectile.velocity.X *= 0.3f;
                            projectile.velocity.Y *= 0.3f;
                        }
                    }
                }
                if (projectile.velocity.X > 0.25f)
                {
                    projectile.spriteDirection = -1;
                }
                else if (projectile.velocity.X < -0.25f)
                {
                    projectile.spriteDirection = 1;
                }
            }
            if (projectile.ai[0] == 1f)
            {
                projectile.rotation = 0;
                projectile.tileCollide = false;
                bool breakAway = false;
                bool spawnDust = false;
                projectile.localAI[0] += 1f;
                if (projectile.localAI[0] % 30f == 0f)
                {
                    spawnDust = true;
                }
                int npcIndex = (int)projectile.ai[1];
                NPC host = Main.npc[npcIndex];
                if (projectile.localAI[0] >= 600000f) //tryna make it stay on there "forever" without glitching
                {
                    breakAway = true;
                }
                else if (npcIndex < 0 || npcIndex >= Main.maxNPCs)
                {
                    breakAway = true;
                }
                else if (host.active && !host.dontTakeDamage && host.defense < 9999)
                {
                    projectile.Center = host.Center - projectile.velocity * 2f;
                    projectile.gfxOffY = host.gfxOffY;
                    if (spawnDust)
                    {
                        host.HitEffect(0, 1.0);
                    }
                }
                else
                {
                    breakAway = true;
                }
                if (breakAway)
                {
                    projectile.ai[0] = 0f;
                    projectile.localAI[0] = 0f;
                    projectile.velocity.X = 0f;
                    projectile.velocity.Y = 0f;
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Player player = Main.player[projectile.owner];
            Rectangle myRect = new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height);

            if (projectile.owner == Main.myPlayer)
            {
                for (int npcIndex = 0; npcIndex < Main.maxNPCs; npcIndex++)
                {
                    NPC npc = Main.npc[npcIndex];
                    //covers most edge cases like voodoo dolls
                    if (npc.active && !npc.dontTakeDamage && npc.defense < 9999 && npc.Calamity().DR < 0.99f &&
                        ((projectile.friendly && (!npc.friendly || (npc.type == NPCID.Guide && projectile.owner < Main.maxPlayers && player.killGuide) || (npc.type == NPCID.Clothier && projectile.owner < Main.maxPlayers && player.killClothier))) ||
                        (projectile.hostile && npc.friendly && !npc.dontTakeDamageFromHostiles)) && (projectile.owner < 0 || npc.immune[projectile.owner] == 0 || projectile.maxPenetrate == 1))
                    {
                        if (npc.noTileCollide || !projectile.ownerHitCheck || projectile.CanHit(npc))
                        {
                            bool stickingToNPC;
                            //Solar Crawltipede tail has special collision
                            if (npc.type == NPCID.SolarCrawltipedeTail)
                            {
                                Rectangle rect = npc.getRect();
                                int num5 = 8;
                                rect.X -= num5;
                                rect.Y -= num5;
                                rect.Width += num5 * 2;
                                rect.Height += num5 * 2;
                                stickingToNPC = projectile.Colliding(myRect, rect);
                            }
                            else
                            {
                                stickingToNPC = projectile.Colliding(myRect, npc.getRect());
                            }
                            if (stickingToNPC)
                            {
                                //reflect projectile if the npc can reflect it (like Selenians)
                                if (npc.reflectingProjectiles && projectile.CanReflect())
                                {
                                    npc.ReflectProjectile(projectile.whoAmI);
                                    return;
                                }

                                //let the projectile know it is sticking and the npc it is sticking too
                                projectile.ai[0] = 1f;
                                projectile.ai[1] = npcIndex;

                                //follow the NPC
                                projectile.velocity = (npc.Center - projectile.Center) * 0.75f;

                                projectile.netUpdate = true;

                                //Count how many projectiles are attached, delete as necessary
                                Point[] array2 = new Point[10];
                                int projCount = 0;
                                for (int projIndex = 0; projIndex < Main.maxProjectiles; projIndex++)
                                {
                                    Projectile proj = Main.projectile[projIndex];
                                    if (projIndex != projectile.whoAmI && proj.active && proj.owner == Main.myPlayer && proj.type == projectile.type && proj.ai[0] == 1f && proj.ai[1] == (float)npcIndex)
                                    {
                                        array2[projCount++] = new Point(projIndex, proj.timeLeft);
                                        if (projCount >= array2.Length)
                                        {
                                            break;
                                        }
                                    }
                                }
                                if (projCount >= array2.Length)
                                {
                                    int num30 = 0;
                                    for (int m = 1; m < array2.Length; m++)
                                    {
                                        if (array2[m].Y < array2[num30].Y)
                                        {
                                            num30 = m;
                                        }
                                    }
                                    Main.projectile[array2[num30].X].Kill();
                                }
                            }
                        }
                    }
                }
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
            {
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            }
            return null;
        }

        public override bool CanDamage() => projectile.ai[0] == 0f;

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.buffImmune[ModContent.BuffType<ShellfishEating>()] = target.Calamity().DR >= 0.99f;
            target.AddBuff(ModContent.BuffType<ShellfishEating>(), 600000);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;
    }
}
