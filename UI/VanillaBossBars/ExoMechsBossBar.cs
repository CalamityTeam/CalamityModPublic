using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.BigProgressBar;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.UI.VanillaBossBars
{
    public class ExoMechsBossBar : ModBossBar
    {
        // Artemis will always be used to check twins, Apollo is there in spirit
        // For showing the correct boss icon when bosses go into solo
        public bool HideAres = false;
        public bool HideArtemis = false;
        public bool HideThanatos = false;

        // For properly showing the first phase health
        public bool AllBossesSpawned = false;

        // Used to determine the max health of a multi-segmented boss
        public NPC FalseNPCSegment;

        public override Asset<Texture2D> GetIconTexture(ref Rectangle? iconFrame)
        {
            // Prioritize based on which boss(es) are hidden
            if (HideArtemis && HideThanatos && !HideAres)
                return TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[NPCType<AresBody>()]];
            if (HideAres && HideThanatos && !HideArtemis)
                return ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Artemis/ArtemisHead");
            if (HideAres && HideArtemis && !HideThanatos)
                return ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Thanatos/ThanatosNormalHead");
            // Otherwise, just run through alphabet sorting: Ares, Artemis, Thanatos
            if (!HideAres)
                return TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[NPCType<AresBody>()]];
            if (!HideArtemis)
                return ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Artemis/ArtemisHead");
            return ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Thanatos/ThanatosNormalHead");
        }

        public override bool? ModifyInfo(ref BigProgressBarInfo info, ref float lifePercent, ref float shieldPercent)
        {
            ValidateAllMechs(ref info);
            NPC target = Main.npc[info.npcIndexToAimAt];

			if (!target.active && !FindMechsAgain(ref info))
            {
                // Reset the spawning bool for refights
                AllBossesSpawned = false;
                return false;
            }

            // Immediately grab the boss's health, whichever one it is. We will check later.
            int life = target.life;
            int lifeMax = target.lifeMax;

            // Checking for all bosses
            if (NPC.AnyNPCs(NPCType<AresBody>()) && NPC.AnyNPCs(NPCType<Artemis>()) && NPC.AnyNPCs(NPCType<ThanatosHead>()))
                AllBossesSpawned = true;

            // Only run this after the first phase, where every boss has spawned
            if (AllBossesSpawned)
            {
                // Override max health by feeding the data of false NPCs
                FalseNPCSegment = new NPC();
                FalseNPCSegment.SetDefaults(NPCType<AresBody>(), target.GetMatchingSpawnParams());
                lifeMax = FalseNPCSegment.lifeMax;
                FalseNPCSegment.SetDefaults(NPCType<Artemis>(), target.GetMatchingSpawnParams());
                lifeMax += FalseNPCSegment.lifeMax;
                FalseNPCSegment.SetDefaults(NPCType<ThanatosHead>(), target.GetMatchingSpawnParams());
                lifeMax += FalseNPCSegment.lifeMax;

                // Find the others
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC ecco = Main.npc[i];
                    if (ecco.active && ecco.type == NPCType<AresBody>() && target.type != NPCType<AresBody>())
                        life += ecco.life;

                    if (ecco.active && ecco.type == NPCType<Artemis>() && target.type != NPCType<Artemis>())
                        life += ecco.life;

                    if (ecco.active && ecco.type == NPCType<ThanatosHead>() && target.type != NPCType<ThanatosHead>())
                        life += ecco.life;
                }
            }
            lifePercent = Utils.Clamp(life / (float)lifeMax, 0f, 1f);
            return true;
        }

        public void ValidateAllMechs(ref BigProgressBarInfo info)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC target = Main.npc[i];
                // Find out first whether or not each of the mechs are in hiding
                if (target.type == NPCType<AresBody>())
                    HideAres = target.Opacity < 0.5f;
                if (target.type == NPCType<Artemis>())
                    HideArtemis = target.Opacity < 0.5f;
                if (target.type == NPCType<ThanatosHead>())
                    HideThanatos = target.Opacity < 0.5f;
			}

            // Manually re-hide bosses once they commit die
            if (!NPC.AnyNPCs(NPCType<AresBody>()))
                HideAres = true;
            if (!NPC.AnyNPCs(NPCType<Artemis>()))
                HideArtemis = true;
            if (!NPC.AnyNPCs(NPCType<ThanatosHead>()))
                HideThanatos = true;
        }

        public bool FindMechsAgain(ref BigProgressBarInfo info)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC target = Main.npc[i];
                if (target.active)
                {
                    // Get the index of the mech that's not hiding
                    if (target.type == NPCType<AresBody>() && !HideAres)
                    {
                        info.npcIndexToAimAt = i;
                        return true;
                    }
                    else if (target.type == NPCType<Artemis>() && !HideArtemis)
                    {
                        info.npcIndexToAimAt = i;
                        return true;
                    }
                    else if (target.type == NPCType<ThanatosHead>() && !HideThanatos)
                    {
                        info.npcIndexToAimAt = i;
                        return true;
                    }
                    // Failsafe
                    else if (target.type == NPCType<AresBody>() || target.type == NPCType<Artemis>() || target.type == NPCType<ThanatosHead>())
                    {
                        info.npcIndexToAimAt = i;
                        return true;
                    }
                }
			}
            return false;
        }
    }
}