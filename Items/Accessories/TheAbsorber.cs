using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

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
            Item.defense = 10;
            Item.width = 20;
            Item.height = 24;
            Item.value = CalamityGlobalItem.Rarity9BuyPrice;
            Item.rare = ItemRarityID.Cyan;
            Item.accessory = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
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
            CreateRecipe().
                AddIngredient<GrandGelatin>().
                AddIngredient<SeaShell>().
                AddIngredient<CrawCarapace>().
                AddIngredient<FungalCarapace>().
                AddIngredient<GiantTortoiseShell>().
                AddIngredient<RoverDrive>().
                AddIngredient<DepthCells>(15).
                AddIngredient<Lumenite>(15).
                AddIngredient<Tenebris>(5).
                AddIngredient<MolluskHusk>(5).
                AddTile(TileID.LunarCraftingStation).
                Register();

            CreateRecipe().
                AddIngredient<GrandGelatin>().
                AddIngredient<SeaShell>().
                AddIngredient<FungalCarapace>().
                AddIngredient<GiantShell>().
                AddIngredient<GiantTortoiseShell>().
                AddIngredient<RoverDrive>().
                AddIngredient<DepthCells>(15).
                AddIngredient<Lumenite>(15).
                AddIngredient<Tenebris>(5).
                AddIngredient<MolluskHusk>(5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
