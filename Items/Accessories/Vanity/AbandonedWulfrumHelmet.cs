using CalamityMod.CalPlayer;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.Accessories.Vanity
{
    [AutoloadEquip(EquipType.Head)]

    //A lot of legacy names that's for sure. A combo of the pre "WulfrumHeadX" names, and the aforementionned "WulfrumHeadX" names.
    //This is done so that non summoners don't end up with a helmet they don't really care about anyways, and is a cute reference to the old look.
    [LegacyName("WulfrumMask")]
    [LegacyName("WulfrumHeadRogue")]
    [LegacyName("WulfrumHeadgear")]
    [LegacyName("WulfrumHeadRanged")]
    [LegacyName("WulfrumHelm")]
    [LegacyName("WulfrumHeadMelee")]
    [LegacyName("WulfrumHood")]
    [LegacyName("WulfrumHeadMagic")]
    public class AbandonedWulfrumHelmet : ModItem
    {
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Accessories/Vanity/AbandonnedWulfrumHelmet_HeadSet", EquipType.Head, name: "WulfrumOldSetHead");
                EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Accessories/Vanity/AbandonnedWulfrumHelmet_Body", EquipType.Body, this);
                EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Accessories/Vanity/AbandonnedWulfrumHelmet_Legs", EquipType.Legs, this);
            }
        }

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Abandoned Wulfrum Helmet");
            Tooltip.SetDefault("A worn and rusty helmet ressembling older models of wulfrum armor\n" +
                //Could include lore about how X kind of people used to wear it. Like "Streets used to be filled with people of The Resistance wearing this cheap yet effective armor.
                "Transforms the holder into a wulfrum robot\n" +
                "Can also be worn in the helmet slot as a regular helm");

            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlotHead = EquipLoader.GetEquipSlot(Mod, "WulfrumOldSetHead", EquipType.Head);
            ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;

            int equipSlotBody = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
            ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
            ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;

            int equipSlotLegs = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);
            ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegs] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 30;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.vanity = true;
        }

        public override void UpdateVanity(Player player)
        {
            player.GetModPlayer<WulfrumTransformationPlayer>().transformationActive = true;
            player.GetModPlayer<WulfrumTransformationPlayer>().vanityEquipped = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!hideVisual)
            {
                player.GetModPlayer<WulfrumTransformationPlayer>().transformationActive = true;
                player.GetModPlayer<WulfrumTransformationPlayer>().vanityEquipped = true;
            }
        }
    }

    public class WulfrumTransformationPlayer : ModPlayer
    {
        public bool vanityEquipped = false;
        public bool transformationActive = false;
        public bool forceHelmetOn = false;

        public override void ResetEffects()
        {
            vanityEquipped = false;
            transformationActive = false;
            forceHelmetOn = false;
        }

        public override void FrameEffects()
        {
            if (forceHelmetOn || transformationActive)
            {
                Player.head = EquipLoader.GetEquipSlot(Mod, "WulfrumOldSetHead", EquipType.Head);
                Player.face = -1;
            }

            if (transformationActive)
            {
                Player.legs = EquipLoader.GetEquipSlot(Mod, "AbandonnedWulfrumHelmet", EquipType.Legs);
                Player.body = EquipLoader.GetEquipSlot(Mod, "AbandonnedWulfrumHelmet", EquipType.Body);
            }
        }
    }
}
