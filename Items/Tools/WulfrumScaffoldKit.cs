using System.IO;
using System.Linq;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityMod.Items.Tools
{
    public class WulfrumScaffoldKit : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Tools";
        public int storedScrap = 0;
        public static int TilesPerScrap = 40;
        public static int TileTime = 6 * 60;
        public static int TileReach = 40;
        public static int PlacedTileType => ModContent.TileType<WulfrumPipes>();

        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 42;
            Item.useTime = Item.useAnimation = 25;
            Item.autoReuse = false;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.HiddenAnimation;
            Item.UseSound = null;
            Item.noMelee = true;
            Item.channel = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(silver: 50);
            storedScrap = 0;
            Item.shoot = ModContent.ProjectileType<WulfrumScaffoldKitHoldout>();
            TileTime = 6 * 60;
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = (ContentSamples.CreativeHelper.ItemGroup)CalamityResearchSorting.ToolsOther;
		}

        public override void HoldItem(Player player)
        {
            player.Calamity().mouseWorldListener = true;
        }

        public override bool CanUseItem(Player player)
        {
            return (storedScrap > 0 || player.HasItem(ModContent.ItemType<WulfrumMetalScrap>())) && !player.noBuilding
                && !Main.projectile.Any(n => n.active && n.owner == player.whoAmI && n.type == Item.shoot);
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            float barScale = 1f;

            var barBG = ModContent.Request<Texture2D>("CalamityMod/UI/MiscTextures/GenericBarBack").Value;
            var barFG = ModContent.Request<Texture2D>("CalamityMod/UI/MiscTextures/GenericBarFront").Value;

            Vector2 drawPos = position + Vector2.UnitY * (frame.Height - 2) * scale + Vector2.UnitX * (frame.Width - barBG.Width * barScale) * scale * 0.5f;
            Rectangle frameCrop = new Rectangle(0, 0, (int)(storedScrap / (float)TilesPerScrap * barFG.Width), barFG.Height);
            Color colorBG = Color.RoyalBlue;
            Color colorFG = Color.Lerp(Color.Teal, Color.YellowGreen, storedScrap / (float)TilesPerScrap);

            spriteBatch.Draw(barBG, drawPos, null, colorBG, 0f, origin, scale * barScale, 0f, 0f);
            spriteBatch.Draw(barFG, drawPos, frameCrop, colorFG * 0.8f, 0f, origin, scale * barScale, 0f, 0f);
        }


        #region saving the durability
        public override ModItem Clone(Item item)
        {
            ModItem clone = base.Clone(item);
            if (clone is WulfrumScaffoldKit a && item.ModItem is WulfrumScaffoldKit a2)
            {
                a.storedScrap = a2.storedScrap;
            }
            return clone;
        }

        public override void SaveData(TagCompound tag)
        {
            tag["storedScrap"] = storedScrap;
        }

        public override void LoadData(TagCompound tag)
        {
            storedScrap = tag.GetInt("storedScrap");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(storedScrap);
        }

        public override void NetReceive(BinaryReader reader)
        {
            storedScrap = reader.ReadInt32();
        }
        #endregion

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<WulfrumMetalScrap>(6).
                AddIngredient<EnergyCore>().
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
