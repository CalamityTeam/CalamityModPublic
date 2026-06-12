using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    // TODO -- The projectile tweaks database and functions should be stored in a ModSystem.
    // ApplyTweaks(ref Projectile proj) would be the one exposed function, which CalamityGlobalProjectile would call in SetDefaults.
    public partial class CalamityGlobalProjectile : GlobalProjectile
    {
        #region Database and Initialization
        internal static SortedDictionary<int, IProjectileTweak[]> currentTweaks = null;

        internal static void LoadTweaks()
        {
            // Various shorthands for projectiles which receive very simple changes, such as setting one flag.
            IProjectileTweak[] defenseDamage = Do(DefenseDamage);
            IProjectileTweak[] trueMelee = Do(TrueMelee, DefaultIDStaticIFrames); // All the tweaked true melee projectiles need to be changed from global to static as well
            IProjectileTweak[] trueMeleeNoSpeed = Do(TrueMeleeNoSpeed, DefaultIDStaticIFrames);
            IProjectileTweak[] defaultIFrames = Do(DefaultIDStaticIFrames);
            IProjectileTweak[] standardBulletTweaks = Do(ExtraUpdatesDelta(+2));
            IProjectileTweak[] standardChainsawTweaks = Do(TrueMeleeNoSpeed, ArmorPenetrationDelta(+15), LocalIFrames(5));
            IProjectileTweak[] standardDrillTweaks = Do(TrueMeleeNoSpeed, ArmorPenetrationDelta(+25), LocalIFrames(5));
            IProjectileTweak[] counterweightTweaks = Do(MaxUpdatesExact(2), DefaultIDStaticIFrames);

            // Shorthand for changing all the stats of a yoyo at once. This handles extra update related math for you.
            // For topSpeed, put in how fast you want the yoyo to be EXACTLY: it will be divided out in extra updates for you.
            static IProjectileTweak[] RebalanceYoyo(float lifetime, float range, float topSpeed, int extraUpdates, int iframes = 10) => new IProjectileTweak[]
            {
                ExtraUpdatesExact(extraUpdates),
                LocalIFrames(iframes * (extraUpdates + 1)),
                YoyoLifetime(lifetime <= 0f ? -1f : lifetime * (extraUpdates + 1)),
                YoyoRange(range),
                YoyoTopSpeed(topSpeed / (extraUpdates + 1)),
            };

            // SORTING NOTES:
            // 1. Sort tweaks by categories first, then sort by the internal name in alphabetical order. Navigate through categories and names using the search function.
            // 2. Higher categories hold priority over lower ones (ie. Balancing with PB tweaks belong in balancing, rather than PB)
            // 3. Ambiguous internal names should have comments for ease of access.
            currentTweaks = new SortedDictionary<int, IProjectileTweak[]>
            {
                #region CATEGORY 1: Vanilla Yoyo Balancing
                // note this is only yoyos, not counterweights

                // original: 15s lifetime | 270px range | 14px/f top speed | 0 extra updates
                { ProjectileID.Amarok, RebalanceYoyo(-1f, 432f, 28f, 1, 12) },

                // original: 13s lifetime | 235px range | 14px/f top speed | 0 extra updates
                { ProjectileID.Cascade, RebalanceYoyo(30f, 384f, 28f, 1, 15) },

                // original: 16s lifetime | 275px range | 17px/f top speed | 0 extra updates
                { ProjectileID.Chik, RebalanceYoyo(-1f, 400f, 32f, 1, 12) },

                // original: 9s lifetime | 220px range | 13px/f top speed | 0 extra updates
                { ProjectileID.Code1, RebalanceYoyo(21f, 320f, 25f, 1, 15) },

                // original: INF lifetime | 280px range | 17px/f top speed | 0 extra updates
                { ProjectileID.Code2, RebalanceYoyo(-1f, 432f, 42f, 1, 12) },

                // original: 7s lifetime | 195px range | 12.5px/f top speed | 0 extra updates
                { ProjectileID.CorruptYoyo, RebalanceYoyo(18f, 288f, 22f, 0, 20) }, // Malaise

                // original: 6s lifetime | 207px range | 12px/f top speed | 0 extra updates
                { ProjectileID.CrimsonYoyo, RebalanceYoyo(18f, 288f, 22f, 0, 20) }, // Artery

                // original: 8s lifetime | 235px range | 15px/f top speed | 0 extra updates
                { ProjectileID.FormatC, RebalanceYoyo(-1f, 384f, 36f, 1, 12) },

                // original: 10s lifetime | 250px range | 12px/f top speed | 0 extra updates
                { ProjectileID.Gradient, RebalanceYoyo(-1f, 384f, 36f, 1, 12) },

                // original: 12s lifetime | 275px range | 15px/f top speed | 0 extra updates
                { ProjectileID.HelFire, RebalanceYoyo(-1f, 352f, 42f, 2, 12) },

                // original: 11s lifetime | 225px range | 14px/f top speed | 0 extra updates
                { ProjectileID.HiveFive, RebalanceYoyo(24f, 320f, 20f, 0, 15) },

                // original: 8s lifetime | 215px range | 13px/f top speed | 0 extra updates
                { ProjectileID.JungleYoyo, RebalanceYoyo(24f, 320f, 20f, 0, 20) }, // Amazon

                // original: INF lifetime | 340px range | 16px/f top speed | 0 extra updates
                { ProjectileID.Kraken, RebalanceYoyo(-1f, 480f, 54f, 2) },

                // original: 5s lifetime | 170px range | 11px/f top speed | 0 extra updates
                { ProjectileID.Rally, RebalanceYoyo(16f, 272f, 20f, 0, 20) },

                // original: INF lifetime | 370px range | 16px/f top speed | 0 extra updates
                { ProjectileID.RedsYoyo, RebalanceYoyo(-1f, 480f, 42f, 2, 12) }, // Red's Throw

                // original: INF lifetime | 400px range | 17.5px/f top speed | 0 extra updates
                { ProjectileID.Terrarian, RebalanceYoyo(-1f, 512f, 54f, 2) },
                // 12AUG2023: Ozzatron: Terrarian has been IL edited to not emit more orb spawns with extra updates. This iframe change is safe.
                { ProjectileID.TerrarianBeam, Do(LocalIFrames(-1)) }, // Terrarian yoyo orbs

                // original: INF lifetime | 360px range | 16.5px/f top speed | 0 extra updates
                { ProjectileID.TheEyeOfCthulhu, RebalanceYoyo(-1f, 480f, 36f, 1, 12) }, // the yoyo, of course

                // original: INF lifetime | 370px range | 16px/f top speed | 0 extra updates
                { ProjectileID.ValkyrieYoyo, RebalanceYoyo(-1f, 480f, 42f, 2, 12) },

                // original: 11s lifetime | 225px range | 14px/f top speed | 0 extra updates
                { ProjectileID.Valor, RebalanceYoyo(30f, 400f, 36f, 1, 15) },

                // original: 3s lifetime | 130px range | 9px/f top speed | 0 extra updates
                { ProjectileID.WoodYoyo, RebalanceYoyo(15f, 240f, 14f, 0, 20) },

                // original: 14s lifetime | 290px range | 16px/f top speed | 0 extra updates
                { ProjectileID.Yelets, RebalanceYoyo(-1f, 400f, 36f, 1, 12) },
                #endregion

                #region CATEGORY 2: Weapon/Enemy Balancing
                { ProjectileID.AdamantiteChainsaw, standardChainsawTweaks },
                { ProjectileID.AdamantiteDrill, standardDrillTweaks },
                { ProjectileID.AdamantiteGlaive, Do(TrueMelee, LocalIFrames(7)) },
                { ProjectileID.Anchor, Do(ExtraUpdatesExact(1), DefaultIDStaticIFrames) },
                { ProjectileID.Arkhalis, Do(TrueMeleeNoSpeed, ScaleExact(1.25f), IDStaticIFrames(5)) }, // Has an exception in Vanilla iframe code, uses 5 iframes
                { ProjectileID.Bee, Do(PiercingExact(2), DefaultIDStaticIFrames) },
                { ProjectileID.BeeArrow, Do(ExtraUpdatesExact(1), DefaultIDStaticIFrames) },
                { ProjectileID.BlackCounterweight, counterweightTweaks },
                { ProjectileID.BlueCounterweight, counterweightTweaks },
                { ProjectileID.BlueMoon, Do(ExtraUpdatesExact(1)) },
                { ProjectileID.Bullet, Do(ExtraUpdatesDelta(+2), DefaultIDStaticIFrames) },
                { ProjectileID.BulletHighVelocity, Do(ExtraUpdatesDelta(+2), LocalIFrames(-1)) },
                { ProjectileID.ButchersChainsaw, Do(TrueMeleeNoSpeed, ArmorPenetrationDelta(+15), LocalIFrames(10), ScaleExact(1.5f)) },
                { ProjectileID.ChlorophyteChainsaw, standardChainsawTweaks },
                { ProjectileID.ChlorophyteDrill, standardDrillTweaks },
                { ProjectileID.ChlorophyteJackhammer, standardDrillTweaks },
                { ProjectileID.ChlorophyteOrb, Do(NoPiercing) },
                { ProjectileID.CobaltChainsaw, standardChainsawTweaks },
                { ProjectileID.CobaltDrill, standardDrillTweaks },
                { ProjectileID.CobaltNaginata, Do(TrueMelee, LocalIFrames(9)) },
                { ProjectileID.CrystalBullet, Do(ExtraUpdatesDelta(+2), DefaultIDStaticIFrames) },
                { ProjectileID.CursedBullet, Do(ExtraUpdatesDelta(+2), DefaultIDStaticIFrames) },
                { ProjectileID.ClusterRocketI, Do(LocalIFrames(15)) },
                { ProjectileID.ClusterFragmentsI, Do(IDStaticIFrames(15)) },
                { ProjectileID.ClusterRocketII, Do(LocalIFrames(15)) },
                { ProjectileID.ClusterFragmentsII, Do(IDStaticIFrames(15)) },
                { ProjectileID.ClusterSnowmanRocketI, Do(LocalIFrames(15)) },
                { ProjectileID.ClusterSnowmanRocketII, Do(LocalIFrames(15)) },
                { ProjectileID.DangerousSpider, Do( ExtraUpdatesExact(2), LocalIFrames(90)) }, //Spider Staff spiders. It has Venom, Dangerous, and Jumping spiders.
                { ProjectileID.DD2SquireSonicBoom, Do(PiercingExact(3), DefaultIDStaticIFrames) }, // Flying Dragon
                { ProjectileID.DeadlySphere, Do(LocalIFrames(30)) },
                { ProjectileID.EmeraldBolt, Do(NoPiercing) },
                { ProjectileID.EnchantedBoomerang, Do(ExtraUpdatesExact(1), DefaultIDStaticIFrames) },
                { ProjectileID.ExplosiveBullet, Do(ExtraUpdatesDelta(+2), IDStaticIFrames(5), SingleHitImmunity) }, // Has an exception in Vanilla iframe code, uses 5 iframes
                { ProjectileID.FairyQueenRangedItemShot, Do(PiercingExact(7), ExtraUpdatesExact(1))  }, // Eventide Convert
                { ProjectileID.FlaironBubble, Do(ExtraUpdatesExact(1), TimeLeftExact(150), DefaultIDStaticIFrames) },
                { ProjectileID.Flamarang, Do(ExtraUpdatesExact(1), DefaultIDStaticIFrames) },
                { ProjectileID.FlamingJack, Do(ExtraUpdatesExact(1), DefaultIDStaticIFrames) },
                { ProjectileID.FlowerPetal, Do(MaxUpdatesExact(4), LocalIFrames(10)) }, // Orichalcum armor
                { ProjectileID.FlyingKnife, Do(ExtraUpdatesExact(1), DefaultIDStaticIFrames) },
                { ProjectileID.FrostBoltStaff, Do(ExtraUpdatesExact(1), DefaultIDStaticIFrames) },
                { ProjectileID.FruitcakeChakram, Do(ExtraUpdatesExact(1), DefaultIDStaticIFrames) },
                { ProjectileID.GiantBee, Do(PiercingExact(2), DefaultIDStaticIFrames) },
                { ProjectileID.GladiusStab, Do(TrueMelee, LocalIFrames(-1)) },
                { ProjectileID.GoldenBullet, Do(ExtraUpdatesDelta(+2), DefaultIDStaticIFrames) },
                { ProjectileID.GoldenShowerFriendly, Do(PiercingExact(2), DefaultIDStaticIFrames) },
                { ProjectileID.GreenCounterweight, counterweightTweaks },
                { ProjectileID.Hamdrax, standardDrillTweaks }, // Drax (never internally renamed since 1.1)
                { ProjectileID.HellfireArrow, Do(ExtraUpdatesDelta(+2)) },
                { ProjectileID.IceBoomerang, Do(ExtraUpdatesExact(1), DefaultIDStaticIFrames) },
                { ProjectileID.IceSickle, Do(ExtraUpdatesExact(1)) },
                { ProjectileID.IchorBullet, Do(ExtraUpdatesDelta(+2), DefaultIDStaticIFrames) },
                { ProjectileID.InfluxWaver, Do(ExtraUpdatesExact(1)) },
                { ProjectileID.InfernoFriendlyBolt, Do(ExtraUpdatesExact(1), DefaultIDStaticIFrames) },
                { ProjectileID.InfernoFriendlyBlast, Do(ExtraUpdatesExact(2), DefaultIDStaticIFrames) },
                { ProjectileID.JumperSpider, Do( ExtraUpdatesExact(2), LocalIFrames(90)) }, //Spider Staff spiders. It has Venom, Dangerous, and Jumping spiders.
                { ProjectileID.LaserDrill, Do(ArmorPenetrationDelta(+25), LocalIFrames(5)) },
                { ProjectileID.LightDisc, Do(MaxUpdatesExact(3), DefaultIDStaticIFrames) },
                { ProjectileID.LostSoulHostile, Do(TileCollide) }, // Ragged Caster
                { ProjectileID.MeteorShot, standardBulletTweaks },
                { ProjectileID.Meowmere, Do(PiercingExact(3), LocalIFrames(-1)) },
                { ProjectileID.MonkStaffT1, Do(TrueMeleeNoSpeed, ScaleExact(3f)) }, // Sleepy Octopod
                { ProjectileID.MonkStaffT2, Do(TrueMelee, IDStaticIFrames(18)) }, // Ghastly Glaive
                { ProjectileID.MonkStaffT3, Do(ScaleRatio(2f)) }, // Sky Dragon's Fury
                { ProjectileID.MoonlordBullet, standardBulletTweaks }, // Luminite Bullet
                { ProjectileID.MythrilChainsaw, standardChainsawTweaks },
                { ProjectileID.MythrilDrill, standardDrillTweaks },
                { ProjectileID.MythrilHalberd, Do(TrueMelee, LocalIFrames(8)) },
                { ProjectileID.NanoBullet, Do(ExtraUpdatesDelta(+2), DefaultIDStaticIFrames) },
                { ProjectileID.NebulaDrill, standardDrillTweaks },
                { ProjectileID.NebulaLaser, Do(ExtraUpdatesDelta(-1)) },
                { ProjectileID.OrichalcumChainsaw, standardChainsawTweaks },
                { ProjectileID.OrichalcumDrill, standardDrillTweaks },
                { ProjectileID.PalladiumChainsaw, standardChainsawTweaks },
                { ProjectileID.PalladiumDrill, standardDrillTweaks },
                { ProjectileID.PartyBullet, Do(ExtraUpdatesDelta(+2), DefaultIDStaticIFrames) },
                { ProjectileID.PurpleCounterweight, counterweightTweaks },
                { ProjectileID.PurpleLaser, Do(LocalIFrames(10 * 5)) }, // Laser Rifle, accounting for extra updates
                { ProjectileID.QueenSlimeGelAttack, Do(NoPiercing) },
                { ProjectileID.QueenSlimeMinionPinkBall, Do(NoPiercing) },
                { ProjectileID.RedCounterweight, counterweightTweaks },
                { ProjectileID.RocketFireworkBlue, Do(TimeLeftDelta(+45)) },
                { ProjectileID.RocketFireworkGreen, Do(TimeLeftDelta(+45)) },
                { ProjectileID.RocketFireworkRed, Do(TimeLeftDelta(+45)) },
                { ProjectileID.RocketFireworkYellow, Do(TimeLeftDelta(+45)) },
                { ProjectileID.SawtoothShark, Do(TrueMeleeNoSpeed, ArmorPenetrationDelta(+15), LocalIFrames(6)) },
                { ProjectileID.Shroomerang, Do(ExtraUpdatesExact(1), DefaultIDStaticIFrames) },
                { ProjectileID.SolarFlareDrill, standardDrillTweaks },
                { ProjectileID.StardustDrill, standardDrillTweaks },
                { ProjectileID.StarWrath, Do(NoPiercing) },
                { ProjectileID.Sunfury, Do(ExtraUpdatesExact(1)) },
                { ProjectileID.SwordBeam, Do(ExtraUpdatesExact(2), DefaultIDStaticIFrames) }, // Beam Sword projectile
                { ProjectileID.Terragrim, Do(TrueMeleeNoSpeed, ScaleExact(1.25f), IDStaticIFrames(5)) }, // Has an exception in Vanilla iframe code, uses 5 iframes
                { ProjectileID.ThunderStaffShot, Do(PiercingExact(2), DefaultIDStaticIFrames) }, //Thunder Zapper projectile
                { ProjectileID.TitaniumChainsaw, standardChainsawTweaks },
                { ProjectileID.TitaniumDrill, standardDrillTweaks },
                { ProjectileID.Trimarang, Do(ExtraUpdatesExact(1), DefaultIDStaticIFrames) },
                { ProjectileID.TrueNightsEdge, Do(PiercingExact(4)) },
                { ProjectileID.VenomBullet, Do(ExtraUpdatesDelta(+2), DefaultIDStaticIFrames) },
                { ProjectileID.VenomSpider, Do( ExtraUpdatesExact(2), LocalIFrames(90)) }, //Spider Staff spiders. It has Venom, Dangerous, and Jumping spiders.
                { ProjectileID.VortexDrill, standardDrillTweaks },
                { ProjectileID.Wasp, Do(PiercingExact(2)) },
                { ProjectileID.WeatherPainShot, Do(ExtraUpdatesExact(3)) },
                { ProjectileID.YellowCounterweight, counterweightTweaks },
                #endregion

                #region CATEGORY 3: True Melee support
                { ProjectileID.CopperShortswordStab, trueMelee },
                { ProjectileID.DarkLance, trueMelee },
                { ProjectileID.GoldShortswordStab, trueMelee },
                { ProjectileID.Gungnir, trueMelee },
                { ProjectileID.HallowJoustingLance, trueMelee },
                { ProjectileID.IronShortswordStab, trueMelee },
                { ProjectileID.JoustingLance, trueMelee },
                { ProjectileID.LeadShortswordStab, trueMelee },
                { ProjectileID.MushroomSpear, trueMelee },
                { ProjectileID.NebulaChainsaw, trueMeleeNoSpeed },
                { ProjectileID.ObsidianSwordfish, trueMelee },
                { ProjectileID.OrichalcumHalberd, trueMelee },
                { ProjectileID.PalladiumPike, trueMelee },
                { ProjectileID.PiercingStarlight, Do(TrueMelee, IDStaticIFrames(4)) }, // Has an exception in Vanilla iframe code, uses 4 iframes
                { ProjectileID.PlatinumShortswordStab, trueMelee },
                { ProjectileID.RulerStab, trueMelee },
                { ProjectileID.ShadowJoustingLance, trueMelee },
                { ProjectileID.SilverShortswordStab, trueMelee },
                { ProjectileID.SolarFlareChainsaw, trueMeleeNoSpeed },
                { ProjectileID.Spark, trueMeleeNoSpeed },
                { ProjectileID.Spear, trueMelee },
                { ProjectileID.StardustChainsaw, trueMeleeNoSpeed },
                { ProjectileID.Swordfish, trueMelee },
                { ProjectileID.TheRottedFork, trueMelee },
                { ProjectileID.TinShortswordStab, trueMelee },
                { ProjectileID.TitaniumTrident, trueMelee },
                { ProjectileID.Trident, trueMelee },
                { ProjectileID.TungstenShortswordStab, trueMelee },
                { ProjectileID.VortexChainsaw, trueMeleeNoSpeed },
                #endregion

                #region CATEGORY 4: Defense Damage support
                { ProjectileID.BombSkeletronPrime, defenseDamage },
                { ProjectileID.CannonballHostile, defenseDamage },
                { ProjectileID.Cthulunado, defenseDamage },
                { ProjectileID.CultistBossFireBall, defenseDamage },
                { ProjectileID.CultistBossLightningOrbArc, defenseDamage }, // Also used by Storm Weaver
                { ProjectileID.DD2BetsyFlameBreath, defenseDamage },
                { ProjectileID.DeerclopsIceSpike, defenseDamage },
                { ProjectileID.DeerclopsRangedProjectile, defenseDamage },
                { ProjectileID.FairyQueenLance, defenseDamage }, // EoL lance
                { ProjectileID.FairyQueenSunDance, defenseDamage }, // EoL sun dance
                { ProjectileID.FlamingScythe, defenseDamage }, // Pumpking scythes
                { ProjectileID.FrostWave, defenseDamage }, // Ice Queen frost waves, reused by Storm Weaver
                { ProjectileID.HallowBossRainbowStreak, defenseDamage }, // EoL rainbow bolt
                { ProjectileID.HallowBossLastingRainbow, defenseDamage }, // EoL everlasting rainbow
                { ProjectileID.PhantasmalDeathray, defenseDamage },
                { ProjectileID.PhantasmalSphere, defenseDamage },
                { ProjectileID.Present, defenseDamage }, // Santa-NK1 presents
                { ProjectileID.QueenSlimeSmash, defenseDamage },
                { ProjectileID.RocketSkeleton, defenseDamage }, // Skeleton Commando rockets, reused by Rev+ Skeletron Prime
                { ProjectileID.SaucerDeathray, defenseDamage },
                { ProjectileID.SaucerMissile, defenseDamage },
                { ProjectileID.Sharknado, defenseDamage },
                { ProjectileID.Spike, defenseDamage }, // Santa-NK1 spike balls
                { ProjectileID.ThornBall, Do(Main.zenithWorld ? IgnoreWater : DontIgnoreWater, DefenseDamage) },
                #endregion

                #region CATEGORY 5: ID-Static Immunity Frame changes
                { ProjectileID.AbigailCounter, defaultIFrames },
                { ProjectileID.Ale, defaultIFrames },
                { ProjectileID.AmberBolt, defaultIFrames },
                { ProjectileID.AmethystBolt, defaultIFrames },
                { ProjectileID.AshBallFalling, defaultIFrames },
                { ProjectileID.BallofFire, defaultIFrames },
                { ProjectileID.BallofFrost, defaultIFrames },
                { ProjectileID.Bananarang, defaultIFrames },
                { ProjectileID.Bat, defaultIFrames },
                { ProjectileID.BeeHive, defaultIFrames },
                { ProjectileID.Beenade, defaultIFrames },
                { ProjectileID.BlackCat, defaultIFrames },
                { ProjectileID.Blizzard, defaultIFrames }, // Blizzard Staff projectiles, re-used in Frostbite Blaster.
                { ProjectileID.BloodArrow, defaultIFrames },
                { ProjectileID.BloodButcherer, defaultIFrames },
                { ProjectileID.BloodNautilusTears, defaultIFrames },
                { ProjectileID.BloodWater, defaultIFrames },
                { ProjectileID.BloodyMachete, defaultIFrames },
                { ProjectileID.BlueFlare, defaultIFrames },
                { ProjectileID.Bomb, defaultIFrames },
                { ProjectileID.BombFish, defaultIFrames },
                { ProjectileID.Bone, defaultIFrames },
                { ProjectileID.BoneArrow, defaultIFrames },
                { ProjectileID.BoneArrowFromMerchant, defaultIFrames },
                { ProjectileID.BoneDagger, defaultIFrames },
                { ProjectileID.BookOfSkullsSkull, defaultIFrames },
                { ProjectileID.BookStaffShot, defaultIFrames },
                { ProjectileID.Boulder, defaultIFrames },
                { ProjectileID.BoulderStaffOfEarth, defaultIFrames },
                { ProjectileID.BouncyBomb, defaultIFrames },
                { ProjectileID.BouncyBoulder, defaultIFrames },
                { ProjectileID.BouncyDynamite, defaultIFrames },
                { ProjectileID.BouncyGrenade, defaultIFrames },
                { ProjectileID.BoxingGlove, defaultIFrames },
                { ProjectileID.Bubble, defaultIFrames },
                { ProjectileID.Bunny, defaultIFrames },
                { ProjectileID.CandyCorn, Do(IDStaticIFrames(7)) }, // Has an exception in Vanilla iframe code, uses 7 iframes
                { ProjectileID.CannonballFriendly, defaultIFrames },
                { ProjectileID.CavelingGardener, defaultIFrames },
                { ProjectileID.Celeb2Rocket, defaultIFrames },
                { ProjectileID.Celeb2RocketExplosive, defaultIFrames },
                { ProjectileID.Celeb2RocketExplosiveLarge, defaultIFrames },
                { ProjectileID.Celeb2RocketLarge, defaultIFrames },
                { ProjectileID.Celeb2Weapon, defaultIFrames },
                { ProjectileID.ChainGuillotine, defaultIFrames },
                { ProjectileID.ChainKnife, defaultIFrames },
                { ProjectileID.ChargedBlasterCannon, defaultIFrames },
                { ProjectileID.ChargedBlasterLaser, defaultIFrames },
                { ProjectileID.ChargedBlasterOrb, defaultIFrames },
                { ProjectileID.ChlorophyteArrow, defaultIFrames },
                { ProjectileID.ChlorophyteBullet, defaultIFrames },
                { ProjectileID.ChlorophytePartisan, defaultIFrames },
                { ProjectileID.ClothiersCurse, defaultIFrames },
                { ProjectileID.ClusterMineI, defaultIFrames },
                { ProjectileID.ClusterMineII, defaultIFrames },
                { ProjectileID.ClusterSnowmanFragmentsI, defaultIFrames },
                { ProjectileID.ClusterSnowmanFragmentsII, defaultIFrames },
                { ProjectileID.CoinPortal, defaultIFrames },
                { ProjectileID.CopperCoin, defaultIFrames },
                { ProjectileID.CorruptSpray, defaultIFrames },
                { ProjectileID.CrimsandBallFalling, defaultIFrames },
                { ProjectileID.CrimsandBallGun, defaultIFrames },
                { ProjectileID.CrimsonHeart, defaultIFrames },
                { ProjectileID.CrimsonSpray, defaultIFrames },
                { ProjectileID.CrystalDart, Do(ExtraUpdatesExact(2), LocalIFrames(-1)) },
                { ProjectileID.CrystalLeaf, defaultIFrames },
                { ProjectileID.CrystalLeafShot, defaultIFrames },
                { ProjectileID.CrystalPulse, defaultIFrames },
                { ProjectileID.CrystalPulse2, defaultIFrames },
                { ProjectileID.CrystalShard, defaultIFrames },
                { ProjectileID.CrystalStorm, defaultIFrames },
                { ProjectileID.CrystalVileShardHead, defaultIFrames },
                { ProjectileID.CrystalVileShardShaft, defaultIFrames },
                { ProjectileID.CursedArrow, defaultIFrames },
                { ProjectileID.CursedDart, Do(ExtraUpdatesExact(1), LocalIFrames(20)) },
                { ProjectileID.CursedDartFlame, defaultIFrames },
                { ProjectileID.CursedFlameFriendly, defaultIFrames },
                { ProjectileID.CursedFlare, defaultIFrames },
                { ProjectileID.DD2PhoenixBow, defaultIFrames },
                { ProjectileID.DemonScythe, defaultIFrames },
                { ProjectileID.DiamondBolt, defaultIFrames },
                { ProjectileID.DirtBall, defaultIFrames },
                { ProjectileID.DirtBomb, defaultIFrames },
                { ProjectileID.DirtSpray, defaultIFrames },
                { ProjectileID.DirtStickyBomb, defaultIFrames },
                { ProjectileID.DripplerFlailExtraBall, defaultIFrames },
                { ProjectileID.DryadsWardCircle, defaultIFrames },
                { ProjectileID.DryBomb, defaultIFrames },
                { ProjectileID.DryMine, defaultIFrames },
                { ProjectileID.DryRocket, defaultIFrames },
                { ProjectileID.DrySnowmanRocket, defaultIFrames },
                { ProjectileID.Dynamite, defaultIFrames },
                { ProjectileID.EatersBite, defaultIFrames },
                { ProjectileID.EbonsandBallFalling, defaultIFrames },
                { ProjectileID.EbonsandBallGun, defaultIFrames },
                { ProjectileID.EighthNote, defaultIFrames },
                { ProjectileID.Electrosphere, Do(IDStaticIFrames(8)) }, // Has an exception in Vanilla iframe code, uses 8 iframes
                { ProjectileID.ElectrosphereMissile, defaultIFrames },
                { ProjectileID.EnchantedBeam, defaultIFrames },
                { ProjectileID.Explosives, defaultIFrames },
                { ProjectileID.FallingStar, defaultIFrames },
                { ProjectileID.FireArrow, defaultIFrames },
                { ProjectileID.Flairon, defaultIFrames },
                { ProjectileID.FlamesTrap, defaultIFrames },
                { ProjectileID.Flare, defaultIFrames },
                { ProjectileID.FlowerPowPetal, defaultIFrames },
                { ProjectileID.FrostArrow, defaultIFrames },
                { ProjectileID.FrostburnArrow, defaultIFrames },
                { ProjectileID.FrostDaggerfish, defaultIFrames },
                { ProjectileID.GasTrap, defaultIFrames },
                { ProjectileID.GelBalloon, defaultIFrames },
                { ProjectileID.Geode, defaultIFrames },
                { ProjectileID.GeyserTrap, defaultIFrames },
                { ProjectileID.GoldCoin, defaultIFrames },
                { ProjectileID.GolemFist, defaultIFrames },
                { ProjectileID.GreenLaser, defaultIFrames },
                { ProjectileID.Grenade, defaultIFrames },
                { ProjectileID.HallowSpray, defaultIFrames },
                { ProjectileID.HallowStar, defaultIFrames },
                { ProjectileID.Hellwing, defaultIFrames },
                { ProjectileID.HolyArrow, defaultIFrames },
                { ProjectileID.HeatRay, Do(DefaultIDStaticIFrames) },
                { ProjectileID.HolyWater, defaultIFrames },
                { ProjectileID.HoneyBomb, defaultIFrames },
                { ProjectileID.HoneyGrenade, defaultIFrames },
                { ProjectileID.HoneyMine, defaultIFrames },
                { ProjectileID.HoneyRocket, defaultIFrames },
                { ProjectileID.HoneySnowmanRocket, defaultIFrames },
                { ProjectileID.HornetStinger, defaultIFrames },
                { ProjectileID.HoundiusShootiusFireball, defaultIFrames },
                { ProjectileID.IceBlock, defaultIFrames },
                { ProjectileID.IceBolt, defaultIFrames },
                { ProjectileID.IchorArrow, defaultIFrames },
                { ProjectileID.IchorDart, Do(ExtraUpdatesExact(1), DefaultIDStaticIFrames) },
                { ProjectileID.JackOLantern, defaultIFrames },
                { ProjectileID.JavelinFriendly, defaultIFrames },
                { ProjectileID.JestersArrow, defaultIFrames },
                { ProjectileID.Landmine, defaultIFrames },
                { ProjectileID.LaserMachinegun, defaultIFrames },
                { ProjectileID.LaserMachinegunLaser, defaultIFrames },
                { ProjectileID.LastPrism, defaultIFrames },
                { ProjectileID.LastPrismLaser, Do(IDStaticIFrames(5)) }, // Has an exception in Vanilla iframe code, uses 5 iframes
                { ProjectileID.LavaBomb, defaultIFrames },
                { ProjectileID.LavaMine, defaultIFrames },
                { ProjectileID.LavaRocket, defaultIFrames },
                { ProjectileID.LavaSnowmanRocket, defaultIFrames },
                { ProjectileID.Leaf, defaultIFrames },
                { ProjectileID.LifeCrystalBoulder, defaultIFrames },
                { ProjectileID.MagicMissile, Do(IDStaticIFrames(8), SingleHitImmunity) }, // Has an exception in Vanilla iframe code, uses 8 iframes
                { ProjectileID.MagnetSphereBolt, defaultIFrames },
                { ProjectileID.MedusaHead, defaultIFrames },
                { ProjectileID.MedusaHeadRay, defaultIFrames },
                { ProjectileID.Meteor1, Do(IDStaticIFrames(5), SingleHitImmunity) }, // Has an exception in Vanilla iframe code, uses 5 iframes
                { ProjectileID.Meteor2, Do(IDStaticIFrames(5), SingleHitImmunity) }, // Has an exception in Vanilla iframe code, uses 5 iframes
                { ProjectileID.Meteor3, Do(IDStaticIFrames(5), SingleHitImmunity) }, // Has an exception in Vanilla iframe code, uses 5 iframes
                { ProjectileID.MinecartMechLaser, defaultIFrames },
                { ProjectileID.MiniBoulder, defaultIFrames },
                { ProjectileID.MiniMinotaur, defaultIFrames },
                { ProjectileID.MiniNukeMineI, defaultIFrames },
                { ProjectileID.MiniNukeMineII, defaultIFrames },
                { ProjectileID.MiniNukeRocketI, defaultIFrames },
                { ProjectileID.MiniNukeRocketII, defaultIFrames },
                { ProjectileID.MiniNukeSnowmanRocketI, defaultIFrames },
                { ProjectileID.MiniNukeSnowmanRocketII, defaultIFrames },
                { ProjectileID.MiniRetinaLaser, defaultIFrames },
                { ProjectileID.MiniSharkron, defaultIFrames },
                { ProjectileID.MolotovCocktail, defaultIFrames },
                { ProjectileID.MolotovFire, defaultIFrames },
                { ProjectileID.MolotovFire2, defaultIFrames },
                { ProjectileID.MolotovFire3, defaultIFrames },
                { ProjectileID.MonkStaffT3_AltShot, defaultIFrames },
                { ProjectileID.MudBall, defaultIFrames },
                { ProjectileID.Mushroom, defaultIFrames },
                { ProjectileID.MushroomSpray, defaultIFrames },
                { ProjectileID.NailFriendly, Do(IDStaticIFrames(1)) }, // Has an exception in Vanilla iframe code, uses 1 iframe
                { ProjectileID.NebulaArcanumExplosionShot, defaultIFrames },
                { ProjectileID.NebulaArcanumExplosionShotShard, defaultIFrames },
                { ProjectileID.NebulaBlaze1, Do(IDStaticIFrames(5), SingleHitImmunity) }, // Has an exception in Vanilla iframe code, uses 5 iframes
                { ProjectileID.NebulaBlaze2, Do(IDStaticIFrames(5), SingleHitImmunity) }, // Has an exception in Vanilla iframe code, uses 5 iframes
                { ProjectileID.NettleBurstEnd, defaultIFrames },
                { ProjectileID.NettleBurstLeft, defaultIFrames },
                { ProjectileID.NettleBurstRight, defaultIFrames },
                { ProjectileID.NightBeam, defaultIFrames },
                { ProjectileID.NorthPoleSpear, defaultIFrames },
                { ProjectileID.NorthPoleWeapon, defaultIFrames },
                { ProjectileID.NurseSyringeHeal, defaultIFrames },
                { ProjectileID.NurseSyringeHurt, defaultIFrames },
                { ProjectileID.OrnamentFriendly, defaultIFrames },
                { ProjectileID.OrnamentStar, defaultIFrames },
                { ProjectileID.PainterPaintball, defaultIFrames },
                { ProjectileID.PaladinsHammerFriendly, defaultIFrames },
                { ProjectileID.PaperAirplaneA, defaultIFrames },
                { ProjectileID.PaperAirplaneB, defaultIFrames },
                { ProjectileID.PartyGirlGrenade, defaultIFrames },
                { ProjectileID.PearlSandBallFalling, defaultIFrames },
                { ProjectileID.PearlSandBallGun, defaultIFrames },
                { ProjectileID.PewMaticHornShot, defaultIFrames },
                { ProjectileID.Phantasm, defaultIFrames },
                { ProjectileID.PhantasmArrow, defaultIFrames },
                { ProjectileID.PineNeedleFriendly, defaultIFrames },
                { ProjectileID.PlatinumCoin, defaultIFrames },
                { ProjectileID.PoisonDart, Do(ExtraUpdatesExact(1), DefaultIDStaticIFrames) },
                { ProjectileID.PoisonDartBlowgun, defaultIFrames },
                { ProjectileID.PoisonDartTrap, defaultIFrames },
                { ProjectileID.PoisonedKnife, defaultIFrames },
                { ProjectileID.PossessedHatchet, defaultIFrames },
                { ProjectileID.PrincessWeapon, defaultIFrames },
                { ProjectileID.ProximityMineI, defaultIFrames },
                { ProjectileID.ProximityMineII, defaultIFrames },
                { ProjectileID.ProximityMineIII, defaultIFrames },
                { ProjectileID.ProximityMineIV, defaultIFrames },
                { ProjectileID.PulseBolt, defaultIFrames },
                { ProjectileID.PureSpray, defaultIFrames },
                { ProjectileID.PurificationPowder, defaultIFrames },
                { ProjectileID.PygmySpear, defaultIFrames },
                { ProjectileID.QuarterNote, defaultIFrames },
                { ProjectileID.RainbowFlare, defaultIFrames },
                { ProjectileID.RocketFireworksBoxBlue, defaultIFrames },
                { ProjectileID.RocketFireworksBoxGreen, defaultIFrames },
                { ProjectileID.RocketFireworksBoxRed, defaultIFrames },
                { ProjectileID.RocketFireworksBoxYellow, defaultIFrames },
                { ProjectileID.RocketI, defaultIFrames },
                { ProjectileID.RocketII, defaultIFrames },
                { ProjectileID.RocketIII, defaultIFrames },
                { ProjectileID.RocketIV, defaultIFrames },
                { ProjectileID.RocketSnowmanI, defaultIFrames },
                { ProjectileID.RocketSnowmanII, defaultIFrames },
                { ProjectileID.RocketSnowmanIII, defaultIFrames },
                { ProjectileID.RocketSnowmanIV, defaultIFrames },
                { ProjectileID.RollingCactus, defaultIFrames },
                { ProjectileID.RollingCactusSpike, defaultIFrames },
                { ProjectileID.RottenEgg, defaultIFrames },
                { ProjectileID.RubyBolt, defaultIFrames },
                { ProjectileID.SandBallFalling, defaultIFrames },
                { ProjectileID.SandBallGun, defaultIFrames },
                { ProjectileID.SandSpray, defaultIFrames },
                { ProjectileID.SantaBombs, defaultIFrames },
                { ProjectileID.SantankMountRocket, defaultIFrames },
                { ProjectileID.SapphireBolt, defaultIFrames },
                { ProjectileID.ScarabBomb, defaultIFrames },
                { ProjectileID.ScutlixLaser, defaultIFrames },
                { ProjectileID.ScutlixLaserFriendly, defaultIFrames },
                { ProjectileID.Seed, defaultIFrames },
                { ProjectileID.SeedlerNut, defaultIFrames },
                { ProjectileID.SeedlerThorn, defaultIFrames },
                { ProjectileID.ShadowBeamFriendly, defaultIFrames },
                { ProjectileID.ShadowFlameArrow, defaultIFrames },
                { ProjectileID.ShadowFlameKnife, defaultIFrames },
                { ProjectileID.ShellPileFalling, defaultIFrames },
                { ProjectileID.ShimmerArrow, defaultIFrames },
                { ProjectileID.ShimmerFlare, defaultIFrames },
                { ProjectileID.Shuriken, defaultIFrames },
                { ProjectileID.SiltBall, defaultIFrames },
                { ProjectileID.SilverBullet, defaultIFrames },
                { ProjectileID.SilverCoin, defaultIFrames },
                { ProjectileID.SkyFracture, defaultIFrames },
                { ProjectileID.SlushBall, defaultIFrames },
                { ProjectileID.SnowBallFriendly, defaultIFrames },
                { ProjectileID.SnowSpray, defaultIFrames },
                { ProjectileID.SolarCounter, defaultIFrames },
                { ProjectileID.SolarFlareRay, defaultIFrames },
                { ProjectileID.SoulDrain, defaultIFrames },
                { ProjectileID.SpearTrap, defaultIFrames },
                { ProjectileID.SpelunkerFlare, defaultIFrames },
                { ProjectileID.Spider, defaultIFrames },
                { ProjectileID.SpiderEgg, defaultIFrames },
                { ProjectileID.SpikyBall, defaultIFrames },
                { ProjectileID.SpikyBallTrap, defaultIFrames },
                { ProjectileID.SpiritFlame, Do(IDStaticIFrames(5)) }, // Has an exception in Vanilla iframe code, uses 5 iframes
                { ProjectileID.SporeGas, defaultIFrames },
                { ProjectileID.SporeGas2, defaultIFrames },
                { ProjectileID.SporeGas3, defaultIFrames },
                { ProjectileID.SporeTrap, defaultIFrames },
                { ProjectileID.SporeTrap2, defaultIFrames },
                { ProjectileID.Stake, defaultIFrames },
                { ProjectileID.StarAnise, defaultIFrames },
                { ProjectileID.StarCannonStar, defaultIFrames },
                { ProjectileID.Starfury, defaultIFrames },
                { ProjectileID.StardustCellMinion, defaultIFrames },
                { ProjectileID.StardustGuardianExplosion, defaultIFrames },
                { ProjectileID.StickyBomb, defaultIFrames },
                { ProjectileID.StickyDynamite, defaultIFrames },
                { ProjectileID.StickyGrenade, defaultIFrames },
                { ProjectileID.StormTigerGem, defaultIFrames },
                { ProjectileID.Stynger, Do(IDStaticIFrames(7), SingleHitImmunity) }, // Has an exception in Vanilla iframe code, uses 7 iframes
                { ProjectileID.StyngerShrapnel, defaultIFrames },
                { ProjectileID.TentacleSpike, defaultIFrames },
                { ProjectileID.ThornChakram, defaultIFrames },
                { ProjectileID.ThrowingKnife, defaultIFrames },
                { ProjectileID.ThunderSpear, defaultIFrames },
                { ProjectileID.ThunderSpearShot, defaultIFrames },
                { ProjectileID.TiedEighthNote, defaultIFrames },
                { ProjectileID.TitaniumStormShard, defaultIFrames },
                { ProjectileID.TopazBolt, defaultIFrames },
                { ProjectileID.ToxicBubble, defaultIFrames },
                { ProjectileID.ToxicCloud, defaultIFrames },
                { ProjectileID.ToxicCloud2, defaultIFrames },
                { ProjectileID.ToxicCloud3, defaultIFrames },
                { ProjectileID.ToxicFlask, defaultIFrames },
                { ProjectileID.Truffle, defaultIFrames },
                { ProjectileID.TruffleSpore, defaultIFrames },
                { ProjectileID.UFOLaser, defaultIFrames },
                { ProjectileID.UFOMinion, defaultIFrames },
                { ProjectileID.UnholyArrow, defaultIFrames },
                { ProjectileID.UnholyTridentFriendly, defaultIFrames },
                { ProjectileID.UnholyWater, defaultIFrames },
                { ProjectileID.VampireKnife, defaultIFrames },
                { ProjectileID.VenomArrow, defaultIFrames },
                { ProjectileID.VenomDartTrap, defaultIFrames },
                { ProjectileID.ViciousPowder, defaultIFrames },
                { ProjectileID.VilePowder, defaultIFrames },
                { ProjectileID.VilethornBase, defaultIFrames },
                { ProjectileID.VilethornTip, defaultIFrames },
                { ProjectileID.VortexBeater, defaultIFrames },
                { ProjectileID.VortexBeaterRocket, defaultIFrames },
                { ProjectileID.VortexVortexLightning, defaultIFrames },
                { ProjectileID.VortexVortexPortal, defaultIFrames },
                { ProjectileID.Waffle, defaultIFrames },
                { ProjectileID.WandOfFrostingFrost, defaultIFrames },
                { ProjectileID.WandOfSparkingSpark, defaultIFrames },
                { ProjectileID.WaterBolt, defaultIFrames },
                { ProjectileID.WaterStream, defaultIFrames },
                { ProjectileID.Web, defaultIFrames },
                { ProjectileID.WetBomb, defaultIFrames },
                { ProjectileID.WetMine, defaultIFrames },
                { ProjectileID.WetRocket, defaultIFrames },
                { ProjectileID.WetSnowmanRocket, defaultIFrames },
                { ProjectileID.Wisp, defaultIFrames },
                { ProjectileID.WoodenArrowFriendly, defaultIFrames },
                { ProjectileID.WoodenBoomerang, defaultIFrames },
                { ProjectileID.Xenopopper, defaultIFrames },
                { ProjectileID.ZapinatorLaser, defaultIFrames },
                { ProjectileID.ZoologistStrikeGreen, defaultIFrames },
                { ProjectileID.ZoologistStrikeRed, defaultIFrames },

                #endregion
                
            };
        }

        internal static void UnloadTweaks()
        {
            currentTweaks?.Clear();
            currentTweaks = null;
        }
        #endregion

        #region SetDefaults (Projectile Tweaks Applied Here)
        internal static void SetDefaults_ApplyTweaks(Projectile proj)
        {
            // Do nothing if the tweaks database is not defined.
            if (currentTweaks is null)
                return;

            // Grab the tweaking or balancing to apply, if any. If nothing comes back, do nothing.
            bool needsTweaking = currentTweaks.TryGetValue(proj.type, out IProjectileTweak[] tweaks);
            if (!needsTweaking)
                return;

            // Apply all alterations sequentially, assuming they are relevant.
            foreach (IProjectileTweak tweak in tweaks)
                if (tweak.AppliesTo(proj))
                    tweak.ApplyTweak(proj);
        }
        #endregion

        #region Internal Structures

        // This function simply concatenates a bunch of Projectile Tweaks into an array.
        // It looks a lot nicer than constantly typing "new IProjectileTweak[]".
        internal static IProjectileTweak[] Do(params IProjectileTweak[] r) => r;

        // Only one applicability lambda.
        internal static bool IsAYoyo(Projectile proj) => proj.aiStyle == ProjAIStyleID.Yoyo;

        #region Projectile Tweak Definitions
        internal interface IProjectileTweak
        {
            bool AppliesTo(Projectile proj);
            void ApplyTweak(Projectile proj);
        }

        #region Built-In Armor Penetration
        internal class ArmorPenetrationDeltaRule : IProjectileTweak
        {
            internal readonly int delta = 0;

            public ArmorPenetrationDeltaRule(int d) => delta = d;
            public bool AppliesTo(Projectile proj) => true;
            public void ApplyTweak(Projectile proj) => proj.ArmorPenetration += delta;
        }
        internal static IProjectileTweak ArmorPenetrationDelta(int d) => new ArmorPenetrationDeltaRule(d);

        internal class ArmorPenetrationExactRule : IProjectileTweak
        {
            internal readonly int armorPen = 0;

            public ArmorPenetrationExactRule(int a) => armorPen = a;
            public bool AppliesTo(Projectile proj) => true;
            public void ApplyTweak(Projectile proj) => proj.ArmorPenetration = armorPen;
        }
        internal static IProjectileTweak ArmorPenetrationExact(int a) => new ArmorPenetrationExactRule(a);
        #endregion

        #region Defense Damage
        internal class DefenseDamageRule : IProjectileTweak
        {
            internal readonly bool flag = true;

            public DefenseDamageRule(bool dd) => flag = dd;
            public bool AppliesTo(Projectile proj) => true;
            public void ApplyTweak(Projectile proj) => proj.Calamity().DealsDefenseDamage = flag;
        }
        internal static IProjectileTweak DefenseDamage => new DefenseDamageRule(true);
        internal static IProjectileTweak NoDefenseDamage => new DefenseDamageRule(false);
        #endregion

        #region Extra Updates
        internal class ExtraUpdatesDeltaRule : IProjectileTweak
        {
            internal readonly int delta = 0;

            public ExtraUpdatesDeltaRule(int d) => delta = d;
            public bool AppliesTo(Projectile proj) => true;
            public void ApplyTweak(Projectile proj)
            {
                proj.extraUpdates += delta;
                if (proj.extraUpdates < 0)
                    proj.extraUpdates = 0;
            }
        }
        internal static IProjectileTweak ExtraUpdatesDelta(int d) => new ExtraUpdatesDeltaRule(d);

        internal class ExtraUpdatesExactRule : IProjectileTweak
        {
            internal readonly int newExtraUpdates = 0;

            public ExtraUpdatesExactRule(int eu) => newExtraUpdates = eu;
            public bool AppliesTo(Projectile proj) => true;
            public void ApplyTweak(Projectile proj)
            {
                proj.extraUpdates = newExtraUpdates;
                if (proj.extraUpdates < 0)
                    proj.extraUpdates = 0;
            }
        }
        internal static IProjectileTweak ExtraUpdatesExact(int eu) => new ExtraUpdatesExactRule(eu);

        // The MaxUpdates property is sometimes used in favor of the raw extraUpdates field.
        // Both are supported by Calamity Global Projectile Tweaks.
        internal class MaxUpdatesExactRule : IProjectileTweak
        {
            internal readonly int newMaxUpdates = 0;

            public MaxUpdatesExactRule(int mu) => newMaxUpdates = mu;
            public bool AppliesTo(Projectile proj) => true;
            public void ApplyTweak(Projectile proj)
            {
                proj.MaxUpdates = newMaxUpdates;
                if (proj.extraUpdates < 0)
                    proj.extraUpdates = 0;
            }
        }
        internal static IProjectileTweak MaxUpdatesExact(int mu) => new MaxUpdatesExactRule(mu);
        #endregion

        #region ID-Static Immunity Frames
        internal class IDStaticIFrameRule : IProjectileTweak
        {
            internal readonly int idStaticIFrameValue = -2;

            public IDStaticIFrameRule(int f) => idStaticIFrameValue = f;
            public bool AppliesTo(Projectile proj) => true;
            public void ApplyTweak(Projectile proj)
            {
                proj.usesLocalNPCImmunity = false;
                proj.localNPCHitCooldown = -2;
                proj.usesIDStaticNPCImmunity = true;
                proj.idStaticNPCHitCooldown = idStaticIFrameValue;
            }
        }
        internal static IProjectileTweak IDStaticIFrames(int f) => new IDStaticIFrameRule(f);
        internal static IProjectileTweak DefaultIDStaticIFrames => new IDStaticIFrameRule(10);
        #endregion

        #region Ignore Water
        internal class IgnoreWaterRule : IProjectileTweak
        {
            internal readonly bool flag = true;

            public IgnoreWaterRule(bool iw) => flag = iw;
            public bool AppliesTo(Projectile proj) => true;
            public void ApplyTweak(Projectile proj) => proj.ignoreWater = flag;
        }
        internal static IProjectileTweak IgnoreWater => new IgnoreWaterRule(true);
        internal static IProjectileTweak DontIgnoreWater => new IgnoreWaterRule(false);
        #endregion

        #region Local Immunity Frames
        internal class LocalIFrameRule : IProjectileTweak
        {
            internal readonly int localIFrameValue = -2;

            public LocalIFrameRule(int f) => localIFrameValue = f;
            public bool AppliesTo(Projectile proj) => true;
            public void ApplyTweak(Projectile proj)
            {
                proj.usesLocalNPCImmunity = true;
                proj.localNPCHitCooldown = localIFrameValue;
                proj.usesIDStaticNPCImmunity = false;
                proj.idStaticNPCHitCooldown = 0;
            }
        }
        internal static IProjectileTweak LocalIFrames(int f) => new LocalIFrameRule(f);
        internal static IProjectileTweak LocalIFramesOneHit = new LocalIFrameRule(-1);
        #endregion

        #region Piercing
        internal class PiercingDeltaRule : IProjectileTweak
        {
            internal readonly int delta = 0;

            public PiercingDeltaRule(int d) => delta = d;
            public bool AppliesTo(Projectile proj) => true;
            public void ApplyTweak(Projectile proj)
            {
                proj.penetrate += delta;
                if (proj.penetrate < 1)
                    proj.penetrate = 1;
                proj.maxPenetrate = proj.penetrate;
            }
        }
        internal static IProjectileTweak PiercingDelta(int p) => new PiercingDeltaRule(p);

        internal class PiercingExactRule : IProjectileTweak
        {
            internal readonly int newPenetrate = -1;

            public PiercingExactRule(int p) => newPenetrate = p;
            public bool AppliesTo(Projectile proj) => true;
            public void ApplyTweak(Projectile proj)
            {
                proj.penetrate = newPenetrate;
                if (proj.penetrate == 0)
                    proj.penetrate = 1;
                proj.maxPenetrate = proj.penetrate;
            }
        }
        internal static IProjectileTweak PiercingExact(int p) => new PiercingExactRule(p);
        internal static IProjectileTweak NoPiercing = new PiercingExactRule(1);
        internal static IProjectileTweak InfinitePiercing = new PiercingExactRule(-1);
        #endregion

        #region Scale
        internal class ScaleDeltaRule : IProjectileTweak
        {
            internal readonly float delta = 0;

            public ScaleDeltaRule(float d) => delta = d;
            public bool AppliesTo(Projectile proj) => true;
            public void ApplyTweak(Projectile proj)
            {
                proj.scale += delta;
                if (proj.scale < 0f)
                    proj.scale = 0f;
            }
        }
        internal static IProjectileTweak ScaleDelta(float d) => new ScaleDeltaRule(d);

        internal class ScaleExactRule : IProjectileTweak
        {
            internal readonly float newScale = 0;

            public ScaleExactRule(float s) => newScale = s;
            public bool AppliesTo(Projectile proj) => true;
            public void ApplyTweak(Projectile proj)
            {
                proj.scale = newScale;
                if (proj.scale < 0f)
                    proj.scale = 0f;
            }
        }
        internal static IProjectileTweak ScaleExact(float s) => new ScaleExactRule(s);

        internal class ScaleRatioRule : IProjectileTweak
        {
            internal readonly float ratio = 1f;

            public ScaleRatioRule(float f) => ratio = f;
            public bool AppliesTo(Projectile proj) => true;
            public void ApplyTweak(Projectile proj)
            {
                proj.scale *= ratio;
                if (proj.scale < 0f)
                    proj.scale = 0f;
            }
        }
        internal static IProjectileTweak ScaleRatio(float f) => new ScaleRatioRule(f);
        #endregion

        #region Single Hit Immunity
        internal class SingleHitImmunityRule : IProjectileTweak
        {
            internal readonly bool flag = false;

            public SingleHitImmunityRule(bool imm) => flag = imm;
            public bool AppliesTo(Projectile proj) => true;
            public void ApplyTweak(Projectile proj) => proj.appliesImmunityTimeOnSingleHits = flag;
        }
        internal static IProjectileTweak SingleHitImmunity => new SingleHitImmunityRule(true);
        #endregion

        #region Tile Collide
        internal class TileCollideRule : IProjectileTweak
        {
            internal readonly bool flag = true;

            public TileCollideRule(bool tc) => flag = tc;
            public bool AppliesTo(Projectile proj) => true;
            public void ApplyTweak(Projectile proj) => proj.tileCollide = flag;
        }
        internal static IProjectileTweak TileCollide => new TileCollideRule(true);
        internal static IProjectileTweak NoTileCollide => new TileCollideRule(false);
        #endregion

        #region Time Left
        internal class TimeLeftDeltaRule : IProjectileTweak
        {
            internal readonly int delta = 0;

            public TimeLeftDeltaRule(int d) => delta = d;
            public bool AppliesTo(Projectile proj) => true;
            public void ApplyTweak(Projectile proj)
            {
                proj.timeLeft += delta;
                if (proj.timeLeft < 1)
                    proj.timeLeft = 1;
            }
        }
        internal static IProjectileTweak TimeLeftDelta(int d) => new TimeLeftDeltaRule(d);

        internal class TimeLeftExactRule : IProjectileTweak
        {
            internal readonly int newTimeLeft = 0;

            public TimeLeftExactRule(int t) => newTimeLeft = t;
            public bool AppliesTo(Projectile proj) => true;
            public void ApplyTweak(Projectile proj)
            {
                proj.timeLeft = newTimeLeft;
                if (proj.timeLeft < 1)
                    proj.timeLeft = 1;
            }
        }
        internal static IProjectileTweak TimeLeftExact(int t) => new TimeLeftExactRule(t);
        #endregion

        #region True Melee
        internal class TrueMeleeRule : IProjectileTweak
        {
            public bool AppliesTo(Projectile proj) => true;
            public void ApplyTweak(Projectile proj) => proj.DamageType = TrueMeleeDamageClass.Instance;
        }
        internal static IProjectileTweak TrueMelee => new TrueMeleeRule();

        internal class TrueMeleeNoSpeedRule : IProjectileTweak
        {
            public bool AppliesTo(Projectile proj) => true;
            public void ApplyTweak(Projectile proj) => proj.DamageType = TrueMeleeNoSpeedDamageClass.Instance;
        }
        internal static IProjectileTweak TrueMeleeNoSpeed => new TrueMeleeNoSpeedRule();
        #endregion

        #region Yoyo Stats
        internal class YoyoLifetimeRule : IProjectileTweak
        {
            internal readonly float newLifetime = -1f; // -1 is unlimited. Otherwise it's the lifetime in seconds

            public YoyoLifetimeRule(float l) => newLifetime = l;
            public bool AppliesTo(Projectile proj) => IsAYoyo(proj);
            public void ApplyTweak(Projectile proj) => ProjectileID.Sets.YoyosLifeTimeMultiplier[proj.type] = newLifetime;
        }
        internal static IProjectileTweak YoyoLifetime(float l) => new YoyoLifetimeRule(l);

        internal class YoyoRangeRule : IProjectileTweak
        {
            internal readonly float newMaxRange = 0f; // Range is measured in pixels

            public YoyoRangeRule(float r) => newMaxRange = r;
            public bool AppliesTo(Projectile proj) => IsAYoyo(proj);
            public void ApplyTweak(Projectile proj) => ProjectileID.Sets.YoyosMaximumRange[proj.type] = newMaxRange;
        }
        internal static IProjectileTweak YoyoRange(float r) => new YoyoRangeRule(r);

        internal class YoyoTopSpeedRule : IProjectileTweak
        {
            internal readonly float newTopSpeed = 0f;

            public YoyoTopSpeedRule(float s) => newTopSpeed = s;
            public bool AppliesTo(Projectile proj) => IsAYoyo(proj);
            public void ApplyTweak(Projectile proj) => ProjectileID.Sets.YoyosTopSpeed[proj.type] = newTopSpeed;
        }
        internal static IProjectileTweak YoyoTopSpeed(float r) => new YoyoTopSpeedRule(r);
        #endregion
        #endregion
        #endregion
    }
}
