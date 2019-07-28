using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.CalamityCustomThrowingDamage
{
    public class XerocPitchfork : CalamityDamageItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Xeroc Pitchfork");
        }

        public override void SafeSetDefaults()
        {
            item.width = 48;
            item.damage = 360;
            item.noMelee = true;
            item.consumable = true;
            item.noUseGraphic = true;
            item.useAnimation = 19;
            item.useStyle = 1;
            item.useTime = 19;
            item.knockBack = 8f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 48;
            item.maxStack = 999;
            item.value = 10000;
            item.rare = 9;
            item.shoot = mod.ProjectileType("XerocPitchforkProjectile");
            item.shootSpeed = 16f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).rogue = true;
		}

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "MeldiateBar");
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this, 20);
            recipe.AddRecipe();
        }
    }
}
