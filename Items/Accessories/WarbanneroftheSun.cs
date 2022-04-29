using CalamityMod.CalPlayer;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Balloon)]
    public class WarbanneroftheSun : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Warbanner of the Sun");
            Tooltip.SetDefault("Increases melee damage, true melee damage and melee speed the closer you are to enemies\n" +
                "Max boost is 20% increased melee damage, true melee damage and melee speed");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 5));
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 78;
            Item.value = CalamityGlobalItem.Rarity11BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.warbannerOfTheSun = true;
        }
    }
}
