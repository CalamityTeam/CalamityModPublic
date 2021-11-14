using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing
{
    public class Spadefish : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spadefish");
            Tooltip.SetDefault("How can a fish be used to dig through the ground?\n" +
                "Some questions are best left unanswered.");
        }

        public override void SetDefaults()
        {
            item.damage = 15;
            item.knockBack = 2f;
            item.useTime = 7;
            item.useAnimation = 20;
            item.pick = 34;

            item.melee = true;
            item.width = 46;
            item.height = 44;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
        }
    }
}
