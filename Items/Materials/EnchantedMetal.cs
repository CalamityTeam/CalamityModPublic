using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class EnchantedMetal : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Enchanted Metal");
        }
        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 20;
            item.value = Item.buyPrice(0, 15, 0, 0);
            item.rare = 5;
            item.maxStack = 99;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(item.type, 3);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(ItemID.MechanicalEye);
            recipe.AddRecipe();

            recipe = new ModRecipe(mod);
            recipe.AddIngredient(item.type, 3);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(ItemID.MechanicalSkull);
            recipe.AddRecipe();

            recipe = new ModRecipe(mod);
            recipe.AddIngredient(item.type, 3);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(ItemID.MechanicalWorm);
            recipe.AddRecipe();
        }
    }
}
