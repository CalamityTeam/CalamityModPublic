using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Projectiles.Magic;

namespace CalamityMod.Items.Weapons.Magic
{
    public class ElementalRay : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Elemental Ray");
            Tooltip.SetDefault("Casts a rainbow ray that splits when enemies are near it");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 150;
            item.magic = true;
            item.mana = 18;
            item.width = 62;
            item.height = 62;
            item.useTime = 16;
            item.useAnimation = 16;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 7.5f;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item60;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<ElementalRayBeam>();
            item.shootSpeed = 6f;
            item.Calamity().postMoonLordRarity = 12;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 5);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddIngredient(ModContent.ItemType<TerraRay>());
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
