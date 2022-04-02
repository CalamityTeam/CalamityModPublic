using CalamityMod.Items.Accessories;
using CalamityMod.Items.Placeables.Banners;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.NormalNPCs
{
	public class Rotdog : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rotdog");
            Main.npcFrameCount[npc.type] = 10;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = 26;
            npc.damage = 18;
            npc.width = 46;
            npc.height = 30;
            npc.defense = 4;
            npc.lifeMax = 60;
            npc.knockBackResist = 0.3f;
            animationType = NPCID.Hellhound;
            aiType = NPCID.Wolf;
            npc.value = Item.buyPrice(0, 0, 2, 0);
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath5;
            banner = npc.type;
            bannerItem = ModContent.ItemType<PitbullBanner>();
			npc.Calamity().VulnerableToHeat = true;
			npc.Calamity().VulnerableToCold = true;
			npc.Calamity().VulnerableToSickness = true;
		}

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || !NPC.downedBoss1 || spawnInfo.player.Calamity().ZoneSulphur)
            {
                return 0f;
            }
            return SpawnCondition.OverworldNightMonster.Chance * 0.045f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Bleeding, 180, true);
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
            float bandageDropRate = Main.expertMode ? 0.02f : 0.01f;
            DropHelper.DropItemChance(npc, ItemID.AdhesiveBandage, bandageDropRate);
            DropHelper.DropItemChance(npc, ModContent.ItemType<RottenDogtooth>(), 8);
        }
    }
}
