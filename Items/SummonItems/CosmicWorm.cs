using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    public class CosmicWorm : ModItem, ILocalizedModType
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
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossItem;
        }

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<DevourerofGodsHead>()) && !BossRushEvent.BossRushActive;
        }

        public override bool? UseItem(Player player)
        {
            string key = "Mods.CalamityMod.Status.Boss.EdgyBossText7";
            Color messageColor = Color.Cyan;
            CalamityUtils.DisplayLocalizedText(key, messageColor);

            CalamityUtils.SpawnBossUsingItem<DevourerofGodsHead>(player, DevourerofGodsHead.SpawnSound);
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ArmoredShell>().
                AddIngredient<TwistingNether>().
                AddIngredient<DarkPlasma>().
                AddTile(TileID.LunarCraftingStation).
                Register()
                .DisableDecraft();

            // sequence breaking recipe
            CreateRecipe().
                AddIngredient(ItemID.LunarBar, 40).
                AddIngredient<GalacticaSingularity>(10).
                AddIngredient<Necroplasm>(40).
                AddTile(TileID.LunarCraftingStation).
                Register()
                .DisableDecraft();
        }
    }
}
