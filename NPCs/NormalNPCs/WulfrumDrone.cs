using CalamityMod.Items.Materials;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Placeables.Banners;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.NormalNPCs
{
    public class WulfrumDrone : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Drone");
            Main.npcFrameCount[npc.type] = 5;
        }

        public override void SetDefaults()
        {
            npc.damage = 10;
            npc.aiStyle = 3;
            aiType = 73;
            npc.width = 40;
            npc.height = 30;
            npc.defense = 4;
            npc.lifeMax = 22;
            npc.knockBackResist = 0.35f;
            npc.value = Item.buyPrice(0, 0, 0, 50);
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            banner = npc.type;
            bannerItem = ModContent.ItemType<WulfrumDroneBanner>();
        }

        public override void PostAI()
        {
            npc.spriteDirection = (npc.direction > 0) ? 1 : -1;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || spawnInfo.player.Calamity().ZoneSulphur)
            {
                return 0f;
            }
            return SpawnCondition.OverworldDaySlime.Chance * (Main.hardMode ? 0.05f : 0.2f);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.GrassBlades, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.GrassBlades, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void NPCLoot()
        {
            DropHelper.DropItem(npc, ModContent.ItemType<WulfrumShard>(), 1, 3);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<WulfrumShard>(), Main.expertMode, 1, 1);
            DropHelper.DropItemChance(npc, ModContent.ItemType<WulfrumBattery>(), 10);
        }
    }
}
