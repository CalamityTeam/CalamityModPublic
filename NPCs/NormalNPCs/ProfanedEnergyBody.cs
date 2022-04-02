using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.NormalNPCs
{
    public class ProfanedEnergyBody : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Profaned Energy");
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            npc.damage = 0;
            npc.npcSlots = 3f;
            npc.width = 72;
            npc.height = 36;
            npc.defense = 50;
            npc.DR_NERD(0.1f);
            npc.lifeMax = 4500;
            npc.knockBackResist = 0f;
            aiType = -1;
            npc.value = Item.buyPrice(0, 0, 50, 0);
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.HitSound = SoundID.NPCHit52;
            npc.DeathSound = SoundID.NPCDeath55;
            banner = npc.type;
            bannerItem = ModContent.ItemType<ProfanedEnergyBanner>();
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToCold = true;
            npc.Calamity().VulnerableToSickness = false;
            npc.Calamity().VulnerableToWater = true;
        }

        public override void AI()
        {
            CalamityGlobalNPC.energyFlame = npc.whoAmI;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (npc.localAI[0] == 0f)
                {
                    npc.localAI[0] = 1f;
                    for (int num723 = 0; num723 < 2; num723++)
                    {
                        NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<ProfanedEnergyLantern>(), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                    }
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || !NPC.downedMoonlord || NPC.AnyNPCs(npc.type))
            {
                return 0f;
            }
            if (SpawnCondition.Underworld.Chance > 0f)
            {
                return SpawnCondition.Underworld.Chance / 6f;
            }
            return SpawnCondition.OverworldHallow.Chance / 6f;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.ProfanedFire, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 50; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.ProfanedFire, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void NPCLoot()
        {
            DropHelper.DropItem(npc, ModContent.ItemType<UnholyEssence>(), 2, 4);
        }
    }
}
