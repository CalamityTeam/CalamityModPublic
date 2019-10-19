using Terraria;
using Terraria.ModLoader; using CalamityMod.Items.Materials;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class StormSurge : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Surge");
            Tooltip.SetDefault("Fear the storm\n" +
                "Does not consume ammo");
        }

        public override void SetDefaults()
        {
            item.damage = 22;
            item.ranged = true;
            item.width = 58;
            item.height = 22;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 5f;
            item.UseSound = SoundID.Item122;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = 3;
            item.autoReuse = false;
            item.shoot = ModContent.ProjectileType<Projectiles.StormSurge>();
            item.shootSpeed = 12f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<StormlionMandible>());
            recipe.AddIngredient(ModContent.ItemType<VictideBar>(), 2);
            recipe.AddIngredient(ModContent.ItemType<AerialiteBar>(), 2);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
