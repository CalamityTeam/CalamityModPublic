using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;
namespace CalamityMod.Items
{
    public class RotBall : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rot Ball");
        }

        public override void SafeSetDefaults()
        {
            item.width = 26;
            item.damage = 26;
            item.noMelee = true;
            item.consumable = true;
            item.noUseGraphic = true;
            item.useAnimation = 13;
            item.crit = 8;
            item.useStyle = 1;
            item.useTime = 13;
            item.knockBack = 2.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 26;
            item.maxStack = 999;
            item.value = 1000;
            item.rare = 3;
            item.shoot = ModContent.ProjectileType<RotBallProjectile>();
            item.shootSpeed = 16f;
            item.Calamity().rogue = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "TrueShadowScale");
            recipe.AddIngredient(ItemID.RottenChunk);
            recipe.AddIngredient(ItemID.DemoniteBar);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this, 100);
            recipe.AddRecipe();
        }
    }
}
