using System.Collections.Generic;
using CalamityMod.World;
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
            IProjectileTweak[] trueMelee = Do(TrueMelee);
            IProjectileTweak[] trueMeleeNoSpeed = Do(TrueMeleeNoSpeed);
            IProjectileTweak[] pointBlank = Do(PointBlank);
            IProjectileTweak[] standardBulletTweaks = Do(PointBlank, ExtraUpdatesDelta(+2));
            IProjectileTweak[] counterweightTweaks = Do(MaxUpdatesExact(2), IDStaticIFrames(10));

            // TODO -- Very few vanilla yoyos have range and speed tweaks. Looks like an unfinished job.

            // SORTING NOTES:
            // 1. Sort tweaks by categories first, then sort by the internal name in alphabetical order. Navigate through categories and names using the search function.
            // 2. Higher categories hold priority over lower ones (ie. Balancing with PB tweaks belong in balancing, rather than PB)
            // 3. Ambiguous internal names should have comments for ease of access.
            currentTweaks = new SortedDictionary<int, IProjectileTweak[]>
            {
                #region CATEGORY 1: Weapon/Enemy Balancing
                { ProjectileID.Amarok, Do(LocalIFrames(10)) },
                { ProjectileID.Anchor, Do(ExtraUpdatesExact(1)) },
                { ProjectileID.Bee, Do(PiercingExact(2)) },
                { ProjectileID.BlackCounterweight, counterweightTweaks },
                { ProjectileID.BlueCounterweight, counterweightTweaks },
                { ProjectileID.BlueMoon, Do(ExtraUpdatesExact(1)) },
                { ProjectileID.Bullet, standardBulletTweaks },
                { ProjectileID.BulletHighVelocity, Do(PointBlank, LocalIFrames(-1)) },
                { ProjectileID.ButchersChainsaw, Do(TrueMeleeNoSpeed, ScaleExact(1.5f)) },
                { ProjectileID.Cascade, Do(LocalIFrames(10)) },
                { ProjectileID.Chik, Do(LocalIFrames(10)) },
                { ProjectileID.Code1, Do(YoyoRange(240f), ExtraUpdatesExact(1), LocalIFrames(20)) }, // Original: 220 range
                { ProjectileID.Code2, Do(LocalIFrames(10)) },
                { ProjectileID.CorruptYoyo, Do(LocalIFrames(10)) }, // Malaise
                { ProjectileID.CrimsonYoyo, Do(LocalIFrames(10)) }, // Artery
                { ProjectileID.CrystalBullet, standardBulletTweaks },
                { ProjectileID.CrystalVileShardHead, Do(LocalIFrames(10)) },
                { ProjectileID.CrystalVileShardShaft, Do(LocalIFrames(10)) },
                { ProjectileID.CursedBullet, standardBulletTweaks },
                { ProjectileID.EmeraldBolt, Do(NoPiercing) },
                { ProjectileID.EmpressBlade, Do(LocalIFrames(30)) }, // Terraprisma
                { ProjectileID.EnchantedBoomerang, Do(ExtraUpdatesExact(1)) },
                { ProjectileID.ExplosiveBullet, standardBulletTweaks },
                { ProjectileID.Flamarang, Do(ExtraUpdatesExact(2)) },
                { ProjectileID.Flames, Do(IDStaticIFrames(6), ExtraUpdatesDelta(+1)) }, // Flamethrower + Elf Melter
                { ProjectileID.FlamingJack, Do(ExtraUpdatesExact(1)) },
                { ProjectileID.FlowerPetal, Do(MaxUpdatesExact(4), LocalIFrames(10)) }, // Orichalcum armor
                { ProjectileID.FlowerPow, Do(ExtraUpdatesExact(1)) },
                { ProjectileID.FlyingKnife, Do(ExtraUpdatesExact(1)) }, 
                { ProjectileID.FormatC, Do(LocalIFrames(10)) },
                { ProjectileID.FrostBoltStaff, Do(ExtraUpdatesExact(1)) },
                { ProjectileID.FruitcakeChakram, Do(ExtraUpdatesExact(1)) },
                { ProjectileID.GiantBee, Do(PiercingExact(2)) },
                { ProjectileID.GoldenBullet, standardBulletTweaks },
                { ProjectileID.Gradient, Do(LocalIFrames(10)) },
                { ProjectileID.GreenCounterweight, counterweightTweaks },
                { ProjectileID.HelFire, Do(LocalIFrames(10)) },
                { ProjectileID.IceBoomerang, Do(ExtraUpdatesExact(1)) },
                { ProjectileID.IchorBullet, standardBulletTweaks },
                { ProjectileID.InfluxWaver, Do(ExtraUpdatesExact(1)) },
                { ProjectileID.JungleYoyo, Do(LocalIFrames(10)) }, // Amazon
                { ProjectileID.Kraken, Do(LocalIFrames(10)) },
                { ProjectileID.LightDisc, Do(MaxUpdatesExact(3)) },
                { ProjectileID.LostSoulHostile, Do(TileCollide) }, // Ragged Caster
                { ProjectileID.MeteorShot, standardBulletTweaks },
                { ProjectileID.MonkStaffT1, Do(TrueMeleeNoSpeed, ScaleExact(3f)) }, // Sleepy Octopod
                { ProjectileID.MonkStaffT2, Do(TrueMelee, IDStaticIFrames(18)) }, // Ghastly Glaive
                { ProjectileID.MonkStaffT3, Do(ScaleRatio(2f)) }, // Sky Dragon's Fury
                { ProjectileID.MoonlordBullet, standardBulletTweaks }, // Luminite Bullet
                { ProjectileID.NanoBullet, standardBulletTweaks },
                { ProjectileID.NebulaLaser, Do(ExtraUpdatesDelta(+1)) },
                { ProjectileID.PartyBullet, standardBulletTweaks },
                { ProjectileID.PoisonFang, Do(LocalIFrames(10)) },
                { ProjectileID.PurpleCounterweight, counterweightTweaks },
                { ProjectileID.QueenSlimeGelAttack, Do(NoPiercing) },
                { ProjectileID.QueenSlimeMinionPinkBall, Do(NoPiercing) },
                { ProjectileID.Rally, Do(LocalIFrames(10)) },
                { ProjectileID.RedCounterweight, counterweightTweaks },
                { ProjectileID.RedsYoyo, Do(LocalIFrames(10)) },
                { ProjectileID.ShadowBeamHostile, Do(TimeLeftExact(60)) },
                { ProjectileID.Shroomerang, Do(ExtraUpdatesExact(1)) },
                { ProjectileID.StarWrath, Do(NoPiercing) },
                { ProjectileID.Sunfury, Do(ExtraUpdatesExact(1)) },
                { ProjectileID.Terrarian, Do(LocalIFrames(10)) },
                { ProjectileID.TerrarianBeam, Do(LocalIFrames(-1)) }, // Terrarian yoyo orbs
                { ProjectileID.TheEyeOfCthulhu, Do(LocalIFrames(10)) }, // this is the yoyo
                { ProjectileID.Trimarang, Do(ExtraUpdatesExact(1)) },
                { ProjectileID.ValkyrieYoyo, Do(LocalIFrames(10)) },
                { ProjectileID.Valor, Do(YoyoRange(384f), YoyoTopSpeed(16f), LocalIFrames(10)) }, // Original: 225 range, 14 speed
                { ProjectileID.VenomBullet, standardBulletTweaks },
                { ProjectileID.VenomFang, Do(LocalIFrames(10)) },
                { ProjectileID.WoodYoyo, Do(LocalIFrames(10)) },
                { ProjectileID.Yelets, Do(LocalIFrames(10)) },
                { ProjectileID.YellowCounterweight, counterweightTweaks },
                #endregion

                #region CATEGORY 2: True Melee support
                { ProjectileID.AdamantiteChainsaw, trueMeleeNoSpeed },
                { ProjectileID.AdamantiteDrill, trueMeleeNoSpeed },
                { ProjectileID.AdamantiteGlaive, trueMelee },
                { ProjectileID.Arkhalis, trueMeleeNoSpeed },
                { ProjectileID.ChlorophyteChainsaw, trueMeleeNoSpeed },
                { ProjectileID.ChlorophyteDrill, trueMeleeNoSpeed },
                { ProjectileID.ChlorophyteJackhammer, trueMeleeNoSpeed },
                { ProjectileID.CobaltChainsaw, trueMeleeNoSpeed },
                { ProjectileID.CobaltDrill, trueMeleeNoSpeed },
                { ProjectileID.CobaltNaginata, trueMelee },
                { ProjectileID.CopperShortswordStab, trueMelee },
                { ProjectileID.DarkLance, trueMelee },
                { ProjectileID.GladiusStab, trueMelee },
                { ProjectileID.GoldShortswordStab, trueMelee },
                { ProjectileID.Gungnir, trueMelee },
                { ProjectileID.HallowJoustingLance, trueMelee },
                { ProjectileID.Hamdrax, trueMeleeNoSpeed }, // Drax (never internally renamed since 1.1)
                { ProjectileID.IronShortswordStab, trueMelee },
                { ProjectileID.JoustingLance, trueMelee },
                { ProjectileID.LeadShortswordStab, trueMelee },
                { ProjectileID.MushroomSpear, trueMelee },
                { ProjectileID.MythrilChainsaw, trueMeleeNoSpeed },
                { ProjectileID.MythrilDrill, trueMeleeNoSpeed },
                { ProjectileID.MythrilHalberd, trueMelee },
                { ProjectileID.NebulaChainsaw, trueMeleeNoSpeed },
                { ProjectileID.NebulaDrill, trueMeleeNoSpeed },
                { ProjectileID.ObsidianSwordfish, trueMelee },
                { ProjectileID.OrichalcumChainsaw, trueMeleeNoSpeed },
                { ProjectileID.OrichalcumDrill, trueMeleeNoSpeed },
                { ProjectileID.OrichalcumHalberd, trueMelee },
                { ProjectileID.PalladiumChainsaw, trueMeleeNoSpeed },
                { ProjectileID.PalladiumDrill, trueMeleeNoSpeed },
                { ProjectileID.PalladiumPike, trueMelee },
                { ProjectileID.PiercingStarlight, trueMelee },
                { ProjectileID.PlatinumShortswordStab, trueMelee },
                { ProjectileID.RulerStab, trueMelee },
                { ProjectileID.SawtoothShark, trueMeleeNoSpeed },
                { ProjectileID.ShadowJoustingLance, trueMelee },
                { ProjectileID.SilverShortswordStab, trueMelee },
                { ProjectileID.SolarFlareChainsaw, trueMeleeNoSpeed },
                { ProjectileID.SolarFlareDrill, trueMeleeNoSpeed },
                { ProjectileID.Spear, trueMelee },
                { ProjectileID.StardustChainsaw, trueMeleeNoSpeed },
                { ProjectileID.StardustDrill, trueMeleeNoSpeed },
                { ProjectileID.Swordfish, trueMelee },
                { ProjectileID.Terragrim, trueMeleeNoSpeed },
                { ProjectileID.TheRottedFork, trueMelee },
                { ProjectileID.TinShortswordStab, trueMelee },
                { ProjectileID.TitaniumChainsaw, trueMeleeNoSpeed },
                { ProjectileID.TitaniumDrill, trueMeleeNoSpeed },
                { ProjectileID.TitaniumTrident, trueMelee },
                { ProjectileID.Trident, trueMelee },
                { ProjectileID.TungstenShortswordStab, trueMelee },
                { ProjectileID.VortexChainsaw, trueMeleeNoSpeed },
                { ProjectileID.VortexDrill, trueMeleeNoSpeed },
                #endregion

                #region CATEGORY 3: Point Blank support
                { ProjectileID.BeeArrow, pointBlank },
                { ProjectileID.Blizzard, pointBlank }, // Blizzard Staff projectiles, re-used in Frostbite Blaster.
                { ProjectileID.BlueFlare, pointBlank },
                { ProjectileID.BoneArrow, pointBlank },
                { ProjectileID.CandyCorn, pointBlank },
                { ProjectileID.ChlorophyteArrow, pointBlank },
                { ProjectileID.ChlorophyteBullet, pointBlank },
                { ProjectileID.CrimsandBallGun, pointBlank },
                { ProjectileID.CrystalDart, pointBlank },
                { ProjectileID.CursedArrow, pointBlank },
                { ProjectileID.CursedDart, pointBlank },
                { ProjectileID.DD2PhoenixBowShot, pointBlank }, // Phantom Phoenix
                { ProjectileID.EbonsandBallGun, pointBlank },
                { ProjectileID.FireArrow, pointBlank },
                { ProjectileID.Flare, pointBlank },
                { ProjectileID.FrostburnArrow, pointBlank },
                { ProjectileID.Harpoon, pointBlank },
                { ProjectileID.HellfireArrow, pointBlank },
                { ProjectileID.Hellwing, pointBlank },
                { ProjectileID.HolyArrow, pointBlank },
                { ProjectileID.IchorArrow, pointBlank },
                { ProjectileID.JestersArrow, pointBlank },
                { ProjectileID.MoonlordArrow, pointBlank }, // Luminite Arrow
                { ProjectileID.PainterPaintball, pointBlank },
                { ProjectileID.PearlSandBallGun, pointBlank },
                { ProjectileID.PhantasmArrow, pointBlank },
                { ProjectileID.PoisonDartBlowgun, pointBlank },
                { ProjectileID.PulseBolt, pointBlank },
                { ProjectileID.SandBallGun, pointBlank },
                { ProjectileID.Seed, pointBlank },
                { ProjectileID.ShadowFlameArrow, pointBlank },
                { ProjectileID.SnowBallFriendly, pointBlank },
                { ProjectileID.Stake, pointBlank },
                { ProjectileID.UnholyArrow, pointBlank },
                { ProjectileID.VenomArrow, pointBlank },
                { ProjectileID.WoodenArrowFriendly, pointBlank },
                #endregion

                #region CATEGORY 4: Defense Damage support
                { ProjectileID.BombSkeletronPrime, defenseDamage },
                { ProjectileID.CannonballHostile, defenseDamage },
                { ProjectileID.Cthulunado, defenseDamage }, // Duke Fishron's larger Sharknados
                { ProjectileID.CultistBossFireBall, defenseDamage },
                { ProjectileID.CultistBossFireBallClone, defenseDamage },
                { ProjectileID.CultistBossIceMist, defenseDamage },
                { ProjectileID.CultistBossLightningOrbArc, defenseDamage },
                { ProjectileID.DD2BetsyFireball, defenseDamage },
                { ProjectileID.DD2BetsyFlameBreath, defenseDamage },
                { ProjectileID.DeerclopsIceSpike, defenseDamage },
                { ProjectileID.DeerclopsRangedProjectile, defenseDamage }, // Deerclops shadow hands
                { ProjectileID.DemonSickle, defenseDamage },
                { ProjectileID.FairyQueenLance, defenseDamage }, // Empress of Light's lance walls
                { ProjectileID.FairyQueenSunDance, defenseDamage }, // Empress of Light's Sun Dance
                { ProjectileID.FlamingScythe, defenseDamage }, // Pumpking orange spinning scythes
                { ProjectileID.FrostWave, defenseDamage }, // Ice Queen frost waves
                { ProjectileID.HallowBossLastingRainbow, defenseDamage }, // Empress of Light's lingering rainbow trail hitboxes
                { ProjectileID.InfernoHostileBlast, defenseDamage }, // Diabolist inferno fork explosions
                { ProjectileID.JavelinHostile, defenseDamage },
                { ProjectileID.PaladinsHammerHostile, defenseDamage },
                { ProjectileID.PhantasmalDeathray, defenseDamage },
                { ProjectileID.PhantasmalSphere, defenseDamage },
                { ProjectileID.Present, defenseDamage }, // Falling present bombs in Frost Moon
                { ProjectileID.RocketSkeleton, defenseDamage }, // Skeleton Commando rockets
                { ProjectileID.RockGolemRock, defenseDamage },
                { ProjectileID.RuneBlast, defenseDamage }, // Rune Wizard shots
                { ProjectileID.SaucerDeathray, defenseDamage },
                { ProjectileID.SaucerMissile, defenseDamage },
                { ProjectileID.Sharknado, defenseDamage },
                { ProjectileID.Skull, defenseDamage }, // Skeletron Expert+ skulls
                { ProjectileID.SniperBullet, defenseDamage }, // Skeleton Sniper bullets
                { ProjectileID.Spike, defenseDamage }, // Santank spike balls
                { ProjectileID.ThornBall, Do(Main.zenithWorld ? IgnoreWater : DontIgnoreWater, DefenseDamage) }, // Plantera bouncing thorn balls
                { ProjectileID.UnholyTridentHostile, defenseDamage },
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
                if (proj.penetrate < 1)
                    proj.penetrate = 1;
                proj.maxPenetrate = proj.penetrate;
            }
        }
        internal static IProjectileTweak PiercingExact(int p) => new PiercingExactRule(p);
        internal static IProjectileTweak NoPiercing = new PiercingExactRule(1);
        internal static IProjectileTweak InfinitePiercing = new PiercingExactRule(-1);
        #endregion

        #region Point Blank
        internal class PointBlankRule : IProjectileTweak
        {
            public bool AppliesTo(Projectile proj) => true;
            public void ApplyTweak(Projectile proj)
                => proj.Calamity().pointBlankShotDuration = DefaultPointBlankDuration;
        }
        internal static IProjectileTweak PointBlank => new PointBlankRule();
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
        internal class YoyoRangeRule : IProjectileTweak
        {
            internal readonly float newMaxRange = 0f;

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
