using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories
{
    public class RottenDogtooth : ModItem
    {
        internal const int ArmorCrunchDebuffTime = 180;
        internal const float StealthStrikeDamageMultiplier = 0.1f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rotten Dogtooth");
            Tooltip.SetDefault($"Makes Stealth strikes inflict Armor Crunch, deal {(int)(StealthStrikeDamageMultiplier * 100)}% more damage and cost 1 less unit of stealth.");
        }

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 22;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().rottenDogTooth = true;
            player.Calamity().flatStealthLossReduction++;
        }
    }
}
