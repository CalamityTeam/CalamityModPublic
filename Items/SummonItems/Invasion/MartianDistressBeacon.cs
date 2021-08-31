using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems.Invasion
{
    public class MartianDistressBeacon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Martian Distress Beacon");
            Tooltip.SetDefault("Summons the Martian Madness");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 60;
            item.maxStack = 99;
            item.rare = ItemRarityID.Yellow;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.consumable = true;
        }

        public override bool CanUseItem(Player player) => Main.invasionType == InvasionID.None;

        public override bool UseItem(Player player)
        {
            Main.StartInvasion(InvasionID.MartianMadness);
            return true;
        }
    }
}
