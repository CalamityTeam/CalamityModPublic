using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class TelluricGlare : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Telluric Glare");
            Tooltip.SetDefault("Shoots an extremely fast energy arrow");
        }

        public override void SetDefaults()
        {
            item.damage = 70;
            item.ranged = true;
            item.width = 54;
            item.height = 92;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4f;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item5;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<TelluricGlareProj>();
            item.shootSpeed = 12f;
            item.useAmmo = AmmoID.Arrow;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<TelluricGlareProj>(), damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }
    }
}
