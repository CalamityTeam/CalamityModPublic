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
            Tooltip.SetDefault("25% increased mining speed\n" +
                "You emit light\n" +
                "5% increased damage reduction and increased life regen\n" +
                "Poison, Freeze, Chill, Frostburn, and Venom immunity\n" +
                "Honey-like life regen with no speed penalty\n" +
                "Most bee/hornet enemies and projectiles do 75% damage to you");
        }

        public override void SetDefaults()
        {
            item.defense = 4;
            item.width = 20;
            item.height = 20;
            item.value = Item.buyPrice(0, 45, 0, 0);
            item.rare = 10;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.beeResist = true;
            modPlayer.aAmpoule = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "CorruptFlask");
            recipe.AddIngredient(null, "ArchaicPowder");
            recipe.AddIngredient(null, "RadiantOoze");
            recipe.AddIngredient(null, "HoneyDew");
            recipe.AddIngredient(null, "Stardust", 15);
            recipe.AddIngredient(null, "CryoBar", 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "CrimsonFlask");
            recipe.AddIngredient(null, "ArchaicPowder");
            recipe.AddIngredient(null, "RadiantOoze");
            recipe.AddIngredient(null, "HoneyDew");
            recipe.AddIngredient(null, "Stardust", 15);
            recipe.AddIngredient(null, "CryoBar", 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
