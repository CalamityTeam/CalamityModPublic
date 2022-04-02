using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class Calamity : ModItem
    {
        public const float MaxNPCSpeed = 5f;

        // This is ONLY the direct DPS of having the cursor over the enemy, not the damage from the flames debuff.
        // The debuff is VulnerabilityHex, check that file for its DPS.
        public const int BaseDamage = 266;
        public const int HitsPerSecond = 12;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Calamity");
            Tooltip.SetDefault("Lights your cursor ablaze with the Witch's flames, summoning a burning sigil around it\n" +
                "Enemies touching the sigil take immense damage and are inflicted with Vulnerability Hex\n" +
                "Equip in a vanity slot to change the cursor without dealing damage");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 6));
        }

        public override void SetDefaults()
        {
            item.width = 44;
            item.height = 108;
            item.Calamity().customRarity = CalamityRarity.Violet;
            item.rare = ItemRarityID.Purple;
            item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().blazingCursorDamage = true;
            player.Calamity().blazingCursorVisuals = true;
        }

        public override void UpdateEquip(Player player) => player.Calamity().blazingCursorVisuals = true;
    }
}
