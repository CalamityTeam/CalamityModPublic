using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Aorta : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aorta");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.Valor);
            item.damage = 25;
            item.useTime = 22;
            item.useAnimation = 22;
            item.useStyle = 5;
            item.channel = true;
            item.melee = true;
            item.knockBack = 4.25f;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = 3;
            item.autoReuse = false;
            item.shoot = ModContent.ProjectileType<AortaProjectile>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "BloodSample", 6);
            recipe.AddIngredient(ItemID.Vertebrae, 3);
            recipe.AddIngredient(ItemID.CrimtaneBar, 3);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
