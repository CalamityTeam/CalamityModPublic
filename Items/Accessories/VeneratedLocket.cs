using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class VeneratedLocket : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<GodSlayerInferno>()];
        }
        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 58;
            Item.value = Item.buyPrice(platinum: 10); // Sold by Bandit
            Item.rare = ModContent.RarityType<CosmicPurple>();
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<ThrowingDamageClass>() += 0.10f;
            player.Calamity().veneratedLocket = true;
        }
    }
}
