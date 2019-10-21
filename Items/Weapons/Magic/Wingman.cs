using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Wingman : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wingman");
            Tooltip.SetDefault("Fires a concentrated laser beam");
        }

        public override void SetDefaults()
        {
            item.damage = 54;
            item.magic = true;
            item.mana = 12;
            item.width = 42;
            item.height = 22;
            item.useTime = 12;
            item.useAnimation = 12;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 3f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
            item.UseSound = SoundID.Item33;
            item.autoReuse = true;
            item.shootSpeed = 25f;
            item.shoot = 440;
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int num6 = 3;
            for (int index = 0; index < num6; ++index)
            {
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, 440, damage, knockBack, player.whoAmI, 0f, 0f);
            }
            return false;
        }
    }
}
