using CalamityMod.NPCs.NormalNPCs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class SuperDummy : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Super Dummy");
            Tooltip.SetDefault("Creates a super dummy\n" +
                "Regenerates 1 million life per second\n" +
                "Will not die when taking damage over time from debuffs\n" +
                "Right click to kill all super dummies");
        }

        public override void SetDefaults()
        {
            item.damage = 0;
            item.width = 20;
            item.height = 30;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
            item.value = 0;
            item.rare = ItemRarityID.Blue;
            item.autoReuse = true;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public static void DeleteDummies()
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.type == ModContent.NPCType<SuperDummyNPC>() && npc.active)
                {
                    npc.life = 0;
                    npc.active = false;

                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, i);
                }
            }
        }

        public override bool UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                if (Main.myPlayer == player.whoAmI)
                {
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        DeleteDummies();

                    // A custom packet must be sent so that the deletion can be done on the server. This hook does not run there.
                    // Why? Well, netUpdate/MessageID.SyncNPC packets do not send data to the server. They only send data to other clients. What this means is that prior to this fix
                    // what would happen was every client EXCEPT THE SERVER would be told by this player that the dummies disappeared.
                    // However, since the server isn't notified, it is inevitable that the server will do a sync of its own, unaware that the 
                    // dummies are gone, and cause them to reappear, making the deletion moot.
                    else
                    {
                        var netMessage = mod.GetPacket();
                        netMessage.Write((byte)CalamityModMessageType.DeleteAllSuperDummies);
                        netMessage.Send();
                    }
                }
            }
            else if (player.whoAmI == Main.myPlayer)
            {
                int x = (int)Main.MouseWorld.X - 9;
                int y = (int)Main.MouseWorld.Y - 20;

                // In single player, just spawn the dummy.
                if (Main.netMode == NetmodeID.SinglePlayer)
                    NPC.NewNPC(x, y, ModContent.NPCType<SuperDummyNPC>());

                // Otherwise, send a message to the server indicating that a Super Dummy should be spawned at this position.
                else
                {
                    var netMessage = mod.GetPacket();
                    netMessage.Write((byte)CalamityModMessageType.SpawnSuperDummy);
                    netMessage.Write(x);
                    netMessage.Write(y);
                    netMessage.Send();
                }
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.TargetDummy);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
