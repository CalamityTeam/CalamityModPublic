using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Ammo
{
    public class NapalmArrow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Napalm Arrow");
            Tooltip.SetDefault("Explodes into fire shards");
        }

        public override void SetDefaults()
        {
            item.damage = 14;
            item.ranged = true;
            item.width = 22;
            item.height = 36;
            item.maxStack = 999;
            item.consumable = true;
            item.knockBack = 1.5f;
            item.value = Item.sellPrice(copper: 12);
            item.rare = ItemRarityID.LightRed;
            item.shoot = ModContent.ProjectileType<NapalmArrowProj>();
            item.shootSpeed = 13f;
            item.ammo = AmmoID.Arrow;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.WoodenArrow, 250);
            recipe.AddIngredient(ModContent.ItemType<EssenceofChaos>());
            recipe.AddIngredient(ItemID.Torch);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 250);
            recipe.AddRecipe();
        }
    }
}
