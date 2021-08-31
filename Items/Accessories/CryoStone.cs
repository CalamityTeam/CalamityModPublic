using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class CryoStone : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cryo Stone");
            Tooltip.SetDefault("One of the ancient relics\n" +
                "Creates a rotating ice shield around you that damages enemies on contact");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(4, 4));
        }

        public override void SetDefaults()
        {
            item.width = 40;
            item.height = 40;
            item.value = CalamityGlobalItem.Rarity5BuyPrice;
            item.rare = ItemRarityID.Pink;
            item.accessory = true;
        }

        /*public override void UpdateAccessory(Player player, bool hideVisual)
        {
            
        }*/
    }
}
