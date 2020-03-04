using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class EternalBlizzard : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eternal Blizzard");
            Tooltip.SetDefault("Fires an additional icicle arrow that shatters on impact");
        }
        public override void SetDefaults()
        {
            item.damage = 38;
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
            item.shoot = ModContent.ProjectileType<IcicleArrowProj>();
            item.shootSpeed = 11f;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			float SpeedX = speedX + (float)Main.rand.Next(-10, 11) * 0.05f;
			float SpeedY = speedY + (float)Main.rand.Next(-10, 11) * 0.05f;
			int index = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<IcicleArrowProj>(), (int)(damage * 0.5f), knockBack, player.whoAmI, 0f, 0f);
			Main.projectile[index].noDropItem = true;

            return true;
        }
    }
}
