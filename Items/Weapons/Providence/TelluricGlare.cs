using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;
//using TerrariaOverhaul;

namespace CalamityMod.Items.Weapons.Providence
{
    public class TelluricGlare : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Telluric Glare");
        }

        public override void SetDefaults()
        {
            item.damage = 70;
            item.ranged = true;
            item.width = 54;
            item.height = 92;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 4;
            item.value = 1000000;
            item.UseSound = SoundID.Item5;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("TelluricGlare");
            item.shootSpeed = 12f;
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
                    line2.overrideColor = new Color(0, 255, 200);
                }
            }
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("TelluricGlare"), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            return false;
        }
    }
}