using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.Items.Tools.ClimateChange;
using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.Astral;
using CalamityMod.NPCs.Crags;
using CalamityMod.NPCs.DraedonLabThings;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.PlagueEnemies;
using CalamityMod.NPCs.SulphurousSea;
using CalamityMod.NPCs.SunkenSea;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Chat;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;


namespace CalamityMod
{
    public static partial class CalamityUtils
    {
        public static void DisplayLocalizedText(string key, Color? textColor = null)
        {
            // An attempt to bypass the need for a separate method and runtime/compile-time parameter
            // constraints by using nulls for defaults.
            if (!textColor.HasValue)
                textColor = Color.White;

            if (Main.netMode == NetmodeID.SinglePlayer)
                Main.NewText(Language.GetTextValue(key), textColor.Value);
            else if (Main.netMode == NetmodeID.Server || Main.netMode == NetmodeID.MultiplayerClient)
                ChatHelper.BroadcastChatMessage(NetworkText.FromKey(key), textColor.Value);
        }

        public static int IngredientIndex(this Recipe r, int itemID)
        {
            for (int i = 0; i < r.requiredItem.Count; ++i)
                if (r.requiredItem[i].type == itemID)
                    return i;
            return -1;
        }

        public static bool ChangeIngredientStack(this Recipe r, int itemID, int stack)
        {
            int idx = r.IngredientIndex(itemID);
            if (idx == -1)
                return false;
            r.requiredItem[idx].stack = stack;
            return true;
        }

        // Yes, this method has a use that Utils.Swap does not; You cannot use refs on array indices.
        // The CLR will not allow that. As such, a custom method must be used to achieve this.

        /// <summary>
        /// Swaps two array indices based on a temporary variable.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="index1">The first index to swap.</param>
        /// <param name="index2">The second index to swap.</param>
        public static void SwapArrayIndices<T>(ref T[] array, int index1, int index2)
        {
            T temp = array[index1];
            array[index1] = array[index2];
            array[index2] = temp;
        }

        public static T[] ShuffleArray<T>(T[] array, Random rand = null)
        {
            if (rand is null)
                rand = new Random();

            for (int i = array.Length; i > 0; --i)
            {
                int j = rand.Next(i);
                T tempElement = array[j];
                array[j] = array[i - 1];
                array[i - 1] = tempElement;
            }
            return array;
        }

