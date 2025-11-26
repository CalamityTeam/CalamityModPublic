using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using CalamityMod.NPCs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityMod.Systems
{
    public record class MusicEventEntry(string Id, int Song, TimeSpan Length, TimeSpan IntroSilence, TimeSpan OutroSilence, Func<bool> ShouldPlay, Func<bool> Enabled);

    public class MusicEventSystem : ModSystem
    {
        #region Statics

        public static MusicEventEntry CurrentEvent { get; set; } = null;

        public static DateTime? TrackStart { get; set; } = null;

        // This allows for some additional silence at the end of a track
        public static DateTime? TrackEnd { get; set; } = null;

        public static int LastPlayedEvent { get; set; } = -1;

        public static TimeSpan? OutroSilence { get; set; } = null;

        // This allows skipping track fade in
        public static bool NoFade { get; set; } = false;

        public static Thread EventTrackerThread { get; set; } = null;

        public static List<string> PlayedEvents { get; set; } = [];

        public static List<MusicEventEntry> EventCollection { get; set; } = [];

        // This is to make sure that old worlds don't immediately queue a bunch of tracks on player entry
        private static bool oldWorld { get; set; } = true;

        #endregion

        #region Events List

        public override void OnModLoad()
        {
            static void AddEntry(string eventId, string songName, TimeSpan length, Func<bool> shouldPlay, Func<bool> enabled, TimeSpan? introSilence = null, TimeSpan? outroSilence = null)
            {
                MusicEventEntry entry = new(eventId, CalamityMod.Instance.GetMusicFromMusicMod(songName).Value, length, introSilence ?? TimeSpan.Zero, outroSilence ?? TimeSpan.Zero, shouldPlay, enabled);
                EventCollection.Add(entry);
            }

            AddEntry("CloneDefeated", "Interlude1", TimeSpan.FromSeconds(214.577d),
                () => DownedBossSystem.downedCalamitasClone, () => CalamityClientConfig.Instance.Interludes);

            AddEntry("MLDefeated", "Interlude2", TimeSpan.FromSeconds(191.912d), () => NPC.downedMoonlord,
                () => CalamityClientConfig.Instance.Interludes, outroSilence: TimeSpan.FromSeconds(1f));

            // Alternative Interlude 2 -> AddEntry("MLDefeated", "Interlude2_CutIntro", TimeSpan.FromSeconds(160.989d),
            //    () => NPC.downedMoonlord, () => CalamityConfig.Instance.Interlude2,
            //    outroSilence: TimeSpan.FromSeconds(1f));

            AddEntry("YharonDefeated", "Interlude3", TimeSpan.FromSeconds(295.932d),
                () => DownedBossSystem.downedYharon, () => CalamityClientConfig.Instance.Interludes);

            AddEntry("DoGDefeated", "DevourerofGodsEulogy", TimeSpan.FromSeconds(203.620d),
                () => DownedBossSystem.downedDoG, () => CalamityClientConfig.Instance.DevourerofGodsEulogy,
                introSilence: TimeSpan.FromSeconds(7.5f));

            // Acceptance is NOT toggleable in the config
            AddEntry("ScalDefeated", "CalamitasDefeat_LongFade", TimeSpan.FromSeconds(58.689d),
                () => CalamityGlobalNPC.SCalAcceptance != -1, () => true);
        }

        public override void Unload() => EventCollection.Clear();

        #endregion

        #region Event Handling

        public override void PostUpdateTime()
        {
            // If the player has already completed conditions to trigger certain music events, we don't
            // want to queue a bunch of tracks to play as soon as they enter the world, so instead just mark them as played
            if (oldWorld)
            {
                foreach (MusicEventEntry entry in EventCollection)
                {
                    if (entry.ShouldPlay())
                        PlayedEvents.Add(entry.Id);
                }

                oldWorld = false;
            }

            //PlayedEvents.Remove("MLDefeated");

            // If the event has just finished, we want a little silence before fading back to normal
            if (TrackEnd is not null)
            {
                // `silence` is the time after a track ends before music goes back to normal
                TimeSpan silence = OutroSilence.Value;
                TimeSpan postTrack = DateTime.Now - TrackEnd.Value;

                // Play silence for the time specified
                if (postTrack < silence)
                {
                    int silenceSlot = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Silence");
                    Main.musicBox2 = silenceSlot;
                }

                else
                {
                    LastPlayedEvent = -1;
                    TrackEnd = null;
                    OutroSilence = null;
                }

                return;
            }

            // Only check for new events to play if none is currently playing
            // This makes sure events always finish before a new one starts
            if (CurrentEvent is null)
            {
                foreach (MusicEventEntry musicEvent in EventCollection)
                {
                    // Make sure the event hasn't already played and SHOULD play
                    if (!PlayedEvents.Contains(musicEvent.Id) && musicEvent.ShouldPlay())
                    {
                        // Even if an event isn't marked as enabled, it should be counted
                        // as "played" so it isn't played when the player doesn't expect it
                        PlayedEvents.Add(musicEvent.Id);
                        
                        // Events are always enabled on the server
                        if (Main.dedServ || musicEvent.Enabled())
                        {
                            // Assign the current event and start time
                            CurrentEvent = musicEvent;
                            TrackStart = DateTime.Now + musicEvent.IntroSilence;

                            // On clients, use a background thread to make sure the track always plays for exactly
                            // the specified length, regardless of if the game gets minimized, lags, or time becomes
                            // detangled from a consistent 60fps in any other way
                            if (!Main.dedServ)
                            {
                                EventTrackerThread = new(WatchMusicEvent);
                                EventTrackerThread.Start();
                            }

                            break;
                        }
                    }
                }
            }

            if (TrackStart is not null)
            {
                if (TrackStart > DateTime.Now)
                {
                    int silenceSlot = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Silence");
                    Main.musicBox2 = silenceSlot;
                    NoFade = true;
                }

                else
                {
                    Main.musicBox2 = CurrentEvent.Song;

                    if (NoFade)
                    {
                        Main.musicFade[CurrentEvent.Song] = 1f;
                        NoFade = false;
                    }

                    // If the event has finished playing, mark the end as now and clear the current event
                    if (DateTime.Now - TrackStart >= CurrentEvent.Length)
                    {
                        int silenceSlot = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Silence");
                        Main.musicBox2 = silenceSlot;
                        Main.musicFade[CurrentEvent.Song] = 0f;

                        TrackEnd = DateTime.Now;
                        LastPlayedEvent = CurrentEvent.Song;
                        OutroSilence = CurrentEvent.OutroSilence;

                        TrackStart = null;
                        CurrentEvent = null;
                    }
                }
            }
        }

        /// <summary>
        /// Watches for the game minimizing at any point, and adjusts the amount of time to play the song for accordingly
        /// </summary>
        public static void WatchMusicEvent()
        {
            DateTime? minimized = null;

            while (CurrentEvent is not null)
            {
                bool musicPaused = !Main.instance.IsActive;
                
                if (musicPaused && !minimized.HasValue)
                    minimized = DateTime.Now;

                else if (!musicPaused && minimized.HasValue)
                {
                    TrackStart += DateTime.Now - minimized.Value;
                    minimized = null;
                }
            }

            EventTrackerThread = null;
        }

        #endregion

        #region Event Saving

        public override void SaveWorldData(TagCompound tag)
        {
            tag["calamityPlayedMusicEventCount"] = PlayedEvents.Count;
            for (int i = 0; i < PlayedEvents.Count; i++)
                tag[$"calamityPlayedMusicEvent{i}"] = PlayedEvents[i];
        }

        public override void LoadWorldData(TagCompound tag)
        {
            PlayedEvents.Clear();

            if (tag.TryGet("calamityPlayedMusicEventCount", out int playedMusicEventCount))
            {
                for (int i = 0; i < playedMusicEventCount; i++)
                {
                    if (tag.TryGet($"calamityPlayedMusicEvent{i}", out string playedEvent))
                        PlayedEvents.Add(playedEvent);
                }
            }

            oldWorld = false;
        }

        public override void OnWorldUnload()
        {
            oldWorld = true;
            TrackStart = null;
            TrackEnd = null;
            CurrentEvent = null;
            PlayedEvents.Clear();
            NoFade = false;
            LastPlayedEvent = -1;
        }

        #endregion

        #region Event Syncing

        public static void SendSyncRequest()
        {
            ModPacket packet = CalamityMod.Instance.GetPacket();
            packet.Write((byte)CalamityModMessageType.MusicEventSyncRequest);
            packet.Send();
        }

        public static void FulfillSyncRequest(int requester)
        {
            // Only fulfill requests as the server host
            if (!Main.dedServ)
                return;

            ModPacket packet = CalamityMod.Instance.GetPacket();
            packet.Write((byte)CalamityModMessageType.MusicEventSyncResponse);

            int trackCount = PlayedEvents.Count;
            packet.Write(trackCount);

            for (int i = 0; i < trackCount; i++)
                packet.Write(PlayedEvents[i]);

            packet.Send(toClient: requester);
        }

        public static void ReceiveSyncResponse(BinaryReader reader)
        {
            // Only receive info on clients
            if (Main.dedServ)
                return;

            PlayedEvents.Clear();
            int trackCount = reader.ReadInt32();

            for (int i = 0; i < trackCount; i++)
                PlayedEvents.Add(reader.ReadString());
        }

        #endregion
    }

    public class CalamityModMusicEventPlayer : ModPlayer
    {
        public override void OnEnterWorld()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient && Player.whoAmI != Main.myPlayer)
                MusicEventSystem.SendSyncRequest();
        }
    }
}
