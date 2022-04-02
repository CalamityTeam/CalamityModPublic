using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.NormalNPCs
{
    public class EbonianBlightSlime : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ebonian Blight Slime");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = 1;
            aiType = NPCID.DungeonSlime;
            npc.damage = 30;
            npc.width = 60;
            npc.height = 42;
            npc.defense = 8;
            npc.lifeMax = 130;
            npc.knockBackResist = 0.3f;
            animationType = NPCID.RainbowSlime;
            npc.value = Item.buyPrice(0, 0, 2, 0);
            npc.alpha = 105;
            npc.lavaImmune = false;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            banner = npc.type;
            bannerItem = ModContent.ItemType<EbonianBlightSlimeBanner>();
            npc.Calamity().VulnerableToHeat = true;
            npc.Calamity().VulnerableToSickness = false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 14, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 40; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 14, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || spawnInfo.player.Calamity().ZoneAbyss)
            {
                return 0f;
            }
            return SpawnCondition.Corruption.Chance * 0.15f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Weak, 120, true);
        }

        public override void NPCLoot()
        {
            DropHelper.DropItem(npc, ModContent.ItemType<EbonianGel>(), 15, 20);
            DropHelper.DropItem(npc, ItemID.Gel, 10, 14);
        }
    }
}
