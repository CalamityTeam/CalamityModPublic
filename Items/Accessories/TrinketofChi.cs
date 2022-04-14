using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories
{
    public class TrinketofChi : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Trinket of Chi");
            Tooltip.SetDefault("After 1 second of standing still and not attacking you gain a buff\n" +
                "This buff boosts your damage by 50% and decreases damage taken by 15%\n" +
                "The buff deactivates after you move or attack once");
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 32;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.trinketOfChi = true;
        }
    }
}
