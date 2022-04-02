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
            item.width = 26;
            item.height = 26;
            item.rare = ItemRarityID.Cyan;
            item.value = CalamityGlobalItem.Rarity9BuyPrice;
            item.accessory = true;
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
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.CharmofMyths);
            recipe.AddIngredient(ItemID.StarVeil);
            recipe.AddIngredient(ModContent.ItemType<AstralBar>(), 10);
            recipe.AddIngredient(ItemID.MeteoriteBar, 10);
            recipe.AddIngredient(ModContent.ItemType<SeaPrism>(), 15);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
