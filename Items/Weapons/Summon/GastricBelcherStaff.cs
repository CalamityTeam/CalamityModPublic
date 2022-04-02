using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
	public class GastricBelcherStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gastric Belcher Staff");
            Tooltip.SetDefault("Summons aquatic aberrations to protect you\n" +
			"Aberrations fire vomit at nearby enemies with every third attack firing bubbles");
        }

        public override void SetDefaults()
        {
            item.damage = 60;
            item.mana = 10;
            item.width = 66;
            item.height = 70;
            item.useTime = item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 1f;
            item.value = CalamityGlobalItem.Rarity7BuyPrice;
            item.rare = ItemRarityID.Lime;
            item.UseSound = SoundID.Item76;
            item.shoot = ModContent.ProjectileType<GastricBelcher>();
            item.shootSpeed = 10f;
            item.summon = true;
			item.autoReuse = true;
        }

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2)
            {
				player.itemTime = item.useTime;
				Vector2 playerPos = player.RotatedRelativePoint(player.MountedCenter, true);
				float directionX = Main.mouseX + Main.screenPosition.X - playerPos.X;
				float directionY = Main.mouseY + Main.screenPosition.Y - playerPos.Y;
				if (player.gravDir == -1f)
				{
					directionY = Main.screenPosition.Y + Main.screenHeight - Main.mouseY - playerPos.Y;
				}
				Vector2 spinningpoint = new Vector2(directionX, directionY);
				float dist = spinningpoint.Length();
				if ((float.IsNaN(spinningpoint.X) && float.IsNaN(spinningpoint.Y)) || (spinningpoint.X == 0f && spinningpoint.Y == 0f))
				{
					spinningpoint.X = player.direction;
					spinningpoint.Y = 0f;
					dist = item.shootSpeed;
				}
				else
				{
					dist = item.shootSpeed / dist;
				}
				spinningpoint.X *= dist;
				spinningpoint.Y *= dist;
				playerPos.X = Main.mouseX + Main.screenPosition.X;
				playerPos.Y = Main.mouseY + Main.screenPosition.Y;
				spinningpoint = spinningpoint.RotatedBy(Math.PI / 2D, default);
				Projectile.NewProjectile(playerPos + spinningpoint, spinningpoint, type, damage, knockBack, player.whoAmI, 0f, 1f);
			}
			return false;
        }
    }
}
