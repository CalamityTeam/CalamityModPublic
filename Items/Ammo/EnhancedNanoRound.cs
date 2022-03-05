using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Ammo
{
    public class EnhancedNanoRound : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Enhanced Nano Round");
            Tooltip.SetDefault("Confuses enemies and releases a cloud of nanites when enemies die");
        }

        public override void SetDefaults()
        {
            item.damage = 12;
            item.ranged = true;
            item.width = 8;
            item.height = 8;
            item.maxStack = 999;
            item.consumable = true;
            item.knockBack = 5.5f;
            item.value = Item.sellPrice(copper: 16);
            item.rare = ItemRarityID.Lime;
            item.shoot = ModContent.ProjectileType<EnhancedNanoRoundProj>();
            item.shootSpeed = 8f;
            item.ammo = AmmoID.Bullet;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.NanoBullet, 250);
            recipe.AddIngredient(ModContent.ItemType<EssenceofEleum>());
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this, 250);
            recipe.AddRecipe();
        }
    }
}
