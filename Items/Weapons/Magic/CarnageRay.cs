using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Magic
{
    public class CarnageRay : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Carnage Ray");
            Tooltip.SetDefault("Fires a blood ray that splits if enemies are near it");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 24;
            item.magic = true;
            item.mana = 10;
            item.width = 46;
            item.height = 46;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 3.25f;
            item.value = Item.buyPrice(0, 12, 0, 0);
            item.rare = 4;
            item.UseSound = SoundID.Item72;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<BloodRay>();
            item.shootSpeed = 6f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.WandofSparking);
            recipe.AddIngredient(ItemID.CrimsonRod);
            recipe.AddIngredient(ItemID.AmberStaff);
            recipe.AddIngredient(ItemID.MagicMissile);
            recipe.AddIngredient(null, "BloodSample", 15);
            recipe.AddIngredient(null, "PurifiedGel", 10);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
