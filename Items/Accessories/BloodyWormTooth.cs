using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class BloodyWormTooth : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Bloody Worm Tooth");
            Tooltip.SetDefault("7% increased melee damage and speed");
        }

        public override void SetDefaults()
        {
            Item.defense = 7;
            Item.width = 12;
            Item.height = 15;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.bloodyWormTooth = true;
            player.GetAttackSpeed<MeleeDamageClass>() += 0.07f;
        }
    }
}
