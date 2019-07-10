using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.Astral
{
    public class AstralachneaStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astralachnea Staff");
            Tooltip.SetDefault("Fires a spread of homing astral spider fangs");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 60;
            item.magic = true;
            item.mana = 19;
            item.width = 52;
            item.height = 52;
            item.useTime = 21;
            item.useAnimation = 21;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 6f;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 7;
            item.UseSound = SoundID.Item46;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("AstralachneaFang");
            item.shootSpeed = 13f;
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            int i = Main.myPlayer;
            float num72 = item.shootSpeed;
            int num73 = damage;
            float num74 = knockBack;
            float num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
            float num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
            float num80 = (float)Math.Sqrt((double)(num78 * num78 + num79 * num79));
            int spikeAmount = 4;
            if (Main.rand.Next(3) == 0)
            {
                spikeAmount++;
            }
            if (Main.rand.Next(4) == 0)
            {
                spikeAmount++;
            }
            if (Main.rand.Next(5) == 0)
            {
                spikeAmount += 2;
            }
            for (int num131 = 0; num131 < spikeAmount; num131++)
            {
                float num132 = num78;
                float num133 = num79;
                float num134 = 0.05f * (float)num131;
                num132 += (float)Main.rand.Next(-400, 400) * num134;
                num133 += (float)Main.rand.Next(-400, 400) * num134;
                num80 = (float)Math.Sqrt((double)(num132 * num132 + num133 * num133));
                num80 = num72 / num80;
                num132 *= num80;
                num133 *= num80;
                float x2 = vector2.X;
                float y2 = vector2.Y;
                Projectile.NewProjectile(x2, y2, num132, num133, mod.ProjectileType("AstralachneaFang"), num73, num74, i, 0f, 0f);
            }
            return false;
        }
    }
}
