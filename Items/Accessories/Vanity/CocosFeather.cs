using CalamityMod.CalPlayer;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using CalamityMod;


namespace CalamityMod.Items.Accessories.Vanity
{
    public class CocosFeather : ModItem, IExtendedHat, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Accessories/Vanity/Birds_Head", EquipType.Head, this);
                EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Accessories/Vanity/Birds_Torso", EquipType.Body, this);
                EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Accessories/Vanity/Birds_Legs", EquipType.Legs, this);
            }
        }

        public override void SetStaticDefaults()
        {
           
            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlotHead = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
            ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;

            int equipSlotBody = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
            ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
            ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;

            int equipSlotLegs = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);
            ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegs] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 38;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.vanity = true;
            Item.Calamity().devItem = true;
        }

        public override void UpdateVanity(Player player)
        {
            player.Calamity().cocosFeather = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!hideVisual)
            {
                player.Calamity().cocosFeather = true;
            }
        }
        public string ExtensionTexture => "CalamityMod/Items/Accessories/Vanity/Birds_Head_Extension";
        public Vector2 ExtensionSpriteOffset(PlayerDrawSet drawInfo) => new Vector2(0, -4f);
    }
}
