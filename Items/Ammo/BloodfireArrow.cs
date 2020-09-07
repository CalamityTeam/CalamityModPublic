using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Ammo
{
    public class BloodfireArrow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloodfire Arrow");
            Tooltip.SetDefault("Heals you a small amount on enemy hits");
        }

        public override void SetDefaults()
        {
            item.damage = 40;
            item.ranged = true;
            item.width = 14;
            item.height = 36;
            item.maxStack = 999;
            item.consumable = true;
            item.knockBack = 3.5f;
            item.value = 2000;
            item.shoot = ModContent.ProjectileType<BloodfireArrowProj>();
            item.shootSpeed = 10f;
            item.ammo = AmmoID.Arrow;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BloodstoneCore>());
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this, 250);
            recipe.AddRecipe();
        }
    }
}
