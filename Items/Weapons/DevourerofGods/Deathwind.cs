using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;
//using TerrariaOverhaul;

namespace CalamityMod.Items.Weapons.DevourerofGods
{
    public class Deathwind : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deathwind");
        }

        public override void SetDefaults()
        {
            item.damage = 200;
            item.ranged = true;
            item.width = 38;
            item.height = 66;
            item.useTime = 14;
            item.useAnimation = 14;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 5;
            item.value = 1250000;
            item.UseSound = SoundID.Item5;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("NebulaShot");
            item.shootSpeed = 20f;
            item.useAmmo = 40;
        }

        /*public void OverhaulInit()
        {
            this.SetTag("bow");
        }*/

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

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float SpeedA = speedX;
            float SpeedB = speedY;
            int num6 = Main.rand.Next(4, 6);
            for (int index = 0; index < num6; ++index)
            {
                float num7 = speedX;
                float num8 = speedY;
                float SpeedX = speedX + (float)Main.rand.Next(-20, 21) * 0.05f;
                float SpeedY = speedY + (float)Main.rand.Next(-20, 21) * 0.05f;
                if (type == ProjectileID.WoodenArrowFriendly)
                {
                    if (Main.rand.Next(3) == 0)
                    {
                        Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, mod.ProjectileType("IceBeam"), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
                    }
                    else
                    {
                        Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, mod.ProjectileType("NebulaShot"), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
                    }
                }
                else
                {
                    int num121 = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
                    Main.projectile[num121].noDropItem = true;
                }
            }
            return false;
        }
    }
}