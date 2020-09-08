using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Ammo
{
    public class HolyFireBullet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Fire Bullet");
            Tooltip.SetDefault("Explosive holy bullets");
        }

        public override void SetDefaults()
        {
            item.damage = 27;
            item.ranged = true;
            item.width = 22;
            item.height = 22;
            item.maxStack = 999;
            item.consumable = true;
            item.knockBack = 2f;
            item.value = 2000;
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
            item.shoot = ModContent.ProjectileType<HolyFireBulletProj>();
            item.shootSpeed = 6f;
            item.ammo = AmmoID.Bullet;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.ExplodingBullet, 100);
            recipe.AddIngredient(ModContent.ItemType<UnholyEssence>());
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this, 100);
            recipe.AddRecipe();
        }
    }
}
