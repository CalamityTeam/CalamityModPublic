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
            Item.damage = 14;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 22;
            Item.height = 36;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.knockBack = 1.5f;
            Item.value = Item.sellPrice(copper: 12);
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ModContent.ProjectileType<NapalmArrowProj>();
            Item.shootSpeed = 13f;
            Item.ammo = AmmoID.Arrow;
        }

        public override void AddRecipes()
        {
            CreateRecipe(250).
                AddIngredient(ItemID.WoodenArrow, 250).
                AddIngredient<EssenceofChaos>().
                AddIngredient(ItemID.Torch).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
