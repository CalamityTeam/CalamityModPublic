using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons
{
    public class Warblade : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Warblade");
        }

        public override void SetDefaults()
        {
            item.damage = 27;
            item.melee = true;
            item.width = 32;
            item.height = 48;
            item.useTime = 19;
            item.useAnimation = 19;
            item.useTurn = true;
            item.useStyle = 1;
            item.knockBack = 4.25f;
            item.value = 50000;
            item.rare = 1;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
        }
    }
}