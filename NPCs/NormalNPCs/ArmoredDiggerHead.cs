using System;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs
{
    [LongDistanceNetSync]
    public class ArmoredDiggerHead : ModNPC
    {
        bool TailSpawned = false;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "CalamityMod/ExtraTextures/Bestiary/ArmoredDigger_Bestiary",
                PortraitPositionXOverride = 40,
                PortraitPositionYOverride = 40
            };
            value.Position.Y += 40;
            value.Position.X += 48;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.damage = 90;
            NPC.npcSlots = 10f;
            NPC.width = 54;
            NPC.height = 54;
            NPC.defense = 10;
            NPC.DR_NERD(0.1f);
            NPC.lifeMax = 20000;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.value = Item.buyPrice(0, 5, 0, 0);
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.netAlways = true;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<ArmoredDiggerBanner>();
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;

            // Scale stats in Expert and Master
            CalamityGlobalNPC.AdjustExpertModeStatScaling(NPC);
            CalamityGlobalNPC.AdjustMasterModeStatScaling(NPC);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.ArmoredDigger")
            });
        }

        public override void AI()
        {
            Point point = NPC.Center.ToTileCoordinates();
            Tile tileSafely = Framing.GetTileSafely(point);
            bool createDust = tileSafely.HasUnactuatedTile && NPC.Distance(Main.player[NPC.target].Center) < 800f;
            if (createDust)
            {
                if (Main.rand.NextBool())
                {
                    Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.TreasureSparkle, 0f, 0f, 150, default(Color), 0.3f);
                    dust.fadeIn = 0.75f;
                    dust.velocity *= 0.1f;
                    dust.noLight = true;
                }
            }

            bool death = CalamityWorld.death;
            if (NPC.ai[3] > 0f)
            {
                NPC.realLife = (int)NPC.ai[3];
            }
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest(true);
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!TailSpawned)
                {
                    NPC.ai[3] = (float)NPC.whoAmI;
                    NPC.realLife = NPC.whoAmI;
                    int thisSegment = NPC.whoAmI;
                    int segmentAmt = 30;
                    for (int j = 0; j <= segmentAmt; j++)
                    {
                        int segmentType = ModContent.NPCType<ArmoredDiggerBody>();
                        if (j == segmentAmt)
                        {
                            segmentType = ModContent.NPCType<ArmoredDiggerTail>();
                        }
                        int newSegment = NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.position.X + (float)(NPC.width / 2)), (int)(NPC.position.Y + (float)NPC.height), segmentType, NPC.whoAmI, 0f, 0f, 0f, 0f, 255);
                        Main.npc[newSegment].ai[3] = (float)NPC.whoAmI;
                        Main.npc[newSegment].realLife = NPC.whoAmI;
                        Main.npc[newSegment].ai[1] = (float)thisSegment;
                        Main.npc[thisSegment].ai[0] = (float)newSegment;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, newSegment, 0f, 0f, 0f, 0, 0, 0);
                        thisSegment = newSegment;
                    }
                    TailSpawned = true;
                }
            }
            int tileXPosition = (int)(NPC.position.X / 16f) - 1;
            int tileWidthXPos = (int)((NPC.position.X + (float)NPC.width) / 16f) + 2;
            int tileYPosition = (int)(NPC.position.Y / 16f) - 1;
            int tileWidthYPos = (int)((NPC.position.Y + (float)NPC.height) / 16f) + 2;
            if (tileXPosition < 0)
            {
                tileXPosition = 0;
            }
            if (tileWidthXPos > Main.maxTilesX)
            {
                tileWidthXPos = Main.maxTilesX;
            }
            if (tileYPosition < 0)
            {
                tileYPosition = 0;
            }
            if (tileWidthYPos > Main.maxTilesY)
            {
                tileWidthYPos = Main.maxTilesY;
            }
            bool flying = false;
            if (!flying)
            {
                for (int k = tileXPosition; k < tileWidthXPos; k++)
                {
                    for (int l = tileYPosition; l < tileWidthYPos; l++)
                    {
                        if (Main.tile[k, l] != null && ((Main.tile[k, l].HasUnactuatedTile && (Main.tileSolid[(int)Main.tile[k, l].TileType] || (Main.tileSolidTop[(int)Main.tile[k, l].TileType] && Main.tile[k, l].TileFrameY == 0))) || Main.tile[k, l].LiquidAmount > 64))
                        {
                            Vector2 segmentPos;
                            segmentPos.X = (float)(k * 16);
                            segmentPos.Y = (float)(l * 16);
                            if (NPC.position.X + (float)NPC.width > segmentPos.X && NPC.position.X < segmentPos.X + 16f && NPC.position.Y + (float)NPC.height > segmentPos.Y && NPC.position.Y < segmentPos.Y + 16f)
                            {
                                flying = true;
                                break;
                            }
                        }
                    }
                }
            }
            if (!flying)
            {
                NPC.localAI[1] = 1f;
                Rectangle rectangle = new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height);
                int noFlyZone = 1000;
                bool outsideNoFlyZone = true;
                if (NPC.position.Y > Main.player[NPC.target].position.Y)
                {
                    foreach (Player player in Main.ActivePlayers)
                    {
                        Rectangle noFlyArea = new Rectangle((int)player.position.X - noFlyZone, (int)player.position.Y - noFlyZone, noFlyZone * 2, noFlyZone * 2);
                        if (rectangle.Intersects(noFlyArea))
                        {
                            outsideNoFlyZone = false;
                            break;
                        }
                    }
                    if (outsideNoFlyZone)
                    {
                        flying = true;
                    }
                }
            }
            else
            {
                NPC.localAI[1] = 0f;
            }
            float maxChaseSpeed = death ? 13.5f : 10f;
            if (Main.player[NPC.target].dead || (!Main.zenithWorld && (double)Main.player[NPC.target].position.Y < Main.rockLayer * 16.0))
            {
                flying = false;
                NPC.velocity.Y = NPC.velocity.Y + 1f;
                if ((double)NPC.position.Y > Main.rockLayer * 16.0)
                {
                    NPC.velocity.Y = NPC.velocity.Y + 1f;
                    maxChaseSpeed *= 2f;
                }
                if ((double)NPC.position.Y > (double)(Main.UnderworldLayer * 16))
                {
                    for (int a = 0; a < Main.maxNPCs; a++)
                    {
                        if (Main.npc[a].type == ModContent.NPCType<ArmoredDiggerHead>() || Main.npc[a].type == ModContent.NPCType<ArmoredDiggerBody>() ||
                            Main.npc[a].type == ModContent.NPCType<ArmoredDiggerTail>())
                        {
                            Main.npc[a].active = false;
                        }
                    }
                }
            }
            float acceleration = death ? 0.0675f : 0.05f;
            float deceleration = death ? 0.10125f : 0.075f;
            Vector2 segmentPosition = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
            float targetXDist = Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2);
            float targetYDist = Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2);
            targetXDist = (float)((int)(targetXDist / 16f) * 16);
            targetYDist = (float)((int)(targetYDist / 16f) * 16);
            segmentPosition.X = (float)((int)(segmentPosition.X / 16f) * 16);
            segmentPosition.Y = (float)((int)(segmentPosition.Y / 16f) * 16);
            targetXDist -= segmentPosition.X;
            targetYDist -= segmentPosition.Y;
            float targetDistance = (float)Math.Sqrt((double)(targetXDist * targetXDist + targetYDist * targetYDist));
            if (!flying)
            {
                NPC.TargetClosest(true);
                NPC.velocity.Y = NPC.velocity.Y + 0.15f;
                if (NPC.velocity.Y > maxChaseSpeed)
                {
                    NPC.velocity.Y = maxChaseSpeed;
                }
                if ((double)(Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < (double)maxChaseSpeed * 0.4)
                {
                    if (NPC.velocity.X < 0f)
                    {
                        NPC.velocity.X = NPC.velocity.X - acceleration * 1.1f;
                    }
                    else
                    {
                        NPC.velocity.X = NPC.velocity.X + acceleration * 1.1f;
                    }
                }
                else if (NPC.velocity.Y == maxChaseSpeed)
                {
                    if (NPC.velocity.X < targetXDist)
                    {
                        NPC.velocity.X = NPC.velocity.X + acceleration;
                    }
                    else if (NPC.velocity.X > targetXDist)
                    {
                        NPC.velocity.X = NPC.velocity.X - acceleration;
                    }
                }
                else if (NPC.velocity.Y > 4f)
                {
                    if (NPC.velocity.X < 0f)
                    {
                        NPC.velocity.X = NPC.velocity.X + acceleration * 0.9f;
                    }
                    else
                    {
                        NPC.velocity.X = NPC.velocity.X - acceleration * 0.9f;
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
                targetDistance = (float)Math.Sqrt((double)(targetXDist * targetXDist + targetYDist * targetYDist));
                float absoluteTargetX = Math.Abs(targetXDist);
                float absoluteTargetY = Math.Abs(targetYDist);
                float timeToReachTarget = maxChaseSpeed / targetDistance;
                targetXDist *= timeToReachTarget;
                targetYDist *= timeToReachTarget;
                if (((NPC.velocity.X > 0f && targetXDist > 0f) || (NPC.velocity.X < 0f && targetXDist < 0f)) && ((NPC.velocity.Y > 0f && targetYDist > 0f) || (NPC.velocity.Y < 0f && targetYDist < 0f)))
                {
                    if (NPC.velocity.X < targetXDist)
                    {
                        NPC.velocity.X = NPC.velocity.X + deceleration;
                    }
                    else if (NPC.velocity.X > targetXDist)
                    {
                        NPC.velocity.X = NPC.velocity.X - deceleration;
                    }
                    if (NPC.velocity.Y < targetYDist)
                    {
                        NPC.velocity.Y = NPC.velocity.Y + deceleration;
                    }
                    else if (NPC.velocity.Y > targetYDist)
                    {
                        NPC.velocity.Y = NPC.velocity.Y - deceleration;
                    }
                }
                if ((NPC.velocity.X > 0f && targetXDist > 0f) || (NPC.velocity.X < 0f && targetXDist < 0f) || (NPC.velocity.Y > 0f && targetYDist > 0f) || (NPC.velocity.Y < 0f && targetYDist < 0f))
                {
                    if (NPC.velocity.X < targetXDist)
                    {
                        NPC.velocity.X = NPC.velocity.X + acceleration;
                    }
                    else if (NPC.velocity.X > targetXDist)
                    {
                        NPC.velocity.X = NPC.velocity.X - acceleration;
                    }
                    if (NPC.velocity.Y < targetYDist)
                    {
                        NPC.velocity.Y = NPC.velocity.Y + acceleration;
                    }
                    else if (NPC.velocity.Y > targetYDist)
                    {
                        NPC.velocity.Y = NPC.velocity.Y - acceleration;
                    }
                    if ((double)Math.Abs(targetYDist) < (double)maxChaseSpeed * 0.2 && ((NPC.velocity.X > 0f && targetXDist < 0f) || (NPC.velocity.X < 0f && targetXDist > 0f)))
                    {
                        if (NPC.velocity.Y > 0f)
                        {
                            NPC.velocity.Y = NPC.velocity.Y + acceleration * 2f;
                        }
                        else
                        {
                            NPC.velocity.Y = NPC.velocity.Y - acceleration * 2f;
                        }
                    }
                    if ((double)Math.Abs(targetXDist) < (double)maxChaseSpeed * 0.2 && ((NPC.velocity.Y > 0f && targetYDist < 0f) || (NPC.velocity.Y < 0f && targetYDist > 0f)))
                    {
                        if (NPC.velocity.X > 0f)
                        {
                            NPC.velocity.X = NPC.velocity.X + acceleration * 2f;
                        }
                        else
                        {
                            NPC.velocity.X = NPC.velocity.X - acceleration * 2f;
                        }
                    }
                }
                else if (absoluteTargetX > absoluteTargetY)
                {
                    if (NPC.velocity.X < targetXDist)
                    {
                        NPC.velocity.X = NPC.velocity.X + acceleration * 1.1f;
                    }
                    else if (NPC.velocity.X > targetXDist)
                    {
                        NPC.velocity.X = NPC.velocity.X - acceleration * 1.1f;
                    }
                    if ((double)(Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < (double)maxChaseSpeed * 0.5)
                    {
                        if (NPC.velocity.Y > 0f)
                        {
                            NPC.velocity.Y = NPC.velocity.Y + acceleration;
                        }
                        else
                        {
                            NPC.velocity.Y = NPC.velocity.Y - acceleration;
                        }
                    }
                }
                else
                {
                    if (NPC.velocity.Y < targetYDist)
                    {
                        NPC.velocity.Y = NPC.velocity.Y + acceleration * 1.1f;
                    }
                    else if (NPC.velocity.Y > targetYDist)
                    {
                        NPC.velocity.Y = NPC.velocity.Y - acceleration * 1.1f;
                    }
                    if ((double)(Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < (double)maxChaseSpeed * 0.5)
                    {
                        if (NPC.velocity.X > 0f)
                        {
                            NPC.velocity.X = NPC.velocity.X + acceleration;
                        }
                        else
                        {
                            NPC.velocity.X = NPC.velocity.X - acceleration;
                        }
                    }
                }
            }

            // Calculate contact damage based on velocity
            float minimalContactDamageVelocity = maxChaseSpeed * 0.25f;
            float minimalDamageVelocity = maxChaseSpeed * 0.5f;
            if (NPC.velocity.Length() <= minimalContactDamageVelocity)
            {
                NPC.damage = (int)Math.Round(NPC.defDamage * 0.5);
            }
            else
            {
                float velocityDamageScalar = MathHelper.Clamp((NPC.velocity.Length() - minimalContactDamageVelocity) / minimalDamageVelocity, 0f, 1f);
                NPC.damage = (int)MathHelper.Lerp((float)Math.Round(NPC.defDamage * 0.5), NPC.defDamage, velocityDamageScalar);
            }

            NPC.rotation = (float)Math.Atan2((double)NPC.velocity.Y, (double)NPC.velocity.X) + 1.57f;

            if (flying)
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
            potionType = ItemID.GreaterHealingPotion;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ArmoredDiggerHead").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ArmoredDiggerHead2").Type, 1f);
                }
            }
        }

        public override bool SpecialOnKill()
        {
            int closestSegmentID = DropHelper.FindClosestWormSegment(NPC,
                ModContent.NPCType<ArmoredDiggerHead>(),
                ModContent.NPCType<ArmoredDiggerBody>(),
                ModContent.NPCType<ArmoredDiggerTail>());
            NPC.position = Main.npc[closestSegmentID].position;
            return false;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ModContent.ItemType<DemonicBoneAsh>(), 1, 2, 4);
            npcLoot.Add(ModContent.ItemType<MysteriousCircuitry>(), 1, 4, 8);
            npcLoot.Add(ModContent.ItemType<DubiousPlating>(), 1, 4, 8);
            npcLoot.AddIf(() => Main.zenithWorld, ModContent.ItemType<UnholyEssence>(), 1, 3, 6);
            npcLoot.AddIf(() => Main.zenithWorld, ModContent.ItemType<SanctifiedSpark>(), 10);
        }

        public override void ModifyTypeName(ref string typeName)
        {
            if (Main.zenithWorld)
            {
                typeName = CalamityUtils.GetTextValue("NPCs.MechanizedSerpent");
            }
        }

        public override Color? GetAlpha(Color drawColor)
        {
            if (Main.zenithWorld)
            {
                Color lightColor = Color.Orange * drawColor.A;
                return lightColor * NPC.Opacity;
            }
            else return null;
        }
    }
}
