using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;
namespace CalamityMod.Items
{
    public class TarragonThrowingDart : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tarragon Throwing Dart");
        }

        public override void SafeSetDefaults()
        {
            item.width = 34;
            item.damage = 380;
            item.noMelee = true;
            item.consumable = true;
            item.noUseGraphic = true;
            item.useAnimation = 11;
            item.useStyle = 1;
            item.useTime = 11;
            item.knockBack = 4.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 34;
            item.maxStack = 999;
            item.value = 25000;
            item.shoot = ModContent.ProjectileType<TarragonThrowingDartProjectile>();
            item.shootSpeed = 24f;
            item.Calamity().rogue = true;
            item.Calamity().postMoonLordRarity = 12;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "UeliaceBar");
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this, 100);
            recipe.AddRecipe();
        }
    }
}
