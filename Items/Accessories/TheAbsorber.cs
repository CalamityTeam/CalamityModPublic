using CalamityMod.Buffs.StatDebuffs;
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
            SacrificeTotal = 1;
            DisplayName.SetDefault("The Absorber");
            Tooltip.SetDefault("5% increased movement and jump speed\n" +
                "+20 max life and mana\n" +
                "Standing still boosts life and mana regen\n" +
                "Enemies take damage when they touch you\n" +
                "You emit a cloud of mushroom spores when you are hit\n" +
                "7% increased damage reduction\n" +
                "5% of the damage from enemy attacks is absorbed and converted into healing\n" +
                "Grants immunity to Armor Crunch");
        }

        public override void SetDefaults()
        {
            Item.defense = 15;
            Item.width = 20;
            Item.height = 24;
            Item.value = CalamityGlobalItem.Rarity9BuyPrice;
            Item.rare = ItemRarityID.Cyan;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            // Removed Giant Shell speed boost from The Absorber
            // modPlayer.gShell = true;
            modPlayer.fCarapace = true;
            modPlayer.absorber = true;
            player.statManaMax2 += 20;
            player.buffImmune[ModContent.BuffType<ArmorCrunch>()] = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<GrandGelatin>().
                AddIngredient<CrawCarapace>().
                AddIngredient<FungalCarapace>().
                AddIngredient<GiantTortoiseShell>().
                AddIngredient<RoverDrive>().
                AddIngredient<DepthCells>(15).
                AddIngredient<Lumenyl>(15).
                AddIngredient<Tenebris>(5).
                AddIngredient<MolluskHusk>(5).
                AddTile(TileID.LunarCraftingStation).
                Register();

            CreateRecipe().
                AddIngredient<GrandGelatin>().
                AddIngredient<FungalCarapace>().
                AddIngredient<GiantShell>().
                AddIngredient<GiantTortoiseShell>().
                AddIngredient<RoverDrive>().
                AddIngredient<DepthCells>(15).
                AddIngredient<Lumenyl>(15).
                AddIngredient<Tenebris>(5).
                AddIngredient<MolluskHusk>(5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
