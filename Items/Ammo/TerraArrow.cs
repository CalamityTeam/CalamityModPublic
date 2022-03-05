using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    public class TerraArrow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Terra Arrow");
            Tooltip.SetDefault("Travels incredibly quickly and explodes into more arrows when it hits a certain velocity");
        }

        public override void SetDefaults()
        {
            item.damage = 15;
            item.ranged = true;
            item.width = 22;
            item.height = 36;
            item.maxStack = 999;
            item.consumable = true;
            item.knockBack = 1.5f;
            item.value = Item.sellPrice(copper: 20);
            item.rare = ItemRarityID.Lime;
            item.shoot = ModContent.ProjectileType<TerraArrowMain>();
            item.shootSpeed = 15f;
            item.ammo = AmmoID.Arrow;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.WoodenArrow, 250);
            recipe.AddIngredient(ModContent.ItemType<LivingShard>());
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this, 250);
            recipe.AddRecipe();
        }
    }
}
