using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.HiveMind
{
    public class HiveCyst : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hive Cyst");
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.npcSlots = 0f;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.damage = 0;
            NPC.width = 30; //324
            NPC.height = 30; //216
            NPC.defense = 0;
            NPC.lifeMax = 1000;
            NPC.knockBackResist = 0f;
            NPC.chaseable = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.rarity = 2;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().ProvidesProximityRage = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundCorruption,

				// Will move to localization whenever that is cleaned up.
				new FlavorTextBestiaryInfoElement("The blemish of a colonial superorganism, the mass of organic matter pulses like a heart. The growth is the result of the corruption’s beings forming together.")
            });
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            //TODO -- Zenith seed.
            bool getFuckedAI = Main.getGoodWorld && Main.masterMode;
            float timeToSpawn = 120f;

            if (getFuckedAI && NPC.AnyNPCs(ModContent.NPCType<HiveMind>()))
            {
                //Passively spawns random enemies
                NPC.ai[0]++;
                
                if (NPC.ai[0] >= timeToSpawn)
                {
                    int spawnRandomizer = Main.rand.Next(0, 5);
                    int type = NPCID.EaterofSouls;

                    switch (spawnRandomizer)
                    {
                        case 0:
                        case 1:
                            break;
                        case 2:
                            type = NPCID.DevourerHead;
                            break;
                        case 3:
                            type = ModContent.NPCType<DankCreeper>();
                            break;
                        case 4:
                            type = ModContent.NPCType<HiveBlob2>();
                            break;
                        default:
                            break;
                    }
                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, type);
                    NPC.ai[0] = 0f;
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (CalamityGlobalNPC.AnyEvents(spawnInfo.Player))
                return 0f;
            if (spawnInfo.Player.Calamity().disableHiveCystSpawns)
                return 0f;

            bool anyBossElements = NPC.AnyNPCs(ModContent.NPCType<HiveCyst>()) || NPC.AnyNPCs(ModContent.NPCType<HiveMind>());
            bool corrupt = TileID.Sets.Corrupt[spawnInfo.SpawnTileType] || spawnInfo.SpawnTileType == TileID.Demonite && spawnInfo.Player.ZoneCorrupt;
            if (anyBossElements || spawnInfo.PlayerSafe || !corrupt)
                return 0f;

            if (NPC.downedBoss2 && !DownedBossSystem.downedHiveMind)
                return 1.5f;

            return Main.hardMode ? 0.05f : 0.5f;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = 2000;
            NPC.damage = 0;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 14, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 14, hitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.MultiplayerClient && NPC.CountNPCS(ModContent.NPCType<HiveMind>()) < 1)
                {
                    Vector2 spawnAt = NPC.Bottom;
                    NPC.NewNPC(NPC.GetSource_Death(), (int)spawnAt.X, (int)spawnAt.Y, ModContent.NPCType<HiveMind>());
                }
            }
        }
    }
}
