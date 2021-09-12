using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Starfall : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Starfall");
            Tooltip.SetDefault("Casts a spread of astral stars at the mouse cursor");
        }

        public override void SetDefaults()
        {
            item.damage = 50;
            item.magic = true;
            item.mana = 15;
            item.rare = ItemRarityID.Cyan;
            item.width = 38;
            item.height = 40;
            item.useTime = 14;
            item.useAnimation = 14;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 3.25f;
            item.value = Item.buyPrice(0, 95, 0, 0);
            item.UseSound = SoundID.Item105;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<AstralStarMagic>();
            item.shootSpeed = 12f;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 25;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float num72 = item.shootSpeed;
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            float num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
            float num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
            if (player.gravDir == -1f)
            {
                num79 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector2.Y;
            }
            if ((float.IsNaN(num78) && float.IsNaN(num79)) || (num78 == 0f && num79 == 0f))
            {
                num78 = (float)player.direction;
                num79 = 0f;
            }
            float num208 = num78;
            float num209 = num79;
            num208 += (float)Main.rand.Next(-1, 2) * 0.5f;
            num209 += (float)Main.rand.Next(-1, 2) * 0.5f;
            vector2 += new Vector2(num208, num209);
            for (int num108 = 0; num108 < 5; num108++)
            {
                float speedX4 = 2f + (float)Main.rand.Next(-8, 5);
                float speedY5 = 15f + (float)Main.rand.Next(1, 6);
                int star = Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, speedY5, type, damage, knockBack, player.whoAmI, 0f, 0f);
            }
            return false;
        }
    }
}
