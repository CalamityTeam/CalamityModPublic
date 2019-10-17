using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items
{
    public class SausageMaker : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sausage Maker");
            Tooltip.SetDefault("Sprays homing blood on hit");
        }

        public override void SetDefaults()
        {
            item.width = 44;
            item.damage = 29;
            item.melee = true;
            item.noMelee = true;
            item.useTurn = true;
            item.noUseGraphic = true;
            item.useAnimation = 20;
            item.useStyle = 5;
            item.useTime = 20;
            item.knockBack = 6.25f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = false;
            item.height = 42;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = 3;
            item.shoot = ModContent.ProjectileType<Projectiles.SausageMaker>();
            item.shootSpeed = 6f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "BloodSample", 8);
            recipe.AddIngredient(ItemID.Vertebrae, 4);
            recipe.AddIngredient(ItemID.CrimtaneBar, 5);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
