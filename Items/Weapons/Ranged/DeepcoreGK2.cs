using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class DeepcoreGK2 : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";

        public override void SetDefaults()
        {
            Item.damage = 45;
            Item.ArmorPenetration = 15;
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;
            Item.width = 142;
            Item.height = 64;
            Item.useTime = Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 7f;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item38;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 20f;
            Item.useAmmo = AmmoID.Bullet;
            Item.Calamity().donorItem = true;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-20, 0);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 shootDirection = velocity.SafeNormalize(Vector2.UnitX * player.direction);
            Vector2 gunTip = position + shootDirection * Item.scale * 120f;
            gunTip.Y -= 18f;

            int p = Projectile.NewProjectile(source, gunTip, velocity, type, damage, knockback, player.whoAmI);
            if (p.WithinBounds(Main.maxProjectiles))
            {
                Main.projectile[p].Calamity().deepcoreBullet = true;
                Main.projectile[p].scale = 2;
            }
            return false;
        }
    }
}
