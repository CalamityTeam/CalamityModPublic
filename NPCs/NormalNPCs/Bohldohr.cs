using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.NormalNPCs
{
    public class Bohldohr : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bohldohr");
        }

        public override void SetDefaults()
        {
            npc.Calamity().canBreakPlayerDefense = true;
            npc.aiStyle = -1;
            aiType = -1;
            npc.damage = 80;
            npc.width = 40;
            npc.height = 40;
            npc.defense = 18;
            npc.lifeMax = 300;
            npc.knockBackResist = 0.95f;
            npc.value = Item.buyPrice(0, 0, 10, 0);
            npc.HitSound = SoundID.NPCHit7;
            npc.DeathSound = SoundID.NPCDeath35;
            npc.behindTiles = true;
            banner = npc.type;
            bannerItem = ModContent.ItemType<BOHLDOHRBanner>();
            npc.Calamity().VulnerableToSickness = false;
            npc.Calamity().VulnerableToWater = true;
        }

        public override void AI()
        {
            CalamityAI.UnicornAI(npc, mod, true, CalamityWorld.death ? 6f : 4f, 5f, 0.2f);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe)
            {
                return 0f;
            }
            return SpawnCondition.JungleTemple.Chance * 0.1f;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 155, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 155, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void NPCLoot()
        {
            if (CalamityWorld.downedSCal)
            {
                // RIP LORDE
                // DropHelper.DropItem(npc, ModContent.ItemType<NO>());
            }
            DropHelper.DropItem(npc, ItemID.LihzahrdBrick, 10, 30);
            DropHelper.DropItemChance(npc, ItemID.LunarTabletFragment, 7, 1, 3); //solar tablet fragment
            DropHelper.DropItemChance(npc, ItemID.LihzahrdPowerCell, 50);
        }
    }
}
