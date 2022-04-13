using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

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
            Item.damage = 15;
            Item.knockBack = 2f;
            Item.useTime = 7;
            Item.useAnimation = 20;
            Item.pick = 34;

            Item.DamageType = DamageClass.Melee;
            Item.width = 46;
            Item.height = 44;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
    }
}
