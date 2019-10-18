using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Magic
{
    public class CoralSpout : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Coral Spout");
            Tooltip.SetDefault("Casts coral water spouts that lay on the ground and damage enemies");
        }

        public override void SetDefaults()
        {
            item.damage = 13;
            item.magic = true;
            item.mana = 4;
            item.width = 28;
            item.height = 30;
            item.useTime = 26;
            item.useAnimation = 26;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 2f;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
            item.UseSound = SoundID.Item17;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<CoralSpike>();
            item.shootSpeed = 16f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "VictideBar", 2);
            recipe.AddIngredient(ItemID.Coral, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
