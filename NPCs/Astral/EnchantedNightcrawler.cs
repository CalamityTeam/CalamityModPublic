using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace CalamityMod.NPCs.Astral
{
    public class EnchantedNightcrawler : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Enchanted Nightcrawler");
            Main.npcFrameCount[NPC.type] = 2;
            Main.npcCatchable[NPC.type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers bestiaryData = new NPCID.Sets.NPCBestiaryDrawModifiers(0){ Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, bestiaryData);
            }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.EnchantedNightcrawler); //ID is 484
            AIType = NPCID.EnchantedNightcrawler;
            AnimationType = NPCID.EnchantedNightcrawler;
        }

        public override bool PreAI()
        {
            NPC.type = NPCID.EnchantedNightcrawler; //Make it immediately turn into vanilla critter
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
            if (CalamityGlobalNPC.AnyEvents(spawnInfo.Player))
            {
                return 0f;
            }
            else if (spawnInfo.Player.InAstral())
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
