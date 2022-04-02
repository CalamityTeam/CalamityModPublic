using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing
{
    public class GlimmeringGemfish : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Glimmering Gemfish");
            Tooltip.SetDefault("Right click to extract gems");
        }

        public override void SetDefaults()
        {
            item.maxStack = 999;
            item.consumable = true;
            item.width = 34;
            item.height = 30;
            item.rare = ItemRarityID.Green;
            item.value = Item.sellPrice(silver: 50);
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void RightClick(Player player)
        {
            int gemMin = 1;
            int gemMax = 3;
            DropHelper.DropItemChance(player, ItemID.Amethyst, 0.5f, gemMin, gemMax);
            DropHelper.DropItemChance(player, ItemID.Topaz, 0.4f, gemMin, gemMax);
            DropHelper.DropItemChance(player, ItemID.Sapphire, 0.3f, gemMin, gemMax);
            DropHelper.DropItemChance(player, ItemID.Emerald, 0.2f, gemMin, gemMax);
            DropHelper.DropItemChance(player, ItemID.Ruby, 0.15f, gemMin, gemMax);
            DropHelper.DropItemChance(player, ItemID.Diamond, 0.1f, gemMin, gemMax);
            DropHelper.DropItemChance(player, ItemID.Amber, 0.25f, gemMin, gemMax);
            Mod thorium = CalamityMod.Instance.thorium;
            if (thorium != null)
            {
                DropHelper.DropItemChance(player, thorium.ItemType("Pearl"), 0.25f, gemMin, gemMax);
                DropHelper.DropItemChance(player, thorium.ItemType("Opal"), 0.25f, gemMin, gemMax);
                DropHelper.DropItemChance(player, thorium.ItemType("Onyx"), 0.25f, gemMin, gemMax);
            }
        }
    }
}
