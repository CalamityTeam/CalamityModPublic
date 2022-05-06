using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
namespace CalamityMod.NPCs.NormalNPCs
{
    public class StormlionCharger : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stormlion");
            Main.npcFrameCount[NPC.type] = 6;
        }

        public override void SetDefaults()
        {
            NPC.damage = 20;
            NPC.aiStyle = 3;
            NPC.width = 33;
            NPC.height = 31;
            NPC.defense = 8;
            NPC.lifeMax = 80;
            NPC.knockBackResist = 0.2f;
            AnimationType = NPCID.WalkingAntlion;
            NPC.value = Item.buyPrice(0, 0, 2, 0);
            NPC.HitSound = SoundID.NPCHit31;
            NPC.DeathSound = SoundID.NPCDeath34;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<StormlionBanner>();
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().VulnerableToElectricity = false;
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
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Stormlion").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Stormlion2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Stormlion3").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Stormlion4").Type, NPC.scale);
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe || spawnInfo.Player.Calamity().ZoneSunkenSea)
            {
                return 0f;
            }
            return SpawnCondition.DesertCave.Chance * 0.3f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Electrified, 120, true);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ModContent.ItemType<StormlionMandible>());
            npcLoot.Add(ModContent.ItemType<StormjawStaff>(), 5);
        }
    }
}
