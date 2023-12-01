using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityMod.Items.Placeables.Furniture.DevPaintings;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.TreasureBags.MiscGrabBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.NPCs.DesertScourge
{
    [AutoloadBossHead]
    public class DesertScourgeHead : ModNPC
    {
        private int biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;
        private bool TailSpawned = false;
        public bool playRoarSound = false;

        public static readonly SoundStyle RoarSound = new("CalamityMod/Sounds/Custom/DesertScourgeRoar");

        public override void SetStaticDefaults()
        {
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Scale = 0.75f,
                PortraitScale = 0.6f,
                CustomTexturePath = "CalamityMod/ExtraTextures/Bestiary/DesertScourge_Bestiary",
                PortraitPositionXOverride = 40,
                PortraitPositionYOverride = 40
            };
            value.Position.X += 95;
            value.Position.Y += 45;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
			NPCID.Sets.MPAllowedEnemies[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.GetNPCDamage();
            NPC.defense = 4;
            NPC.npcSlots = 12f;
            NPC.width = 32;
            NPC.height = 80;

            NPC.LifeMaxNERB(2500, 3000, 1650000);
            if (Main.getGoodWorld)
                NPC.lifeMax *= 4;

            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.value = Item.buyPrice(0, 5, 0, 0);
            NPC.alpha = 255;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.netAlways = true;

            if (BossRushEvent.BossRushActive)
                NPC.scale *= 1.25f;
            else if (CalamityWorld.death)
                NPC.scale *= 1.2f;
            else if (CalamityWorld.revenge)
                NPC.scale *= 1.15f;
            else if (Main.expertMode)
                NPC.scale *= 1.1f;

            if (Main.getGoodWorld)
                NPC.scale *= 0.4f;

            if (Main.zenithWorld)
                NPC.scale *= 4f;


            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Desert,
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.DesertScourge")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(biomeEnrageTimer);
            writer.Write(playRoarSound);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            biomeEnrageTimer = reader.ReadInt32();
            playRoarSound = reader.ReadBoolean();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void AI()
        {
            bool bossRush = BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool death = CalamityWorld.death || bossRush;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            // Enrage
            if (!player.ZoneDesert && !bossRush)
            {
                if (biomeEnrageTimer > 0)
                    biomeEnrageTimer--;
            }
            else
                biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;

            bool biomeEnraged = biomeEnrageTimer <= 0 || bossRush;

            float enrageScale = bossRush ? 1f : 0f;
            if (biomeEnraged)
            {
                NPC.Calamity().CurrentlyEnraged = !bossRush;
                enrageScale += 2f;
            }

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            if (revenge || lifeRatio < (expertMode ? 0.75f : 0.5f))
                NPC.Calamity().newAI[0] += 1f;

            float burrowTimeGateValue = death ? 420f : 540f;
            bool burrow = NPC.Calamity().newAI[0] >= burrowTimeGateValue;
            bool resetTime = NPC.Calamity().newAI[0] >= burrowTimeGateValue + 600f;
            bool lungeUpward = burrow && NPC.Calamity().newAI[1] == 1f;
            bool quickFall = NPC.Calamity().newAI[1] == 2f;

            float speed = 0.09f;
            float turnSpeed = 0.06f;

            if (expertMode)
            {
                float velocityScale = death ? 0.12f : 0.06f;
                speed += velocityScale * (1f - lifeRatio);
                float accelerationScale = death ? 0.075f : 0.05f;
                turnSpeed += accelerationScale * (1f - lifeRatio);
            }

            speed += 0.12f * enrageScale;
            turnSpeed += 0.06f * enrageScale;

            if (Main.getGoodWorld)
            {
                speed *= 1.1f;
                turnSpeed *= 1.2f;
            }

            if (lungeUpward)
            {
                speed *= 1.25f;
                turnSpeed *= 1.5f;

                if (NPC.Calamity().newAI[3] == 0f)
                    NPC.Calamity().newAI[3] = player.Center.Y - 600f;
            }

            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];

            NPC.alpha -= 42;
            if (NPC.alpha < 0)
                NPC.alpha = 0;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                // become moist if in an aquatic biome
                if (Main.zenithWorld && (player.ZoneBeach || player.Calamity().ZoneAbyss || player.Calamity().ZoneSunkenSea || player.Calamity().ZoneSulphur) && NPC.CountNPCS(ModContent.NPCType<AquaticScourgeHead>()) < 1)
                {
                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2), (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<AquaticScourgeHead>());
                    NPC.active = false;
                }
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!TailSpawned && NPC.ai[0] == 0f)
                {
                    int Previous = NPC.whoAmI;
                    int minLength = death ? 40 : revenge ? 35 : expertMode ? 30 : 25;
                    if (Main.getGoodWorld)
                        minLength *= 3;

                    for (int i = 0; i < minLength + 1; i++)
                    {
                        int lol;
                        if (i >= 0 && i < minLength)
                        {
                            lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2), (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<DesertScourgeBody>(), NPC.whoAmI);
                        }
                        else
                        {
                            lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2), (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<DesertScourgeTail>(), NPC.whoAmI);
                        }
                        Main.npc[lol].ai[2] = NPC.whoAmI;
                        Main.npc[lol].realLife = NPC.whoAmI;
                        Main.npc[lol].ai[1] = Previous;
                        Main.npc[Previous].ai[0] = lol;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, lol, 0f, 0f, 0f, 0);
                        Previous = lol;
                    }
                    TailSpawned = true;
                }
            }

            if (expertMode)
            {
                if (NPC.Calamity().newAI[2] < 180f)
                    NPC.Calamity().newAI[2] += 1f;

                if (NPC.SafeDirectionTo(player.Center).AngleBetween((NPC.rotation - MathHelper.PiOver2).ToRotationVector2()) < MathHelper.ToRadians(18f) &&
                    NPC.Calamity().newAI[2] >= 180f && Vector2.Distance(NPC.Center, player.Center) > 320f &&
                    Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height))
                {
                    if (NPC.Calamity().newAI[2] % 30f == 0f)
                    {
                        SoundEngine.PlaySound(SoundID.Item21, NPC.Center);

                        float velocity = bossRush ? 6f : death ? 5.5f : 5f;
                        int type = ModContent.ProjectileType<SandBlast>();
                        int type2 = ModContent.ProjectileType<HorsWaterBlast>();
                        int damage = NPC.GetProjectileDamage(type);
                        Vector2 projectileVelocity = Vector2.Normalize(player.Center - NPC.Center) * velocity;
                        int baseProjectileAmt = bossRush ? 6 : death ? 4 : revenge ? 3 : 2;
                        int numProj = NPC.Calamity().newAI[2] % 60f == 0f ? (int)(baseProjectileAmt * 1.5) : baseProjectileAmt;
                        int spread = bossRush ? 18 : death ? 14 : revenge ? 12 : 10;
                        float rotation = MathHelper.ToRadians(spread);
                        for (int i = 0; i < numProj; i++)
                        {
                            Vector2 perturbedSpeed = projectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                            if (CalamityWorld.LegendaryMode && CalamityWorld.revenge)
                                perturbedSpeed *= Main.rand.NextFloat() + 0.5f;

                            for (int k = 0; k < 10; k++)
                                Dust.NewDust(NPC.Center + Vector2.Normalize(perturbedSpeed) * 5f, 10, 10, 85, perturbedSpeed.X, perturbedSpeed.Y);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.Normalize(perturbedSpeed) * 5f, perturbedSpeed, type, damage, 0f, Main.myPlayer);
                                if (Main.zenithWorld)
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.Normalize(perturbedSpeed) * 3f, perturbedSpeed, type2, damage, 0f, Main.myPlayer);
                            }
                        }
                    }

                    NPC.Calamity().newAI[2] += 1f;
                    if (NPC.Calamity().newAI[2] > 240f)
                        NPC.Calamity().newAI[2] = 0f;
                }
            }

            int tilePositionX = (int)(NPC.position.X / 16f) - 1;
            int tileWidthPosX = (int)((NPC.position.X + (float)NPC.width) / 16f) + 2;
            int tilePositionY = (int)(NPC.position.Y / 16f) - 1;
            int tileWidthPosY = (int)((NPC.position.Y + (float)NPC.height) / 16f) + 2;
            if (tilePositionX < 0)
            {
                tilePositionX = 0;
            }
            if (tileWidthPosX > Main.maxTilesX)
            {
                tileWidthPosX = Main.maxTilesX;
            }
            if (tilePositionY < 0)
            {
                tilePositionY = 0;
            }
            if (tileWidthPosY > Main.maxTilesY)
            {
                tileWidthPosY = Main.maxTilesY;
            }
            bool shouldFly = lungeUpward;
            if (!shouldFly)
            {
                for (int k = tilePositionX; k < tileWidthPosX; k++)
                {
                    for (int l = tilePositionY; l < tileWidthPosY; l++)
                    {
                        if (Main.tile[k, l] != null && ((Main.tile[k, l].HasUnactuatedTile && (Main.tileSolid[(int)Main.tile[k, l].TileType] || (Main.tileSolidTop[(int)Main.tile[k, l].TileType] && Main.tile[k, l].TileFrameY == 0))) || Main.tile[k, l].LiquidAmount > 64))
                        {
                            Vector2 vector2;
                            vector2.X = (float)(k * 16);
                            vector2.Y = (float)(l * 16);
                            if (NPC.position.X + (float)NPC.width > vector2.X && NPC.position.X < vector2.X + 16f && NPC.position.Y + (float)NPC.height > vector2.Y && NPC.position.Y < vector2.Y + 16f)
                            {
                                shouldFly = true;
                                break;
                            }
                        }
                    }
                }
            }
            if (!shouldFly)
            {
                NPC.localAI[1] = 1f;
                Rectangle rectangle = new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height);
                int directChaseDistance = 1000;
                if (enrageScale > 0f)
                    directChaseDistance = 100;

                bool shouldDirectlyChase = true;
                if (NPC.position.Y > player.position.Y)
                {
                    int rectWidth = directChaseDistance * 2;
                    int rectHeight = directChaseDistance * 2;
                    for (int m = 0; m < 255; m++)
                    {
                        if (Main.player[m].active)
                        {
                            int rectX = (int)Main.player[m].position.X - directChaseDistance;
                            int rectY = (int)Main.player[m].position.Y - directChaseDistance;
                            Rectangle directChaseRect = new Rectangle(rectX, rectY, rectWidth, rectHeight);
                            if (rectangle.Intersects(directChaseRect))
                            {
                                shouldDirectlyChase = false;
                                break;
                            }
                        }
                    }
                    if (shouldDirectlyChase)
                    {
                        shouldFly = true;
                    }
                }
            }
            else
            {
                NPC.localAI[1] = 0f;
            }

            float maxChaseSpeed = 16f;
            if (player.dead)
            {
                shouldFly = false;
                NPC.velocity.Y += 1f;
                if ((double)NPC.position.Y > Main.worldSurface * 16.0)
                {
                    NPC.velocity.Y += 1f;
                    maxChaseSpeed = 32f;
                }
                if ((double)NPC.position.Y > Main.rockLayer * 16.0)
                {
                    for (int a = 0; a < Main.maxNPCs; a++)
                    {
                        if (Main.npc[a].type == ModContent.NPCType<DesertScourgeHead>() || Main.npc[a].type == ModContent.NPCType<DesertScourgeBody>() ||
                            Main.npc[a].type == ModContent.NPCType<DesertScourgeTail>())
                        {
                            Main.npc[a].active = false;
                        }
                    }
                }
            }

            float speedCopy = speed;
            float turnSpeedCopy = turnSpeed;
            float burrowDistance = bossRush ? 500f : 800f;
            float burrowTarget = player.Center.Y + burrowDistance;
            float lungeTarget = NPC.Calamity().newAI[3];
            Vector2 npcCenter = NPC.Center;
            float playerX = player.Center.X;
            float targettingPosition = lungeUpward ? lungeTarget : burrow ? burrowTarget : player.Center.Y;
            playerX = (float)((int)(playerX / 16f) * 16);
            targettingPosition = (float)((int)(targettingPosition / 16f) * 16);
            npcCenter.X = (float)((int)(npcCenter.X / 16f) * 16);
            npcCenter.Y = (float)((int)(npcCenter.Y / 16f) * 16);
            playerX -= npcCenter.X;
            targettingPosition -= npcCenter.Y;
            float targetDistance = (float)Math.Sqrt((double)(playerX * playerX + targettingPosition * targettingPosition));

            // Lunge up towards target
            if (burrow && NPC.Center.Y >= burrowTarget - 16f)
            {
                NPC.Calamity().newAI[1] = 1f;
                NPC.Calamity().newAI[2] = 0f;
                if (!playRoarSound)
                {
                    SoundEngine.PlaySound(RoarSound, player.Center);
                    playRoarSound = true;
                }
            }

            // Quickly fall back down once above target
            if (lungeUpward && NPC.Center.Y <= NPC.Calamity().newAI[3] + 600f - 420f)
            {
                // Spit a huge spread of sand upwards that falls down
                SoundEngine.PlaySound(SoundID.NPCDeath13, NPC.Center);
                float velocity = (CalamityWorld.LegendaryMode && CalamityWorld.revenge) ? 16f : bossRush ? 9f : death ? 7f : 6f;
                int type = ModContent.ProjectileType<GreatSandBlast>();
                int damage = NPC.GetProjectileDamage(type);
                Vector2 projectileVelocity = Vector2.Normalize(NPC.Center + NPC.velocity * 10f - NPC.Center) * velocity;
                int numProj = bossRush ? 30 : death ? 24 : revenge ? 20 : expertMode ? 16 : 12;
                if (Main.getGoodWorld)
                    numProj *= 2;

                int spread = (CalamityWorld.LegendaryMode && CalamityWorld.revenge) ? 120 : 90;
                float rotation = MathHelper.ToRadians(spread);
                for (int i = 0; i < numProj; i++)
                {
                    Vector2 perturbedSpeed = projectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));

                    for (int k = 0; k < 10; k++)
                        Dust.NewDust(NPC.Center + Vector2.Normalize(perturbedSpeed) * 5f, 10, 10, 85, perturbedSpeed.X, perturbedSpeed.Y);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.Normalize(perturbedSpeed) * 5f, perturbedSpeed, type, damage, 0f, Main.myPlayer);
                }

                NPC.TargetClosest();
                NPC.Calamity().newAI[1] = 2f;
                playRoarSound = false;
            }

            // Quickly fall and reset variables once at target's Y position
            if (quickFall)
            {
                NPC.velocity.Y += Main.getGoodWorld ? 1f : 0.5f;
                if (NPC.Center.Y >= NPC.Calamity().newAI[3] + 600f)
                {
                    NPC.Calamity().newAI[0] = 0f;
                    NPC.Calamity().newAI[1] = 0f;
                    NPC.Calamity().newAI[3] = 0f;
                    playRoarSound = false;
                }
            }

            // Reset variables if the burrow and lunge attack is taking too long
            if (resetTime)
            {
                NPC.Calamity().newAI[0] = 0f;
                NPC.Calamity().newAI[1] = 0f;
            }

            if (!shouldFly)
            {
                NPC.TargetClosest();
                NPC.velocity.Y = NPC.velocity.Y + 0.15f;
                if (NPC.velocity.Y > maxChaseSpeed)
                {
                    NPC.velocity.Y = maxChaseSpeed;
                }
                if ((double)(Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < (double)maxChaseSpeed * 0.4)
                {
                    if (NPC.velocity.X < 0f)
                    {
                        NPC.velocity.X = NPC.velocity.X - speedCopy * 1.1f;
                    }
                    else
                    {
                        NPC.velocity.X = NPC.velocity.X + speedCopy * 1.1f;
                    }
                }
                else if (NPC.velocity.Y == maxChaseSpeed)
                {
                    if (NPC.velocity.X < playerX)
                    {
                        NPC.velocity.X = NPC.velocity.X + speedCopy;
                    }
                    else if (NPC.velocity.X > playerX)
                    {
                        NPC.velocity.X = NPC.velocity.X - speedCopy;
                    }
                }
                else if (NPC.velocity.Y > 4f)
                {
                    if (NPC.velocity.X < 0f)
                    {
                        NPC.velocity.X = NPC.velocity.X + speedCopy * 0.9f;
                    }
                    else
                    {
                        NPC.velocity.X = NPC.velocity.X - speedCopy * 0.9f;
                    }
                }
            }
            else
            {
                if (NPC.soundDelay == 0)
                {
                    float soundDelay = targetDistance / 40f;
                    if (soundDelay < 10f)
                    {
                        soundDelay = 10f;
                    }
                    if (soundDelay > 20f)
                    {
                        soundDelay = 20f;
                    }
                    NPC.soundDelay = (int)soundDelay;
                    SoundEngine.PlaySound(SoundID.WormDig, NPC.Center);
                }
                targetDistance = (float)Math.Sqrt((double)(playerX * playerX + targettingPosition * targettingPosition));
                float absolutePlayerX = Math.Abs(playerX);
                float absoluteTargetPos = Math.Abs(targettingPosition);
                float timeToReachTarget = maxChaseSpeed / targetDistance;
                playerX *= timeToReachTarget;
                targettingPosition *= timeToReachTarget;
                if (((NPC.velocity.X > 0f && playerX > 0f) || (NPC.velocity.X < 0f && playerX < 0f)) && ((NPC.velocity.Y > 0f && targettingPosition > 0f) || (NPC.velocity.Y < 0f && targettingPosition < 0f)))
                {
                    if (NPC.velocity.X < playerX)
                    {
                        NPC.velocity.X = NPC.velocity.X + turnSpeedCopy;
                    }
                    else if (NPC.velocity.X > playerX)
                    {
                        NPC.velocity.X = NPC.velocity.X - turnSpeedCopy;
                    }
                    if (NPC.velocity.Y < targettingPosition)
                    {
                        NPC.velocity.Y = NPC.velocity.Y + turnSpeedCopy;
                    }
                    else if (NPC.velocity.Y > targettingPosition)
                    {
                        NPC.velocity.Y = NPC.velocity.Y - turnSpeedCopy;
                    }
                }
                if ((NPC.velocity.X > 0f && playerX > 0f) || (NPC.velocity.X < 0f && playerX < 0f) || (NPC.velocity.Y > 0f && targettingPosition > 0f) || (NPC.velocity.Y < 0f && targettingPosition < 0f))
                {
                    if (NPC.velocity.X < playerX)
                    {
                        NPC.velocity.X = NPC.velocity.X + speedCopy;
                    }
                    else if (NPC.velocity.X > playerX)
                    {
                        NPC.velocity.X = NPC.velocity.X - speedCopy;
                    }
                    if (NPC.velocity.Y < targettingPosition)
                    {
                        NPC.velocity.Y = NPC.velocity.Y + speedCopy;
                    }
                    else if (NPC.velocity.Y > targettingPosition)
                    {
                        NPC.velocity.Y = NPC.velocity.Y - speedCopy;
                    }
                    if ((double)Math.Abs(targettingPosition) < (double)maxChaseSpeed * 0.2 && ((NPC.velocity.X > 0f && playerX < 0f) || (NPC.velocity.X < 0f && playerX > 0f)))
                    {
                        if (NPC.velocity.Y > 0f)
                        {
                            NPC.velocity.Y = NPC.velocity.Y + speedCopy * 2f;
                        }
                        else
                        {
                            NPC.velocity.Y = NPC.velocity.Y - speedCopy * 2f;
                        }
                    }
                    if ((double)Math.Abs(playerX) < (double)maxChaseSpeed * 0.2 && ((NPC.velocity.Y > 0f && targettingPosition < 0f) || (NPC.velocity.Y < 0f && targettingPosition > 0f)))
                    {
                        if (NPC.velocity.X > 0f)
                        {
                            NPC.velocity.X = NPC.velocity.X + speedCopy * 2f;
                        }
                        else
                        {
                            NPC.velocity.X = NPC.velocity.X - speedCopy * 2f;
                        }
                    }
                }
                else if (absolutePlayerX > absoluteTargetPos)
                {
                    if (NPC.velocity.X < playerX)
                    {
                        NPC.velocity.X = NPC.velocity.X + speedCopy * 1.1f;
                    }
                    else if (NPC.velocity.X > playerX)
                    {
                        NPC.velocity.X = NPC.velocity.X - speedCopy * 1.1f;
                    }
                    if ((double)(Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < (double)maxChaseSpeed * 0.5)
                    {
                        if (NPC.velocity.Y > 0f)
                        {
                            NPC.velocity.Y = NPC.velocity.Y + speedCopy;
                        }
                        else
                        {
                            NPC.velocity.Y = NPC.velocity.Y - speedCopy;
                        }
                    }
                }
                else
                {
                    if (NPC.velocity.Y < targettingPosition)
                    {
                        NPC.velocity.Y = NPC.velocity.Y + speedCopy * 1.1f;
                    }
                    else if (NPC.velocity.Y > targettingPosition)
                    {
                        NPC.velocity.Y = NPC.velocity.Y - speedCopy * 1.1f;
                    }
                    if ((double)(Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < (double)maxChaseSpeed * 0.5)
                    {
                        if (NPC.velocity.X > 0f)
                        {
                            NPC.velocity.X = NPC.velocity.X + speedCopy;
                        }
                        else
                        {
                            NPC.velocity.X = NPC.velocity.X - speedCopy;
                        }
                    }
                }
            }
            NPC.rotation = (float)Math.Atan2((double)NPC.velocity.Y, (double)NPC.velocity.X) + 1.57f;
            if (shouldFly)
            {
                if (NPC.localAI[0] != 1f)
                {
                    NPC.netUpdate = true;
                }
                NPC.localAI[0] = 1f;
            }
            else
            {
                if (NPC.localAI[0] != 0f)
                {
                    NPC.netUpdate = true;
                }
                NPC.localAI[0] = 0f;
            }
            if (((NPC.velocity.X > 0f && NPC.oldVelocity.X < 0f) || (NPC.velocity.X < 0f && NPC.oldVelocity.X > 0f) || (NPC.velocity.Y > 0f && NPC.oldVelocity.Y < 0f) || (NPC.velocity.Y < 0f && NPC.oldVelocity.Y > 0f)) && !NPC.justHit)
            {
                NPC.netUpdate = true;
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.SandBlock;
        }

        public override void OnKill()
        {
            CalamityGlobalNPC.SetNewBossJustDowned(NPC);

            // If Desert Scourge has not been killed yet, notify players that the Sunken Sea is open and Sandstorms can happen
            if (!DownedBossSystem.downedDesertScourge)
            {
                string key = "Mods.CalamityMod.Status.Progression.OpenSunkenSea";
                Color messageColor = Color.Aquamarine;
                string key2 = "Mods.CalamityMod.Status.Progression.SandstormTrigger";
                Color messageColor2 = Color.PaleGoldenrod;

                CalamityUtils.DisplayLocalizedText(key, messageColor);
                CalamityUtils.DisplayLocalizedText(key2, messageColor2);

                if (!Terraria.GameContent.Events.Sandstorm.Happening)
                    CalamityUtils.StartSandstorm();
            }

            // Mark Desert Scourge as dead
            DownedBossSystem.downedDesertScourge = true;
            CalamityNetcode.SyncWorld();
        }

        public override bool SpecialOnKill()
        {
            int closestSegmentID = DropHelper.FindClosestWormSegment(NPC,
                ModContent.NPCType<DesertScourgeHead>(),
                ModContent.NPCType<DesertScourgeBody>(),
                ModContent.NPCType<DesertScourgeTail>());
            NPC.position = Main.npc[closestSegmentID].position;
            return false;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Boss bag
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<DesertScourgeBag>()));

            // Extraneous potions
			npcLoot.DefineConditionalDropSet(() => true).Add(DropHelper.PerPlayer(ItemID.LesserHealingPotion, 1, 5, 15), hideLootReport: true); // Healing Potions don't show up in the Bestiary

            // Normal drops: Everything that would otherwise be in the bag
            var normalOnly = npcLoot.DefineNormalOnlyDropSet();
            {
                // Weapons and accessories
                int[] items = new int[]
                {
                    ModContent.ItemType<SaharaSlicers>(),
                    ModContent.ItemType<Barinade>(),
                    ModContent.ItemType<SandstreamScepter>(),
                    ModContent.ItemType<BrittleStarStaff>(),
                    ModContent.ItemType<ScourgeoftheDesert>()
                };
                normalOnly.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, items));

                // Vanity
                normalOnly.Add(ModContent.ItemType<DesertScourgeMask>(), 7);
                normalOnly.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);

                // Materials
                normalOnly.Add(ItemID.Coral, 1, 25, 30);
                normalOnly.Add(ItemID.Seashell, 1, 25, 30);
                normalOnly.Add(ItemID.Starfish, 1, 25, 30);
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<PearlShard>(), 1, 25, 30));

                // Equipment
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<OceanCrest>()));
                normalOnly.Add(ModContent.ItemType<AeroStone>(), DropHelper.NormalWeaponDropRateFraction);
                normalOnly.Add(ModContent.ItemType<SandCloak>(), DropHelper.NormalWeaponDropRateFraction);

                // Fishing
                normalOnly.Add(ModContent.ItemType<SandyAnglingKit>());
            }

            // Trophy (always directly from boss, never in bag)
            npcLoot.Add(ModContent.ItemType<DesertScourgeTrophy>(), 10);

            // Relic
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<DesertScourgeRelic>());

            // GFB Sand Shark Tooth Necklace drop
            npcLoot.DefineConditionalDropSet(DropHelper.GFB).Add(ModContent.ItemType<SandSharkToothNecklace>(), hideLootReport: true);

            // Lore
            npcLoot.AddConditionalPerPlayer(() => !DownedBossSystem.downedDesertScourge, ModContent.ItemType<LoreDesertScourge>(), desc: DropHelper.FirstKillText);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ScourgeHead").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ScourgeHead2").Type, NPC.scale);
                }
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
                NPC.Opacity = 1f;
            return true;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance);
            NPC.damage = (int)(NPC.damage * NPC.GetExpertDamageMultiplier());
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(BuffID.Bleeding, 600, true);
        }

        public override Color? GetAlpha(Color drawColor)
        {
            if (Main.zenithWorld)
            {
                Color lightColor = Color.MediumBlue * drawColor.A;
                return lightColor * NPC.Opacity;
            }
            else return null;
        }
    }
}
