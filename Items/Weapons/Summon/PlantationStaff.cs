using CalamityMod.Projectiles.Summon;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class PlantationStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plantation Staff");
            Tooltip.SetDefault("Summons a miniature plantera to protect you\n" +
			"Fires seeds and spiky balls from afar to poison targets\n" +
			"Enrages when you get under 50% health and begins ramming enemies\n" +
			"Occupies 2 minion slots and there can only be one");
        }

        public override void SetDefaults()
        {
            item.damage = 60;
            item.mana = 10;
            item.width = 66;
            item.height = 70;
            item.useTime = 36;
            item.useAnimation = 36;
            item.useStyle = 1;
            item.noMelee = true;
            item.knockBack = 1f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
            item.UseSound = SoundID.Item76;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<PlantSummon>();
            item.shootSpeed = 10f;
            item.summon = true;
        }

        public override bool CanUseItem(Player player)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.type == ModContent.ProjectileType<PlantSummon>() && p.owner == player.whoAmI)
                {
                    return false;
                }
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<LivingShard>(), 10);
            recipe.AddIngredient(ModContent.ItemType<BlightedEyeStaff>());
            recipe.AddIngredient(ModContent.ItemType<DeepseaStaff>());
            recipe.AddIngredient(ItemID.OpticStaff);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2)
            {
				int i = Main.myPlayer;
				float num72 = item.shootSpeed;
				float num74 = knockBack;
				num74 = player.GetWeaponKnockback(item, num74);
				player.itemTime = item.useTime;
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
					num80 = num72;
				}
				else
				{
					num80 = num72 / num80;
				}
				num78 *= num80;
				num79 *= num80;
				vector2.X = (float)Main.mouseX + Main.screenPosition.X;
				vector2.Y = (float)Main.mouseY + Main.screenPosition.Y;
				Vector2 spinningpoint = new Vector2(num78, num79);
				spinningpoint = spinningpoint.RotatedBy(1.5707963705062866, default);
				Projectile.NewProjectile(vector2.X + spinningpoint.X, vector2.Y + spinningpoint.Y, spinningpoint.X, spinningpoint.Y, type, damage, num74, i, 0f, 0f);
			}
			return false;
        }
    }
}
