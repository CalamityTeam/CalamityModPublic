using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Astral
{
    public class EnchantedNightcrawler : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Enchanted Nightcrawler");
            Main.npcFrameCount[npc.type] = 2;
            Main.npcCatchable[npc.type] = true;
        }

        public override void SetDefaults()
        {
            npc.CloneDefaults(NPCID.EnchantedNightcrawler); //ID is 484
            aiType = NPCID.EnchantedNightcrawler;
            animationType = NPCID.EnchantedNightcrawler;
        }

        public override bool PreAI()
        {
            npc.type = NPCID.EnchantedNightcrawler; //Make it immediately turn into vanilla critter
            return true;
        }

        public override bool? CanBeHitByItem(Player player, Item item)
        {
            return true;
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            return true;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (CalamityGlobalNPC.AnyEvents(spawnInfo.player))
            {
                return 0f;
            }
            else if (spawnInfo.player.InAstral())
            {
                return SpawnCondition.TownCritter.Chance;
            }
            return 0f;
        }

        public override void OnCatchNPC(Player player, Item item)
        {
            try
            {
            } catch
            {
                return;
            }
        }
    }
}
