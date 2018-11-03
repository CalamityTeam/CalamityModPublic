using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.Providence
{
    public class HolyCollider : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Collider");
            Tooltip.SetDefault("Striking enemies will cause them to explode into holy fire");
        }

        public override void SetDefaults()
        {
            item.width = 94;
            item.damage = 400;
            item.melee = true;
            item.useAnimation = 22;
            item.useStyle = 1;
            item.useTime = 22;
            item.useTurn = true;
            item.knockBack = 7.75f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 80;
            item.maxStack = 1;
            item.value = 5000000;
            item.shootSpeed = 10f;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(0, 255, 200);
                }
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            float spread = 45f * 0.0174f;
            double startAngle = Math.Atan2(item.shootSpeed, item.shootSpeed) - spread / 2;
            double deltaAngle = spread / 8f;
            double offsetAngle;
            int i;
            for (i = 0; i < 4; i++)
            {
                offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i;
                int projectile1 = Projectile.NewProjectile(target.Center.X, target.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), mod.ProjectileType("HolyFire2"), (int)((double)damage * 0.25), knockback, Main.myPlayer);
                int projectile2 = Projectile.NewProjectile(target.Center.X, target.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), mod.ProjectileType("HolyFire2"), (int)((double)damage * 0.25), knockback, Main.myPlayer);
                Main.projectile[projectile1].hostile = false;
                Main.projectile[projectile1].friendly = true;
                Main.projectile[projectile1].timeLeft = 60;
                Main.projectile[projectile2].hostile = false;
                Main.projectile[projectile2].friendly = true;
                Main.projectile[projectile2].timeLeft = 60;
            }
        }
    }
}
