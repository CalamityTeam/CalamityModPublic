using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Rogue;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

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
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.damage = 20;
            NPC.width = 40;
            NPC.height = 40;
            NPC.defense = 10;
            NPC.lifeMax = 30;
            NPC.knockBackResist = 0.8f;
            NPC.value = Item.buyPrice(0, 0, 0, 80);
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath15;
            NPC.behindTiles = true;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<SeaUrchinBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
        }

        public override void AI()
        {
            CalamityAI.UnicornAI(NPC, Mod, true, NPC.wet ? 9f : 4f, NPC.wet ? 5.5f : 2.2f, NPC.wet ? 0.09f : 0.04f, NPC.wet ? -14f : -6.5f, NPC.wet ? -12f : -6f, NPC.wet ? -11f : -5f, NPC.wet ? -10f : -4f, NPC.wet ? -13f : -6f);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Venom, 120, true);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe || spawnInfo.Player.Calamity().ZoneSulphur)
            {
                return 0f;
            }
            return SpawnCondition.OceanMonster.Chance * 0.2f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.Add(ModContent.ItemType<UrchinStinger>(), 1, 15, 25);

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
    }
}
