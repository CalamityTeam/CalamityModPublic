using System.Collections.Generic;
using System.Linq;
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
    public class Terminus : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Terminus");
            Tooltip.SetDefault("Activates Boss Rush Mode, using it again will deactivate Boss Rush Mode\n" +
                                "During the Boss Rush, all wires and wired devices will be disabled");
        }

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Blue;
            Item.width = CalamityWorld.getFixedBoi ? 54 : 28;
            Item.height = CalamityWorld.getFixedBoi ? 78 : 28;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<TerminusHoldout>();
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frameI, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (CalamityWorld.getFixedBoi)
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
            if (CalamityWorld.getFixedBoi)
            {
                Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Items/SummonItems/Terminus_GFB").Value;
                spriteBatch.Draw(texture, Item.position - Main.screenPosition, null, lightColor, 0f, Vector2.Zero, 1f, 0, 0);
                return false;
            }
            else
                return true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            Player player = Main.LocalPlayer;
            TooltipLine name = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "ItemName");

            if (CalamityWorld.getFixedBoi)
            {
                name.Text = "Ogscule";
            }
            else
            {
                name.Text = "Terminus";
            }
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			// The wiki classifies Boss Rush as an event
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.EventItem;
		}
    }
}
