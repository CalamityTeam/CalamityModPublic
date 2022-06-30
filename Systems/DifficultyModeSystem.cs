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

namespace CalamityMod.Systems
{
    public class DifficultyModeSystem : ModSystem
    {

        public static DifficultyMode[] Difficulties;
        public static List<DifficultyMode[]> DifficultyTiers;
        public static int MostAlternateDifficulties;

        public override void Load()
        {
            MostAlternateDifficulties = 1;
            Difficulties = new DifficultyMode[] { new NoDifficulty(), new RevengeanceDifficulty(), new DeathDifficulty(), new MaliceDifficulty() };

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
    }

    public abstract class DifficultyMode
    {
        public abstract bool Enabled
        {
            get; set;
        }

        public abstract Asset<Texture2D> Texture { get; }

        public float DifficultyScale;
        public string Name;
        public string ShortDescription;
        public string ExpandedDescription;

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
            ShortDescription = "The classic Terraria experience, with no Calamity difficulty changes";
            ExpandedDescription = "Blah Blah";

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

        public RevengeanceDifficulty()
        {
            DifficultyScale = 0.25f;
            Name = "Revengeance";
            ShortDescription = "[c/F7412D:Calamity's intended difficulty. Switches up bossfights by giving them new AI, aand unlocks new mechanics]";
            ExpandedDescription = "Gloubigboulga";

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

        public DeathDifficulty()
        {
            DifficultyScale = 0.5f;
            Name = "Death";
            ShortDescription = "[c/C82DF7:One step above Revengeance, this mode includes more CRAZY ZANY tweaks!]";
            ExpandedDescription = "Gloobologie";

            ActivationTextKey = "Mods.CalamityMod.DeathText";
            DeactivationTextKey = "Mods.CalamityMod.DeathText2";

            ActivationSound = SoundID.NPCHit39;

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

    public class MaliceDifficulty : DifficultyMode
    {
        public override bool Enabled
        {
            get => CalamityWorld.malice;
            set => CalamityWorld.malice = value;
        }

        private Asset<Texture2D> _texture;
        public override Asset<Texture2D> Texture
        {
            get
            {
                if (_texture == null)
                    _texture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/ModeIndicator_Malice");

                return _texture;
            }
        }

        public MaliceDifficulty()
        {
            DifficultyScale = 0.75f;
            Name = "Malice";
            ShortDescription = "[c/F7CF2D:Enrages bosses so you may brag about it in calamity mod discord]";
            ExpandedDescription = "Malicious";

            ActivationTextKey = "Mods.CalamityMod.MaliceText";
            DeactivationTextKey = "Mods.CalamityMod.MaliceText2";

            ActivationSound = SoundID.NPCDeath38;

            ChatTextColor = Color.Gold;
        }

        public override int FavoredDifficultyAtTier(int tier)
        {
            DifficultyMode[] tierList = DifficultyModeSystem.DifficultyTiers[tier];

            for (int i = 0; i < tierList.Length; i++)
            {
                if (tierList[i].Name == "Revengeance" || tierList[i].Name == "Death")
                    return i;
            }

            return 0;
        }
    }
}
