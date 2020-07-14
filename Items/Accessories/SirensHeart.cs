using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class SirensHeart : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aquatic Heart");
            Tooltip.SetDefault("Transforms the holder into a water elemental\n" +
                "Going underwater gives you a buff\n" +
                "Greatly reduces breath loss and provides a small amount of light in the abyss\n" +
                "Enemies become frozen when they touch you\n" +
                "You have a layer of ice around you that absorbs 20% damage but breaks after one hit\n" +
                "After 30 seconds the ice shield will regenerate\n" +
                "Wow, you can swim now!\n" +
                "Most of these effects are only active after Skeletron has been defeated\n" +
                "Revengeance drop");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.accessory = true;
            item.value = CalamityGlobalItem.Rarity4BuyPrice;
            item.rare = 4;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.sirenBoobs = true;
            if (hideVisual)
                modPlayer.sirenBoobsHide = true;
        }
    }

    public class SirenHead : EquipTexture
    {
        public override bool DrawHead()
        {
            return false;
        }
    }

    public class SirenBody : EquipTexture
    {
        public override bool DrawBody()
        {
            return false;
        }
    }

    public class SirenLegs : EquipTexture
    {
        public override bool DrawLegs()
        {
            return false;
        }
    }
}
