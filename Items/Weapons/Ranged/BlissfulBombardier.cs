using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class BlissfulBombardier : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blissful Bombardier");
            Tooltip.SetDefault("Fires flare rockets");
        }

        public override void SetDefaults()
        {
            item.damage = 75;
            item.ranged = true;
            item.width = 76;
            item.height = 30;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 7.5f;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item11;
            item.autoReuse = true;
            item.shootSpeed = 24f;
            item.shoot = ModContent.ProjectileType<Nuke>();
            item.useAmmo = 771;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<Nuke>(), damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }
    }
}
