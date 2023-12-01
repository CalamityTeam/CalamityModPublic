using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Items.Accessories;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.Audio;

namespace CalamityMod.NPCs.PlagueEnemies
{
    public class PlaguebringerMiniboss : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 12;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Scale = 0.7f,
                PortraitScale = 0.8f,
            };
            value.Position.X += 20f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.damage = 70;
            NPC.npcSlots = 8f;
            NPC.width = 66;
            NPC.height = 66;
            NPC.defense = 24;
            NPC.DR_NERD(0.2f);
            NPC.lifeMax = 3000;
            NPC.value = Item.buyPrice(0, 1, 50, 0);
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            AIType = -1;
            AnimationType = NPCID.QueenBee;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<PlaguebringerBanner>();
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
            if (Main.zenithWorld)
            {
                NPC.scale = 2f;
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Jungle,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundJungle,
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.PlaguebringerMiniboss")
            });
        }

        public override void AI()
        {
            Lighting.AddLight((int)((NPC.position.X + (float)(NPC.width / 2)) / 16f), (int)((NPC.position.Y + (float)(NPC.height / 2)) / 16f), 0.1f, 0.3f, 0f);
            bool outsideJungle = false;
            if (!Main.player[NPC.target].ZoneJungle)
            {
                outsideJungle = true;
                if (NPC.timeLeft > 150)
                {
                    NPC.timeLeft = 150;
                }
            }
            else
            {
                if (NPC.timeLeft < 750)
                {
                    NPC.timeLeft = 750;
                }
            }
            int playerAmt = 0;
            for (int i = 0; i < 255; i++)
            {
                if (Main.player[i].active && !Main.player[i].dead && (NPC.Center - Main.player[i].Center).Length() < 1000f)
                {
                    playerAmt++;
                }
            }
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }
            bool dead4 = Main.player[NPC.target].dead;
            if (dead4 && Main.expertMode)
            {
                if ((double)NPC.position.Y < Main.worldSurface * 16.0 + 2000.0)
                {
                    NPC.velocity.Y = NPC.velocity.Y + 0.04f;
                }
                if (NPC.position.X < (float)(Main.maxTilesX * 8))
                {
                    NPC.velocity.X = NPC.velocity.X - 0.04f;
                }
                else
                {
                    NPC.velocity.X = NPC.velocity.X + 0.04f;
                }
                if (NPC.timeLeft > 10)
                {
                    NPC.timeLeft = 10;
                    return;
                }
            }
            else if (NPC.ai[0] == -1f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float currentAttack = NPC.ai[1];
                    int nextAttack;
                    do
                    {
                        nextAttack = Main.rand.Next(3);
                        if (nextAttack == 1)
                        {
                            nextAttack = 2;
                        }
                        else if (nextAttack == 2)
                        {
                            nextAttack = 3;
                        }
                    }
                    while ((float)nextAttack == currentAttack);
                    NPC.ai[0] = (float)nextAttack;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    return;
                }
            }
            else if (NPC.ai[0] == 0f)
            {
                int chargeDelay = 2; //2
                if (outsideJungle)
                {
                    chargeDelay += 1;
                }
                if (NPC.ai[1] > (float)(2 * chargeDelay) && NPC.ai[1] % 2f == 0f)
                {
                    NPC.ai[0] = -1f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.netUpdate = true;
                    return;
                }
                if (NPC.ai[1] % 2f == 0f)
                {
                    NPC.TargetClosest(true);
                    if (Math.Abs(NPC.position.Y + (float)(NPC.height / 2) - (Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2))) < 20f)
                    {
                        NPC.localAI[0] = 1f;
                        NPC.ai[1] += 1f;
                        NPC.ai[2] = 0f;
                        float chargeSpeed = 15f;
                        if (outsideJungle)
                        {
                            chargeSpeed += 2f;
                        }
                        Vector2 chargeBeePos = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                        float chargeTargetX = Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2) - chargeBeePos.X;
                        float chargeTargetY = Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2) - chargeBeePos.Y;
                        float chargeTargetDist = (float)Math.Sqrt((double)(chargeTargetX * chargeTargetX + chargeTargetY * chargeTargetY));
                        chargeTargetDist = chargeSpeed / chargeTargetDist;
                        NPC.velocity.X = chargeTargetX * chargeTargetDist;
                        NPC.velocity.Y = chargeTargetY * chargeTargetDist;
                        NPC.spriteDirection = NPC.direction;
                        SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                        return;
                    }
                    NPC.localAI[0] = 0f;
                    float lineUpSpeed = 12.25f;
                    float lineUpAcceleration = 0.155f;
                    if (outsideJungle)
                    {
                        lineUpSpeed += 1f;
                        lineUpAcceleration += 0.075f;
                    }
                    if (NPC.position.Y + (float)(NPC.height / 2) < Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2))
                    {
                        NPC.velocity.Y = NPC.velocity.Y + lineUpAcceleration;
                    }
                    else
                    {
                        NPC.velocity.Y = NPC.velocity.Y - lineUpAcceleration;
                    }
                    if (NPC.velocity.Y < -12f)
                    {
                        NPC.velocity.Y = -lineUpSpeed;
                    }
                    if (NPC.velocity.Y > 12f)
                    {
                        NPC.velocity.Y = lineUpSpeed;
                    }
                    if (Math.Abs(NPC.position.X + (float)(NPC.width / 2) - (Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2))) > 600f)
                    {
                        NPC.velocity.X = NPC.velocity.X + 0.15f * (float)NPC.direction;
                    }
                    else if (Math.Abs(NPC.position.X + (float)(NPC.width / 2) - (Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2))) < 300f)
                    {
                        NPC.velocity.X = NPC.velocity.X - 0.15f * (float)NPC.direction;
                    }
                    else
                    {
                        NPC.velocity.X = NPC.velocity.X * 0.8f;
                    }
                    if (NPC.velocity.X < -16f)
                    {
                        NPC.velocity.X = -16f;
                    }
                    if (NPC.velocity.X > 16f)
                    {
                        NPC.velocity.X = 16f;
                    }
                    NPC.spriteDirection = NPC.direction;
                }
                else
                {
                    if (NPC.velocity.X < 0f)
                    {
                        NPC.direction = -1;
                    }
                    else
                    {
                        NPC.direction = 1;
                    }
                    NPC.spriteDirection = NPC.direction;
                    int chargeDirection = 1;
                    if (NPC.position.X + (float)(NPC.width / 2) < Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2))
                    {
                        chargeDirection = -1;
                    }
                    if (NPC.direction == chargeDirection && Math.Abs(NPC.position.X + (float)(NPC.width / 2) - (Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2))) > 500f)
                    {
                        NPC.ai[2] = 1f;
                    }
                    if (NPC.ai[2] != 1f)
                    {
                        NPC.localAI[0] = 1f;
                        return;
                    }
                    NPC.TargetClosest(true);
                    NPC.spriteDirection = NPC.direction;
                    NPC.localAI[0] = 0f;
                    NPC.velocity *= 0.9f;
                    float chargeDeceleration = 0.105f;
                    if (outsideJungle)
                    {
                        NPC.velocity *= 0.9f;
                        chargeDeceleration += 0.075f;
                    }
                    if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < chargeDeceleration)
                    {
                        NPC.ai[2] = 0f;
                        NPC.ai[1] += 1f;
                    }
                }
            }
            else if (NPC.ai[0] == 2f)
            {
                NPC.TargetClosest(true);
                NPC.spriteDirection = NPC.direction;
                float hoverForBeesSpeed = 12f;
                float hoverForBeesAccel = 0.1f;
                if (outsideJungle)
                {
                    hoverForBeesAccel = 0.12f;
                }
                Vector2 hoverForBeesPosition = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                float hoverForBeesTargetX = Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2) - hoverForBeesPosition.X;
                float hoverForBeesTargetY = Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2) - 200f - hoverForBeesPosition.Y;
                float hoverForBeesTargetDist = (float)Math.Sqrt((double)(hoverForBeesTargetX * hoverForBeesTargetX + hoverForBeesTargetY * hoverForBeesTargetY));
                if (hoverForBeesTargetDist < 400f)
                {
                    NPC.ai[0] = 1f;
                    NPC.ai[1] = 0f;
                    NPC.netUpdate = true;
                    return;
                }
                hoverForBeesTargetDist = hoverForBeesSpeed / hoverForBeesTargetDist;
                if (NPC.velocity.X < hoverForBeesTargetX)
                {
                    NPC.velocity.X = NPC.velocity.X + hoverForBeesAccel;
                    if (NPC.velocity.X < 0f && hoverForBeesTargetX > 0f)
                    {
                        NPC.velocity.X = NPC.velocity.X + hoverForBeesAccel;
                    }
                }
                else if (NPC.velocity.X > hoverForBeesTargetX)
                {
                    NPC.velocity.X = NPC.velocity.X - hoverForBeesAccel;
                    if (NPC.velocity.X > 0f && hoverForBeesTargetX < 0f)
                    {
                        NPC.velocity.X = NPC.velocity.X - hoverForBeesAccel;
                    }
                }
                if (NPC.velocity.Y < hoverForBeesTargetY)
                {
                    NPC.velocity.Y = NPC.velocity.Y + hoverForBeesAccel;
                    if (NPC.velocity.Y < 0f && hoverForBeesTargetY > 0f)
                    {
                        NPC.velocity.Y = NPC.velocity.Y + hoverForBeesAccel;
                    }
                }
                else if (NPC.velocity.Y > hoverForBeesTargetY)
                {
                    NPC.velocity.Y = NPC.velocity.Y - hoverForBeesAccel;
                    if (NPC.velocity.Y > 0f && hoverForBeesTargetY < 0f)
                    {
                        NPC.velocity.Y = NPC.velocity.Y - hoverForBeesAccel;
                    }
                }
            }
            else if (NPC.ai[0] == 1f)
            {
                NPC.localAI[0] = 0f;
                NPC.TargetClosest(true);
                Vector2 beeSpawnPosition = new Vector2(NPC.position.X + (float)(NPC.width / 2) + (float)(40 * NPC.direction), NPC.position.Y + (float)NPC.height * 0.8f);
                Vector2 plaguebringerBeeSpawnPos = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                float beeSpawnTargetX = Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2) - plaguebringerBeeSpawnPos.X;
                float beeSpawnTargetY = Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2) - plaguebringerBeeSpawnPos.Y;
                float beeSpawnTargetDist = (float)Math.Sqrt((double)(beeSpawnTargetX * beeSpawnTargetX + beeSpawnTargetY * beeSpawnTargetY));
                NPC.ai[1] += 1f;
                NPC.ai[1] += (float)(playerAmt / 2);
                bool canSpawnBee = false;
                if (NPC.ai[1] > 10f)
                {
                    NPC.ai[1] = 0f;
                    NPC.ai[2] += 1f;
                    canSpawnBee = true;
                }
                if (Collision.CanHit(beeSpawnPosition, 1, 1, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height) && canSpawnBee)
                {
                    SoundEngine.PlaySound(SoundID.NPCHit8, NPC.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int beeType;
                        if (Main.rand.NextBool(4))
                        {
                            beeType = ModContent.NPCType<PlagueChargerLarge>();
                        }
                        else
                        {
                            beeType = ModContent.NPCType<PlagueCharger>();
                        }
                        if (NPC.CountNPCS(ModContent.NPCType<PlagueCharger>()) < 3)
                        {
                            int notTheBees = NPC.NewNPC(NPC.GetSource_FromAI(), (int)beeSpawnPosition.X, (int)beeSpawnPosition.Y, beeType, 0, 0f, 0f, 0f, 0f, 255);
                            Main.npc[notTheBees].velocity.X = (float)Main.rand.Next(-200, 201) * 0.005f;
                            Main.npc[notTheBees].velocity.Y = (float)Main.rand.Next(-200, 201) * 0.005f;
                            Main.npc[notTheBees].localAI[0] = 60f;
                            Main.npc[notTheBees].netUpdate = true;
                        }
                    }
                }
                if (beeSpawnTargetDist > 400f || !Collision.CanHit(new Vector2(beeSpawnPosition.X, beeSpawnPosition.Y - 30f), 1, 1, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                {
                    float beeSpawnSpeed = 14.5f;
                    float beeSpawnAcceleration = 0.105f;
                    plaguebringerBeeSpawnPos = beeSpawnPosition;
                    beeSpawnTargetX = Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2) - plaguebringerBeeSpawnPos.X;
                    beeSpawnTargetY = Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2) - plaguebringerBeeSpawnPos.Y;
                    beeSpawnTargetDist = (float)Math.Sqrt((double)(beeSpawnTargetX * beeSpawnTargetX + beeSpawnTargetY * beeSpawnTargetY));
                    beeSpawnTargetDist = beeSpawnSpeed / beeSpawnTargetDist;
                    if (NPC.velocity.X < beeSpawnTargetX)
                    {
                        NPC.velocity.X = NPC.velocity.X + beeSpawnAcceleration;
                        if (NPC.velocity.X < 0f && beeSpawnTargetX > 0f)
                        {
                            NPC.velocity.X = NPC.velocity.X + beeSpawnAcceleration;
                        }
                    }
                    else if (NPC.velocity.X > beeSpawnTargetX)
                    {
                        NPC.velocity.X = NPC.velocity.X - beeSpawnAcceleration;
                        if (NPC.velocity.X > 0f && beeSpawnTargetX < 0f)
                        {
                            NPC.velocity.X = NPC.velocity.X - beeSpawnAcceleration;
                        }
                    }
                    if (NPC.velocity.Y < beeSpawnTargetY)
                    {
                        NPC.velocity.Y = NPC.velocity.Y + beeSpawnAcceleration;
                        if (NPC.velocity.Y < 0f && beeSpawnTargetY > 0f)
                        {
                            NPC.velocity.Y = NPC.velocity.Y + beeSpawnAcceleration;
                        }
                    }
                    else if (NPC.velocity.Y > beeSpawnTargetY)
                    {
                        NPC.velocity.Y = NPC.velocity.Y - beeSpawnAcceleration;
                        if (NPC.velocity.Y > 0f && beeSpawnTargetY < 0f)
                        {
                            NPC.velocity.Y = NPC.velocity.Y - beeSpawnAcceleration;
                        }
                    }
                }
                else
                {
                    NPC.velocity *= 0.9f; //changed from 0.9
                }
                NPC.spriteDirection = NPC.direction;
                if (NPC.ai[2] > 2f)
                {
                    NPC.ai[0] = -1f;
                    NPC.ai[1] = 1f;
                    NPC.netUpdate = true;
                }
            }
            else if (NPC.ai[0] == 3f)
            {
                float stingerAttackSpeed = 7f;
                float stingerAttackAccel = 0.075f;
                if (outsideJungle)
                {
                    stingerAttackAccel = 0.09f;
                    stingerAttackSpeed = 8f;
                }
                Vector2 stingerSpawnPos = new Vector2(NPC.position.X + (float)(NPC.width / 2) + (float)(40 * NPC.direction), NPC.position.Y + (float)NPC.height * 0.8f);
                Vector2 stingerAttackPos = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                float stingerAttackTargetX = Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2) - stingerAttackPos.X;
                float stingerAttackTargetY = Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2) - 300f - stingerAttackPos.Y;
                float stingerAttackTargetDist = (float)Math.Sqrt((double)(stingerAttackTargetX * stingerAttackTargetX + stingerAttackTargetY * stingerAttackTargetY));
                NPC.ai[1] += 1f;
                bool canFireStinger = false;
                if (NPC.ai[1] % 35f == 34f)
                {
                    canFireStinger = true;
                }
                if (canFireStinger && NPC.position.Y + (float)NPC.height < Main.player[NPC.target].position.Y && Collision.CanHit(stingerSpawnPos, 1, 1, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                {
                    SoundEngine.PlaySound(SoundID.Item42, NPC.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float stingerSpeed = 6f;
                        if (outsideJungle)
                        {
                            stingerSpeed += 2f;
                        }
                        float stingerTargetX = Main.player[NPC.target].position.X + (float)Main.player[NPC.target].width * 0.5f - stingerSpawnPos.X;
                        float stingerTargetY = Main.player[NPC.target].position.Y + (float)Main.player[NPC.target].height * 0.5f - stingerSpawnPos.Y;
                        float stingerTargetDist = (float)Math.Sqrt((double)(stingerTargetX * stingerTargetX + stingerTargetY * stingerTargetY));
                        stingerTargetDist = stingerSpeed / stingerTargetDist;
                        stingerTargetX *= stingerTargetDist;
                        stingerTargetY *= stingerTargetDist;
                        bool fireRocket = Main.rand.NextBool(15);
                        int type = fireRocket ? ModContent.ProjectileType<HiveBombGoliath>() : ModContent.ProjectileType<PlagueStingerGoliathV2>();
                        int damage = fireRocket ? 72 : 52;
                        if (Main.expertMode)
                            damage = fireRocket ? 50 : 35;

                        Projectile.NewProjectile(NPC.GetSource_FromAI(), stingerSpawnPos.X, stingerSpawnPos.Y, stingerTargetX, stingerTargetY, type, damage, 0f, Main.myPlayer, 0f, 0f);
                    }
                }
                if (!Collision.CanHit(new Vector2(stingerSpawnPos.X, stingerSpawnPos.Y - 30f), 1, 1, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                {
                    stingerAttackAccel = 0.105f;
                    stingerAttackPos = stingerSpawnPos;
                    stingerAttackTargetX = Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2) - stingerAttackPos.X;
                    stingerAttackTargetY = Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2) - stingerAttackPos.Y;
                    if (NPC.velocity.X < stingerAttackTargetX)
                    {
                        NPC.velocity.X = NPC.velocity.X + stingerAttackAccel;
                        if (NPC.velocity.X < 0f && stingerAttackTargetX > 0f)
                        {
                            NPC.velocity.X = NPC.velocity.X + stingerAttackAccel;
                        }
                    }
                    else if (NPC.velocity.X > stingerAttackTargetX)
                    {
                        NPC.velocity.X = NPC.velocity.X - stingerAttackAccel;
                        if (NPC.velocity.X > 0f && stingerAttackTargetX < 0f)
                        {
                            NPC.velocity.X = NPC.velocity.X - stingerAttackAccel;
                        }
                    }
                    if (NPC.velocity.Y < stingerAttackTargetY)
                    {
                        NPC.velocity.Y = NPC.velocity.Y + stingerAttackAccel;
                        if (NPC.velocity.Y < 0f && stingerAttackTargetY > 0f)
                        {
                            NPC.velocity.Y = NPC.velocity.Y + stingerAttackAccel;
                        }
                    }
                    else if (NPC.velocity.Y > stingerAttackTargetY)
                    {
                        NPC.velocity.Y = NPC.velocity.Y - stingerAttackAccel;
                        if (NPC.velocity.Y > 0f && stingerAttackTargetY < 0f)
                        {
                            NPC.velocity.Y = NPC.velocity.Y - stingerAttackAccel;
                        }
                    }
                }
                else if (stingerAttackTargetDist > 100f)
                {
                    NPC.TargetClosest(true);
                    NPC.spriteDirection = NPC.direction;
                    stingerAttackTargetDist = stingerAttackSpeed / stingerAttackTargetDist;
                    if (NPC.velocity.X < stingerAttackTargetX)
                    {
                        NPC.velocity.X = NPC.velocity.X + stingerAttackAccel;
                        if (NPC.velocity.X < 0f && stingerAttackTargetX > 0f)
                        {
                            NPC.velocity.X = NPC.velocity.X + stingerAttackAccel * 2f;
                        }
                    }
                    else if (NPC.velocity.X > stingerAttackTargetX)
                    {
                        NPC.velocity.X = NPC.velocity.X - stingerAttackAccel;
                        if (NPC.velocity.X > 0f && stingerAttackTargetX < 0f)
                        {
                            NPC.velocity.X = NPC.velocity.X - stingerAttackAccel * 2f;
                        }
                    }
                    if (NPC.velocity.Y < stingerAttackTargetY)
                    {
                        NPC.velocity.Y = NPC.velocity.Y + stingerAttackAccel;
                        if (NPC.velocity.Y < 0f && stingerAttackTargetY > 0f)
                        {
                            NPC.velocity.Y = NPC.velocity.Y + stingerAttackAccel * 2f;
                        }
                    }
                    else if (NPC.velocity.Y > stingerAttackTargetY)
                    {
                        NPC.velocity.Y = NPC.velocity.Y - stingerAttackAccel;
                        if (NPC.velocity.Y > 0f && stingerAttackTargetY < 0f)
                        {
                            NPC.velocity.Y = NPC.velocity.Y - stingerAttackAccel * 2f;
                        }
                    }
                }
                if (NPC.ai[1] > 600f)
                {
                    NPC.ai[0] = -1f;
                    NPC.ai[1] = 3f;
                    NPC.netUpdate = true;
                }
            }

            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, NPC.whoAmI, 0f, 0f, 0f, 0, 0, 0);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[NPC.type].Value;
            Vector2 halfSizeTexture = new Vector2((float)(TextureAssets.Npc[NPC.type].Value.Width / 2), (float)(TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2));
            int afterimageAmt = 7;
            if (NPC.ai[0] != 0f)
                afterimageAmt = 5;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int i = 1; i < afterimageAmt; i += 2)
                {
                    Color afterimageColor = drawColor;
                    afterimageColor = Color.Lerp(afterimageColor, Color.White, 0.5f);
                    afterimageColor = NPC.GetAlpha(afterimageColor);
                    afterimageColor *= (float)(afterimageAmt - i) / 15f;
                    Vector2 afterimagePos = NPC.oldPos[i] + new Vector2((float)NPC.width, (float)NPC.height) / 2f - screenPos;
                    afterimagePos -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[NPC.type])) * NPC.scale / 2f;
                    afterimagePos += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, afterimagePos, NPC.frame, afterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
                }
            }

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[NPC.type])) * NPC.scale / 2f;
            drawLocation += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe || !NPC.downedGolemBoss || !spawnInfo.Player.ZoneJungle)
                return 0f;

            // Keep this as a separate if check, because it's a loop and we don't want to be checking it constantly.
            if (NPC.AnyNPCs(NPC.type))
                return 0f;

            return SpawnCondition.HardmodeJungle.Chance * 0.02f;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Plague, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                NPC.position.X = NPC.position.X + (float)(NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (float)(NPC.height / 2);
                NPC.width = 100;
                NPC.height = 100;
                NPC.position.X = NPC.position.X - (float)(NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (float)(NPC.height / 2);
                for (int i = 0; i < 40; i++)
                {
                    int plagueDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.Plague, 0f, 0f, 100, default, 2f);
                    Main.dust[plagueDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[plagueDust].scale = 0.5f;
                        Main.dust[plagueDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 70; j++)
                {
                    int plagueDust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.Plague, 0f, 0f, 100, default, 3f);
                    Main.dust[plagueDust2].noGravity = true;
                    Main.dust[plagueDust2].velocity *= 5f;
                    plagueDust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.Plague, 0f, 0f, 100, default, 2f);
                    Main.dust[plagueDust2].velocity *= 2f;
                }
            }
        }

        public override void OnKill()
        {
            int heartAmt = Main.rand.Next(3) + 3;
            for (int i = 0; i < heartAmt; i++)
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.Heart);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ModContent.ItemType<PlagueCellCanister>(), 1, 8, 12);
            npcLoot.Add(ModContent.ItemType<PlaguedFuelPack>(), 4);
            npcLoot.Add(ModContent.ItemType<PlagueCaller>(), 50);
            npcLoot.Add(ItemID.Stinger);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<Plague>(), 120, true);
        }
    }
}
