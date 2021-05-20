using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.HiveMind
{
	[AutoloadBossHead]
    public class HiveMind : ModNPC
    {
        int burrowTimer = 420;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Hive Mind");
            Main.npcFrameCount[npc.type] = 16;
        }

        public override void SetDefaults()
        {
            npc.npcSlots = 5f;
			npc.GetNPCDamage();
			npc.width = 150;
            npc.height = 120;
            npc.defense = 10;
            npc.LifeMaxNERB(1200, 1800, 350000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.boss = true;
            npc.value = 0f;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            music = CalamityMod.Instance.GetMusicFromMusicMod("HiveMind") ?? MusicID.Boss2;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.dontTakeDamage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.dontTakeDamage = reader.ReadBoolean();
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 1f / 6f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
			// Get a target
			if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest();

			// Despawn safety, make sure to target another player if the current player target is too far away
			if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
				npc.TargetClosest();

			Player player = Main.player[npc.target];
			if (!player.active || player.dead)
			{
				npc.TargetClosest(false);
				player = Main.player[npc.target];
				if (!player.active || player.dead)
				{
					if (npc.timeLeft > 60)
						npc.timeLeft = 60;
					if (npc.localAI[3] < 120f)
					{
						float[] aiArray = npc.localAI;
						int number = 3;
						float num244 = aiArray[number];
						aiArray[number] = num244 + 1f;
					}
					if (npc.localAI[3] > 60f)
					{
						npc.velocity.Y += (npc.localAI[3] - 60f) * 0.5f;
						npc.noGravity = true;
						npc.noTileCollide = true;
						if (burrowTimer > 30)
							burrowTimer = 30;
					}
					return;
				}
			}
			else if (npc.timeLeft < 1800)
				npc.timeLeft = 1800;

            if (npc.localAI[3] > 0f)
            {
                float[] aiArray = npc.localAI;
                int number = 3;
                float num244 = aiArray[number];
                aiArray[number] = num244 - 1f;
                return;
            }

            npc.noGravity = false;
            npc.noTileCollide = false;

			bool malice = CalamityWorld.malice;
			bool expertMode = Main.expertMode || BossRushEvent.BossRushActive || malice;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive || malice;
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive || malice;
			CalamityGlobalNPC.hiveMind = npc.whoAmI;

			float enrageScale = 0f;
			if ((npc.position.Y / 16f) < Main.worldSurface || malice)
				enrageScale += 1f;
			if (!player.ZoneCorrupt || malice)
				enrageScale += 1f;

			if (BossRushEvent.BossRushActive)
				enrageScale = 0f;

			if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (npc.localAI[0] == 0f)
                {
                    npc.localAI[0] = 1f;
					int maxBlobs = death ? 15 : revenge ? 7 : expertMode ? 6 : 5;
                    for (int i = 0; i < maxBlobs; i++)
                    {
                        NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<HiveBlob>(), npc.whoAmI);
                    }
                }
            }

            bool flag100 = false;
            int num568 = 0;
            if (expertMode)
            {
                for (int num569 = 0; num569 < Main.maxNPCs; num569++)
                {
                    if (Main.npc[num569].active && Main.npc[num569].type == ModContent.NPCType<DankCreeper>())
                    {
                        flag100 = true;
                        num568++;
                    }
                }

                npc.defense += num568 * 25;

				if (!flag100)
					npc.defense = npc.defDefense;
			}

            if (npc.ai[3] == 0f && npc.life > 0)
            {
                npc.ai[3] = npc.lifeMax;
            }
            if (npc.life > 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int num660 = (int)(npc.lifeMax * 0.25);
                    if ((npc.life + num660) < npc.ai[3])
                    {
                        npc.ai[3] = npc.life;
						int maxSpawns = malice ? 10 : death ? 5 : revenge ? 4 : expertMode ? Main.rand.Next(3, 5) : Main.rand.Next(2, 4);
						int maxDankSpawns = malice ? 4 : death ? Main.rand.Next(2, 4) : revenge ? 2 : expertMode ? Main.rand.Next(1, 3) : 1;
						for (int num662 = 0; num662 < maxSpawns; num662++)
                        {
                            int x = (int)(npc.position.X + Main.rand.Next(npc.width - 32));
                            int y = (int)(npc.position.Y + Main.rand.Next(npc.height - 32));
                            int type = ModContent.NPCType<HiveBlob>();
                            if (NPC.CountNPCS(ModContent.NPCType<DankCreeper>()) < maxDankSpawns || npc.Calamity().enraged > 0)
                            {
                                type = ModContent.NPCType<DankCreeper>();
                            }
                            int num664 = NPC.NewNPC(x, y, type);
                            Main.npc[num664].SetDefaults(type);
                            if (Main.netMode == NetmodeID.Server && num664 < Main.maxNPCs)
                            {
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num664, 0f, 0f, 0f, 0, 0, 0);
                            }
                        }
                        return;
                    }
                }
            }

            burrowTimer--;
            if (burrowTimer < -120)
            {
                burrowTimer = (death ? 180 : revenge ? 300 : expertMode ? 360 : 420) - (int)enrageScale * 60;
                npc.scale = 1f;
                npc.alpha = 0;
                npc.dontTakeDamage = false;
                npc.damage = npc.defDamage;
            }
            else if (burrowTimer < -60)
            {
                npc.scale += 0.0165f;
                npc.alpha -= 4;
                int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.Center.Y), npc.width, npc.height / 2, 14, 0f, -3f, 100, default, 2.5f * npc.scale);
                Main.dust[num622].velocity *= 2f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
                for (int i = 0; i < 2; i++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.Center.Y), npc.width, npc.height / 2, 14, 0f, -3f, 100, default, 3.5f * npc.scale);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 3.5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.Center.Y), npc.width, npc.height / 2, 14, 0f, -3f, 100, default, 2.5f * npc.scale);
                    Main.dust[num624].velocity *= 1f;
                }
            }
            else if (burrowTimer == -60)
            {
                npc.scale = 0.01f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.Center = player.Center;
                    npc.position.Y = player.position.Y - npc.height;
                    int tilePosX = (int)npc.Center.X / 16;
                    int tilePosY = (int)(npc.position.Y + npc.height) / 16 + 1;
                    if (Main.tile[tilePosX, tilePosY] == null)
                        Main.tile[tilePosX, tilePosY] = new Tile();
                    while (!(Main.tile[tilePosX, tilePosY].nactive() && Main.tileSolid[Main.tile[tilePosX, tilePosY].type]))
                    {
                        tilePosY++;
                        npc.position.Y += 16;
                        if (Main.tile[tilePosX, tilePosY] == null)
                            Main.tile[tilePosX, tilePosY] = new Tile();
                    }
                }
                npc.netUpdate = true;
            }
            else if (burrowTimer < 0)
            {
                npc.scale -= 0.0165f;
                npc.alpha += 4;
                int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.Center.Y), npc.width, npc.height / 2, 14, 0f, -3f, 100, default, 2.5f * npc.scale);
                Main.dust[num622].velocity *= 2f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
                for (int i = 0; i < 2; i++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.Center.Y), npc.width, npc.height / 2, 14, 0f, -3f, 100, default, 3.5f * npc.scale);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 3.5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.Center.Y), npc.width, npc.height / 2, 14, 0f, -3f, 100, default, 2.5f * npc.scale);
                    Main.dust[num624].velocity *= 1f;
                }
            }
            else if (burrowTimer == 0)
            {
                if (!player.active || player.dead)
                {
                    burrowTimer = 30;
                }
                else
                {
					npc.TargetClosest();
                    npc.dontTakeDamage = true;
                    npc.damage = 0;
                }
            }
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * npc.GetExpertDamageMultiplier());
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life > 0)
            {
                if (NPC.CountNPCS(NPCID.EaterofSouls) < 3 && NPC.CountNPCS(NPCID.DevourerHead) < 1)
                {
                    if (Main.rand.NextBool(60) && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 spawnAt = npc.Center + new Vector2(0f, npc.height / 2f);
                        NPC.NewNPC((int)spawnAt.X, (int)spawnAt.Y, NPCID.EaterofSouls);
                    }
                    if (Main.rand.NextBool(150) && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 spawnAt = npc.Center + new Vector2(0f, npc.height / 2f);
                        NPC.NewNPC((int)spawnAt.X, (int)spawnAt.Y, NPCID.DevourerHead);
                    }
                }
                int num285 = 0;
                while (num285 < damage / npc.lifeMax * 100.0)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 14, hitDirection, -1f, 0, default, 1f);
                    num285++;
                }
            }
            else
            {
                int goreAmount = 7;
                for (int i = 1; i <= goreAmount; i++)
                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/HiveMindGores/HiveMindGore" + i), 1f);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (NPC.CountNPCS(ModContent.NPCType<HiveMindP2>()) < 1)
                    {
                        NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<HiveMindP2>(), npc.whoAmI, 0f, 0f, 0f, 0f, npc.target);
                        Main.PlaySound(SoundID.Roar, (int)npc.Center.X, (int)npc.Center.Y, 0);
                    }
                }
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return npc.scale == 1f; //no damage when shrunk
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return npc.scale == 1f;
        }

        public override bool PreNPCLoot()
        {
            return false;
        }
    }
}
