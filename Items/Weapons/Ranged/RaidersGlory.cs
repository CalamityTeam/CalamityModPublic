using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class RaidersGlory : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Raider's Glory");
            Tooltip.SetDefault("Fires ichor arrows");
        }

        public override void SetDefaults()
        {
            item.damage = 40;
            item.ranged = true;
            item.crit += 10;
            item.width = 58;
            item.height = 22;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 2.25f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.UseSound = SoundID.Item5;
            item.autoReuse = true;
            item.shoot = 10;
            item.shootSpeed = 15f;
            item.useAmmo = 40;
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int projectile = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ProjectileID.IchorArrow, damage, knockBack, player.whoAmI, 0f, 0f);
            Main.projectile[projectile].extraUpdates++;
            return false;
        }
    }
}
