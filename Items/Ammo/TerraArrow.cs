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
            Item.damage = 15;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 22;
            Item.height = 36;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.knockBack = 1.5f;
            Item.value = Item.sellPrice(copper: 20);
            Item.rare = ItemRarityID.Lime;
            Item.shoot = ModContent.ProjectileType<TerraArrowMain>();
            Item.shootSpeed = 15f;
            Item.ammo = AmmoID.Arrow;
        }

        public override void AddRecipes()
        {
            CreateRecipe(250)
                .AddIngredient(ItemID.WoodenArrow, 250)
                .AddIngredient<LivingShard>()
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
