using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Magic
{
    public class RougeSlash : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rouge Slash");
            Tooltip.SetDefault("Fires a wave of 3 rouge air slashes");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 61;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 30;
            Item.width = 28;
            Item.height = 32;
            Item.useTime = 19;
            Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7.5f;
            Item.UseSound = SoundID.Item91;
            Item.value = CalamityGlobalItem.Rarity11BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<RougeSlashLarge>();
            Item.shootSpeed = 24f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<RougeSlashLarge>(), damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, position.X, position.Y, velocity.X * 0.8f, velocity.Y * 0.8f, ModContent.ProjectileType<RougeSlashMedium>(), damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, position.X, position.Y, velocity.X * 0.6f, velocity.Y * 0.6f, ModContent.ProjectileType<RougeSlashSmall>(), damage / 2, knockback, player.whoAmI);
            return false;
        }
    }
}
