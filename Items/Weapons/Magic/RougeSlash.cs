using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class RougeSlash : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rouge Slash");
            Tooltip.SetDefault("Fires a wave of 3 rouge air slashes");
        }

        public override void SetDefaults()
        {
            item.damage = 61;
            item.magic = true;
            item.mana = 30;
            item.width = 28;
            item.height = 32;
            item.useTime = 19;
            item.useAnimation = 19;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 7.5f;
            item.UseSound = SoundID.Item91;
            item.value = CalamityGlobalItem.Rarity11BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<RougeSlashLarge>();
            item.shootSpeed = 24f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<RougeSlashLarge>(), damage, knockBack, player.whoAmI);
            Projectile.NewProjectile(position.X, position.Y, speedX * 0.8f, speedY * 0.8f, ModContent.ProjectileType<RougeSlashMedium>(), damage, knockBack, player.whoAmI);
            Projectile.NewProjectile(position.X, position.Y, speedX * 0.6f, speedY * 0.6f, ModContent.ProjectileType<RougeSlashSmall>(), damage / 2, knockBack, player.whoAmI);
            return false;
        }
    }
}
