using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Magic
{
    public class GleamingMagnolia : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gleaming Magnolia");
            Tooltip.SetDefault("Casts a gleaming bolt that explodes into smaller bolts");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 32;
            item.magic = true;
            item.mana = 11;
            item.width = 52;
            item.height = 54;
            item.useTime = 27;
            item.useAnimation = 27;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 5.5f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.UseSound = SoundID.Item109;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<GleamingBolt>();
            item.shootSpeed = 14f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "ManaRose");
            recipe.AddIngredient(ItemID.HallowedBar, 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
