using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;
using CalamityMod.Events;
using System.Collections.Generic;
using System.Linq;
using Terraria.Localization;
using static CalamityMod.CalamityUtils;
using System;
using Terraria.ID;
using ReLogic.Content;
using Terraria.Audio;
using Terraria.ModLoader.IO;
using CalamityMod.Items.Armor.Demonshade;

namespace CalamityMod.Systems
{
    public class DifficultyModeSystem : ModSystem
    {
        internal static bool _hasCheckedItOutYet = false; //Simple variable to add a cool effect to the mode selector 

        public static DifficultyMode[] Difficulties; //Difficulty modes ordered by ascending difficulty
        public static List<DifficultyMode[]> DifficultyTiers; //Difficulty modes grouped together by difficulty
        public static int MostAlternateDifficulties; //The most alternate difficulties at any tier that exists. Used to know the widest space to take in the ui

        public override void Load()
        {
            MostAlternateDifficulties = 1;
            Difficulties = new DifficultyMode[] { new NoDifficulty(), new RevengeanceDifficulty(), new DeathDifficulty(), new WhereMalice()};

            //All of this should happen after a hook happens which lets other mods add their own difficulties
            Difficulties = Difficulties.OrderBy(d => d.DifficultyScale).ToArray();

            DifficultyTiers = new List<DifficultyMode[]>();
            float currentTier = -1;
            int tierIndex = -1;

            for (int i = 0; i < Difficulties.Length; i++)
            {
                //if we are at a new tier, create a new list of difficulties at that tier.
                if (currentTier != Difficulties[i].DifficultyScale)
                {
                    DifficultyTiers.Add(new DifficultyMode[] { Difficulties[i] });
                    currentTier = Difficulties[i].DifficultyScale;
                    tierIndex++;
                }

                //if the tier already exists, just add it to the list of other difficulties at that tier.
                else
                {
                    //ugly
                    DifficultyTiers[tierIndex] = DifficultyTiers[tierIndex].Append(Difficulties[i]).ToArray();

                    MostAlternateDifficulties = Math.Max(DifficultyTiers[tierIndex].Length, MostAlternateDifficulties);
                }

                Difficulties[i]._difficultyTier = tierIndex;
            }
        }

        public override void Unload()
        {
            Difficulties = null;
        }

        public static DifficultyMode GetCurrentDifficulty
        {
            get
            {
                DifficultyMode mode = Difficulties[0];

                for (int i = 1; i < Difficulties.Length; i++)
                {
                    if (Difficulties[i].Enabled)
                        mode = Difficulties[i];
                }

                return mode;
            }
        }

        public override void SaveWorldData(TagCompound tag)
        {
            //Apparently this is ran after worldgen so it cant always be set to true
             tag["hasCheckedOutTheCoolDifficultyUI"] = _hasCheckedItOutYet;
        }

        public override void OnWorldLoad()
        {
            _hasCheckedItOutYet = false;
        }

        public override void OnWorldUnload()
        {
            _hasCheckedItOutYet = false;
        }

        public override void PostWorldGen()
        {
            _hasCheckedItOutYet = false;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            _hasCheckedItOutYet = tag.GetBool("hasCheckedOutTheCoolDifficultyUI");

            //No need to check it out if rev is already on (Such as in old worlds)
            if (CalamityWorld.revenge)
                _hasCheckedItOutYet = true;
        }
    }

    public abstract class DifficultyMode
    {
        public abstract bool Enabled
        {
            get; set;
        }

        public abstract Asset<Texture2D> Texture { get; }
        public virtual string ExpandedDescription => "";

        public float DifficultyScale;
        public string Name;
        public string ShortDescription;

        public string ActivationTextKey;
        public string DeactivationTextKey;

        public SoundStyle ActivationSound;

        public Color ChatTextColor;

        internal int _difficultyTier;

        /// <summary>
        /// Used to know which difficulties to toggle on when selecting a particular difficulty.
        /// </summary>
        public virtual bool RequiresDifficulty(DifficultyMode mode) => false;

        public virtual int FavoredDifficultyAtTier(int tier) => 0;
    }

