using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Quagmire : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Quagmire");
            Tooltip.SetDefault("Fires spore clouds");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.HelFire);
            item.width = 30;
            item.height = 36;
            item.damage = 52;
            item.useTime = 22;
            item.useAnimation = 22;
            item.useStyle = 5;
            item.channel = true;
            item.melee = true;
            item.knockBack = 3.5f;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 7;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<QuagmireProjectile>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<DraedonBar>(), 6);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
