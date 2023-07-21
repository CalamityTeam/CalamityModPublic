using System.Collections.Generic;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    [LegacyName("BossRush")]
    public class Terminus : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.SummonItems";
        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Blue;
            Item.width = Main.zenithWorld ? 54 : 28;
            Item.height = Main.zenithWorld ? 78 : 28;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<TerminusHoldout>();
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }

        public override void UpdateInventory(Player player)
        {
            if (Main.zenithWorld)
                Item.SetNameOverride(this.GetLocalizedValue("GFBName"));
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frameI, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (Main.zenithWorld)
            {
                Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Items/SummonItems/Terminus_GFB").Value;
                Color overlay = Color.White;
                spriteBatch.Draw(texture, position, null, overlay, 0f, origin, scale, 0, 0);
                return false;
            }
            else
                return true;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            if (Main.zenithWorld)
            {
                Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Items/SummonItems/Terminus_GFB").Value;
                spriteBatch.Draw(texture, Item.position - Main.screenPosition, null, lightColor, 0f, Vector2.Zero, 1f, 0, 0);
                return false;
            }
            else
                return true;
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			// The wiki classifies Boss Rush as an event
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.EventItem;
		}
    }
}
