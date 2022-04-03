using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Weapons.Magic
{
    public class PurgeGuzzler : ModItem
    {
        private const float Spread = 0.025f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Purge Guzzler");
            Tooltip.SetDefault("Emits three beams of holy energy in a tight spread");
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
            Item.UseSound = Mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon");
            Item.shoot = ModContent.ProjectileType<HolyLaser>();
            Item.shootSpeed = 6f;

            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 velocity = new Vector2(speedX, speedY);

            // Fire extra lasers to the left and right
            Projectile.NewProjectile(position, velocity.RotatedBy(-Spread), type, damage, knockBack, player.whoAmI);
            Projectile.NewProjectile(position, velocity.RotatedBy(+Spread), type, damage, knockBack, player.whoAmI);

            // Still also fire the center laser
            return true;
        }
    }
}
