using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Ammo
{
    public class AcidBullet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Acid Round");
            Tooltip.SetDefault("Explodes into acid that inflicts the plague\n" +
                "Does more damage the higher the target's defense");
        }

        public override void SetDefaults()
        {
            item.damage = 20;
            item.ranged = true;
            item.width = 8;
            item.height = 8;
            item.maxStack = 999;
            item.consumable = true;
            item.knockBack = 1.5f;
            item.value = Item.sellPrice(copper: 16);
            item.rare = ItemRarityID.Yellow;
            item.shoot = ModContent.ProjectileType<AcidBulletProj>();
            item.shootSpeed = 10f;
            item.ammo = AmmoID.Bullet;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.MusketBall, 150);
            recipe.AddIngredient(ModContent.ItemType<PlagueCellCluster>());
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this, 150);
            recipe.AddRecipe();
        }
    }
}
