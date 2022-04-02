using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Materials;
using CalamityMod.World;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.NormalNPCs
{
	public class AngryDog : ModNPC
    {
        private bool reset = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Angry Dog");
            Main.npcFrameCount[npc.type] = 9;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            npc.damage = 10;
            npc.width = 56;
            npc.height = 56;
            npc.defense = 4;
            npc.lifeMax = 50;
            if (CalamityWorld.downedCryogen)
            {
                npc.damage = 60;
                npc.defense = 10;
                npc.lifeMax = 1000;
            }
            npc.knockBackResist = 0.3f;
            animationType = NPCID.Hellhound;
            aiType = -1;
            npc.value = Item.buyPrice(0, 0, 3, 0);
            npc.HitSound = mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AngryDogHit");
            npc.DeathSound = SoundID.NPCDeath5;
            banner = npc.type;
            bannerItem = ModContent.ItemType<AngryDogBanner>();
			npc.coldDamage = true;
			npc.Calamity().VulnerableToHeat = true;
			npc.Calamity().VulnerableToCold = false;
			npc.Calamity().VulnerableToSickness = true;
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
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/AngryDogGrowl"), (int)npc.position.X, (int)npc.position.Y);
            }
            bool phase2 = (double)npc.life <= (double)npc.lifeMax * (CalamityWorld.death ? 0.9 : 0.5);
            if (phase2)
            {
                if (!reset)
                {
                    npc.ai[0] = 0f;
                    npc.ai[3] = 0f;
                    reset = true;
                    npc.netUpdate = true;
                }
                if (npc.ai[1] < 7f)
                {
                    npc.ai[1] += 1f;
                }
				CalamityAI.UnicornAI(npc, mod, true, CalamityWorld.death ? 6f : 4f, 5f, 0.2f);
                return;
            }
			CalamityAI.UnicornAI(npc, mod, false, CalamityWorld.death ? 6f : 4f, 6f, CalamityWorld.death ? 0.1f : 0.07f);
        }

        public override void FindFrame(int frameHeight)
        {
            if (npc.ai[1] < 7f && npc.ai[1] > 0f)
            {
                npc.frame.Y = frameHeight * 7;
                npc.frameCounter = 0.0;
                return;
            }
            if (npc.ai[1] >= 7f)
            {
                npc.frame.Y = frameHeight * 8;
                npc.frameCounter = 0.0;
                return;
            }
            if (npc.velocity.Y > 0f || npc.velocity.Y < 0f)
            {
                npc.frame.Y = frameHeight * 5;
                npc.frameCounter = 0.0;
            }
            else
            {
                npc.spriteDirection = npc.direction;
                npc.frameCounter += (double)(npc.velocity.Length() / 16f);
                if (npc.frameCounter > 12.0)
                {
                    npc.frame.Y = npc.frame.Y + frameHeight;
                    npc.frameCounter = 0.0;
                }
                if (npc.frame.Y >= frameHeight * 6)
                {
                    npc.frame.Y = 0;
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.player.ZoneSnow &&
                !spawnInfo.player.PillarZone() &&
                !spawnInfo.player.ZoneDungeon &&
                !spawnInfo.player.InSunkenSea() &&
                !spawnInfo.playerInTown && !spawnInfo.player.ZoneOldOneArmy && !Main.snowMoon && !Main.pumpkinMoon ? 0.012f : 0f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Frostburn, 120, true);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(npc, ItemID.Leather, 1, 1, 2);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<EssenceofEleum>(), CalamityWorld.downedCryogen, 3, 1, 1);
        }
    }
}
