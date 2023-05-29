using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    public class NapalmArrow : ModItem, ILocalizedModType
    {
        public string LocalizationCategory => "Items.Ammo";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
        {
            Item.damage = 11;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 22;
            Item.height = 36;
            Item.maxStack = 9999;
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
                AddIngredient<EssenceofHavoc>().
                AddIngredient(ItemID.Torch).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
