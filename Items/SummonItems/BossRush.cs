using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    public class BossRush : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Terminus");
            Tooltip.SetDefault("A ritualistic artifact, thought to have brought upon The End many millennia ago\n" +
                                "Sealed away in the abyss, far from those that would seek to misuse it\n" +
                                "Activates Boss Rush Mode, using it again will deactivate Boss Rush Mode\n" +
                                "During the Boss Rush, all wires and wired devices will be disabled");
        }

        public override void SetDefaults()
        {
            item.rare = 1;
            item.width = 28;
            item.height = 28;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item123;
            item.consumable = false;
        }

        public override bool UseItem(Player player)
        {
            for (int doom = 0; doom < 200; doom++)
            {
                if (Main.npc[doom].active && Main.npc[doom].boss)
                {
                    Main.npc[doom].active = false;
                    Main.npc[doom].netUpdate = true;
                }
            }
            if (!BossRushEvent.BossRushActive)
            {
                BossRushEvent.BossRushStage = 0;
                BossRushEvent.BossRushActive = true;
                string key = "Mods.CalamityMod.BossRushStartText";
                Color messageColor = Color.LightCoral;
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(Language.GetTextValue(key), messageColor);
                }
                else if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                }
            }
            else
            {
                BossRushEvent.BossRushStage = 0;
                BossRushEvent.BossRushActive = false;
            }

            CalamityMod.UpdateServerBoolean();
            if (Main.netMode == NetmodeID.Server)
            {
                var netMessage = mod.GetPacket();
                netMessage.Write((byte)CalamityModMessageType.BossRushStage);
                netMessage.Write(BossRushEvent.BossRushStage);
                netMessage.Send();
            }
            return true;
        }
    }
}
