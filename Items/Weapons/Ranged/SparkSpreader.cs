using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class SparkSpreader : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.damage = 10;
            Item.knockBack = 1f;
            Item.DamageType = DamageClass.Ranged;
            Item.autoReuse = true;
            Item.useTime = 12;
            Item.useAnimation = 36;
            Item.useAmmo = AmmoID.Gel;
            Item.consumeAmmoOnFirstShotOnly = true;
            Item.shootSpeed = 6f;
            Item.shoot = ModContent.ProjectileType<SparkSpreaderFire>();

            Item.width = 52;
            Item.height = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item34;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-4, 0);

        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnyGoldBar", 10).
                AddIngredient(ItemID.Ruby).
                AddIngredient(ItemID.Gel, 12).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
