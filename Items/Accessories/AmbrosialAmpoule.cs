using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class AmbrosialAmpoule : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ambrosial Ampoule");
            Tooltip.SetDefault("You emit light\n" +
                "7% increased damage reduction and increased life regen\n" +
                "Freeze, chill and frostburn immunity");
        }

        public override void SetDefaults()
        {
            Item.defense = 6;
            Item.width = 20;
            Item.height = 20;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.aAmpoule = true;
            modPlayer.rOoze = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<CorruptFlask>()).AddIngredient(ModContent.ItemType<RadiantOoze>()).AddIngredient(ModContent.ItemType<VerstaltiteBar>(), 5).AddIngredient(ModContent.ItemType<SeaPrism>(), 10).AddTile(TileID.MythrilAnvil).Register();
            CreateRecipe(1).AddIngredient(ModContent.ItemType<CrimsonFlask>()).AddIngredient(ModContent.ItemType<RadiantOoze>()).AddIngredient(ModContent.ItemType<VerstaltiteBar>(), 5).AddIngredient(ModContent.ItemType<SeaPrism>(), 10).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
