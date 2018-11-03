using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.AbyssItems
{
    public class CloakingGland : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cloaking Gland");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.maxStack = 999;
            item.value = 3000;
            item.rare = 3;
        }
    }
}