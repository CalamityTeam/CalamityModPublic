using CalamityMod.CalPlayer;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class DeificAmulet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deific Amulet");
            Tooltip.SetDefault("Causes stars to fall and grants increased immune time when damaged\n" +
                "Provides life regeneration and reduces the cooldown of healing potions\n");
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.rare = ItemRarityID.Cyan;
            Item.value = CalamityGlobalItem.Rarity9BuyPrice;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.dAmulet = true;
            player.longInvince = true;
            player.lifeRegen += 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CharmofMyths)
                .AddIngredient(ItemID.StarVeil)
                .AddIngredient<AstralBar>(10)
                .AddIngredient(ItemID.MeteoriteBar, 10)
                .AddIngredient<SeaPrism>(15)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
