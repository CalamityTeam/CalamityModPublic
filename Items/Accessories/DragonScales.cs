using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class DragonScales : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public static int ShitBaseDamage = 57;
        public static int TornadoBaseDamage = 210;
        
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 34;
            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.rare = ModContent.RarityType<Violet>();
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.dragonScales = true;
            player.buffImmune[ModContent.BuffType<Dragonfire>()] = true;
        }
    }
}
