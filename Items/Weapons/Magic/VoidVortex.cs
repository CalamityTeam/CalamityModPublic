using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class VoidVortex : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Void Vortex");
            Tooltip.SetDefault("Fires a circular spread of magnetic orbs around the mouse cursor");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 195;
            item.magic = true;
            item.mana = 60;
            item.width = 130;
            item.height = 130;
            item.useTime = 41;
            item.useAnimation = 41;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 0f;
			item.value = CalamityGlobalItem.Rarity15BuyPrice;
			item.Calamity().customRarity = CalamityRarity.Violet;
			item.rare = ItemRarityID.Purple;
            item.UseSound = SoundID.Item20;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<Climax2>();
            item.shootSpeed = 12f;
			item.Calamity().challengeDrop = true;
		}

        public override Vector2? HoldoutOrigin()
        {
            return new Vector2(30, 30);
        }

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
            float num80 = (float)Math.Sqrt((double)(num78 * num78 + num79 * num79));
            if ((float.IsNaN(num78) && float.IsNaN(num79)) || (num78 == 0f && num79 == 0f))
            {
                num78 = (float)player.direction;
                num79 = 0f;
            }
            else
            {
                num80 = num72 / num80;
            }
            vector2 += new Vector2(num78, num79);
            float spread = 45f * 0.0174f;
            double startAngle = Math.Atan2(speedX, speedY) - spread / 2;
            double deltaAngle = spread / 8f;
            double offsetAngle;
            int i;
            float passedVar = 1f;
            for (i = 0; i < 4; i++)
            {
                offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                Projectile.NewProjectile(vector2.X, vector2.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), type, damage, knockBack, player.whoAmI, passedVar, 0f);
                Projectile.NewProjectile(vector2.X, vector2.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), type, damage, knockBack, player.whoAmI, -passedVar, 0f);
                passedVar += 1f;
            }
            return false;
        }
    }
}
