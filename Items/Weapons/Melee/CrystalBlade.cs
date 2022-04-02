using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Melee;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class CrystalBlade : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal Blade");
            Tooltip.SetDefault("Fires slow-moving clouds of crystal dust");
        }

        public override void SetDefaults()
        {
            item.width = 66;
            item.damage = 45;
            item.melee = true;
            item.useAnimation = 25;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 25;
            item.useTurn = true;
            item.knockBack = 4.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 66;
            item.value = CalamityGlobalItem.Rarity4BuyPrice;
            item.rare = ItemRarityID.LightRed;
            item.shoot = ModContent.ProjectileType<CrystalDust>();
            item.shootSpeed = 3f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.CrystalShard, 10);
            recipe.AddIngredient(ModContent.ItemType<SeaPrism>(), 15);
            recipe.AddRecipeGroup("AnyCobaltBar", 8);
            recipe.AddIngredient(ItemID.PixieDust, 10);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
