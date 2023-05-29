using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class TheAbsorber : ModItem, ILocalizedModType
    {
        public string LocalizationCategory => "Items.Accessories";
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
                AddIngredient<MolluskHusk>(5).
                AddTile(TileID.LunarCraftingStation).
                Register();

            CreateRecipe().
                AddIngredient<GrandGelatin>().
                AddIngredient<GiantShell>().
                AddIngredient<FungalCarapace>().
                AddIngredient<GiantTortoiseShell>().
                AddIngredient<MolluskHusk>(5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
