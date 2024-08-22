using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.Bumblebirb;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    [LegacyName("BirbPheromones")]
    public class ExoticPheromones : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.SummonItems";
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 19; // Celestial Sigil
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.rare = ItemRarityID.Red;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossItem;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ZoneJungle && !NPC.AnyNPCs(ModContent.NPCType<Bumblefuck>()) && !BossRushEvent.BossRushActive;
        }

        public override bool? UseItem(Player player)
        {
            CalamityUtils.SpawnBossUsingItem<Bumblefuck>(player, SoundID.Roar);
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<LifeAlloy>(5).
                AddIngredient(ItemID.FragmentSolar, 20).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
