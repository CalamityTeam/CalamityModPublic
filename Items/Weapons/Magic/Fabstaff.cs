using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Fabstaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fabstaff");
            Tooltip.SetDefault("Casts a bouncing beam that splits when enemies are near it\n" +
                "The splitting can trigger long after the beam has vanished");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 800;
            item.magic = true;
            item.mana = 50;
            item.width = 84;
            item.height = 84;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 5f;
            item.value = Item.buyPrice(5, 0, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item60;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<FabRay>();
            item.shootSpeed = 6f;
            item.Calamity().postMoonLordRarity = 18;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Phantoplasm>(), 100);
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 50);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
