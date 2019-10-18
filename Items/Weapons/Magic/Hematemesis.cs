using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Hematemesis : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hematemesis");
            Tooltip.SetDefault("Casts a barrage of blood geysers from below");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 90;
            item.magic = true;
            item.mana = 14;
            item.rare = 8;
            item.width = 48;
            item.height = 54;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 3.75f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.UseSound = SoundID.Item21;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<BloodBlast>();
            item.shootSpeed = 10f;
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            position = Main.MouseWorld;
            for (int x = 0; x < 10; x++)
            {
                Projectile.NewProjectile(position.X + (float)Main.rand.Next(-150, 150), position.Y + 600f, 0f, -10f, type, damage, knockBack, Main.myPlayer, 0f, 0f);
            }
            return false;
        }
    }
}
