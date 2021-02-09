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
            item.width = 58;
            item.height = 44;
            item.damage = 281;
            item.magic = true;
            item.mana = 22;
            item.useTime = 19;
            item.useAnimation = 19;
            item.autoReuse = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4.5f;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon");
            item.shoot = ModContent.ProjectileType<HolyLaser>();
            item.shootSpeed = 6f;

            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
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