        public static T[,] ShaveOffEdge<T>(this T[,] array)
        {
            if (array.GetLength(0) <= 2 || array.GetLength(1) <= 2)
                return array;

            T[,] result = new T[array.GetLength(0) - 2, array.GetLength(1) - 2];
            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    result[i, j] = array[i + 1, j + 1];
                }
            }
            return result;
        }

        /// <summary>
        /// Retrieves all the colors from a <see cref="Texture2D"/> and returns them as a 2D <see cref="Color"/> array.
        /// </summary>
        /// <param name="texture">The texture to load.</param>
        /// <returns></returns>
        public static Color[,] GetColorsFromTexture(this Texture2D texture)
        {
            Color[] alignedColors = new Color[texture.Width * texture.Height];
            texture.GetData(alignedColors); // Fills the color array with all the colors in the texture

            Color[,] colors2D = new Color[texture.Width, texture.Height];
            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    colors2D[x, y] = alignedColors[x + y * texture.Width];
                }
            }
            return colors2D;
        }

        /// <summary>
        /// Determines if a list contains an entry of a specific type. Specifically intended to account for derived types.
        /// </summary>
        /// <typeparam name="T">The base type of the collection.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="type">The type to search for.</param>
        public static bool ContainsType<T>(this IEnumerable<T> collection, Type type) => collection.Any(entry => entry.GetType() == type.GetType());

        /// <summary>
        /// Calculates the sound volume and panning for a sound which is played at the specified location in the game world.<br/>
        /// Note that sound does not play on dedicated servers or during world generation.
        /// </summary>
        /// <param name="soundPos">The position the sound is emitting from. If either X or Y is -1, the sound does not fade with distance.</param>
        /// <param name="ambient">Whether the sound is considered ambient, which makes it use the ambient sound slider in the options. Defaults to false.</param>
        /// <returns>Volume and pan, in that order. Volume is always between 0 and 1. Pan is always between -1 and 1.</returns>
        public static (float, float) CalculateSoundStats(Vector2 soundPos, bool ambient = false)
        {
            float volume = 0f;
            float pan = 0f;

            if (soundPos.X == -1f || soundPos.Y == -1f)
                volume = 1f;
            else if (WorldGen.gen || Main.dedServ || Main.netMode == NetmodeID.Server)
                volume = 0f;
            else
            {
                float topLeftX = Main.screenPosition.X - Main.screenWidth * 2f;
                float topLeftY = Main.screenPosition.Y - Main.screenHeight * 2f;

                // Sounds cannot be heard from more than ~2.5 screens away.
                // This rectangle is 5x5 screens centered on the current screen center position.
                Rectangle audibleArea = new Rectangle((int)topLeftX, (int)topLeftY, Main.screenWidth * 5, Main.screenHeight * 5);
                Rectangle soundHitbox = new Rectangle((int)soundPos.X, (int)soundPos.Y, 1, 1);
                Vector2 screenCenter = Main.screenPosition + new Vector2(Main.screenWidth * 0.5f, Main.screenHeight * 0.5f);
                if (audibleArea.Intersects(soundHitbox))
                {
                    pan = (soundPos.X - screenCenter.X) / (Main.screenWidth * 0.5f);
                    float dist = Vector2.Distance(soundPos, screenCenter);
                    volume = 1f - (dist / (Main.screenWidth * 1.5f));
                }
            }

            pan = MathHelper.Clamp(pan, -1f, 1f);
            volume = MathHelper.Clamp(volume, 0f, 1f);
            if (ambient)
                volume = Main.gameInactive ? 0f : volume * Main.ambientVolume;
            else
                volume *= Main.soundVolume;

            // This is actually done by vanilla. I guess if the sound volume gets corrupted during gameplay, you can't blast your eardrums out.
            volume = MathHelper.Clamp(volume, 0f, 1f);
            return (volume, pan);
        }

        /// <summary>
        /// Convenience function to utilize CalculateSoundStats immediately on an existing sound effect.<br/>
        /// This allows updating a looping sound every single frame to have the correct volume and pan, even if the player drags the audio sliders around.
        /// </summary>
        /// <param name="sfx">The SoundEffectInstance which is having its values updated.</param>
        /// <param name="soundPos">The position the sound is emitting from. If either X or Y is -1, the sound does not fade with distance.</param>
        /// <param name="ambient">Whether the sound is considered ambient, which makes it use the ambient sound slider in the options. Defaults to false.</param>
        public static void ApplySoundStats(ref SoundEffectInstance sfx, Vector2 soundPos, bool ambient = false)
        {
            if (sfx is null || sfx.IsDisposed)
                return;
            (sfx.Volume, sfx.Pan) = CalculateSoundStats(soundPos, ambient);
        }

        /// <summary>
        /// Method to change the volume of a sound without having to manually do a nullcheck or having to clamp it down to between 0 and 1
        /// </summary>
        /// <param name="sfx">The SoundEffectInstance which is having its values updated.</param>
        /// <param name="volumeMultiplier">How much the sound's volume should get changed</param>
        public static void SafeVolumeChange(ref SoundEffectInstance sfx, float volumeMultiplier)
        {
            if (sfx is null || sfx.IsDisposed)
                return;
            sfx.Volume = MathHelper.Clamp(sfx.Volume * volumeMultiplier, 0f, 1f);
        }

        public static void StartRain(bool torrentialTear = false, bool maxSeverity = false)
        {
            int num = 86400;
            int num2 = num / 24;
            Main.rainTime = Main.rand.Next(num2 * 8, num);
            if (Main.rand.NextBool(3))
            {
                Main.rainTime += Main.rand.Next(0, num2);
            }
            if (Main.rand.NextBool(4))
            {
                Main.rainTime += Main.rand.Next(0, num2 * 2);
            }
            if (Main.rand.NextBool(5))
            {
                Main.rainTime += Main.rand.Next(0, num2 * 2);
            }
            if (Main.rand.NextBool(6))
            {
                Main.rainTime += Main.rand.Next(0, num2 * 3);
            }
            if (Main.rand.NextBool(7))
            {
                Main.rainTime += Main.rand.Next(0, num2 * 4);
            }
            if (Main.rand.NextBool(8))
            {
                Main.rainTime += Main.rand.Next(0, num2 * 5);
            }
            float num3 = 1f;
            if (Main.rand.NextBool(2))
            {
                num3 += 0.05f;
            }
            if (Main.rand.NextBool(3))
            {
                num3 += 0.1f;
            }
            if (Main.rand.NextBool(4))
            {
                num3 += 0.15f;
            }
            if (Main.rand.NextBool(5))
            {
                num3 += 0.2f;
            }
            Main.rainTime = (int)(Main.rainTime * num3);
            Main.raining = true;
            if (torrentialTear)
                TorrentialTear.AdjustRainSeverity(maxSeverity);
            CalamityNetcode.SyncWorld();
        }

        public static void StartSandstorm()
        {
			// If it's not windy enough, make it windy enough for a sandstorm
			// 0.6f is the minimum for vanilla but Calamity changes it to 0.2f
			// Windy days occur when wind speed is at least 0.5f (0.4f in vanilla) so this should never cause a windy day
			float windSpeed = 0f;
			if (Main.windSpeedCurrent < 0.2f && Main.windSpeedCurrent > 0f)
			{
				windSpeed = Main.rand.NextFloat(0.2f, 0.4f);
			}
			else if (Main.windSpeedCurrent > -0.2f && Main.windSpeedCurrent < 0f)
			{
				windSpeed = Main.rand.NextFloat(-0.4f, -0.2f);
			}
			if (windSpeed != 0f)
			{
				Main.windSpeedCurrent = windSpeed < 0f ? -0.2f : 0.2f;
				Main.windSpeedTarget = windSpeed;
			}

            Sandstorm.StartSandstorm();
        }

        public static void StopSandstorm()
        {
            Terraria.GameContent.Events.Sandstorm.Happening = false;
        }

        public static void AddWithCondition<T>(this List<T> list, T type, bool condition)
        {
            if (condition)
                list.Add(type);
        }

        public static int SecondsToFrames(int seconds) => seconds * 60;
        public static int SecondsToFrames(float seconds) => (int)(seconds * 60);

        public static bool WithinBounds(this int index, int cap) => index >= 0 && index < cap;

        /// Clamps the distance between vectors via normalization.
        /// </summary>
        /// <param name="start">The starting point.</param>
        /// <param name="end">The ending point.</param>
        /// <param name="maxDistance">The maximum possible distance between the two vectors before they get clamped.</param>
        public static void DistanceClamp(ref Vector2 start, ref Vector2 end, float maxDistance)
        {
            if (Vector2.Distance(end, start) > maxDistance)
            {
                end = start + Vector2.Normalize(end - start) * maxDistance;
            }
        }

        public static void ChangeTime(bool changeToDay)
        {
            Main.time = 0D;
            Main.dayTime = changeToDay;
            CalamityNetcode.SyncWorld();
        }

        public static bool IntoMorseCode(string originalText, float completion)
        {
            int spaceLength = 13;
            int betweenLetterLength = 7;
            int betweenBlipLength = 4;
            int shortLength = 3;
            int longLength = 8;

            char[] TextKeys = { ' ', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r',
                's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
            string[] MorseKeys = { " ", ".-|", "-...|", "-.-. |", "-..|", ".|", "..-.|"
                    , "--.|", "....|", "..|", ".---|","-.-|",".-..|","--|",
                      "-.|","---|",".--.|","--.-|",".-.|","...|","-|","..-|",
                      "...-|",".--|","-..-|","-.--|","--..|",".----|",
                      "..---|","...--|","....-|",".....|","-....|","--...|",
                      "---..|","----.|","-----|" };

            string morseText = "";
            originalText = originalText.ToLower();

            //Construct a string of text that replaces all the stuff with morse.
            for (int i = 0; i < originalText.Length; i++)
            {
                for (int j = 0; j < 37; j++)
                {
                    if (TextKeys[j] == originalText[i])
                    {
                        morseText += MorseKeys[j];
                        break;
                    }
                }
            }

            List<bool> morseState = new List<bool>();

            for (int i = 0; i < morseText.Length; i++)
            {
                if (morseText[i] == " ".ToCharArray()[0])
                    morseState.AddRange(Enumerable.Repeat(false, spaceLength));

                if (morseText[i] == "|".ToCharArray()[0])
                    morseState.AddRange(Enumerable.Repeat(false, betweenLetterLength));

                if (morseText[i] == ".".ToCharArray()[0])
                    morseState.AddRange(Enumerable.Repeat(true, shortLength));

                if (morseText[i] == "-".ToCharArray()[0])
                    morseState.AddRange(Enumerable.Repeat(true, longLength));

                morseState.AddRange(Enumerable.Repeat(false, betweenBlipLength));
            }

            return morseState[(int)((morseState.Count - 1) * completion)];
        }

        public static int GetBannerNPC(int style)
        {
            int npc = -1;
            switch (style)
            {
                case 0:
                    npc = ModContent.NPCType<RepairUnitCritter>();
                    break;
                case 1:
                    npc = ModContent.NPCType<Sulflounder>();
                    break;
                case 2:
                    npc = ModContent.NPCType<Gnasher>();
                    break;
                case 3:
                    npc = ModContent.NPCType<Trasher>();
                    break;
                case 4:
                    npc = ModContent.NPCType<Toxicatfish>();
                    break;
                case 6:
                    npc = ModContent.NPCType<Androomba>();
                    break;
                case 7:
                    npc = ModContent.NPCType<AquaticUrchin>();
                    break;
                case 8:
                    npc = ModContent.NPCType<Frogfish>();
                    break;
                case 9:
                    npc = ModContent.NPCType<MantisShrimp>();
                    break;
                case 12:
                    npc = ModContent.NPCType<SeaUrchin>();
                    break;
                case 13:
                    npc = ModContent.NPCType<BoxJellyfish>();
                    break;
                case 14:
                    npc = ModContent.NPCType<MorayEel>();
                    break;
                case 15:
                    npc = ModContent.NPCType<DevilFish>();
                    break;
                case 16:
                    npc = ModContent.NPCType<Cuttlefish>();
                    break;
                case 17:
                    npc = ModContent.NPCType<ToxicMinnow>();
                    break;
                case 18:
                    npc = ModContent.NPCType<Viperfish>();
                    break;
                case 19:
                    npc = ModContent.NPCType<LuminousCorvina>();
                    break;
                case 20:
                    npc = ModContent.NPCType<GiantSquid>();
                    break;
                case 21:
                    npc = ModContent.NPCType<Laserfish>();
                    break;
                case 22:
                    npc = ModContent.NPCType<OarfishHead>();
                    break;
                case 23:
                    npc = ModContent.NPCType<ColossalSquid>();
                    break;
                case 24:
                    npc = ModContent.NPCType<MirageJelly>();
                    break;
                case 25:
                    npc = ModContent.NPCType<Eidolist>();
                    break;
                case 26:
                    npc = ModContent.NPCType<GulperEelHead>();
                    break;
                case 27:
                    npc = ModContent.NPCType<EidolonWyrmHead>();
                    break;
                case 28:
                    npc = ModContent.NPCType<Bloatfish>();
                    break;
                case 29:
                    npc = ModContent.NPCType<BobbitWormHead>();
                    break;
                case 30:
                    npc = ModContent.NPCType<ChaoticPuffer>();
                    break;
                case 31:
                    npc = ModContent.NPCType<AstralProbe>();
                    break;
                case 32:
                    npc = ModContent.NPCType<SightseerCollider>();
                    break;
                case 33:
                    npc = ModContent.NPCType<SightseerSpitter>();
                    break;
                case 34:
                    npc = ModContent.NPCType<Aries>();
                    break;
                case 35:
                    npc = ModContent.NPCType<AstralSlime>();
                    break;
                case 36:
                    npc = ModContent.NPCType<Atlas>();
                    break;
                case 37:
                    npc = ModContent.NPCType<Mantis>();
                    break;
                case 38:
                    npc = ModContent.NPCType<Nova>();
                    break;
                case 39:
                    npc = ModContent.NPCType<AstralachneaGround>();
                    break;
                case 40:
                    npc = ModContent.NPCType<HiveEnemy>();
                    break;
                case 41:
                    npc = ModContent.NPCType<StellarCulex>();
                    break;
                case 42:
                    npc = ModContent.NPCType<FusionFeeder>();
                    break;
                case 43:
                    npc = ModContent.NPCType<Hadarian>();
                    break;
                case 44:
                    npc = ModContent.NPCType<HeatSpirit>();
                    break;
                case 45:
                    npc = ModContent.NPCType<Scryllar>();
                    break;
                case 46:
                    npc = ModContent.NPCType<DespairStone>();
                    break;
                case 47:
                    npc = ModContent.NPCType<SoulSlurper>();
                    break;
                case 48:
                    npc = ModContent.NPCType<ImpiousImmolator>();
                    break;
                case 49:
                    npc = ModContent.NPCType<ScornEater>();
                    break;
                case 50:
                    npc = ModContent.NPCType<ProfanedEnergyBody>();
                    break;
                case 52:
                    npc = ModContent.NPCType<WulfrumDrone>();
                    break;
                case 53:
                    npc = ModContent.NPCType<Rotdog>();
                    break;
                case 55:
                    npc = ModContent.NPCType<CalamityEye>();
                    break;
                case 56:
                    npc = ModContent.NPCType<Sunskater>();
                    break;
                case 57:
                    npc = ModContent.NPCType<ShockstormShuttle>();
                    break;
                case 58:
                    npc = ModContent.NPCType<ThiccWaifu>();
                    break;
                case 59:
                    npc = ModContent.NPCType<Rimehound>();
                    break;
                case 60:
                    npc = ModContent.NPCType<Cryon>();
                    break;
                case 61:
                    npc = ModContent.NPCType<IceClasper>();
                    break;
                case 62:
                    npc = ModContent.NPCType<Stormlion>();
                    break;
                case 63:
                    npc = ModContent.NPCType<Cnidrion>();
                    break;
                case 66:
                    npc = ModContent.NPCType<CrawlerAmethyst>();
                    break;
                case 67:
                    npc = ModContent.NPCType<CrawlerTopaz>();
                    break;
                case 68:
                    npc = ModContent.NPCType<CrawlerSapphire>();
                    break;
                case 69:
                    npc = ModContent.NPCType<CrawlerEmerald>();
                    break;
                case 70:
                    npc = ModContent.NPCType<CrawlerRuby>();
                    break;
                case 71:
                    npc = ModContent.NPCType<CrawlerDiamond>();
                    break;
                case 72:
                    npc = ModContent.NPCType<CrawlerAmber>();
                    break;
                case 73:
                    npc = ModContent.NPCType<CrawlerCrystal>();
                    break;
                case 76:
                    npc = ModContent.NPCType<Horse>();
                    break;
                case 77:
                    npc = ModContent.NPCType<ArmoredDiggerHead>();
                    break;
                case 78:
                    npc = ModContent.NPCType<Melter>();
                    break;
                case 79:
                    npc = ModContent.NPCType<PestilentSlime>();
                    break;
                case 80:
                    npc = ModContent.NPCType<Plagueshell>();
                    break;
                case 81:
                    npc = ModContent.NPCType<PlagueCharger>();
                    break;
                case 82:
                    npc = ModContent.NPCType<Viruling>();
                    break;
                case 83:
                    npc = ModContent.NPCType<PlaguebringerMiniboss>();
                    break;
                case 84:
                    npc = ModContent.NPCType<PhantomSpirit>();
                    break;
                case 85:
                    npc = ModContent.NPCType<OverloadedSoldier>();
                    break;
                case 87:
                    npc = ModContent.NPCType<Bohldohr>();
                    break;
                case 88:
                    npc = ModContent.NPCType<EbonianBlightSlime>();
                    break;
                case 89:
                    npc = ModContent.NPCType<CrimulanBlightSlime>();
                    break;
                case 90:
                    npc = ModContent.NPCType<AeroSlime>();
                    break;
                case 91:
                    npc = ModContent.NPCType<CryoSlime>();
                    break;
                case 92:
                    npc = ModContent.NPCType<PerennialSlime>();
                    break;
                case 93:
                    npc = ModContent.NPCType<InfernalCongealment>();
                    break;
                case 94:
                    npc = ModContent.NPCType<BloomSlime>();
                    break;
                case 95:
                    npc = ModContent.NPCType<RenegadeWarlock>();
                    break;
                case 96:
                    npc = ModContent.NPCType<ReaperShark>();
                    break;
                case 97:
                    npc = ModContent.NPCType<IrradiatedSlime>();
                    break;
                case 98:
                    npc = ModContent.NPCType<PrismBack>();
                    break;
                case 99:
                    npc = ModContent.NPCType<Clam>();
                    break;
                case 100:
                    npc = ModContent.NPCType<EutrophicRay>();
                    break;
                case 101:
                    npc = ModContent.NPCType<GhostBell>();
                    break;
                case 102:
                    npc = ModContent.NPCType<BabyGhostBell>();
                    break;
                case 103:
                    npc = ModContent.NPCType<SeaFloaty>();
                    break;
                case 104:
                    npc = ModContent.NPCType<BlindedAngler>();
                    break;
                case 105:
                    npc = ModContent.NPCType<SeaMinnow>();
                    break;
                case 106:
                    npc = ModContent.NPCType<SeaSerpent1>();
                    break;
                case 108:
                    npc = ModContent.NPCType<Piggy>();
                    break;
                case 109:
                    npc = ModContent.NPCType<FearlessGoldfishWarrior>();
                    break;
                case 110:
                    npc = ModContent.NPCType<Radiator>();
                    break;
                case 111:
                    npc = ModContent.NPCType<Trilobite>();
                    break;
                case 112:
                    npc = ModContent.NPCType<Orthocera>();
                    break;
                case 113:
                    npc = ModContent.NPCType<Skyfin>();
                    break;
                case 115:
                    npc = ModContent.NPCType<AcidEel>();
                    break;
                case 116:
                    npc = ModContent.NPCType<NuclearToad>();
                    break;
                case 117:
                    npc = ModContent.NPCType<FlakCrab>();
                    break;
                case 118:
                    npc = ModContent.NPCType<SulphurousSkater>();
                    break;
                case 119:
                    npc = ModContent.NPCType<BabyFlakCrab>();
                    break;
                case 120:
                    npc = ModContent.NPCType<AnthozoanCrab>();
                    break;
                case 121:
                    npc = ModContent.NPCType<BelchingCoral>();
                    break;
                case 122:
                    npc = ModContent.NPCType<GammaSlime>();
                    break;
                case 123:
                    npc = ModContent.NPCType<WulfrumGyrator>();
                    break;
                case 124:
                    npc = ModContent.NPCType<WulfrumHovercraft>();
                    break;
                case 125:
                    npc = ModContent.NPCType<WulfrumRover>();
                    break;
                case 126:
                    npc = ModContent.NPCType<WulfrumAmplifier>();
                    break;
                default:
                    break;
            }
            return npc;
        }
    }
}
