using CalamityMod.Buffs.Alcohol;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityMod.Items.Placeables.Furniture.DevPaintings;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using CalamityMod.Items.Potions.Alcohol;

namespace CalamityMod.NPCs.Crabulon
{
    [AutoloadBossHead]
    public class Crabulon : ModNPC
    {
        private int biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;
        private bool stomping = false;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Scale = 0.32f,
                PortraitScale = 0.55f,
                PortraitPositionYOverride = 54f
            };
            value.Position.Y += 80f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
			NPCID.Sets.MPAllowedEnemies[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.npcSlots = 14f;
            NPC.GetNPCDamage();
            NPC.width = 196;
            NPC.height = 196;
            NPC.defense = 8;
            NPC.LifeMaxNERB(3350, 4000, 680000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.boss = true;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 10, 0, 0);
            NPC.HitSound = SoundID.NPCHit45;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;

            if (Main.getGoodWorld)
            {
                NPC.scale *= 1.5f;
                NPC.defense += 12;
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundMushroom,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Crabulon")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(biomeEnrageTimer);
            writer.Write(NPC.localAI[0]);
            writer.Write(stomping);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            biomeEnrageTimer = reader.ReadInt32();
            NPC.localAI[0] = reader.ReadSingle();
            stomping = reader.ReadBoolean();
        }

        public override void AI()
        {
            Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 0f, 0.3f, 0.7f);

            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool expertMode = Main.expertMode || bossRush;

            NPC.spriteDirection = NPC.direction;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Phases
            bool phase2 = lifeRatio < 0.66f && expertMode;
            bool phase3 = lifeRatio < 0.33f && expertMode;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];
            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead)
                {
                    NPC.noTileCollide = true;

                    if (NPC.velocity.Y < -3f)
                        NPC.velocity.Y = -3f;
                    NPC.velocity.Y += 0.1f;
                    if (NPC.velocity.Y > 12f)
                        NPC.velocity.Y = 12f;

                    if (NPC.timeLeft > 60)
                        NPC.timeLeft = 60;

                    if (NPC.ai[0] != 0f)
                    {
                        NPC.ai[0] = 0f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.ai[3] = 0f;
                        NPC.netUpdate = true;
                    }
                    return;
                }
            }
            else if (NPC.timeLeft < 1800)
                NPC.timeLeft = 1800;

            // Enrage
            if ((!player.ZoneGlowshroom || (NPC.position.Y / 16f) < Main.worldSurface) && !bossRush)
            {
                if (biomeEnrageTimer > 0)
                    biomeEnrageTimer--;
            }
            else
                biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;

            bool biomeEnraged = biomeEnrageTimer <= 0 || bossRush;

            float enrageScale = bossRush ? 1f : 0f;
            if (biomeEnraged && ((NPC.position.Y / 16f) < Main.worldSurface || bossRush))
            {
                NPC.Calamity().CurrentlyEnraged = !bossRush;
                enrageScale += 1f;
            }
            if (biomeEnraged && (!player.ZoneGlowshroom || bossRush))
            {
                NPC.Calamity().CurrentlyEnraged = !bossRush;
                enrageScale += 1f;
            }

            if (Main.getGoodWorld)
                enrageScale += 0.5f;

            if (NPC.ai[0] < 2f)
            {
                Vector2 vector34 = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);
                int num352 = 1;
                NPC.localAI[3] += 2f;
                if (phase2)
                {
                    NPC.localAI[3] += 1f;
                    num352 += 2;
                }
                if (phase3)
                {
                    NPC.localAI[3] += 2f;
                    num352 += 3;
                }
                if (NPC.ai[3] == 0f)
                {
                    if (NPC.localAI[3] > 600f)
                    {
                        NPC.ai[3] = 1f;
                        NPC.localAI[3] = 0f;
                    }
                }
                else if (NPC.localAI[3] > 45f)
                {
                    NPC.localAI[3] = 0f;
                    NPC.ai[3] += 1f;
                    if (NPC.ai[3] >= num352)
                        NPC.ai[3] = 0f;

                    float num353 = 10f;
                    int type = ModContent.ProjectileType<MushBomb>();
                    SoundEngine.PlaySound(SoundID.Item42, NPC.Center);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (phase2)
                            num353 += 1f;
                        if (phase3)
                            num353 += 1f;

                        vector34 = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);
                        float num349 = player.position.X + player.width * 0.5f - vector34.X;
                        float num350 = player.position.Y + player.height * 0.5f - vector34.Y;
                        float num351 = (float)Math.Sqrt(num349 * num349 + num350 * num350);
                        num351 = num353 / num351;
                        num349 *= num351;
                        num350 *= num351;
                        vector34.X += num349;
                        vector34.Y += num350;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), vector34.X, vector34.Y, num349, num350 - 5f, type, NPC.GetProjectileDamage(type), 0f, Main.myPlayer, 0f, player.Center.Y);
                    }
                }
            }

            if (NPC.ai[0] == 0f)
            {
                NPC.velocity *= 0.98f;

                NPC.ai[1] += 1f;
                if (phase2)
                    NPC.ai[1] += 1f;
                if (phase3)
                    NPC.ai[1] += 2f;
                if (NPC.justHit)
                    NPC.ai[1] += 4f;

                if (NPC.ai[1] >= (death ? 150f : revenge ? 180f : 240f))
                {
                    NPC.TargetClosest();
                    NPC.noGravity = true;
                    NPC.noTileCollide = true;
                    NPC.ai[0] = 1f;
                    NPC.ai[1] = 0f;
                    NPC.netUpdate = true;
                }
            }
            else if (NPC.ai[0] == 1f)
            {
                float walkingVelocity = 2f;
                if (phase2)
                    walkingVelocity = 2.5f;
                if (phase3)
                    walkingVelocity = 3f;
                if (death)
                    walkingVelocity += 2f * (1f - lifeRatio);
                if (CalamityWorld.LegendaryMode && CalamityWorld.revenge)
                    walkingVelocity *= 2f;
                walkingVelocity += 2.5f * enrageScale;

                bool flag51 = false;
                if (Math.Abs(NPC.Center.X - player.Center.X) < 50f)
                    flag51 = true;

                if (flag51)
                {
                    NPC.velocity.X *= 0.9f;
                    if (NPC.velocity.X > -0.1 && NPC.velocity.X < 0.1)
                        NPC.velocity.X = 0f;
                }
                else
                {
                    float playerLocation = NPC.Center.X - player.Center.X;
                    NPC.direction = playerLocation < 0 ? 1 : -1;

                    if (NPC.direction > 0)
                        NPC.velocity.X = (NPC.velocity.X * 20f + walkingVelocity) / 21f;
                    if (NPC.direction < 0)
                        NPC.velocity.X = (NPC.velocity.X * 20f - walkingVelocity) / 21f;
                }

                if (Collision.CanHit(NPC.position, NPC.width, NPC.height, player.Center, 1, 1) && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height) && player.position.Y <= NPC.position.Y + NPC.height && !NPC.collideX)
                {
                    NPC.noGravity = false;
                    NPC.noTileCollide = false;
                }
                else
                {
                    NPC.noGravity = true;
                    NPC.noTileCollide = true;

                    int num854 = 80;
                    int num855 = 20;
                    Vector2 position2 = new Vector2(NPC.Center.X - (num854 / 2), NPC.position.Y + NPC.height - num855);

                    bool fallDownOnTopOfTarget = NPC.position.X < player.position.X && NPC.position.X + NPC.width > player.position.X + player.width && NPC.position.Y + NPC.height < player.position.Y + player.height - 16f;
                    if (fallDownOnTopOfTarget)
                    {
                        NPC.velocity.Y += 0.5f;
                    }
                    else if (Collision.SolidCollision(position2, num854, num855))
                    {
                        if (NPC.velocity.Y > 0f)
                            NPC.velocity.Y = 0f;

                        if (NPC.velocity.Y > -0.2f)
                            NPC.velocity.Y -= 0.025f;
                        else
                            NPC.velocity.Y -= 0.2f;

                        if (NPC.velocity.Y < -4f)
                            NPC.velocity.Y = -4f;
                    }
                    else
                    {
                        if (NPC.velocity.Y < 0f)
                            NPC.velocity.Y = 0f;

                        if (NPC.velocity.Y < 0.1f)
                            NPC.velocity.Y += 0.025f;
                        else
                            NPC.velocity.Y += 0.5f;
                    }
                }

                NPC.ai[1] += 1f;
                if (NPC.ai[1] >= (360f - (death ? 120f * (1f - lifeRatio) : 0f)))
                {
                    NPC.TargetClosest();
                    NPC.noGravity = false;
                    NPC.noTileCollide = false;
                    NPC.ai[0] = 2f;
                    NPC.ai[1] = 0f;
                    NPC.netUpdate = true;
                }

                if (NPC.velocity.Y > 10f)
                    NPC.velocity.Y = 10f;
            }
            else if (NPC.ai[0] == 2f)
            {
                NPC.noTileCollide = false;
                if (NPC.velocity.Y == 0f)
                {
                    NPC.velocity.X *= 0.8f;
                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] % 15f == 14f)
                        NPC.netUpdate = true;

                    if (NPC.ai[1] > 0f)
                    {
                        if (revenge)
                        {
                            switch ((int)NPC.ai[3])
                            {
                                case 0:
                                    break;
                                case 1:
                                case 2:
                                    NPC.ai[1] += 3f;
                                    break;
                                case 3:
                                    NPC.ai[1] += 6f;
                                    break;
                                default:
                                    break;
                            }
                        }
                        if (phase2)
                            NPC.ai[1] += !revenge ? 4f : 1f;
                        if (phase3)
                            NPC.ai[1] += !revenge ? 4f : 1f;
                    }

                    float jumpGateValue = (bossRush ? 40f : 120f) / (enrageScale + 1f);
                    if (NPC.ai[1] >= jumpGateValue)
                    {
                        NPC.ai[1] = -20f;
                    }
                    else if (NPC.ai[1] == -1f)
                    {
                        int velocityX = 4;
                        float velocityY = -12f;

                        float distanceBelowTarget = NPC.position.Y - (player.position.Y + 80f);
                        float speedMult = 1f;

                        if (revenge)
                        {
                            switch ((int)NPC.ai[3])
                            {
                                case 0: // Normal
                                    break;
                                case 1: // High
                                    velocityY -= 4f;
                                    break;
                                case 2: // Low
                                    velocityY += 4f;
                                    break;
                                case 3: // Long and low
                                    velocityX += 4;
                                    velocityY += 4f;
                                    break;
                                default:
                                    break;
                            }

                            if (distanceBelowTarget > 0f)
                                speedMult += distanceBelowTarget * 0.001f;

                            if (speedMult > 2f)
                                speedMult = 2f;

                            velocityY *= speedMult;
                        }

                        if (expertMode)
                        {
                            if (player.position.Y < NPC.Bottom.Y)
                                NPC.velocity.Y = velocityY;
                            else
                                NPC.velocity.Y = 1f;

                            NPC.noTileCollide = true;
                        }
                        else
                            NPC.velocity.Y = velocityY;

                        float playerLocation = NPC.Center.X - player.Center.X;
                        NPC.direction = playerLocation < 0 ? 1 : -1;

                        NPC.velocity.X = velocityX * NPC.direction;

                        NPC.ai[0] = 3f;
                        NPC.ai[1] = 0f;
                        NPC.netUpdate = true;
                    }
                }
            }
            else
            {
                if (NPC.velocity.Y == 0f)
                {
                    SoundEngine.PlaySound(SoundID.Item14, NPC.Center);

                    int type = ModContent.ProjectileType<MushBombFall>();
                    int damage = NPC.GetProjectileDamage(type);

                    if (NPC.ai[2] % 2f == 0f && phase2 && revenge)
                    {
                        SoundEngine.PlaySound(SoundID.Item42, NPC.Center);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float projectileVelocity = BossRushEvent.BossRushActive ? 24f : CalamityWorld.death ? 12f : 10f;
                            Vector2 destination = new Vector2(NPC.Center.X, NPC.Center.Y - 100f) - NPC.Center;
                            destination.Normalize();
                            destination *= projectileVelocity;
                            int numProj = bossRush ? 48 : CalamityWorld.death ? 24 : 20;
                            float rotation = MathHelper.ToRadians(90);
                            for (int i = 0; i < numProj; i++)
                            {
                                Vector2 perturbedSpeed = destination.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                                Vector2 randomVelocityVector = new Vector2((Main.rand.NextFloat() - 0.5f) * 4f, (Main.rand.NextFloat() - 0.5f) * 4f);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(perturbedSpeed.X, -projectileVelocity) + randomVelocityVector, type, damage, 0f, Main.myPlayer, 0f, player.Center.Y);
                            }
                        }
                    }

                    NPC.TargetClosest();
                    NPC.ai[2] += 1f;
                    if (NPC.ai[2] >= (phase2 ? 4f : 3f))
                    {
                        if (revenge && !phase2)
                        {
                            SoundEngine.PlaySound(SoundID.Item42, NPC.Center);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                float projectileVelocity = BossRushEvent.BossRushActive ? 24f : CalamityWorld.death ? 12f : 10f;
                                Vector2 destination = new Vector2(NPC.Center.X, NPC.Center.Y - 100f) - NPC.Center;
                                destination.Normalize();
                                destination *= projectileVelocity;
                                int numProj = bossRush ? 30 : CalamityWorld.death ? 15 : 11;
                                float rotation = MathHelper.ToRadians(60);
                                for (int i = 0; i < numProj; i++)
                                {
                                    Vector2 perturbedSpeed = destination.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                                    Vector2 randomVelocityVector = new Vector2((Main.rand.NextFloat() - 0.5f) * 4f, (Main.rand.NextFloat() - 0.5f) * 4f);
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(perturbedSpeed.X, -projectileVelocity) + randomVelocityVector, type, damage, 0f, Main.myPlayer, 0f, player.Center.Y);
                                }
                            }
                        }

                        NPC.ai[0] = 0f;
                        NPC.ai[2] = 0f;
                        if (revenge)
                            NPC.ai[3] = 0f;

                        NPC.netUpdate = true;
                    }
                    else
                    {
                        float playerLocation = NPC.Center.X - player.Center.X;
                        NPC.direction = playerLocation < 0 ? 1 : -1;

                        NPC.ai[0] = 2f;
                        if (revenge)
                            NPC.ai[3] += 1f;

                        NPC.netUpdate = true;
                    }

                    for (int num622 = (int)NPC.position.X - 20; num622 < (int)NPC.position.X + NPC.width + 40; num622 += 20)
                    {
                        for (int num623 = 0; num623 < 4; num623++)
                        {
                            int num624 = Dust.NewDust(new Vector2(NPC.position.X - 20f, NPC.position.Y + NPC.height), NPC.width + 20, 4, 56, 0f, 0f, 100, default, 1.5f);
                            Main.dust[num624].velocity *= 0.2f;
                        }
                        if (Main.zenithWorld)
                        {
                            int x = num622 / 16;
                            int y = (int)(NPC.position.Y + NPC.height) / 16;
                            Tile groundTile = CalamityUtils.ParanoidTileRetrieval(x, y);
                            Tile walkTile = CalamityUtils.ParanoidTileRetrieval(x, y - 1);
                            if (!walkTile.HasTile && walkTile.LiquidAmount == 0 && groundTile != null && WorldGen.SolidTile(groundTile))
                            {
                                walkTile.TileFrameY = 0;
                                walkTile.Get<TileWallWireStateData>().Slope = SlopeType.Solid;
                                walkTile.Get<TileWallWireStateData>().IsHalfBlock = false;
                                if (groundTile.TileType == TileID.MushroomGrass || groundTile.TileType == TileID.Mud)
                                {
                                    walkTile.Get<TileWallWireStateData>().HasTile = true;
                                    walkTile.TileType = TileID.MushroomPlants;
                                    walkTile.TileFrameX = (short)(Main.rand.Next(5) * 18);

                                    if (Main.netMode == NetmodeID.MultiplayerClient)
                                        NetMessage.SendTileSquare(-1, x, y - 1, 1, TileChangeType.None);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (!player.dead && expertMode)
                    {
                        if ((player.position.Y > NPC.Bottom.Y && NPC.velocity.Y > 0f) || (player.position.Y < NPC.Bottom.Y && NPC.velocity.Y < 0f))
                            NPC.noTileCollide = true;
                        else if ((NPC.velocity.Y > 0f && NPC.Bottom.Y > Main.player[NPC.target].Top.Y) || (Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].Center, 1, 1) && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height)))
                            NPC.noTileCollide = false;
                    }

                    if (NPC.position.X < player.position.X && NPC.position.X + NPC.width > player.position.X + player.width)
                    {
                        NPC.velocity.X *= 0.9f;
                        NPC.velocity.Y += death ? 0.18f : 0.15f;
                    }
                    else
                    {
                        float velocityX = 0.11f +
                            (expertMode ? 0.02f : 0f) +
                            (revenge ? 0.02f : 0f) +
                            (death ? 0.02f : 0f);
                        velocityX += 0.05f * enrageScale;

                        if (NPC.direction < 0)
                            NPC.velocity.X -= velocityX;
                        else if (NPC.direction > 0)
                            NPC.velocity.X += velocityX;

                        float num626 = 2.5f;
                        num626 += enrageScale;
                        if (revenge)
                        {
                            num626 += 1f;
                        }
                        if (phase2)
                        {
                            num626 += 1f;
                        }
                        if (phase3)
                        {
                            num626 += 1f;
                        }
                        if (NPC.velocity.X < -num626)
                        {
                            NPC.velocity.X = -num626;
                        }
                        if (NPC.velocity.X > num626)
                        {
                            NPC.velocity.X = num626;
                        }
                    }
                }
            }

            if (NPC.localAI[0] == 0f && NPC.life > 0)
            {
                NPC.localAI[0] = NPC.lifeMax;
            }
            if (NPC.life > 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int num660 = (int)(NPC.lifeMax * ((CalamityWorld.LegendaryMode && CalamityWorld.revenge) ? 0.01 : Main.getGoodWorld ? 0.025 : 0.05));
                    if ((NPC.life + num660) < NPC.localAI[0])
                    {
                        NPC.localAI[0] = NPC.life;
                        int num661 = death ? 3 : expertMode ? Main.rand.Next(2, 4) : 2;
                        for (int num662 = 0; num662 < num661; num662++)
                        {
                            int x = (int)(NPC.position.X + Main.rand.Next(NPC.width - 32));
                            int y = (int)(NPC.position.Y + Main.rand.Next(NPC.height - 32));
                            int num663 = ModContent.NPCType<CrabShroom>();
                            int num664 = NPC.NewNPC(NPC.GetSource_FromAI(), x, y, num663);
                            Main.npc[num664].SetDefaults(num663);
                            Main.npc[num664].velocity.X = Main.rand.Next(-50, 51) * (Main.getGoodWorld ? 0.2f : 0.1f);
                            Main.npc[num664].velocity.Y = Main.rand.Next(-50, -31) * (Main.getGoodWorld ? 0.2f : 0.1f);
                            if (Main.netMode == NetmodeID.Server && num664 < Main.maxNPCs)
                            {
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num664, 0f, 0f, 0f, 0, 0, 0);
                            }
                        }
                    }
                }
            }
        }

        // Can only hit the target if within certain distance
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            Vector2 npcCenter = NPC.Center;

            // NOTE: Right and left hitboxes are interchangeable, each hitbox is the same size and is located to the right or left of the center hitbox.
            Rectangle leftHitbox = new Rectangle((int)(npcCenter.X - (NPC.width / 2f) + 6f * NPC.scale), (int)(npcCenter.Y - (NPC.height / 4f)), NPC.width / 4, NPC.height / 2);
            Rectangle bodyHitbox = new Rectangle((int)(npcCenter.X - (NPC.width / 4f)), (int)(npcCenter.Y - (NPC.height / 2f)), NPC.width / 2, NPC.height);
            Rectangle rightHitbox = new Rectangle((int)(npcCenter.X + (NPC.width / 4f) - 6f * NPC.scale), (int)(npcCenter.Y - (NPC.height / 4f)), NPC.width / 4, NPC.height / 2);

            Vector2 leftHitboxCenter = new Vector2(leftHitbox.X + (leftHitbox.Width / 2), leftHitbox.Y + (leftHitbox.Height / 2));
            Vector2 bodyHitboxCenter = new Vector2(bodyHitbox.X + (bodyHitbox.Width / 2), bodyHitbox.Y + (bodyHitbox.Height / 2));
            Vector2 rightHitboxCenter = new Vector2(rightHitbox.X + (rightHitbox.Width / 2), rightHitbox.Y + (rightHitbox.Height / 2));

            Rectangle targetHitbox = target.Hitbox;

            float leftDist1 = Vector2.Distance(leftHitboxCenter, targetHitbox.TopLeft());
            float leftDist2 = Vector2.Distance(leftHitboxCenter, targetHitbox.TopRight());
            float leftDist3 = Vector2.Distance(leftHitboxCenter, targetHitbox.BottomLeft());
            float leftDist4 = Vector2.Distance(leftHitboxCenter, targetHitbox.BottomRight());

            float minLeftDist = leftDist1;
            if (leftDist2 < minLeftDist)
                minLeftDist = leftDist2;
            if (leftDist3 < minLeftDist)
                minLeftDist = leftDist3;
            if (leftDist4 < minLeftDist)
                minLeftDist = leftDist4;

            bool insideLeftHitbox = minLeftDist <= 45f * NPC.scale;

            float bodyDist1 = Vector2.Distance(bodyHitboxCenter, targetHitbox.TopLeft());
            float bodyDist2 = Vector2.Distance(bodyHitboxCenter, targetHitbox.TopRight());
            float bodyDist3 = Vector2.Distance(bodyHitboxCenter, targetHitbox.BottomLeft());
            float bodyDist4 = Vector2.Distance(bodyHitboxCenter, targetHitbox.BottomRight());

            float minBodyDist = bodyDist1;
            if (bodyDist2 < minBodyDist)
                minBodyDist = bodyDist2;
            if (bodyDist3 < minBodyDist)
                minBodyDist = bodyDist3;
            if (bodyDist4 < minBodyDist)
                minBodyDist = bodyDist4;

            bool insideBodyHitbox = minBodyDist <= 90f * NPC.scale;

            float rightDist1 = Vector2.Distance(rightHitboxCenter, targetHitbox.TopLeft());
            float rightDist2 = Vector2.Distance(rightHitboxCenter, targetHitbox.TopRight());
            float rightDist3 = Vector2.Distance(rightHitboxCenter, targetHitbox.BottomLeft());
            float rightDist4 = Vector2.Distance(rightHitboxCenter, targetHitbox.BottomRight());

            float minRightDist = rightDist1;
            if (rightDist2 < minRightDist)
                minRightDist = rightDist2;
            if (rightDist3 < minRightDist)
                minRightDist = rightDist3;
            if (rightDist4 < minRightDist)
                minRightDist = rightDist4;

            bool insideRightHitbox = minRightDist <= 45f * NPC.scale;

            return insideLeftHitbox || insideBodyHitbox || insideRightHitbox;
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.ai[0] > 1f)
            {
                if (NPC.velocity.Y == 0f && NPC.ai[1] >= 0f && NPC.ai[0] == 2f) // Idle just before jump
                {
                    if (stomping)
                        stomping = false;

                    NPC.frameCounter += 0.15;
                    NPC.frameCounter %= Main.npcFrameCount[NPC.type];
                    int frame = (int)NPC.frameCounter;
                    NPC.frame.Y = frame * frameHeight;
                }
                else if (NPC.velocity.Y <= 0f || NPC.ai[1] < 0f) // Prepare to jump and then jump
                {
                    NPC.frameCounter += 1D;
                    if (NPC.frameCounter > 12D)
                    {
                        NPC.frame.Y += frameHeight;
                        NPC.frameCounter = 0D;
                    }
                    if (NPC.frame.Y >= frameHeight)
                        NPC.frame.Y = frameHeight;
                }
                else // Stomping
                {
                    if (!stomping)
                    {
                        stomping = true;
                        NPC.frameCounter = 0D;
                    }

                    NPC.frameCounter += 1D;
                    if (NPC.frameCounter > 8D)
                    {
                        NPC.frame.Y += frameHeight;
                        NPC.frameCounter = 0D;
                    }
                    if (NPC.frame.Y >= frameHeight * 5)
                        NPC.frame.Y = frameHeight * 5;
                }
            }
            else
            {
                if (stomping)
                    stomping = false;

                NPC.frameCounter += 0.15f;
                NPC.frameCounter %= Main.npcFrameCount[NPC.type];
                int frame = (int)NPC.frameCounter;
                NPC.frame.Y = frame * frameHeight;
            }
        }

        public override Color? GetAlpha(Color drawColor) => Main.zenithWorld ? new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, drawColor.A) * NPC.Opacity : null;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D textureIdle = TextureAssets.Npc[NPC.type].Value;
            Texture2D glowIdle = ModContent.Request<Texture2D>("CalamityMod/NPCs/Crabulon/CrabulonGlow").Value;
            Texture2D textureWalk = ModContent.Request<Texture2D>("CalamityMod/NPCs/Crabulon/CrabulonAlt").Value;
            Texture2D glowWalk = ModContent.Request<Texture2D>("CalamityMod/NPCs/Crabulon/CrabulonAltGlow").Value;
            Texture2D textureAttack = ModContent.Request<Texture2D>("CalamityMod/NPCs/Crabulon/CrabulonAttack").Value;
            Texture2D glowAttack = ModContent.Request<Texture2D>("CalamityMod/NPCs/Crabulon/CrabulonAttackGlow").Value;
            Color colorToShift = Main.zenithWorld ? new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB) : Color.Cyan;
            Color glowColor = Color.Lerp(Color.White, colorToShift, 0.5f);

            int ClonesOnEachSide = Main.zenithWorld ? 2 : 0;
            for (int c = 0 - ClonesOnEachSide; c < 1 + ClonesOnEachSide; c++)
            {
                Vector2 drawOrigin = new Vector2(textureIdle.Width / 2, textureIdle.Height / Main.npcFrameCount[NPC.type] / 2);
                Vector2 drawPos = NPC.Center - screenPos + (Vector2.UnitX * textureIdle.Width * c * 1.6f);
                // Jumping
                if (NPC.ai[0] > 2f && NPC.velocity.Y != 0f)
                {
                    drawOrigin = new Vector2(textureAttack.Width / 2, textureAttack.Height / 2);
                    drawPos -= new Vector2(textureAttack.Width, textureAttack.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                    drawPos += drawOrigin * NPC.scale + new Vector2(0f, NPC.gfxOffY);

                    spriteBatch.Draw(textureAttack, drawPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0f);
                    spriteBatch.Draw(glowAttack, drawPos, NPC.frame, glowColor, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0f);
                }
                // Walking
                else if (NPC.ai[0] == 1f)
                {
                    drawOrigin = new Vector2(textureWalk.Width / 2, textureWalk.Height / 2);
                    drawPos -= new Vector2(textureWalk.Width, textureWalk.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                    drawPos += drawOrigin * NPC.scale + new Vector2(0f, NPC.gfxOffY);

                    spriteBatch.Draw(textureWalk, drawPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0f);
                    spriteBatch.Draw(glowWalk, drawPos, NPC.frame, glowColor, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0f);
                }
                // Standing still
                else
                {
                    drawPos -= new Vector2(textureIdle.Width, textureIdle.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                    drawPos += drawOrigin * NPC.scale + new Vector2(0f, NPC.gfxOffY);

                    spriteBatch.Draw(textureIdle, drawPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0f);
                    spriteBatch.Draw(glowIdle, drawPos, NPC.frame, glowColor, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0f);
                }
            }
            return false;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<CrabulonBag>()));

            // Normal drops: Everything that would otherwise be in the bag
            var normalOnly = npcLoot.DefineNormalOnlyDropSet();
            {
                // Weapons
                int[] weapons = new int[]
                {
                    ModContent.ItemType<MycelialClaws>(),
                    ModContent.ItemType<Fungicide>(),
                    ModContent.ItemType<HyphaeRod>(),
                    ModContent.ItemType<Mycoroot>(),
                    ModContent.ItemType<InfestedClawmerang>(),
                    ModContent.ItemType<PuffShroom>(),
                };
                normalOnly.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, weapons));

                // Equipment
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<FungalClump>()));

                // Vanity
                normalOnly.Add(ModContent.ItemType<CrabulonMask>(), 7);
                normalOnly.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
            }

            npcLoot.Add(ModContent.ItemType<CrabulonTrophy>(), 10);

            // Relic
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<CrabulonRelic>());

            // GFB Odd Mushroom drop
            npcLoot.DefineConditionalDropSet(DropHelper.GFB).Add(ModContent.ItemType<OddMushroom>(), 1, 1, 9999);

            // Lore
            npcLoot.AddConditionalPerPlayer(() => !DownedBossSystem.downedCrabulon, ModContent.ItemType<LoreCrabulon>(), desc: DropHelper.FirstKillText);
        }

        public override void OnKill()
        {
            CalamityGlobalNPC.SetNewBossJustDowned(NPC);

            // Mark Crabulon as dead
            DownedBossSystem.downedCrabulon = true;
            CalamityNetcode.SyncWorld();

            if (Main.zenithWorld && Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < Main.rand.Next(10, 23); i++)
                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-NPC.width / 2, NPC.width / 2), (int)NPC.Center.Y + Main.rand.Next(-NPC.height / 2, NPC.height / 2), NPCID.Crab);
            }
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance);
            NPC.damage = (int)(NPC.damage * NPC.GetExpertDamageMultiplier());
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 56, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                NPC.position.X = NPC.position.X + (NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (NPC.height / 2);
                NPC.width = (int)(200 * NPC.scale);
                NPC.height = (int)(100 * NPC.scale);
                NPC.position.X = NPC.position.X - (NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (NPC.height / 2);
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 56, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 70; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 56, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 56, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    float randomSpread = Main.rand.Next(-200, 201) / 100f;
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("Crabulon").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("Crabulon2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("Crabulon3").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("Crabulon4").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("Crabulon5").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("Crabulon6").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("Crabulon7").Type, NPC.scale);
                }
            }
        }
    }
}
