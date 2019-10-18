using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class EternalBlizzard : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eternal Blizzard");
            Tooltip.SetDefault("Wooden arrows turn into icicle arrows that shatter on impact");
        }
        public override void SetDefaults()
        {
            item.damage = 48;
            item.ranged = true;
            item.width = 42;
            item.height = 62;
            item.useTime = 19;
            item.useAnimation = 19;
            item.useStyle = 5;
            item.useTurn = false;
            item.noMelee = true;
            item.knockBack = 3f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.useAmmo = AmmoID.Arrow;
            item.UseSound = SoundID.Item5;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<Projectiles.IcicleArrow>();
            item.shootSpeed = 11f;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (type == ProjectileID.WoodenArrowFriendly)
                type = ModContent.ProjectileType<Projectiles.IcicleArrow>();

            return true;
        }
    }
}
