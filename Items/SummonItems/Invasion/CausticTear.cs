using CalamityMod.Events;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems.Invasion
{
    public class CausticTear : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.SummonItems";
        public override void SetStaticDefaults()
        {
           	ItemID.Sets.SortingPriorityBossSpawns[Type] = 4; // Goblin Battle Standard
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.maxStack = 1;
            Item.rare = ItemRarityID.Green;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.EventItem;
		}

        public override bool CanUseItem(Player player)
        {
            return !AcidRainEvent.AcidRainEventIsOngoing;
        }

        public override bool? UseItem(Player player)
        {
            CalamityNetcode.SyncWorld();
            AcidRainEvent.TryStartEvent(true);
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SulphuricScale>(5).
                AddCondition(Condition.NearWater).
                Register();
        }
    }
}