    public class NoDifficulty : DifficultyMode
    {
        public override bool Enabled
        {
            get => true;
            set { }
        }

        private Asset<Texture2D> _texture;
        public override Asset<Texture2D> Texture
        {
            get
            {
                if (_texture == null)
                    _texture = ModContent.Request<Texture2D>("CalamityMod/UI/ModeIndicator/ModeIndicator_None");

                return _texture;
            }
        }
        
        public NoDifficulty()
        {
            DifficultyScale = 0;
            Name = "None";
            ShortDescription = "The classic Terraria experience, with no Calamity difficulty changes";
            
            ActivationTextKey = "";
            DeactivationTextKey = "";

            ActivationSound = SoundID.MenuTick with { Volume = 1f};

            ChatTextColor = Color.White;
        }
    }

    public class RevengeanceDifficulty : DifficultyMode
    {
        public override bool Enabled
        {
            get => CalamityWorld.revenge;
            set => CalamityWorld.revenge = value;
        }

        private Asset<Texture2D> _texture;
        public override Asset<Texture2D> Texture
        {
            get
            {
                if (_texture == null)
                    _texture = ModContent.Request<Texture2D>("CalamityMod/UI/ModeIndicator/ModeIndicator_Rev");

                return _texture;
            }
        }
        
        public override string ExpandedDescription
        {
            get
            {
                string mainColor = "B55362";

                string rageKey = "[c/FFCE85:" + CalamityKeybinds.RageHotKey.TooltipHotkeyString() + "]";
                string adrenKey = "[c/79DFBF:" + CalamityKeybinds.AdrenalineHotKey.TooltipHotkeyString() + "]";


                return ("[c/"+mainColor+":Enables the][c/85FFE9: Adrenaline] [c/"+mainColor+":mechanic. You gain Adrenaline while fighting bosses. Getting hit drops Adrenaline back to 0] \n" +
                        "[c/"+mainColor+":    When Adrenaline is maxed, press] " + adrenKey + " [c/"+mainColor+":for a large damage boost] \n" +
                        "[c/"+mainColor+":Enables the ][c/FF8B54:Rage][c/"+mainColor+": mechanic. You gain Rage when in proximity of enemies or by using certain items] \n" +
                        "[c/"+mainColor+":    When Rage is maxed, press] " + rageKey + " [c/"+mainColor+":for a temporary damage boost] \n" +
                        "[c/"+mainColor+":Most foes have higher stats and deal more damage] \n" +
                        "[c/"+mainColor+":Bosses have new AI mechanics, new phases, and will drop Master Mode loot. Enemies spawn more frequently] \n" +
                        "[c/F7342A:This mode is more difficult than Expert. Be sure to prepare for the challenge]");

            }
        }

        public RevengeanceDifficulty()
        {
            DifficultyScale = 0.25f;
            Name = "Revengeance";
            ShortDescription = "[c/F54254:The intended Calamity experience!]";

            ActivationTextKey = "Mods.CalamityMod.RevengeText";
            DeactivationTextKey = "Mods.CalamityMod.RevengeText2";

            ActivationSound = SoundID.Item119;

            ChatTextColor = Color.Crimson;
        }
    }

    public class DeathDifficulty : DifficultyMode
    {
        public override bool Enabled
        {
            get => CalamityWorld.death;
            set => CalamityWorld.death = value;
        }

        private Asset<Texture2D> _texture;
        public override Asset<Texture2D> Texture
        {
            get
            {
                if (_texture == null)
                    _texture = ModContent.Request<Texture2D>("CalamityMod/UI/ModeIndicator/ModeIndicator_Death");

                return _texture;
            }
        }
        
        public override string ExpandedDescription =>"[c/B834E0:All foes will pose a much larger threat with aggressive AI and increased damage] \n" +
                        "[c/B834E0:Bosses have substantially harder AI changes. Enemies are even more numerous and can easily overwhelm you] \n" +
                        "[c/B834E0:Debuffs are especially lethal and the Abyss is significantly more dangerous] \n" +
                        "[c/E945FF:Vigilance and tenacity are crucial to survival]";

