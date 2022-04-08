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
            Main.projFrames[Projectile.type] = 2;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 24;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.minionSlots = 2;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 120;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            CalamityGlobalProjectile modProj = Projectile.Calamity();
            if (spawnDust)
            {
                modProj.spawnedPlayerMinionDamageValue = player.MinionDamage();
                modProj.spawnedPlayerMinionProjectileDamageValue = Projectile.damage;
                int dustAmt = 20;
                for (int d = 0; d < dustAmt; d++)
                {
                    int water = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y + 16f), Projectile.width, Projectile.height - 16, 33, 0f, 0f, 0, default, 1f);
                    Main.dust[water].velocity *= 2f;
                    Main.dust[water].scale *= 1.15f;
                }
                spawnDust = false;
            }
            if (player.MinionDamage() != Projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)modProj.spawnedPlayerMinionProjectileDamageValue /
                    modProj.spawnedPlayerMinionDamageValue * player.MinionDamage());
                Projectile.damage = damage2;
            }

            bool correctMinion = Projectile.type == ModContent.ProjectileType<Shellfish>();
            player.AddBuff(ModContent.BuffType<ShellfishBuff>(), 3600);
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.shellfish = false;
                }
                if (modPlayer.shellfish)
                {
                    Projectile.timeLeft = 2;
                }
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 3)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 1)
            {
                Projectile.frame = 0;
            }

            if (Projectile.ai[0] == 0f)
            {
                Projectile.ai[1] += 1f;
                if (!fly)
                {
                    Projectile.tileCollide = true;
                    Vector2 playerVec = player.Center - Projectile.Center;
                    float playerDistance = playerVec.Length();
                    if (Projectile.velocity.Y == 0f && (Projectile.velocity.X != 0f || playerDistance > 200f))
                    {
                        float jumpHeight = Utils.SelectRandom(Main.rand, new float[]
                        {
                            5f,
                            7.5f,
                            10f
                        });
                        Projectile.velocity.Y -= jumpHeight;
                    }
                    Projectile.velocity.Y += 0.3f;
                    float maxDistance = 1000f;
                    bool chaseNPC = false;
                    float npcPositionX = 0f;
                    if (player.HasMinionAttackTargetNPC)
                    {
                        NPC npc = Main.npc[player.MinionAttackTargetNPC];
                        if (npc.CanBeChasedBy(Projectile, false))
                        {
                            float npcDist = Vector2.Distance(npc.Center, Projectile.Center);
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
                            if (npc.CanBeChasedBy(Projectile, false))
                            {
                                float npcDist = Vector2.Distance(npc.Center, Projectile.Center);
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
                        if (npcPositionX - Projectile.position.X > 0f)
                        {
                            float rightDist = Utils.SelectRandom(Main.rand, new float[]
                            {
                                0.15f,
                                0.2f
                            });
                            Projectile.velocity.X += rightDist;

                            if (Projectile.velocity.X > 8f)
                            {
                                Projectile.velocity.X = 8f;
                            }
                        }
                        else
                        {
                            float leftDist = Utils.SelectRandom(Main.rand, new float[]
                            {
                                0.15f,
                                0.2f
                            });
                            Projectile.velocity.X -= leftDist;

                            if (Projectile.velocity.X < -8f)
                            {
                                Projectile.velocity.X = -8f;
                            }
                        }
                    }
                    else
                    {
                        if (playerDistance > 800f)
                        {
                            fly = true;
                            Projectile.velocity.X = 0f;
                            Projectile.velocity.Y = 0f;
                            Projectile.tileCollide = false;
                        }
                        if (playerDistance > 200f)
                        {
                            if (player.position.X - Projectile.position.X > 0f)
                            {
                                float rightDist = Utils.SelectRandom(Main.rand, new float[]
                                {
                                    0.05f,
                                    0.1f,
                                    0.15f
                                });
                                Projectile.velocity.X += rightDist;

                                if (Projectile.velocity.X > 6f)
                                {
                                    Projectile.velocity.X = 6f;
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
                                Projectile.velocity.X -= leftDist;

                                if (Projectile.velocity.X < -6f)
                                {
                                    Projectile.velocity.X = -6f;
                                }
                            }
                        }
                        if (playerDistance < 200f)
                        {
                            if (Projectile.velocity.X != 0f)
                            {
                                if (Projectile.velocity.X > 0.5f)
                                {
                                    float leftDist = Utils.SelectRandom(Main.rand, new float[]
                                    {
                                        0.05f,
                                        0.1f,
                                        0.15f
                                    });
                                    Projectile.velocity.X -= leftDist;
                                }
                                else if (Projectile.velocity.X < -0.5f)
                                {
                                    float rightDist = Utils.SelectRandom(Main.rand, new float[]
                                    {
                                        0.05f,
                                        0.1f,
                                        0.15f
                                    });
                                    Projectile.velocity.X += rightDist;
                                }
                                else if (Math.Abs(Projectile.velocity.X) < 0.5f)
                                {
                                    Projectile.velocity.X = 0f;
                                }
                            }
                        }
                    }
                }
                else if (fly)
                {
                    Vector2 playerVec = player.Center - Projectile.Center + new Vector2(0f, 0f);
                    float playerDistance = playerVec.Length();
                    playerVec.Normalize();
                    playerVec *= 14f;
                    Projectile.velocity = (Projectile.velocity * 40f + playerVec) / 41f;

                    Projectile.rotation = Projectile.velocity.X * 0.03f;
                    if (playerDistance > 1500f)
                    {
                        Projectile.Center = player.Center;
                        Projectile.netUpdate = true;
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
                        if (playerStill > 30 && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                        {
                            fly = false;
                            Projectile.tileCollide = true;
                            Projectile.rotation = 0;
                            Projectile.velocity.X *= 0.3f;
                            Projectile.velocity.Y *= 0.3f;
                        }
                    }
                }
                if (Projectile.velocity.X > 0.25f)
                {
                    Projectile.spriteDirection = -1;
                }
                else if (Projectile.velocity.X < -0.25f)
                {
                    Projectile.spriteDirection = 1;
                }
            }
            if (Projectile.ai[0] == 1f)
            {
                Projectile.rotation = 0;
                Projectile.tileCollide = false;
                bool breakAway = false;
                bool spawnDust = false;
                Projectile.localAI[0] += 1f;
                if (Projectile.localAI[0] % 30f == 0f)
                {
                    spawnDust = true;
                }
                int npcIndex = (int)Projectile.ai[1];
                NPC host = Main.npc[npcIndex];
                if (Projectile.localAI[0] >= 600000f) //tryna make it stay on there "forever" without glitching
                {
                    breakAway = true;
                }
                else if (npcIndex < 0 || npcIndex >= Main.maxNPCs)
                {
                    breakAway = true;
                }
                else if (host.active && !host.dontTakeDamage && host.defense < 9999)
                {
                    Projectile.Center = host.Center - Projectile.velocity * 2f;
                    Projectile.gfxOffY = host.gfxOffY;
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
                    Projectile.ai[0] = 0f;
                    Projectile.localAI[0] = 0f;
                    Projectile.velocity.X = 0f;
                    Projectile.velocity.Y = 0f;
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Player player = Main.player[Projectile.owner];
            Rectangle myRect = new Rectangle((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height);

            if (Projectile.owner == Main.myPlayer)
            {
                for (int npcIndex = 0; npcIndex < Main.maxNPCs; npcIndex++)
                {
                    NPC npc = Main.npc[npcIndex];
                    //covers most edge cases like voodoo dolls
                    if (npc.active && !npc.dontTakeDamage && npc.defense < 9999 && npc.Calamity().DR < 0.99f &&
                        ((Projectile.friendly && (!npc.friendly || (npc.type == NPCID.Guide && Projectile.owner < Main.maxPlayers && player.killGuide) || (npc.type == NPCID.Clothier && Projectile.owner < Main.maxPlayers && player.killClothier))) ||
                        (Projectile.hostile && npc.friendly && !npc.dontTakeDamageFromHostiles)) && (Projectile.owner < 0 || npc.immune[Projectile.owner] == 0 || Projectile.maxPenetrate == 1))
                    {
                        if (npc.noTileCollide || !Projectile.ownerHitCheck)
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
                                stickingToNPC = Projectile.Colliding(myRect, rect);
                            }
                            else
                            {
                                stickingToNPC = Projectile.Colliding(myRect, npc.getRect());
                            }
                            if (stickingToNPC)
                            {
                                //reflect projectile if the npc can reflect it (like Selenians)
                                if (npc.reflectsProjectiles && Projectile.CanBeReflected())
                                {
                                    npc.ReflectProjectile(Projectile);
                                    return;
                                }

                                //let the projectile know it is sticking and the npc it is sticking too
                                Projectile.ai[0] = 1f;
                                Projectile.ai[1] = npcIndex;

                                //follow the NPC
                                Projectile.velocity = (npc.Center - Projectile.Center) * 0.75f;

                                Projectile.netUpdate = true;

                                //Count how many projectiles are attached, delete as necessary
                                Point[] array2 = new Point[10];
                                int projCount = 0;
                                for (int projIndex = 0; projIndex < Main.maxProjectiles; projIndex++)
                                {
                                    Projectile proj = Main.projectile[projIndex];
                                    if (projIndex != Projectile.whoAmI && proj.active && proj.owner == Main.myPlayer && proj.type == Projectile.type && proj.ai[0] == 1f && proj.ai[1] == (float)npcIndex)
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

        public override bool? CanDamage() => Projectile.ai[0] == 0f;

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.buffImmune[ModContent.BuffType<ShellfishEating>()] = target.Calamity().DR >= 0.99f;
            target.AddBuff(ModContent.BuffType<ShellfishEating>(), 600000);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;
    }
}
