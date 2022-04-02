using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Ammo
{
    public class ElysianArrow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Elysian Arrow");
            Tooltip.SetDefault("Summons meteors from the sky on death");
        }

        public override void SetDefaults()
        {
            item.damage = 20;
            item.ranged = true;
            item.width = 22;
            item.height = 36;
            item.maxStack = 999;
            item.consumable = true;
            item.knockBack = 3f;
            item.value = Item.sellPrice(copper: 24);
            item.rare = ItemRarityID.Purple;
            item.shoot = ModContent.ProjectileType<ElysianArrowProj>();
            item.shootSpeed = 10f;
            item.ammo = AmmoID.Arrow;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.HolyArrow, 150);
            recipe.AddIngredient(ModContent.ItemType<UnholyEssence>());
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this, 150);
            recipe.AddRecipe();
        }
    }
}
