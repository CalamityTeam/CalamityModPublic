using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class Calamity : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public const float MaxNPCSpeed = 5f;

        // This is ONLY the direct DPS of having the cursor over the enemy, not the damage from the flames debuff.
        // The debuff is VulnerabilityHex, check that file for its DPS.
        public const int BaseDamage = 320;
        public const int FramesPerHit = 5;

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 6));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 108;
            Item.rare = ModContent.RarityType<CalamityRed>();
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.accessory = true;
            Item.expert = true;
        }

        public override void UpdateVanity(Player player)
        {
            // Do a vanity/social slot check for SCal's expert drop since alternatives to get this working are a pain in the ass to create.
            player.Calamity().blazingCursorVisuals = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().blazingCursorDamage = true;
            player.Calamity().blazingCursorVisuals = true;
        }

        public override void UpdateItemDye(Player player, int dye, bool hideVisual) => player.Calamity().CalamityFireDyeShader = GameShaders.Armor.GetSecondaryShader(dye, player);

        public override void UpdateEquip(Player player) => player.Calamity().blazingCursorVisuals = true;

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = Main.zenithWorld ? ModContent.Request<Texture2D>("CalamityMod/Items/Accessories/Calamity_GFB").Value : TextureAssets.Item[Type].Value;
            CalamityUtils.DrawInventoryCustomScale(
                spriteBatch,
                texture,
                position,
                frame,
                drawColor,
                itemColor,
                origin,
                scale,
                wantedScale: 0.5f,
                drawOffset: new(0f, -4f)
            );
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            if (Main.zenithWorld)
            {
                Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Items/Accessories/Calamity_GFB").Value;
                spriteBatch.Draw(texture, Item.position - Main.screenPosition, Main.itemAnimations[Type].GetFrame(texture), lightColor, 0f, Vector2.Zero, 1f, 0, 0);
                return false;
            }
            else
                return true;
        }
    }
}
