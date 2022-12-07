using CalamityMod.CalPlayer;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class FrostFlare : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Frost Flare");
            Tooltip.SetDefault("All attacks and projectiles inflict frostbite\n" +
                "Immunity to frostburn, chilled and frozen\n" +
                "Being above 75% life grants 10% increased damage\n" +
                "Being below 25% life grants 20 defense and 15% increased max movement speed and acceleration\n" +
                "Grants resistance against cold attacks");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 4));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 24;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.frostFlare = true;
        }
    }
}
