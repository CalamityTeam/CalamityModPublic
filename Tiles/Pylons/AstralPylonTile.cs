using CalamityMod.Systems;
using CalamityMod.TileEntities;
using CalamityMod.Items.Placeables.Pylons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;

namespace CalamityMod.Tiles.Pylons
{

    public class AstralPylonTile : ModPylon
    {
        public const int CrystalHorizontalFrameCount = 1;
        public const int CrystalVerticalFrameCount = 8;
        public const int CrystalFrameHeight = 64;

        public Asset<Texture2D> crystalTexture;
        public Asset<Texture2D> mapIcon;

        public override void Load()
        {
            // Pre-loading textures
            crystalTexture = ModContent.Request<Texture2D>(Texture + "_Crystal");
            mapIcon = ModContent.Request<Texture2D>(Texture + "_MapIcon");
        }

        public override void SetStaticDefaults()
        {
            TEModdedPylon moddedPylon = ModContent.GetInstance<TECalamityPylon>();
            this.SetUpPylon(moddedPylon, true);

            TileID.Sets.InteractibleByNPCs[Type] = true;
            TileID.Sets.PreventsSandfall[Type] = true;

            ModTranslation pylonName = CreateMapEntryName();
            AddMapEntry(Color.White, pylonName);
        }

        public override int? IsPylonForSale(int npcType, Player player, bool isNPCHappyEnough)
        {
            // Pylon will sell regardless of NPC happiness.
            // TODO -- It would probably be nice to have pylons sell dependent on NPC happiness, but not a huge priority.
            return player.Calamity().ZoneAstral ? ModContent.ItemType<AstralPylon>() : null;
        }

        public override void MouseOver(int i, int j)
        {
            // Uses the pylon item as an icon when hovering over it.
            Main.LocalPlayer.cursorItemIconEnabled = true;
            Main.LocalPlayer.cursorItemIconID = ModContent.ItemType<AstralPylon>();
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            ModContent.GetInstance<TECalamityPylon>().Kill(i, j);
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 2, 3, ModContent.ItemType<AstralPylon>());
        }

        public override bool ValidTeleportCheck_BiomeRequirements(TeleportPylonInfo pylonInfo, SceneMetrics sceneData)
        {
            // Copied from ExampleMod for documentation if needed later -pixl
            //
            // Right before this hook is called, the sceneData parameter exports its information based on wherever the destination pylon is,
            // and by extension, it will call ALL ModSystems that use the TileCountsAvailable method. This means, that if you determine biomes
            // based off of tile count, when this hook is called, you can simply check the tile threshold.

            return BiomeTileCounterSystem.AstralTiles >= 100;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameX < 66)
            {
                r = 0.8f;
                g = 0.5f;
                b = 0.8f;
            }
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            // We want to draw the pylon crystal the exact same way vanilla does, so we can use this built in method in ModPylon for default crystal drawing:
            DefaultDrawPylonCrystal(spriteBatch, i, j, crystalTexture, Color.White, CrystalFrameHeight, CrystalHorizontalFrameCount, CrystalVerticalFrameCount);
        }

        public override void DrawMapIcon(ref MapOverlayDrawContext context, ref string mouseOverText, TeleportPylonInfo pylonInfo, bool isNearPylon, Color drawColor, float deselectedScale, float selectedScale)
        {
            // Just like in SpecialDraw, we want things to be handled the EXACT same way vanilla would handle it, which ModPylon also has built in methods for:
            bool mouseOver = DefaultDrawMapIcon(ref context, mapIcon, pylonInfo.PositionInTiles.ToVector2() + new Vector2(1.5f, 2f), drawColor, deselectedScale, selectedScale);
            DefaultMapClickHandle(mouseOver, pylonInfo, "Mods.CalamityMod.ItemName.AstralPylon", ref mouseOverText);
        }
    }
}
