using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Barinautical : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Barinautical");
            Tooltip.SetDefault("Converts wooden arrows into strings of 3 electric bolt arrows that explode");
        }

        public override void SetDefaults()
        {
            item.damage = 28;
            item.ranged = true;
            item.width = 30;
            item.height = 42;
            item.useTime = 4;
            item.reuseDelay = 20;
            item.useAnimation = 12;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.UseSound = SoundID.Item5;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<BoltArrow>();
            item.shootSpeed = 15f;
            item.useAmmo = AmmoID.Arrow;
            item.Calamity().canFirePointBlankShots = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (type == ProjectileID.WoodenArrowFriendly)
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<BoltArrow>(), damage, knockBack, player.whoAmI);
            else
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);

            return false;
        }
    }
}
