using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.NPCs.Astral;
using CalamityMod.NPCs.Crags;
using CalamityMod.NPCs.Deconstructors;
using CalamityMod.NPCs.DraedonLabThings;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.NormalNPCs.HorribleHog;
using CalamityMod.NPCs.PlagueEnemies;
using CalamityMod.NPCs.SulphurousSea;
using CalamityMod.NPCs.SunkenSea;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Tiles
{
    public class MonsterBanner : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.MultiTileSway[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.SolidBottom | AnchorType.PlanterBox, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.DrawYOffset = -2;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.Platform, TileObjectData.newTile.Width, 0);
            TileObjectData.newAlternate.DrawYOffset = -10;
            TileObjectData.addAlternate(0);
            TileObjectData.addTile(Type);
            DustType = -1;
            AddMapEntry(new Color(13, 88, 130), Language.GetText("MapObject.Banner"));
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (closer)
                return;

            int style = Main.tile[i, j].TileFrameX / 18;
            int npc = GetBannerNPC(style);
            if (npc == -1)
                return;

            int itemType = TileLoader.GetItemDropFromTypeAndStyle(Type, style);
            if (ItemID.Sets.BannerStrength.IndexInRange(itemType) && ItemID.Sets.BannerStrength[itemType].Enabled)
            {
                Main.SceneMetrics.NPCBannerBuff[npc] = true;
                Main.SceneMetrics.hasBanner = true;
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) => CalamityUtils.DrawSwayingMultiTile(i, j);

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) => offsetY += 2;

        public static int GetBannerNPC(int style)
        {
            int npc = -1;
            switch (style)
            {
                case 0:
                    npc = NPCType<RepairUnitCritter>();
                    break;
                case 1:
                    npc = NPCType<Sulflounder>();
                    break;
                case 2:
                    npc = NPCType<Gnasher>();
                    break;
                case 3:
                    npc = NPCType<Trasher>();
                    break;
                case 4:
                    npc = NPCType<Toxicatfish>();
                    break;
                case 5:
                    npc = NPCType<SlabCrab>();
                    break;
                case 6:
                    npc = NPCType<Androomba>();
                    break;
                case 7:
                    npc = NPCType<AquaticUrchin>();
                    break;
                case 8:
                    npc = NPCType<Frogfish>();
                    break;
                case 9:
                    npc = NPCType<MantisShrimp>();
                    break;
                case 10:
                    npc = NPCType<AuroraSpirit>();
                    break;
                case 11:
                    npc = NPCType<WildBumblebirb>(); // There's also the boss variant but I dont think we want banners to affect them?
                    break;
                /*case 12:
                    npc = NPCType<Unused>(); - Formerly Sea Urchin
                    break*/
                case 13:
                    npc = NPCType<BoxJellyfish>();
                    break;
                case 14:
                    npc = NPCType<MorayEel>();
                    break;
                case 15:
                    npc = NPCType<DevilFish>();
                    break;
                case 16:
                    npc = NPCType<Cuttlefish>();
                    break;
                case 17:
                    npc = NPCType<ToxicMinnow>();
                    break;
                case 18:
                    npc = NPCType<Viperfish>();
                    break;
                case 19:
                    npc = NPCType<LuminousCorvina>();
                    break;
                case 20:
                    npc = NPCType<GiantSquid>();
                    break;
                case 21:
                    npc = NPCType<Laserfish>();
                    break;
                case 22:
                    npc = NPCType<OarfishHead>();
                    break;
                case 23:
                    npc = NPCType<ColossalSquid>();
                    break;
                case 24:
                    npc = NPCType<MirageJelly>();
                    break;
                case 25:
                    npc = NPCType<Eidolist>();
                    break;
                case 26:
                    npc = NPCType<GulperEelHead>();
                    break;
                case 27:
                    npc = NPCType<EidolonWyrmHead>();
                    break;
                case 28:
                    npc = NPCType<Bloatfish>();
                    break;
                case 29:
                    npc = NPCType<BobbitWormHead>();
                    break;
                case 30:
                    npc = NPCType<ChaoticPuffer>();
                    break;
                case 31:
                    npc = NPCType<AstralProbe>();
                    break;
                case 32:
                    npc = NPCType<SightseerCollider>();
                    break;
                case 33:
                    npc = NPCType<SightseerSpitter>();
                    break;
                case 34:
                    npc = NPCType<Aries>();
                    break;
                case 35:
                    npc = NPCType<AstralSlime>();
                    break;
                case 36:
                    npc = NPCType<Atlas>();
                    break;
                case 37:
                    npc = NPCType<Mantis>();
                    break;
                case 38:
                    npc = NPCType<Nova>();
                    break;
                case 39:
                    npc = NPCType<AstralachneaGround>();
                    break;
                case 40:
                    npc = NPCType<Astraglomerate>();
                    break;
                case 41:
                    npc = NPCType<StellarCulex>();
                    break;
                case 42:
                    npc = NPCType<FusionFeeder>();
                    break;
                case 43:
                    npc = NPCType<Hadarian>();
                    break;
                case 44:
                    npc = NPCType<HeatSpirit>();
                    break;
                case 45:
                    npc = NPCType<Scryllar>();
                    break;
                case 46:
                    npc = NPCType<DespairStone>();
                    break;
                case 47:
                    npc = NPCType<SoulSlurper>();
                    break;
                case 48:
                    npc = NPCType<ImpiousImmolator>();
                    break;
                case 49:
                    npc = NPCType<ScornEater>();
                    break;
                case 50:
                    npc = NPCType<ProfanedEnergyBody>();
                    break;
                case 51:
                    npc = NPCType<Shroomble>();
                    break;
                case 52:
                    npc = NPCType<WulfrumDrone>();
                    break;
                case 53:
                    npc = NPCType<Rotdog>();
                    break;
                case 54:
                    npc = NPCType<CladCrab>();
                    break;
                case 55:
                    npc = NPCType<CalamityEye>();
                    break;
                case 56:
                    npc = NPCType<Sunskater>();
                    break;
                case 57:
                    npc = NPCType<ShockstormShuttle>();
                    break;
                case 58:
                    npc = NPCType<CloudElemental>();
                    break;
                case 59:
                    npc = NPCType<Rimehound>();
                    break;
                case 60:
                    npc = NPCType<Cryon>();
                    break;
                case 61:
                    npc = NPCType<IceClasper>();
                    break;
                case 62:
                    npc = NPCType<Stormlion>();
                    break;
                case 63:
                    npc = NPCType<Cnidrion>();
                    break;
                /*case 64:
                    npc = NPCType<Unused>(); - Formerly Sand Tortoise
                    break*/
                /*case 65:
                    npc = NPCType<Unused>(); - Formerly Great Sand Shark
                    break*/
                case 66:
                    npc = NPCType<CrawlerAmethyst>();
                    break;
                case 67:
                    npc = NPCType<CrawlerTopaz>();
                    break;
                case 68:
                    npc = NPCType<CrawlerSapphire>();
                    break;
                case 69:
                    npc = NPCType<CrawlerEmerald>();
                    break;
                case 70:
                    npc = NPCType<CrawlerRuby>();
                    break;
                case 71:
                    npc = NPCType<CrawlerDiamond>();
                    break;
                case 72:
                    npc = NPCType<CrawlerAmber>();
                    break;
                case 73:
                    npc = NPCType<CrawlerCrystal>();
                    break;
                /*case 74:
                    npc = NPCType<Unused>(); - Formerly Sun Bat
                    break*/
                /*case 75:
                    npc = NPCType<Unused>(); - Formerly Cosmic Elemental
                    break*/
                case 76:
                    npc = NPCType<EarthElemental>();
                    break;
                case 77:
                    npc = NPCType<Burrower>();
                    break;
                case 78:
                    npc = NPCType<Melter>();
                    break;
                case 79:
                    npc = NPCType<PestilentSlime>();
                    break;
                case 80:
                    npc = NPCType<Plagueshell>();
                    break;
                case 81:
                    npc = NPCType<PlagueCharger>();
                    break;
                case 82:
                    npc = NPCType<Viruling>();
                    break;
                case 83:
                    npc = NPCType<PlaguebringerMiniboss>();
                    break;
                case 84:
                    npc = NPCType<PhantomSpirit>();
                    break;
                case 85:
                    npc = NPCType<OverloadedSoldier>();
                    break;
                /*case 86:
                    npc = NPCType<Unused>(); - Formerly Phantom Debris
                    break*/
                case 87:
                    npc = NPCType<Bohldohr>();
                    break;
                case 88:
                    npc = NPCType<EbonianBlightSlime>();
                    break;
                case 89:
                    npc = NPCType<CrimulanBlightSlime>();
                    break;
                case 90:
                    npc = NPCType<AeroSlime>();
                    break;
                case 91:
                    npc = NPCType<CryoSlime>();
                    break;
                case 92:
                    npc = NPCType<PerennialSlime>();
                    break;
                case 93:
                    npc = NPCType<InfernalCongealment>();
                    break;
                case 94:
                    npc = NPCType<BloomSlime>();
                    break;
                case 95:
                    npc = NPCType<RenegadeWarlock>();
                    break;
                case 96:
                    npc = NPCType<ReaperShark>();
                    break;
                case 97:
                    npc = NPCType<IrradiatedSlime>();
                    break;
                case 98:
                    npc = NPCType<PrismBack>();
                    break;
                case 99:
                    npc = NPCType<Clam>();
                    break;
                case 100:
                    npc = NPCType<EutrophicRay>();
                    break;
                case 101:
                    npc = NPCType<GhostBell>();
                    break;
                case 102:
                    npc = NPCType<BabyGhostBell>();
                    break;
                case 103:
                    npc = NPCType<SeaFloaty>();
                    break;
                case 104:
                    npc = NPCType<BlindedAngler>();
                    break;
                case 105:
                    npc = NPCType<SeaMinnow>();
                    break;
                case 106:
                    npc = NPCType<SeaSerpent1>();
                    break;
                /*case 107:
                    npc = NPCType<Unused>(); - Formerly Giant Clam
                    break*/
                case 108:
                    npc = NPCType<Piggy>();
                    break;
                case 109:
                    npc = NPCType<FearlessGoldfishWarrior>();
                    break;
                case 110:
                    npc = NPCType<Radiator>();
                    break;
                case 111:
                    npc = NPCType<Trilobite>();
                    break;
                case 112:
                    npc = NPCType<Orthocera>();
                    break;
                case 113:
                    npc = NPCType<Skyfin>();
                    break;
                /*case 114:
                    npc = NPCType<Unused>(); - Formerly Water Leech
                    break*/
                case 115:
                    npc = NPCType<AcidEel>();
                    break;
                case 116:
                    npc = NPCType<NuclearToad>();
                    break;
                case 117:
                    npc = NPCType<FlakCrab>();
                    break;
                case 118:
                    npc = NPCType<SulphurousSkater>();
                    break;
                case 119:
                    npc = NPCType<BabyFlakCrab>();
                    break;
                case 120:
                    npc = NPCType<AnthozoanCrab>();
                    break;
                case 121:
                    npc = NPCType<BelchingCoral>();
                    break;
                case 122:
                    npc = NPCType<GammaSlime>();
                    break;
                case 123:
                    npc = NPCType<WulfrumGyrator>();
                    break;
                case 124:
                    npc = NPCType<WulfrumHovercraft>();
                    break;
                case 125:
                    npc = NPCType<WulfrumRover>();
                    break;
                case 126:
                    npc = NPCType<WulfrumAmplifier>();
                    break;
                case 127:
                    npc = NPCType<CannonballJellyfish>();
                    break;
                case 128:
                    npc = NPCType<BabyCannonballJellyfish>();
                    break;
                // Hello source code diver, here are a bunch of Sunken Sea enemy names, have fun
                /*case 129:
                    npc = NPCType<Sharkoon>();
                    break; */
                /* case 130:
                    npc = NPCType<Probesnout>();
                    break; */
                /* case 131:
                    npc = NPCType<CarrionSwallower>();
                    break; */
                /* case 132:
                    npc = NPCType<GildedAxolotl>();
                    break; */
                /* case 133:
                    npc = NPCType<Grubbolt>();
                    break; */
                /* case 134:
                    npc = NPCType<HauntedChum>();
                    break; */
                /* case 135:
                    npc = NPCType<Jellyghoul>();
                    break; */
                /* case 136:
                    npc = NPCType<KelpieSeadragon>();
                    break; */
                /* case 137:
                    npc = NPCType<Leerslug>();
                    break; */
                /* case 138:
                    npc = NPCType<LostShoal>();
                    break; */
                /* case 139:
                    npc = NPCType<MirageNewt>();
                    break; */
                /* case 140:
                    npc = NPCType<Panasea>();
                    break; */
                /* case 141:
                    npc = NPCType<PearlpodWhite>(); // Logic should be applying to other variants
                    break; */
                /* case 142:
                    npc = NPCType<StormlionSentry>();
                    break; */
                /* case 143:
                    npc = NPCType<PodobooKoi>();
                    break; */
                /* case 144:
                    npc = NPCType<Polyperil>();
                    break; */
                /* case 145:
                    npc = NPCType<PrismaticGuppy>();
                    break; */
                /* case 146:
                    npc = NPCType<Scavenger>();
                    break; */
                /* case 147:
                    npc = NPCType<Searslug>();
                    break; */
                /* case 148:
                    npc = NPCType<Shoreskipper>();
                    break; */
                /* case 149:
                    npc = NPCType<Slugbun>();
                    break; */
                /* case 150:
                    npc = NPCType<Steampod>();
                    break; */
                /* case 151:
                    npc = NPCType<StormlionScout>();
                    break; */
                case 152:
                    npc = NPCType<HorribleHog>();
                    break;
                default:
                    break;
            }
            return npc;
        }
    }
}
