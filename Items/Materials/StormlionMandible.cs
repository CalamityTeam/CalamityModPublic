using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.DataStructures;

namespace CalamityMod.Items.Materials
{
    public class StormlionMandible : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 8));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 38;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(silver: 1, copper: 40);
            Item.rare = ItemRarityID.Blue;
        }
    }
}
