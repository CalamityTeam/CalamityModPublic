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
            item.width = 28;
            item.height = 28;
            item.maxStack = 99;
            item.rare = 1;
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
            CalamityWorld.demonMode = true;
            CalamityNetcode.SyncWorld();
            if (Main.netMode == NetmodeID.Server)
            {
                var netMessage = mod.GetPacket();
                netMessage.Write((byte)CalamityModMessageType.DemonTrophyBoolSync);
                netMessage.Write(CalamityWorld.demonMode);
                netMessage.Send();
            }
            return true;
        }
    }
}
