using CalamityMod.Events;
using CalamityMod.NPCs.Providence;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    [LegacyName("ProfanedCoreUnlimited")]
    public class ProfanedCore : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.SummonItems";
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 19; // Celestial Sigil
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
            Item.rare = ItemRarityID.Purple;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossItem;
        }

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<Providence>()) && (player.ZoneHallow || player.ZoneUnderworldHeight) && !BossRushEvent.BossRushActive;
        }

        public override bool? UseItem(Player player)
        {
            int posX = (int)(player.position.X + Main.rand.Next(-500, 501));
            int posY = (int)(player.position.Y - 250f);
            int bossToSpawn = ModContent.NPCType<Providence>();
            CalamityUtils.SpawnBossOnPosUsingItem(player, bossToSpawn, posX, posY, Providence.SpawnSound);
            return true;
        }
    }
}
