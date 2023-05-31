using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityMod.UI.ResourceSets
{
    public class CalamityResourceOverlay : ModResourceOverlay
    {
        // Most of this is taken from ExampleMod. See that for additional explanations.
		private Dictionary<string, Asset<Texture2D>> vanillaAssetCache = new();
        public string baseFolder = "CalamityMod/UI/ResourceSets/";

        // Determines which health UI to draw based on player upgrades.
        public string LifeTexturePath()
        {
            string folder = $"{baseFolder}HP";
            CalamityPlayer modPlayer = Main.LocalPlayer.Calamity();
            if (modPlayer.dFruit)
                return folder + "Dragonfruit";
            if (modPlayer.eBerry)
                return folder + "Elderberry";
            if (modPlayer.mFruit)
                return folder + "MiracleFruit";
            if (modPlayer.bOrange)
                return folder + "BloodOrange";
            return string.Empty;
        }

        // Determines which mana UI to draw based on player upgrades.
        public string ManaTexturePath()
        {
            string folder = $"{baseFolder}MP";
            CalamityPlayer modPlayer = Main.LocalPlayer.Calamity();
            if (modPlayer.pHeart)
                return folder + "PhantomHeart";
            if (modPlayer.eCore)
                return folder + "EtherealCore";
            if (modPlayer.cShard)
                return folder + "CometShard";
            return string.Empty;
        }

		public override void PostDrawResource(ResourceOverlayDrawContext context)
        {
			Asset<Texture2D> asset = context.texture;
            // Vanilla texture paths
            string fancyFolder = "Images/UI/PlayerResourceSets/FancyClassic/";
            string barsFolder = "Images/UI/PlayerResourceSets/HorizontalBars/";

            if (ManaTexturePath() != string.Empty)
            {
                // Draw stars for Classic and Fancy
                if (asset == TextureAssets.Mana || CompareAssets(asset, fancyFolder + "Star_Fill"))
                {
                    context.texture = ModContent.Request<Texture2D>(ManaTexturePath() + "Star");
                    context.Draw();
                }
                // Draw mana bars
                else if (CompareAssets(asset, barsFolder + "MP_Fill"))
                {
                    context.texture = ModContent.Request<Texture2D>(ManaTexturePath() + "Bar");
                    context.Draw();
                }
            }

            if (LifeTexturePath() == string.Empty)
				return;

			// Draw hearts for Classic and Fancy
			if (asset == TextureAssets.Heart || asset == TextureAssets.Heart2 || CompareAssets(asset, fancyFolder + "Heart_Fill") || CompareAssets(asset, fancyFolder + "Heart_Fill_B"))
			{
                context.texture = ModContent.Request<Texture2D>(LifeTexturePath() + "Heart");
			    context.Draw();
            }
            // Draw health bars
			else if (CompareAssets(asset, barsFolder + "HP_Fill") || CompareAssets(asset, barsFolder + "HP_Fill_Honey"))
			{
                context.texture = ModContent.Request<Texture2D>(LifeTexturePath() + "Bar");
    			context.Draw();
            }
		}

		private bool CompareAssets(Asset<Texture2D> currentAsset, string compareAssetPath)
        {
			if (!vanillaAssetCache.TryGetValue(compareAssetPath, out var asset))
				asset = vanillaAssetCache[compareAssetPath] = Main.Assets.Request<Texture2D>(compareAssetPath);

			return currentAsset == asset;
		}
	}
}
