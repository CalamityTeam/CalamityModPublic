using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class GoldenEagle : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        private const float Spread = 0.0425f;

        public override void SetDefaults()
        {
            Item.damage = 85;
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;
            Item.width = 46;
            Item.height = 30;
            Item.useTime = 19;
            Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 3f;
            Item.value = CalamityGlobalItem.Rarity11BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item41;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 20f;
            Item.useAmmo = AmmoID.Bullet;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Fire extra bullets to the left and right
            for (int i = 0; i < 2; i++)
            {
                Projectile.NewProjectile(source, position, velocity.RotatedBy(-Spread * (i + 1)), type, damage, knockback, player.whoAmI);
                Projectile.NewProjectile(source, position, velocity.RotatedBy(Spread * (i + 1)), type, damage, knockback, player.whoAmI);
            }

            return true;
        }
    }
}
