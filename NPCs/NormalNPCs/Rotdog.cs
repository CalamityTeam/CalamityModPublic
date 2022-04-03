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
            Main.npcFrameCount[NPC.type] = 10;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = 26;
            NPC.damage = 18;
            NPC.width = 46;
            NPC.height = 30;
            NPC.defense = 4;
            NPC.lifeMax = 60;
            NPC.knockBackResist = 0.3f;
            animationType = NPCID.Hellhound;
            AIType = NPCID.Wolf;
            NPC.value = Item.buyPrice(0, 0, 2, 0);
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath5;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<PitbullBanner>();
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || !NPC.downedBoss1 || spawnInfo.Player.Calamity().ZoneSulphur)
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

        public override void NPCLoot()
        {
            float bandageDropRate = Main.expertMode ? 0.02f : 0.01f;
            DropHelper.DropItemChance(NPC, ItemID.AdhesiveBandage, bandageDropRate);
            DropHelper.DropItemChance(NPC, ModContent.ItemType<RottenDogtooth>(), 8);
        }
    }
}
