using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class TelluricGlare : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Telluric Glare");
            Tooltip.SetDefault("Fires volleys of four colossal radiant arrows which can pass through walls");
        }

        public override void SetDefaults()
        {
            item.damage = 176;
            item.ranged = true;
            item.width = 54;
            item.height = 92;
            item.useTime = 5;
            item.useAnimation = 20;
            item.reuseDelay = 23;
            item.knockBack = 7.5f;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;

            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Turquoise;

            item.UseSound = SoundID.Item102;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<TelluricGlareArrow>();
            item.shootSpeed = 18f;
            item.useAmmo = AmmoID.Arrow;
            item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-14f, 0f);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            // Always fires Radiant Arrows regardless of ammo used
            type = item.shoot;

            // The arrow appears from a random location "on the bow".
            // They are also moved backwards so that they have some time to build up past positions. This helps make them not appear out of thin air.
            Vector2 velocity = new Vector2(speedX, speedY);
            Vector2 offset = Vector2.Normalize(velocity.RotatedBy(MathHelper.PiOver2));
            position += offset * Main.rand.NextFloat(-19f, 19f);
            position -= 3f * velocity;
            return true;
        }
    }
}
