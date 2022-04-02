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
            Main.npcFrameCount[npc.type] = 8;
        }

        public override void SetDefaults()
        {
            npc.npcSlots = 2f;
            npc.damage = 70;
            npc.aiStyle = 39;
            npc.width = 46;
            npc.height = 32;
            npc.defense = 30;
            npc.DR_NERD(0.25f);
            npc.scale = 1.5f;
            npc.lifeMax = 580;
            npc.knockBackResist = 0.2f;
            animationType = NPCID.GiantTortoise;
            npc.value = Item.buyPrice(0, 0, 15, 0);
            npc.HitSound = SoundID.NPCHit24;
            npc.DeathSound = SoundID.NPCDeath27;
            npc.noGravity = false;
            banner = npc.type;
            bannerItem = ModContent.ItemType<SandTortoiseBanner>();
            npc.Calamity().VulnerableToCold = true;
            npc.Calamity().VulnerableToSickness = true;
            npc.Calamity().VulnerableToWater = true;
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

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || !Main.hardMode || spawnInfo.player.Calamity().ZoneSunkenSea)
            {
                return 0f;
            }
            return SpawnCondition.DesertCave.Chance * 0.05f;
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(npc, ItemID.TurtleShell, 0.1f);
        }
    }
}
