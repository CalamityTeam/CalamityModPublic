using CalamityMod.BiomeManagers;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.Banners;
using System.IO;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
namespace CalamityMod.NPCs.SunkenSea
{
    public class Clam : ModNPC
    {
        private int hitAmount = 0;
        private bool hasBeenHit = false;
        private bool statChange = false;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                SpriteDirection = 1
            };
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.damage = 30;
            NPC.width = 56;
            NPC.height = 38;
            NPC.defense = 9999;
            NPC.DR_NERD(0.25f);
            NPC.lifeMax = Main.hardMode ? 300 : 150;
            if (Main.expertMode)
            {
                NPC.lifeMax *= 2;
            }
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.value = Main.hardMode ? Item.buyPrice(0, 0, 10, 0) : Item.buyPrice(0, 0, 1, 0);
            NPC.HitSound = SoundID.NPCHit4;
            NPC.knockBackResist = 0.05f;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<ClamBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<SunkenSeaBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Clam")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(hitAmount);
            writer.Write(NPC.chaseable);
            writer.Write(hasBeenHit);
            writer.Write(statChange);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            hitAmount = reader.ReadInt32();
            NPC.chaseable = reader.ReadBoolean();
            hasBeenHit = reader.ReadBoolean();
            statChange = reader.ReadBoolean();
        }

        public override void AI()
        {
            NPC.TargetClosest(true);
            if (Main.player[NPC.target].Calamity().clamity)
            {
                hitAmount = 3;
                hasBeenHit = true;
            }
            if (NPC.justHit && hitAmount < 3)
            {
                ++hitAmount;
                hasBeenHit = true;
            }
            NPC.chaseable = hasBeenHit;
            if (hitAmount == 3)
            {
                if (!statChange)
                {
                    NPC.defense = 6;
                    NPC.damage = Main.expertMode ? 60 : 30;
                    if (Main.hardMode)
                    {
                        NPC.defense = 15;
                        NPC.damage = Main.expertMode ? 120 : 60;
                    }
                    statChange = true;
                }
                if (NPC.ai[0] == 0f)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (NPC.velocity.X != 0f || NPC.velocity.Y < 0f || (double)NPC.velocity.Y > 0.9)
                        {
                            NPC.ai[0] = 1f;
                            NPC.netUpdate = true;
                            return;
                        }
                        NPC.ai[0] = 1f;
                        NPC.netUpdate = true;
                        return;
                    }
                }
                else if (NPC.velocity.Y == 0f)
                {
                    NPC.ai[2] += 1f;
                    int decelerationTimer = 20;
                    if (NPC.ai[1] == 0f)
                    {
                        decelerationTimer = 12;
                    }
                    if (NPC.ai[2] < (float)decelerationTimer)
                    {
                        NPC.velocity.X *= 0.9f;
                        return;
                    }
                    NPC.ai[2] = 0f;
                    NPC.TargetClosest(true);
                    if (NPC.direction == 0)
                    {
                        NPC.direction = -1;
                    }
                    NPC.spriteDirection = -NPC.direction;
                    NPC.ai[1] += 1f;
                    NPC.ai[3] += 1f;
                    if (NPC.ai[3] >= 4f)
                    {
                        NPC.ai[3] = 0f;
                        if (NPC.ai[1] == 2f)
                        {
                            float multiplierX = (float)Main.rand.Next(3, 7);
                            NPC.velocity.X = (float)NPC.direction * multiplierX;
                            NPC.velocity.Y = -8f;
                            NPC.ai[1] = 0f;
                        }
                        else
                        {
                            float multiplierX = (float)Main.rand.Next(5, 9);
                            NPC.velocity.X = (float)NPC.direction * multiplierX;
                            NPC.velocity.Y = -4f;
                        }
                    }
                    NPC.netUpdate = true;
                    return;
                }
                else
                {
                    if (NPC.direction == 1 && NPC.velocity.X < 1f)
                    {
                        NPC.velocity.X = NPC.velocity.X + 0.1f;
                        return;
                    }
                    if (NPC.direction == -1 && NPC.velocity.X > -1f)
                    {
                        NPC.velocity.X = NPC.velocity.X - 0.1f;
                        return;
                    }
                }
            }
            else
            {
                NPC.damage = 0;
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
            NPC.frameCounter += 1.0;
            if (NPC.frameCounter > 4.0)
            {
                NPC.frameCounter = 0.0;
                NPC.frame.Y = NPC.frame.Y + frameHeight;
            }
            if (hitAmount < 3)
            {
                NPC.frame.Y = frameHeight * 4;
            }
            else
            {
                if (NPC.frame.Y > frameHeight * 3)
                {
                    NPC.frame.Y = 0;
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.Calamity().ZoneSunkenSea && spawnInfo.Water)
            {
                return SpawnCondition.CaveJellyfish.Chance * 1.2f;
            }
            return 0f;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 37, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 50; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 37, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Clam1").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Clam2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Clam3").Type, 1f);
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ModContent.ItemType<Navystone>(), 1, 8, 12);
            npcLoot.Add(ItemID.WhitePearl, 8);
            npcLoot.Add(ItemID.BlackPearl, 16);
            npcLoot.Add(ItemID.PinkPearl, 40);
            npcLoot.AddIf(() => Main.hardMode, ModContent.ItemType<MolluskHusk>(), 2);
        }
    }
}
