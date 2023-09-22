using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    [LegacyName("FabledTortoiseShell")]
    public class FlameLickedShell : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.defense = 20;
            Item.width = 36;
            Item.height = 42;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.flameLickedShell = true;
            player.lavaImmune = true;
        }
    }
}
