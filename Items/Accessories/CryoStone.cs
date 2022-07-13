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
            SacrificeTotal = 1;
            DisplayName.SetDefault("Cryo Stone");
            Tooltip.SetDefault("One of the ancient relics\n" +
                "Creates a rotating ice shield around you that damages and slows down enemies on contact");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 4));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual) => player.Calamity().CryoStone = true;
    }
}
