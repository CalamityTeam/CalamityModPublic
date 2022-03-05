using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Ammo
{
    public class TerraBullet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Terra Bullet");
            Tooltip.SetDefault("Explodes and splits into homing terra shards on death");
        }

        public override void SetDefaults()
        {
            item.damage = 9;
            item.ranged = true;
            item.width = 8;
            item.height = 8;
            item.maxStack = 999;
            item.consumable = true;
            item.knockBack = 1.25f;
            item.value = Item.sellPrice(copper: 16);
            item.rare = ItemRarityID.Lime;
            item.shoot = ModContent.ProjectileType<TerraBulletMain>();
            item.shootSpeed = 10f;
            item.ammo = AmmoID.Bullet;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.CrystalBullet, 100);
            recipe.AddIngredient(ModContent.ItemType<LivingShard>());
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this, 100);
            recipe.AddRecipe();
        }
    }
}
