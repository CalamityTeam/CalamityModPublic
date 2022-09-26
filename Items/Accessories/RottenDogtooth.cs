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
            SacrificeTotal = 1;
            DisplayName.SetDefault("Rotten Dogtooth");
            Tooltip.SetDefault($"Makes Stealth strikes inflict Armor Crunch and deal {(int)(StealthStrikeDamageMultiplier * 100)}% more damage");
        }

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 22;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().rottenDogTooth = true;
            player.Calamity().bonusStealthDamage += StealthStrikeDamageMultiplier;
        }
    }
}
