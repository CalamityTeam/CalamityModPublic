using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public abstract class BaseMusicSceneEffect : ModSceneEffect
    {
		// Scene Effect Priorities
		// Setting it to SceneEffectPriority.BossLow on bosses makes Boss 1 play for half a second so don't do that
		// ??? (Anahita Lure): SceneEffectPriority.BossLow
		// Most Bosses: SceneEffectPriority.BossMedium
		// Major PML Bosses: SceneEffectPriority.BossHigh

        #region Overridable Properties
        public abstract int NPCType { get; }
        public abstract int? MusicModMusic { get; }
        public abstract int VanillaMusic { get; }
        public abstract int OtherworldMusic { get; }
        public virtual int MusicDistance => 5000;
        #endregion

		#region Overridable Methods
        public virtual bool AdditionalCheck() => true;
		#endregion

        public virtual int SetMusic()
		{
			if (MusicModMusic is not null)
				return (int)MusicModMusic;
			// This function is private.  Todo: bug tmod devs
			/*if (Main.swapMusic)
				return OtherworldMusic;*/
			return VanillaMusic;
		}

        public virtual bool SetSceneEffect(Player player)
		{
			if (!AdditionalCheck())
				return false;

			if (MusicModMusic is null && VanillaMusic == -1)
				return false;
			/*if (MusicModMusic is null && Main.swapMusic && OtherworldMusic == -1)
				return false;*/

			for (int j = 0; j < Main.maxNPCs; j++)
			{
				NPC npc = Main.npc[j];
				if (!npc.active)
					continue;

				if (npc.type != NPCType)
					continue;

				Rectangle screenRect = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);
				Rectangle npcBox = new Rectangle((int)npc.Center.X - MusicDistance, (int)npc.Center.Y - MusicDistance, MusicDistance * 2, MusicDistance * 2);
				if (screenRect.Intersects(npcBox))
					return true;
			}
			return false;
		}

        public override int Music => SetMusic();

        public override bool IsSceneEffectActive(Player player) => SetSceneEffect(player);
    }
}
