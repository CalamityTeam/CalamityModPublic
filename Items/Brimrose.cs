using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items
{
    class Brimrose : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimrose");
            Tooltip.SetDefault("Summons a brimrose mount");
        }

        public override void SetDefaults()
        {
            item.width = 64;
            item.height = 64;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = 1;
			item.value = Item.buyPrice(1, 50, 0, 0);
			item.rare = 10;
			item.expert = true;
            item.UseSound = SoundID.Item3;
            item.noMelee = true;
            item.mountType = mod.MountType("PhuppersChair");
        }
    }
}
