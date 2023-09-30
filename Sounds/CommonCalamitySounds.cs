using Terraria.Audio;

namespace CalamityMod.Sounds
{
    public static class CommonCalamitySounds
	{
        public static readonly SoundStyle ExoHitSound = new("CalamityMod/Sounds/NPCHit/ExoHit", 4) { Volume = 0.4f };

        public static readonly SoundStyle ExoDeathSound = new("CalamityMod/Sounds/NPCKilled/ExoDeath") { MaxInstances = 1 };

        public static readonly SoundStyle OtherwordlyHitSound = new("CalamityMod/Sounds/NPCHit/OtherworldlyHit");

        public static readonly SoundStyle AstralNPCHitSound = new("CalamityMod/Sounds/NPCHit/AstralEnemyHit", 3);

        public static readonly SoundStyle AstralNPCDeathSound = new("CalamityMod/Sounds/NPCKilled/AstralEnemyDeath") { Volume = 0.7f };
        public static readonly SoundStyle WulfrumNPCDeathSound = new("CalamityMod/Sounds/NPCKilled/WulfrumDeath");

        public static readonly SoundStyle PlagueBoomSound = new("CalamityMod/Sounds/Custom/PlagueSounds/PlagueBoom", 4);

        public static readonly SoundStyle WyrmScreamSound = new("CalamityMod/Sounds/Custom/WyrmScream");

        public static readonly SoundStyle LightningSound = new("CalamityMod/Sounds/Custom/LightningStrike") { Volume = 1.5f }; //This is just the regular SoundID.Thunder sound, except its only the first variant

        public static readonly SoundStyle LaserCannonSound = new("CalamityMod/Sounds/Item/LaserCannon") { Volume = 0.85f };

        public static readonly SoundStyle ELRFireSound = new("CalamityMod/Sounds/Item/ELRFire"); //What does ELR stand for

        public static readonly SoundStyle ExoLaserShootSound = new("CalamityMod/Sounds/Custom/ExoMechs/ExoLaserShoot");

        public static readonly SoundStyle ExoPlasmaExplosionSound = new("CalamityMod/Sounds/Custom/ExoMechs/ExoPlasmaExplosion", 2);

        public static readonly SoundStyle ExoPlasmaShootSound = new("CalamityMod/Sounds/Custom/ExoMechs/ExoPlasmaShoot");

        public static readonly SoundStyle LargeWeaponFireSound = new("CalamityMod/Sounds/Item/LargeWeaponFire");

        public static readonly SoundStyle GaussWeaponFire = new("CalamityMod/Sounds/Item/GaussWeaponFire");

        public static readonly SoundStyle FlareSound = new("CalamityMod/Sounds/Item/FlareSound");

        public static readonly SoundStyle PlasmaBoltSound = new("CalamityMod/Sounds/Item/PlasmaBolt") { Volume = 0.8f };

        public static readonly SoundStyle PlasmaBlastSound = new("CalamityMod/Sounds/Item/PlasmaBlast");

        public static readonly SoundStyle MeatySlashSound = new("CalamityMod/Sounds/Custom/MeatySlash");

        public static readonly SoundStyle SwiftSliceSound = new("CalamityMod/Sounds/Custom/SwiftSlice");

        public static readonly SoundStyle ScissorGuillotineSnapSound = new("CalamityMod/Sounds/Custom/ScissorGuillotineSnap");

        public static readonly SoundStyle LouderPhantomPhoenix = new("CalamityMod/Sounds/Item/LouderPhantomPhoenix", 3);

        public static readonly SoundStyle LouderSwingWoosh = new("CalamityMod/Sounds/Custom/LoudSwingWoosh");

    }
}
