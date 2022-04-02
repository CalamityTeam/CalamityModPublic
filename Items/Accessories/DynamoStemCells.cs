using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class DynamoStemCells : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dynamo Stem Cells");
            Tooltip.SetDefault(@"10% increased movement speed
Ranged weapons have a chance to fire mini swarmers");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 5));
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = CalamityGlobalItem.Rarity11BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().dynamoStemCells = true;
            player.moveSpeed += 0.1f;
        }
    }
}
