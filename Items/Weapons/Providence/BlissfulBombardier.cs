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
    public class BlissfulBombardier : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blissful Bombardier");
            Tooltip.SetDefault("Fires flare rockets");
        }

        public override void SetDefaults()
        {
            item.damage = 85;
            item.ranged = true;
            item.width = 76;
            item.height = 30;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 7.5f;
            item.value = 5000000;
            item.UseSound = SoundID.Item11;
            item.autoReuse = true;
            item.shootSpeed = 24f;
            item.shoot = mod.ProjectileType("Nuke");
            item.useAmmo = 771;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
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

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("Nuke"), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            return false;
        }
    }
}