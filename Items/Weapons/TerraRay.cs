using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class TerraRay : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Terra Ray");
            Tooltip.SetDefault("Casts an energy ray that splits if enemies are near it");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 85;
            item.magic = true;
            item.mana = 10;
            item.width = 54;
            item.height = 54;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 4f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
            item.UseSound = SoundID.Item60;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("TerraRay");
            item.shootSpeed = 6f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "NightsRay");
            recipe.AddIngredient(null, "LivingShard", 7);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "CarnageRay");
            recipe.AddIngredient(null, "LivingShard", 7);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
