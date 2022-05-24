using System;
using Terraria.Audio;

namespace CalamityMod.Sounds
{
	public static class CommonNPCSounds
	{
        //Sigma grindset rule #43 
        //"Don't bother actually fully updating sounds in a PR all about entirely overhauling the sound engine"
        //          -Mirsario, probably.
        /// <summary>
        /// Gets a zombie sound variant from its variant number
        /// </summary>
        public static SoundStyle GetZombieSound(int id) => new ("Terraria/Sounds/Zombie_" + id.ToString())

        public static readonly SoundStyle OtherwordlyHitSound = new("Sounds/NPCHit/OtherworldlyHit");
	}
}
