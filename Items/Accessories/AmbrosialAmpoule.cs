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
            SacrificeTotal = 1;
            DisplayName.SetDefault("Ambrosial Ampoule");
            Tooltip.SetDefault("You emit light\n" +
                "5% increased damage reduction and increased life regen\n" +
                "Grants immunity to the Frozen, Chilled, Frostburn, Cursed Inferno and Burning Blood debuffs");
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
            CreateRecipe().
                AddIngredient<CorruptFlask>().
                AddIngredient<RadiantOoze>().
                AddIngredient<CryonicBar>(5).
                AddIngredient<SeaPrism>(10).
                AddTile(TileID.MythrilAnvil).
                Register();

            CreateRecipe().
                AddIngredient<CrimsonFlask>().
                AddIngredient<RadiantOoze>().
                AddIngredient<CryonicBar>(5).
                AddIngredient<SeaPrism>(10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
