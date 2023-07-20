using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class TelluricGlare : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.damage = 216;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 54;
            Item.height = 92;
            Item.useTime = 5;
            Item.useAnimation = 20;
            Item.reuseDelay = 23;
            Item.useLimitPerAnimation = 4;
            Item.knockBack = 7.5f;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;

            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();

            Item.UseSound = SoundID.Item102;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<TelluricGlareArrow>();
            Item.shootSpeed = 18f;
            Item.useAmmo = AmmoID.Arrow;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-14f, 0f);

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            // Always fires Radiant Arrows regardless of ammo used
            type = Item.shoot;

            // The arrow appears from a random location "on the bow".
            // They are also moved backwards so that they have some time to build up past positions. This helps make them not appear out of thin air.

            Vector2 offset = Vector2.Normalize(velocity.RotatedBy(MathHelper.PiOver2));
            position += offset * Main.rand.NextFloat(-19f, 19f);
            position -= 3f * velocity;
        }
    }
}
