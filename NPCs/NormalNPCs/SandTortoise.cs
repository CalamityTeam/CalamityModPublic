using CalamityMod.Items.Placeables.Banners;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.NormalNPCs
{
    public class SandTortoise : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sand Tortoise");
            Main.npcFrameCount[NPC.type] = 8;
        }

        public override void SetDefaults()
        {
            NPC.npcSlots = 2f;
            NPC.damage = 70;
            NPC.aiStyle = 39;
            NPC.width = 46;
            NPC.height = 32;
            NPC.defense = 30;
            NPC.DR_NERD(0.25f);
            NPC.scale = 1.5f;
            NPC.lifeMax = 580;
            NPC.knockBackResist = 0.2f;
            animationType = NPCID.GiantTortoise;
            NPC.value = Item.buyPrice(0, 0, 15, 0);
            NPC.HitSound = SoundID.NPCHit24;
            NPC.DeathSound = SoundID.NPCDeath27;
            NPC.noGravity = false;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<SandTortoiseBanner>();
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().VulnerableToWater = true;
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

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe || !Main.hardMode || spawnInfo.Player.Calamity().ZoneSunkenSea)
            {
                return 0f;
            }
            return SpawnCondition.DesertCave.Chance * 0.05f;
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(NPC, ItemID.TurtleShell, 0.1f);
        }
    }
}
