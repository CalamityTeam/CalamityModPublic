using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.DevourerofGods
{
    public class EradicatorMelee : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eradicator");
        }

        public override void SetDefaults()
        {
            item.width = 38;
            item.damage = 225;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.autoReuse = true;
            item.useAnimation = 19;
            item.useStyle = 1;
            item.useTime = 19;
            item.knockBack = 7f;
            item.UseSound = SoundID.Item1;
            item.melee = true;
            item.height = 54;
            item.value = 1250000;
            item.shoot = mod.ProjectileType("EradicatorMeleeProjectile");
            item.shootSpeed = 12f;
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
    }
}
