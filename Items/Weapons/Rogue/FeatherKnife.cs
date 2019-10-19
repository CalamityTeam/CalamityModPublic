using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
namespace CalamityMod.Items.Weapons.Rogue
{
    public class FeatherKnife : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Feather Knife");
        }

        public override void SafeSetDefaults()
        {
            item.width = 18;
            item.damage = 24;
            item.noMelee = true;
            item.consumable = true;
            item.noUseGraphic = true;
            item.useAnimation = 11;
            item.useStyle = 1;
            item.useTime = 11;
            item.knockBack = 2f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 32;
            item.maxStack = 999;
            item.value = 300;
            item.rare = 3;
            item.shoot = ModContent.ProjectileType<FeatherKnifeProjectile>();
            item.shootSpeed = 14f;
            item.Calamity().rogue = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AerialiteBar>());
            recipe.AddTile(TileID.SkyMill);
            recipe.SetResult(this, 30);
            recipe.AddRecipe();
        }
    }
}
