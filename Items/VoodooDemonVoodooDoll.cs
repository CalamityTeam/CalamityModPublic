using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using static CalamityMod.CalamityUtils;

namespace CalamityMod.Items
{
    public class VoodooDemonVoodooDoll : ModItem, ILocalizedModType
    {
        public static bool state = false;
        public new string LocalizationCategory => "Items.Misc";

        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 34;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.consumable = false;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = (ContentSamples.CreativeHelper.ItemGroup)CalamityResearchSorting.SpawnPrevention;
        }

        public override bool CanRightClick() => true;

        public override void RightClick(Player player)
        {
            if (player.Calamity().disableVoodooSpawns == true)
                player.Calamity().disableVoodooSpawns = false;
            else
                player.Calamity().disableVoodooSpawns = true;
            state = player.Calamity().disableVoodooSpawns;

            Item.RestoreConsumedItemByRightClick();
        }

        /*
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frameI, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture;

            if (state)
            {
                //Replace with enabled texture
                texture = ModContent.Request<Texture2D>("CalamityMod/Items/VoodooDemonVoodooDoll").Value;
                spriteBatch.Draw(texture, position, null, Color.White, 0f, origin, scale, SpriteEffects.None, 0);
            }
            else
            {
                texture = ModContent.Request<Texture2D>("CalamityMod/Items/VoodooDemonVoodooDoll").Value;
                spriteBatch.Draw(texture, position, null, Color.White, 0f, origin, scale, SpriteEffects.None, 0);
            }

            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture;

            if (state)
            {
                //Replace with enabled texture
                texture = ModContent.Request<Texture2D>("CalamityMod/Items/VoodooDemonVoodooDoll").Value;
                spriteBatch.Draw(texture, Item.position - Main.screenPosition, null, lightColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            }
            else
            {
                texture = ModContent.Request<Texture2D>("CalamityMod/Items/VoodooDemonVoodooDoll").Value;
                spriteBatch.Draw(texture, Item.position - Main.screenPosition, null, lightColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            }
            return false;
        }
        */

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string text;
            if (state == true)
                text = GetTextValue("Items.Misc.SpawnBlockersOn");
            else
                text = GetTextValue("Items.Misc.SpawnBlockersOff");
            tooltips.FindAndReplace("[STATE]", text);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HellstoneBar, 5).
                AddIngredient(ItemID.Silk, 5).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
