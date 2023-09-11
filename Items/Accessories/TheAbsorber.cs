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
        public new string LocalizationCategory => "Items.Accessories";
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
            modPlayer.absorber = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<GrandGelatin>().
                AddIngredient<Baroclaw>().
                AddIngredient<GiantTortoiseShell>().
                AddIngredient<MolluskHusk>(5).
                AddIngredient<MeldConstruct>(6).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
