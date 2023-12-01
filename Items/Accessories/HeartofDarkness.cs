using CalamityMod.CalPlayer;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Items.Accessories
{
    public class HeartofDarkness : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        // The percentage of a full Rage bar that is gained every second with Heart of Darkness equipped.
        public const float RagePerSecond = 0.01f;

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 5));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<StressPills>();
        }

        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 66;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.heartOfDarkness = true;
        }
    }
}
