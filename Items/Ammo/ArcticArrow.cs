using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    public class ArcticArrow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arctic Arrow");
            Tooltip.SetDefault("Freezes enemies for a short time");
        }

        public override void SetDefaults()
        {
            item.damage = 18;
            item.ranged = true;
            item.width = 22;
            item.height = 36;
            item.maxStack = 999;
            item.consumable = true;
            item.knockBack = 1.5f;
            item.value = Item.sellPrice(copper: 12);
            item.rare = ItemRarityID.Pink;
            item.shoot = ModContent.ProjectileType<ArcticArrowProj>();
            item.shootSpeed = 13f;
            item.ammo = AmmoID.Arrow;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<VerstaltiteBar>());
            recipe.AddTile(TileID.IceMachine);
            recipe.SetResult(this, 250);
            recipe.AddRecipe();
        }
    }
}
