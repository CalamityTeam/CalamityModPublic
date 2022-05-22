using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories
{
    public class DragonScales : ModItem
    {
        internal static int ShitBaseDamage = 50;
        
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Dragon Scales");
            Tooltip.SetDefault("Only a living dragon holds true treasure\n" +
                               "Rogue projectiles create slow fireballs as they travel\n" +
                               "Stealth strikes create infernados on death\n" +
                               "+10% max run speed and acceleration\n" +
                               "Grants immunity to Dragon Fire");
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 34;
            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.Calamity().customRarity = CalamityRarity.Violet;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.dragonScales = true;
            player.buffImmune[ModContent.BuffType<LethalLavaBurn>()] = true;
        }
    }
}
