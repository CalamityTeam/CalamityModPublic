using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.DifficultyItems
{
    public class MLGRune : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Demon Trophy");
            Tooltip.SetDefault("Boosts spawn rate by 1.25 times\n" +
                               "Effects cannot be reversed");
        }

        public override void SetDefaults()
        {
            item.width = item.height = 54;
            item.maxStack = 99;
            item.rare = ItemRarityID.Blue;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item119;
            item.consumable = true;
        }

        public override bool CanUseItem(Player player)
        {
            return !CalamityWorld.demonMode;
        }

        public override bool UseItem(Player player)
        {
            // This world syncing code should only be run by one entity- the server, to prevent a race condition
            // with the packets.
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return true;

            CalamityWorld.demonMode = true;
            CalamityNetcode.SyncWorld();
            return true;
        }
    }
}
