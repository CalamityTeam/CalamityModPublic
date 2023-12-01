using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Placeables.Ores;
using Microsoft.Xna.Framework;
using System;
using CalamityMod.Items.Accessories.Vanity;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class ShockstormShuttle : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.npcSlots = 3f;
            NPC.damage = 30;
            NPC.width = 64;
            NPC.height = 38;
            NPC.defense = 15;
            NPC.DR_NERD(0.15f);
            NPC.lifeMax = 150;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<ShockstormShuttleBanner>();
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.ShockstormShuttle")
            });
        }

        public override void AI()
        {
            if (NPC.justHit)
                NPC.localAI[0] = 0f;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.localAI[0] += 1f;
                if (NPC.localAI[0] >= 60f)
                {
                    NPC.localAI[0] = 0f;
                    if (Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                    {
                        float projSpeed = 12f;
                        Vector2 npcPos = NPC.Center;
                        float targetX = Main.player[NPC.target].Center.X - npcPos.X;
                        float YAdjust = Math.Abs(targetX) * 0.1f;
                        float targetY = Main.player[NPC.target].Center.Y - npcPos.Y - YAdjust;
                        Vector2 velocity = new Vector2(targetX, targetY);
                        float targetDist = velocity.Length();
                        targetDist = projSpeed / targetDist;
                        velocity.X *= targetDist;
                        velocity.Y *= targetDist;
                        int projDmg = 30;
                        if (Main.expertMode)
                        {
                            projDmg = 22;
                        }
                        int projType = ProjectileID.MartianTurretBolt;
                        if (Main.rand.NextBool(8))
                        {
                            projType = ProjectileID.SaucerLaser;
                        }
                        npcPos.X += velocity.X;
                        npcPos.Y += velocity.Y;
                        int spread = Main.getGoodWorld ? 100 : 20;
                        for (int i = 0; i < (Main.getGoodWorld ? 10 : 2); i++)
                        {
                            velocity = Main.player[NPC.target].Center - npcPos;
                            targetDist = velocity.Length();
                            targetDist = projSpeed / targetDist;
                            velocity.X += Main.rand.Next(-spread, spread + 1);
                            velocity.Y += Main.rand.Next(-spread, spread + 1);
                            velocity.X *= targetDist;
                            velocity.Y *= targetDist;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), npcPos, velocity, projType, projDmg, 0f, Main.myPlayer, 0f, 0f);
                        }
                    }
                }
            }
            if (NPC.localAI[3] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.localAI[3] = 1f;
            }
            Vector2 shuttleCenter = NPC.Center;
            Player targetedPlayer = Main.player[NPC.target];
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || targetedPlayer.dead || !targetedPlayer.active)
            {
                NPC.TargetClosest();
                targetedPlayer = Main.player[NPC.target];
                NPC.netUpdate = true;
            }
            if ((targetedPlayer.dead || Vector2.Distance(targetedPlayer.Center, shuttleCenter) > 3200f) && NPC.ai[0] != 1f)
            {
                NPC.ai[0] = -1f;
                NPC.netUpdate = true;
            }
            if (NPC.ai[0] == -1f)
            {
                NPC.velocity.Y = NPC.velocity.Y - 0.4f;
                if (NPC.timeLeft > 10)
                {
                    NPC.timeLeft = 10;
                }
                if (!targetedPlayer.dead)
                {
                    NPC.timeLeft = 300;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.netUpdate = true;
                    return;
                }
            }
            else if (NPC.ai[0] == 0f)
            {
                int movementPattern = 0;
                if (NPC.ai[3] >= 580f)
                {
                    movementPattern = 0;
                }
                else if (NPC.ai[3] >= 440f)
                {
                    movementPattern = 5;
                }
                else if (NPC.ai[3] >= 420f)
                {
                    movementPattern = 4;
                }
                else if (NPC.ai[3] >= 280f)
                {
                    movementPattern = 3;
                }
                else if (NPC.ai[3] >= 260f)
                {
                    movementPattern = 2;
                }
                else if (NPC.ai[3] >= 20f)
                {
                    movementPattern = 1;
                }
                NPC.ai[3] += 1f;
                if (NPC.ai[3] >= 600f)
                {
                    NPC.ai[3] = 0f;
                }
                int patternCompare = movementPattern;
                if (NPC.ai[3] >= 580f)
                {
                    movementPattern = 0;
                }
                else if (NPC.ai[3] >= 440f)
                {
                    movementPattern = 5;
                }
                else if (NPC.ai[3] >= 420f)
                {
                    movementPattern = 4;
                }
                else if (NPC.ai[3] >= 280f)
                {
                    movementPattern = 3;
                }
                else if (NPC.ai[3] >= 260f)
                {
                    movementPattern = 2;
                }
                else if (NPC.ai[3] >= 20f)
                {
                    movementPattern = 1;
                }
                if (movementPattern != patternCompare)
                {
                    if (movementPattern == 0)
                    {
                        NPC.ai[2] = 0f;
                    }
                    if (movementPattern == 1)
                    {
                        NPC.ai[2] = (float)((Math.Sign((targetedPlayer.Center - shuttleCenter).X) == 1) ? 1 : -1);
                    }
                    if (movementPattern == 2)
                    {
                        NPC.ai[2] = 0f;
                    }
                    NPC.netUpdate = true;
                }
                if (movementPattern == 0)
                {
                    if (NPC.ai[2] == 0f)
                    {
                        NPC.ai[2] = (float)(-600 * Math.Sign((shuttleCenter - targetedPlayer.Center).X));
                    }
                    Vector2 targetingDirection = targetedPlayer.Center + new Vector2(NPC.ai[2], -250f) - shuttleCenter;
                    if (targetingDirection.Length() < 50f)
                    {
                        NPC.ai[3] = 19f;
                    }
                    else
                    {
                        targetingDirection.Normalize();
                        NPC.velocity = Vector2.Lerp(NPC.velocity, targetingDirection * 16f, 0.1f);
                    }
                }
                if (movementPattern == 1)
                {
                    int npcTileX = (int)NPC.Center.X / 16;
                    int npcTileY = (int)(NPC.position.Y + (float)NPC.height) / 16;
                    int upwardMoveAmt = 0;
                    bool solidTilePresent = Main.tile[npcTileX, npcTileY].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[npcTileX, npcTileY].TileType] && !Main.tileSolidTop[(int)Main.tile[npcTileX, npcTileY].TileType];
                    if (solidTilePresent)
                    {
                        upwardMoveAmt = 1;
                    }
                    else
                    {
                        while (upwardMoveAmt < 150 && npcTileY + upwardMoveAmt < Main.maxTilesY)
                        {
                            int npcTileYUp = npcTileY + upwardMoveAmt;
                            bool solidTilePresentAgain = Main.tile[npcTileX, npcTileYUp].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[npcTileX, npcTileYUp].TileType] && !Main.tileSolidTop[(int)Main.tile[npcTileX, npcTileYUp].TileType];
                            if (solidTilePresentAgain)
                            {
                                upwardMoveAmt--;
                                break;
                            }
                            upwardMoveAmt++;
                        }
                    }
                    float pixelsUpAmt = (float)(upwardMoveAmt * 16);
                    if (pixelsUpAmt < 250f)
                    {
                        float velocityUpwards = -4f;
                        if (-velocityUpwards > pixelsUpAmt)
                        {
                            velocityUpwards = -pixelsUpAmt;
                        }
                        NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, velocityUpwards, 0.05f);
                    }
                    else
                    {
                        NPC.velocity.Y = NPC.velocity.Y * 0.95f;
                    }
                    NPC.velocity.X = 3.5f * NPC.ai[2];
                }
                if (movementPattern == 2)
                {
                    if (NPC.ai[2] == 0f)
                    {
                        NPC.ai[2] = (float)(300 * Math.Sign((shuttleCenter - targetedPlayer.Center).X));
                    }
                    Vector2 targetingCenter2 = targetedPlayer.Center + new Vector2(NPC.ai[2], -170f) - shuttleCenter;
                    int npcTileX2 = (int)NPC.Center.X / 16;
                    int npcTileY2 = (int)(NPC.position.Y + (float)NPC.height) / 16;
                    int upwardsMovement = 0;
                    bool solidTile = Main.tile[npcTileX2, npcTileY2].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[npcTileX2, npcTileY2].TileType] && !Main.tileSolidTop[(int)Main.tile[npcTileX2, npcTileY2].TileType];
                    if (solidTile)
                    {
                        upwardsMovement = 1;
                    }
                    else
                    {
                        while (upwardsMovement < 150 && npcTileY2 + upwardsMovement < Main.maxTilesY)
                        {
                            int npcTileYUp2 = npcTileY2 + upwardsMovement;
                            bool solidTileAgain = Main.tile[npcTileX2, npcTileYUp2].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[npcTileX2, npcTileYUp2].TileType] && !Main.tileSolidTop[(int)Main.tile[npcTileX2, npcTileYUp2].TileType];
                            if (solidTileAgain)
                            {
                                upwardsMovement--;
                                break;
                            }
                            upwardsMovement++;
                        }
                    }
                    float pixelsUpAmt2 = (float)(upwardsMovement * 16);
                    if (pixelsUpAmt2 < 170f)
                    {
                        targetingCenter2.Y -= 170f - pixelsUpAmt2;
                    }
                    if (targetingCenter2.Length() < 70f)
                    {
                        NPC.ai[3] = 279f;
                    }
                    else
                    {
                        targetingCenter2.Normalize();
                        NPC.velocity = Vector2.Lerp(NPC.velocity, targetingCenter2 * 20f, 0.1f);
                    }
                }
                else if (movementPattern == 3)
                {
                    float decelerationMult = 0.85f;
                    int npcTileX3 = (int)NPC.Center.X / 16;
                    int npcTileY3 = (int)(NPC.position.Y + (float)NPC.height) / 16;
                    int upwardsMovement3 = 0;
                    bool thereIsSolidTile = Main.tile[npcTileX3, npcTileY3].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[npcTileX3, npcTileY3].TileType] && !Main.tileSolidTop[(int)Main.tile[npcTileX3, npcTileY3].TileType];
                    if (thereIsSolidTile)
                    {
                        upwardsMovement3 = 1;
                    }
                    else
                    {
                        while (upwardsMovement3 < 150 && npcTileY3 + upwardsMovement3 < Main.maxTilesY)
                        {
                            int npcTileYUp3 = npcTileY3 + upwardsMovement3;
                            bool thereIsSolidTileAgain = Main.tile[npcTileX3, npcTileYUp3].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[npcTileX3, npcTileYUp3].TileType] && !Main.tileSolidTop[(int)Main.tile[npcTileX3, npcTileYUp3].TileType];
                            if (thereIsSolidTileAgain)
                            {
                                upwardsMovement3--;
                                break;
                            }
                            upwardsMovement3++;
                        }
                    }
                    float pixelUpAmt3 = (float)(upwardsMovement3 * 16);
                    if (pixelUpAmt3 < 170f)
                    {
                        float velocityUpwards3 = -4f;
                        if (-velocityUpwards3 > pixelUpAmt3)
                        {
                            velocityUpwards3 = -pixelUpAmt3;
                        }
                        NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, velocityUpwards3, 0.05f);
                    }
                    else
                    {
                        NPC.velocity.Y = NPC.velocity.Y * decelerationMult;
                    }
                    NPC.velocity.X = NPC.velocity.X * decelerationMult;
                }
                if (movementPattern == 4)
                {
                    Vector2 targetingCenter4 = targetedPlayer.Center + new Vector2(0f, -250f) - shuttleCenter;
                    if (targetingCenter4.Length() < 50f)
                    {
                        NPC.ai[3] = 439f;
                        return;
                    }
                    targetingCenter4.Normalize();
                    NPC.velocity = Vector2.Lerp(NPC.velocity, targetingCenter4 * 16f, 0.1f);
                    return;
                }
                else if (movementPattern == 5)
                {
                    NPC.velocity *= 0.85f;
                    return;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 234, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 234, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ShockstormShuttle").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ShockstormShuttle2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ShockstormShuttle3").Type, 1f);
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe || !Main.hardMode)
            {
                return 0f;
            }
            return SpawnCondition.Sky.Chance * (Main.getGoodWorld ? 0.5f : 0.1f);
        }
        
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.AddIf(() => NPC.downedGolemBoss, ItemID.MartianConduitPlating, 1, 10, 30);
            npcLoot.Add(ModContent.ItemType<EssenceofSunlight>(), 2);
            npcLoot.Add(ModContent.ItemType<TheTransformer>(), 10);
            npcLoot.Add(ModContent.ItemType<OracleHeadphones>(), (DateTime.Now.Day == 25 && DateTime.Now.Month == 5) ? 9 : 20);
            var postML = npcLoot.DefineConditionalDropSet(() => NPC.downedMoonlord);
            postML.Add(DropHelper.NormalVsExpertQuantity(ModContent.ItemType<ExodiumCluster>(), 1, 8, 12, 11, 16));
        }
    }
}
