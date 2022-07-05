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
            IProjectileTweak[] trueMelee = Do(TrueMelee);
            IProjectileTweak[] trueMeleeNoSpeed = Do(TrueMeleeNoSpeed);
            IProjectileTweak[] pointBlank = Do(PointBlank);
            IProjectileTweak[] standardBulletTweaks = Do(PointBlank, ExtraUpdatesDelta(+2));
            IProjectileTweak[] counterweightTweaks = Do(MaxUpdatesExact(2), IDStaticIFrames(10));

            // TODO -- Very few vanilla yoyos have range and speed tweaks. Looks like an unfinished job.

            // Please keep this strictly alphabetical. It's the only way to keep it sane. Thanks in advance.
            // - Ozzatron
            currentTweaks = new SortedDictionary<int, IProjectileTweak[]>
            {
                { ProjectileID.AdamantiteChainsaw, trueMeleeNoSpeed },
                { ProjectileID.AdamantiteDrill, trueMeleeNoSpeed },
                { ProjectileID.AdamantiteGlaive, trueMelee },
                { ProjectileID.Amarok, Do(LocalIFrames(10)) },
                { ProjectileID.Anchor, Do(ExtraUpdatesExact(1)) },
                { ProjectileID.Arkhalis, Do(TrueMeleeNoSpeed, ScaleExact(1.5f)) },
                { ProjectileID.Bee, Do(PiercingExact(2)) },
                { ProjectileID.BeeArrow, pointBlank },
                { ProjectileID.BlackCounterweight, counterweightTweaks },
                { ProjectileID.Blizzard, pointBlank }, // Blizzard Staff projectiles, re-used in Frostbite Blaster.
                { ProjectileID.BlueCounterweight, counterweightTweaks },
                { ProjectileID.BlueFlare, pointBlank },
                { ProjectileID.BombSkeletronPrime, defenseDamage },
                { ProjectileID.BoneArrow, pointBlank },
                { ProjectileID.BlueMoon, Do(ExtraUpdatesExact(1)) },
                { ProjectileID.Bullet, standardBulletTweaks },
                { ProjectileID.BulletHighVelocity, pointBlank },
                { ProjectileID.ButchersChainsaw, Do(TrueMeleeNoSpeed, ScaleExact(1.5f)) },
                { ProjectileID.CandyCorn, pointBlank },
                { ProjectileID.CannonballHostile, defenseDamage },
                { ProjectileID.Cascade, Do(LocalIFrames(10)) },
                { ProjectileID.Chik, Do(LocalIFrames(10)) },
                { ProjectileID.ChlorophyteArrow, pointBlank },
                { ProjectileID.ChlorophyteBullet, pointBlank },
                { ProjectileID.ChlorophyteChainsaw, trueMeleeNoSpeed },
                { ProjectileID.ChlorophyteDrill, trueMeleeNoSpeed },
                { ProjectileID.ChlorophyteJackhammer, trueMeleeNoSpeed },
                { ProjectileID.CobaltChainsaw, trueMeleeNoSpeed },
                { ProjectileID.CobaltDrill, trueMeleeNoSpeed },
                { ProjectileID.CobaltNaginata, trueMelee },
                { ProjectileID.Code1, Do(YoyoRange(240f), ExtraUpdatesExact(1), LocalIFrames(20)) },
                { ProjectileID.Code2, Do(LocalIFrames(10)) },
                { ProjectileID.CorruptYoyo, Do(YoyoRange(190f), LocalIFrames(10)) }, // Malaise
                { ProjectileID.CrimsandBallGun, pointBlank },
                { ProjectileID.CrimsonYoyo, Do(LocalIFrames(10)) }, // Artery
                { ProjectileID.CrystalBullet, standardBulletTweaks },
                { ProjectileID.CrystalDart, pointBlank },
                { ProjectileID.CrystalVileShardHead, Do(LocalIFrames(10)) },
                { ProjectileID.CrystalVileShardShaft, Do(LocalIFrames(10)) },
                { ProjectileID.Cthulunado, defenseDamage }, // Duke Fishron's larger Sharknados
                { ProjectileID.CultistBossFireBall, defenseDamage },
                { ProjectileID.CultistBossFireBallClone, defenseDamage },
                { ProjectileID.CultistBossIceMist, defenseDamage },
                { ProjectileID.CultistBossLightningOrbArc, defenseDamage },
                { ProjectileID.CursedArrow, pointBlank },
                { ProjectileID.CursedBullet, standardBulletTweaks },
                { ProjectileID.CursedDart, pointBlank },
                { ProjectileID.DarkLance, trueMelee },
                { ProjectileID.DD2BetsyFireball, defenseDamage },
                { ProjectileID.DD2BetsyFlameBreath, defenseDamage },
                { ProjectileID.DD2PhoenixBowShot, pointBlank }, // Phantom Phoenix
                { ProjectileID.DeerclopsIceSpike, defenseDamage },
                { ProjectileID.DeerclopsRangedProjectile, defenseDamage }, // Deerclops shadow hands
                { ProjectileID.DemonSickle, defenseDamage },
                { ProjectileID.EbonsandBallGun, pointBlank },
                { ProjectileID.EmeraldBolt, Do(NoPiercing) },
                { ProjectileID.EnchantedBoomerang, Do(ExtraUpdatesExact(1)) },
                { ProjectileID.ExplosiveBullet, standardBulletTweaks },
                { ProjectileID.FairyQueenLance, defenseDamage }, // Empress of Light's lance walls
                { ProjectileID.FairyQueenSunDance, defenseDamage }, // Empress of Light's Sun Dance
                { ProjectileID.FireArrow, pointBlank },
                { ProjectileID.Flamarang, Do(ExtraUpdatesExact(2)) },
                { ProjectileID.Flames, Do(IDStaticIFrames(6), ExtraUpdatesDelta(+1)) }, // Flamethrower + Elf Melter
                { ProjectileID.FlamingJack, Do(ExtraUpdatesExact(1)) },
                { ProjectileID.FlamingScythe, defenseDamage }, // Pumpking orange spinning scythes
                { ProjectileID.Flare, pointBlank },
                { ProjectileID.FlowerPetal, Do(MaxUpdatesExact(4), LocalIFrames(10)) },
                { ProjectileID.FlowerPow, Do(ExtraUpdatesExact(1)) },
                { ProjectileID.FormatC, Do(LocalIFrames(10)) },
                { ProjectileID.FrostBoltStaff, Do(ExtraUpdatesExact(1)) },
                { ProjectileID.FrostburnArrow, pointBlank },
                { ProjectileID.FrostWave, defenseDamage }, // Ice Queen frost waves
                { ProjectileID.FruitcakeChakram, Do(ExtraUpdatesExact(1)) },
                { ProjectileID.GiantBee, Do(PiercingExact(2)) },
                { ProjectileID.GoldenBullet, standardBulletTweaks },
                { ProjectileID.Gradient, Do(LocalIFrames(10)) },
                { ProjectileID.GreenCounterweight, counterweightTweaks },
                { ProjectileID.Gungnir, trueMelee },
                { ProjectileID.HallowBossLastingRainbow, defenseDamage }, // Empress of Light's lingering rainbow trail hitboxes
                { ProjectileID.HallowJoustingLance, trueMelee },
                { ProjectileID.Hamdrax, trueMeleeNoSpeed }, // Drax (never internally renamed since 1.1)
                { ProjectileID.Harpoon, pointBlank },
                { ProjectileID.HelFire, Do(LocalIFrames(10)) },
                { ProjectileID.HellfireArrow, pointBlank },
                { ProjectileID.Hellwing, pointBlank },
                { ProjectileID.HolyArrow, pointBlank },
                { ProjectileID.IceBoomerang, Do(ExtraUpdatesExact(1)) },
                { ProjectileID.IchorArrow, pointBlank },
                { ProjectileID.IchorBullet, standardBulletTweaks },
                { ProjectileID.InfernoHostileBlast, defenseDamage }, // Diabolist inferno fork explosions
                { ProjectileID.JavelinHostile, defenseDamage },
                { ProjectileID.JestersArrow, pointBlank },
                { ProjectileID.JoustingLance, trueMelee },
                { ProjectileID.JungleYoyo, Do(YoyoTopSpeed(14f), LocalIFrames(10)) }, // Amazon
                { ProjectileID.Kraken, Do(LocalIFrames(10)) },
                { ProjectileID.LightBeam, Do(PiercingExact(2)) },
                { ProjectileID.LightDisc, Do(MaxUpdatesExact(6)) },
                { ProjectileID.LostSoulFriendly, pointBlank }, // TODO -- why does LostSoulFriendly have point blank enabled
                { ProjectileID.LostSoulHostile, Do(TileCollide) },
                { ProjectileID.MeteorShot, standardBulletTweaks },
                { ProjectileID.MiniRetinaLaser, Do(LocalIFrames(10)) },
                { ProjectileID.MonkStaffT1, Do(TrueMeleeNoSpeed, ScaleExact(3f)) }, // Sleepy Octopod
                { ProjectileID.MonkStaffT2, Do(TrueMelee, IDStaticIFrames(18)) }, // Ghastly Glaive
                { ProjectileID.MonkStaffT3, Do(ScaleRatio(2f)) }, // Sky Dragon's Fury
                { ProjectileID.MoonlordArrow, pointBlank }, // Luminite Arrow
                { ProjectileID.MoonlordBullet, standardBulletTweaks }, // Luminite Bullet
                { ProjectileID.MushroomSpear, trueMelee },
                { ProjectileID.MythrilChainsaw, trueMeleeNoSpeed },
                { ProjectileID.MythrilDrill, trueMeleeNoSpeed },
                { ProjectileID.MythrilHalberd, trueMelee },
                { ProjectileID.NanoBullet, standardBulletTweaks },
                { ProjectileID.NebulaChainsaw, trueMeleeNoSpeed },
                { ProjectileID.NebulaDrill, trueMeleeNoSpeed },
                { ProjectileID.NebulaLaser, Do(ExtraUpdatesDelta(+1)) },
                { ProjectileID.ObsidianSwordfish, trueMelee },
                { ProjectileID.OrichalcumChainsaw, trueMeleeNoSpeed },
                { ProjectileID.OrichalcumDrill, trueMeleeNoSpeed },
                { ProjectileID.OrichalcumHalberd, trueMelee },
                { ProjectileID.PainterPaintball, pointBlank },
                { ProjectileID.PaladinsHammerHostile, defenseDamage },
                { ProjectileID.PalladiumChainsaw, trueMeleeNoSpeed },
                { ProjectileID.PalladiumDrill, trueMeleeNoSpeed },
                { ProjectileID.PalladiumPike, trueMelee },
                { ProjectileID.PartyBullet, standardBulletTweaks },
                { ProjectileID.PearlSandBallGun, pointBlank },
                { ProjectileID.PhantasmalDeathray, defenseDamage },
                { ProjectileID.PhantasmalSphere, defenseDamage },
                { ProjectileID.PhantasmArrow, pointBlank },
                { ProjectileID.PoisonDartBlowgun, pointBlank },
                { ProjectileID.PoisonFang, Do(LocalIFrames(10)) },
                { ProjectileID.Present, defenseDamage }, // Falling present bombs in Frost Moon
                { ProjectileID.PulseBolt, pointBlank },
                { ProjectileID.PurpleCounterweight, counterweightTweaks },
                { ProjectileID.QueenSlimeGelAttack, Do(NoPiercing) },
                { ProjectileID.QueenSlimeMinionPinkBall, Do(NoPiercing) },
                { ProjectileID.Rally, Do(LocalIFrames(10)) },
                { ProjectileID.RedCounterweight, counterweightTweaks },
                { ProjectileID.RedsYoyo, Do(LocalIFrames(10)) },
                { ProjectileID.Retanimini, Do(LocalIFrames(10)) }, // Optic Staff (Mini Retinazer)
                { ProjectileID.RocketSkeleton, defenseDamage }, // Skeleton Commando rockets
                { ProjectileID.RockGolemRock, defenseDamage },
                { ProjectileID.RuneBlast, defenseDamage }, // Rune Wizard shots
                { ProjectileID.SandBallGun, pointBlank },
                { ProjectileID.SaucerDeathray, defenseDamage },
                { ProjectileID.SaucerMissile, defenseDamage },
                { ProjectileID.SawtoothShark, trueMeleeNoSpeed },
                { ProjectileID.Seed, pointBlank },
                { ProjectileID.ShadowBeamHostile, Do(TimeLeftExact(60)) },
                { ProjectileID.ShadowFlameArrow, pointBlank },
                { ProjectileID.ShadowJoustingLance, trueMelee },
                { ProjectileID.Sharknado, defenseDamage },
                { ProjectileID.Skull, defenseDamage }, // Skeletron Expert+ skulls
                { ProjectileID.SniperBullet, defenseDamage }, // Skeleton Sniper bullets
                { ProjectileID.SnowBallFriendly, pointBlank },
                { ProjectileID.SolarFlareChainsaw, trueMeleeNoSpeed },
                { ProjectileID.SolarFlareDrill, trueMeleeNoSpeed },
                { ProjectileID.Spazmamini, Do(IDStaticIFrames(12)) }, // Optic Staff (Mini Spazmatism)
                { ProjectileID.Spear, trueMelee },
                { ProjectileID.Spike, defenseDamage }, // UNKNOWN
                { ProjectileID.Stake, pointBlank },
                { ProjectileID.StardustChainsaw, trueMeleeNoSpeed },
                { ProjectileID.StardustDrill, trueMeleeNoSpeed },
                { ProjectileID.StarWrath, Do(NoPiercing) },
                { ProjectileID.Sunfury, Do(ExtraUpdatesExact(1)) },
                { ProjectileID.Swordfish, trueMelee },
                { ProjectileID.Terragrim, Do(TrueMeleeNoSpeed, ScaleExact(1.5f)) },
                { ProjectileID.Terrarian, Do(LocalIFrames(10)) },
                { ProjectileID.TerrarianBeam, Do(LocalIFrames(-1)) }, // Terrarian yoyo orbs
                { ProjectileID.TheEyeOfCthulhu, Do(LocalIFrames(10)) }, // this is the yoyo
                { ProjectileID.TheRottedFork, trueMelee },
                { ProjectileID.ThornBall, defenseDamage }, // Plantera bouncing thorn balls
                { ProjectileID.TitaniumChainsaw, trueMeleeNoSpeed },
                { ProjectileID.TitaniumDrill, trueMeleeNoSpeed },
                { ProjectileID.TitaniumTrident, trueMelee },
                { ProjectileID.Trident, trueMelee },
                { ProjectileID.UnholyArrow, pointBlank },
                { ProjectileID.UnholyTridentHostile, defenseDamage },
                { ProjectileID.ValkyrieYoyo, Do(LocalIFrames(10)) },
                { ProjectileID.Valor, Do(YoyoRange(384f), YoyoTopSpeed(16f), LocalIFrames(10)) },
                { ProjectileID.VenomArrow, pointBlank },
                { ProjectileID.VenomBullet, standardBulletTweaks },
                { ProjectileID.VenomFang, Do(LocalIFrames(10)) },
                { ProjectileID.VortexChainsaw, trueMeleeNoSpeed },
                { ProjectileID.VortexDrill, trueMeleeNoSpeed },
                { ProjectileID.WoodenArrowFriendly, pointBlank },
                { ProjectileID.WoodYoyo, Do(LocalIFrames(10)) },
                { ProjectileID.Yelets, Do(LocalIFrames(10)) },
                { ProjectileID.YellowCounterweight, counterweightTweaks },
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
