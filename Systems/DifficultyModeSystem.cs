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
            Difficulties = new DifficultyMode[] { new NoDifficulty(), new RevengeanceDifficulty(), new DeathDifficulty()};

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
            //Always save it as true
             tag["hasCheckedOutTheCoolDifficultyUI"] = true;
        }

        public override void OnWorldLoad()
        {
            _hasCheckedItOutYet = false;
        }

        public override void OnWorldUnload()
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
                    _texture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/ModeIndicator_None");

                return _texture;
            }
        }
        
        public NoDifficulty()
        {
            DifficultyScale = 0;
            Name = "None";
            ShortDescription = "The classic Terraria experience, with no Calamity difficulty changes.";
            
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
                    _texture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/ModeIndicator_Rev");

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


                return ("[c/"+mainColor+":Enables the][c/85FFE9: Adrenaline] [c/"+mainColor+":mechanic. You gain Adrenaline while fighting bosses.Getting hit drops Adrenaline back to 0.] \n" +
                        "[c/"+mainColor+":    When Adrenaline is maxed press] " + adrenKey + " [c/"+mainColor+":for a large damage boost.] \n" +
                        "[c/"+mainColor+":Enables the ][c/FF8B54:Rage][c/"+mainColor+": mechanic. You gain Rage when in proximity of enemies or by using certain items.] \n" +
                        "[c/"+mainColor+":    When Rage is maxed press] " + rageKey + " [c/"+mainColor+":for a temporary damage boost.] \n" +
                        "[c/"+mainColor+":All foes have higher stats and deal more damage.] \n" +
                        "[c/"+mainColor+":Bosses have new AI mechanics and new phases.Enemies spawn more frequently.] \n" +
                        "[c/F7342A:This mode is more difficult than Expert. Be sure to prepare for the challenge.]");

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
                    _texture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/ModeIndicator_Death");

                return _texture;
            }
        }
        
        public override string ExpandedDescription
        {
            get
            {
                return ("[c/B834E0:All foes will pose a much larger threat with aggressive AI and increased damage.] \n" +
                        "[c/B834E0:Bosses have substantially harder AI changes. Enemies are even more numerous and can easily overwhelm you.] \n" +
                        "[c/B834E0:Debuffs are especially lethal and the Abyss is significantly more dangerous.] \n" +
                        "[c/E945FF:Vigilance and tenacity are crucial to survival.]");
            }
        }

        public DeathDifficulty()
        {
            DifficultyScale = 0.5f;
            Name = "Death";
            ShortDescription = "[c/C82DF7:A tougher challenge for the more experienced, or for those that want a step up from Revengeance Mode.]";

            ActivationTextKey = "Mods.CalamityMod.DeathText";
            DeactivationTextKey = "Mods.CalamityMod.DeathText2";

            ActivationSound = SoundID.ScaryScream;

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
}
