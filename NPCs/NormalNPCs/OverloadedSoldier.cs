using System;
using Microsoft.Xna.Framework;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class OverloadedSoldier : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 14;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.damage = NPC.downedMoonlord ? 84 : 42;
            NPC.width = 18;
            NPC.height = 40;
            NPC.defense = 18;
            NPC.DR_NERD(0.15f);
            NPC.lifeMax = NPC.downedMoonlord ? 1350 : 135;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 2, 0);
            NPC.HitSound = SoundID.NPCHit2;
            NPC.DeathSound = SoundID.NPCDeath2;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<OverloadedSoldierBanner>();
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Underground,
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.OverloadedSoldier")
            });
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                NPC.frameCounter += 1;
                if (NPC.frameCounter > 6.0)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y >= frameHeight * 13)
                {
                    NPC.frame.Y = frameHeight;
                }
            }
            else
            {
                NPC.frameCounter += (double)Math.Abs(NPC.velocity.X);
                if (NPC.frameCounter > 6.0)
                {
                    NPC.frameCounter = 0.0;
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                }
                if (NPC.velocity.Y == 0f)
                {
                    if (NPC.direction == 1)
                        NPC.spriteDirection = 1;
                    if (NPC.direction == -1)
                        NPC.spriteDirection = -1;
                }
                else
                {
                    NPC.frameCounter = 0.0;
                    NPC.frame.Y = 0;
                    return;
                }
                if (NPC.velocity.X == 0f)
                {
                    NPC.frameCounter = 0.0;
                    NPC.frame.Y = 0;
                }
                else
                {
                    if (NPC.frame.Y < frameHeight)
                        NPC.frame.Y = frameHeight;
                    if (NPC.frame.Y > frameHeight * 13)
                        NPC.frame.Y = frameHeight;
                }
            }            
        }

        public override void AI()
        {
            Lighting.AddLight((int)((NPC.position.X + (float)(NPC.width / 2)) / 16f), (int)((NPC.position.Y + (float)(NPC.height / 2)) / 16f), 0.35f, 0f, 0.15f);
            bool noXMovement = false;
            if (NPC.velocity.X == 0f)
            {
                noXMovement = true;
            }
            if (NPC.justHit)
            {
                noXMovement = false;
            }
            bool isWalking = false;
            bool unusedFlag = false;
            int backUpTimer = 60;
            if (NPC.velocity.Y == 0f && ((NPC.velocity.X > 0f && NPC.direction < 0) || (NPC.velocity.X < 0f && NPC.direction > 0)))
            {
                isWalking = true;
            }
            if ((NPC.position.X == NPC.oldPosition.X || NPC.ai[3] >= (float)backUpTimer) | isWalking)
            {
                NPC.ai[3] += 1f;
            }
            else if ((double)Math.Abs(NPC.velocity.X) > 0.9 && NPC.ai[3] > 0f)
            {
                NPC.ai[3] -= 1f;
            }
            if (NPC.ai[3] > (float)(backUpTimer * 10))
            {
                NPC.ai[3] = 0f;
            }
            if (NPC.justHit)
            {
                NPC.ai[3] = 0f;
            }
            if (NPC.ai[3] == (float)backUpTimer)
            {
                NPC.netUpdate = true;
            }
            if (NPC.ai[3] < (float)backUpTimer)
            {
                NPC.TargetClosest(true);
            }
            else if (NPC.ai[2] <= 0f)
            {
                if (NPC.velocity.X == 0f)
                {
                    if (NPC.velocity.Y == 0f)
                    {
                        NPC.ai[0] += 1f;
                        if (NPC.ai[0] >= 2f)
                        {
                            NPC.direction *= -1;
                            NPC.spriteDirection = NPC.direction;
                            NPC.ai[0] = 0f;
                        }
                    }
                }
                else
                {
                    NPC.ai[0] = 0f;
                }
                if (NPC.direction == 0)
                {
                    NPC.direction = 1;
                }
            }
            float maxVelocity = 1.5f;
            float acceleration = 0.1f;
            if (CalamityWorld.death)
            {
                maxVelocity *= 1.5f;
                acceleration *= 1.5f;
            }
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) < 400f)
            {
                maxVelocity += (CalamityWorld.death ? 8f : CalamityWorld.revenge ? 6f : 4f) - ((Main.player[NPC.target].Center - NPC.Center).Length() * 0.01f);
             }
            if (NPC.velocity.X < -maxVelocity || NPC.velocity.X > maxVelocity)
            {
                if (NPC.velocity.Y == 0f)
                    NPC.velocity *= 0.7f;
            }
            else if (NPC.velocity.X < maxVelocity && NPC.direction == 1)
            {
                NPC.velocity.X = NPC.velocity.X + acceleration;
                if (NPC.velocity.X > maxVelocity)
                    NPC.velocity.X = maxVelocity;
            }
            else if (NPC.velocity.X > -maxVelocity && NPC.direction == -1)
            {
                NPC.velocity.X = NPC.velocity.X - acceleration;
                if (NPC.velocity.X < -maxVelocity)
                    NPC.velocity.X = -maxVelocity;
            }
            bool isOnSolidTile = false;
            if (NPC.velocity.Y == 0f)
            {
                int yTile = (int)(NPC.position.Y + (float)NPC.height + 7f) / 16;
                int initialXTile = (int)NPC.position.X / 16;
                int maxXTile = (int)(NPC.position.X + (float)NPC.width) / 16;
                for (int xTile = initialXTile; xTile <= maxXTile; xTile++)
                {
                    if (Main.tile[xTile, yTile] == null)
                    {
                        return;
                    }
                    if (Main.tile[xTile, yTile].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[xTile, yTile].TileType])
                    {
                        isOnSolidTile = true;
                        break;
                    }
                }
            }
            if (NPC.velocity.Y >= 0f)
            {
                int fallFaceDirection = 0;
                if (NPC.velocity.X < 0f)
                {
                    fallFaceDirection = -1;
                }
                if (NPC.velocity.X > 0f)
                {
                    fallFaceDirection = 1;
                }
                Vector2 ghostPosition = NPC.position;
                ghostPosition.X += NPC.velocity.X;
                int xTileBelow = (int)((ghostPosition.X + (float)(NPC.width / 2) + (float)((NPC.width / 2 + 1) * fallFaceDirection)) / 16f);
                int yTileBelow = (int)((ghostPosition.Y + (float)NPC.height - 1f) / 16f);
                if ((float)(xTileBelow * 16) < ghostPosition.X + (float)NPC.width && (float)(xTileBelow * 16 + 16) > ghostPosition.X && ((Main.tile[xTileBelow, yTileBelow].HasUnactuatedTile && !Main.tile[xTileBelow, yTileBelow].TopSlope && !Main.tile[xTileBelow, yTileBelow - 1].TopSlope && Main.tileSolid[(int)Main.tile[xTileBelow, yTileBelow].TileType] && !Main.tileSolidTop[(int)Main.tile[xTileBelow, yTileBelow].TileType]) || (Main.tile[xTileBelow, yTileBelow - 1].IsHalfBlock && Main.tile[xTileBelow, yTileBelow - 1].HasUnactuatedTile)) && (!Main.tile[xTileBelow, yTileBelow - 1].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[xTileBelow, yTileBelow - 1].TileType] || Main.tileSolidTop[(int)Main.tile[xTileBelow, yTileBelow - 1].TileType] || (Main.tile[xTileBelow, yTileBelow - 1].IsHalfBlock && (!Main.tile[xTileBelow, yTileBelow - 4].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[xTileBelow, yTileBelow - 4].TileType] || Main.tileSolidTop[(int)Main.tile[xTileBelow, yTileBelow - 4].TileType]))) && (!Main.tile[xTileBelow, yTileBelow - 2].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[xTileBelow, yTileBelow - 2].TileType] || Main.tileSolidTop[(int)Main.tile[xTileBelow, yTileBelow - 2].TileType]) && (!Main.tile[xTileBelow, yTileBelow - 3].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[xTileBelow, yTileBelow - 3].TileType] || Main.tileSolidTop[(int)Main.tile[xTileBelow, yTileBelow - 3].TileType]) && (!Main.tile[xTileBelow - fallFaceDirection, yTileBelow - 3].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[xTileBelow - fallFaceDirection, yTileBelow - 3].TileType]))
                {
                    float yPixelDistance = (float)(yTileBelow * 16);
                    if (Main.tile[xTileBelow, yTileBelow].IsHalfBlock)
                    {
                        yPixelDistance += 8f;
                    }
                    if (Main.tile[xTileBelow, yTileBelow - 1].IsHalfBlock)
                    {
                        yPixelDistance -= 8f;
                    }
                    if (yPixelDistance < ghostPosition.Y + (float)NPC.height)
                    {
                        float percentageTileRisen = ghostPosition.Y + (float)NPC.height - yPixelDistance;
                        float fullTileAmt = 16.1f;
                        if (percentageTileRisen <= fullTileAmt)
                        {
                            NPC.gfxOffY += NPC.position.Y + (float)NPC.height - yPixelDistance;
                            NPC.position.Y = yPixelDistance - (float)NPC.height;
                            if (percentageTileRisen < 9f)
                            {
                                NPC.stepSpeed = 1f;
                            }
                            else
                            {
                                NPC.stepSpeed = 2f;
                            }
                        }
                    }
                }
            }
            if (isOnSolidTile)
            {
                int doorCheckX = (int)((NPC.position.X + (float)(NPC.width / 2) + (float)(15 * NPC.direction)) / 16f);
                int doorCheckY = (int)((NPC.position.Y + (float)NPC.height - 15f) / 16f);
                if ((Main.tile[doorCheckX, doorCheckY - 1].HasUnactuatedTile && (Main.tile[doorCheckX, doorCheckY - 1].TileType == 10 || Main.tile[doorCheckX, doorCheckY - 1].TileType == 388)) & unusedFlag)
                {
                    NPC.ai[2] += 1f;
                    NPC.ai[3] = 0f;
                    if (NPC.ai[2] >= 60f)
                    {
                        NPC.velocity.X = 0.5f * (float)-(float)NPC.direction;
                        int doorOpenInc = 5;
                        if (Main.tile[doorCheckX, doorCheckY - 1].TileType == 388)
                        {
                            doorOpenInc = 2;
                        }
                        NPC.ai[1] += (float)doorOpenInc;
                        NPC.ai[2] = 0f;
                        bool letMeIn = false;
                        if (NPC.ai[1] >= 10f)
                        {
                            letMeIn = true;
                            NPC.ai[1] = 10f;
                        }
                        WorldGen.KillTile(doorCheckX, doorCheckY - 1, true, false, false);
                        if ((Main.netMode != NetmodeID.MultiplayerClient || !letMeIn) && letMeIn && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (Main.tile[doorCheckX, doorCheckY - 1].TileType == 10)
                            {
                                bool canOpenDoor = WorldGen.OpenDoor(doorCheckX, doorCheckY - 1, NPC.direction);
                                if (!canOpenDoor)
                                {
                                    NPC.ai[3] = (float)backUpTimer;
                                    NPC.netUpdate = true;
                                }
                                if (Main.netMode == NetmodeID.Server & canOpenDoor)
                                {
                                    NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, null, 0, (float)doorCheckX, (float)(doorCheckY - 1), (float)NPC.direction, 0, 0, 0);
                                }
                            }
                            if (Main.tile[doorCheckX, doorCheckY - 1].TileType == 388)
                            {
                                bool canOpenTallGate = WorldGen.ShiftTallGate(doorCheckX, doorCheckY - 1, false);
                                if (!canOpenTallGate)
                                {
                                    NPC.ai[3] = (float)backUpTimer;
                                    NPC.netUpdate = true;
                                }
                                if (Main.netMode == NetmodeID.Server & canOpenTallGate)
                                {
                                    NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, null, 4, (float)doorCheckX, (float)(doorCheckY - 1), 0f, 0, 0, 0);
                                }
                            }
                        }
                    }
                }
                else
                {
                    int faceDirection = NPC.spriteDirection;
                    if ((NPC.velocity.X < 0f && faceDirection == -1) || (NPC.velocity.X > 0f && faceDirection == 1))
                    {
                        if (NPC.height >= 32 && Main.tile[doorCheckX, doorCheckY - 2].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[doorCheckX, doorCheckY - 2].TileType])
                        {
                            if (Main.tile[doorCheckX, doorCheckY - 3].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[doorCheckX, doorCheckY - 3].TileType])
                            {
                                NPC.velocity.Y = -8f;
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                NPC.velocity.Y = -7f;
                                NPC.netUpdate = true;
                            }
                        }
                        else if (Main.tile[doorCheckX, doorCheckY - 1].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[doorCheckX, doorCheckY - 1].TileType])
                        {
                            NPC.velocity.Y = -6f;
                            NPC.netUpdate = true;
                        }
                        else if (NPC.position.Y + (float)NPC.height - (float)(doorCheckY * 16) > 20f && Main.tile[doorCheckX, doorCheckY].HasUnactuatedTile && !Main.tile[doorCheckX, doorCheckY].TopSlope && Main.tileSolid[(int)Main.tile[doorCheckX, doorCheckY].TileType])
                        {
                            NPC.velocity.Y = -5f;
                            NPC.netUpdate = true;
                        }
                        else if (NPC.directionY < 0 && (!Main.tile[doorCheckX, doorCheckY + 1].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[doorCheckX, doorCheckY + 1].TileType]) && (!Main.tile[doorCheckX + NPC.direction, doorCheckY + 1].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[doorCheckX + NPC.direction, doorCheckY + 1].TileType]))
                        {
                            NPC.velocity.Y = -8f;
                            NPC.velocity.X = NPC.velocity.X * 1.5f;
                            NPC.netUpdate = true;
                        }
                        else if (unusedFlag)
                        {
                            NPC.ai[1] = 0f;
                            NPC.ai[2] = 0f;
                        }
                        if ((NPC.velocity.Y == 0f & noXMovement) && NPC.ai[3] == 1f)
                        {
                            NPC.velocity.Y = -5f;
                        }
                    }
                }
            }
            else if (unusedFlag)
            {
                NPC.ai[1] = 0f;
                NPC.ai[2] = 0f;
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe || !Main.hardMode || spawnInfo.Player.Calamity().ZoneAbyss ||
                spawnInfo.Player.Calamity().ZoneSunkenSea)
            {
                return 0f;
            }
            return SpawnCondition.Underground.Chance * 0.02f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 120);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Polterplasm, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Polterplasm, hit.HitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ModContent.ItemType<AncientBoneDust>());
            LeadingConditionRule postML = npcLoot.DefineConditionalDropSet(DropHelper.PostML());
            postML.Add(ModContent.ItemType<Polterplasm>());
        }
    }
}
