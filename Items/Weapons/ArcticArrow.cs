using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class ArcticArrow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arctic Arrow");
            Tooltip.SetDefault("Freezes enemies for a short time");
        }

        public override void SetDefaults()
        {
            item.damage = 16;
            item.ranged = true;
            item.width = 22;
            item.height = 36;
            item.maxStack = 999;
            item.consumable = true;
            item.knockBack = 1.5f;
            item.value = 1200;
            item.rare = 3;
            item.shoot = mod.ProjectileType("ArcticArrow");
            item.shootSpeed = 13f;
            item.ammo = 40;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "CryoBar");
            recipe.AddTile(TileID.IceMachine);
            recipe.SetResult(this, 250);
            recipe.AddRecipe();
        }
    }
}