        public DeathDifficulty()
        {
            DifficultyScale = 0.5f;
            Name = "Death";
            ShortDescription = "[c/C82DF7:A tougher challenge for the more experienced, or for those that want a step up from Revengeance Mode]";

            ActivationTextKey = "Mods.CalamityMod.DeathText";
            DeactivationTextKey = "Mods.CalamityMod.DeathText2";

            ActivationSound = DemonshadeHelm.ActivationSound;

            ChatTextColor = Color.MediumOrchid;
        }

        public override int FavoredDifficultyAtTier(int tier)
        {
            DifficultyMode[] tierList = DifficultyModeSystem.DifficultyTiers[tier];

            for (int i = 0; i < tierList.Length; i++)
            {
                if (tierList[i].Name == "Revengeance")
                    return i;
            }

            return 0;
        }
    }

    public class WhereMalice : DifficultyMode
    {
        public override bool Enabled
        {
            get => false;
            set { }
        }

        private Asset<Texture2D> _texture;
        public override Asset<Texture2D> Texture
        {
            get
            {
                if (_texture == null)
                    _texture = ModContent.Request<Texture2D>("CalamityMod/UI/ModeIndicator/ModeIndicator_WhereMalice");

                return _texture;
            }
        }

        public override string ExpandedDescription
        {
            get
            {
                string realIntentionsHex = (Color.Lerp(Color.White, new Color(255, 148, 122), 0.5f + 0.5f * (float)Math.Sin(Main.GlobalTimeWrappedHourly + 0.5f))).Hex3();
                string harderOptionsHex = (Color.Lerp(Color.White, Color.Gold, 0.5f + 0.5f * (float)Math.Sin(Main.GlobalTimeWrappedHourly))).Hex3();

                return "[c/EDEDED:Malice originally came about from an idea to make obtaining Legendary items less RNG-based]\n" +
                            "[c/EDEDED:It was meant to allow the player to enrage specific bosses, giving them new phases or attacks, to guarantee the drop]\n" +
                            "[c/EDEDED:However, this was watered down to what Malice was on release, being a difficulty which affected all bosses, and included RIV(Rare Item Variants)]\n" +
                            "\n" +
                            "[c/EDEDED:At the time, Malice was received poorly by a lot of players due to the bosses being overtuned to the point of being unfair, while locking items behind them]\n" +
                            "[c/939393:      ...No tester had beaten the Exo Mechs on Malice Mode when it released]\n" +
                            "[c/" + realIntentionsHex + ":The explanation was that it was meant to be a \"refight\" mode, but that was never the real intention]\n" +
                            "\n" +
                            "[c/EDEDED:Malice-exclusive drops were eventually removed, as we did not want players to feel obligated to play a poorly balanced difficulty]\n" +
                            "[c/EDEDED:Over time, Malice was balanced to be less unfair, but we realized that with the exclusive drops gone, it was not worth the effort]\n" +
                            "[c/EDEDED:This is why we came down to deleting Malice, so that dev time and effort may be spent elsewhere, notably Death Mode]\n" +
                            "\n" +
                            "[c/" + harderOptionsHex + ":We acknowledge that some players liked Malice because Death isn't challenging enough for their tastes]\n" +
                            "[c/" + harderOptionsHex + ":If you want more difficulty, try the Infernum Addon, Master Mode, higher Boss HP Config, or for amusement, For The Worthy]";

            }
        }
            
        public WhereMalice()
        {
            DifficultyScale = 1000000f; //So its always at the end of the list
            Name = "Where's Malice?";
            ShortDescription = "[c/FB7152:Malice Mode has been removed from the mod, but its AI changes will live on in Boss Rush]\n" +
                               "[c/FB7152:We sincerely apologize for the disappointment if you were planning on a playthrough]";

            ActivationTextKey = "Mods.CalamityMod.DeathText"; //Copy values from death since clicing on the icon enables death
            DeactivationTextKey = "";

            ActivationSound = DemonshadeHelm.ActivationSound;

            ChatTextColor = Color.MediumOrchid;
        }
    }
}
