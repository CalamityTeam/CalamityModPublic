using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles
{
    public class MonsterBanner : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.SolidBottom, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);
            dustType = -1;
            disableSmartCursor = true;
            AddMapEntry(new Color(13, 88, 130), Language.GetText("MapObject.Banner"));
        }

        // TODO -- encode these giant switch statements as a sequence in the main class' lists section
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            int style = frameX / 18;
            string item;
            switch (style)
            {
                case 0:
                    item = "AquaticParasiteBanner";
                    break;
                case 1:
                    item = "FlounderBanner";
                    break;
                case 2:
                    item = "GnasherBanner";
                    break;
                case 3:
                    item = "TrasherBanner";
                    break;
                case 4:
                    item = "CatfishBanner";
                    break;
                case 5:
                    item = "MaulerBanner";
                    break;
                case 6:
                    item = "AquaticSeekerBanner";
                    break;
                case 7:
                    item = "AquaticUrchinBanner";
                    break;
                case 8:
                    item = "FrogfishBanner";
                    break;
                case 9:
                    item = "MantisShrimpBanner";
                    break;
                case 10:
                    item = "AquaticAberrationBanner";
                    break;
                case 11:
                    item = "ParaseaBanner";
                    break;
                case 12:
                    item = "SeaUrchinBanner";
                    break;
                case 13:
                    item = "BoxJellyfishBanner";
                    break;
                case 14:
                    item = "MorayEelBanner";
                    break;
                case 15:
                    item = "DevilFishBanner";
                    break;
                case 16:
                    item = "CuttlefishBanner";
                    break;
                case 17:
                    item = "ToxicMinnowBanner";
                    break;
                case 18:
                    item = "ViperfishBanner";
                    break;
                case 19:
                    item = "LuminousCorvinaBanner";
                    break;
                case 20:
                    item = "GiantSquidBanner";
                    break;
                case 21:
                    item = "LaserfishBanner";
                    break;
                case 22:
                    item = "OarfishBanner";
                    break;
                case 23:
                    item = "ColossalSquidBanner";
                    break;
                case 24:
                    item = "MirageJellyBanner";
                    break;
                case 25:
                    item = "EidolistBanner";
                    break;
                case 26:
                    item = "GulperEelBanner";
                    break;
                case 27:
                    item = "EidolonWyrmJuvenileBanner";
                    break;
                case 28:
                    item = "BloatfishBanner";
                    break;
                case 29:
                    item = "BobbitWormBanner";
                    break;
                case 30:
                    item = "ChaoticPufferBanner";
                    break;
                case 31:
                    item = "AstralProbeBanner";
                    break;
                case 32:
                    item = "SmallSightseerBanner";
                    break;
                case 33:
                    item = "BigSightseerBanner";
                    break;
                case 34:
                    item = "AriesBanner";
                    break;
                case 35:
                    item = "AstralSlimeBanner";
                    break;
                case 36:
                    item = "AtlasBanner";
                    break;
                case 37:
                    item = "MantisBanner";
                    break;
                case 38:
                    item = "NovaBanner";
                    break;
                case 39:
                    item = "AstralachneaBanner";
                    break;
                case 40:
                    item = "HiveBanner";
                    break;
                case 41:
                    item = "StellarCullexBanner";
                    break;
                case 42:
                    item = "FusionFeederBanner";
                    break;
                case 43:
                    item = "HadarianBanner";
                    break;
                case 44:
                    item = "HeatSpiritBanner";
                    break;
                case 45:
                    item = "ScryllarBanner";
                    break;
                case 46:
                    item = "DespairStoneBanner";
                    break;
                case 47:
                    item = "SoulSlurperBanner";
                    break;
                case 48:
                    item = "ImpiousImmolatorBanner";
                    break;
                case 49:
                    item = "ScornEaterBanner";
                    break;
                case 50:
                    item = "ProfanedEnergyBanner";
                    break;
                case 51:
                    item = "WulfrumSlimeBanner";
                    break;
                case 52:
                    item = "WulfrumDroneBanner";
                    break;
                case 53:
                    item = "PitbullBanner";
                    break;
                case 54:
                    item = "BlightedEyeBanner";
                    break;
                case 55:
                    item = "CalamityEyeBanner";
                    break;
                case 56:
                    item = "SunskaterBanner";
                    break;
                case 57:
                    item = "ShockstormShuttleBanner";
                    break;
                case 58:
                    item = "CloudElementalBanner";
                    break;
                case 59:
                    item = "AngryDogBanner";
                    break;
                case 60:
                    item = "CryonBanner";
                    break;
                case 61:
                    item = "IceClasperBanner";
                    break;
                case 62:
                    item = "StormlionBanner";
                    break;
                case 63:
                    item = "CnidrionBanner";
                    break;
                case 64:
                    item = "SandTortoiseBanner";
                    break;
                case 65:
                    item = "GreatSandSharkBanner";
                    break;
                case 66:
                    item = "AmethystCrawlerBanner";
                    break;
                case 67:
                    item = "TopazCrawlerBanner";
                    break;
                case 68:
                    item = "SapphireCrawlerBanner";
                    break;
                case 69:
                    item = "EmeraldCrawlerBanner";
                    break;
                case 70:
                    item = "RubyCrawlerBanner";
                    break;
                case 71:
                    item = "DiamondCrawlerBanner";
                    break;
                case 72:
                    item = "AmberCrawlerBanner";
                    break;
                case 73:
                    item = "CrystalCrawlerBanner";
                    break;
                case 74:
                    item = "SunBatBanner";
                    break;
                case 75:
                    item = "CosmicElementalBanner";
                    break;
                case 76:
                    item = "EarthElementalBanner";
                    break;
                case 77:
                    item = "ArmoredDiggerBanner";
                    break;
                case 78:
                    item = "MelterBanner";
                    break;
                case 79:
                    item = "PestilentSlimeBanner";
                    break;
                case 80:
                    item = "PlagueshellBanner";
                    break;
                case 81:
                    item = "PlagueChargerBanner";
                    break;
                case 82:
                    item = "VirulingBanner";
                    break;
                case 83:
                    item = "PlaguebringerBanner";
                    break;
                case 84:
                    item = "PhantomSpiritBanner";
                    break;
                case 85:
                    item = "OverloadedSoldierBanner";
                    break;
                case 86:
                    item = "PhantomDebrisBanner";
                    break;
                case 87:
                    item = "BOHLDOHRBanner";
                    break;
                case 88:
                    item = "EbonianBlightSlimeBanner";
                    break;
                case 89:
                    item = "CrimulanBlightSlimeBanner";
                    break;
                case 90:
                    item = "AeroSlimeBanner";
                    break;
                case 91:
                    item = "CryoSlimeBanner";
                    break;
                case 92:
                    item = "PerennialSlimeBanner";
                    break;
                case 93:
                    item = "CharredSlimeBanner";
                    break;
                case 94:
                    item = "BloomSlimeBanner";
                    break;
                case 95:
                    item = "CultistAssassinBanner";
                    break;
                case 96:
                    item = "ReaperSharkBanner";
                    break;
                case 97:
                    item = "IrradiatedSlimeBanner";
                    break;
                case 98:
                    item = "PrismTurtleBanner";
                    break;
                case 99:
                    item = "ClamBanner";
                    break;
                case 100:
                    item = "EutrophicRayBanner";
                    break;
                case 101:
                    item = "GhostBellBanner";
                    break;
                case 102:
                    item = "GhostBellSmallBanner";
                    break;
                case 103:
                    item = "SeaFloatyBanner";
                    break;
                case 104:
                    item = "BlindedAnglerBanner";
                    break;
                case 105:
                    item = "SeaMinnowBanner";
                    break;
                case 106:
                    item = "SeaSerpentBanner";
                    break;
                case 107:
                    item = "GiantClamBanner";
                    break;
                case 108:
                    item = "PiggyBanner";
                    break;
                case 109:
                    item = "FearlessGoldfishWarriorBanner";
                    break;
                case 110:
                    item = "RadiatorBanner";
                    break;
                case 111:
                    item = "TrilobiteBanner";
                    break;
                case 112:
                    item = "OrthoceraBanner";
                    break;
                case 113:
                    item = "SkyfinBanner";
                    break;
                case 114:
                    item = "WaterLeechBanner";
                    break;
                case 115:
                    item = "AcidEelBanner";
                    break;
                case 116:
                    item = "NuclearToadBanner";
                    break;
                case 117:
                    item = "FlakCrabBanner";
                    break;
                case 118:
                    item = "SulfurousSkaterBanner";
                    break;
                case 119:
                    item = "FlakBabyBanner";
                    break;
                case 120:
                    item = "AnthozoanCrabBanner";
                    break;
                case 121:
                    item = "BelchingCoralBanner";
                    break;
                case 122:
                    item = "GammaSlimeBanner";
                    break;
                case 123:
                    item = "WulfrumGyratorBanner";
                    break;
                case 124:
                    item = "WulfrumHovercraftBanner";
                    break;
                case 125:
                    item = "WulfrumRoverBanner";
                    break;
                case 126:
                    item = "WulfrumPylonBanner";
                    break;
                default:
                    return;
            }
            Item.NewItem(i * 16, j * 16, 16, 48, mod.ItemType(item));
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (!closer)
                return;
            Player player = Main.LocalPlayer;
            if (player is null || !player.active || player.dead)
                return;

            int style = Main.tile[i, j].frameX / 18;
            string type;
            switch (style)
            {
                case 0:
                    type = "AquaticParasite";
                    break;
                case 1:
                    type = "Flounder";
                    break;
                case 2:
                    type = "Gnasher";
                    break;
                case 3:
                    type = "Trasher";
                    break;
                case 4:
                    type = "Catfish";
                    break;
                case 5:
                    type = "Mauler";
                    break;
                case 6:
                    type = "AquaticSeekerHead";
                    break;
                case 7:
                    type = "AquaticUrchin";
                    break;
                case 8:
                    type = "Frogfish";
                    break;
                case 9:
                    type = "MantisShrimp";
                    break;
                case 10:
                    type = "AquaticAberration";
                    break;
                case 11:
                    type = "Parasea";
                    break;
                case 12:
                    type = "SeaUrchin";
                    break;
                case 13:
                    type = "BoxJellyfish";
                    break;
                case 14:
                    type = "MorayEel";
                    break;
                case 15:
                    type = "DevilFish";
                    break;
                case 16:
                    type = "Cuttlefish";
                    break;
                case 17:
                    type = "ToxicMinnow";
                    break;
                case 18:
                    type = "Viperfish";
                    break;
                case 19:
                    type = "LuminousCorvina";
                    break;
                case 20:
                    type = "GiantSquid";
                    break;
                case 21:
                    type = "Laserfish";
                    break;
                case 22:
                    type = "OarfishHead";
                    break;
                case 23:
                    type = "ColossalSquid";
                    break;
                case 24:
                    type = "MirageJelly";
                    break;
                case 25:
                    type = "Eidolist";
                    break;
                case 26:
                    type = "GulperEelHead";
                    break;
                case 27:
                    type = "EidolonWyrmHead";
                    break;
                case 28:
                    type = "Bloatfish";
                    break;
                case 29:
                    type = "BobbitWormHead";
                    break;
                case 30:
                    type = "ChaoticPuffer";
                    break;
                case 31:
                    type = "AstralProbe";
                    break;
                case 32:
                    type = "SmallSightseer";
                    break;
                case 33:
                    type = "BigSightseer";
                    break;
                case 34:
                    type = "Aries";
                    break;
                case 35:
                    type = "AstralSlime";
                    break;
                case 36:
                    type = "Atlas";
                    break;
                case 37:
                    type = "Mantis";
                    break;
                case 38:
                    type = "Nova";
                    break;
                case 39:
                    type = "AstralachneaGround";
                    break;
                case 40:
                    type = "Hive";
                    break;
                case 41:
                    type = "StellarCulex";
                    break;
                case 42:
                    type = "FusionFeeder";
                    break;
                case 43:
                    type = "Hadarian";
                    break;
                case 44:
                    type = "HeatSpirit";
                    break;
                case 45:
                    type = "Scryllar";
                    break;
                case 46:
                    type = "DespairStone";
                    break;
                case 47:
                    type = "SoulSlurper";
                    break;
                case 48:
                    type = "ImpiousImmolator";
                    break;
                case 49:
                    type = "ScornEater";
                    break;
                case 50:
                    type = "ProfanedEnergyBody";
                    break;
                case 52:
                    type = "WulfrumDrone";
                    break;
                case 53:
                    type = "Pitbull";
                    break;
                case 54:
                    type = "BlightedEye";
                    break;
                case 55:
                    type = "CalamityEye";
                    break;
                case 56:
                    type = "Sunskater";
                    break;
                case 57:
                    type = "ShockstormShuttle";
                    break;
                case 58:
                    type = "ThiccWaifu";
                    break;
                case 59:
                    type = "AngryDog";
                    break;
                case 60:
                    type = "Cryon";
                    break;
                case 61:
                    type = "IceClasper";
                    break;
                case 62:
                    type = "StormlionCharger";
                    break;
                case 63:
                    type = "Cnidrion";
                    break;
                case 64:
                    type = "SandTortoise";
                    break;
                case 65:
                    type = "GreatSandShark";
                    break;
                case 66:
                    type = "CrawlerAmethyst";
                    break;
                case 67:
                    type = "CrawlerTopaz";
                    break;
                case 68:
                    type = "CrawlerSapphire";
                    break;
                case 69:
                    type = "CrawlerEmerald";
                    break;
                case 70:
                    type = "CrawlerRuby";
                    break;
                case 71:
                    type = "CrawlerDiamond";
                    break;
                case 72:
                    type = "CrawlerAmber";
                    break;
                case 73:
                    type = "CrawlerCrystal";
                    break;
                case 74:
                    type = "SunBat";
                    break;
                case 75:
                    type = "CosmicElemental";
                    break;
                case 76:
                    type = "Horse";
                    break;
                case 77:
                    type = "ArmoredDiggerHead";
                    break;
                case 78:
                    type = "PlaguedFlyingFox";
                    break;
                case 79:
                    type = "PlaguedJungleSlime";
                    break;
                case 80:
                    type = "PlaguedTortoise";
                    break;
                case 81:
                    type = "PlagueBee";
                    break;
                case 82:
                    type = "PlaguedDerpling";
                    break;
                case 83:
                    type = "PlaguebringerShade";
                    break;
                case 84:
                    type = "PhantomSpirit";
                    break;
                case 85:
                    type = "OverloadedSoldier";
                    break;
                case 86:
                    type = "PhantomDebris";
                    break;
                case 87:
                    type = "Bohldohr";
                    break;
                case 88:
                    type = "EbonianBlightSlime";
                    break;
                case 89:
                    type = "CrimulanBlightSlime";
                    break;
                case 90:
                    type = "AeroSlime";
                    break;
                case 91:
                    type = "CryoSlime";
                    break;
                case 92:
                    type = "PerennialSlime";
                    break;
                case 93:
                    type = "CharredSlime";
                    break;
                case 94:
                    type = "BloomSlime";
                    break;
                case 95:
                    type = "CultistAssassin";
                    break;
                case 96:
                    type = "Reaper";
                    break;
                case 97:
                    type = "IrradiatedSlime";
                    break;
                case 98:
                    type = "PrismTurtle";
                    break;
                case 99:
                    type = "Clam";
                    break;
                case 100:
                    type = "EutrophicRay";
                    break;
                case 101:
                    type = "GhostBell";
                    break;
                case 102:
                    type = "GhostBellSmall";
                    break;
                case 103:
                    type = "SeaFloaty";
                    break;
                case 104:
                    type = "BlindedAngler";
                    break;
                case 105:
                    type = "SeaMinnow";
                    break;
                case 106:
                    type = "SeaSerpent1";
                    break;
                case 107:
                    type = "GiantClam";
                    break;
                case 108:
                    type = "Piggy";
                    break;
                case 109:
                    type = "FearlessGoldfishWarrior";
                    break;
                case 110:
                    type = "Radiator";
                    break;
                case 111:
                    type = "Trilobite";
                    break;
                case 112:
                    type = "Orthocera";
                    break;
                case 113:
                    type = "Skyfin";
                    break;
                case 114:
                    type = "WaterLeech";
                    break;
                case 115:
                    type = "AcidEel";
                    break;
                case 116:
                    type = "NuclearToad";
                    break;
                case 117:
                    type = "FlakCrab";
                    break;
                case 118:
                    type = "SulfurousSkater";
                    break;
                case 119:
                    type = "FlakBaby";
                    break;
                case 120:
                    type = "AnthozoanCrab";
                    break;
                case 121:
                    type = "BelchingCoral";
                    break;
                case 122:
                    type = "GammaSlime";
                    break;
                case 123:
                    type = "WulfrumGyrator";
                    break;
                case 124:
                    type = "WulfrumHovercraft";
                    break;
                case 125:
                    type = "WulfrumRover";
                    break;
                case 126:
                    type = "WulfrumPylon";
                    break;
                default:
                    return;
            }
			if (type.Length > 0)
			{
				player.NPCBannerBuff[mod.NPCType(type)] = true;
				player.hasBanner = true;
			}
        }

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            if (i % 2 == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
        }
    }
}
