using CalamityMod.Projectiles;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class TheJailor : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";

        public override void SetStaticDefaults()
        {
            ItemID.Sets.IsRangedSpecialistWeapon[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 102;
            Item.height = 70;
            Item.damage = 235;
            Item.DamageType = DamageClass.Ranged;
            Item.useAnimation = Item.useTime = 28;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 8f;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<ExoticRainbow>();
            Item.UseSound = SoundID.Item14;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<PrismMine>();
            Item.shootSpeed = 28f;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-40f, -16f);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 shootVelocity = velocity;
            Vector2 shootDirection = shootVelocity.SafeNormalize(Vector2.UnitX * player.direction);
            Vector2 gunTip = position + shootDirection * Item.scale * 45f;

            Projectile.NewProjectile(source, gunTip, shootVelocity, Item.shoot, damage, knockback, player.whoAmI);
            return false;
        }
    }
}
