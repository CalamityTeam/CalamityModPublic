using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class PestilentDefiler : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pestilent Defiler");
            Tooltip.SetDefault("Fires a plague round that explodes and splits on death");
        }

        public override void SetDefaults()
        {
            item.damage = 135;
            item.ranged = true;
            item.width = 46;
            item.height = 20;
            item.useTime = 37;
            item.useAnimation = 37;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 9.5f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = ItemRarityID.Yellow;
            item.UseSound = SoundID.Item40;
            item.autoReuse = true;
            item.shootSpeed = 20f;
            item.shoot = ModContent.ProjectileType<SicknessRound>();
            item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(0, -5);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<SicknessRound>(), damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }
    }
}
