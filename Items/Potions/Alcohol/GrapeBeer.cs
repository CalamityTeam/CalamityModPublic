using CalamityMod.Buffs.Alcohol;
using CalamityMod.World;
using System.Collections.Generic;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class GrapeBeer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Grape Beer");
            Tooltip.SetDefault(@"This crap is abhorrent but you might like it
Reduces defense by 3% and movement speed by 5%");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.useTurn = true;
            item.maxStack = 30;
            item.rare = ItemRarityID.LightRed;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.UseSound = SoundID.Item3;
            item.healLife = 100;
            item.healMana = 100;
            item.consumable = true;
            item.potion = true;
            item.value = Item.buyPrice(0, 0, 20, 0);
        }

        public override void OnConsumeItem(Player player)
        {
            player.AddBuff(ModContent.BuffType<GrapeBeerBuff>(), 900);
        }
    }
}
