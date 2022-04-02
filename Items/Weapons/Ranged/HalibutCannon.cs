using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class HalibutCannon : ModItem
    {        
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Halibut Cannon");
            Tooltip.SetDefault("Yes, it's still overpowered");
        }

        public override void SetDefaults()
        {
            item.damage = 50;
            item.ranged = true;
            item.width = 108;
            item.height = 54;
            item.useTime = 10;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.rare = ItemRarityID.Red;
            item.noMelee = true;
            item.knockBack = 1f;
            item.value = Item.buyPrice(1, 0, 0, 0);
            item.UseSound = null;
            item.autoReuse = true;
            item.shoot = ProjectileID.Bullet;
            item.shootSpeed = 12f;
            item.useAmmo = AmmoID.Bullet;
            item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-15, 0);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Main.PlaySound(SoundID.Item38, (int)player.Center.X, (int)player.Center.Y);

            int bulletAmt = Main.rand.Next(25, 36);
            for (int index = 0; index < bulletAmt; ++index)
            {
                float SpeedX = speedX + Main.rand.Next(-10, 11) * 0.05f;
                float SpeedY = speedY + Main.rand.Next(-10, 11) * 0.05f;
                int shot = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI);
                Main.projectile[shot].timeLeft = 120;
            }

            return false;
        }
    }
}
