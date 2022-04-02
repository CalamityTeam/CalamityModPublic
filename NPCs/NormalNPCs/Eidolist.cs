using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.SummonItems;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class Eidolist : ModNPC
    {
        public bool hasBeenHit = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eidolist");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            aiType = -1;
            npc.damage = 0;
            npc.width = 60;
            npc.height = 80;
            npc.lifeMax = 10000;
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 1, 0, 0);
            npc.Opacity = 0f;
            npc.noGravity = true;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit13;
            npc.DeathSound = mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/EidolistDeath");
            npc.timeLeft = NPC.activeTime * 2;
            banner = npc.type;
            bannerItem = ModContent.ItemType<EidolistBanner>();
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToSickness = true;
            npc.Calamity().VulnerableToElectricity = true;
            npc.Calamity().VulnerableToWater = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(hasBeenHit);
            writer.Write(npc.chaseable);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            hasBeenHit = reader.ReadBoolean();
            npc.chaseable = reader.ReadBoolean();
        }

        public override void AI()
        {
            bool adultWyrmAlive = false;
            if (CalamityGlobalNPC.adultEidolonWyrmHead != -1)
            {
                if (Main.npc[CalamityGlobalNPC.adultEidolonWyrmHead].active)
                    adultWyrmAlive = true;
            }

            npc.Opacity += 0.15f;
            if (npc.Opacity > 1f)
                npc.Opacity = 1f;

            Lighting.AddLight((int)(npc.Center.X / 16f), (int)(npc.Center.Y / 16f), 0f, 0.4f, 0.5f);
            if (npc.justHit || adultWyrmAlive)
            {
                hasBeenHit = true;
            }
            npc.chaseable = hasBeenHit;
            if (!hasBeenHit)
            {
                if (npc.collideX)
                {
                    npc.velocity.X = npc.velocity.X * -1f;
                    npc.direction *= -1;
                    npc.netUpdate = true;
                }
                if (npc.collideY)
                {
                    npc.netUpdate = true;
                    if (npc.velocity.Y > 0f)
                    {
                        npc.velocity.Y = Math.Abs(npc.velocity.Y) * -1f;
                        npc.directionY = -1;
                        npc.localAI[2] = -1f;
                    }
                    else if (npc.velocity.Y < 0f)
                    {
                        npc.velocity.Y = Math.Abs(npc.velocity.Y);
                        npc.directionY = 1;
                        npc.localAI[2] = 1f;
                    }
                }
                npc.velocity.X += npc.direction * 0.1f;
                if (npc.velocity.X < -2f || npc.velocity.X > 2f)
                {
                    npc.velocity.X = npc.velocity.X * 0.95f;
                }
                if (npc.localAI[2] == -1f)
                {
                    npc.velocity.Y = npc.velocity.Y - 0.01f;
                    if (npc.velocity.Y < -0.3f)
                    {
                        npc.localAI[2] = 1f;
                    }
                }
                else
                {
                    npc.velocity.Y = npc.velocity.Y + 0.01f;
                    if (npc.velocity.Y > 0.3f)
                    {
                        npc.localAI[2] = -1f;
                    }
                }
                if (npc.velocity.Y > 0.4f || npc.velocity.Y < -0.4f)
                {
                    npc.velocity.Y *= 0.95f;
                }
                return;
            }
            npc.noTileCollide = true;
            float num1446 = adultWyrmAlive ? 14f : 7f;
            float num1447 = 480f;
            if (npc.localAI[1] == 1f)
            {
                npc.localAI[1] = 0f;
                if (Main.rand.NextBool(4))
                {
                    npc.ai[0] = num1447;
                }
            }
            npc.TargetClosest(true);
            npc.rotation = Math.Abs(npc.velocity.X) * (float)npc.direction * 0.1f;
            npc.spriteDirection = (npc.direction > 0) ? 1 : -1;
            Vector2 value53 = npc.Center + new Vector2((float)(npc.direction * 20), 6f);
            Vector2 vector251 = Main.player[npc.target].Center - value53;
            bool flag104 = Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1);
            npc.localAI[0] += 1f;
            if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[0] >= Main.rand.Next(90, 601))
            {
                npc.localAI[0] = -90f;
                npc.netUpdate = true;
                if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                {
                    float speed = adultWyrmAlive ? 10f : 5f;
                    Vector2 vector = new Vector2(npc.Center.X, npc.Center.Y);
                    float xDist = Main.player[npc.target].Center.X - vector.X + Main.rand.NextFloat(-10f, 10f);
                    float yDist = Main.player[npc.target].Center.Y - vector.Y + Main.rand.NextFloat(-10f, 10f);
                    Vector2 targetVec = new Vector2(yDist, yDist);
                    float targetDist = targetVec.Length();
                    targetDist = speed / targetDist;
                    targetVec.X *= targetDist;
                    targetVec.Y *= targetDist;
                    int damage = adultWyrmAlive ? (Main.expertMode ? 150 : 200) : (Main.expertMode ? 30 : 40);
                    if (Main.rand.NextBool(2))
                    {
                        Projectile.NewProjectile(npc.Center, targetVec, ProjectileID.CultistBossLightningOrb, damage, 0f, Main.myPlayer, 0f, 0f);
                    }
                    else
                    {
                        Vector2 vec = Vector2.Normalize(Main.player[npc.target].Center - npc.Center);
                        vec = Vector2.Normalize(Main.player[npc.target].Center - npc.Center + Main.player[npc.target].velocity * 20f);
                        if (vec.HasNaNs())
                        {
                            vec = new Vector2((float)npc.direction, 0f);
                        }
                        for (int n = 0; n < 1; n++)
                        {
                            Vector2 vector4 = vec * 4f;
                            Projectile.NewProjectile(npc.Center, vector4, ProjectileID.CultistBossIceMist, damage, 0f, Main.myPlayer, 0f, 1f);
                        }
                    }
                }
            }
            if (vector251.Length() > 400f || !flag104)
            {
                Vector2 value54 = vector251;
                if (value54.Length() > num1446)
                {
                    value54.Normalize();
                    value54 *= num1446;
                }
                int num1448 = 30;
                npc.velocity = (npc.velocity * (float)(num1448 - 1) + value54) / (float)num1448;
            }
            else
            {
                npc.velocity *= 0.98f;
            }
            if (npc.ai[2] != 0f && npc.ai[3] != 0f)
            {
                Main.PlaySound(SoundID.Item8, npc.Center);
                for (int num1449 = 0; num1449 < 20; num1449++)
                {
                    int num1450 = Dust.NewDust(npc.position, npc.width, npc.height, 20, 0f, 0f, 100, Color.Transparent, 1f);
                    Dust dust = Main.dust[num1450];
                    dust.velocity *= 3f;
                    dust.noGravity = true;
                    dust.scale = 2.5f;
                }
                npc.Center = new Vector2(npc.ai[2] * 16f, npc.ai[3] * 16f);
                npc.velocity = Vector2.Zero;
                npc.ai[2] = 0f;
                npc.ai[3] = 0f;
                Main.PlaySound(SoundID.Item8, npc.Center);
                for (int num1451 = 0; num1451 < 20; num1451++)
                {
                    int num1452 = Dust.NewDust(npc.position, npc.width, npc.height, 20, 0f, 0f, 100, Color.Transparent, 1f);
                    Dust dust = Main.dust[num1452];
                    dust.velocity *= 3f;
                    dust.noGravity = true;
                    dust.scale = 2.5f;
                }
            }
            npc.ai[0] += 1f;
            if (npc.ai[0] >= num1447 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.ai[0] = 0f;
                Point point12 = npc.Center.ToTileCoordinates();
                Point point13 = Main.player[npc.target].Center.ToTileCoordinates();
                int num1453 = 20;
                int num1454 = 3;
                int num1455 = 10;
                int num1456 = 1;
                int num1457 = 0;
                bool flag106 = false;
                if (vector251.Length() > 2000f)
                {
                    flag106 = true;
                }
                while (!flag106 && num1457 < 100)
                {
                    num1457++;
                    int num1458 = Main.rand.Next(point13.X - num1453, point13.X + num1453 + 1);
                    int num1459 = Main.rand.Next(point13.Y - num1453, point13.Y + num1453 + 1);
                    if ((num1459 < point13.Y - num1455 || num1459 > point13.Y + num1455 || num1458 < point13.X - num1455 || num1458 > point13.X + num1455) && (num1459 < point12.Y - num1454 || num1459 > point12.Y + num1454 || num1458 < point12.X - num1454 || num1458 > point12.X + num1454) && !Main.tile[num1458, num1459].nactive())
                    {
                        bool flag107 = true;
                        if (flag107 && Main.tile[num1458, num1459].lava())
                        {
                            flag107 = false;
                        }
                        if (flag107 && Collision.SolidTiles(num1458 - num1456, num1458 + num1456, num1459 - num1456, num1459 + num1456))
                        {
                            flag107 = false;
                        }
                        if (flag107)
                        {
                            npc.ai[2] = (float)num1458;
                            npc.ai[3] = (float)num1459;
                            break;
                        }
                    }
                }
                npc.netUpdate = true;
            }
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.minion && !projectile.Calamity().overridesMinionDamagePrevention)
            {
                return hasBeenHit;
            }
            return null;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += hasBeenHit ? 0.15f : 0.075f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (!Main.hardMode || NPC.AnyNPCs(ModContent.NPCType<Eidolist>()))
            {
                return 0f;
            }
            if (spawnInfo.player.Calamity().ZoneAbyssLayer3 && spawnInfo.water)
            {
                return 0.05f;
            }
            return 0f;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 4, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 4, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/EidiolistGores/Eidolist"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/EidiolistGores/Eidolist2"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/EidiolistGores/Eidolist3"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/EidiolistGores/Eidolist4"), npc.scale);
            }
        }

        public override bool PreNPCLoot()
        {
            bool adultWyrmAlive = false;
            if (CalamityGlobalNPC.adultEidolonWyrmHead != -1)
            {
                if (Main.npc[CalamityGlobalNPC.adultEidolonWyrmHead].active)
                    adultWyrmAlive = true;
            }

            return !adultWyrmAlive;
        }

        public override void NPCLoot()
        {
            if (Main.rand.NextBool(10))
            {
                DropHelper.DropItem(npc, ItemID.BlueLunaticHood);
                DropHelper.DropItem(npc, ItemID.BlueLunaticRobe);
            }
            DropHelper.DropItemCondition(npc, ModContent.ItemType<EidolonTablet>(), !NPC.LunarApocalypseIsUp, 0.25f);
            int minLumenyl = Main.expertMode ? 10 : 8;
            int maxLumenyl = Main.expertMode ? 14 : 10;
            DropHelper.DropItemCondition(npc, ModContent.ItemType<Lumenite>(), CalamityWorld.downedCalamitas, 1f, minLumenyl, maxLumenyl);
            DropHelper.DropItemCondition(npc, ItemID.Ectoplasm, NPC.downedPlantBoss, 1f, 3, 5);
        }
    }
}
