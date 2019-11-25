using CalamityMod.Projectiles.Melee.Yoyos;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class YinYo : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("YinYo");
            Tooltip.SetDefault("Fires light or dark shards when enemies are near\n" +
                "Light shards fly up and down while dark shards fly left and right");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.Chik);
            item.damage = 34;
            item.useTime = 25;
            item.useAnimation = 25;
            item.useStyle = 5;
            item.channel = true;
            item.melee = true;
            item.knockBack = 3.5f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<YinYoyo>();
            ItemID.Sets.Yoyo[item.type] = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.DarkShard);
            recipe.AddIngredient(ItemID.LightShard);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
