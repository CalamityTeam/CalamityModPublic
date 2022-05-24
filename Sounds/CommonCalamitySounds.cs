using System;
using Terraria.Audio;

namespace CalamityMod.Sounds
{
	public static class CommonCalamitySounds
	{
        //Sigma grindset rule #43 
        //"Don't bother actually fully updating sounds in a PR all about entirely overhauling the sound engine"
        //          -Mirsario, probably.
        /// <summary>
        /// Gets a zombie sound variant from its variant number
        /// </summary>
        public static SoundStyle GetZombieSound(int id) => new("Terraria/Sounds/Zombie_" + id.ToString());

        public static readonly SoundStyle OtherwordlyHitSound = new("CalamityMod/Sounds/NPCHit/OtherworldlyHit");

        public static readonly SoundStyle AstralNPCHitSound = new("CalamityMod/Sounds/NPCHit/AstralEnemyHit", 3);

        public static readonly SoundStyle AstralNPCDeathSound = new("CalamityMod/Sounds/NPCKilled/AstralEnemyDeath") { Volume = 0.7f };

        public static readonly SoundStyle PlagueBoomSound = new("CalamityMod/Sounds/Custom/PlagueSounds/PlagueBoom", 4);

        public static readonly SoundStyle WyrmScreamSound = new("CalamityMod/Sounds/Custom/WyrmScream");

        public static readonly SoundStyle LightningSound = new("CalamityMod/Sounds/Custom/LightningStrike") { Volume = 1.5f }; //This is just the regular SoundID.Thunder sound, except its only the first variant

        public static readonly SoundStyle LaserCannonSound = new("CalamityMod/Sounds/Item/LaserCannon") { Volume = 0.85f };

        public static readonly SoundStyle ELRFireSound = new("CalamityMod/Sounds/Item/ELRFire"); //What does ELR stand for

        public static readonly SoundStyle LargeWeaponFireSound = new("CalamityMod/Sounds/Item/LargeWeaponFire");

        public static readonly SoundStyle GaussWeaponFire = new("CalamityMod/Sounds/Item/GaussWeaponFire");

        public static readonly SoundStyle FlareSound = new("CalamityMod/Sounds/Item/FlareSound");

        public static readonly SoundStyle PlasmaBoltSound = new("CalamityMod/Sounds/Item/PlasmaBolt") { Volume = 0.8f };

        public static readonly SoundStyle PlasmaBlastSound = new("CalamityMod/Sounds/Item/PlasmaBlast");

        public static readonly SoundStyle MeatySlashSound = new("CalamityMod/Sounds/Custom/MeatySlash");

        public static readonly SoundStyle SwiftSliceSound = new("CalamityMod/Sounds/Custom/SwiftSlice");

        public static readonly SoundStyle ScissorGuillotineSnapSound = new("CalamityMod/Sounds/Custom/ScissorGuillotineSnap");
    }
}
