using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Shellshooter : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shellshooter");
            Tooltip.SetDefault("Converts wooden arrows into slow, powerful shells");
        }

        public override void SetDefaults()
        {
            item.damage = 33;
            item.ranged = true;
            item.width = 30;
            item.height = 38;
            item.useTime = 70;
            item.useAnimation = 70;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6f;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item5;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<Shell>();
            item.shootSpeed = 1.5f;
            item.useAmmo = AmmoID.Arrow;
            item.Calamity().canFirePointBlankShots = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 15;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (type == ProjectileID.WoodenArrowFriendly)
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<Shell>(), damage, knockBack, player.whoAmI);
            else
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);

            return false;
        }
    }
}
