using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class IcicleTrident : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Icicle Trident");
            Tooltip.SetDefault("Shoots piercing icicles");
            Item.staff[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.damage = 69;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 21;
            Item.width = Item.height = 44;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTurn = false;
            Item.noMelee = true;
            Item.knockBack = 7f;
            Item.value = Item.buyPrice(0, 36, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item8;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<TridentIcicle>();
            Item.shootSpeed = 12f;
        }

        public override Vector2? HoldoutOrigin()
        {
            return new Vector2(15, 15);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 speed = new Vector2(speedX, speedY);
            Projectile.NewProjectile(position, speed, type, damage, knockBack, player.whoAmI);
            Projectile.NewProjectile(position, speed.RotatedBy(MathHelper.ToRadians(5)), type, damage, knockBack, player.whoAmI);
            Projectile.NewProjectile(position, speed.RotatedBy(MathHelper.ToRadians(-5)), type, damage, knockBack, player.whoAmI);
            return false;
        }
    }
}
