using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class ClamorRifle : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Clamor Rifle");
            Tooltip.SetDefault("Converts musket balls into homing energy bolts");
        }

        public override void SetDefaults()
        {
            item.damage = 23;
            item.ranged = true;
            item.width = 64;
            item.height = 30;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2.5f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/PlasmaBolt");
            item.autoReuse = true;
            item.rare = ItemRarityID.Pink;
            item.shoot = ModContent.ProjectileType<ClamorRifleProj>();
            item.shootSpeed = 15f;
            item.useAmmo = AmmoID.Bullet;
            item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (type == ProjectileID.Bullet)
                Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<ClamorRifleProj>(), damage, knockBack, player.whoAmI);
            else
                Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);

            return false;
        }
    }
}
