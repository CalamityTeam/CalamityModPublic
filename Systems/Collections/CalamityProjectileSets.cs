using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Projectiles.Typeless;
using ReLogic.Reflection;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Systems.Collections
{
    [ReinitializeDuringResizeArrays]
    public static class CalamityProjectileSets
    {
        private static SetFactory Factory = ProjectileID.Sets.Factory;

        /// <summary>
        /// If <see langword="true"/> for a projectile type, then that minion will completely ignore Calamity's summon damage penalty mechanic with no exceptions.<br/>
        /// Unused by Calamity itself, and is only used for external mods to add to through mod calls.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] MinionWhichIgnoresSummonerNerf = Factory.CreateNamedSet("MinionWhichIgnoresSummonerNerf")
            .Description("Makes this minion ignore the summon damage penalty.")
            .RegisterBoolSet();

        /// <summary>
        /// If <see langword="true"/> for a projectile type, then that projectile is spawned by a post-Plantera Dungeon enemy.<br/>
        /// This increases the projectile's damage by a flat 30 if Moon Lord has been defeated.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] IsBuffedDungeonProjectile = Factory.CreateNamedSet("IsBuffedDungeonProjectile")
            .Description("Makes this Dungeon projectile get a damage buff after defeating Moon Lord.")
            .RegisterBoolSet(ProjectileID.PaladinsHammerHostile, ProjectileID.ShadowBeamHostile, ProjectileID.InfernoHostileBolt, ProjectileID.InfernoHostileBlast,
                ProjectileID.LostSoulHostile, ProjectileID.SniperBullet, ProjectileID.RocketSkeleton, ProjectileID.BulletDeadeye, ProjectileID.Shadowflames);

        /// <summary>
        /// If <see langword="true"/> for a projectile type, then that projectile is spawned by a Solar Eclipse, Pumpkin Moon, or Frost Moon enemy.<br/>
        /// This increases the projectile's damage by a flat 15 during those events if Devourer of Gods has been defeated.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] IsBuffedEventProjectile = Factory.CreateNamedSet("IsBuffedEventProjectile")
            .Description("Makes this Pumpkin Moon, Frost Moon, or Solar Eclipse projectile get a damage buff after defeating Devourer of Gods.")
            .RegisterBoolSet(ProjectileID.FlamingWood, ProjectileID.GreekFire1, ProjectileID.GreekFire2, ProjectileID.GreekFire3, ProjectileID.FlamingScythe, ProjectileID.FlamingArrow,
                ProjectileID.PineNeedleHostile, ProjectileID.OrnamentHostile, ProjectileID.OrnamentHostileShrapnel, ProjectileID.FrostWave, ProjectileID.FrostShard, ProjectileID.Missile,
                ProjectileID.Present, ProjectileID.Spike, ProjectileID.BulletDeadeye, ProjectileID.EyeLaser, ProjectileID.Nail, ProjectileID.DrManFlyFlask);

        /// <summary>
        /// If <see langword="true"/> for a projectile type, then that projectile is considered a friendly bee.<br/>
        /// Used to allow the projectile to inflict Plague while wearing the Plaguebringer armor.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] IsFriendlyBeeProjectile = Factory.CreateNamedSet("IsFriendlyBeeProjectile")
            .Description("Labels this projectile as a friendly bee, used to make it inflict Plague with Plaguebringer armor.")
            .RegisterBoolSet(ProjectileID.GiantBee, ProjectileID.Bee, ProjectileID.Wasp, ProjectileID.Hornet,ProjectileID.HornetStinger, ProjectileType<PlaguenadeBee>(),
                ProjectileType<PlaguePrincess>(), ProjectileType<BabyPlaguebringer>(), ProjectileType<PlagueBeeSmall>(), ProjectileType<BetterHornetStinger>(), ProjectileType<BasicPlagueBee>());

        /// <summary>
        /// If <see langword="true"/> for a projectile type, then that projectile is a bomb or other explosive which is not a weapon.<br/>
        /// Used to provide early-game worm bosses a resistance to their explosive damage.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] ResistedExplosiveProjectile = Factory.CreateNamedSet("ResistedExplosiveProjectile")
            .Description("Makes this grenade/bomb/dynamite projectile be resisted by earlygame worm bosses.")
            .RegisterBoolSet(ProjectileID.Grenade, ProjectileID.StickyGrenade, ProjectileID.BouncyGrenade, ProjectileID.Bomb, ProjectileID.StickyBomb, ProjectileID.BouncyBomb,
                ProjectileID.Dynamite, ProjectileID.StickyDynamite, ProjectileID.BouncyDynamite, ProjectileID.Explosives, ProjectileID.ExplosiveBunny, ProjectileID.PartyGirlGrenade,
                ProjectileID.BombFish, ProjectileID.ScarabBomb, ProjectileID.TNTBarrel, ProjectileType<AeroExplosive>());

        /// <summary>
        /// If <see langword="true"/> for a projectile type, then that projectile will never be reflected by armor or accessory effects.<br/>
        /// Set this for persistent projectiles such as deathrays to avoid major screwing of their behavior.<br/>
        /// Only needs to be set for hostile projectiles, as these effects already have a check to ensure they never trigger in PvP.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] ShouldNotBeReflected = Factory.CreateNamedSet("ShouldNotBeReflected")
            .Description("Prevents this projectile from being reflected by armor or accessory effects.")
            .RegisterBoolSet(ProjectileID.SaucerDeathray, ProjectileID.PhantasmalDeathray, ProjectileType<BrimstoneMonster>(), ProjectileType<InfernadoRevenge>(),
                ProjectileType<OverlyDramaticDukeSummoner>(), ProjectileType<ProvidenceHolyRay>(), ProjectileType<OldDukeVortex>(), ProjectileType<BrimstoneRay>(),
                ProjectileType<AresDeathBeamStart>(), ProjectileType<AresGaussNukeProjectileBoom>(), ProjectileType<AresLaserBeamStart>(), ProjectileType<ArtemisSpinLaserbeam>(),
                ProjectileType<BirbAura>(), ProjectileType<ThanatosBeamStart>());

        /// <summary>
        /// If <see langword="true"/> for a projectile type, then that projectile will be blacklisted from receiving the homing effect of Grape Beer.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] DoesNotGetHomingWithGrapeBeer = Factory.CreateNamedSet("DoesNotGetHomingWithGrapeBeer")
            .Description("Prevents this projectile from receiving homing from Grape Beer.")
            .RegisterBoolSet(ProjectileType<NukeOfBliss>(), ProjectileType<PrismaticEnergyBlast>(), ProjectileType<PrismEnergyBullet>(), ProjectileType<PrismMine>(),
                ProjectileType<ScorchedEarthRocket>(), ProjectileType<UltimaRay>(), ProjectileType<SproutingArrowMain>(), ProjectileType<MegalodonShot>(), ProjectileType<BlahajBlast>(), 
                ProjectileType<VoidBlast>(), ProjectileType<AbyssalFire>());

        /// <summary>
        /// If <see langword="true"/> for a projectile type, then that projectile will be unable to trigger Daawnlight Spirit Origin's bullseyes.<br/>
        /// Defaults to <see langword="false"/>.
        /// </summary>
        public static bool[] DaawnlightBlacklist = Factory.CreateNamedSet("DaawnlightBlacklist")
            .Description("Prevents this projectile from triggering Daawnlight Spirit Origin's bullseyes.")
            .RegisterBoolSet(ProjectileType<SeasSearingSpout>(), ProjectileType<WaterSpout>(), ProjectileType<TyphoonBubble>());

        /// <summary>
        /// Determines what other projectiles this projectile will share ID-static immunity frames with. Defaults to -1, which means that it does not share immunity frames.<br/>
        /// Each "set" of projectile which shares immunity frames is registered to point to the same ID reference, usually the first projectile in the set. If a projectile with this reference hits an NPC, all other projectiles with that reference also have their ID-static immunity cooldown set for that NPC.
        /// </summary>
        public static int[] SharedIDStaticIFrames = Factory.CreateNamedSet("SharedIDStaticIFrames")
            .Description("Determines what other projectiles this projectile will share ID-static immunity frames with. Defaults to -1, which means that it does not share immunity frames.")
            .RegisterIntSet(-1,
            // Vanilla bees
            ProjectileID.Bee, ProjectileID.Bee,
            ProjectileID.GiantBee, ProjectileID.Bee,
            // Vilethorn
            ProjectileID.VilethornBase, ProjectileID.VilethornBase,
            ProjectileID.VilethornTip, ProjectileID.VilethornBase,
            // Crystal Vile Shard
            ProjectileID.CrystalVileShardHead, ProjectileID.CrystalVileShardHead,
            ProjectileID.CrystalVileShardShaft, ProjectileID.CrystalVileShardHead,
            // Nettle Burst
            ProjectileID.NettleBurstRight, ProjectileID.NettleBurstRight,
            ProjectileID.NettleBurstLeft, ProjectileID.NettleBurstRight,
            ProjectileID.NettleBurstEnd, ProjectileID.NettleBurstRight,
            // Magical Harp notes
            ProjectileID.QuarterNote, ProjectileID.QuarterNote,
            ProjectileID.EighthNote, ProjectileID.QuarterNote,
            ProjectileID.TiedEighthNote, ProjectileID.QuarterNote,
            // North Pole
            ProjectileID.NorthPoleWeapon, ProjectileID.NorthPoleWeapon,
            ProjectileID.NorthPoleSpear, ProjectileID.NorthPoleWeapon,
            // Spore gas clouds
            ProjectileID.SporeTrap, ProjectileID.SporeTrap,
            ProjectileID.SporeTrap2, ProjectileID.SporeTrap,
            ProjectileID.SporeGas, ProjectileID.SporeTrap,
            ProjectileID.SporeGas2, ProjectileID.SporeTrap,
            ProjectileID.SporeGas3, ProjectileID.SporeTrap,
            // Astral Staff
            ProjectileType<AstralCrystal>(), ProjectileType<AstralCrystal>(),
            ProjectileType<AstralCrystalInvisibleExplosion>(), ProjectileType<AstralCrystal>(),
            // Keelhaul
            ProjectileType<KeelhaulGeyserBottom>(), ProjectileType<KeelhaulGeyserBottom>(),
            ProjectileType<KeelhaulGeyserTop>(), ProjectileType<KeelhaulGeyserBottom>(),
            // Toxic clouds
            ProjectileID.ToxicCloud, ProjectileID.ToxicCloud,
            ProjectileID.ToxicCloud2, ProjectileID.ToxicCloud,
            ProjectileID.ToxicCloud3, ProjectileID.ToxicCloud
            );
    }
}
