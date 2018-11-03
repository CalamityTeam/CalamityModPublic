using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;
//using TerrariaOverhaul;

namespace CalamityMod.Items.Weapons
{
    public class RaidersGlory : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Raider's Glory");
        }

        public override void SetDefaults()
        {
            item.damage = 37;
            item.ranged = true;
            item.crit += 10;
            item.width = 58;
            item.height = 22;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 2.25f;
            item.value = 100000;
            item.rare = 5;
            item.UseSound = SoundID.Item5;
            item.autoReuse = true;
            item.shoot = 10;
            item.shootSpeed = 15f;
            item.useAmmo = 40;
        }

        /*public void OverhaulInit()
        {
            this.SetTag("crossbow");
        }*/
    }
}