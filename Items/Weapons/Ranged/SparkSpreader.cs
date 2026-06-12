using CalamityMod.Projectiles.Ranged;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class SparkSpreader : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [BuffID.OnFire];
        }
        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 26;
            Item.damage = 16;
            Item.knockBack = 1f;
            Item.DamageType = DamageClass.Ranged;
            Item.autoReuse = true;
            Item.useTime = 12;
            Item.useAnimation = 48;
            Item.reuseDelay = 12;
            Item.useLimitPerAnimation = 4;
            Item.useAmmo = AmmoID.Gel;
            Item.consumeAmmoOnFirstShotOnly = true;
            Item.shootSpeed = 6f;
            Item.shoot = ModContent.ProjectileType<SparkSpreaderFire>();

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item34;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
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
