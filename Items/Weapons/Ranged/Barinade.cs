using Terraria.DataStructures;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Barinade : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.damage = 3;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 30;
            Item.height = 44;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.2f;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BarinadeArrow>();
            Item.shootSpeed = 15f;
            Item.useAmmo = AmmoID.Arrow;
            Item.Calamity().canFirePointBlankShots = true;
        }
        public override Vector2? HoldoutOffset() => new Vector2(0, 4);
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position + velocity.RotatedBy(-0.55f), velocity.RotatedBy(0.025f), ModContent.ProjectileType<BarinadeArrow>(), damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, position + velocity.RotatedBy(0.55f), velocity.RotatedBy(-0.025f), ModContent.ProjectileType<BarinadeArrow>(), damage, knockback, player.whoAmI);
            return false;
        }
    }
}
