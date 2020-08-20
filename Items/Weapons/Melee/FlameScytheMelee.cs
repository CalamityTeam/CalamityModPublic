using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Hybrid;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class FlameScytheMelee : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Subduction Slicer");
            Tooltip.SetDefault("Throws a scythe that explodes on enemy hits");
        }

        public override void SetDefaults()
        {
            item.width = 50;
            item.damage = 90;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.autoReuse = true;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 20;
            item.knockBack = 8.5f;
            item.UseSound = SoundID.Item1;
            item.melee = true;
            item.height = 48;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
            item.shoot = ModContent.ProjectileType<FlameScytheProjectile>();
            item.shootSpeed = 16f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CruptixBar>(), 9);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
