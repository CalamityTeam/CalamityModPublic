using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems.Invasion
{
    public class MartianDistressBeacon : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Martian Distress Beacon");
            Tooltip.SetDefault("Summons the Martian Madness\n" +
                "Not consumable");
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 60;
            Item.maxStack = 1;
            Item.rare = ItemRarityID.Yellow;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player) => Main.invasionType == InvasionID.None;

        public override bool? UseItem(Player player)
        {
            Main.StartInvasion(InvasionID.MartianMadness);
            return true;
        }
    }
}
