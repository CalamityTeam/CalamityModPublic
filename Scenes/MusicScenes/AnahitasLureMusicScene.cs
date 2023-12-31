﻿using CalamityMod.NPCs.Leviathan;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class AnahitasLureMusicScene : BaseMusicSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossLow;

        public override int NPCType => ModContent.NPCType<LeviathanStart>();
        public override int? MusicModMusic => CalamityMod.Instance.GetMusicFromMusicMod("AnahitasLure");
        public override int VanillaMusic => -1;
        public override int OtherworldMusic => -1;
        public override int MusicDistance => 1600;
    }
}
