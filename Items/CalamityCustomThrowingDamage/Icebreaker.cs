using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.CalamityCustomThrowingDamage
{
    public class Icebreaker : CalamityDamageItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Icebreaker");
        }

        public override void SafeSetDefaults()
        {
            item.width = 60;
            item.damage = 57;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 14;
            item.useStyle = 1;
            item.useTime = 14;
            item.knockBack = 6.75f;
            item.UseSound = SoundID.Item1;
            item.melee = true;
            item.height = 60;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.shoot = mod.ProjectileType("Icebreaker");
            item.shootSpeed = 16f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).rogue = true;
		}

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "CryoBar", 11);
            recipe.AddTile(TileID.IceMachine);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
