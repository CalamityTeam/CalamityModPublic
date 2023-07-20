using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Placeables.Ores;
using Microsoft.Xna.Framework;
using System;
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
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0);
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
                        for (int num186 = 0; num186 < (Main.getGoodWorld ? 10 : 2); num186++)
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
            Vector2 center16 = NPC.Center;
            Player player8 = Main.player[NPC.target];
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || player8.dead || !player8.active)
            {
                NPC.TargetClosest();
                player8 = Main.player[NPC.target];
                NPC.netUpdate = true;
            }
            if ((player8.dead || Vector2.Distance(player8.Center, center16) > 3200f) && NPC.ai[0] != 1f)
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
                if (!player8.dead)
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
                int num1580 = 0;
                if (NPC.ai[3] >= 580f)
                {
                    num1580 = 0;
                }
                else if (NPC.ai[3] >= 440f)
                {
                    num1580 = 5;
                }
                else if (NPC.ai[3] >= 420f)
                {
                    num1580 = 4;
                }
                else if (NPC.ai[3] >= 280f)
                {
                    num1580 = 3;
                }
                else if (NPC.ai[3] >= 260f)
                {
                    num1580 = 2;
                }
                else if (NPC.ai[3] >= 20f)
                {
                    num1580 = 1;
                }
                NPC.ai[3] += 1f;
                if (NPC.ai[3] >= 600f)
                {
                    NPC.ai[3] = 0f;
                }
                int num1581 = num1580;
                if (NPC.ai[3] >= 580f)
                {
                    num1580 = 0;
                }
                else if (NPC.ai[3] >= 440f)
                {
                    num1580 = 5;
                }
                else if (NPC.ai[3] >= 420f)
                {
                    num1580 = 4;
                }
                else if (NPC.ai[3] >= 280f)
                {
                    num1580 = 3;
                }
                else if (NPC.ai[3] >= 260f)
                {
                    num1580 = 2;
                }
                else if (NPC.ai[3] >= 20f)
                {
                    num1580 = 1;
                }
                if (num1580 != num1581)
                {
                    if (num1580 == 0)
                    {
                        NPC.ai[2] = 0f;
                    }
                    if (num1580 == 1)
                    {
                        NPC.ai[2] = (float)((Math.Sign((player8.Center - center16).X) == 1) ? 1 : -1);
                    }
                    if (num1580 == 2)
                    {
                        NPC.ai[2] = 0f;
                    }
                    NPC.netUpdate = true;
                }
                if (num1580 == 0)
                {
                    if (NPC.ai[2] == 0f)
                    {
                        NPC.ai[2] = (float)(-600 * Math.Sign((center16 - player8.Center).X));
                    }
                    Vector2 vector196 = player8.Center + new Vector2(NPC.ai[2], -250f) - center16;
                    if (vector196.Length() < 50f)
                    {
                        NPC.ai[3] = 19f;
                    }
                    else
                    {
                        vector196.Normalize();
                        NPC.velocity = Vector2.Lerp(NPC.velocity, vector196 * 16f, 0.1f);
                    }
                }
                if (num1580 == 1)
                {
                    int num1582 = (int)NPC.Center.X / 16;
                    int num1583 = (int)(NPC.position.Y + (float)NPC.height) / 16;
                    int num1584 = 0;
                    bool flag149 = Main.tile[num1582, num1583].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[num1582, num1583].TileType] && !Main.tileSolidTop[(int)Main.tile[num1582, num1583].TileType];
                    if (flag149)
                    {
                        num1584 = 1;
                    }
                    else
                    {
                        while (num1584 < 150 && num1583 + num1584 < Main.maxTilesY)
                        {
                            int num1585 = num1583 + num1584;
                            bool flag150 = Main.tile[num1582, num1585].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[num1582, num1585].TileType] && !Main.tileSolidTop[(int)Main.tile[num1582, num1585].TileType];
                            if (flag150)
                            {
                                num1584--;
                                break;
                            }
                            num1584++;
                        }
                    }
                    float num1586 = (float)(num1584 * 16);
                    float num1587 = 250f;
                    if (num1586 < num1587)
                    {
                        float num1588 = -4f;
                        if (-num1588 > num1586)
                        {
                            num1588 = -num1586;
                        }
                        NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, num1588, 0.05f);
                    }
                    else
                    {
                        NPC.velocity.Y = NPC.velocity.Y * 0.95f;
                    }
                    NPC.velocity.X = 3.5f * NPC.ai[2];
                }
                if (num1580 == 2)
                {
                    if (NPC.ai[2] == 0f)
                    {
                        NPC.ai[2] = (float)(300 * Math.Sign((center16 - player8.Center).X));
                    }
                    Vector2 vector197 = player8.Center + new Vector2(NPC.ai[2], -170f) - center16;
                    int num1589 = (int)NPC.Center.X / 16;
                    int num1590 = (int)(NPC.position.Y + (float)NPC.height) / 16;
                    int num1591 = 0;
                    bool flag151 = Main.tile[num1589, num1590].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[num1589, num1590].TileType] && !Main.tileSolidTop[(int)Main.tile[num1589, num1590].TileType];
                    if (flag151)
                    {
                        num1591 = 1;
                    }
                    else
                    {
                        while (num1591 < 150 && num1590 + num1591 < Main.maxTilesY)
                        {
                            int num1592 = num1590 + num1591;
                            bool flag152 = Main.tile[num1589, num1592].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[num1589, num1592].TileType] && !Main.tileSolidTop[(int)Main.tile[num1589, num1592].TileType];
                            if (flag152)
                            {
                                num1591--;
                                break;
                            }
                            num1591++;
                        }
                    }
                    float num1593 = (float)(num1591 * 16);
                    float num1594 = 170f;
                    if (num1593 < num1594)
                    {
                        vector197.Y -= num1594 - num1593;
                    }
                    if (vector197.Length() < 70f)
                    {
                        NPC.ai[3] = 279f;
                    }
                    else
                    {
                        vector197.Normalize();
                        NPC.velocity = Vector2.Lerp(NPC.velocity, vector197 * 20f, 0.1f);
                    }
                }
                else if (num1580 == 3)
                {
                    float num1595 = 0.85f;
                    int num1596 = (int)NPC.Center.X / 16;
                    int num1597 = (int)(NPC.position.Y + (float)NPC.height) / 16;
                    int num1598 = 0;
                    bool flag153 = Main.tile[num1596, num1597].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[num1596, num1597].TileType] && !Main.tileSolidTop[(int)Main.tile[num1596, num1597].TileType];
                    if (flag153)
                    {
                        num1598 = 1;
                    }
                    else
                    {
                        while (num1598 < 150 && num1597 + num1598 < Main.maxTilesY)
                        {
                            int num1599 = num1597 + num1598;
                            bool flag154 = Main.tile[num1596, num1599].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[num1596, num1599].TileType] && !Main.tileSolidTop[(int)Main.tile[num1596, num1599].TileType];
                            if (flag154)
                            {
                                num1598--;
                                break;
                            }
                            num1598++;
                        }
                    }
                    float num1600 = (float)(num1598 * 16);
                    float num1601 = 170f;
                    if (num1600 < num1601)
                    {
                        float num1602 = -4f;
                        if (-num1602 > num1600)
                        {
                            num1602 = -num1600;
                        }
                        NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, num1602, 0.05f);
                    }
                    else
                    {
                        NPC.velocity.Y = NPC.velocity.Y * num1595;
                    }
                    NPC.velocity.X = NPC.velocity.X * num1595;
                }
                if (num1580 == 4)
                {
                    Vector2 vector198 = player8.Center + new Vector2(0f, -250f) - center16;
                    if (vector198.Length() < 50f)
                    {
                        NPC.ai[3] = 439f;
                        return;
                    }
                    vector198.Normalize();
                    NPC.velocity = Vector2.Lerp(NPC.velocity, vector198 * 16f, 0.1f);
                    return;
                }
                else if (num1580 == 5)
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
            var postML = npcLoot.DefineConditionalDropSet(() => NPC.downedMoonlord);
            postML.Add(DropHelper.NormalVsExpertQuantity(ModContent.ItemType<ExodiumCluster>(), 1, 8, 12, 11, 16));
        }
    }
}
