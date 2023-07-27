using System;
using CalamityMod.CalPlayer;
using CalamityMod.NPCs.PrimordialWyrm;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.CalClone;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.Cryogen;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.Signus;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.Yharon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.UI
{
    // TODO -- Combine with SpeedrunTimerStoppingSystem, which should be renamed and call the UI draw code in this file.
    public class SpeedrunTimerUI
    {
        // These values put the Speedrun Timer roughly at the top center of a 1080p screen.
        internal const float DefaultTimerPosX = 46f;
        internal const float DefaultTimerPosY = 1.481f;

        private static readonly float SplitHorizontalOffset = +30f;
        private static readonly float SplitVerticalOffset = +44f;

        public static void Draw(Player player)
        {
            if (Main.gameMenu || !CalamityConfig.Instance.SpeedrunTimer)
                return;

            // Sanity check the planned position before drawing
            Vector2 screenRatioPosition = new Vector2(CalamityConfig.Instance.SpeedrunTimerPosX, CalamityConfig.Instance.SpeedrunTimerPosY);
            if (screenRatioPosition.X < 0f || screenRatioPosition.X > 100f)
                screenRatioPosition.X = DefaultTimerPosX;
            if (screenRatioPosition.Y < 0f || screenRatioPosition.Y > 100f)
                screenRatioPosition.Y = DefaultTimerPosY;

            // Convert the screen ratio position to an absolute position in pixels
            // Cast to integer to prevent blurriness which results from decimal pixel positions
            Vector2 screenPos = screenRatioPosition;
            screenPos.X = (int)(screenPos.X * 0.01f * Main.screenWidth);
            screenPos.Y = (int)(screenPos.Y * 0.01f * Main.screenHeight);

            CalamityPlayer calamityPlayer = player.Calamity();

            // Main timer
            string formatStr = @"hh\:mm\:ss\.ff";
            string formatStrDays = @"d\:hh\:mm\:ss\.ff";
            TimeSpan totalTime = CalamityMod.SpeedrunTimer.Elapsed.Add(calamityPlayer.previousSessionTotal);
            string text = totalTime.ToString(totalTime.Days > 0 ? formatStrDays : formatStr);
            float scale = 2f;
            Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, text, screenPos.X, screenPos.Y, Color.White, Color.Black, default, scale);

            if (calamityPlayer.lastSplitType == -1)
                return;

            // Latest split
            TimeSpan split = calamityPlayer.lastSplit;
            text = split.ToString(split.Days > 0 ? formatStrDays : formatStr);
            scale = 1f;
            float lineTwoX = screenPos.X + SplitHorizontalOffset;
            float lineTwoY = screenPos.Y + SplitVerticalOffset;
            Texture2D texture = GetSplitIcon(calamityPlayer.lastSplitType);

            // If a split icon exists, draw it.
            if (texture != null)
                Main.spriteBatch.Draw(texture, new Vector2(lineTwoX - texture.Width - 4f, lineTwoY), null, Color.White, 0f, default, 1f, SpriteEffects.None, 0f);

            // Draw the latest split time.
            Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, text, lineTwoX, lineTwoY, Color.White, Color.Black, default, scale);
        }

        // TODO -- these should not be magic numbers that are not used ANYWHERE else
        private static Texture2D GetSplitIcon(int magicNumber) => magicNumber switch
        {
            1 => TextureAssets.NpcHeadBoss[7].Value, // King Slime
            2 => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<DesertScourgeHead>()]].Value,
            3 => TextureAssets.NpcHeadBoss[1].Value, // Eye of Cthulhu
            4 => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<Crabulon>()]].Value,
            5 => TextureAssets.NpcHeadBoss[2].Value, // Eater of Worlds
            6 => TextureAssets.NpcHeadBoss[23].Value, // Brain of Cthulhu
            7 => ModContent.Request<Texture2D>("CalamityMod/NPCs/HiveMind/HiveMindP2_Head_Boss").Value,
            8 => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<PerforatorHive>()]].Value,
            9 => TextureAssets.NpcHeadBoss[14].Value, // Queen Bee
            10 => TextureAssets.NpcHeadBoss[19].Value, // Skeletron
            11 => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<SlimeGodCore>()]].Value,
            12 => TextureAssets.NpcHeadBoss[22].Value, // Wall of Flesh
            13 => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<Cryogen>()]].Value,
            14 => TextureAssets.NpcHeadBoss[21].Value, // The Twins
            15 => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<AquaticScourgeHead>()]].Value,
            16 => TextureAssets.NpcHeadBoss[25].Value, // The Destroyer
            17 => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<BrimstoneElemental>()]].Value,
            18 => TextureAssets.NpcHeadBoss[18].Value, // Skeletron Prime
            19 => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<CalamitasClone>()]].Value,
            20 => TextureAssets.NpcHeadBoss[12].Value, // Plantera
            21 => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<Leviathan>()]].Value,
            22 => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<AstrumAureus>()]].Value,
            23 => TextureAssets.NpcHeadBoss[5].Value, // Golem
            24 => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<PlaguebringerGoliath>()]].Value,
            25 => TextureAssets.NpcHeadBoss[4].Value, // Duke Fishron
            26 => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<RavagerBody>()]].Value,
            27 => TextureAssets.NpcHeadBoss[31].Value, // Lunatic Cultist
            28 => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<AstrumDeusHead>()]].Value,
            29 => TextureAssets.NpcHeadBoss[8].Value, // Moon Lord
            30 => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<ProfanedGuardianCommander>()]].Value,
            31 => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<Bumblefuck>()]].Value,
            32 => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<Providence>()]].Value,
            33 => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<CeaselessVoid>()]].Value,
            34 => ModContent.Request<Texture2D>("CalamityMod/NPCs/StormWeaver/StormWeaverHeadNaked_Head_Boss").Value,
            35 => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<Signus>()]].Value,
            36 => ModContent.Request<Texture2D>("CalamityMod/NPCs/Polterghast/Necroplasm_Head_Boss").Value,
            37 => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<OldDuke>()]].Value,
            38 => ModContent.Request<Texture2D>("CalamityMod/NPCs/DevourerofGods/DevourerofGodsHead_Head_Boss").Value,
            39 => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<Yharon>()]].Value,
            40 => ModContent.Request<Texture2D>("CalamityMod/NPCs/SupremeCalamitas/HoodlessHeadIcon").Value,
            41 => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<AresBody>()]].Value,
            42 => TextureAssets.NpcHeadBoss[NPCID.Sets.BossHeadTextures[ModContent.NPCType<PrimordialWyrmHead>()]].Value,
            43 => TextureAssets.NpcHeadBoss[38].Value, // Queen Slime
            44 => TextureAssets.NpcHeadBoss[37].Value, // Empress of Light
            45 => TextureAssets.NpcHeadBoss[39].Value, // Deerclops
            _ => null
        };
    }
}
