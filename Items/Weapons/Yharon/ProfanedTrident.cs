using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.Yharon
{
    public class ProfanedTrident : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Infernal Spear");
        }

        public override void SetDefaults()
        {
            item.width = 72;
            item.damage = 1040;
            item.thrown = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 13;
            item.useStyle = 1;
            item.useTime = 13;
            item.knockBack = 8f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 72;
            item.maxStack = 1;
            item.value = 10000000;
            item.shoot = mod.ProjectileType("ProfanedTrident");
            item.shootSpeed = 28f;
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
    }
}
