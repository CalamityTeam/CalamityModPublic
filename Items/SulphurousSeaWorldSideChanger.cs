using CalamityMod.Events;
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
            Item.ResearchUnlockCount = 0;
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
            CalamityNetcode.SyncWorld();
            string key = "Mods.CalamityMod.Misc.SulphurSwitchLeft";
            if (Abyss.AtLeftSideOfWorld)
            {
                Abyss.AtLeftSideOfWorld = false;
                key = "Mods.CalamityMod.Misc.SulphurSwitchRight";
            }
            else
            {
                Abyss.AtLeftSideOfWorld = true;
            }
            CalamityUtils.DisplayLocalizedText(key, AcidRainEvent.TextColor);
            return true;
        }
    }
}
