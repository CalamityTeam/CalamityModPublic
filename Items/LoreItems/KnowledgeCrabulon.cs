using CalamityMod.Buffs.StatBuffs;
using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeCrabulon : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crabulon");
            Tooltip.SetDefault("A crab and its mushrooms, a love story.\n" +
                "It's interesting how creatures can adapt given certain circumstances.\n" +
                "Place in your inventory to gain the Mushy buff while underground or in the mushroom biome.\n" +
				"However, your movement speed will be decreased while in these areas due to you being covered in fungi.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = 2;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void UpdateInventory(Player player)
        {
			CalamityPlayer modPlayer = player.Calamity();
			modPlayer.crabulonLore = true;
        }
    }
}
