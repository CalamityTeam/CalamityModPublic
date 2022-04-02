using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Rogue;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class SeaUrchin : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sea Urchin");
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            aiType = -1;
            npc.damage = 20;
            npc.width = 40;
            npc.height = 40;
            npc.defense = 10;
            npc.lifeMax = 30;
            npc.knockBackResist = 0.8f;
            npc.value = Item.buyPrice(0, 0, 0, 80);
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath15;
            npc.behindTiles = true;
            banner = npc.type;
            bannerItem = ModContent.ItemType<SeaUrchinBanner>();
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToSickness = false;
            npc.Calamity().VulnerableToElectricity = true;
            npc.Calamity().VulnerableToWater = false;
        }

        public override void AI()
        {
            CalamityAI.UnicornAI(npc, mod, true, npc.wet ? 9f : 4f, npc.wet ? 5.5f : 2.2f, npc.wet ? 0.09f : 0.04f, npc.wet ? -14f : -6.5f, npc.wet ? -12f : -6f, npc.wet ? -11f : -5f, npc.wet ? -10f : -4f, npc.wet ? -13f : -6f);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Venom, 120, true);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || spawnInfo.player.Calamity().ZoneSulphur)
            {
                return 0f;
            }
            return SpawnCondition.OceanMonster.Chance * 0.2f;
        }

        public override void NPCLoot()
        {
            DropHelper.DropItem(npc, ModContent.ItemType<UrchinStinger>(), 15, 25);
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
    }
}
