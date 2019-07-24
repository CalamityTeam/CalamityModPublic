using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items
{
    public class CharredRelic : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Charred Relic");
            Tooltip.SetDefault("Contains a small amount of brimstone");
        }

        public override void SetDefaults()
        {
            item.shoot = mod.ProjectileType("Brimgling");
            item.buffType = mod.BuffType("BrimlingBuff");
			item.rare = 4;
			item.UseSound = SoundID.NPCHit51;
        }

        public override void UseStyle(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(item.buffType, 3600, true);
            }
        }
    }
}