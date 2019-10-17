using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items
{
    public class AstralJelly : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aureus Cell");
            Tooltip.SetDefault("Gives mana regeneration and magic power for 6 minutes");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.useTurn = true;
            item.maxStack = 30;
            item.useAnimation = 17;
            item.useTime = 17;
            item.rare = 7;
            item.useStyle = 2;
            item.healMana = 200;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.value = Item.buyPrice(0, 4, 50, 0);
            item.buffType = BuffID.WellFed;
            item.buffTime = 108000;
        }

        public override void OnConsumeItem(Player player)
        {
            player.AddBuff(BuffID.ManaRegeneration, 21600);
            player.AddBuff(BuffID.MagicPower, 21600);
            player.AddBuff(BuffID.WellFed, 108000);
        }
    }
}
