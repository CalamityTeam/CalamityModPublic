using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class EvergladeSpray : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Everglade Spray");
            Tooltip.SetDefault("Fires a stream of burning green ichor");
        }

        public override void SetDefaults()
        {
            item.damage = 28;
            item.magic = true;
            item.mana = 8;
            item.width = 34;
            item.height = 30;
            item.useTime = 6;
            item.useAnimation = 18;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4.5f;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = ItemRarityID.Lime;
            item.UseSound = SoundID.Item13;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<EvergladeSprayProjectile>();
            item.shootSpeed = 10f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.GoldenShower);
            recipe.AddIngredient(ModContent.ItemType<DraedonBar>(), 3);
            recipe.AddTile(TileID.Bookcases);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.CursedFlames);
            recipe.AddIngredient(ModContent.ItemType<DraedonBar>(), 3);
            recipe.AddTile(TileID.Bookcases);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
