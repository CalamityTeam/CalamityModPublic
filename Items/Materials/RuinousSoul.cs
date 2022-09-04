using CalamityMod.Rarities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class RuinousSoul : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 25;
            DisplayName.SetDefault("Ruinous Soul");
            Tooltip.SetDefault("A shard of the distant past");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 6));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
			ItemID.Sets.SortingPriorityMaterials[Type] = 111;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 42;
            Item.maxStack = 999;
            Item.value = Item.buyPrice(0, 7, 0, 0);
            Item.rare = ModContent.RarityType<PureGreen>();
        }
    }
}
