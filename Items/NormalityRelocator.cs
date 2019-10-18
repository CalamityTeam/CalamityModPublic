using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ID;

namespace CalamityMod.Items
{
    public class NormalityRelocator : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Normality Relocator");
            Tooltip.SetDefault("I'll be there in the blink of an eye\n" +
                "Press Z to teleport to the position of the mouse\n" +
                "Boosts movement and fall speed by 10%\n" +
                "Works while in the inventory");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 7));
        }

        public override void SetDefaults()
        {
            item.width = 38;
            item.height = 38;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.Calamity().postMoonLordRarity = 21;
        }

        public override void UpdateInventory(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.normalityRelocator = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.RodofDiscord);
            recipe.AddIngredient(ItemID.FragmentStardust, 30);
            recipe.AddIngredient(null, "ExodiumClusterOre", 10);
            recipe.AddIngredient(null, "Cinderplate", 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
