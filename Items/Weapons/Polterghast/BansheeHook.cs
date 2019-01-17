using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.Polterghast
{
    public class BansheeHook : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Banshee Hook");
            Tooltip.SetDefault("Swings a banshee hook that explodes on contact");
        }

        public override void SetDefaults()
        {
            item.width = 16;
            item.damage = 155;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.melee = true;
            item.useAnimation = 21;
            item.useStyle = 5;
            item.useTime = 21;
            item.knockBack = 8.5f;
            item.UseSound = SoundID.DD2_GhastlyGlaivePierce;
            item.autoReuse = true;
            item.height = 16;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.shoot = mod.ProjectileType("BansheeHook");
            item.shootSpeed = 42f;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(0, 255, 0);
                }
            }
        }

        public override bool CanUseItem(Player player)
        {
            for (int i = 0; i < 1000; ++i)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == item.shoot)
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float num82 = (float)Main.mouseX + Main.screenPosition.X - position.X;
            float num83 = (float)Main.mouseY + Main.screenPosition.Y - position.Y;
            if (player.gravDir == -1f)
            {
                num83 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - position.Y;
            }
            float num84 = (float)Math.Sqrt((double)(num82 * num82 + num83 * num83));
            float num85 = num84;
            if ((float.IsNaN(num82) && float.IsNaN(num83)) || (num82 == 0f && num83 == 0f))
            {
                num82 = (float)player.direction;
                num83 = 0f;
                num84 = item.shootSpeed;
            }
            else
            {
                num84 = item.shootSpeed / num84;
            }
            num82 *= num84;
            num83 *= num84;
            float ai4 = Main.rand.NextFloat() * item.shootSpeed * 0.75f * (float)player.direction;
            Vector2 velocity = new Vector2(num82, num83);
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("BansheeHook"), damage, knockBack, player.whoAmI, ai4, 0.0f);
            return false;
        }
    }
}
