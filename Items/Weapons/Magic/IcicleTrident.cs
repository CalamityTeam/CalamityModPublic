using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Magic
{
    public class IcicleTrident : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Icicle Trident");
            Tooltip.SetDefault("Shoots piercing icicles");
            Item.staff[Item.type] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
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

        public override Vector2? HoldoutOrigin() => new Vector2(15, 15);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 speed = velocity;
            Projectile.NewProjectile(source, position, speed, type, damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, speed.RotatedBy(MathHelper.ToRadians(5)), type, damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, speed.RotatedBy(MathHelper.ToRadians(-5)), type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}
