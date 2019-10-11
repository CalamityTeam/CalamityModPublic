using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.SlimeGod
{
    public class TheGodsGambit : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The God's Gambit");
            Tooltip.SetDefault("Fires a stream of slime when enemies are near");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(3291);
            item.damage = 28;
            item.useTime = 21;
            item.useAnimation = 21;
            item.useStyle = 5;
            item.channel = true;
            item.melee = true;
            item.knockBack = 3.5f;
            item.value = Item.buyPrice(0, 12, 0, 0);
            item.rare = 4;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("TheGodsGambitProjectile");
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "PurifiedGel", 30);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
