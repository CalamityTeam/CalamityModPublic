using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
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
            item.buffType = BuffID.MagicPower;
            item.buffTime = 21600;
        }

        public override void OnConsumeItem(Player player)
        {
            player.AddBuff(BuffID.MagicPower, 21600);
            player.AddBuff(BuffID.ManaRegeneration, 21600);
        }
    }
}
