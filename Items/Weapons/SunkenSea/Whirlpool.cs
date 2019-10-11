using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.SunkenSea
{
    public class Whirlpool : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Riptide");
            Tooltip.SetDefault("Sprays a spiral of aqua streams in random directions");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.TheEyeOfCthulhu);
            item.damage = 18;
            item.width = 30;
            item.height = 44;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
            item.knockBack = 1f;
            item.channel = true;
            item.melee = true;
            item.useStyle = 5;
            item.useAnimation = 25;
            item.useTime = 25;
            item.shoot = mod.ProjectileType("WhirlpoolProjectile");
            item.shootSpeed = 18f;
            item.UseSound = SoundID.Item1;
            ItemID.Sets.Yoyo[item.type] = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "SeaPrism", 7);
            recipe.AddIngredient(null, "Navystone", 10);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
