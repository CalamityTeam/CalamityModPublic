using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Materials;
using CalamityMod.World;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.NPCs.NormalNPCs
{
    public class AngryDog : ModNPC
    {
        private bool reset = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Angry Dog");
            Main.npcFrameCount[NPC.type] = 9;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.damage = 10;
            NPC.width = 56;
            NPC.height = 56;
            NPC.defense = 4;
            NPC.lifeMax = 50;
            if (DownedBossSystem.downedCryogen)
            {
                NPC.damage = 60;
                NPC.defense = 10;
                NPC.lifeMax = 1000;
            }
            NPC.knockBackResist = 0.3f;
            animationType = NPCID.Hellhound;
            AIType = -1;
            NPC.value = Item.buyPrice(0, 0, 3, 0);
            NPC.HitSound = Mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AngryDogHit");
            NPC.DeathSound = SoundID.NPCDeath5;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<AngryDogBanner>();
            NPC.coldDamage = true;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = false;
            NPC.Calamity().VulnerableToSickness = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(reset);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            reset = reader.ReadBoolean();
        }

        public override void AI()
        {
            if (Main.rand.NextBool(900))
            {
                SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/AngryDogGrowl"), (int)NPC.position.X, (int)NPC.position.Y);
            }
            bool phase2 = (double)NPC.life <= (double)NPC.lifeMax * (CalamityWorld.death ? 0.9 : 0.5);
            if (phase2)
            {
                if (!reset)
                {
                    NPC.ai[0] = 0f;
                    NPC.ai[3] = 0f;
                    reset = true;
                    NPC.netUpdate = true;
                }
                if (NPC.ai[1] < 7f)
                {
                    NPC.ai[1] += 1f;
                }
                CalamityAI.UnicornAI(NPC, Mod, true, CalamityWorld.death ? 6f : 4f, 5f, 0.2f);
                return;
            }
            CalamityAI.UnicornAI(NPC, Mod, false, CalamityWorld.death ? 6f : 4f, 6f, CalamityWorld.death ? 0.1f : 0.07f);
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.ai[1] < 7f && NPC.ai[1] > 0f)
            {
                NPC.frame.Y = frameHeight * 7;
                NPC.frameCounter = 0.0;
                return;
            }
            if (NPC.ai[1] >= 7f)
            {
                NPC.frame.Y = frameHeight * 8;
                NPC.frameCounter = 0.0;
                return;
            }
            if (NPC.velocity.Y > 0f || NPC.velocity.Y < 0f)
            {
                NPC.frame.Y = frameHeight * 5;
                NPC.frameCounter = 0.0;
            }
            else
            {
                NPC.spriteDirection = NPC.direction;
                NPC.frameCounter += (double)(NPC.velocity.Length() / 16f);
                if (NPC.frameCounter > 12.0)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y >= frameHeight * 6)
                {
                    NPC.frame.Y = 0;
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.ZoneSnow &&
                !spawnInfo.Player.PillarZone() &&
                !spawnInfo.Player.ZoneDungeon &&
                !spawnInfo.Player.InSunkenSea() &&
                !spawnInfo.playerInTown && !spawnInfo.Player.ZoneOldOneArmy && !Main.snowMoon && !Main.pumpkinMoon ? 0.012f : 0f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Frostburn, 120, true);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemID.Leather, 1, 1, 2); // 100% chance of 1-2 leather
            npcLoot.AddIf(() => DownedBossSystem.downedCryogen, ModContent.ItemType<EssenceofEleum>(), 3);
        }
    }
}
