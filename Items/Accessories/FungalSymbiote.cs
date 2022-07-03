using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class FungalSymbiote : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Fungal Symbiote");
            Tooltip.SetDefault("Various melee weapons emit mushrooms in true melee range\n" +
                "15% increased true melee damage");
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 36;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<TrueMeleeDamageClass>() += 0.15f;
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.fungalSymbiote = true;
        }
    }
}
