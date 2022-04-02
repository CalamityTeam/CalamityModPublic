using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.Banners;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.SunkenSea
{
    public class Clam : ModNPC
    {
        private int hitAmount = 0;
        private bool hasBeenHit = false;
        private bool statChange = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Clam");
            Main.npcFrameCount[npc.type] = 5;
        }

        public override void SetDefaults()
        {
            npc.damage = 30;
            npc.width = 56;
            npc.height = 38;
            npc.defense = 9999;
            npc.DR_NERD(0.25f);
            npc.lifeMax = Main.hardMode ? 300 : 150;
            if (Main.expertMode)
            {
                npc.lifeMax *= 2;
            }
            npc.aiStyle = -1;
            aiType = -1;
            npc.value = Main.hardMode ? Item.buyPrice(0, 0, 10, 0) : Item.buyPrice(0, 0, 1, 0);
            npc.HitSound = SoundID.NPCHit4;
            npc.knockBackResist = 0.05f;
            banner = npc.type;
            bannerItem = ModContent.ItemType<ClamBanner>();
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToSickness = true;
            npc.Calamity().VulnerableToElectricity = true;
            npc.Calamity().VulnerableToWater = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(hitAmount);
            writer.Write(npc.chaseable);
            writer.Write(hasBeenHit);
            writer.Write(statChange);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            hitAmount = reader.ReadInt32();
            npc.chaseable = reader.ReadBoolean();
            hasBeenHit = reader.ReadBoolean();
            statChange = reader.ReadBoolean();
        }

        public override void AI()
        {
            npc.TargetClosest(true);
            if (Main.player[npc.target].Calamity().clamity)
            {
                hitAmount = 3;
                hasBeenHit = true;
            }
            if (npc.justHit && hitAmount < 3)
            {
                ++hitAmount;
                hasBeenHit = true;
            }
            npc.chaseable = hasBeenHit;
            if (hitAmount == 3)
            {
                if (!statChange)
                {
                    npc.buffImmune[BuffID.Confused] = false;
                    npc.defense = 6;
                    npc.damage = Main.expertMode ? 60 : 30;
                    if (Main.hardMode)
                    {
                        npc.defense = 15;
                        npc.damage = Main.expertMode ? 120 : 60;
                    }
                    statChange = true;
                }
                if (npc.ai[0] == 0f)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (npc.velocity.X != 0f || npc.velocity.Y < 0f || (double)npc.velocity.Y > 0.9)
                        {
                            npc.ai[0] = 1f;
                            npc.netUpdate = true;
                            return;
                        }
                        npc.ai[0] = 1f;
                        npc.netUpdate = true;
                        return;
                    }
                }
                else if (npc.velocity.Y == 0f)
                {
                    npc.ai[2] += 1f;
                    int num321 = 20;
                    if (npc.ai[1] == 0f)
                    {
                        num321 = 12;
                    }
                    if (npc.ai[2] < (float)num321)
                    {
                        npc.velocity.X *= 0.9f;
                        return;
                    }
                    npc.ai[2] = 0f;
                    npc.TargetClosest(true);
                    if (npc.direction == 0)
                    {
                        npc.direction = -1;
                    }
                    npc.spriteDirection = -npc.direction;
                    npc.ai[1] += 1f;
                    npc.ai[3] += 1f;
                    if (npc.ai[3] >= 4f)
                    {
                        npc.ai[3] = 0f;
                        if (npc.ai[1] == 2f)
                        {
                            float multiplierX = (float)Main.rand.Next(3, 7);
                            npc.velocity.X = (float)npc.direction * multiplierX;
                            npc.velocity.Y = -8f;
                            npc.ai[1] = 0f;
                        }
                        else
                        {
                            float multiplierX = (float)Main.rand.Next(5, 9);
                            npc.velocity.X = (float)npc.direction * multiplierX;
                            npc.velocity.Y = -4f;
                        }
                    }
                    npc.netUpdate = true;
                    return;
                }
                else
                {
                    if (npc.direction == 1 && npc.velocity.X < 1f)
                    {
                        npc.velocity.X = npc.velocity.X + 0.1f;
                        return;
                    }
                    if (npc.direction == -1 && npc.velocity.X > -1f)
                    {
                        npc.velocity.X = npc.velocity.X - 0.1f;
                        return;
                    }
                }
            }
            else
            {
                npc.damage = 0;
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
            npc.frameCounter += 1.0;
            if (npc.frameCounter > 4.0)
            {
                npc.frameCounter = 0.0;
                npc.frame.Y = npc.frame.Y + frameHeight;
            }
            if (hitAmount < 3)
            {
                npc.frame.Y = frameHeight * 4;
            }
            else
            {
                if (npc.frame.Y > frameHeight * 3)
                {
                    npc.frame.Y = 0;
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.player.Calamity().ZoneSunkenSea && spawnInfo.water)
            {
                return SpawnCondition.CaveJellyfish.Chance * 1.2f;
            }
            return 0f;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 37, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 50; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 37, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Clam/Clam1"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Clam/Clam2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Clam/Clam3"), 1f);
            }
        }
        public override void NPCLoot()
        {
            DropHelper.DropItem(npc, ModContent.ItemType<Navystone>(), 8, 12);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<MolluskHusk>(), Main.hardMode, 0.5f);
        }
    }
}
