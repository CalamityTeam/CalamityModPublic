using Terraria;
using CalamityMod.Projectiles;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TyphonsGreed : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Typhon's Greed");
            Tooltip.SetDefault("Summons water spirits while in use");
        }

        public override void SetDefaults()
        {
            item.damage = 75;
            item.melee = true;
            item.width = 16;
            item.height = 16;
            item.useTime = 30;
            item.useAnimation = 30;
            item.useStyle = 5;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.knockBack = 5f;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 7;
            item.UseSound = SoundID.DD2_SkyDragonsFurySwing;
            item.autoReuse = true;
            item.channel = true;
            item.shoot = ModContent.ProjectileType<TyphonsGreedStaff>();
            item.shootSpeed = 24f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "DepthCells", 30);
            recipe.AddIngredient(null, "Lumenite", 10);
            recipe.AddIngredient(null, "Tenebris", 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
