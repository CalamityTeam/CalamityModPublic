using CalamityMod.Rarities;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class SulphurousSeaWorldSideChanger : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sulphurous Sea World Side Changer");
            Tooltip.SetDefault("Spawn and use this if your regular ocean counts as a Sulphurous Sea\n"+"This will also fix the Abyss if it is broken");
        }

        public override void SetDefaults()
        {
            Item.width = 54;
            Item.height = 46;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.value = 0;
            Item.autoReuse = false;
            Item.useStyle = 4;
            Item.UseSound = SoundID.Item111;
        }

        public override bool? UseItem(Player player)
        {
            if (Abyss.AtLeftSideOfWorld)
            {
                Abyss.AtLeftSideOfWorld = false;
            }
            else
            {
                Abyss.AtLeftSideOfWorld = true;
            }
            return true;
        }
    }
}
