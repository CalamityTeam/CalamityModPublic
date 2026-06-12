using CalamityMod.CalPlayer;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class ArcFlashRing : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";

        public static int LightningSpawnPercent = 5;
        public static float LightningDamageMult = 3f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(LightningSpawnPercent, LightningDamageMult);
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [BuffID.Electrified];
        }
        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 64;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.accessory = true;
            Item.expert = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.arcFlashRing = true;
            if (!hideVisual)
                modPlayer.arcFlashRingVisual = true;
            else
                modPlayer.arcFlashRingVisual = false;
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
                wantedScale: 0.7f,
                drawOffset: new(1f, 0f)
            );
            return false;
        }
    }
}
