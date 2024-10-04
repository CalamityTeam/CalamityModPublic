using CalamityMod.NPCs.NormalNPCs;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class SuperDummy : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Misc";
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 30;
            Item.damage = 0;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.value = 0;
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public static void DeleteDummies()
        {
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.type == ModContent.NPCType<SuperDummyNPC>())
                {
                    npc.life = 0;
                    npc.active = false;

                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                }
            }
        }

        public override bool? UseItem(Player player)
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
                        var netMessage = Mod.GetPacket();
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
                    NPC.NewNPC(new EntitySource_ItemUse(player, Item), x, y, ModContent.NPCType<SuperDummyNPC>());

                // Otherwise, send a message to the server indicating that a Super Dummy should be spawned at this position.
                else
                {
                    var netMessage = Mod.GetPacket();
                    netMessage.Write((byte)CalamityModMessageType.SpawnSuperDummy);
                    netMessage.Write(x);
                    netMessage.Write(y);
                    netMessage.Send();
                }
            }
            return true;
        }

        public override bool CanRightClick() => true;

        // RightClick only called by client, no worry about potential packetstorm
        public override void RightClick(Player player)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                DeleteDummies();
            }
            else
            {
                var netMessage = Mod.GetPacket();
                netMessage.Write((byte)CalamityModMessageType.DeleteAllSuperDummies);
                netMessage.Send();
            }

            Item.RestoreConsumedItemByRightClick();
        }

        public override void AddRecipes()
        {
            Recipe r = CreateRecipe();
            r.AddIngredient(ItemID.TargetDummy);
            r.Register();

            // Super Dummy revert to Target Dummy
            r = Recipe.Create(ItemID.TargetDummy);
            r.AddIngredient<SuperDummy>();
            r.Register();
        }
    }
}
