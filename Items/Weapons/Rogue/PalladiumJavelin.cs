using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;
namespace CalamityMod.Items
{
    public class PalladiumJavelin : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Palladium Javelin");
        }

        public override void SafeSetDefaults()
        {
            item.width = 44;
            item.damage = 50;
            item.noMelee = true;
            item.consumable = true;
            item.noUseGraphic = true;
            item.useAnimation = 19;
            item.useStyle = 1;
            item.useTime = 19;
            item.knockBack = 5.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 44;
            item.shoot = 330;
            item.maxStack = 999;
            item.value = 1200;
            item.rare = 4;
            item.shoot = ModContent.ProjectileType<PalladiumJavelinProjectile>();
            item.shootSpeed = 16f;
            item.Calamity().rogue = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.PalladiumBar);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 20);
            recipe.AddRecipe();
        }
    }
}
