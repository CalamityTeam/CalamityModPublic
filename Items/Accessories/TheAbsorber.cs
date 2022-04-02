using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class TheAbsorber : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Absorber");
            Tooltip.SetDefault("5% increased movement and jump speed\n" +
                "+20 max life and mana\n" +
                "Standing still boosts life and mana regen\n" +
                "Increased defense, movement speed and damage reduction while submerged in liquid\n" +
                "Enemies take damage when they hit you\n" +
                "You emit a cloud of mushroom spores when you are hit\n" +
                "10% increased damage reduction\n" +
                "5% of the damage from enemy attacks is absorbed and converted into healing");
        }

        public override void SetDefaults()
        {
            item.defense = 10;
            item.width = 20;
            item.height = 24;
            item.value = CalamityGlobalItem.Rarity9BuyPrice;
            item.rare = ItemRarityID.Cyan;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            // Removed Giant Shell speed boost from The Absorber
            // modPlayer.gShell = true;
            modPlayer.fCarapace = true;
            modPlayer.seaShell = true;
            modPlayer.absorber = true;
            player.statManaMax2 += 20;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<GrandGelatin>());
            recipe.AddIngredient(ModContent.ItemType<SeaShell>());
            recipe.AddIngredient(ModContent.ItemType<CrawCarapace>());
            recipe.AddIngredient(ModContent.ItemType<FungalCarapace>());
            recipe.AddIngredient(ModContent.ItemType<GiantTortoiseShell>());
            recipe.AddIngredient(ModContent.ItemType<RoverDrive>());
            recipe.AddIngredient(ModContent.ItemType<DepthCells>(), 15);
            recipe.AddIngredient(ModContent.ItemType<Lumenite>(), 15);
            recipe.AddIngredient(ModContent.ItemType<Tenebris>(), 5);
            recipe.AddIngredient(ModContent.ItemType<MolluskHusk>(), 5); // mfw ingredient bloat
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<GrandGelatin>());
            recipe.AddIngredient(ModContent.ItemType<SeaShell>());
            recipe.AddIngredient(ModContent.ItemType<FungalCarapace>());
            recipe.AddIngredient(ModContent.ItemType<GiantShell>());
            recipe.AddIngredient(ModContent.ItemType<GiantTortoiseShell>());
            recipe.AddIngredient(ModContent.ItemType<RoverDrive>());
            recipe.AddIngredient(ModContent.ItemType<DepthCells>(), 15);
            recipe.AddIngredient(ModContent.ItemType<Lumenite>(), 15);
            recipe.AddIngredient(ModContent.ItemType<Tenebris>(), 5);
            recipe.AddIngredient(ModContent.ItemType<MolluskHusk>(), 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
