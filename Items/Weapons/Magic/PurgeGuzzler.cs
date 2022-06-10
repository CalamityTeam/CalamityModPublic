using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Sounds;

namespace CalamityMod.Items.Weapons.Magic
{
    public class PurgeGuzzler : ModItem
    {
        private const float Spread = 0.025f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Purge Guzzler");
            Tooltip.SetDefault("Emits three beams of holy energy in a tight spread");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 44;
            Item.damage = 120;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 22;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4.5f;
            Item.UseSound = CommonCalamitySounds.LaserCannonSound;
            Item.shoot = ModContent.ProjectileType<HolyLaser>();
            Item.shootSpeed = 6f;

            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Fire extra lasers to the left and right
            Projectile.NewProjectile(source, position, velocity.RotatedBy(-Spread), type, damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, velocity.RotatedBy(+Spread), type, damage, knockback, player.whoAmI);

            // Still also fire the center laser
            return true;
        }
    }
}
