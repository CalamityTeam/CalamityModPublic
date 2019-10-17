using Terraria;
using CalamityMod.Projectiles;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items
{
    public class Downpour : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Downpour");
            Tooltip.SetDefault("Fires a spray of water that drips extra trails of water");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 43;
            item.magic = true;
            item.mana = 10;
            item.width = 42;
            item.height = 42;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 3f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.UseSound = SoundID.Item13;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<WaterStream>();
            item.shootSpeed = 14f;
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            return false;
        }
    }
}
