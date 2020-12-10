using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class GoldenEagle : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Golden Eagle");
            Tooltip.SetDefault("Fires 5 bullets at once");
        }

        public override void SetDefaults()
        {
            item.damage = 36;
            item.ranged = true;
            item.noMelee = true;
            item.width = 46;
            item.height = 30;
            item.useTime = 10;
            item.useAnimation = 10;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 3f;
			item.value = CalamityGlobalItem.Rarity11BuyPrice;
			item.rare = ItemRarityID.Purple;
			item.UseSound = SoundID.Item41;
            item.autoReuse = true;
            item.shoot = ProjectileID.PurificationPowder;
            item.shootSpeed = 20f;
            item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float SpeedX = speedX + 5f * 0.05f;
            float SpeedY = speedY + 5f * 0.05f;
            float SpeedX2 = speedX - 5f * 0.05f;
            float SpeedY2 = speedY - 5f * 0.05f;
            float SpeedX3 = speedX + 0f * 0.05f;
            float SpeedY3 = speedY + 0f * 0.05f;
            float SpeedX4 = speedX - 10f * 0.05f;
            float SpeedY4 = speedY - 10f * 0.05f;
            float SpeedX5 = speedX + 10f * 0.05f;
            float SpeedY5 = speedY + 10f * 0.05f;
            Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            Projectile.NewProjectile(position.X, position.Y, SpeedX2, SpeedY2, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            Projectile.NewProjectile(position.X, position.Y, SpeedX3, SpeedY3, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            Projectile.NewProjectile(position.X, position.Y, SpeedX4, SpeedY4, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            Projectile.NewProjectile(position.X, position.Y, SpeedX5, SpeedY5, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            return false;
        }
    }
}
