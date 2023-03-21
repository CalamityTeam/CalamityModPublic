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
    public class HeartofDarkness : ModItem
    {
        // The percentage of a full Rage bar that is gained every second with Heart of Darkness equipped.
        public const float RagePerSecond = 0.01f;

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Heart of Darkness");
            Tooltip.SetDefault("You constantly gain rage over time\n" +
                "Rage does not fade away when out of combat\n" +
                "Revengeance drop");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 4));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.heartOfDarkness = true;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            CalamityUtils.DrawInventoryCustomScale(
                spriteBatch,
                texture: TextureAssets.Item[Type].Value,
                position,
                frame,
                drawColor,
                itemColor,
                origin,
                scale,
                wantedScale: 0.8f,
                drawOffset: new(-2f, -2f)
            );
            return false;
        }
    }
}
