using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.CalamityCustomThrowingDamage
{
    public class GelDart : CalamityDamageItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gel Dart");
			Tooltip.SetDefault("Throws bouncing darts");
		}

        public override void SafeSetDefaults()
        {
            item.width = 14;
            item.damage = 25;
            item.noMelee = true;
            item.consumable = true;
            item.noUseGraphic = true;
            item.useAnimation = 11;
            item.useStyle = 1;
            item.useTime = 11;
            item.knockBack = 2.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 28;
            item.maxStack = 999;
            item.value = 1000;
            item.rare = 4;
            item.shoot = mod.ProjectileType("GelDartProjectile");
            item.shootSpeed = 12f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).rogue = true;
		}

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "PurifiedGel", 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 150);
            recipe.AddRecipe();
        }
    }
}
