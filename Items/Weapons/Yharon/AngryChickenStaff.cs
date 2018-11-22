using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.GameContent;
using Terraria.IO;
using Terraria.ObjectData;
using Terraria.Utilities;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.Yharon
{
    public class AngryChickenStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Yharon's Kindle Staff");
            Tooltip.SetDefault("Summons the Son of Yharon to fight for you\n" +
                               "The dragon increases your life regen, defense, and movement speed while summoned\n" +
                               "The dragon imbues your melee attacks with Holy Flames while summoned\n" +
                               "Requires 4 minion slots to use");
        }

        public override void SetDefaults()
        {
            item.mana = 50;
            item.damage = 232;
            item.useStyle = 1;
            item.width = 32;
            item.height = 32;
            item.useTime = 36;
            item.useAnimation = 36;
            item.noMelee = true;
            item.knockBack = 7f;
            item.value = 10000000;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/FlareSound");
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("AngryChicken");
            item.shootSpeed = 10f;
            item.summon = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(43, 96, 222);
                }
            }
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int i = Main.myPlayer;
            float num72 = item.shootSpeed;
            int num73 = damage;
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
            float num81 = num80;
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
            num78 = 0f;
            num79 = 0f;
            vector2.X = (float)Main.mouseX + Main.screenPosition.X;
            vector2.Y = (float)Main.mouseY + Main.screenPosition.Y;
            Projectile.NewProjectile(vector2.X, vector2.Y, num78, num79, mod.ProjectileType("AngryChicken"), num73, num74, i, 0f, 0f);
            return false;
        }
    }
}