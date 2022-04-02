using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class ShockstormShuttle : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shockstorm Shuttle");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.npcSlots = 3f;
            npc.damage = 30;
            npc.width = 64;
            npc.height = 38;
            npc.defense = 15;
            npc.DR_NERD(0.15f);
            npc.lifeMax = 150;
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 0, 5, 0);
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.noGravity = true;
            npc.noTileCollide = true;
            banner = npc.type;
            bannerItem = ModContent.ItemType<ShockstormShuttleBanner>();
            npc.Calamity().VulnerableToSickness = false;
            npc.Calamity().VulnerableToElectricity = true;
        }

        public override void AI()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.localAI[0] += 1f;
                if (npc.localAI[0] >= 60f)
                {
                    npc.localAI[0] = 0f;
                    if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                    {
                        float projSpeed = 12f;
                        Vector2 npcPos = npc.Center;
                        float targetX = Main.player[npc.target].Center.X - npcPos.X;
                        float YAdjust = Math.Abs(targetX) * 0.1f;
                        float targetY = Main.player[npc.target].Center.Y - npcPos.Y - YAdjust;
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
                        for (int num186 = 0; num186 < 2; num186++)
                        {
                            velocity = Main.player[npc.target].Center - npcPos;
                            targetDist = velocity.Length();
                            targetDist = projSpeed / targetDist;
                            velocity.X += Main.rand.Next(-20, 21);
                            velocity.Y += Main.rand.Next(-20, 21);
                            velocity.X *= targetDist;
                            velocity.Y *= targetDist;
                            Projectile.NewProjectile(npcPos, velocity, projType, projDmg, 0f, Main.myPlayer, 0f, 0f);
                        }
                    }
                }
            }
            if (npc.localAI[3] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.localAI[3] = 1f;
            }
            Vector2 center16 = npc.Center;
            Player player8 = Main.player[npc.target];
            if (npc.target < 0 || npc.target == Main.maxPlayers || player8.dead || !player8.active)
            {
                npc.TargetClosest();
                player8 = Main.player[npc.target];
                npc.netUpdate = true;
            }
            if ((player8.dead || Vector2.Distance(player8.Center, center16) > 3200f) && npc.ai[0] != 1f)
            {
                npc.ai[0] = -1f;
                npc.netUpdate = true;
            }
            if (npc.ai[0] == -1f)
            {
                npc.velocity.Y = npc.velocity.Y - 0.4f;
                if (npc.timeLeft > 10)
                {
                    npc.timeLeft = 10;
                }
                if (!player8.dead)
                {
                    npc.timeLeft = 300;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.netUpdate = true;
                    return;
                }
            }
            else if (npc.ai[0] == 0f)
            {
                int num1580 = 0;
                if (npc.ai[3] >= 580f)
                {
                    num1580 = 0;
                }
                else if (npc.ai[3] >= 440f)
                {
                    num1580 = 5;
                }
                else if (npc.ai[3] >= 420f)
                {
                    num1580 = 4;
                }
                else if (npc.ai[3] >= 280f)
                {
                    num1580 = 3;
                }
                else if (npc.ai[3] >= 260f)
                {
                    num1580 = 2;
                }
                else if (npc.ai[3] >= 20f)
                {
                    num1580 = 1;
                }
                npc.ai[3] += 1f;
                if (npc.ai[3] >= 600f)
                {
                    npc.ai[3] = 0f;
                }
                int num1581 = num1580;
                if (npc.ai[3] >= 580f)
                {
                    num1580 = 0;
                }
                else if (npc.ai[3] >= 440f)
                {
                    num1580 = 5;
                }
                else if (npc.ai[3] >= 420f)
                {
                    num1580 = 4;
                }
                else if (npc.ai[3] >= 280f)
                {
                    num1580 = 3;
                }
                else if (npc.ai[3] >= 260f)
                {
                    num1580 = 2;
                }
                else if (npc.ai[3] >= 20f)
                {
                    num1580 = 1;
                }
                if (num1580 != num1581)
                {
                    if (num1580 == 0)
                    {
                        npc.ai[2] = 0f;
                    }
                    if (num1580 == 1)
                    {
                        npc.ai[2] = (float)((Math.Sign((player8.Center - center16).X) == 1) ? 1 : -1);
                    }
                    if (num1580 == 2)
                    {
                        npc.ai[2] = 0f;
                    }
                    npc.netUpdate = true;
                }
                if (num1580 == 0)
                {
                    if (npc.ai[2] == 0f)
                    {
                        npc.ai[2] = (float)(-600 * Math.Sign((center16 - player8.Center).X));
                    }
                    Vector2 vector196 = player8.Center + new Vector2(npc.ai[2], -250f) - center16;
                    if (vector196.Length() < 50f)
                    {
                        npc.ai[3] = 19f;
                    }
                    else
                    {
                        vector196.Normalize();
                        npc.velocity = Vector2.Lerp(npc.velocity, vector196 * 16f, 0.1f);
                    }
                }
                if (num1580 == 1)
                {
                    int num1582 = (int)npc.Center.X / 16;
                    int num1583 = (int)(npc.position.Y + (float)npc.height) / 16;
                    int num1584 = 0;
                    bool flag149 = Main.tile[num1582, num1583].nactive() && Main.tileSolid[(int)Main.tile[num1582, num1583].type] && !Main.tileSolidTop[(int)Main.tile[num1582, num1583].type];
                    if (flag149)
                    {
                        num1584 = 1;
                    }
                    else
                    {
                        while (num1584 < 150 && num1583 + num1584 < Main.maxTilesY)
                        {
                            int num1585 = num1583 + num1584;
                            bool flag150 = Main.tile[num1582, num1585].nactive() && Main.tileSolid[(int)Main.tile[num1582, num1585].type] && !Main.tileSolidTop[(int)Main.tile[num1582, num1585].type];
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
                        npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, num1588, 0.05f);
                    }
                    else
                    {
                        npc.velocity.Y = npc.velocity.Y * 0.95f;
                    }
                    npc.velocity.X = 3.5f * npc.ai[2];
                }
                if (num1580 == 2)
                {
                    if (npc.ai[2] == 0f)
                    {
                        npc.ai[2] = (float)(300 * Math.Sign((center16 - player8.Center).X));
                    }
                    Vector2 vector197 = player8.Center + new Vector2(npc.ai[2], -170f) - center16;
                    int num1589 = (int)npc.Center.X / 16;
                    int num1590 = (int)(npc.position.Y + (float)npc.height) / 16;
                    int num1591 = 0;
                    bool flag151 = Main.tile[num1589, num1590].nactive() && Main.tileSolid[(int)Main.tile[num1589, num1590].type] && !Main.tileSolidTop[(int)Main.tile[num1589, num1590].type];
                    if (flag151)
                    {
                        num1591 = 1;
                    }
                    else
                    {
                        while (num1591 < 150 && num1590 + num1591 < Main.maxTilesY)
                        {
                            int num1592 = num1590 + num1591;
                            bool flag152 = Main.tile[num1589, num1592].nactive() && Main.tileSolid[(int)Main.tile[num1589, num1592].type] && !Main.tileSolidTop[(int)Main.tile[num1589, num1592].type];
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
                        npc.ai[3] = 279f;
                    }
                    else
                    {
                        vector197.Normalize();
                        npc.velocity = Vector2.Lerp(npc.velocity, vector197 * 20f, 0.1f);
                    }
                }
                else if (num1580 == 3)
                {
                    float num1595 = 0.85f;
                    int num1596 = (int)npc.Center.X / 16;
                    int num1597 = (int)(npc.position.Y + (float)npc.height) / 16;
                    int num1598 = 0;
                    bool flag153 = Main.tile[num1596, num1597].nactive() && Main.tileSolid[(int)Main.tile[num1596, num1597].type] && !Main.tileSolidTop[(int)Main.tile[num1596, num1597].type];
                    if (flag153)
                    {
                        num1598 = 1;
                    }
                    else
                    {
                        while (num1598 < 150 && num1597 + num1598 < Main.maxTilesY)
                        {
                            int num1599 = num1597 + num1598;
                            bool flag154 = Main.tile[num1596, num1599].nactive() && Main.tileSolid[(int)Main.tile[num1596, num1599].type] && !Main.tileSolidTop[(int)Main.tile[num1596, num1599].type];
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
                        npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, num1602, 0.05f);
                    }
                    else
                    {
                        npc.velocity.Y = npc.velocity.Y * num1595;
                    }
                    npc.velocity.X = npc.velocity.X * num1595;
                }
                if (num1580 == 4)
                {
                    Vector2 vector198 = player8.Center + new Vector2(0f, -250f) - center16;
                    if (vector198.Length() < 50f)
                    {
                        npc.ai[3] = 439f;
                        return;
                    }
                    vector198.Normalize();
                    npc.velocity = Vector2.Lerp(npc.velocity, vector198 * 16f, 0.1f);
                    return;
                }
                else if (num1580 == 5)
                {
                    npc.velocity *= 0.85f;
                    return;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 234, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 234, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || !Main.hardMode)
            {
                return 0f;
            }
            return SpawnCondition.Sky.Chance * 0.1f;
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemCondition(npc, ItemID.MartianConduitPlating, NPC.downedGolemBoss, 1, 10, 30);
            DropHelper.DropItemChance(npc, ModContent.ItemType<EssenceofCinder>(), 3);
            DropHelper.DropItemChance(npc, ModContent.ItemType<TheTransformer>(), 10);
        }
    }
}
