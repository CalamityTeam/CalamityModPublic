using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories
{
    public class AquaticHeart : ModItem
    {
        public override void Load()
        {
            // All code below runs only if we're not loading on a server
            if (Main.netMode != NetmodeID.Server)
            {
                // Add equip textures
                Mod.AddEquipTexture(new EquipTexture(), this, EquipType.Head, "CalamityMod/Items/Accessories/AquaticTrans_Head");
                Mod.AddEquipTexture(new EquipTexture(), this, EquipType.Body, "CalamityMod/Items/Accessories/AquaticTrans_Body");
                Mod.AddEquipTexture(new EquipTexture(), this, EquipType.Legs, "CalamityMod/Items/Accessories/AquaticTrans_Legs");
            }
        }

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Aquatic Heart");
            Tooltip.SetDefault("Transforms the holder into a water elemental\n" +
                "Going underwater gives you a buff\n" +
                "Greatly reduces breath loss and provides a small amount of light in the abyss\n" +
                "Enemies become frozen when they touch you\n" +
                "You have a layer of ice around you that absorbs 20% damage but breaks after one hit\n" +
                "After 30 seconds the ice shield will regenerate\n" +
                "Wow, you can swim now!\n" +
                "Most of these effects are only active after Skeletron has been defeated");

            if (Main.netMode != NetmodeID.Server)
            {
                int equipSlotHead = Mod.GetEquipSlot(Name, EquipType.Head);
                int equipSlotBody = Mod.GetEquipSlot(Name, EquipType.Body);
                int equipSlotLegs = Mod.GetEquipSlot(Name, EquipType.Legs);
                ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
                ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
                ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;
                ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegs] = true;
            }
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.aquaticHeart = true;
            if (hideVisual)
                modPlayer.aquaticHeartHide = true;
        }
    }
}
