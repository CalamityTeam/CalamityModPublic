using CalamityMod.NPCs.TownNPCs;
using CalamityMod.Packets;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems.TownPets
{
    public class TheHousingContract : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.SummonItems";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.UseSound = SoundID.Item92;
            Item.width = 28;
            Item.height = 28;
            Item.maxStack = Item.CommonMaxStack;
            Item.SetShopValues(ItemRarityColor.Green2, Item.buyPrice(gold: 5)); // Same price as other Pet Licenses; Sold by Shady Salesman
        }

        public override bool? UseItem(Player player)
        {
            int npcType = ModContent.NPCType<TownPiggy>();
            if (player.ItemAnimationJustStarted && (!CalamityWorld.unlockedTownPig || NPC.AnyNPCs(npcType)))
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    SpawnPiggy();
                }
                return true;
            }
            return false;
        }

        public static void SpawnPiggy()
        {
            Color color = new(50, 255, 130);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                if (!CalamityWorld.unlockedTownPig)
                {
                    SyncTownPigLicensePacket.Send();
                }
            }
            else if (!CalamityWorld.unlockedTownPig)
            {
                CalamityWorld.unlockedTownPig = true;
                ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Mods.CalamityMod.Misc.PiggyLicense"), color);
                CalamityNetcode.SyncWorld();
            }
        }
    }
}
