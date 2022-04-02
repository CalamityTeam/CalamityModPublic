using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
            item.width = 14;
            item.height = 22;
            item.value = CalamityGlobalItem.Rarity1BuyPrice;
            item.rare = ItemRarityID.Blue;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
		{
            player.Calamity().rottenDogTooth = true;
            player.Calamity().flatStealthLossReduction++;
        }
    }
}
