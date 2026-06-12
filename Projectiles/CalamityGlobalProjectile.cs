using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CalamityMod.Buffs;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.Dusts;
using CalamityMod.EntitySources;
using CalamityMod.Enums;
using CalamityMod.Events;
using CalamityMod.ExtraTextures;
using CalamityMod.Graphics;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Ammo;
using CalamityMod.Items.Armor.Daedalus;
using CalamityMod.Items.Armor.Reaver;
using CalamityMod.Items.Fishing;
using CalamityMod.Items.Fishing.FishingRods;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.Items.SummonItems;
using CalamityMod.Items.VanillaArmorChanges;
using CalamityMod.NPCs;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.PlagueEnemies;
using CalamityMod.NPCs.VanillaNPCAIOverrides.Bosses;
using CalamityMod.Packets.Entities;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Healing;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Projectiles.VanillaProjectileOverrides;
using CalamityMod.Systems;
using CalamityMod.Systems.Collections;
using CalamityMod.Systems.Mechanic;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Tiles.Astral;
using CalamityMod.Tiles.AstralDesert;
using CalamityMod.Tiles.AstralSnow;
using CalamityMod.Tiles.Crags.Tree;
using CalamityMod.Tiles.FurnitureAuric;
using CalamityMod.Tiles.Ores;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;
using Terraria.WorldBuilding;
using static Terraria.ModLoader.ModContent;
using NanotechProjectile = CalamityMod.Projectiles.Typeless.Nanotech;


namespace CalamityMod.Projectiles
{
    public partial class CalamityGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// If true, this projectile was spawned from a dash hit.<br/>
        /// Solely used to prevent the projectile from inflicting Vulnerability Hex when using the Aflame enchantment.
        /// </summary>
        public bool CreatedByPlayerDash = false;
        /// <summary>
        /// If a projectile is spawned from a hostile NPC, this variable is set to the index of the NPC.<br/>
        /// Used for identifying which NPC to apply certain retaliatory effects on from projectiles hitting the player.
        /// </summary>
        public int ParentNPCIndex = -1;

        /// <summary> Constant variable used as a speed cap for boss laser projectiles with 2 extra updates. </summary>
        public const float AcceleratingBossLaserVelocityCap = 8f;

        /// <summary>
        /// Used for executing frame one hacks. Things set during this include:
        /// <br/>* Reducing projectile damage for Hardmode enemies and in Master Mode.
        /// <br/>* Adding armor penetration to rogue projectiles spawned while wearing Filthy Glove or its upgrades.
        /// <br/>* Increasing the damage of projectiles from the post-Moon Lord Dungeon, and from the post-DoG Pumpkin Moon, Frost Moon, and Solar Eclipse.
        /// </summary>
        private bool frameOneHacksExecuted = false;

        /// <summary>
        /// Enables "supercrits". When crit is over 100%, projectiles with this bool enabled can "supercrit".<br/>
        /// For every 100% critical strike chance over 100%, "supercrit" projectiles do a guaranteed +100% damage.<br/>
        /// They then take the remainder and roll against that for a final +100% (like normal crits).<br/>
        /// For example if you have 716% critical strike chance, you are guaranteed +700% damage and then have a 16% chance for +800% damage instead.<br/>
        /// An example of this is Soma Prime, but any bullet fired from that gun can supercrit when this bool is activated.<br/>
        /// Set this to -1 if you want the projectile to supercrit forever, and to any positive value to make it supercrit only x times.
        /// </summary>
        public int supercritHits = 0;

        /// <summary>
        /// Bonus damage on critical strikes expressed as a ratio. <br />
        /// 0f = 200% crit damage, vanilla standard.<br />
        /// 1f = 300% crit damage.<br /><br />
        /// This applies independently of supercrits and will stack additively with them.
        /// </summary>
        public float bonusCritDamage = 0f;

        // 08MAR2025: This remains so that "delayed forced crits" can still be set. This is used for Marksman Ricoshots.
        /// <summary> Set to true to force a projectile to critically strike. </summary>
        public bool forcedCrit = false;

        /// <summary> The total bonus damage (as a ratio of the projectile's own damage) applied to this projectile as a result of a ricoshot combo. </summary>
        public float totalRicoshotDamageBonus = 0f;

        /// <summary> If true, this projectile can apply the infinitely-stacking Shred debuff iconic to Soma Prime. </summary>
        public bool appliesSomaShred = false;

        /// <summary>
        /// If true, this projectile is able to spawn lightning while using Arc Flash Ring.<br/>
        /// This is set to false when lightning is procced, and is reset to true when the cooldown ends.
        /// </summary>
        public bool showArcFlash = true;
        /// <summary> Cooldown variable for Arc Flash Ring's lightning. Primarily used for lingering projectiles and minions. </summary>
        public int arcFlashCooldown = 0;
        /// <summary>
        /// The location of this projectile in an arena box, used for maintaining this when the arena box moves.
        /// </summary>
        public Vector2 arenaBoxPosition = Vector2.Zero;
        /// <summary>
        /// Which Box this projectile is attached to
        /// </summary>
        public ArenaWallSystem.Box arenaBox = null;
        /// <summary>
        /// If true, adds a brimstone trail to the projectile, and makes it inflict Brimstone Flames.<br/>
        /// Used by Animosity.
        /// </summary>
        public bool brimstoneBullets = false;
        /// <summary>
        /// If true, adds a fire trail to the projectile, and makes it inflict Hellfire.<br/>
        /// Used by Thermocline Blaster.
        /// </summary>
        public bool fireBullet = false;
        /// <summary>
        /// If true, allows projectiles to track damage scaling.<br/>
        /// Used by Megalodon and Voidragon.
        /// </summary>
        public bool sharkBullets = false;
        /// <summary>
        /// If true, adds an ice trail to the projectile, and makes it inflict Frostbite.<br/>
        /// Used by Thermocline Blaster.
        /// </summary>
        public bool iceBullet = false;
        /// <summary>
        /// If true, adds an electric trail to the projectile, and makes it inflict Electrified and create an explosion on hit.<br/>
        /// Used by Arietes 41.
        /// </summary>
        public bool shockBullet = false;
        /// <summary>
        /// If true, adds a white trail to the projectile, and allows it to lifesteal.<br/>
        /// Used by Arietes 41.
        /// </summary>
        public bool lifeBullet = false;
        /// <summary>
        /// If true, this projectile creates visual impact sparks upon hitting enemies.<br/>
        /// Used by Deepcore GK2.
        /// </summary>
        public bool deepcoreBullet = false;
        /// <summary>
        /// If true, adds a blue and pearly trail to the projectile, and makes it create an explosion on hit.<br/>
        /// Used by Pearl God.
        /// </summary>
        public bool pearlBullet1 = false;
        /// <summary>
        /// If true, adds a pink and pearly trail to the projectile, and makes it create an explosion on hit.<br/>
        /// Used by Pearl God.
        /// </summary>
        public bool pearlBullet2 = false;
        /// <summary>
        /// If true, adds a khaki and pearly trail to the projectile, and makes it create an explosion on hit.<br/>
        /// Used by Pearl God.
        /// </summary>
        public bool pearlBullet3 = false;
        /// <summary>
        /// If true, adds a blue, pink, and khaki helix trail to the projectile, and allows it to lifesteal.<br/>
        /// Used by Pearl God.
        /// </summary>
        public bool betterLifeBullet1 = false;
        /// <summary> <inheritdoc cref="betterLifeBullet1"/> </summary>
        public bool betterLifeBullet2 = false;
        /// <summary>
        /// If set to a value greater than 0, causes this projectile to gain homing with a range equal to the value in pixels.<br/>
        /// Used by Arterial Assault, Deific Amulet's stars, and certain sand projectiles from the Sand Gun.
        /// </summary>
        public float conditionalHomingRange = 0f;
        public float conditionalHomingVelocity = 12f;
        public float conditionalHomingInertia = 20f;

        /// <summary>
        /// Whether or not this proj was spawned with grape beer on
        /// this does NOT mean it has grape beer homing!
        /// </summary>
        public bool grapeBeer = false;

        /// <summary>
        /// How strong grape beer homing is on this. It speeds up the longer the target is targeted.
        /// </summary>
        public float grapeBeerHomingPower = 0.01f;
        /// <summary>
        /// ID of last target from grapebeer
        /// </summary>
        public int grapeBeerHomingTarget = -1;

        /// <summary>
        /// Variable used for storing the actual amount of extra updates a projectile has.<br/>
        /// This is NOT set automatically, and must be set whenever it is needed.
        /// </summary>
        public int defExtraUpdates = -1;

        /// <summary>
        /// How many times this projectile has pierced an enemy which applies pierce resist.<br/>
        /// Used for calculating pierce resist damage reduction.
        /// </summary>
        public int timesPierced = 0;

        // Empress of Light variables
        private const float EmpressRainbowStreakSpreadOutCutoff = 140f;
        private const int EmpressLastingRainbowTotalDuration = 660;
        private const int EmpressLastingRainbowTimeBeforeDealingDamage = 60;

        // Duke Fishron variables
        private const int FishronSharknadoTotalDuration = 540;
        private const int FishronCthulhunadoTotalDuration = 840;
        private const int FishronTornadoTimeBeforeDealingDamage = 60;

        /// <summary> Timer for how long a projectile's damage is reduced by the value in <see cref="flatDR"/>. </summary>
        public int flatDRTimer = 0;
        /// <summary> A temporary flat amount subtracted from the projectile's damage when hitting the player. Resets to 0 if <see cref="flatDRTimer"/> drops to 0. </summary>
        public int flatDR = 0;

        /// <summary> Timer for how long a projectile's damage is reduced by the value in <see cref="multiplicativeDR"/>. </summary>
        public int multiplicativeDRTimer = 0;
        /// <summary> A temporary multiplicative amount of damage reduction when hitting the player. Resets to 0 if <see cref="multiplicativeDRTimer"/> drops to 0. </summary>
        public float multiplicativeDR = 0;

        /// <summary> If true, this projectile is a hook which has spawned a flower on it from Bloom Stone. Used to prevent spawning multiple flowers. </summary>
        public bool hookCanSpawnFlower = false;

        /// <summary> If true, allows hostile projectiles to deal defense damage to the player. Used mostly for hard-hitting bosses. </summary>
        public bool DealsDefenseDamage = false;

        /// <summary>
        /// Determines how this projectile's damage is affected by the Old Fashioned buff. This does not have an effect unless the player has the buff consumed.<br/>
        /// If true, its damage is buffed. If false, its damage is nerfed. If null, its damage is unchanged. Defaults to null.
        /// </summary>
        public bool? buffedByOldFashioned;
        /// <summary> If true, this projectile has been buffed by Quiver of Nihility's void fields. Used to ensure the buff is only applied once. </summary>
        public bool nihilicArrow = false;

        // Rogue Stuff
        /// <summary> If true, this projectile is from a rogue stealth strike. The primary variable responsible for triggering stealth strike effects. </summary>
        public bool stealthStrike = false;
        /// <summary>
        /// If true, this projectile is spawned from a stealth strike, but is not a primary projectile. Used to avoid proccing things like Raiders Talisman.
        /// </summary>
        public bool stealthStrikeSubProjectile = false;
        /// <summary>
        /// Number of times this stealth strike projectile has pierced an enemy.<br/>
        /// Used to limit the number of times a projectile can proc certain effects from rogue accessories.
        /// </summary>
        public int stealthStrikeHitCount = 0;
        /// <summary> Variable to ensure that Ethereal Extorter's soul projectiles only spawn on projectiles spawning directly from item use. </summary>
        public bool extorterBoost = false;
        /// <summary>
        /// If true, this projectile is a Venerated Locket clone projectile.<br/>
        /// Used to prevent certain rogue weapon and accessory effects.
        /// </summary>
        public bool LocketClone = false;
        /// <summary>
        /// If true, this projectile cannot proc projectile effects from rogue accessories.<br/>
        /// Set this variable for projectiles that are prone to spawning an excessive amount of projectiles.
        /// </summary>
        public bool CannotProc = false;
        /// <summary> Tracks whether this projectile has already triggered Scuttler's Jewel's projectile effect. </summary>
        public bool JewelSpikeSpawned = false;

        /// <summary> Cooldown variable used to prevent projectiles from spawning orbs while in The Transformer's aura. </summary>
        public int TransformerTimer = 0;

        /// <summary>
        /// There are several NPCs in Calamity which do not take damage from minions in certain circumstances.<br/>
        /// If true, this variable allows a projectile that deals summon damage to bypass this mechanic.
        /// </summary>
        public bool overridesMinionDamagePrevention = false;

        // Enchantment variables.
        /// <summary>
        /// Variable used as a timer for minions with the Hellbound enchantment.<br/>
        /// When spawned, this value is set to <see cref="ExplosiveEnchantTime"/> and gets decremented every frame. When it reaches 0, the minion explodes and despawns.<br/>
        /// Also used for spawning smaller explosions and for scaling damage as the timer decreases.
        /// </summary>
        public int ExplosiveEnchantCountdown = 0;
        /// <summary> Constant variable which stores the duration of minions with the Hellbound enchantment, in frames. </summary>
        public const int ExplosiveEnchantTime = 2400;

        /// <summary>
        /// Custom update priority.<br/>
        /// Calamity sorts projectiles by their update priority to fix otherwise absurdly difficult to resolve visual bugs on certain weapons.<br/>
        /// Examples include Void Eater Marionette segments detaching or Rancor's laser beam being offset from the magic circle.
        /// </summary>
        public float UpdatePriority = 0f;

        public int BloodstoneOrbValue = 0;

        public int HomingTarget = -1;

        /// <summary>
        /// Flag used during Rev+ Brain of Cthulhu to denote projectiles that were spawned prior to its Illusion Trick attack starting.
        /// </summary>
        public bool IgnoreBoCIllusions = false;

        #region On Spawn
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            // TODO -- it would be nice to move frame one hacks here, but this runs in the middle of NewProjectile
            // which is way too early, the projectile's own initialization isn't even done yet

            CreatedByPlayerDash = source is ProjectileSource_PlayerDashHit;

            IEntitySource sourceItem = source as EntitySource_ItemUse_WithAmmo;
            if (sourceItem != null)
                extorterBoost = true;

            // Whenever the player has Daawnlight Spirit Origin, any ranged projectile will have the capacity to infintely supercrit.
            if (Main.player[projectile.owner].Calamity().spiritOrigin && projectile.CountsAsClass<RangedDamageClass>())
            {
                projectile.CritChance += Main.player[projectile.owner].Calamity().spiritOriginCritBoost;
                projectile.Calamity().supercritHits = -1;
            }

            void ApplyGrapeBeer()
            {
                grapeBeer = true;
                var ivDripPlayer = Main.player[projectile.owner].GetModPlayer<IVDripPlayer>();
                conditionalHomingRange = ivDripPlayer.HasAlcohol(AlcoholType.GrapeBeer) ? 800 : 300;
                if (projectile.timeLeft > 300 * projectile.MaxUpdates)
                    projectile.timeLeft = 300 * projectile.MaxUpdates;
                //Calamity adds a hybrid iframe system when both local and static are set to true, so this works fine for both global and static projectiles.
                projectile.usesLocalNPCImmunity = true;
                projectile.localNPCHitCooldown = -1;
            }
            if (source is EntitySource_ItemUse_WithAmmo { Item: Item item })
            {
                if (source is EntitySource_Parent { Entity: Player player })
                {
                    if (player.Calamity().grapeBeer && (item.useAmmo == AmmoID.Bullet || item.useAmmo == AmmoID.Arrow || item.useAmmo == AmmoID.Dart || item.useAmmo == AmmoID.Rocket))
                    {
                        if (player.heldProj != projectile.whoAmI && projectile.aiStyle != ProjAIStyleID.HeldProjectile && projectile.damage > 0 && !CalamityProjectileSets.DoesNotGetHomingWithGrapeBeer[projectile.type])
                            ApplyGrapeBeer();
                        else
                            grapeBeer = true;
                    }
                }
            }

            if (source is EntitySource_Parent { Entity: NPC npc })
            {
                if (!npc.friendly)
                    ParentNPCIndex = npc.whoAmI;
            }
            //
            // SPECIFIC PROJECTILE BALANCE CHANGES (and Grape Beer homing)
            //
            else if (source is EntitySource_Parent { Entity: Projectile parent })
            {
                //Grape Beer homing
                if (parent.Calamity().grapeBeer)
                {
                    if (Main.player[projectile.owner].heldProj != projectile.whoAmI && projectile.aiStyle != ProjAIStyleID.HeldProjectile && projectile.damage > 0 && !CalamityProjectileSets.DoesNotGetHomingWithGrapeBeer[projectile.type])
                        ApplyGrapeBeer();
                    else
                        grapeBeer = true;
                }

                // Apply some parent information to children
                if (parent.Calamity().ParentNPCIndex != -1)
                    ParentNPCIndex = parent.Calamity().ParentNPCIndex;
                if (parent.Calamity().IgnoreBoCIllusions)
                    IgnoreBoCIllusions = true;

                // Nerf Crystal bullet shard damage by 45%
                // Vanilla crystal shards deal 50% of the bullet's damage which is absurd, this nerfs them to 27.5%
                if (parent.type == ProjectileID.CrystalBullet && projectile.type == ProjectileID.CrystalShard)
                    projectile.damage = (int)(projectile.damage * 0.55f);

                // Nerf Mushroom Spear mushroom damage by 50%
                if (parent.type == ProjectileID.MushroomSpear && projectile.type == ProjectileID.Mushroom)
                    projectile.damage /= 2;

                // Nerf Luminite Arrow trail damage by 50%
                if (parent.type == ProjectileID.MoonlordArrow && projectile.type == ProjectileID.MoonlordArrowTrail)
                    projectile.damage /= 2;

                // Nerf Cursed Dart flame damage by 50%
                if (parent.type == ProjectileID.CursedDart && projectile.type == ProjectileID.CursedDartFlame)
                    projectile.damage /= 2;

                // Nerf Chlorophyte armor's set bonus crystal damage by 30%
                if (parent.type == ProjectileID.CrystalLeaf && projectile.type == ProjectileID.CrystalLeafShot)
                    projectile.damage = (int)(projectile.damage * 0.7f);
            }

            if (source is EntitySource_OnHit e)
            {
                // Nerf Spectre armor's set bonus soul damage by 50%
                if (e.Context == "SetBonus_GhostHurt")
                    projectile.damage /= 2;
            }

            if (source is EntitySource_Parent { Entity: Projectile proj })
            {
                if (proj.Calamity().stealthStrike || proj.Calamity().stealthStrikeSubProjectile)
                    stealthStrikeSubProjectile = true;
            }

        }
        public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(ParentNPCIndex);
        }
        public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
        {
            ParentNPCIndex = binaryReader.ReadInt32();
        }
        #endregion On Spawn

        #region Set Defaults
        public override void SetStaticDefaults()
        {
            // Making Rocket Launcher, Grenade Launcher, Proximity Mine Launcher, and Cluster Rocket fragments not damage the player.
            for (int type = 0; type < ProjectileID.Count; type++)
            {
                if ((type >= 133 && type <= 144) || (type >= 776 && type <= 801))
                    ProjectileID.Sets.RocketsSkipDamageForPlayers[type] = true;
            }
        }

        public override void SetDefaults(Projectile projectile)
        {
            // This code is needed to ensure that the code for preventing damage multipliers from triggering more than once works
            if (projectile.type == ProjectileID.ZapinatorLaser)
                projectile.originalDamage = projectile.damage;

            // Make Piranha Gun have dynamic stat tweaking
            if (projectile.type == ProjectileID.MechanicalPiranha)
                projectile.ContinuouslyUpdateDamageStats = true;

            // Apply Calamity Global Projectile Tweaks.
            SetDefaults_ApplyTweaks(projectile);
        }
        #endregion

        #region Pre AI
        public override bool PreAI(Projectile projectile)
        {
            //Light Pets will add light to the abyss darkness.
            //This uses local player on purpose, as the abyss darkness system is entirely client side.
            if (ProjectileID.Sets.LightPet[projectile.type] && Main.LocalPlayer.Calamity().ZoneAbyss)
                EnhancedDarknessSystem.lights.Add(new() { center = projectile.Center, scale = 1 });

            // 24DEC2025: Ozzatron: victide bobber culled with SSO 
            if (projectile.bobber /*&& projectile.type != ProjectileType<VictideBobber>()*/ && RunFishingMinigames(projectile) && !Main.LocalPlayer.dead)
                return false;

            //Reset the Homing Target immediately before AI can re-set it on applicable projectiles
            HomingTarget = -1;
            #region Vanilla Summons AI Changes

            //
            // MINION AI CHANGES:
            //

            // Hornet Staff's minion changes.
            if (projectile.type == ProjectileID.Hornet)
                return HornetMinionAI.DoHornetMinionAI(projectile);

            // Imp Staff's minion changes.
            if (projectile.type == ProjectileID.FlyingImp)
                return ImpMinionAI.DoImpMinionAI(projectile);

            // Raven Staff's minion changes.
            if (projectile.type == ProjectileID.Raven)
                return RavenMinionAI.DoRavenMinionAI(projectile);

            //
            // SENTRY AI CHANGES:
            //

            // Deerclop's sentry drop projectile changes.
            if (projectile.type == ProjectileID.HoundiusShootiusFireball)
                return HoundiusShootiusFireballAI.DoHoundiusShootiusFireballAI(projectile);

            #endregion

            // Reduce secondary yoyo damage if the player has Yoyo Glove
            // Brief behavior documentation of yoyo AI: ai[0, 1] are the x, y co-ords and localAI[0] is the airtime in frames
            // All secondary yoyos are spawned with ai[0] of 1 which tells then tell its AI to do secondary yoyo AI
            if (Main.player[projectile.owner].yoyoGlove && projectile.aiStyle == ProjAIStyleID.Yoyo)
            {
                // Store damage on first frame
                if (projectile.localAI[2] == 0f)
                    projectile.localAI[2] = projectile.damage;

                var MainYoyo = Main.projectile.First(x => x.active && x.type == projectile.type && x.owner == projectile.owner).whoAmI;

                // Halve damage if not the main yoyo
                if (projectile.whoAmI != MainYoyo)
                    projectile.damage = (int)(projectile.localAI[2] * 0.5f);
                else
                    projectile.damage = (int)projectile.localAI[2];
            }

            if (projectile.minion && ExplosiveEnchantCountdown > 0)
            {
                ExplosiveEnchantCountdown--;

                // Make fizzle sounds and fire dust to indicate the impending explosion.
                if (ExplosiveEnchantCountdown <= 300)
                {
                    if (Main.rand.NextBool(24))
                        SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, projectile.Center);

                    Dust fire = Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Circular(projectile.width, projectile.height) * 0.42f, DustID.RainbowMk2);
                    fire.color = Color.Lerp(Color.Orange, Color.Red, Main.rand.NextFloat(0.45f, 1f));
                    fire.scale = Main.rand.NextFloat(1.4f, 1.65f);
                    fire.fadeIn = 0.5f;
                    fire.noGravity = true;
                }

                if (ExplosiveEnchantCountdown % 40 == 39 && Main.rand.NextBool(12))
                {
                    int damage = (int)Main.player[projectile.owner].GetTotalDamage<SummonDamageClass>().ApplyTo(2000);
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Vector2.Zero, ProjectileType<SummonBrimstoneExplosionSmall>(), damage, 0f, projectile.owner);
                }

                if (ExplosiveEnchantCountdown <= 0)
                {
                    SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion, projectile.Center);
                    if (Main.myPlayer == projectile.owner)
                    {
                        if (projectile.minionSlots > 0f)
                        {
                            int damage = (int)Main.player[projectile.owner].GetTotalDamage<SummonDamageClass>().ApplyTo(6000);
                            Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Vector2.Zero, ProjectileType<SummonBrimstoneExplosion>(), damage, 0f, projectile.owner);
                        }
                        projectile.Kill();
                    }
                }
            }

            if (projectile.type == ProjectileID.Skull)
            {
                bool fromRevSkeletron = projectile.ai[0] < 0f;
                bool revSkeletronHomingSkull = projectile.ai[0] == -1f;
                bool revSkeletronAcceleratingSkull = projectile.ai[0] == -2f;
                bool revSkeletronPrimeHomingSkull = projectile.ai[0] == -3f;

                if (revSkeletronHomingSkull || revSkeletronPrimeHomingSkull)
                    projectile.alpha = 0;

                if (projectile.alpha > 0)
                    projectile.alpha -= 75;

                if (projectile.alpha < 0)
                    projectile.alpha = 0;

                projectile.frame++;
                if (projectile.frame > 2)
                    projectile.frame = 0;

                // Accelerate if fired in a spread from Skeletron in Rev+
                if (revSkeletronAcceleratingSkull)
                {
                    float maxVelocity = CalamityWorld.death ? 18f : 15f;
                    if (projectile.velocity.Length() < maxVelocity)
                    {
                        float acceleration = 1.015f;
                        projectile.velocity *= acceleration;
                        if (projectile.velocity.Length() > maxVelocity)
                        {
                            projectile.velocity.Normalize();
                            projectile.velocity *= maxVelocity;
                        }
                    }
                }

                if (!revSkeletronHomingSkull && !revSkeletronPrimeHomingSkull)
                {
                    int numDust = revSkeletronAcceleratingSkull ? 1 : 2;
                    int dustType = revSkeletronAcceleratingSkull ? 91 : 6;
                    float dustScale = revSkeletronAcceleratingSkull ? 1f : 2f;
                    float dustVelocityOffset = revSkeletronAcceleratingSkull ? 1f : 2f;
                    for (int i = 0; i < numDust; i++)
                    {
                        Dust flame = Dust.NewDustDirect(new Vector2(projectile.position.X + 4f, projectile.position.Y + 4f), projectile.width - 8, projectile.height - 8, dustType, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default(Color), dustScale);
                        flame.position -= projectile.velocity * dustVelocityOffset;
                        flame.noGravity = true;
                        flame.velocity *= 0.3f;
                    }
                }
                else
                {
                    for (int num173 = 0; num173 < 2; num173++)
                    {
                        int num174 = Dust.NewDust(new Vector2(projectile.position.X + 4f, projectile.position.Y + 4f), projectile.width - 8, projectile.height - 8, DustID.Blood, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default(Color), 1.5f);
                        Main.dust[num174].position -= projectile.velocity;
                        Main.dust[num174].noGravity = true;
                        Main.dust[num174].velocity.X *= 0.3f;
                        Main.dust[num174].velocity.Y *= 0.3f;
                    }

                    int num133 = 0;
                    num133 = Player.FindClosest(projectile.Center, 1, 1);
                    projectile.ai[1] += 1f;
                    float homingStartTime = revSkeletronPrimeHomingSkull ? 10f : 30f;
                    float homingEndTime = CalamityWorld.death ? 105f : 90f;
                    if (revSkeletronPrimeHomingSkull)
                        homingEndTime += 60f;

                    // Stop homing when within a certain distance of the target
                    if (Vector2.Distance(projectile.Center, Main.player[num133].Center) < (revSkeletronPrimeHomingSkull ? ((CalamityWorld.death || BossRushEvent.BossRushActive) ? 192f : 120f) : 96f) && projectile.ai[1] < homingEndTime)
                        projectile.ai[1] = homingEndTime;

                    if (projectile.ai[1] < homingEndTime && projectile.ai[1] > homingStartTime)
                    {
                        float num134 = projectile.velocity.Length();
                        Vector2 vector24 = Main.player[num133].Center - projectile.Center;
                        vector24.Normalize();
                        vector24 *= num134;
                        float inertia = (CalamityWorld.death || BossRushEvent.BossRushActive) ? (revSkeletronPrimeHomingSkull ? 20f : 25f) : (revSkeletronPrimeHomingSkull ? 25f : 30f);
                        projectile.velocity = (projectile.velocity * (inertia - 1f) + vector24) / inertia;
                        projectile.velocity.Normalize();
                        projectile.velocity *= num134;
                    }

                    float maxVelocity = (CalamityWorld.death || BossRushEvent.BossRushActive) ? 18f : 15f;
                    float acceleration = (CalamityWorld.death || BossRushEvent.BossRushActive) ? 1.02f : 1.015f;
                    if (projectile.velocity.Length() < maxVelocity)
                        projectile.velocity *= acceleration;

                    if (projectile.localAI[0] == 0f)
                    {
                        projectile.localAI[0] = 1f;
                        SoundEngine.PlaySound(SoundID.Item8, projectile.Center);
                        for (int num135 = 0; num135 < 10; num135++)
                        {
                            int num136 = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Blood, projectile.velocity.X, projectile.velocity.Y, 0, default(Color), 2f);
                            Main.dust[num136].noGravity = true;
                            Main.dust[num136].velocity = projectile.Center - Main.dust[num136].position;
                            Main.dust[num136].velocity.Normalize();
                            Main.dust[num136].velocity *= -5f;
                            Main.dust[num136].velocity += projectile.velocity / 2f;
                        }
                    }
                }

                if (projectile.ai[0] == 0f)
                {
                    float num180 = (float)Math.Sqrt(projectile.velocity.X * projectile.velocity.X + projectile.velocity.Y * projectile.velocity.Y);
                    float num181 = projectile.localAI[0];

                    if (num181 == 0f)
                    {
                        projectile.localAI[0] = num180;
                        num181 = num180;
                    }

                    float num182 = projectile.position.X;
                    float num183 = projectile.position.Y;
                    float num184 = 300f;
                    bool flag4 = false;
                    int num185 = 0;

                    if (projectile.ai[1] == 0f)
                    {
                        for (int num186 = 0; num186 < Main.maxNPCs; num186++)
                        {
                            if (Main.npc[num186].CanBeChasedBy(this) && (projectile.ai[1] == 0f || projectile.ai[1] == (float)(num186 + 1)))
                            {
                                float num187 = Main.npc[num186].position.X + (float)(Main.npc[num186].width / 2);
                                float num188 = Main.npc[num186].position.Y + (float)(Main.npc[num186].height / 2);
                                float num189 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num187) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num188);
                                if (num189 < num184 && Collision.CanHit(new Vector2(projectile.position.X + (float)(projectile.width / 2), projectile.position.Y + (float)(projectile.height / 2)), 1, 1, Main.npc[num186].position, Main.npc[num186].width, Main.npc[num186].height))
                                {
                                    num184 = num189;
                                    num182 = num187;
                                    num183 = num188;
                                    flag4 = true;
                                    num185 = num186;
                                }
                            }
                        }

                        if (flag4)
                            projectile.ai[1] = num185 + 1;

                        flag4 = false;
                    }

                    if (projectile.ai[1] > 0f)
                    {
                        int num190 = (int)(projectile.ai[1] - 1f);
                        if (Main.npc[num190].active && Main.npc[num190].CanBeChasedBy(this, ignoreDontTakeDamage: true) && !Main.npc[num190].dontTakeDamage)
                        {
                            float num191 = Main.npc[num190].position.X + (float)(Main.npc[num190].width / 2);
                            float num192 = Main.npc[num190].position.Y + (float)(Main.npc[num190].height / 2);
                            if (Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num191) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num192) < 1000f)
                            {
                                flag4 = true;
                                num182 = Main.npc[num190].position.X + (float)(Main.npc[num190].width / 2);
                                num183 = Main.npc[num190].position.Y + (float)(Main.npc[num190].height / 2);
                            }
                        }
                        else
                            projectile.ai[1] = 0f;
                    }

                    if (!projectile.friendly)
                        flag4 = false;

                    if (flag4)
                    {
                        float num193 = num181;
                        Vector2 vector9 = new(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                        float num194 = num182 - vector9.X;
                        float num195 = num183 - vector9.Y;
                        float num196 = (float)Math.Sqrt(num194 * num194 + num195 * num195);
                        num196 = num193 / num196;
                        num194 *= num196;
                        num195 *= num196;
                        int num197 = 32;
                        projectile.velocity.X = (projectile.velocity.X * (float)(num197 - 1) + num194) / (float)num197;
                        projectile.velocity.Y = (projectile.velocity.Y * (float)(num197 - 1) + num195) / (float)num197;
                    }
                }

                projectile.spriteDirection = projectile.direction;

                if (revSkeletronAcceleratingSkull)
                    projectile.rotation += MathHelper.Pi / 90f * projectile.velocity.Length() * projectile.direction;
                else
                {
                    if (projectile.direction < 0)
                        projectile.rotation = (float)Math.Atan2(0f - projectile.velocity.Y, 0f - projectile.velocity.X);
                    else
                        projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X);
                }

                return false;
            }

            // This has 2 extra updates, dust was reduced because of this fact
            else if (projectile.type == ProjectileID.Shadowflames && projectile.ai[1] == 1f)
            {
                float spawnDustGateValue = 2f * projectile.MaxUpdates;
                if (projectile.localAI[0] == spawnDustGateValue)
                {
                    SoundEngine.PlaySound(SoundID.Item8, projectile.Center);
                    for (int i = 0; i < 20; i++)
                    {
                        int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.GemDiamond, 0f, 0f, 100);
                        Main.dust[dust].velocity *= 3f;
                        Main.dust[dust].velocity += projectile.velocity * 0.75f;
                        Main.dust[dust].scale *= 1.2f;
                        Main.dust[dust].noGravity = true;
                    }
                }

                projectile.localAI[0] += 1f;
                if (projectile.localAI[0] > spawnDustGateValue)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.GemDiamond, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100);
                        Main.dust[dust].velocity *= 0.6f;
                        Main.dust[dust].scale *= 1.4f;
                        Main.dust[dust].noGravity = true;
                    }
                }

                projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;

                return false;
            }
            // This has extra updates but increments lifespan through AI slots, so we need to offset that
            else if (projectile.type == ProjectileID.WeatherPainShot)
            {
                if (!projectile.FinalExtraUpdate())
                    projectile.ai[1]--;
            }

            else if (projectile.type == ProjectileID.BloodShot)
            {
                if (projectile.localAI[0] == 0f)
                {
                    SoundEngine.PlaySound(SoundID.Item17, projectile.Center);
                    projectile.localAI[0] = 1f;
                    for (int i = 0; i < 8; i++)
                    {
                        Dust blood1 = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Blood, projectile.velocity.X, projectile.velocity.Y, 100)];
                        blood1.velocity = (Main.rand.NextFloatDirection() * (float)Math.PI).ToRotationVector2() * 2f + projectile.velocity.SafeNormalize(Vector2.Zero) * 3f;
                        blood1.scale = 1.5f;
                        blood1.fadeIn = 1.7f;
                        blood1.position = projectile.Center;
                    }
                }

                projectile.alpha = 0;

                Dust blood2 = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Blood, projectile.velocity.X, projectile.velocity.Y, 100)];
                blood2.velocity = blood2.velocity / 4f + projectile.velocity / 2f;
                blood2.scale = 1.2f;
                blood2.position = projectile.Center + Main.rand.NextFloat() * projectile.velocity * 2f;

                projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;

                return false;
            }

            else if (projectile.type == ProjectileID.BloodNautilusShot)
            {
                if (projectile.localAI[0] == 0f)
                {
                    SoundEngine.PlaySound(SoundID.Item171, projectile.Center);
                    projectile.localAI[0] = 1f;
                    for (int i = 0; i < 8; i++)
                    {
                        Dust blood1 = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Blood, projectile.velocity.X, projectile.velocity.Y, 100);
                        blood1.velocity = (Main.rand.NextFloatDirection() * MathHelper.Pi).ToRotationVector2() * 2f + projectile.velocity.SafeNormalize(Vector2.Zero) * 2f;
                        blood1.scale = 0.9f;
                        blood1.fadeIn = 1.1f;
                        blood1.position = projectile.Center;
                    }
                }

                projectile.alpha = 0;

                Dust blood2 = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Blood, projectile.velocity.X, projectile.velocity.Y, 100);
                blood2.velocity = blood2.velocity / 4f + projectile.velocity / 2f;
                blood2.scale = 1.2f;
                blood2.position = projectile.Center + Main.rand.NextFloat() * projectile.velocity * 2f;

                int trailLength = projectile.oldPos.Length / 2;
                for (int j = 1; j < trailLength && !(projectile.oldPos[j] == Vector2.Zero); j++)
                {
                    if (Main.rand.NextBool(3))
                    {
                        Dust blood3 = Dust.NewDustDirect(projectile.oldPos[j], projectile.width, projectile.height, DustID.Blood, projectile.velocity.X, projectile.velocity.Y, 100);
                        blood3.velocity = blood3.velocity / 4f + projectile.velocity / 2f;
                        blood3.scale = 1.2f;
                        blood3.position = projectile.oldPos[j] + projectile.Size / 2f + Main.rand.NextFloat() * projectile.velocity * 2f;
                    }
                }

                projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;

                return false;
            }

            else if (projectile.type == ProjectileID.DD2OgreSmash || projectile.type == ProjectileID.QueenSlimeSmash)
            {
                float maxHitboxSize = 30f;
                if (projectile.type == ProjectileID.QueenSlimeSmash)
                    maxHitboxSize = 20f;

                projectile.ai[0] += 1f;
                if (projectile.ai[0] <= 0f)
                {
                    if (projectile.ai[0] == 0f)
                        SoundEngine.PlaySound(SoundID.Item167, projectile.Center);

                    return false;
                }
                if (projectile.ai[0] > 9f)
                {
                    projectile.Kill();
                    return false;
                }

                projectile.velocity = Vector2.Zero;
                projectile.position = projectile.Center;
                projectile.Size = new Vector2(16f, 8f) * MathHelper.Lerp(5f, maxHitboxSize, Utils.GetLerpValue(0f, 9f, projectile.ai[0]));
                projectile.Center = projectile.position;
                Point point = projectile.TopLeft.ToTileCoordinates();
                Point point2 = projectile.BottomRight.ToTileCoordinates();
                int num2 = point.X / 2 + point2.X / 2;
                int num3 = projectile.width / 2;
                if ((int)projectile.ai[0] % 3 != 0)
                    return false;

                int num4 = (int)projectile.ai[0] / 3;
                for (int i = point.X; i <= point2.X; i++)
                {
                    for (int j = point.Y; j <= point2.Y; j++)
                    {
                        if (Vector2.Distance(projectile.Center, new Vector2(i * 16, j * 16)) > (float)num3)
                            continue;

                        Tile tileSafely = Framing.GetTileSafely(i, j);
                        bool isPlatform = tileSafely.HasTile && (TileID.Sets.Platforms[tileSafely.TileType] || tileSafely.TileType == TileID.PlanterBox);
                        if (!isPlatform)
                        {
                            if (!tileSafely.HasTile || !Main.tileSolid[tileSafely.TileType] || Main.tileSolidTop[tileSafely.TileType] || Main.tileFrameImportant[tileSafely.TileType])
                                continue;

                            Tile tileSafely2 = Framing.GetTileSafely(i, j - 1);
                            if (tileSafely2.HasTile && Main.tileSolid[tileSafely2.TileType] && !Main.tileSolidTop[tileSafely2.TileType])
                                continue;
                        }

                        int num5 = WorldGen.KillTile_GetTileDustAmount(fail: true, tileSafely, i, j);
                        for (int k = 0; k < num5; k++)
                        {
                            Dust obj = Main.dust[WorldGen.KillTile_MakeTileDust(i, j, tileSafely)];
                            obj.velocity.Y -= 3f + (float)num4 * 1.5f;
                            obj.velocity.Y *= Main.rand.NextFloat();
                            obj.velocity.Y *= 0.75f;
                            obj.scale += (float)num4 * 0.03f;
                        }

                        if (num4 >= 2)
                        {
                            if (projectile.type == ProjectileID.QueenSlimeSmash)
                            {
                                Color newColor = NPC.AI_121_QueenSlime_GetDustColor();
                                newColor.A = 150;
                                for (int l = 0; l < num5 - 1; l++)
                                {
                                    int num6 = Dust.NewDust(projectile.position, 12, 12, DustID.TintableDust, 0f, 0f, 50, newColor, 1.5f);
                                    Main.dust[num6].velocity.Y -= 0.1f + (float)num4 * 0.5f;
                                    Main.dust[num6].velocity.Y *= Main.rand.NextFloat();
                                    Main.dust[num6].velocity.X *= Main.rand.NextFloatDirection() * 3f;
                                    Main.dust[num6].position = new Vector2(i * 16 + Main.rand.Next(16), j * 16 + Main.rand.Next(16));
                                    if (!Main.rand.NextBool(3))
                                    {
                                        Main.dust[num6].velocity *= 0.5f;
                                        Main.dust[num6].noGravity = true;
                                    }
                                }
                            }
                            else
                            {
                                for (int m = 0; m < num5 - 1; m++)
                                {
                                    Dust obj2 = Main.dust[WorldGen.KillTile_MakeTileDust(i, j, tileSafely)];
                                    obj2.velocity.Y -= 1f + (float)num4;
                                    obj2.velocity.Y *= Main.rand.NextFloat();
                                    obj2.velocity.Y *= 0.75f;
                                }
                            }
                        }

                        if (num5 <= 0 || Main.rand.NextBool(3))
                            continue;

                        float num7 = (float)Math.Abs(num2 - i) / (maxHitboxSize / 2f);
                        if (projectile.type == ProjectileID.QueenSlimeSmash)
                        {
                            Color newColor2 = NPC.AI_121_QueenSlime_GetDustColor();
                            newColor2.A = 150;
                            for (int n = 0; n < 3; n++)
                            {
                                int num8 = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Smoke, 0f, 0f, 50, newColor2, 2f - (float)num4 * 0.15f + num7 * 0.5f);
                                Main.dust[num8].velocity.Y -= 0.1f + (float)num4 * 0.5f + num7 * (float)num4 * 1f;
                                Main.dust[num8].velocity.Y *= Main.rand.NextFloat();
                                Main.dust[num8].velocity.X *= Main.rand.NextFloatDirection() * 3f;
                                Main.dust[num8].position = new Vector2(i * 16 + 20, j * 16 + 20);
                                if (!Main.rand.NextBool(3))
                                {
                                    Main.dust[num8].velocity *= 0.5f;
                                    Main.dust[num8].noGravity = true;
                                }
                            }
                        }
                        else
                        {
                            Gore gore = Gore.NewGoreDirect(projectile.GetSource_FromAI(), projectile.position, Vector2.Zero, 61 + Main.rand.Next(3), 1f - (float)num4 * 0.15f + num7 * 0.5f);
                            gore.velocity.Y -= 0.1f + (float)num4 * 0.5f + num7 * (float)num4 * 1f;
                            gore.velocity.Y *= Main.rand.NextFloat();
                            gore.position = new Vector2(i * 16 + 20, j * 16 + 20);
                        }
                    }
                }

                return false;
            }

            else if (projectile.type == ProjectileID.FrostWave && projectile.ai[1] > 0f)
            {
                if (projectile.ai[0] < 0f)
                {
                    projectile.ai[0] += 1f;
                }
                else
                {
                    if (projectile.velocity.Length() < projectile.ai[1])
                    {
                        projectile.velocity *= 1.04f;
                        if (projectile.velocity.Length() > projectile.ai[1])
                        {
                            projectile.velocity.Normalize();
                            projectile.velocity *= projectile.ai[1];
                        }
                    }
                    else
                    {
                        if (projectile.ai[0] == 0f || projectile.ai[0] == 2f)
                        {
                            projectile.scale += 0.005f;
                            projectile.alpha -= 25;
                            if (projectile.alpha <= 0)
                            {
                                projectile.ai[0] = 1f;
                                projectile.alpha = 0;
                            }
                        }
                        else if (projectile.ai[0] == 1f)
                        {
                            projectile.scale -= 0.005f;
                            projectile.alpha += 25;
                            if (projectile.alpha >= 255)
                            {
                                projectile.ai[0] = 2f;
                                projectile.alpha = 255;
                            }
                        }
                    }
                }

                projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;

                return false;
            }

            // Phase 1 sharknado
            else if (projectile.type == ProjectileID.SharknadoBolt)
            {
                if (projectile.ai[1] == 0f)
                {
                    float num552 = (float)Math.PI / 15f;
                    float num553 = 4f;
                    float num554 = (float)(Math.Cos(num552 * projectile.ai[0]) - 0.5) * num553;
                    projectile.velocity.Y -= num554;
                    projectile.ai[0]++;
                    num554 = (float)(Math.Cos(num552 * projectile.ai[0]) - 0.5) * num553;
                    projectile.velocity.Y += num554;
                    projectile.localAI[0]++;
                    if (projectile.localAI[0] > 10f)
                    {
                        projectile.alpha -= 5;
                        if (projectile.alpha < 100)
                            projectile.alpha = 100;

                        projectile.rotation += projectile.velocity.X * 0.1f;
                        projectile.frame = (int)(projectile.localAI[0] / 3f) % 3;
                    }

                    if (projectile.wet)
                    {
                        projectile.position.Y -= 16f;
                        projectile.Kill();
                    }

                    return false;
                }
                else if (projectile.ai[1] < 0f)
                {
                    projectile.timeLeft -= 2;

                    float num623 = (float)Math.PI / 15f;
                    float num624 = -2f;
                    float num625 = (float)(Math.Cos(num623 * projectile.ai[0]) - 0.5) * num624;

                    projectile.velocity.Y -= num625;

                    projectile.ai[0] += 1f;

                    num625 = (float)(Math.Cos(num623 * projectile.ai[0]) - 0.5) * num624;

                    projectile.velocity.Y += num625;

                    projectile.localAI[0] += 1f;
                    if (projectile.localAI[0] > 10f)
                    {
                        projectile.alpha -= 5;
                        if (projectile.alpha < 100)
                            projectile.alpha = 100;

                        projectile.rotation += projectile.velocity.X * 0.1f;
                        projectile.frame = (int)(projectile.localAI[0] / 3f) % 3;
                    }

                    if (projectile.wet)
                    {
                        projectile.position.Y -= 16f;
                        projectile.Kill();
                    }

                    return false;
                }
            }

            // Larger cthulhunadoes
            else if (projectile.type == ProjectileID.Cthulunado)
            {
                if (CalamityWorld.revenge || BossRushEvent.BossRushActive)
                {
                    bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

                    int num606 = 16;
                    int num607 = 16;
                    float segmentScale = death ? 2.5f : 2f;
                    int segmentWidth = 150;
                    int segmentHeight = 42;

                    if (projectile.velocity.X != 0f)
                        projectile.direction = projectile.spriteDirection = -Math.Sign(projectile.velocity.X);

                    int num3 = projectile.frameCounter;
                    projectile.frameCounter = num3 + 1;
                    if (projectile.frameCounter > 2)
                    {
                        num3 = projectile.frame;
                        projectile.frame = num3 + 1;
                        projectile.frameCounter = 0;
                    }
                    if (projectile.frame >= 6)
                        projectile.frame = 0;

                    if (projectile.localAI[0] == 0f && Main.myPlayer == projectile.owner)
                    {
                        projectile.localAI[0] = 1f;
                        projectile.position.X += projectile.width / 2;
                        projectile.position.Y += projectile.height / 2;
                        projectile.scale = (num606 + num607 - projectile.ai[1]) * segmentScale / (num607 + num606);
                        projectile.width = (int)(segmentWidth * projectile.scale);
                        projectile.height = (int)(segmentHeight * projectile.scale);
                        projectile.position.X -= projectile.width / 2;
                        projectile.position.Y -= projectile.height / 2;
                        projectile.netUpdate = true;
                    }

                    if (projectile.ai[1] != -1f)
                    {
                        projectile.scale = (num606 + num607 - projectile.ai[1]) * segmentScale / (num607 + num606);
                        projectile.width = (int)(segmentWidth * projectile.scale);
                        projectile.height = (int)(segmentHeight * projectile.scale);
                    }

                    int maxAlpha = 150;
                    int minAlpha = 100;
                    if (projectile.timeLeft > FishronCthulhunadoTotalDuration - FishronTornadoTimeBeforeDealingDamage)
                    {
                        maxAlpha = 220;
                        minAlpha = 200;
                    }

                    if (!Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                    {
                        projectile.alpha -= 30;
                        if (projectile.alpha < minAlpha)
                            projectile.alpha = minAlpha;
                    }
                    else
                    {
                        projectile.alpha += 30;
                        if (projectile.alpha > maxAlpha)
                            projectile.alpha = maxAlpha;
                    }

                    if (projectile.ai[0] > 0f)
                        projectile.ai[0] -= 1f;

                    if (projectile.ai[0] == 1f && projectile.ai[1] > 0f && projectile.owner == Main.myPlayer)
                    {
                        projectile.netUpdate = true;

                        Vector2 center = projectile.Center;
                        center.Y -= segmentHeight * projectile.scale / 2f;

                        float num611 = (num606 + num607 - projectile.ai[1] + 1f) * segmentScale / (num607 + num606);
                        center.Y -= segmentHeight * num611 / 2f;
                        center.Y += 2f;

                        float segmentSpawnDelay = 10f;
                        Projectile.NewProjectile(projectile.GetSource_FromThis(), center, projectile.velocity, projectile.type, projectile.damage, projectile.knockBack, projectile.owner, segmentSpawnDelay, projectile.ai[1] - 1f);

                        int sharkronSpawnGateValue = death ? 2 : 3;
                        if ((int)projectile.ai[1] % sharkronSpawnGateValue == 0 && projectile.ai[1] != 0f)
                        {
                            int sharkron = NPC.NewNPC(projectile.GetSource_FromAI(), (int)center.X, (int)center.Y, NPCID.Sharkron2);
                            Main.npc[sharkron].velocity = projectile.velocity;
                            Main.npc[sharkron].scale = 1.5f;
                            Main.npc[sharkron].netUpdate = true;
                            Main.npc[sharkron].ai[2] = projectile.width;
                            Main.npc[sharkron].ai[3] = -1.5f;
                        }
                    }

                    if (projectile.ai[0] <= 0f)
                    {
                        float swayAmount = MathHelper.Pi / 30f;
                        float widthSwayScale = projectile.width / 5f * 2.5f;
                        float sway = (float)(Math.Cos(swayAmount * -(double)projectile.ai[0]) - 0.5) * widthSwayScale;

                        projectile.position.X -= sway * -projectile.direction;

                        projectile.ai[0] -= 1f;

                        sway = (float)(Math.Cos(swayAmount * -(double)projectile.ai[0]) - 0.5) * widthSwayScale;
                        projectile.position.X += sway * -projectile.direction;
                    }

                    return false;
                }
                else
                {
                    int minAlpha = 100;
                    if (projectile.timeLeft > FishronCthulhunadoTotalDuration - FishronTornadoTimeBeforeDealingDamage)
                        minAlpha = 200;

                    int alphaChange = 30;
                    if (!Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                    {
                        projectile.alpha -= alphaChange;
                        if (projectile.alpha < minAlpha + alphaChange)
                            projectile.alpha = minAlpha + alphaChange;
                    }
                }
            }

            else if (projectile.type == ProjectileID.HallowBossRainbowStreak && projectile.hostile)
            {
                bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

                bool spreadOut = false;
                bool homeIn = false;
                float spreadOutCutoffTime = EmpressRainbowStreakSpreadOutCutoff;
                float homeInCutoffTime = NPC.ShouldEmpressBeEnraged() ? (death ? 55f : 65f) : (death ? 70f : 80f);
                float spreadDeceleration = 0.97f;
                float minAcceleration = death ? 0.075f : 0.05f;
                float maxAcceleration = death ? 0.15f : 0.1f;
                float homingVelocity = death ? 36f : 30f;
                float maxVelocity = homingVelocity * 1.5f;
                float accelerationToMaxVelocity = 1.01f;

                if (projectile.timeLeft > spreadOutCutoffTime)
                    spreadOut = true;
                else if (projectile.timeLeft > homeInCutoffTime)
                    homeIn = true;

                if (spreadOut)
                {
                    float spreadVelocity = (float)Math.Cos(projectile.whoAmI % 6f / 6f + projectile.position.X / 320f + projectile.position.Y / 160f);
                    projectile.velocity *= spreadDeceleration;
                    projectile.velocity = projectile.velocity.RotatedBy(spreadVelocity * MathHelper.TwoPi * 0.125f * 1f / 30f);
                }

                if (homeIn)
                {
                    int playerIndex = (int)projectile.ai[0];
                    Vector2 velocity = projectile.velocity;
                    if (Main.player.IndexInRange(playerIndex))
                    {
                        Player player = Main.player[playerIndex];
                        velocity = projectile.DirectionTo(player.Center) * homingVelocity;
                    }

                    float amount = MathHelper.Lerp(minAcceleration, maxAcceleration, Utils.GetLerpValue(spreadOutCutoffTime, 30f, projectile.timeLeft, clamped: true));
                    projectile.velocity = Vector2.SmoothStep(projectile.velocity, velocity, amount);
                }
                else
                {
                    if (projectile.velocity.Length() < maxVelocity)
                    {
                        projectile.velocity *= accelerationToMaxVelocity;
                        if (projectile.velocity.Length() > maxVelocity)
                        {
                            projectile.velocity.Normalize();
                            projectile.velocity *= maxVelocity;
                        }
                    }
                }

                projectile.Opacity = spreadOut ? 0.4f : Utils.GetLerpValue(240f, 220f, projectile.timeLeft, clamped: true);
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

                return false;
            }

            else if (projectile.type == ProjectileID.CultistBossLightningOrb)
            {
                if (NPC.AnyNPCs(NPCID.CultistBoss))
                {
                    if (projectile.localAI[1] == 0f)
                    {
                        SoundEngine.PlaySound(SoundID.Item121, projectile.Center);
                        projectile.localAI[1] = 1f;
                    }

                    if (projectile.ai[0] < 180f)
                    {
                        projectile.alpha -= 5;
                        if (projectile.alpha < 0)
                            projectile.alpha = 0;
                    }
                    else
                    {
                        projectile.alpha += 5;
                        if (projectile.alpha > 255)
                        {
                            projectile.alpha = 255;
                            projectile.Kill();
                            return false;
                        }
                    }

                    ref float reference = ref projectile.ai[0];
                    ref float reference46 = ref reference;
                    float num15 = reference;
                    reference46 = num15 + 1f;

                    if (projectile.ai[0] % 30f == 0f && projectile.ai[0] < 180f && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int maxTargets = 2;
                        int[] array6 = new int[maxTargets];
                        Vector2[] array7 = new Vector2[maxTargets];
                        int num731 = 0;
                        float num732 = 2000f;

                        for (int num733 = 0; num733 < Main.maxPlayers; num733++)
                        {
                            if (!Main.player[num733].active || Main.player[num733].dead)
                                continue;

                            Vector2 center9 = Main.player[num733].Center;
                            float num734 = Vector2.Distance(center9, projectile.Center);
                            if (num734 < num732 && Collision.CanHit(projectile.Center, 1, 1, center9, 1, 1))
                            {
                                array6[num731] = num733;
                                array7[num731] = center9;
                                int num34 = num731 + 1;
                                num731 = num34;
                                if (num34 >= array7.Length)
                                    break;
                            }
                        }

                        for (int num735 = 0; num735 < num731; num735++)
                        {
                            Vector2 vector52 = array7[num735] - projectile.Center;
                            float ai = Main.rand.Next(100);
                            Vector2 vector53 = Vector2.Normalize(vector52.RotatedByRandom(MathHelper.PiOver4)) * 7f;
                            Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, vector53, ProjectileID.CultistBossLightningOrbArc, projectile.damage, 0f, Main.myPlayer, vector52.ToRotation(), ai);
                        }
                    }

                    Lighting.AddLight(projectile.Center, 0.4f, 0.85f, 0.9f);

                    if (++projectile.frameCounter >= 4)
                    {
                        projectile.frameCounter = 0;
                        if (++projectile.frame >= Main.projFrames[projectile.type])
                            projectile.frame = 0;
                    }

                    if (projectile.alpha >= 150 || !(projectile.ai[0] < 180f))
                        return false;

                    for (int num736 = 0; num736 < 1; num736++)
                    {
                        float num737 = (float)Main.rand.NextDouble() * 1f - 0.5f;
                        if (num737 < -0.5f)
                            num737 = -0.5f;
                        if (num737 > 0.5f)
                            num737 = 0.5f;

                        Vector2 value40 = new Vector2(-projectile.width * 0.2f * projectile.scale, 0f).RotatedBy(num737 * MathHelper.TwoPi).RotatedBy(projectile.velocity.ToRotation());
                        Dust zap = Dust.NewDustDirect(projectile.Center - Vector2.One * 5f, 10, 10, DustID.Electric, (0f - projectile.velocity.X) / 3f, (0f - projectile.velocity.Y) / 3f, 150, Color.Transparent, 0.7f);
                        zap.position = projectile.Center + value40;
                        zap.velocity = Vector2.Normalize(zap.position - projectile.Center) * 2f;
                        zap.noGravity = true;
                    }

                    for (int num739 = 0; num739 < 1; num739++)
                    {
                        float num740 = (float)Main.rand.NextDouble() * 1f - 0.5f;
                        if (num740 < -0.5f)
                            num740 = -0.5f;
                        if (num740 > 0.5f)
                            num740 = 0.5f;

                        Vector2 value41 = new Vector2(-projectile.width * 0.6f * projectile.scale, 0f).RotatedBy(num740 * MathHelper.TwoPi).RotatedBy(projectile.velocity.ToRotation());
                        Dust zap = Dust.NewDustDirect(projectile.Center - Vector2.One * 5f, 10, 10, DustID.Electric, (0f - projectile.velocity.X) / 3f, (0f - projectile.velocity.Y) / 3f, 150, Color.Transparent, 0.7f);
                        zap.velocity = Vector2.Zero;
                        zap.position = projectile.Center + value41;
                        zap.noGravity = true;
                    }

                    return false;
                }
            }

            // Copy pasted vanilla AI with minor changes to the homing distance and velocity formula
            else if (projectile.type == ProjectileID.SpiritFlame)
            {
                float maxSpeed = 12f;
                int accelerationTime = 30;

                if (projectile.localAI[0] > 0f)
                    projectile.localAI[0]--;

                if (projectile.localAI[0] == 0f && projectile.ai[0] < 0f && projectile.owner == Main.myPlayer)
                {
                    projectile.localAI[0] = 5f;
                    for (int num1034 = 0; num1034 < Main.maxNPCs; num1034++)
                    {
                        NPC nPC13 = Main.npc[num1034];
                        if (nPC13.CanBeChasedBy(this))
                        {
                            bool flag63 = projectile.ai[0] < 0f || Main.npc[(int)projectile.ai[0]].Distance(projectile.Center) > nPC13.Distance(projectile.Center);
                            if ((flag63 & (nPC13.Distance(projectile.Center) < 500f)) && (Collision.CanHitLine(projectile.Center, 0, 0, nPC13.Center, 0, 0) || Collision.CanHitLine(projectile.Center, 0, 0, nPC13.Top, 0, 0)))
                                projectile.ai[0] = num1034;
                        }
                    }

                    if (projectile.ai[0] >= 0f)
                    {
                        projectile.timeLeft = 300;
                        projectile.netUpdate = true;
                    }
                }

                if (projectile.timeLeft > 30 && projectile.alpha > 0)
                    projectile.alpha -= 12;

                if (projectile.timeLeft > 30 && projectile.alpha < 128 && Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                    projectile.alpha = 128;

                if (projectile.alpha < 0)
                    projectile.alpha = 0;

                if (++projectile.frameCounter > 4)
                {
                    projectile.frameCounter = 0;
                    if (++projectile.frame >= 4)
                        projectile.frame = 0;
                }

                float num1035 = 0.5f;
                if (projectile.timeLeft < 120)
                    num1035 = 1.1f;

                if (projectile.timeLeft < 60)
                    num1035 = 1.6f;

                projectile.ai[1]++;
                float num1036 = projectile.ai[1] / 180f * MathHelper.TwoPi;
                for (float num1037 = 0f; num1037 < 3f; num1037++)
                {
                    if (Main.rand.NextBool(3))
                    {
                        Dust shflame = Dust.NewDustDirect(projectile.Center, 0, 0, DustID.Shadowflame, 0f, -2f, 200);
                        shflame.position = projectile.Center + Vector2.UnitY.RotatedBy(num1037 * MathHelper.TwoPi / 3f + projectile.ai[1]) * 10f;
                        shflame.noGravity = true;
                        shflame.velocity = projectile.DirectionFrom(shflame.position);
                        shflame.scale = num1035;
                        shflame.fadeIn = 0.5f;
                    }
                }

                if (projectile.timeLeft > 2 && Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                    projectile.timeLeft = 2;

                int num1038 = (int)projectile.ai[0];
                if (num1038 >= 0 && Main.npc[num1038].active)
                {
                    if (projectile.Distance(Main.npc[num1038].Center) > 1f)
                    {
                        Vector2 vector106 = projectile.DirectionTo(Main.npc[num1038].Center).SafeNormalize(Vector2.UnitX);
                        float length = projectile.velocity.Length();
                        float step = maxSpeed / accelerationTime;
                        if (length >= maxSpeed)
                            step = 0f;

                        projectile.velocity = vector106 * (length + step);

                        if (length >= maxSpeed)
                        {
                            if ((projectile.Center + projectile.velocity).Distance(Main.npc[num1038].Center) > projectile.Center.Distance(Main.npc[num1038].Center))
                            {
                                projectile.velocity = Vector2.Zero;
                                projectile.Center = Main.npc[num1038].Center;
                            }
                        }
                    }

                    return false;
                }

                if (projectile.ai[0] == -1f && projectile.timeLeft > 5)
                    projectile.timeLeft = 5;

                if (projectile.ai[0] == -2f && projectile.timeLeft > 180)
                    projectile.timeLeft = 180;

                if (projectile.ai[0] >= 0f)
                {
                    projectile.ai[0] = -1f;
                    projectile.netUpdate = true;
                }

                return false;
            }

            else if (projectile.type == ProjectileID.ChlorophyteBullet)
            {
                if (projectile.alpha < 170)
                {
                    int totalDust = 5;
                    for (int i = 0; i < totalDust; i++)
                    {
                        float x2 = projectile.position.X - projectile.velocity.X / (float)totalDust * (float)i;
                        float y2 = projectile.position.Y - projectile.velocity.Y / (float)totalDust * (float)i;
                        int dust = Dust.NewDust(new Vector2(x2, y2), 1, 1, DustID.CursedTorch);
                        Main.dust[dust].alpha = projectile.alpha;
                        Main.dust[dust].position.X = x2;
                        Main.dust[dust].position.Y = y2;
                        Main.dust[dust].velocity *= 0f;
                        Main.dust[dust].noGravity = true;
                    }
                }

                float velocityLength = (float)Math.Sqrt(projectile.velocity.X * projectile.velocity.X + projectile.velocity.Y * projectile.velocity.Y);
                float cachedVelocityLength = projectile.localAI[0];
                if (cachedVelocityLength == 0f)
                {
                    projectile.localAI[0] = velocityLength;
                    cachedVelocityLength = velocityLength;
                }

                if (projectile.alpha > 0)
                    projectile.alpha -= 25;

                if (projectile.alpha < 0)
                    projectile.alpha = 0;

                float posX = projectile.position.X;
                float posY = projectile.position.Y;
                float homingDistance = 300f;
                bool homeIn = false;
                int target = 0;
                if (projectile.ai[1] == 0f)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (Main.npc[i].CanBeChasedBy(this) && (projectile.ai[1] == 0f || projectile.ai[1] == (float)(i + 1)))
                        {
                            float targetCenterX = Main.npc[i].Center.X;
                            float targetCenterY = Main.npc[i].Center.Y;
                            float targetDistance = Math.Abs(projectile.Center.X - targetCenterX) + Math.Abs(projectile.Center.Y - targetCenterY);
                            if (targetDistance < homingDistance && Collision.CanHit(projectile.Center, 1, 1, Main.npc[i].position, Main.npc[i].width, Main.npc[i].height))
                            {
                                homingDistance = targetDistance;
                                posX = targetCenterX;
                                posY = targetCenterY;
                                homeIn = true;
                                target = i;
                            }
                        }
                    }

                    if (homeIn)
                        projectile.ai[1] = target + 1;

                    homeIn = false;
                }

                if (projectile.ai[1] > 0f)
                {
                    int targetIndex = (int)(projectile.ai[1] - 1f);
                    if (Main.npc[targetIndex].active && Main.npc[targetIndex].CanBeChasedBy(this, ignoreDontTakeDamage: true) && !Main.npc[targetIndex].dontTakeDamage)
                    {
                        float targetCenterX = Main.npc[targetIndex].Center.X;
                        float targetCenterY = Main.npc[targetIndex].Center.Y;
                        float homingCutOffDistance = 1000f;
                        if (Math.Abs(projectile.Center.X - targetCenterX) + Math.Abs(projectile.Center.Y - targetCenterY) < homingCutOffDistance)
                        {
                            homeIn = true;
                            posX = Main.npc[targetIndex].Center.X;
                            posY = Main.npc[targetIndex].Center.Y;
                        }
                    }
                    else
                        projectile.ai[1] = 0f;
                }

                if (!projectile.friendly)
                    homeIn = false;

                if (homeIn)
                {
                    int inertia = 8;
                    float homingSpeed = cachedVelocityLength;
                    Vector2 destination = new Vector2(posX, posY);
                    Vector2 homeDirection = (destination - projectile.Center).SafeNormalize(Vector2.UnitY);
                    projectile.velocity = (projectile.velocity * inertia + homeDirection * homingSpeed) / (inertia + 1f);
                }

                projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;

                return false;
            }

            // Adjust dust to avoid lag.
            else if (projectile.type == ProjectileID.VampireHeal)
            {
                projectile.HealingProjectile((int)projectile.ai[1], (int)projectile.ai[0], 4f, 15f);

                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.VampireHeal, 0f, 0f, 100);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0f;
                Main.dust[dust].position.X -= projectile.velocity.X * 0.2f;
                Main.dust[dust].position.Y += projectile.velocity.Y * 0.2f;

                return false;
            }

            // Adjust dust to avoid lag.
            else if (projectile.type == ProjectileID.SpectreWrath)
            {
                projectile.ai[1] += 1f;
                if (projectile.ai[1] >= 60f)
                {
                    projectile.friendly = true;
                    int target = (int)projectile.ai[0];
                    if (Main.myPlayer == projectile.owner && (target == -1 || !Main.npc[target].CanBeChasedBy(projectile)))
                    {
                        target = -1;
                        int[] array = new int[Main.maxNPCs];
                        int randomTargets = 0;
                        float homingDistance = 800f;
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            if (Main.npc[i].CanBeChasedBy(projectile))
                            {
                                float distanceFromTarget = Math.Abs(Main.npc[i].Center.X - projectile.Center.X) + Math.Abs(Main.npc[i].Center.Y - projectile.Center.Y);
                                if (distanceFromTarget < homingDistance)
                                {
                                    array[randomTargets] = i;
                                    randomTargets++;
                                }
                            }
                        }

                        if (randomTargets == 0)
                        {
                            projectile.Kill();
                            return false;
                        }

                        target = array[Main.rand.Next(randomTargets)];
                        projectile.ai[0] = target;
                        projectile.netUpdate = true;
                    }

                    if (target != -1)
                    {
                        int inertia = 30;
                        float homingSpeed = 4f;
                        Vector2 homeDirection = (Main.npc[target].Center - projectile.Center).SafeNormalize(Vector2.UnitY);
                        projectile.velocity = (projectile.velocity * inertia + homeDirection * homingSpeed) / (inertia + 1f);
                    }
                }

                int maxDust = 3;
                float dustOffsetMultiplier = 1f / (float)maxDust;
                for (int i = 0; i < maxDust; i++)
                {
                    float dustOffsetX = projectile.velocity.X * dustOffsetMultiplier * (float)i;
                    float dustOffsetY = (0f - projectile.velocity.Y * dustOffsetMultiplier) * (float)i;
                    int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.SpectreStaff, 0f, 0f, 100, default, 1.3f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 0f;
                    Main.dust[dust].position.X -= dustOffsetX;
                    Main.dust[dust].position.Y -= dustOffsetY;
                }

                return false;
            }

            // Adjust dust to avoid lag.
            else if (projectile.type == ProjectileID.SpiritHeal)
            {
                projectile.HealingProjectile((int)projectile.ai[1], (int)projectile.ai[0], 4f, 15f);

                int maxDust = 3;
                float dustOffsetMultiplier = 1f / (float)maxDust;
                for (int i = 0; i < maxDust; i++)
                {
                    float dustOffsetX = projectile.velocity.X * dustOffsetMultiplier * (float)i;
                    float dustOffsetY = (0f - projectile.velocity.Y * dustOffsetMultiplier) * (float)i;
                    int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.SpectreStaff, 0f, 0f, 100, default, 1.3f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 0f;
                    Main.dust[dust].position.X -= dustOffsetX;
                    Main.dust[dust].position.Y -= dustOffsetY;
                }

                return false;
            }

            else if (projectile.type == ProjectileID.NurseSyringeHeal)
            {
                ref float initialSpeed = ref projectile.localAI[1];
                if (initialSpeed == 0f)
                    initialSpeed = projectile.velocity.Length();

                bool invalidHealTarget = !Main.npc.IndexInRange((int)projectile.ai[0]) || !Main.npc[(int)projectile.ai[0]].active || !Main.npc[(int)projectile.ai[0]].townNPC;
                if (invalidHealTarget)
                {
                    projectile.Kill();
                    return false;
                }

                NPC npcToHeal = Main.npc[(int)projectile.ai[0]];

                // If the needle is not colliding with the target, attempt to move towards it while falling.
                if (!projectile.WithinRange(npcToHeal.Center, initialSpeed) && !projectile.Hitbox.Intersects(npcToHeal.Hitbox))
                {
                    Vector2 flySpeed = projectile.SafeDirectionTo(npcToHeal.Center) * initialSpeed;

                    // Prevent the needle from ever violating its gravity.
                    if (flySpeed.Y < projectile.velocity.Y)
                        flySpeed.Y = projectile.velocity.Y;

                    flySpeed.Y++;

                    projectile.velocity = Vector2.Lerp(projectile.velocity, flySpeed, 0.04f);
                    projectile.rotation += projectile.velocity.X * 0.05f;
                    return false;
                }

                // Otherwise, die immediately and heal the target.
                projectile.Kill();

                int healAmount = npcToHeal.lifeMax - npcToHeal.life;
                int maxHealAmount = 20;

                // If the target has more than 250 max life, incorporate their total life into the max amount to heal.
                // This is done so that more powerful NPCs do not take an eternity to receive meaningful healing benefits
                // from the Nurse.
                if (npcToHeal.lifeMax > 250)
                    maxHealAmount = (int)Math.Max(maxHealAmount, npcToHeal.lifeMax * 0.05f);

                if (healAmount > maxHealAmount)
                    healAmount = maxHealAmount;

                if (healAmount > 0)
                {
                    npcToHeal.life += healAmount;
                    npcToHeal.HealEffect(healAmount, true);
                    return false;
                }

                return false;
            }

            bool adultWyrmAlive = false;
            if (CalamityGlobalNPC.adultEidolonWyrmHead != -1)
            {
                if (Main.npc[CalamityGlobalNPC.adultEidolonWyrmHead].active)
                    adultWyrmAlive = true;
            }

            if (adultWyrmAlive || (CalamityWorld.death && !CalamityPlayer.areThereAnyDamnBosses))
            {
                if (projectile.type == ProjectileID.CultistBossFireBallClone)
                {
                    if (projectile.ai[1] == 0f)
                    {
                        projectile.ai[1] = 1f;
                        SoundEngine.PlaySound(SoundID.Item34, projectile.Center);
                    }
                    else if (projectile.ai[1] == 1f && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int num13 = -1;
                        float num14 = 2000f;
                        for (int num15 = 0; num15 < Main.maxPlayers; num15++)
                        {
                            if (Main.player[num15].active && !Main.player[num15].dead)
                            {
                                Vector2 center2 = Main.player[num15].Center;
                                float num16 = Vector2.Distance(center2, projectile.Center);
                                if ((num16 < num14 || num13 == -1) && Collision.CanHit(projectile.Center, 1, 1, center2, 1, 1))
                                {
                                    num14 = num16;
                                    num13 = num15;
                                }
                            }
                        }

                        if (num14 < 20f)
                        {
                            projectile.Kill();
                            return false;
                        }

                        if (num13 != -1)
                        {
                            projectile.ai[1] = 21f;
                            projectile.ai[0] = num13;
                            projectile.netUpdate = true;
                        }
                    }
                    else if (projectile.ai[1] > 20f && projectile.ai[1] < 200f)
                    {
                        projectile.ai[1] += 1f;
                        int num17 = (int)projectile.ai[0];
                        if (!Main.player[num17].active || Main.player[num17].dead)
                        {
                            projectile.ai[1] = 1f;
                            projectile.ai[0] = 0f;
                            projectile.netUpdate = true;
                        }
                        else
                        {
                            float num18 = projectile.velocity.ToRotation();
                            Vector2 vector2 = Main.player[num17].Center - projectile.Center;
                            if (vector2.Length() < 20f)
                            {
                                projectile.Kill();
                                return false;
                            }

                            float targetAngle2 = vector2.ToRotation();
                            if (vector2 == Vector2.Zero)
                                targetAngle2 = num18;

                            float num19 = num18.AngleLerp(targetAngle2, 0.01f);
                            projectile.velocity = new Vector2(projectile.velocity.Length(), 0f).RotatedBy(num19);
                        }
                    }

                    if (projectile.ai[1] >= 1f && projectile.ai[1] < 20f)
                    {
                        projectile.ai[1] += 1f;
                        if (projectile.ai[1] == 20f)
                            projectile.ai[1] = 1f;
                    }

                    projectile.alpha -= 40;
                    if (projectile.alpha < 0)
                        projectile.alpha = 0;

                    projectile.spriteDirection = projectile.direction;

                    projectile.frameCounter++;
                    if (projectile.frameCounter >= 3)
                    {
                        projectile.frame++;
                        projectile.frameCounter = 0;
                        if (projectile.frame >= 4)
                            projectile.frame = 0;
                    }

                    if (Main.rand.NextBool(4))
                    {
                        Vector2 value4 = -Vector2.UnitX.RotatedByRandom(MathHelper.ToRadians(11.25f)).RotatedBy(projectile.velocity.ToRotation());
                        Dust smoke = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Smoke, 0f, 0f, 100);
                        smoke.velocity *= 0.1f;
                        smoke.position = projectile.Center + value4 * projectile.width / 2f;
                        smoke.fadeIn = 0.9f;
                    }

                    if (Main.rand.NextBool(32))
                    {
                        Vector2 value5 = -Vector2.UnitX.RotatedByRandom(MathHelper.ToRadians(22.5f)).RotatedBy(projectile.velocity.ToRotation());
                        Dust smoke = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Smoke, 0f, 0f, 155, default, 0.8f);
                        smoke.velocity *= 0.3f;
                        smoke.position = projectile.Center + value5 * projectile.width / 2f;
                        if (Main.rand.NextBool(2))
                            smoke.fadeIn = 1.4f;
                    }

                    if (Main.rand.NextBool(2))
                    {
                        Vector2 value6 = -Vector2.UnitX.RotatedByRandom(MathHelper.ToRadians(45f)).RotatedBy(projectile.velocity.ToRotation());
                        Dust shflame = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Shadowflame, 0f, 0f, 0, default, 1.2f);
                        shflame.velocity *= 0.3f;
                        shflame.noGravity = true;
                        shflame.position = projectile.Center + value6 * projectile.width / 2f;
                        if (Main.rand.NextBool(2))
                            shflame.fadeIn = 1.4f;
                    }

                    return false;
                }
                else if (projectile.type == ProjectileID.CultistBossIceMist)
                {
                    if (projectile.localAI[1] == 0f)
                    {
                        projectile.localAI[1] = 1f;
                        SoundEngine.PlaySound(SoundID.Item120, projectile.Center);
                    }

                    projectile.ai[0] += 1f;

                    // Main projectile
                    float duration = 300f;
                    if (projectile.ai[1] == 1f)
                    {
                        if (projectile.ai[0] >= duration - 20f)
                            projectile.alpha += 10;
                        else
                            projectile.alpha -= 10;

                        if (projectile.alpha < 0)
                            projectile.alpha = 0;
                        if (projectile.alpha > 255)
                            projectile.alpha = 255;

                        if (projectile.ai[0] >= duration)
                        {
                            projectile.Kill();
                            return false;
                        }

                        int num103 = Player.FindClosest(projectile.Center, 1, 1);
                        Vector2 vector11 = Main.player[num103].Center - projectile.Center;
                        float scaleFactor2 = projectile.velocity.Length();
                        vector11.Normalize();
                        vector11 *= scaleFactor2;
                        projectile.velocity = (projectile.velocity * 15f + vector11) / 16f;
                        projectile.velocity.Normalize();
                        projectile.velocity *= scaleFactor2;

                        if (projectile.ai[0] % 60f == 0f && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 vector50 = projectile.rotation.ToRotationVector2();
                            Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, vector50, projectile.type, projectile.damage, projectile.knockBack, projectile.owner);
                        }

                        projectile.rotation += MathHelper.Pi / 30f;

                        return false;
                    }

                    // Split projectiles
                    projectile.position -= projectile.velocity;

                    if (projectile.ai[0] >= duration - 260f)
                        projectile.alpha += 3;
                    else
                        projectile.alpha -= 40;

                    if (projectile.alpha < 0)
                        projectile.alpha = 0;
                    if (projectile.alpha > 255)
                        projectile.alpha = 255;

                    if (projectile.ai[0] >= duration - 255f)
                    {
                        projectile.Kill();
                        return false;
                    }

                    Vector2 value39 = new Vector2(0f, -720f).RotatedBy(projectile.velocity.ToRotation());
                    float scaleFactor3 = projectile.ai[0] % (duration - 255f) / (duration - 255f);
                    Vector2 spinningpoint13 = value39 * scaleFactor3;

                    for (int num724 = 0; num724 < 6; num724++)
                    {
                        Vector2 vector51 = projectile.Center + spinningpoint13.RotatedBy(num724 * MathHelper.TwoPi / 6f);
                        Dust ice = Dust.NewDustDirect(vector51 + Utils.RandomVector2(Main.rand, -8f, 8f) / 2f, 8, 8, DustID.NorthPole, 0f, 0f, 100, Color.Transparent);
                        ice.noGravity = true;
                    }

                    return false;
                }
                else if (projectile.type == ProjectileID.CultistBossLightningOrbArc && !projectile.friendly)
                {
                    projectile.frameCounter++;
                    if (projectile.velocity == Vector2.Zero)
                    {
                        if (projectile.frameCounter >= projectile.extraUpdates * 2)
                        {
                            projectile.frameCounter = 0;
                            bool flag30 = true;
                            for (int num742 = 1; num742 < projectile.oldPos.Length; num742++)
                            {
                                if (projectile.oldPos[num742] != projectile.oldPos[0])
                                    flag30 = false;
                            }

                            if (flag30)
                            {
                                projectile.Kill();
                                return false;
                            }
                        }
                    }
                    else
                    {
                        if (projectile.frameCounter < projectile.extraUpdates * 2)
                            return false;

                        projectile.frameCounter = 0;
                        float num748 = projectile.velocity.Length();
                        UnifiedRandom unifiedRandom = new((int)projectile.ai[1]);
                        int num749 = 0;
                        Vector2 spinningpoint14 = -Vector2.UnitY;
                        while (true)
                        {
                            int num750 = unifiedRandom.Next();
                            projectile.ai[1] = num750;
                            num750 %= 100;
                            float f = (float)num750 / 100f * MathHelper.TwoPi;
                            Vector2 vector55 = f.ToRotationVector2();
                            if (vector55.Y > 0f)
                            {
                                vector55.Y *= -1f;
                            }

                            bool flag31 = false;
                            if (vector55.Y > -0.02f)
                            {
                                flag31 = true;
                            }
                            if (vector55.X * (float)(projectile.extraUpdates + 1) * 2f * num748 + projectile.localAI[0] > 40f)
                            {
                                flag31 = true;
                            }
                            if (vector55.X * (float)(projectile.extraUpdates + 1) * 2f * num748 + projectile.localAI[0] < -40f)
                            {
                                flag31 = true;
                            }

                            if (flag31)
                            {
                                if (num749++ >= 100)
                                {
                                    projectile.velocity = Vector2.Zero;
                                    projectile.localAI[1] = 1f;
                                    break;
                                }
                                continue;
                            }

                            spinningpoint14 = vector55;

                            break;
                        }

                        if (projectile.velocity != Vector2.Zero)
                        {
                            projectile.localAI[0] += spinningpoint14.X * (float)(projectile.extraUpdates + 1) * 2f * num748;
                            projectile.velocity = spinningpoint14.RotatedBy(projectile.ai[0] + MathHelper.Pi / 2f) * num748;
                            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.Pi / 2f;
                        }
                    }

                    return false;
                }
            }

            if (CalamityWorld.revenge || BossRushEvent.BossRushActive)
            {
                bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

                if (projectile.type == ProjectileID.DeerclopsIceSpike)
                {
                    int dustType = 16;
                    float dustVelocityMultiplier = 0.75f;
                    int numDust = 5;
                    int numDust2 = 5;
                    int fadeInTime = 25;
                    int fadeOutGateValue = death ? 90 : 65;
                    float killGateValue = death ? 100f : 75f;
                    int maxFrames = 5;

                    bool fadeIn = projectile.ai[0] < (float)fadeInTime;
                    bool fadeOut = projectile.ai[0] >= (float)fadeOutGateValue;
                    bool killProjectile = projectile.ai[0] >= killGateValue;
                    projectile.ai[0] += 1f;
                    if (projectile.localAI[0] == 0f)
                    {
                        projectile.localAI[0] = 1f;
                        projectile.rotation = projectile.velocity.ToRotation();
                        projectile.frame = Main.rand.Next(maxFrames);

                        for (int i = 0; i < numDust; i++)
                        {
                            Dust dust = Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Circular(24f, 24f), dustType, projectile.velocity * dustVelocityMultiplier * MathHelper.Lerp(0.2f, 0.7f, Main.rand.NextFloat()));
                            dust.velocity += Main.rand.NextVector2Circular(0.5f, 0.5f);
                            dust.scale = 0.8f + Main.rand.NextFloat() * 0.5f;
                        }

                        for (int j = 0; j < numDust2; j++)
                        {
                            Dust dust2 = Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Circular(24f, 24f), dustType, Main.rand.NextVector2Circular(2f, 2f) + projectile.velocity * dustVelocityMultiplier * MathHelper.Lerp(0.2f, 0.5f, Main.rand.NextFloat()));
                            dust2.velocity += Main.rand.NextVector2Circular(0.5f, 0.5f);
                            dust2.scale = 0.8f + Main.rand.NextFloat() * 0.5f;
                            dust2.fadeIn = 1f;
                        }

                        SoundEngine.PlaySound(SoundID.DeerclopsIceAttack, projectile.Center);
                    }

                    if (fadeIn)
                    {
                        projectile.Opacity += 0.04f;
                        if (projectile.Opacity > 1f)
                            projectile.Opacity = 1f;

                        projectile.scale = projectile.Opacity * projectile.ai[1];
                    }

                    if (fadeOut)
                        projectile.Opacity -= 0.2f;

                    if (killProjectile)
                        projectile.Kill();

                    return false;
                }

                // Override Deerclops rubble behavior to create a wave of rubble instead of it all flying up at the same time
                // Rubble doesn't deal damage if it's not moving
                else if (projectile.type == ProjectileID.DeerclopsRangedProjectile)
                {
                    projectile.ai[0] += 1f;

                    projectile.frame = (int)projectile.ai[1];

                    if (projectile.localAI[0] == 0f)
                    {
                        projectile.localAI[0] = 1f;
                        projectile.rotation = projectile.velocity.ToRotation();
                        for (int dustIndex = 0; dustIndex < 5; dustIndex++)
                        {
                            Dust dust = Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Circular(24f, 24f), DustID.Cloud, projectile.velocity * MathHelper.Lerp(0.2f, 0.7f, Main.rand.NextFloat()));
                            dust.velocity += Main.rand.NextVector2Circular(0.5f, 0.5f);
                            dust.scale = 0.8f + Main.rand.NextFloat() * 0.5f;
                        }

                        for (int dustIndex = 0; dustIndex < 5; dustIndex++)
                        {
                            Dust dust = Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Circular(24f, 24f), DustID.Cloud, Main.rand.NextVector2Circular(2f, 2f) + projectile.velocity * MathHelper.Lerp(0.2f, 0.5f, Main.rand.NextFloat()));
                            dust.velocity += Main.rand.NextVector2Circular(0.5f, 0.5f);
                            dust.scale = 0.8f + Main.rand.NextFloat() * 0.5f;
                            dust.fadeIn = 1f;
                        }
                    }

                    if (projectile.ai[0] >= 5f + projectile.ai[2])
                        projectile.velocity.Y += 0.3f;

                    // Create a wave of rubble
                    // Make sure the projectile doesn't despawn before it starts going up
                    if (projectile.ai[0] <= projectile.ai[2])
                    {
                        projectile.Opacity = 0.4f;

                        projectile.timeLeft += 1;

                        // Use the expected velocity when the time is right
                        if (projectile.ai[0] == projectile.ai[2])
                        {
                            projectile.velocity *= 100f;
                            projectile.velocity *= (death ? 16f : 12f) + Main.rand.NextFloat() * 2f;
                        }
                    }
                    else
                        projectile.Opacity = 1f;

                    return false;
                }

                else if (projectile.type == ProjectileID.DemonSickle)
                {
                    if (Main.wofNPCIndex < 0 || !Main.npc[Main.wofNPCIndex].active || Main.npc[Main.wofNPCIndex].life <= 0 || projectile.tileCollide)
                        return true;

                    if (projectile.ai[0] == 0f)
                        SoundEngine.PlaySound(SoundID.Item8, projectile.Center);

                    projectile.rotation += projectile.direction * 0.8f;

                    projectile.ai[0] += 1f;
                    if (projectile.velocity.Length() < projectile.ai[1])
                    {
                        if (projectile.ai[0] >= 30f)
                            projectile.velocity *= 1.06f;
                    }

                    Vector2 vector11 = Main.player[Main.npc[Main.wofNPCIndex].target].Center - projectile.Center;
                    if (vector11.Length() < 10f)
                    {
                        projectile.Kill();
                        return false;
                    }

                    if (projectile.ai[0] < 210f)
                    {
                        float scaleFactor2 = projectile.velocity.Length();
                        vector11.Normalize();
                        vector11 *= scaleFactor2;
                        projectile.velocity = (projectile.velocity * 30f + vector11) / 31f;
                        projectile.velocity.Normalize();
                        projectile.velocity *= scaleFactor2;
                    }

                    for (int i = 0; i < 2; i++)
                    {
                        Dust shflame = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Shadowflame, 0f, 0f, 100);
                        shflame.noGravity = true;
                    }

                    return false;
                }

                else if (projectile.type == ProjectileID.QueenSlimeGelAttack)
                {
                    // Phase 1 and 2 projectiles do not bounce.
                    if (projectile.ai[1] == -2f)
                    {
                        if (projectile.alpha == 0 && Main.rand.NextBool(3))
                        {
                            Color newColor = NPC.AI_121_QueenSlime_GetDustColor();
                            newColor.A = 150;
                            int num72 = 8;
                            bool noGravity = Main.rand.NextBool();
                            Dust slime = Dust.NewDustDirect(projectile.position - new Vector2(num72, num72) + projectile.velocity, projectile.width + num72 * 2, projectile.height + num72 * 2, DustID.TintableDust, 0f, 0f, 50, newColor, 1.2f);
                            slime.velocity *= 0.3f;
                            slime.velocity += projectile.velocity * 0.3f;
                            slime.noGravity = noGravity;
                        }

                        projectile.alpha -= 50;
                        if (projectile.alpha < 0)
                            projectile.alpha = 0;

                        projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.05f;

                        projectile.velocity.Y += 0.1f;
                        if (projectile.velocity.Y > 16f)
                            projectile.velocity.Y = 16f;

                        if (Main.getGoodWorld && projectile.velocity.Length() > 4f)
                            projectile.velocity *= 0.985f;

                        return false;
                    }
                }

                else if (projectile.type == ProjectileID.QueenSlimeMinionBlueSpike)
                {
                    // When Queen Slime fires these they aren't as affected by gravity.
                    if (projectile.ai[1] < 0f)
                    {
                        if (projectile.frameCounter == 0)
                        {
                            projectile.frameCounter = 1;
                            projectile.frame = Main.rand.Next(3);
                        }

                        if (projectile.alpha == 0 && Main.rand.NextBool(3))
                        {
                            Color newColor = new Color(78, 136, 255, 150);
                            Dust slime = Dust.NewDustDirect(projectile.position + projectile.velocity, projectile.width, projectile.height, DustID.TintableDust, 0f, 0f, 50, newColor, 1.2f);
                            slime.velocity *= 0.3f;
                            slime.velocity += projectile.velocity * 0.3f;
                            slime.noGravity = true;
                        }

                        projectile.alpha -= 50;
                        if (projectile.alpha < 0)
                            projectile.alpha = 0;

                        if (projectile.ai[1] == -1f)
                        {
                            if (projectile.ai[0] >= 5f)
                                projectile.velocity.Y += 0.05f;
                            else
                                projectile.ai[0] += 1f;

                            if (projectile.velocity.Y > 16f)
                                projectile.velocity.Y = 16f;
                        }

                        projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;

                        return false;
                    }
                }

                else if (projectile.type == ProjectileID.RocketSkeleton && projectile.ai[1] >= 1f)
                {
                    bool primeCannonProjectile = projectile.ai[1] == 2f;
                    bool homeIn = false;
                    float homingTime = 140f;
                    float spreadOutCutoffTime = 510f;
                    float homeInCutoffTime = spreadOutCutoffTime - homingTime;
                    float minAcceleration = 0.08f;
                    float maxAcceleration = 0.12f;
                    float homingVelocity = 25f;
                    float maxVelocity = 15f;

                    if (!primeCannonProjectile)
                    {
                        if (projectile.timeLeft > homeInCutoffTime && projectile.timeLeft <= spreadOutCutoffTime)
                            homeIn = true;
                        else if (projectile.velocity.Length() < maxVelocity)
                            projectile.velocity *= 1.01f;
                    }
                    else
                    {
                        if (projectile.velocity.Length() < maxVelocity)
                            projectile.velocity *= 1.01f;
                    }

                    if (homeIn)
                    {
                        int playerIndex = (int)projectile.ai[0];
                        Vector2 velocity = projectile.velocity;
                        if (Main.player.IndexInRange(playerIndex))
                        {
                            Player player = Main.player[playerIndex];
                            velocity = projectile.DirectionTo(player.Center) * homingVelocity;
                        }

                        float amount = MathHelper.Lerp(minAcceleration, maxAcceleration, Utils.GetLerpValue(spreadOutCutoffTime, 30f, projectile.timeLeft, clamped: true));
                        projectile.velocity = Vector2.SmoothStep(projectile.velocity, velocity, amount);

                        // Stop homing when within a certain distance of the target
                        if (Vector2.Distance(projectile.Center, Main.player[playerIndex].Center) < 96f && projectile.timeLeft > homeInCutoffTime)
                            projectile.timeLeft = (int)homeInCutoffTime;
                    }

                    if (projectile.timeLeft <= 3)
                    {
                        projectile.position = projectile.Center;
                        projectile.width = projectile.height = 128;
                        projectile.position.X -= projectile.width / 2;
                        projectile.position.Y -= projectile.height / 2;
                    }

                    if (projectile.owner == Main.myPlayer && projectile.timeLeft <= 3)
                    {
                        projectile.tileCollide = false;
                        projectile.alpha = 255;
                    }
                    else
                    {
                        for (int n = 0; n < 2; n++)
                        {
                            float num22 = 0f;
                            float num23 = 0f;
                            if (n == 1)
                            {
                                num22 = projectile.velocity.X * 0.5f;
                                num23 = projectile.velocity.Y * 0.5f;
                            }

                            Dust fire = Dust.NewDustDirect(new Vector2(projectile.position.X + 3f + num22, projectile.position.Y + 3f + num23) - projectile.velocity * 0.5f, projectile.width - 8, projectile.height - 8, DustID.Torch, 0f, 0f, 100);
                            fire.scale *= 2f + Main.rand.Next(10) * 0.1f;
                            fire.velocity *= 0.2f;
                            fire.noGravity = true;

                            Dust smoke = Dust.NewDustDirect(new Vector2(projectile.position.X + 3f + num22, projectile.position.Y + 3f + num23) - projectile.velocity * 0.5f, projectile.width - 8, projectile.height - 8, DustID.Smoke, 0f, 0f, 100, default(Color), 0.5f);
                            smoke.fadeIn = 1f + Main.rand.Next(5) * 0.1f;
                            smoke.velocity *= 0.05f;
                        }
                    }

                    if (projectile.velocity != Vector2.Zero)
                        projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;

                    return false;
                }

                else if (projectile.type == ProjectileID.SeedPlantera || projectile.type == ProjectileID.PoisonSeedPlantera)
                {
                    // For Plantera's shotgun spread.
                    // Make the seeds faster until they hit the intended velocity to avoid unfair hits after charges.
                    if (projectile.ai[2] > 0f)
                    {
                        if (projectile.velocity.Length() < projectile.ai[2])
                            projectile.velocity *= 1.012f;
                    }

                    projectile.frameCounter++;
                    if (projectile.frameCounter > 1)
                    {
                        projectile.frameCounter = 0;
                        projectile.frame++;

                        if (projectile.frame > 1)
                            projectile.frame = 0;
                    }

                    if (projectile.ai[1] == 0f)
                    {
                        projectile.ai[1] = 1f;
                        SoundEngine.PlaySound(SoundID.Item17, projectile.Center);
                    }

                    if (projectile.alpha > 0)
                        projectile.alpha -= 30;
                    if (projectile.alpha < 0)
                        projectile.alpha = 0;

                    projectile.ai[0] += 1f;
                    if (projectile.ai[0] >= 120f)
                    {
                        if (projectile.velocity.Length() < 18f)
                            projectile.velocity *= 1.01f;
                    }

                    projectile.tileCollide = projectile.ai[0] >= 300f;

                    if (projectile.timeLeft > 600)
                        projectile.timeLeft = 600;

                    projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;

                    return false;
                }

                else if (projectile.type == ProjectileID.ThornBall && projectile.ai[2] != 0)
                {
                    projectile.tileCollide = false;
                    if (projectile.alpha > 0)
                    {
                        projectile.alpha -= 30;
                        if (projectile.alpha < 0)
                            projectile.alpha = 0;
                    }

                    Point point = projectile.Center.ToTileCoordinates();
                    Tile tileSafely = Framing.GetTileSafely(point);
                    bool stickOnCollision = tileSafely.HasUnactuatedTile && Main.tileSolid[tileSafely.TileType];
                    if (stickOnCollision)
                    {
                        projectile.velocity = Vector2.Zero;
                        projectile.ai[1] += 1f;
                        float explodeGateValue = 600f;
                        if (projectile.ai[1] >= explodeGateValue)
                        {
                            if (projectile.owner == Main.myPlayer)
                            {
                                int totalProjectiles = death ? 12 : 8;
                                float radians = MathHelper.TwoPi / totalProjectiles;
                                int type = ProjectileType<ThornBallSpike>();
                                float velocity = 1f;
                                Vector2 spinningPoint = new Vector2(0f, -velocity);
                                for (int k = 0; k < totalProjectiles; k++)
                                {
                                    Vector2 velocity2 = spinningPoint.RotatedBy(radians * k);
                                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center + Vector2.Normalize(velocity2) * 16f, velocity2, type, PlanteraAI.ThornBallSpikeDamage, 0f, Main.myPlayer);
                                }
                            }

                            SoundEngine.PlaySound(SoundID.Item17, projectile.Center);

                            for (int i = 0; i < 8; i++)
                            {
                                int randomDustType = Main.rand.NextBool() ? 125 : 148;
                                Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, randomDustType, 0f, 0f, 0, default, 2f);
                                dust.velocity *= 3f;
                                if (Main.rand.NextBool())
                                {
                                    dust.scale = 0.5f;
                                    dust.fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                                }
                            }
                            for (int i = 0; i < 10; i++)
                            {
                                int randomDustType = Main.rand.NextBool() ? 125 : 148;
                                Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, randomDustType, 0f, 0f, 0, default, 3f);
                                dust.noGravity = true;
                                dust.velocity *= 5f;
                                dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, randomDustType, 0f, 0f, 0, default, 2f);
                                dust.velocity *= 2f;
                            }

                            projectile.Kill();
                        }
                    }
                    else
                    {
                        int closestPlayer = Player.FindClosest(projectile.Center, 1, 1);
                        float homingSpeed = 7.5f + Vector2.Distance(Main.player[closestPlayer].Center, projectile.Center) * 0.01f;
                        Vector2 homingVelocity = Vector2.Normalize(Main.player[closestPlayer].Center - projectile.Center) * homingSpeed;
                        int inertia = 200;
                        projectile.velocity.X = (projectile.velocity.X * (inertia - 1) + homingVelocity.X) / inertia;

                        if (projectile.velocity.Length() > 16f)
                        {
                            projectile.velocity.Normalize();
                            projectile.velocity *= 16f;
                        }

                        projectile.ai[0] += 1f;
                        if (projectile.ai[0] > 15f)
                            projectile.velocity.Y += 0.1f;

                        projectile.rotation += projectile.velocity.X * 0.05f;

                        if (projectile.velocity.Y > 16f)
                            projectile.velocity.Y = 16f;
                    }

                    return false;
                }

                else if (projectile.type == ProjectileID.AncientDoomProjectile)
                {
                    if (projectile.velocity.Length() < 6f)
                        projectile.velocity *= 1.005f;
                }

                else if (projectile.type == ProjectileID.CultistBossIceMist)
                {
                    if (projectile.localAI[1] == 0f)
                    {
                        projectile.localAI[1] = 1f;
                        SoundEngine.PlaySound(SoundID.Item120, projectile.Center);
                    }

                    projectile.ai[0] += 1f;

                    // Main projectile
                    float duration = 300f;
                    if (projectile.ai[1] == 1f)
                    {
                        if (projectile.ai[0] >= duration - 20f)
                            projectile.alpha += 10;
                        else
                            projectile.alpha -= 10;

                        if (projectile.alpha < 0)
                            projectile.alpha = 0;
                        if (projectile.alpha > 255)
                            projectile.alpha = 255;

                        if (projectile.ai[0] >= duration)
                        {
                            projectile.Kill();
                            return false;
                        }

                        int num103 = Player.FindClosest(projectile.Center, 1, 1);
                        Vector2 vector11 = Main.player[num103].Center - projectile.Center;
                        float scaleFactor2 = projectile.velocity.Length();
                        vector11.Normalize();
                        if (vector11.HasNaNs())
                            vector11 = Vector2.Zero;
                        vector11 *= scaleFactor2;
                        projectile.velocity = (projectile.velocity * 20f + vector11) / 21f;
                        projectile.velocity.Normalize();
                        projectile.velocity *= scaleFactor2;
                        projectile.velocity *= 0.99f;


                        if (projectile.ai[0] % (death ? 20f : 30f) == 0f && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 vector50 = projectile.rotation.ToRotationVector2();
                            Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, vector50, projectile.type, projectile.damage, projectile.knockBack, projectile.owner);
                        }

                        projectile.rotation += MathHelper.Pi / 30f;

                        Lighting.AddLight(projectile.Center, 0.3f, 0.75f, 0.9f);

                        return false;
                    }

                    // Split projectiles
                    // MOST WACK ASS JANK FUCKING SHIT EVER
                    // WHAT THE FUCK
                    projectile.position -= projectile.velocity;

                    float splitProjectileDuration = duration - 255f;
                    if (projectile.ai[0] >= splitProjectileDuration - 5f)
                        projectile.alpha += 3;
                    else
                        projectile.alpha -= 40;

                    if (projectile.alpha < 0)
                        projectile.alpha = 0;
                    if (projectile.alpha > 255)
                        projectile.alpha = 255;

                    if (projectile.ai[0] >= splitProjectileDuration)
                    {
                        projectile.Kill();
                        return false;
                    }

                    Vector2 value39 = new Vector2(0f, -720f).RotatedBy(projectile.velocity.ToRotation());
                    float scaleFactor3 = projectile.ai[0] % splitProjectileDuration / splitProjectileDuration;
                    Vector2 spinningpoint13 = value39 * scaleFactor3;

                    for (int num724 = 0; num724 < 6; num724++)
                    {
                        Vector2 vector51 = projectile.Center + spinningpoint13.RotatedBy(num724 * MathHelper.TwoPi / 6f);

                        Lighting.AddLight(vector51, 0.3f, 0.75f, 0.9f);

                        for (int num725 = 0; num725 < 2; num725++)
                        {
                            Dust ice = Dust.NewDustDirect(vector51 + Utils.RandomVector2(Main.rand, -8f, 8f) / 2f, 8, 8, DustID.NorthPole, 0f, 0f, 100, Color.Transparent);
                            ice.noGravity = true;
                        }
                    }

                    return false;
                }

                // Change the stupid homing eyes
                else if (projectile.type == ProjectileID.PhantasmalEye)
                {
                    projectile.alpha -= 40;
                    if (projectile.alpha < 0)
                        projectile.alpha = 0;

                    if (projectile.ai[0] == 0f)
                    {
                        projectile.localAI[0] += 1f;
                        if (projectile.localAI[0] >= 45f)
                        {
                            projectile.localAI[0] = 0f;
                            projectile.ai[0] = 1f;
                            projectile.ai[1] = 0f - projectile.ai[1];
                            projectile.netUpdate = true;
                        }

                        projectile.velocity.X = projectile.velocity.RotatedBy(projectile.ai[1]).X;
                        projectile.velocity.X = MathHelper.Clamp(projectile.velocity.X, -6f, 6f);
                        projectile.velocity.Y -= 0.08f;

                        if (projectile.velocity.Y > 0f)
                            projectile.velocity.Y -= 0.2f;
                        if (projectile.velocity.Y < -7f)
                            projectile.velocity.Y = -7f;
                    }
                    else if (projectile.ai[0] == 1f)
                    {
                        projectile.localAI[0] += 1f;
                        if (projectile.localAI[0] >= 90f)
                        {
                            projectile.localAI[0] = 0f;
                            projectile.ai[0] = 2f;
                            projectile.ai[1] = Player.FindClosest(projectile.position, projectile.width, projectile.height);
                            projectile.netUpdate = true;
                        }

                        projectile.velocity.X = projectile.velocity.RotatedBy(projectile.ai[1]).X;
                        projectile.velocity.X = MathHelper.Clamp(projectile.velocity.X, -6f, 6f);
                        projectile.velocity.Y -= 0.08f;

                        if (projectile.velocity.Y > 0f)
                            projectile.velocity.Y -= 0.2f;
                        if (projectile.velocity.Y < -7f)
                            projectile.velocity.Y = -7f;
                    }
                    else if (projectile.ai[0] == 2f)
                    {
                        projectile.localAI[0] += 1f;
                        if (projectile.localAI[0] >= 45f)
                        {
                            projectile.localAI[0] = 0f;
                            projectile.ai[0] = 3f;
                            projectile.netUpdate = true;
                        }

                        Vector2 value23 = Main.player[(int)projectile.ai[1]].Center - projectile.Center;
                        value23.Normalize();
                        value23 *= 12f;
                        value23 = Vector2.Lerp(projectile.velocity, value23, 0.6f);

                        float num675 = 0.4f;
                        if (projectile.velocity.X < value23.X)
                        {
                            projectile.velocity.X += num675;
                            if (projectile.velocity.X < 0f && value23.X > 0f)
                                projectile.velocity.X += num675;
                        }
                        else if (projectile.velocity.X > value23.X)
                        {
                            projectile.velocity.X -= num675;
                            if (projectile.velocity.X > 0f && value23.X < 0f)
                                projectile.velocity.X -= num675;
                        }
                        if (projectile.velocity.Y < value23.Y)
                        {
                            projectile.velocity.Y += num675;
                            if (projectile.velocity.Y < 0f && value23.Y > 0f)
                                projectile.velocity.Y += num675;
                        }
                        else if (projectile.velocity.Y > value23.Y)
                        {
                            projectile.velocity.Y -= num675;
                            if (projectile.velocity.Y > 0f && value23.Y < 0f)
                                projectile.velocity.Y -= num675;
                        }
                    }
                    else if (projectile.ai[0] == 3f)
                    {
                        Vector2 value23 = Main.player[(int)projectile.ai[1]].Center - projectile.Center;
                        if (value23.Length() < 30f)
                        {
                            projectile.Kill();
                            return false;
                        }

                        float velocityLimit = (death ? 28f : 24f) / MathHelper.Clamp(projectile.ai[2] * 0.75f, 1f, 3f);
                        if (projectile.velocity.Length() < velocityLimit)
                            projectile.velocity *= 1.01f;
                    }

                    if (projectile.alpha < 40)
                    {
                        Dust dust = Dust.NewDustDirect(projectile.Center - Vector2.One * 5f, 10, 10, DustID.Vortex, (0f - projectile.velocity.X) / 3f, (0f - projectile.velocity.Y) / 3f, 150, Color.Transparent, 1.2f);
                        dust.noGravity = true;
                    }

                    projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

                    return false;
                }

                // Moon Lord big eye spheres
                else if (projectile.type == ProjectileID.PhantasmalSphere && Main.npc[(int)projectile.ai[1]].type == NPCID.MoonLordHand)
                {
                    float velocityLimit = death ? 14f : 12f;
                    if (projectile.velocity.Length() < velocityLimit)
                        projectile.velocity *= 1.0075f;

                    return true;
                }

                // Moon Lord leech tongue
                else if (projectile.type == ProjectileID.MoonLeech)
                {
                    Vector2 value35 = new Vector2(0f, 216f);
                    projectile.alpha -= 15;
                    if (projectile.alpha < 0)
                        projectile.alpha = 0;

                    int num738 = (int)Math.Abs(projectile.ai[0]) - 1;
                    int num739 = (int)projectile.ai[1];
                    if (!Main.npc[num738].active || Main.npc[num738].type != NPCID.MoonLordHead)
                    {
                        projectile.Kill();
                        return false;
                    }

                    projectile.localAI[0]++;
                    if (projectile.localAI[0] >= 330f && projectile.ai[0] > 0f && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        projectile.ai[0] *= -1f;
                        projectile.netUpdate = true;
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient && projectile.ai[0] > 0f && (!Main.player[(int)projectile.ai[1]].active || Main.player[(int)projectile.ai[1]].dead))
                    {
                        projectile.ai[0] *= -1f;
                        projectile.netUpdate = true;
                    }

                    projectile.rotation = (Main.npc[(int)Math.Abs(projectile.ai[0]) - 1].Center - Main.player[(int)projectile.ai[1]].Center + value35).ToRotation() + MathHelper.Pi / 2f;
                    if (projectile.ai[0] > 0f)
                    {
                        Vector2 value36 = Main.player[(int)projectile.ai[1]].Center - projectile.Center;
                        if (value36.X != 0f || value36.Y != 0f)
                            projectile.velocity = Vector2.Normalize(value36) * Math.Min(32f, value36.Length());
                        else
                            projectile.velocity = Vector2.Zero;

                        if (value36.Length() < 40f && projectile.localAI[1] == 0f)
                        {
                            projectile.localAI[1] = 1f;
                            int timeToAdd = 840;
                            if (Main.expertMode)
                                timeToAdd = 960;

                            if (!Main.player[num739].creativeGodMode)
                                Main.player[num739].AddBuff(BuffID.MoonLeech, timeToAdd);
                        }
                    }
                    else
                    {
                        Vector2 value37 = Main.npc[(int)Math.Abs(projectile.ai[0]) - 1].Center - projectile.Center + value35;
                        if (value37.X != 0f || value37.Y != 0f)
                            projectile.velocity = Vector2.Normalize(value37) * Math.Min(32f, value37.Length());
                        else
                            projectile.velocity = Vector2.Zero;

                        if (value37.Length() < 40f)
                            projectile.Kill();
                    }

                    return false;
                }

                // Moon Lord Deathray
                else if (projectile.type == ProjectileID.PhantasmalDeathray)
                {
                    if (Main.npc[(int)projectile.ai[1]].type == NPCID.MoonLordHead)
                    {
                        Vector2? vector78 = null;

                        if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
                            projectile.velocity = -Vector2.UnitY;

                        if (Main.npc[(int)projectile.ai[1]].active)
                        {
                            Vector2 value21 = new(27f, 59f);
                            Vector2 value22 = Utils.Vector2FromElipse(Main.npc[(int)projectile.ai[1]].localAI[0].ToRotationVector2(), value21 * Main.npc[(int)projectile.ai[1]].localAI[1]);
                            projectile.position = Main.npc[(int)projectile.ai[1]].Center + value22 - projectile.Size / 2f;
                        }

                        if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
                            projectile.velocity = -Vector2.UnitY;

                        if (projectile.localAI[0] == 0f)
                            SoundEngine.PlaySound(SoundID.Zombie104, projectile.Center);

                        float num801 = 1f;
                        projectile.localAI[0] += 1f;
                        if (projectile.localAI[0] >= 180f)
                        {
                            projectile.Kill();
                            return false;
                        }

                        projectile.scale = (float)Math.Sin(projectile.localAI[0] * MathHelper.Pi / 180f) * 10f * num801;
                        if (projectile.scale > num801)
                            projectile.scale = num801;

                        float num804 = projectile.velocity.ToRotation();
                        num804 += projectile.ai[0];
                        projectile.rotation = num804 - MathHelper.PiOver2;
                        projectile.velocity = num804.ToRotationVector2();

                        float num805 = 3f;
                        float num806 = projectile.width;

                        Vector2 samplingPoint = projectile.Center;
                        if (vector78.HasValue)
                        {
                            samplingPoint = vector78.Value;
                        }

                        float[] array3 = new float[(int)num805];
                        Collision.LaserScan(samplingPoint, projectile.velocity, num806 * projectile.scale, 2400f, array3);
                        float num807 = 0f;
                        int num3;
                        for (int num808 = 0; num808 < array3.Length; num808 = num3 + 1)
                        {
                            num807 += array3[num808];
                            num3 = num808;
                        }
                        num807 /= num805;

                        // Fire laser through walls at max length if target cannot be seen
                        if (!Collision.CanHitLine(Main.npc[(int)projectile.ai[1]].Center, 1, 1, Main.player[Main.npc[(int)projectile.ai[1]].target].Center, 1, 1) &&
                            Main.npc[(int)projectile.ai[1]].Calamity().newAI[0] == 1f)
                        {
                            num807 = 2400f;
                        }

                        float amount = 0.5f;
                        projectile.localAI[1] = MathHelper.Lerp(projectile.localAI[1], num807, amount);

                        Vector2 vector79 = projectile.Center + projectile.velocity * (projectile.localAI[1] - 14f);
                        for (int num809 = 0; num809 < 2; num809 = num3 + 1)
                        {
                            float num810 = projectile.velocity.ToRotation() + ((Main.rand.NextBool(2)) ? -1f : 1f) * MathHelper.PiOver2;
                            float num811 = (float)Main.rand.NextDouble() * 2f + 2f;
                            Vector2 vector80 = new((float)Math.Cos(num810) * num811, (float)Math.Sin(num810) * num811);
                            Dust dust = Dust.NewDustDirect(vector79, 0, 0, DustID.Vortex, vector80.X, vector80.Y);
                            dust.noGravity = true;
                            dust.scale = 1.7f;
                            num3 = num809;
                        }

                        if (Main.rand.NextBool(5))
                        {
                            Vector2 value29 = projectile.velocity.RotatedBy(MathHelper.PiOver2) * ((float)Main.rand.NextDouble() - 0.5f) * projectile.width;
                            Dust smoke = Dust.NewDustDirect(vector79 + value29 - Vector2.One * 4f, 8, 8, DustID.Smoke, 0f, 0f, 100, default, 1.5f);
                            smoke.velocity *= 0.5f;
                            smoke.velocity.Y = -Math.Abs(smoke.velocity.Y);
                        }

                        DelegateMethods.v3_1 = new Vector3(0.3f, 0.65f, 0.7f);
                        Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * projectile.localAI[1], projectile.width * projectile.scale, DelegateMethods.CastLight);

                        return false;
                    }
                }
            }

            return true;
        }
        #endregion

        #region Fishing Minigames
        // All these fields are exclusively used for fishing minigames, so I declared them in this region for organization
        /// <summary>
        /// Reel state of bobber.<br/>
        /// ai[0] in bobber AI.
        /// </summary>
        private float isReelingIn = 0;
        /// <summary>
        /// How long you have to catch a fish when bitten.<br/>
        /// ai[1] in bobber AI.
        /// </summary>
        private float CatchTime = 0;
        /// <summary>
        /// How long until a fish bites.<br/>
        /// LocalAI[1] in bobber AI.
        /// </summary>
        private float TimerToCatch = 0;
        /// <summary> Item ID of hooked item for fishing minigames. </summary>
        public int CaughtItemID = -1;
        /// <summary> What this does depends on the fishing minigame. Used to store data between updates. </summary>
        public float PersistentFishingData = -1;
        /// <summary> <inheritdoc cref="PersistentFishingData"/> </summary>
        public Vector2 PersistentFishingDataVector2 = Vector2.Zero;

        public bool RunFishingMinigames(Projectile projectile)
        {
            var owner = Main.player[projectile.owner];
            var cplayer = owner.Calamity();
            owner.Fishing_GetBait(out Item baitItem);
            bool reelingIn = projectile.ai[0] == 1;
            bool lineSnapped = projectile.ai[0] == 2;

            #region Utilities
            void SmallSplashAtOffset(Vector2 offset)
            {
                int num = Dust.NewDust(new Vector2(projectile.position.X - 6f, projectile.position.Y - 10f) + offset, projectile.width + 12, 24, Dust.dustWater());
                Main.dust[num].velocity.Y -= 4f;
                Main.dust[num].velocity.X *= 2.5f;
                Main.dust[num].scale = 0.8f;
                Main.dust[num].alpha = 100;
                Main.dust[num].noGravity = true;
            }
            void SmallDustAtOffset(Vector2 offset, int dustID)
            {
                int num = Dust.NewDust(new Vector2(projectile.position.X - 6f, projectile.position.Y - 10f) + offset, projectile.width + 12, 24, dustID);
                Main.dust[num].velocity.Y -= 4f;
                Main.dust[num].velocity.X *= 2.5f;
                Main.dust[num].scale = 0.8f;
                Main.dust[num].alpha = 100;
                Main.dust[num].noGravity = true;
            }
            void Splash()
            {
                for (int i = 0; i < 100; i++)
                {
                    int num = Dust.NewDust(new Vector2(projectile.position.X - 6f, projectile.position.Y - 10f), projectile.width + 12, 24, Dust.dustWater());
                    Main.dust[num].velocity.Y -= 4f;
                    Main.dust[num].velocity.X *= 2.5f;
                    Main.dust[num].scale = 0.8f;
                    Main.dust[num].alpha = 100;
                    Main.dust[num].noGravity = true;
                }

                SoundEngine.PlaySound(SoundID.SplashWeak, projectile.Center);
            }
            void ReelTheBobberChecks()
            {
                if (projectile.localAI[1] == 1)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.SpawnOnPlayer(owner.whoAmI, 370);
                    }
                    else
                    {
                        NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, -1, -1, null, owner.whoAmI, 370f);
                    }
                    projectile.ai[0] = 2f;
                }
                else if (projectile.localAI[1] < 1)
                {
                    Point point = default(Point);
                    point = new Point((int)projectile.position.X, (int)projectile.position.Y);
                    int num = (int)(0f - projectile.localAI[1]);
                    if (num == 618)
                    {
                        point.Y += 64;
                    }
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        NetMessage.SendData(MessageID.FishOutNPC, -1, -1, null, point.X / 16, point.Y / 16, num);
                    }
                    else
                    {
                        if (num == 682)
                        {
                            NPC.unlockedSlimeRedSpawn = true;
                        }
                        NPC.NewNPC(new EntitySource_FishedOut(owner), point.X, point.Y, num);
                        projectile.ai[0] = 2f;
                        WorldGen.CheckAchievement_RealEstateAndTownSlimes();
                    }
                }
                else if (Main.rand.NextBool(7) && !owner.accFishingLine)
                {
                    projectile.ai[0] = 2f;
                    projectile.ai[1] = 0;
                    projectile.localAI[1] = 0;
                }
                else
                {
                    projectile.ai[1] = projectile.localAI[1];
                }
                projectile.netUpdate = true;
            }

            Vector2 GetWaterLine()
            {
                Vector2 FoundWaterline = projectile.Center;
                var tilePos = projectile.Center.ToTileCoordinates();
                if (Main.tile[tilePos.X, tilePos.Y].LiquidAmount > 0)
                {
                    for (var i = 0; i < 1000; i++)
                    {
                        if (i <= 0 || i >= Main.maxTilesY || Main.tile[tilePos.X, tilePos.Y - i].LiquidAmount > 0)
                            continue;
                        FoundWaterline.Y = (tilePos.Y - i + 1) * 16f;
                        FoundWaterline.Y += (1 - Main.tile[tilePos.X, tilePos.Y - i + 1].LiquidAmount / 255f) * 16f;
                        break;
                    }
                }
                else
                {
                    for (var i = 0; i < 1000; i++)
                    {
                        if (i <= 0 || i >= Main.maxTilesY || Main.tile[tilePos.X, tilePos.Y + i].LiquidAmount == 0)
                            continue;
                        FoundWaterline.Y = (tilePos.Y + i) * 16f;
                        FoundWaterline.Y += (1 - Main.tile[tilePos.X, tilePos.Y + i].LiquidAmount / 255f) * 16f;
                        break;
                    }
                }
                return FoundWaterline;
            }
            #endregion
            //Doze's notes on bobber AI

            //ai[0] - Bobber state. 0: thrown/idle/bitten, 1: reeling in, 2: snapped
            //ai[1] - Amount of time to reel in the bobber once a bite happens. Is negative and counts up
            //When reeled in, this becomes the value of localAI[1] (ID of item/NPC)
            //At 0, the bobber is just floating waiting for a bite
            //localAI[1] - The timer for fish to try and bite the hook. When it exceeds 660, it resets to 0.
            //If ai[1] is not 0, this is set to the item ID of the hooked item/NPC
            //If hooking an NPC, this is set to the NPC ID but negative. Still need to find how this gets treated upon reeling in.

            switch (owner.Calamity().SelectedFishingMinigame)
            {
                case CalamityPlayer.FishingMinigames.WulfrumBobber:
                    {
                        if (CatchTime < 0 || (isReelingIn == 1 && CaughtItemID > 0))
                        {
                            owner.Calamity().ShouldHideControls = true;
                        }
                        if (isReelingIn != projectile.ai[0])
                            return false;

                        if (CatchTime < 0)
                            CatchTime++;
                        if (projectile.wet || (projectile.lavaWet && owner.accLavaFishing) || projectile.honeyWet)
                            TimerToCatch++;
                        var speedup = Math.Min(owner.Calamity().consecutiveCaughtFish * 5, 25);
                        var totalTime = 60 - speedup;
                        if (TimerToCatch + 90 >= 160 - owner.HeldItem.fishingPole && CatchTime >= 0)
                        {
                            // TODO: Implications of rand here?
                            if (PersistentFishingData == 0)
                                PersistentFishingData = Main.rand.NextBool() ? 1 : -1;
                            var timer = TimerToCatch - 160 - owner.HeldItem.fishingPole;

                            // TODO: Fine for others to see?
                            SmallSplashAtOffset(new Vector2(200 * PersistentFishingData * (timer / 90f), 0));
                        }
                        if (TimerToCatch >= 160 - owner.HeldItem.fishingPole && CatchTime >= 0)
                        {
                            PersistentFishingData = 0;
                            projectile.FishingCheck();
                            if (projectile.ai[1] < 0)
                            {
                                CatchTime = -totalTime * 4;
                                CaughtItemID = (int)projectile.localAI[1];
                            }
                            TimerToCatch = 0;
                        }
                        if (CatchTime < 0 && CatchTime % totalTime == 0)
                        {
                            // TODO: Fine for others to see?
                            Splash();
                            // TODO: Implications of rand here?
                            PersistentFishingData = Main.rand.Next(0, 4);

                            if (owner.whoAmI == Main.myPlayer)
                            {
                                var particle = new CustomSpark(projectile.Center, Vector2.UnitX.RotatedBy(PersistentFishingData * MathHelper.PiOver2) * 10, "CalamityMod/Particles/HighResHollowCircleHardEdgeAlt", false, 45 - speedup, 0.04f, Color.Blue, new(1, 2), shrinkSpeed: -0.2f);
                                GeneralParticleHandler.SpawnParticle(particle);
                            }
                        }
                        if (CatchTime < 0 && CatchTime % totalTime == -15)
                        {
                            var playerDir = -1;
                            if (owner.Calamity().pressedUp)
                            {
                                playerDir = 3;
                            }
                            if (owner.Calamity().pressedLeft)
                            {
                                playerDir = 2;
                            }
                            if (owner.Calamity().pressedDown)
                            {
                                playerDir = 1;
                            }
                            if (owner.Calamity().pressedRight)
                            {
                                playerDir = 0;
                            }
                            if (playerDir == PersistentFishingData)
                            {

                            }
                            else
                            {
                                if (owner.whoAmI == Main.myPlayer)
                                {
                                    if (playerDir >= 0)
                                    {
                                        var particle = new CustomSpark(projectile.Center, Vector2.UnitX.RotatedBy(playerDir * MathHelper.PiOver2) * 10, "CalamityMod/Particles/HighResHollowCircleHardEdgeAlt", false, 15, 0.04f, Color.Red, new(1, 2), shrinkSpeed: -0.2f);
                                        GeneralParticleHandler.SpawnParticle(particle);
                                    }
                                    else
                                    {
                                        var particle = new CustomSpark(projectile.Center, Vector2.Zero, "CalamityMod/Particles/HighResHollowCircleHardEdge", false, 15, 0.06f, Color.Red, Vector2.One);
                                        GeneralParticleHandler.SpawnParticle(particle);
                                    }
                                }
                                TimerToCatch = 0;
                                CatchTime = 0;
                                CaughtItemID = -1;
                                PersistentFishingData = 0;
                                owner.Calamity().consecutiveCaughtFish = 0;
                            }
                        }
                        if (CatchTime == -1)
                        {
                            projectile.localAI[1] = CaughtItemID;
                            ReelTheBobberChecks();
                            owner.Calamity().consecutiveCaughtFish++;
                            if (projectile.ai[0] == 2)
                                return false;
                            isReelingIn = 1;
                            CatchTime = (float)CaughtItemID;
                            TimerToCatch = 0;
                        }
                        if (projectile.ai[0] == 0)
                        {
                            projectile.ai[1] = CatchTime;
                            projectile.localAI[1] = TimerToCatch;
                            if (CaughtItemID != -1 && isReelingIn == 0)
                            {
                                projectile.localAI[1] = 0;
                                projectile.ai[1] = 0;
                            }
                            projectile.ai[0] = isReelingIn;
                        }
                        return false;
                    }


                case CalamityPlayer.FishingMinigames.VolcanicSinker:
                    {
                        if (PersistentFishingData == -1)
                        {
                            PersistentFishingData = 1;
                            TimerToCatch = 300;
                        }
                        var Waterline = GetWaterLine();
                        if (Waterline.Y >= projectile.Center.Y)
                        {
                            TimerToCatch = 300;
                            projectile.velocity.Y += 0.4f;
                        }
                        else if (projectile.ai[0] == 0)
                        {
                            owner.Calamity().ShouldHideControls = true;
                            if (owner.Calamity().pressedUp)
                            {
                                projectile.velocity.Y -= 0.125f;
                                projectile.netUpdate = true;
                            }

                            if (owner.Calamity().pressedDown)
                            {
                                projectile.velocity.Y += 0.125f;
                                projectile.netUpdate = true;
                            }

                            if (owner.Calamity().pressedLeft)
                            {
                                projectile.velocity.X -= 0.125f;
                                projectile.netUpdate = true;
                            }

                            if (owner.Calamity().pressedRight)
                            {
                                projectile.velocity.X += 0.125f;
                                projectile.netUpdate = true;
                            }
                        }
                        projectile.velocity *= 0.975f;

                        if (Waterline.Y <= projectile.Center.Y && projectile.ai[0] == 0 && !(projectile.lavaWet && !owner.accLavaFishing))
                        {
                            TimerToCatch -= Main.rand.Next(1, 5);
                            if (TimerToCatch <= 0)
                            {
                                projectile.FishingCheck();
                                if (projectile.ai[1] < 0)
                                {
                                    CaughtItemID = (int)projectile.localAI[1];
                                    projectile.ai[1] = 0;
                                    projectile.localAI[1] = 0;

                                    if (owner.whoAmI == Main.myPlayer)
                                    {
                                        for (var i = 0; i < 1000; i++)
                                        {
                                            var vectorToCheck = projectile.Center + new Vector2(Main.rand.Next(-200, 201), Main.rand.Next(-200, 201));
                                            var tileCoordsToCheck = vectorToCheck.ToSafeTileCoordinates();
                                            if (new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight).Contains((int)vectorToCheck.X, (int)vectorToCheck.Y) && !Main.tile[tileCoordsToCheck.X, tileCoordsToCheck.Y].IsTileSolid() && Main.tile[tileCoordsToCheck.X, tileCoordsToCheck.Y].LiquidAmount > 0)
                                            {
                                                if (projectile.localAI[2] >= 1)
                                                {
                                                    int customSonarText = (int)(projectile.localAI[2] - 1);
                                                    if (Main.popupText[customSonarText].sonar)
                                                    {
                                                        Main.popupText[customSonarText].position = vectorToCheck - FontAssets.MouseText.Value.MeasureString(Main.popupText[customSonarText].name) / 2f;
                                                    }
                                                }
                                                PersistentFishingDataVector2 = vectorToCheck;
                                                TimerToCatch = 600;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            if (PersistentFishingDataVector2 != Vector2.Zero && owner.whoAmI == Main.myPlayer)
                            {
                                Dust.NewDustPerfect(PersistentFishingDataVector2, DustID.Smoke);
                                if (Main.rand.NextBool(3))
                                {
                                    Dust.NewDustPerfect(PersistentFishingDataVector2, DustID.Torch);
                                }
                                var particle = new HeavySmokeParticle(PersistentFishingDataVector2, Vector2.One.RotatedByRandom(6.28) * Main.rand.NextFloat(1f, 4f), Color.Lerp(Color.DarkOrange, Color.DarkGray, Main.rand.NextFloat()), 15, 0.2f, 1);
                                GeneralParticleHandler.SpawnParticle(particle);
                            }

                            //When in lava, give the bobber a particle over it to show its location
                            if (projectile.lavaWet && owner.whoAmI == Main.myPlayer)
                            {
                                var particle = new CustomSpark(projectile.Center, Vector2.Zero, "CalamityMod/Particles/HighResHollowCircleHardEdge", false, 2, 0.01f, Color.DarkGray, Vector2.One);
                                GeneralParticleHandler.SpawnParticle(particle);
                            }
                            List<(Vector2, int)> validRifts = new();

                            foreach (var item in Main.ActiveProjectiles)
                            {
                                if (item.bobber && item.owner == projectile.owner && item.ai[0] == 0 && item.Calamity().PersistentFishingDataVector2 != Vector2.Zero)
                                {
                                    validRifts.Add((item.Calamity().PersistentFishingDataVector2, item.whoAmI));
                                }
                            }
                            foreach (var item in validRifts)
                            {
                                if (projectile.Distance(item.Item1) < 16)
                                {
                                    projectile.localAI[1] = Main.projectile[item.Item2].Calamity().CaughtItemID;
                                    ReelTheBobberChecks();
                                    if (projectile.ai[0] < 2)
                                        projectile.ai[0] = 1;
                                    Main.projectile[item.Item2].Calamity().PersistentFishingDataVector2 = PersistentFishingDataVector2;
                                    Main.projectile[item.Item2].Calamity().CaughtItemID = CaughtItemID;
                                    break;
                                }

                            }
                        }
                        if (projectile.ai[0] == 0)
                            return true;
                        break;
                    }


                case CalamityPlayer.FishingMinigames.FeralBobber:
                    if (projectile.ai[0] == 0)
                    {
                        if (projectile.wet) //Fishing in water
                        {

                            projectile.extraUpdates = 0;
                            if (cplayer.mouseRight && projectile.ai[1] == 0)
                            {
                                projectile.localAI[1] += 2;
                                owner.lifeRegenCount -= 60; // -0.5 health per tick
                            }
                        }
                        else //Attached to an enemy
                        {
                            if (PersistentFishingData >= 0 && Main.npc[(int)PersistentFishingData].active && projectile.Distance(Main.npc[(int)PersistentFishingData].Center + PersistentFishingDataVector2) < 128)
                            {
                                projectile.Center = Main.npc[(int)PersistentFishingData].Center + PersistentFishingDataVector2;
                                projectile.velocity = Vector2.Zero;

                                if (owner.Distance(Main.npc[(int)PersistentFishingData].Center) > 1450) // Slightly before when fishing lines stop rendering due to distance from target
                                {
                                    projectile.ai[0] = 1;
                                }

                                if (cplayer.mouseRight && projectile.ai[1] == 0 && owner.miscCounter % 10 == 0)
                                {
                                    owner.lifeRegenCount -= 600; // -5 health per 10 ticks

                                    // Diminishing returns on bait power beyond 75
                                    int baitPower = baitItem?.bait ?? 0;
                                    if (baitItem?.type == ItemID.TruffleWorm)
                                    {
                                        projectile.ai[0] = 1;
                                        projectile.localAI[1] = 1;
                                        ReelTheBobberChecks();
                                    }
                                    else if (baitItem?.type == ItemType<BloodwormItem>())
                                    {
                                        projectile.ai[0] = 1;
                                        projectile.localAI[1] = NPCType<OldDuke>() * -1;
                                        baitPower = BloodwormItem.SpoofBaitNumber; // Manually set the power to the displayed value
                                        ReelTheBobberChecks();
                                    }
                                    else if (baitPower > 75)
                                    {
                                        baitPower = (int)Math.Pow((baitPower - 75), 0.5f) + 75;
                                    }

                                    var projID = Projectile.NewProjectile(projectile.GetSource_FromThis(), Main.npc[(int)PersistentFishingData].Center, Vector2.Zero, ModContent.ProjectileType<DirectStrike>(), (int)owner.GetBestClassDamage().ApplyTo(owner.HeldItem.fishingPole + owner.fishingSkill + baitPower), 0f, owner.whoAmI, Main.npc[(int)PersistentFishingData].whoAmI);
                                    if (Main.projectile.IndexInRange(projID))
                                    {
                                        Main.projectile[projID].ArmorPenetration = 100; //This should ignore almost all armor
                                    }
                                }
                                projectile.timeLeft++;

                                projectile.extraUpdates = 0;
                                return true;
                            }
                            else if (PersistentFishingData >= 0)
                            {
                                PersistentFishingData = -1;
                                projectile.velocity.Y -= 3;
                            }
                            foreach (var item in Main.ActiveNPCs)
                            {
                                if (item.friendly || item.dontTakeDamage)
                                    continue;
                                if (projectile.Colliding(projectile.Hitbox, item.Hitbox))
                                {
                                    PersistentFishingDataVector2 = projectile.Center - item.Center;
                                    PersistentFishingData = item.whoAmI;
                                    break;
                                }
                            }
                            if (PersistentFishingData == -1)
                                projectile.extraUpdates = 1;
                            else
                                projectile.extraUpdates = 0;

                        }
                    }
                    else if (projectile.ai[0] == 1)
                        projectile.extraUpdates = 3;
                    break;


                case CalamityPlayer.FishingMinigames.SunkenSinker:
                    {
                        if (CatchTime < 0 || (isReelingIn == 1 && CaughtItemID > 0))
                        {
                            owner.Calamity().ShouldHideControls = true;
                        }
                        if (isReelingIn != projectile.ai[0])
                            return false;
                        var waterline = GetWaterLine();
                        var AdjustedOwnerWaterline = waterline;
                        if (AdjustedOwnerWaterline.Y < owner.Center.Y)
                            AdjustedOwnerWaterline.Y = owner.Center.Y;
                        if (AdjustedOwnerWaterline.Y >= projectile.Center.Y)
                        {
                            projectile.velocity.Y += 0.4f;
                        }
                        else
                        {
                            projectile.velocity.Y -= 0.1f;
                        }
                        projectile.velocity *= 0.975f;
                        if (CatchTime < 0)
                            CatchTime++;
                        if (projectile.wet || (projectile.lavaWet && owner.accLavaFishing) || projectile.honeyWet)
                            TimerToCatch++;
                        var speedup = 15;
                        var totalTime = 60 - speedup;
                        if (TimerToCatch + 90 >= 300 && CatchTime >= 0)
                        {
                            if (PersistentFishingData == 0)
                                PersistentFishingData = Main.rand.NextBool() ? 1 : -1;

                            if (owner.whoAmI == Main.myPlayer)
                            {
                                var timer = TimerToCatch - 300;
                                SmallSplashAtOffset(new Vector2(200 * PersistentFishingData * (timer / 90f), 0));
                                SmallDustAtOffset(new Vector2(200 * PersistentFishingData * (timer / 90f), 0), DustID.BlueCrystalShard);
                            }
                        }
                        if (TimerToCatch >= 300 && CatchTime >= 0)
                        {
                            PersistentFishingData = 0;
                            projectile.FishingCheck();
                            if (projectile.ai[1] < 0)
                            {
                                CatchTime = -totalTime * 4;
                                CaughtItemID = (int)projectile.localAI[1];
                            }
                            TimerToCatch = 0;
                        }
                        if (CatchTime < 0 && CatchTime % totalTime == 0 && owner.whoAmI == Main.myPlayer)
                        {
                            Splash();
                            PersistentFishingData = Main.rand.Next(0, 2);
                            var particle = new CustomSpark(projectile.Center, Vector2.UnitX.RotatedBy(PersistentFishingData * MathHelper.Pi) * 10, "CalamityMod/Particles/HighResHollowCircleHardEdgeAlt", false, 45 - speedup, 0.04f, Color.Blue, new(1, 2), shrinkSpeed: -0.2f);
                            GeneralParticleHandler.SpawnParticle(particle);
                        }
                        if (CatchTime < 0 && CatchTime % totalTime == -15)
                        {
                            var playerDir = -1;
                            if (owner.Calamity().pressedLeft)
                            {
                                playerDir = 1;
                            }
                            if (owner.Calamity().pressedRight)
                            {
                                playerDir = 0;
                            }
                            if (playerDir == PersistentFishingData)
                            {

                            }
                            else
                            {
                                if (owner.whoAmI == Main.myPlayer)
                                {
                                    if (playerDir >= 0)
                                    {
                                        var particle = new CustomSpark(projectile.Center, Vector2.UnitX.RotatedBy(playerDir * MathHelper.Pi) * 10, "CalamityMod/Particles/HighResHollowCircleHardEdgeAlt", false, 15, 0.04f, Color.Red, new(1, 2), shrinkSpeed: -0.2f);
                                        GeneralParticleHandler.SpawnParticle(particle);
                                    }
                                    else
                                    {
                                        var particle = new CustomSpark(projectile.Center, Vector2.Zero, "CalamityMod/Particles/HighResHollowCircleHardEdge", false, 15, 0.06f, Color.Red, Vector2.One);
                                        GeneralParticleHandler.SpawnParticle(particle);
                                    }
                                }
                                TimerToCatch = 0;
                                CatchTime = 0;
                                CaughtItemID = -1;
                                PersistentFishingData = 0;
                                owner.Calamity().consecutiveCaughtFish = 0;
                            }
                        }
                        if (CatchTime == -1)
                        {
                            projectile.localAI[1] = CaughtItemID;
                            ReelTheBobberChecks();
                            owner.Calamity().consecutiveCaughtFish++;
                            if (projectile.ai[0] == 2)
                                return false;
                            isReelingIn = 1;
                            CatchTime = (float)CaughtItemID;
                            TimerToCatch = 0;
                        }
                        if (projectile.ai[0] == 0)
                        {


                            projectile.ai[1] = CatchTime;
                            projectile.localAI[1] = TimerToCatch;
                            if (CaughtItemID != -1 && isReelingIn == 0)
                            {
                                projectile.localAI[1] = 0;
                                projectile.ai[1] = 0;
                            }
                            projectile.ai[0] = isReelingIn;
                        }
                        if (projectile.ai[0] == 0)
                            return true;
                        break;
                    }


                case CalamityPlayer.FishingMinigames.DevourerofCods: //Technically not a fishing minigame, but the code is already all here
                    var fishToEat = TheDevourerofCods.FishToEat;
                    if (projectile.ai[1] < -1)
                    {
                        //Coonsuming a fish provides/increases the player's food buffs.
                        if (fishToEat.Contains((int)projectile.localAI[1]))
                        {
                            projectile.ai[1] = 0;
                            projectile.localAI[1] = 0;
                            SoundEngine.PlaySound(SoundID.Item2, projectile.Center);
                            //If the player doesn't have Ex. Stuffed, either give them Plenty Satisfied or inrease existing Plenty Satisfied duration
                            if (!owner.HasBuff(BuffID.WellFed3))
                            {

                                if (owner.HasBuff(BuffID.WellFed2))
                                {
                                    var bIndex = owner.FindBuffIndex(BuffID.WellFed2);
                                    if (owner.buffTime[bIndex] < CalamityUtils.MinutesToFrames(24))
                                    {
                                        owner.buffTime[bIndex] += 300;
                                    }
                                }
                                else
                                {

                                    owner.AddBuff(BuffID.WellFed2, 300);
                                }
                            }
                            else //If the player DOES have Ex. Stuffed, increase the duration of it
                            {
                                var bIndex = owner.FindBuffIndex(BuffID.WellFed3);
                                if (owner.buffTime[bIndex] < CalamityUtils.MinutesToFrames(24)) //Requires fishing once per day if you'd like to keep free ExStuffed forever
                                {
                                    owner.buffTime[bIndex] += 300;
                                }
                            }
                        }
                        else //If the hooked item isn't consumable, make it stay hooked forever.
                        {
                            projectile.ai[1] = -30;
                        }
                    }

                    return false;


                case CalamityPlayer.FishingMinigames.AcrobaticBobber:
                    {
                        if (CatchTime < 0 || (isReelingIn == 1 && CaughtItemID > 0))
                        {
                            owner.Calamity().ShouldHideControls = true;
                        }
                        if (PersistentFishingData == -1)
                        {
                            PersistentFishingData = 1;
                            TimerToCatch = 300;
                        }
                        var Waterline = GetWaterLine();
                        if (Waterline.Y >= projectile.Center.Y)
                        {
                            projectile.velocity.Y += 0.4f;
                        }
                        else if (projectile.ai[0] <= 0)
                        {
                            projectile.velocity.Y -= 0.1f;
                        }
                        if (projectile.ai[0] <= 1)
                        {
                            owner.Calamity().ShouldHideControls = true;
                        }
                        projectile.velocity *= 0.975f;
                        if (projectile.wet)
                            projectile.velocity *= 0.99f;

                        //When in lava, give the bobber a particle over it to show its location
                        if (projectile.lavaWet && owner.whoAmI == Main.myPlayer)
                        {
                            var particle = new CustomSpark(projectile.Center, Vector2.Zero, "CalamityMod/Particles/HighResHollowCircleHardEdge", false, 2, 0.01f, Color.DarkGray, Vector2.One);
                            GeneralParticleHandler.SpawnParticle(particle);
                        }
                        if (projectile.ai[0] == 0)
                        {

                            if (owner.Calamity().pressedUp && projectile.velocity.Y < 0)
                            {
                                projectile.velocity.Y -= 0.3f;
                                projectile.netUpdate = true;
                            }

                            if (owner.Calamity().pressedUp && Waterline.Y < projectile.Center.Y)
                            {
                                projectile.velocity.Y -= 0.3f;
                                projectile.netUpdate = true;
                            }

                            if (owner.Calamity().pressedDown)
                            {
                                projectile.velocity.Y += 0.25f;
                                projectile.netUpdate = true;
                            }

                            if (owner.Calamity().pressedLeft)
                            {
                                projectile.velocity.X -= 0.25f;
                                projectile.netUpdate = true;
                            }

                            if (owner.Calamity().pressedRight)
                            {
                                projectile.velocity.X += 0.25f;
                                projectile.netUpdate = true;
                            }

                            if (Waterline.Y <= projectile.Center.Y - 8)
                                TimerToCatch -= Main.rand.Next(1, 5);
                            if (TimerToCatch <= 0)
                            {
                                projectile.FishingCheck();
                                var fishingCond = Main.player[projectile.owner].GetFishingConditions();
                                bool canLavaFish = ItemID.Sets.CanFishInLava[fishingCond.PoleItemType] || ItemID.Sets.IsLavaBait[fishingCond.BaitItemType] || Main.player[projectile.owner].accLavaFishing;
                                if (projectile.ai[1] < 0 && !(projectile.lavaWet && !canLavaFish))
                                {
                                    CaughtItemID = (int)projectile.localAI[1];
                                    projectile.ai[1] = 0;
                                    projectile.localAI[1] = 0;
                                    for (var i = 0; i < 1000; i++)
                                    {
                                        var vectorToCheck = Waterline + new Vector2(Main.rand.Next(-200, 201), Main.rand.Next(-200, 32));
                                        var tileCoordsToCheck = vectorToCheck.ToSafeTileCoordinates();
                                        if (new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight).Contains((int)vectorToCheck.X, (int)vectorToCheck.Y) && !Main.tile[tileCoordsToCheck.X, tileCoordsToCheck.Y].IsTileSolid() && Main.tile[tileCoordsToCheck.X, tileCoordsToCheck.Y].LiquidAmount == 0)
                                        {
                                            if (projectile.localAI[2] >= 1)
                                            {
                                                int customSonarText = (int)(projectile.localAI[2] - 1);
                                                if (Main.popupText[customSonarText].sonar)
                                                {
                                                    Main.popupText[customSonarText].position = vectorToCheck - FontAssets.MouseText.Value.MeasureString(Main.popupText[customSonarText].name) / 2f;
                                                }
                                            }
                                            PersistentFishingDataVector2 = vectorToCheck;
                                            TimerToCatch = 600;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (PersistentFishingDataVector2 != Vector2.Zero && owner.miscCounter % 15 == 0 && owner.whoAmI == Main.myPlayer)
                            {

                                var particle = new CustomSpark(PersistentFishingDataVector2, Vector2.Zero, "CalamityMod/Particles/HighResHollowCircleHardEdge", false, 25, 0.02f, Color.Green, Vector2.One);
                                GeneralParticleHandler.SpawnParticle(particle);
                            }
                            List<(Vector2, int)> validRifts = new();

                            foreach (var item in Main.ActiveProjectiles)
                            {
                                if (item.bobber && item.owner == projectile.owner && item.ai[0] == 0 && item.Calamity().PersistentFishingDataVector2 != Vector2.Zero)
                                {
                                    validRifts.Add((item.Calamity().PersistentFishingDataVector2, item.whoAmI));
                                }
                            }
                            foreach (var item in validRifts)
                            {
                                if (projectile.Distance(item.Item1) < 16)
                                {
                                    projectile.localAI[1] = Main.projectile[item.Item2].Calamity().CaughtItemID;
                                    projectile.ai[0] = -1;
                                    Main.projectile[item.Item2].Calamity().PersistentFishingDataVector2 = PersistentFishingDataVector2;
                                    Main.projectile[item.Item2].Calamity().CaughtItemID = CaughtItemID;
                                    break;
                                }

                            }
                        }
                        if (projectile.ai[0] == -1 && (Waterline.Y <= projectile.Center.Y || projectile.velocity.Length() < 0.5f))
                        {
                            ReelTheBobberChecks();
                            if (projectile.ai[0] < 2)
                                projectile.ai[0] = 1;
                        }
                        if (projectile.ai[0] == 0)
                            return true;
                        break;
                    }

            }

            bool alreadyJunk = projectile.ai[1] == ItemID.OldShoe || projectile.ai[1] == ItemID.FishingSeaweed || projectile.ai[1] == ItemID.TinCan;
            // When using rage bait, gives a chance that your items turn to junk
            if (baitItem?.type == ModContent.ItemType<RageBait>() && reelingIn && !alreadyJunk && Main.rand.NextBool(RageBait.junkChance.Item1, RageBait.junkChance.Item2) && projectile.localAI[1] != 0)
            {
                var junk = Main.rand.Next(3) switch
                {
                    0 => ItemID.OldShoe,
                    1 => ItemID.FishingSeaweed,
                    _ => ItemID.TinCan,
                };

                SoundStyle epicFail = new("CalamityMod/Sounds/Item/Swine", 2);
                SoundEngine.PlaySound(epicFail with { Volume = 0.9f, Pitch = 0.3f }, projectile.Center);
                if (CalamityWorld.revenge)
                    owner.Calamity().rage += 25;

                for (int i = 0; i < 12; i++)
                {
                    Particle spray = new CustomSpark(projectile.Center + Vector2.UnitX * Main.rand.NextFloat(-10, 10), -Vector2.UnitY * Main.rand.NextFloat(3, 8), "CalamityMod/Particles/BloomCircle", true, Main.rand.Next(18, 24) * 5, Main.rand.NextFloat(0.3f, 0.5f), Color.Lerp(Color.Gray, Color.White, Main.rand.NextFloat(0, 0.7f)) * 0.3f, new Vector2(0.8f, 1f), true, false, 0, false, false);
                    GeneralParticleHandler.SpawnParticle(spray);

                    if (i % 2 == 0)
                    {
                        Particle rage = new CustomSpark(owner.Center + Vector2.UnitX * Main.rand.NextFloat(-10, 10) + -Vector2.UnitY * 7.5f, -Vector2.UnitY * Main.rand.NextFloat(0.5f, 2f), "CalamityMod/Particles/BloomCircle", false, Main.rand.Next(12, 18) * 4, Main.rand.NextFloat(0.3f, 0.5f), Color.Lerp(Color.Crimson, Color.Red, Main.rand.NextFloat(0, 0.7f)) * 0.7f, new Vector2(0.7f, 1f), true, false, 0, false, false);
                        GeneralParticleHandler.SpawnParticle(rage);
                    }
                }

                projectile.ai[1] = junk;
                return false;
            }

            bool pulledNPC = Main.rand.NextBool(TrustyOldRod.enemyChance.Item1, TrustyOldRod.enemyChance.Item2);

            if (owner.HeldItem.type == ModContent.ItemType<TrustyOldRod>() && reelingIn && projectile.ai[1] != 0 && projectile.localAI[1] != 0 && pulledNPC)
            {
                // Remove any item you would have fished up
                projectile.ai[1] = 0;
                // Get rarity of pull
                int rarity;
                int roll = Main.rand.Next(1, 100 + 1);
                if (roll == 1) rarity = 3; // UltraRare (1/100)
                else if (roll <= 21) rarity = 2; // Rare ~(1/5)
                else rarity = 1; // Common ~(4/5)

                if (Main.netMode == NetmodeID.MultiplayerClient && Main.myPlayer == owner.whoAmI)
                    TrustyOldRodEnemyPacket.Send(owner, projectile.identity, rarity, projectile.lavaWet, projectile.honeyWet);
                else if(Main.netMode == NetmodeID.SinglePlayer)
                    TrustyOldRodEnemySystem.SpawnTrustyOldRodNPC(owner, projectile.identity, rarity, projectile.lavaWet, projectile.honeyWet);

                TrustyOldRodEnemySystem.DoTrustyOldRodVFX(owner, projectile.identity, rarity, projectile.lavaWet, projectile.honeyWet);
                projectile.ai[0] = 2; // snap line
                return false;
            }
            return false;
        }
        #endregion

        #region AI
        public override void AI(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            if (!frameOneHacksExecuted)
            {
                if (projectile.hostile)
                {
                    // Reduce Nail damage from Nailheads because they're stupid
                    if (projectile.type == ProjectileID.Nail && Main.expertMode)
                        projectile.damage /= 2;

                    // Nerf several Hardmode enemy projectiles because they deal way too much damage
                    if (!CalamityPlayer.areThereAnyDamnBosses)
                    {
                        switch (projectile.type)
                        {
                            case ProjectileID.JavelinHostile:
                            case ProjectileID.PinkLaser:
                            case ProjectileID.FrostBlastHostile:
                            case ProjectileID.GoldenShowerHostile:
                            case ProjectileID.RainNimbus:
                            case ProjectileID.FlamingArrow:
                            case ProjectileID.BulletDeadeye:
                            case ProjectileID.CannonballHostile:
                            case ProjectileID.UnholyTridentHostile:
                            case ProjectileID.FrostBeam:
                            case ProjectileID.CursedFlameHostile:
                            case ProjectileID.Stinger:
                            case ProjectileID.BloodShot:
                            case ProjectileID.BloodNautilusShot:
                            case ProjectileID.BloodNautilusTears:
                            case ProjectileID.RockGolemRock:
                            case ProjectileID.IcewaterSpit:
                            case ProjectileID.RocketSkeleton:
                            case ProjectileID.SniperBullet:
                            case ProjectileID.DrManFlyFlask:
                                projectile.damage = (int)Math.Round(projectile.damage * 0.65);
                                break;
                        }
                    }
                }
                else
                {
                    // 13NOV2024: Ozzatron: Removed Deadshot Brooch extra updates.
                    // This mechanic was fundamentally unacceptable and was an invisible controlling hand that affected the development of dozens of ranged weapons.
                    // This has many knock-on effects going forward with many many ranged weapons, and will be a pain to test and correct for.
                    // I don't care. This effect had to be removed.
                    //

                    if ((projectile.minion || ProjectileID.Sets.MinionShot[projectile.type] || projectile.sentry || ProjectileID.Sets.SentryShot[projectile.type]) && (player.ownedProjectileCounts[ModContent.ProjectileType<RelicOfDeliveranceSpear>()] > 0 || player.ownedProjectileCounts[ModContent.ProjectileType<RelicOfConvergenceCrystal>()] > 0))
                        projectile.damage = (int)(projectile.damage * 0.1);

                    if (projectile.CountsAsClass<RogueDamageClass>() && stealthStrike)
                    {
                        int gloveArmorPenAmt = modPlayer.nanotech ? 15 : 8;
                        if (modPlayer.filthyGlove || modPlayer.bloodyGlove)
                            projectile.ArmorPenetration += gloveArmorPenAmt;
                    }
                }

                if (NPC.downedMoonlord)
                {
                    if (CalamityProjectileSets.IsBuffedDungeonProjectile[projectile.type])
                    {
                        // ai[1] being set to 1 is done only by the Calamity usages of these projectiles in Skeletron and Skeletron Prime boss fights
                        bool isSkeletronBossProjectile = (projectile.type == ProjectileID.RocketSkeleton || projectile.type == ProjectileID.Shadowflames) && projectile.ai[1] > 0f;

                        if (!isSkeletronBossProjectile)
                            projectile.damage += 30;
                    }
                }

                if (DownedBossSystem.downedDoG && (Main.pumpkinMoon || Main.snowMoon || Main.eclipse))
                {
                    if (CalamityProjectileSets.IsBuffedEventProjectile[projectile.type])
                        projectile.damage += 15;
                }

                frameOneHacksExecuted = true;
            }

            // Setting this in SetDefaults didn't work
            switch (projectile.type)
            {
                // GFB traps, boulders and explosives go hard
                case ProjectileID.SpikyBallTrap:
                case ProjectileID.FlamethrowerTrap:
                case ProjectileID.SpearTrap:
                case ProjectileID.PoisonDartTrap:
                case ProjectileID.VenomDartTrap:
                case ProjectileID.Dynamite:
                case ProjectileID.BouncyDynamite:
                case ProjectileID.StickyDynamite:
                case ProjectileID.Bomb:
                case ProjectileID.BombFish:
                case ProjectileID.BouncyBomb:
                case ProjectileID.DirtBomb:
                case ProjectileID.DirtStickyBomb:
                case ProjectileID.DryBomb:
                case ProjectileID.HoneyBomb:
                case ProjectileID.LavaBomb:
                case ProjectileID.ScarabBomb:
                case ProjectileID.StickyBomb:
                case ProjectileID.WetBomb:
                case ProjectileID.Boulder:
                case ProjectileID.MiniBoulder:
                case ProjectileID.BouncyBoulder:
                case ProjectileID.LifeCrystalBoulder:
                case ProjectileID.MoonBoulder:
                    projectile.extraUpdates = Main.zenithWorld ? 1 : 0;
                    break;

                case ProjectileID.RockGolemRock:
                case ProjectileID.BloodShot:
                case ProjectileID.DandelionSeed:
                    projectile.extraUpdates = CalamityWorld.revenge ? 1 : 0;
                    break;

                case ProjectileID.Bee:
                case ProjectileID.Wasp:
                case ProjectileID.TinyEater:
                case ProjectileID.GiantBee:
                case ProjectileID.Bat:
                    projectile.extraUpdates = 1;
                    break;
            }

            // Makes flares from Firestorm Cannon ignore gravity
            // Spectralstorm Cannon uses a custom flare projectile
            if (projectile.aiStyle == ProjAIStyleID.Flare && projectile.ai[2] == 1f && projectile.localAI[0] == 0f)
                projectile.localAI[1]--;

            // Hack to allow Desert Tiger minion to fall through platforms while attacking
            if (projectile.type >= ProjectileID.StormTigerTier1 && projectile.type <= ProjectileID.StormTigerTier3)
            {
                if (projectile.ai[0] == 5f)
                    projectile.tileCollide = false;
            }

            // Jack O Lantern Launcher projectile tweak
            if (projectile.type == ProjectileID.JackOLantern)
            {
                if (projectile.ai[0] >= 20f)
                {
                    // Offset the gravity until 30 frames later
                    projectile.ai[2]++;
                    if (projectile.ai[2] < 30f)
                        projectile.velocity.Y -= 0.5f;
                }
            }

            #region Flail Extendo Hacks
            // Very hacky solution for making Dao of Pow's flail throw travel faster
            if (projectile.type == ProjectileID.TheDaoofPow)
            {
                // Guide to ai[0]:
                // 0: Spinning. 1: Being thrown. 2: Return after throw. 6: Dropped on ground. 4: Return after being dropped.
                if (projectile.ai[0] == 1f)
                {
                    if (projectile.ai[1] == 0f)
                        projectile.velocity *= 1.33f;
                }
            }
            // Similar hacky solution for The Meatball
            if (projectile.type == ProjectileID.TheMeatball)
            {
                if (projectile.ai[0] == 1f)
                {
                    if (projectile.ai[1] > 0f)
                    {
                        projectile.ai[2]++;
                        if (projectile.ai[2] <= 7f) // When ai[1] reaches 14, it starts returning, so this makes it take an extra 7 frames to return
                            projectile.ai[1]--;
                    }
                }
            }
            // And Flower Pow!
            if (projectile.type == ProjectileID.FlowerPow)
            {
                if (projectile.ai[0] == 1f)
                {
                    if (projectile.ai[1] == 0f)
                        projectile.velocity *= 1.25f;
                }
                // Return speed is also increased using an IL edit, because why does it have a slower return speed than Dao of Pow???
            }
            #endregion

            // True Night's Edge projectiles instantly start with max velocity
            if (projectile.type == ProjectileID.TrueNightsEdge)
            {
                if (projectile.localAI[1] < 32f)
                    projectile.localAI[1] = 32f;
            }

            // Random velocities for Bouncy Boulders in GFB
            if (projectile.type == ProjectileID.BouncyBoulder && Main.zenithWorld)
            {
                // 5% chance every frame to get a random velocity multiplier (this is actually rolled twice per frame, due to the extra update in GFB)
                if (Main.rand.Next(100) >= 95)
                    projectile.velocity *= Main.rand.NextFloat(0.9f, 1.25f);
            }

            // Prevents them from being affected by gravity
            if (projectile.type == ProjectileID.QueenBeeStinger)
                projectile.ai[0]--;

            // Acceleration for certain lasers
            if ((projectile.type == ProjectileID.EyeLaser || projectile.type == ProjectileID.DeathLaser) && projectile.ai[0] == 1f)
            {
                if (projectile.velocity.Length() < AcceleratingBossLaserVelocityCap)
                    projectile.velocity *= 1.0025f;
            }

            // Sharknado is more translucent before dealing damage
            if (projectile.type == ProjectileID.Sharknado)
            {
                if (projectile.timeLeft > FishronCthulhunadoTotalDuration - FishronTornadoTimeBeforeDealingDamage)
                {
                    if (projectile.alpha < 200)
                        projectile.alpha = 200;
                    if (projectile.alpha > 220)
                        projectile.alpha = 220;
                }
            }

            // Accelerate for 1.5 seconds to full velocity
            if (projectile.type == ProjectileID.HallowBossLastingRainbow && (CalamityWorld.revenge || BossRushEvent.BossRushActive))
            {
                int spreadOutTime = 90;
                if (projectile.timeLeft > EmpressLastingRainbowTotalDuration - spreadOutTime)
                    projectile.velocity *= 1.015525f;
            }

            // Zapinator lasers cannot trigger their damage multiplier more than once
            if (projectile.type == ProjectileID.ZapinatorLaser && projectile.damage > projectile.originalDamage)
            {
                if (projectile.ai[0] == 0f)
                {
                    projectile.originalDamage = projectile.damage;
                    projectile.ai[0] = 1f;
                }
                else
                {
                    projectile.damage = projectile.originalDamage;
                }
            }

            // Golf Balls go nyoom on touching Auric Ore/Repulsers
            if (ProjectileID.Sets.IsAGolfBall[projectile.type])
            {
                int auricOreID = TileType<AuricOre>();
                int auricRepulserID = TileType<AuricRepulserPanelTile>();
                // Get a list of tiles that are colliding with the ball.
                // This is just Collision.GetEntityTiles but with a larger detection square because golf balls are too small and dumb to get detected half the time apparently
                List<Point> EdgeTiles = new List<Point>();
                int extraDist = 8;
                int left = (int)projectile.position.X - extraDist;
                int up = (int)projectile.position.Y - extraDist;
                int right = (int)projectile.Right.X + extraDist;
                int down = (int)projectile.Bottom.Y + extraDist;
                if (left % 16 == 0)
                {
                    left--;
                }

                if (up % 16 == 0)
                {
                    up--;
                }

                if (right % 16 == 0)
                {
                    right++;
                }

                if (down % 16 == 0)
                {
                    down++;
                }

                int width = right / 16 - left / 16;
                int height = down / 16 - up / 16;
                left /= 16;
                up /= 16;
                for (int i = left; i <= left + width; i++)
                {
                    EdgeTiles.Add(new Point(i, up));
                    EdgeTiles.Add(new Point(i, up + height));
                }

                for (int j = up; j < up + height; j++)
                {
                    EdgeTiles.Add(new Point(left, j));
                    EdgeTiles.Add(new Point(left + width, j));
                }
                foreach (Point touchedTile in EdgeTiles)
                {
                    Tile tile = Framing.GetTileSafely(touchedTile);
                    if (!tile.HasTile || !tile.HasUnactuatedTile)
                        continue;
                    if (tile.TileType == auricOreID || tile.TileType == auricRepulserID)
                    {
                        // Force Auric Ore to animate with its crackling electricity
                        if (tile.TileType == auricOreID)
                        {
                            AuricOre.Animate = true;
                        }

                        var yeetVec = Vector2.Normalize(projectile.Center - touchedTile.ToWorldCoordinates());
                        projectile.velocity += yeetVec * 40f;
                        SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/ExoMechs/TeslaShoot1") with { Pitch = 0.4f });
                    }
                }
            }

            if (!projectile.npcProj && !projectile.trap && projectile.friendly && projectile.damage > 0)
            {
                if (modPlayer.fungalSymbiote && player.HasBuff(BuffType<Mushy>()))
                {
                    if (Main.player[projectile.owner].miscCounter % 6 == 0 && projectile.FinalExtraUpdate())
                    {
                        if (projectile.owner == Main.myPlayer && player.ownedProjectileCounts[ProjectileID.Mushroom] < 15)
                        {
                            //Note: these don't count as true melee anymore but its useful code to keep around
                            if (projectile.type == ProjectileType<NebulashFlail>() || projectile.type == ProjectileType<CosmicDischargeFlail>() ||
                                projectile.type == ProjectileType<MourningstarFlail>() || projectile.type == ProjectileID.SolarWhipSword)
                            {
                                Vector2 vector24 = Main.OffsetsPlayerOnhand[Main.player[projectile.owner].bodyFrame.Y / 56] * 2f;
                                if (player.direction != 1)
                                {
                                    vector24.X = player.bodyFrame.Width - vector24.X;
                                }
                                if (player.gravDir != 1f)
                                {
                                    vector24.Y = player.bodyFrame.Height - vector24.Y;
                                }
                                vector24 -= new Vector2(player.bodyFrame.Width - player.width, player.bodyFrame.Height - 42) / 2f;
                                Vector2 newCenter = player.RotatedRelativePoint(player.position + vector24, true) + projectile.velocity;
                                Projectile.NewProjectile(projectile.GetSource_FromThis(), newCenter, Vector2.Zero, ProjectileID.Mushroom, 0, 0f, projectile.owner);
                            }
                            else
                            {
                                Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Vector2.Zero, ProjectileID.Mushroom, 0, 0f, projectile.owner);
                            }
                        }
                    }
                }

                if (projectile.CountsAsClass<RogueDamageClass>())
                {
                    if (!LocketClone && !CannotProc)
                    {
                        if (modPlayer.nanotech)
                        {
                            if (Main.player[projectile.owner].miscCounter % 30 == 0 && projectile.FinalExtraUpdate())
                            {
                                if (projectile.owner == Main.myPlayer && player.ownedProjectileCounts[ProjectileType<NanotechProjectile>()] < 5)
                                {
                                    int damage = (int)player.GetTotalDamage<RogueDamageClass>().ApplyTo(60);
                                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Vector2.Zero, ProjectileType<NanotechProjectile>(), damage, 0f, projectile.owner);
                                }
                            }
                        }

                        if (modPlayer.dragonScales)
                        {
                            if (Main.player[projectile.owner].miscCounter % 50 == 0 && projectile.FinalExtraUpdate())
                            {
                                if (projectile.owner == Main.myPlayer && player.ownedProjectileCounts[ProjectileType<DragonShit>()] < 5)
                                {
                                    int damage = (int)player.GetTotalDamage<RogueDamageClass>().ApplyTo(DragonScales.ShitBaseDamage);

                                    int proj = Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Vector2.One.RotatedByRandom(MathHelper.TwoPi) * 1.2f, ProjectileType<DragonShit>(), damage, 0f, projectile.owner);
                                    if (proj.WithinBounds(Main.maxProjectiles))
                                    {
                                        Main.projectile[proj].DamageType = DamageClass.Generic;
                                        Main.projectile[proj].ArmorPenetration = 10;
                                    }
                                }
                            }
                        }

                        if (modPlayer.daedalusSplit)
                        {
                            if (Main.player[projectile.owner].miscCounter % 30 == 0 && projectile.FinalExtraUpdate())
                            {
                                if (projectile.owner == Main.myPlayer && player.ownedProjectileCounts[ProjectileID.CrystalShard] < DaedalusHeadRogue.ShardCountLimit)
                                {
                                    int crystalDamage = CalamityUtils.DamageSoftCap(projectile.damage * DaedalusHeadRogue.ShardDamageRatio, DaedalusHeadRogue.ShardDamageSoftcap);

                                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                                    int shard = Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, velocity, ProjectileID.CrystalShard, crystalDamage, 0f, projectile.owner);
                                    if (shard.WithinBounds(Main.maxProjectiles))
                                        Main.projectile[shard].DamageType = DamageClass.Generic;
                                }
                            }
                        }
                    }

                    if (player.meleeEnchant > 0 && !projectile.noEnchantments && !projectile.noEnchantmentVisuals)
                    {
                        switch (player.meleeEnchant)
                        {
                            case 1:
                                if (!Main.rand.NextBool(3))
                                {
                                    Dust venom = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Venom, 0f, 0f, 100);
                                    venom.noGravity = true;
                                    venom.fadeIn = 1.5f;
                                    venom.velocity *= 0.25f;
                                }
                                break;
                            case 2:
                                if (Main.rand.NextBool())
                                {
                                    Dust cflame = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.CursedTorch, projectile.velocity.X * 0.2f + (projectile.direction * 3), projectile.velocity.Y * 0.2f, 100, new Color(), 2.5f);
                                    cflame.noGravity = true;
                                    cflame.velocity *= 0.7f;
                                    cflame.velocity.Y -= 0.5f;
                                }
                                break;
                            case 3:
                                if (Main.rand.NextBool())
                                {
                                    Dust fire = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Torch, projectile.velocity.X * 0.2f + (projectile.direction * 3), projectile.velocity.Y * 0.2f, 100, new Color(), 2.5f);
                                    fire.noGravity = true;
                                    fire.velocity *= 0.7f;
                                    fire.velocity.Y -= 0.5f;
                                }
                                break;
                            case 4:
                                if (Main.rand.NextBool())
                                {
                                    Dust gold = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Enchanted_Gold, projectile.velocity.X * 0.2f + (projectile.direction * 3), projectile.velocity.Y * 0.2f, 100, new Color(), 1.1f);
                                    gold.noGravity = true;
                                    gold.velocity *= 0.5f;
                                }
                                break;
                            case 5:
                                if (Main.rand.NextBool())
                                {
                                    Dust ichor = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.IchorTorch, 0f, 0f, 100);
                                    ichor.velocity.X += projectile.direction;
                                    ichor.velocity.Y += 0.2f;
                                    ichor.noGravity = true;
                                }
                                break;
                            case 6:
                                if (Main.rand.NextBool())
                                {
                                    Dust nanite = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.IceTorch, 0f, 0f, 100);
                                    nanite.velocity.X += projectile.direction;
                                    nanite.velocity.Y += 0.2f;
                                    nanite.noGravity = true;
                                }
                                break;
                            case 8:
                                if (Main.rand.NextBool(4))
                                {
                                    Dust poison = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Poisoned, 0f, 0f, 100);
                                    poison.noGravity = true;
                                    poison.fadeIn = 1.5f;
                                    poison.velocity *= 0.25f;
                                }
                                break;
                            case CalamityGlobalBuff.ModdedFlaskEnchant:
                                int dustType = player.Calamity().flaskHoly ? (Main.rand.NextBool() ? 87 : (int)CalamityDusts.ProfanedFire) : player.Calamity().flaskBrimstone ? (Main.rand.NextBool() ? 114 : DustType<BrimstoneFlame>()) : (Main.rand.NextBool() ? 121 : DustID.Stone);
                                if (Main.rand.NextBool(4))
                                {
                                    Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, dustType, 0f, 0f, 100, default, Main.rand.NextFloat(0.6f, 0.9f));
                                    dust.noGravity = dust.type == 121 ? false : true;
                                    if (!player.Calamity().flaskHoly)
                                        dust.fadeIn = 1f;
                                    dust.velocity = player.Calamity().flaskHoly && Main.rand.NextBool(3) ? new Vector2(Main.rand.NextFloat(-0.9f, 0.9f), Main.rand.NextFloat(-6.6f, -9.8f)) : dust.type == 121 ? new Vector2(Main.rand.NextFloat(-0.7f, 0.7f), Main.rand.NextFloat(0.6f, 1.8f)) : -projectile.velocity * 0.2f;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }

                if (projectile.CountsAsClass<MeleeDamageClass>() || projectile.CountsAsClass<SummonMeleeSpeedDamageClass>())
                {
                    if ((player.Calamity().flaskBrimstone || player.Calamity().flaskCrumbling || player.Calamity().flaskHoly) && !projectile.noEnchantments && !projectile.noEnchantmentVisuals)
                    {
                        int dustType = player.Calamity().flaskHoly ? (Main.rand.NextBool() ? 87 : (int)CalamityDusts.ProfanedFire) : player.Calamity().flaskBrimstone ? (Main.rand.NextBool() ? 114 : DustType<BrimstoneFlame>()) : (Main.rand.NextBool() ? 121 : DustID.Stone);
                        if (Main.rand.NextBool(player.Calamity().flaskCrumbling ? 5 : 4))
                        {
                            Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, dustType, 0f, 0f, 100, default, Main.rand.NextFloat(0.6f, 0.9f));
                            dust.noGravity = dust.type == 121 ? false : true;
                            if (player.Calamity().flaskBrimstone)
                                dust.fadeIn = 0.8f;
                            dust.velocity = player.Calamity().flaskHoly && Main.rand.NextBool(3) ? new Vector2(Main.rand.NextFloat(-0.9f, 0.9f), Main.rand.NextFloat(-6.6f, -9.8f)) : dust.type == 121 ? new Vector2(Main.rand.NextFloat(-0.7f, 0.7f), Main.rand.NextFloat(0.6f, 1.8f)) : -projectile.velocity * 0.2f;
                        }
                    }
                }

                if (modPlayer.theBee && projectile.owner == Main.myPlayer && projectile.damage > 0)
                {
                    bool lifeAndShieldCondition = player.statLife >= player.statLifeMax2 && (!modPlayer.HasAnyEnergyShield || modPlayer.TotalEnergyShielding >= modPlayer.TotalMaxShieldDurability);
                    if (lifeAndShieldCondition && Main.rand.NextBool(5))
                    {
                        Dust dust = Dust.NewDustDirect(projectile.position + projectile.velocity, projectile.width, projectile.height, DustID.GemDiamond, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f, 0, default, 0.5f);
                        dust.noGravity = true;
                    }
                }

                // Adds Elemental Gauntlet dust to melee projectiles to mirror Fire Gauntlet's behavior.
                if (modPlayer.eGauntlet && modPlayer.eGauntletVisuals && projectile.CountsAsClass<MeleeDamageClass>())
                {
                    if (Main.rand.NextBool(3))
                    {
                        int element = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, DustID.RainbowTorch, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f, 100, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.25f);
                        Main.dust[element].noGravity = true;
                    }
                }

                if (!projectile.CountsAsClass<MeleeDamageClass>() && player.meleeEnchant == 7 && !projectile.noEnchantmentVisuals) //flask of party affects all types of weapons
                {
                    Vector2 velocity = projectile.velocity;
                    if (velocity.Length() > 4.0)
                        velocity *= 4f / velocity.Length();
                    if (Main.rand.NextBool(20))
                    {
                        Dust confetti = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, Main.rand.Next(139, 143), velocity.X, velocity.Y, 0, new Color(), 1.2f);
                        confetti.scale *= 1f + Main.rand.Next(-20, 21) * 0.01f;
                        confetti.velocity.X *= 1f + Main.rand.Next(-50, 51) * 0.01f;
                        confetti.velocity.Y *= 1f + Main.rand.Next(-50, 51) * 0.01f;
                        confetti.velocity.X += Main.rand.Next(-50, 51) * 0.05f;
                        confetti.velocity.Y += Main.rand.Next(-50, 51) * 0.05f;
                    }
                    if (Main.rand.NextBool(40) && !Main.dedServ)
                    {
                        int Type = Main.rand.Next(276, 283);
                        Gore confetti = Gore.NewGoreDirect(projectile.GetSource_FromAI(), projectile.position, velocity, Type, 1f);
                        confetti.scale *= 1f + Main.rand.Next(-20, 21) * 0.01f;
                        confetti.velocity.X *= 1f + Main.rand.Next(-50, 51) * 0.01f;
                        confetti.velocity.Y *= 1f + Main.rand.Next(-50, 51) * 0.01f;
                        confetti.velocity.X += Main.rand.Next(-50, 51) * 0.05f;
                        confetti.velocity.Y += Main.rand.Next(-50, 51) * 0.05f;
                    }
                }
                // Support to help things like holdout swords look better with Arc Flash Ring
                if (!showArcFlash && projectile.numHits == 0)
                    showArcFlash = true;
                // Cooldown for the arc flash so that long lasting projectiles (like dashing summons) can spawn multiple bolts
                if (arcFlashCooldown >= 0)
                    arcFlashCooldown--;
                if (arcFlashCooldown == 0)
                    showArcFlash = true;
                if (conditionalHomingRange > 0f && Main.player[projectile.owner].heldProj != projectile.whoAmI && projectile.aiStyle != ProjAIStyleID.HeldProjectile && !grapeBeer)
                {
                    CalamityUtils.HomeInOnNPC(projectile, !projectile.tileCollide, conditionalHomingRange, conditionalHomingVelocity, conditionalHomingInertia, true);
                }

                // Grape beer-specific homing. If the projectile is aligned a certain amount to the direction to the target, it will home in.
                else if (conditionalHomingRange > 0f && Main.player[projectile.owner].heldProj != projectile.whoAmI && projectile.aiStyle != ProjAIStyleID.HeldProjectile && grapeBeer)
                {
                    float npcDistCompare = 25000f; // Initializing the value to a large number so the first entry is basically guaranteed to replace it.
                    NPC target = null;
                    foreach (NPC n in Main.ActiveNPCs)
                    {
                        float extraDistance = (n.width / 2) + (n.height / 2);
                        if (!n.CanBeChasedBy(projectile, false) || !projectile.WithinRange(n.Center, conditionalHomingRange + extraDistance) || ((projectile.localNPCImmunity[n.whoAmI] > 0 || projectile.localNPCImmunity[n.whoAmI] == -1 || n.immune[projectile.owner] > 0)))
                            continue;

                        float currentNPCDist = Vector2.Distance(n.Center, projectile.Center);
                        if (Projectile.perIDStaticNPCImmunity[projectile.type][n.whoAmI] > Main.GameUpdateCount)
                            currentNPCDist += 1600; //Prioritize things that don't currently have iframes, but still home in on stuff that has static iframes if needed.
                        if ((currentNPCDist < npcDistCompare) && (!projectile.tileCollide || Collision.CanHit(projectile.Center, 1, 1, n.Center, 1, 1)))
                        {
                            npcDistCompare = currentNPCDist;
                            target = n;
                        }
                    }

                    if (target is not null)
                    {
                        if (grapeBeerHomingTarget == target.whoAmI)
                        {
                            grapeBeerHomingPower += 0.004f;
                        }
                        else
                        {
                            grapeBeerHomingPower = 0.015f;
                        }

                        HomingTarget = grapeBeerHomingTarget = target.whoAmI;
                        Vector2 targetDirection = projectile.SafeDirectionTo(target.Center);

                        float trackingSpeed = grapeBeerHomingPower;

                        var currVelocity = projectile.velocity.Length();
                        if (currVelocity < 8)
                            currVelocity += grapeBeerHomingPower;

                        projectile.velocity = projectile.velocity.ToRotation().AngleTowards(targetDirection.ToRotation(), trackingSpeed).ToRotationVector2() * currVelocity;
                    }
                }

                if (projectile.Opacity > 0 && projectile.scale > 0.01f) // Only apply bullet visuals if the bullet is visible
                {
                    if (brimstoneBullets)
                    {
                        PointParticle spark = new PointParticle(projectile.Center + projectile.velocity * 3, projectile.velocity, false, 2, 0.9f * projectile.scale, Color.Crimson * 0.7f);
                        GeneralParticleHandler.SpawnParticle(spark);

                        Dust dust = Dust.NewDustPerfect(projectile.Center - projectile.velocity, Main.rand.NextBool(3) ? 90 : DustType<BrimstoneFlame>(), projectile.velocity * Main.rand.NextFloat(0.05f, 0.9f));
                        dust.noGravity = true;
                        dust.scale = Main.rand.NextFloat(0.5f, 1f) * projectile.scale;
                    }
                    if (fireBullet)
                    {
                        float helixOffset = (float)Math.Sin(projectile.timeLeft / 25f * MathHelper.TwoPi) * 8f;
                        Vector2 spawnOffset = new Vector2(helixOffset, 10f).RotatedBy(projectile.rotation) * projectile.scale;

                        for (int i = 0; i < 2; ++i)
                        {
                            Dust dust = Dust.NewDustPerfect(projectile.Center + spawnOffset, Main.rand.NextBool() ? 174 : 6, projectile.velocity * Main.rand.NextFloat(0.1f, 0.9f));
                            dust.noGravity = true;
                            dust.scale = Main.rand.NextFloat(0.4f, 0.8f) * projectile.scale;
                        }
                    }
                    if (iceBullet)
                    {
                        float helixOffset = (float)Math.Sin(projectile.timeLeft / 25f * MathHelper.TwoPi) * -8f;
                        Vector2 spawnOffset = new Vector2(helixOffset, 10f).RotatedBy(projectile.rotation) * projectile.scale;

                        for (int i = 0; i < 2; ++i)
                        {
                            Dust dust = Dust.NewDustPerfect(projectile.Center + spawnOffset, Main.rand.NextBool() ? 135 : 137, projectile.velocity * Main.rand.NextFloat(0.1f, 0.9f));
                            dust.noGravity = true;
                            dust.scale = Main.rand.NextFloat(0.4f, 0.8f) * projectile.scale;
                        }
                    }
                    if (shockBullet)
                    {
                        float targetDist = Vector2.Distance(player.Center, projectile.Center);
                        if (targetDist < 1400f)
                        {
                            SparkParticle spark = new SparkParticle(projectile.Center + projectile.velocity, -projectile.velocity * 0.05f, false, 2, 1.1f * projectile.scale, Color.Turquoise * 0.75f);
                            GeneralParticleHandler.SpawnParticle(spark);
                            if (Main.rand.NextBool(3))
                            {
                                SparkParticle spark2 = new SparkParticle(projectile.Center + Main.rand.NextVector2Circular(6, 6) * projectile.scale, -projectile.velocity * Main.rand.NextFloat(0.05f, 0.4f), false, 20, 0.4f * projectile.scale, Color.Turquoise * 0.75f);
                                GeneralParticleHandler.SpawnParticle(spark2);
                            }
                        }
                    }
                    if ((pearlBullet1 || pearlBullet2 || pearlBullet3))
                    {
                        float targetDist = Vector2.Distance(player.Center, projectile.Center);
                        if (targetDist < 1400f)
                        {
                            Color color = pearlBullet1 ? Color.LightBlue : pearlBullet2 ? Color.LightPink : Color.Khaki;
                            Particle spark = new GlowSparkParticle(projectile.Center + projectile.velocity * 1.5f, -projectile.velocity * 0.05f, false, 3, 0.0093f * projectile.scale, color, new Vector2(0.6f, 1.8f), false, false);
                            GeneralParticleHandler.SpawnParticle(spark);
                            if (Main.rand.NextBool(5))
                            {
                                PearlParticle pearl1 = new PearlParticle(projectile.Center + Main.rand.NextVector2Circular(6, 6) * projectile.scale, -projectile.velocity * Main.rand.NextFloat(0.05f, 0.3f), false, Main.rand.Next(15, 20 + 1), Main.rand.NextFloat(0.4f, 0.55f) * projectile.scale, color, 0.9f, Main.rand.NextFloat(1, -1), true);
                                GeneralParticleHandler.SpawnParticle(pearl1);
                            }
                        }
                    }
                    if (lifeBullet)
                    {
                        float targetDist = Vector2.Distance(player.Center, projectile.Center);
                        if (targetDist < 1400f)
                        {
                            SparkParticle spark = new SparkParticle(projectile.Center + projectile.velocity, -projectile.velocity * 0.05f, false, 2, 0.85f * projectile.scale, Color.White * 0.75f);
                            GeneralParticleHandler.SpawnParticle(spark);

                            for (int i = 0; i < 2; ++i)
                            {
                                Dust dust = Dust.NewDustPerfect(projectile.Center + projectile.velocity, DustID.AncientLight, -projectile.velocity * Main.rand.NextFloat(0.1f, 0.9f));
                                dust.noGravity = true;
                                dust.scale = Main.rand.NextFloat(0.65f, 0.9f) * projectile.scale;
                                dust.alpha = 100;
                            }

                        }
                    }
                    #region betterLifeBullet
                    if (betterLifeBullet1)
                    {
                        float targetDist = Vector2.Distance(player.Center, projectile.Center);
                        if (targetDist < 1400f)
                        {
                            int randomColor = Main.rand.Next(1, 3 + 1);
                            Color color = randomColor == 1 ? Color.LightBlue : randomColor == 2 ? Color.LightPink : Color.Khaki;

                            float helixOffset = (float)Math.Sin(projectile.timeLeft / 25f * MathHelper.TwoPi) * -8f;
                            Vector2 spawnOffset = new Vector2(helixOffset, 10f).RotatedBy(projectile.rotation);

                            for (int i = 0; i < 3; ++i)
                            {
                                Dust dust = Dust.NewDustPerfect(projectile.Center + spawnOffset, DustID.FireworksRGB, projectile.velocity * Main.rand.NextFloat(0.05f, 0.2f));
                                dust.noGravity = true;
                                dust.scale = Main.rand.NextFloat(0.35f, 0.45f) * projectile.scale;
                                dust.color = color;
                            }

                            SparkParticle spark = new SparkParticle(projectile.Center + projectile.velocity, projectile.velocity * 0.05f, false, 2, 0.85f * projectile.scale, color);
                            GeneralParticleHandler.SpawnParticle(spark);

                            if (Main.rand.NextBool(3))
                            {
                                SparkParticle spark3 = new SparkParticle(projectile.Center + Main.rand.NextVector2Circular(6, 6) * projectile.scale, -projectile.velocity * Main.rand.NextFloat(0.05f, 0.3f), false, 20, 0.55f * projectile.scale, color * 0.5f);
                                GeneralParticleHandler.SpawnParticle(spark3);
                            }
                        }
                    }
                    if (betterLifeBullet2)
                    {
                        float targetDist = Vector2.Distance(player.Center, projectile.Center);
                        if (targetDist < 1400f)
                        {
                            int randomColor = Main.rand.Next(1, 3 + 1);
                            Color color = randomColor == 1 ? Color.LightBlue : randomColor == 2 ? Color.LightPink : Color.Khaki;

                            float helixOffset = (float)Math.Sin(projectile.timeLeft / 25f * MathHelper.TwoPi) * 8f;
                            Vector2 spawnOffset = new Vector2(helixOffset, 10f).RotatedBy(projectile.rotation) * projectile.scale;

                            for (int i = 0; i < 3; ++i)
                            {
                                Dust dust = Dust.NewDustPerfect(projectile.Center + spawnOffset, DustID.FireworksRGB, projectile.velocity * Main.rand.NextFloat(0.05f, 0.2f));
                                dust.noGravity = true;
                                dust.scale = Main.rand.NextFloat(0.35f, 0.45f) * projectile.scale;
                                dust.color = color;
                            }

                            SparkParticle spark = new SparkParticle(projectile.Center + projectile.velocity, projectile.velocity * 0.05f, false, 2, 0.85f * projectile.scale, color);
                            GeneralParticleHandler.SpawnParticle(spark);

                            if (Main.rand.NextBool(3))
                            {
                                SparkParticle spark3 = new SparkParticle(projectile.Center + Main.rand.NextVector2Circular(6, 6) * projectile.scale, -projectile.velocity * Main.rand.NextFloat(0.05f, 0.3f), false, 20, 0.55f * projectile.scale, color * 0.5f);
                                GeneralParticleHandler.SpawnParticle(spark3);
                            }
                        }
                    }
                    #endregion
                }
            }
        }
        #endregion

        #region Post AI
        public override void PostAI(Projectile projectile)
        {
            if (projectile.FinalExtraUpdate() && flatDRTimer > 0)
            {
                flatDRTimer--;
                if (flatDRTimer <= 0)
                    flatDR = 0;
            }
            if (projectile.FinalExtraUpdate() && multiplicativeDRTimer > 0)
            {
                multiplicativeDRTimer--;
                if (multiplicativeDRTimer <= 0)
                    multiplicativeDR = 0;
            }
            if (projectile.FinalExtraUpdate() && TransformerTimer > 0)
            {
                TransformerTimer--;
            }

            // Spawn Bloom Stone flower on landed hooks
            // Should only spawn if: Projectile is a hook, hook is grappled to a tile, the player is wearing Bloom Stone, no flower has been spawned from this hook
            if (projectile.aiStyle == ProjAIStyleID.Hook && projectile.ai[0] == 2f &&
                Main.player[projectile.owner].Calamity().bloomStone && !hookCanSpawnFlower)
            {
                hookCanSpawnFlower = true;
                if (Main.myPlayer == projectile.owner)
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Vector2.Zero, ProjectileType<BloomStoneFlower>(), 0, 0f, projectile.owner, projectile.whoAmI);
            }

            // Fertilizer
            if (projectile.type == ProjectileID.Fertilizer && projectile.owner == Main.myPlayer/* && Main.netMode != NetmodeID.MultiplayerClient*/)
            {
                int x = (int)(projectile.Center.X / 16f);
                int y = (int)(projectile.Center.Y / 16f);

                if (!WorldGen.InWorld(x, y, 3))
                    return;

                for (int i = x - 1; i <= x + 1; i++)
                {
                    for (int j = y - 1; j <= y + 1; j++)
                    {
                        Tile tile = Main.tile[i, j];

                        if (tile.TileType == ModContent.TileType<AstralTreeSapling>() || tile.TileType == ModContent.TileType<AstralSnowTreeSapling>())
                        {
                            bool isPlayerNear = WorldGen.PlayerLOS(i, j);
                            bool success = WorldGen.GrowTree(i, j);
                            if (success && isPlayerNear)
                                WorldGen.TreeGrowFXCheck(i, j);
                        }
                        else if (tile.TileType == ModContent.TileType<AstralPalmSapling>() || tile.TileType == ModContent.TileType<AcidWoodTreeSapling>())
                        {
                            bool isPlayerNear = WorldGen.PlayerLOS(i, j);
                            bool success = WorldGen.GrowPalmTree(i, j);
                            if (success && isPlayerNear)
                                WorldGen.TreeGrowFXCheck(i, j);
                        }
                        else if (tile.TileType == ModContent.TileType<SpineSapling>())
                        {
                            bool isPlayerNear = WorldGen.PlayerLOS(i, j);
                            if (isPlayerNear && Main.tile[i, j + 1].TileType != ModContent.TileType<SpineSapling>())
                                SpineTree.Spawn(i, j, 22, 28, true);
                        }
                        NetMessage.SendTileSquare(-1, i, j, 1, 1);
                    }
                }
            }
        }
        #endregion

        #region Grappling Hooks
        public override void UseGrapple(Player player, ref int type)
        {
            if (player.Calamity().bloomStoneHookVisuals)
            {
                for (int i = 0; i < 8; i++)
                {
                    Particle floweyFromHitGameUndertale = new CustomSpark(player.Center, Utils.DirectionTo(player.Center, player.Calamity().mouseWorld).RotatedByRandom(0.6f) * Main.rand.NextFloat(4f, 11f),
                        "CalamityMod/Particles/MiniFlower", false, Main.rand.Next(65, 78 + 1), Main.rand.NextFloat(1.8f, 2.8f),
                        Color.Lerp(Color.HotPink, Color.Plum, Main.rand.NextFloat(0, 0.65f)), new Vector2(1f, 1f), true, extraRotation: Main.rand.NextFloat(0, MathHelper.TwoPi));
                    GeneralParticleHandler.SpawnParticle(floweyFromHitGameUndertale);

                    Dust pollenDust = Dust.NewDustPerfect(player.Center, DustType<SquashDust>());
                    pollenDust.noLightEmittence = true;
                    pollenDust.noGravity = true;
                    pollenDust.scale = Main.rand.NextFloat(0.9f, 1.4f);
                    pollenDust.color = Main.rand.NextBool() ? Color.Gold : Color.HotPink;
                    pollenDust.velocity = Utils.DirectionTo(player.Center, player.Calamity().mouseWorld).RotatedByRandom(0.6f) * Main.rand.NextFloat(4f, 11f);
                    pollenDust.fadeIn = -0.5f;
                }
            }
        }
        public override void GrapplePullSpeed(Projectile projectile, Player player, ref float speed)
        {
            if (player.Calamity().bloomStone)
                speed += 6f;
            float mult = 1f;
            if (player.Calamity().reaverSpeed)
                mult += ReaverHeadMobility.SetBonusHookBoost;
            if (player.Calamity().tungstenArmorHookBoost)
                mult += TungstenArmorSetChange.HookBoost;

            speed *= mult;
            if (player.velocity.Length() > 2f)
            {
                player.Calamity().hookPullVisuals = 60;
            }
        }
        public override void GrappleRetreatSpeed(Projectile projectile, Player player, ref float speed)
        {
            if (player.Calamity().bloomStone)
                speed += 6f;
            float mult = 1f;
            if (player.Calamity().reaverSpeed)
                mult += ReaverHeadMobility.SetBonusHookBoost;
            if (player.Calamity().tungstenArmorHookBoost)
                mult += TungstenArmorSetChange.HookBoost;
            speed *= mult;
        }
        #endregion

        #region Modify Hit NPC
        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            if (grapeBeer)
            {
                Vector2 pointToCheck = Vector2.Clamp(player.Center, target.TopLeft, target.BottomRight);
                float remap = Utils.Remap(Utils.Distance(player.Center, pointToCheck), GrapeBeer.CloseRangeDistance, GrapeBeer.LongRangeDistance, GrapeBeer.CloseRangeDamage, GrapeBeer.LongRangeDamage);
                modifiers.SourceDamage *= remap;
            }

            // Old Fashioned buffs (or debuffs) apply if the player has Old Fashioned
            if (modPlayer.oldFashioned && buffedByOldFashioned.HasValue)
                modifiers.SourceDamage *= buffedByOldFashioned.Value ? OldFashioned.DamageBoostMultiplier : OldFashioned.DamageReductionMultiplier;

            var dripPlayer = player.GetModPlayer<IVDripPlayer>();
            if (dripPlayer.HasAlcohol(AlcoholType.OldFashioned) && buffedByOldFashioned.HasValue)
                modifiers.SourceDamage *= buffedByOldFashioned.Value ? OldFashioned.DamageBoostMultiplier : OldFashioned.DamageReductionMultiplier;

            if ((modPlayer.rum || dripPlayer.HasAlcohol(AlcoholType.Rum)) && projectile.DamageType.CountsAsClass(DamageClass.Summon))
            {
                if (projectile.minion || ProjectileID.Sets.MinionShot[projectile.type])
                    modifiers.SourceDamage *= (modPlayer.rum ? Rum.MinionBoost : 1f) * (dripPlayer.HasAlcohol(AlcoholType.Rum) ? Rum.MinionBoost : 1f);
                else
                    modifiers.SourceDamage *= (modPlayer.rum ? Rum.NonMinionBoost : 1f) * (dripPlayer.HasAlcohol(AlcoholType.Rum) ? Rum.NonMinionBoost : 1f);
            }

            if (modPlayer.moscowMule || modPlayer.bloodyMary || dripPlayer.HasAlcohol(AlcoholType.MoscowMule) || dripPlayer.HasAlcohol(AlcoholType.BloodyMary))
            {
                if (projectile.DamageType == DamageClass.Summon || (PierceResistNPC.exemptProjectiles.Contains(projectile.type) || (PierceResistNPC.singleHitboxExemptProjectiles.ContainsKey(projectile.type) && PierceResistNPC.singleHitboxExemptProjectiles[projectile.type])))
                {
                    modifiers.SourceDamage *= ((modPlayer.moscowMule ? MoscowMule.PierceDamageMultiplier : 1f) * (modPlayer.bloodyMary ? BloodyMary.PierceDamageMultiplier : 1f) * (dripPlayer.HasAlcohol(AlcoholType.MoscowMule) ? MoscowMule.PierceDamageMultiplier : 1f) * (dripPlayer.HasAlcohol(AlcoholType.BloodyMary) ? BloodyMary.PierceDamageMultiplier : 1f));
                }
            }

            if (projectile.type == ProjectileID.JoustingLance || projectile.type == ProjectileID.HallowJoustingLance || projectile.type == ProjectileID.ShadowJoustingLance)
            {
                // The vanilla damage Jousting Lance multiplier is as follows. Calamity overrides this with a new formula
                float vanillaVelocityDamageMultiplier = 0.1f + player.velocity.Length() / 7f * 0.9f;
                float baseVelocityDamageMultiplier = 0.01f + player.velocity.Length() * 0.002f;
                float calamityVelocityDamageMultiplier = 100f * (1f - (1f / (1f + baseVelocityDamageMultiplier)));
                modifiers.SourceDamage *= calamityVelocityDamageMultiplier / vanillaVelocityDamageMultiplier;
            }

            // Adamantite Throwing Axe's lightning has damage falloff
            if (projectile.type == ProjectileID.CultistBossLightningOrbArc && projectile.ai[2] == 1f)
            {
                if (projectile.numHits > 0)
                    projectile.damage = (int)(projectile.damage * 0.8f);
                if (projectile.damage < 1)
                    projectile.damage = 1;
            }

            // Super Star Shooter has damage falloff
            if (projectile.type == ProjectileID.SuperStar)
            {
                if (projectile.numHits > 0)
                    projectile.damage = (int)(projectile.damage * 0.95f);
                if (projectile.damage < 1)
                    projectile.damage = 1;
            }

            // Stardust Wings buff the Stardust Guardian's damage
            if (player.wingsLogic == (int)VanillaWingID.WingsStardust && player.wingTime > 0f && projectile.type == ProjectileID.StardustGuardian)
                modifiers.SourceDamage *= MathF.Max(1f, MathHelper.Lerp(4f, 1f, player.wingTime / (float)player.wingTimeMax));

            // If applicable, use ricoshot bonus damage
            if (totalRicoshotDamageBonus > 0f)
                modifiers.ScalingBonusDamage += totalRicoshotDamageBonus;

            // If this projectile is forced to crit, simply set the crit bool
            if (forcedCrit)
                modifiers.SetCrit();

            if (modPlayer.rottenDogTooth && projectile.Calamity().stealthStrike && !modPlayer.vampiricTalisman)
                target.AddBuff(BuffType<Crumbling>(), RottenDogtooth.ArmorCrunchDebuffTime);
            else if (modPlayer.vampiricTalisman && projectile.Calamity().stealthStrike)
            {
                target.AddBuff(BuffType<ArmorCrunch>(), VampiricTalisman.ArmorCrunchDebuffTime);
                target.AddBuff(BuffType<HeavyBleeding>(), VampiricTalisman.HeavyBleedingDebuffTime);
            }

            if (modPlayer.flamingItemEnchant && !projectile.minion && !projectile.npcProj && !projectile.Calamity().CreatedByPlayerDash)
                target.AddBuff(BuffType<VulnerabilityHex>(), VulnerabilityHex.AflameDuration);

            // This can apply to minions or minion shots
            if (ExplosiveEnchantCountdown > 0)
                modifiers.SourceDamage *= MathHelper.SmoothStep(1f, 1.6f, 1f - ExplosiveEnchantCountdown / (float)ExplosiveEnchantTime);

            if (modPlayer.farProximityRewardEnchant)
            {
                float proximityDamageInterpolant = Utils.GetLerpValue(250f, 2400f, target.Distance(player.Center), true);
                float proximityDamageFactor = MathHelper.SmoothStep(0.7f, 1.45f, proximityDamageInterpolant);
                modifiers.SourceDamage *= proximityDamageFactor;
            }

            if (modPlayer.closeProximityRewardEnchant)
            {
                float proximityDamageInterpolant = Utils.GetLerpValue(400f, 175f, target.Distance(player.Center), true);
                float proximityDamageFactor = MathHelper.SmoothStep(0.75f, 1.75f, proximityDamageInterpolant);
                modifiers.SourceDamage *= proximityDamageFactor;
            }

            // Aerial Bane does 50% damage to "airborne" enemies. This is just simple math to revert that as it is a very unbalanced mechanic.
            if (projectile.type == ProjectileID.DD2BetsyArrow)
            {
                if (!WorldUtils.Find(projectile.Center.ToTileCoordinates(), Searches.Chain(new Searches.Down(12), new Conditions.IsSolid()), out _))
                    modifiers.SourceDamage /= 1.5f;
            }

            // Debuff infliction must be placed here as opposed to a more common hook like OnHitNPCWithProj because its on-hit logic breaks the loop before it reaches that hook
            if (projectile.type == ProjectileID.InfluxWaver)
                target.AddBuff(BuffID.Electrified, 180);

            // Create sparks on hit to hammer in the defense shredding.
            if (deepcoreBullet)
            {
                if (!Main.dedServ)
                {
                    Vector2 cen = Vector2.Lerp(projectile.Center, target.Center, 0.65f);
                    int numSparks = Main.rand.Next(2, 5);
                    for (int i = 0; i < numSparks; i++)
                    {
                        Vector2 sparkVelocity = projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedByRandom(0.6f) * Main.rand.NextFloat(2f, 8f);
                        Color sparkColor = Color.Lerp(Color.AliceBlue, Color.DarkSlateBlue, Main.rand.NextFloat(0.7f));
                        sparkColor = Color.Lerp(sparkColor, Color.CornflowerBlue, Main.rand.NextFloat());
                        SparkParticle sparke = new(cen, -sparkVelocity, false, 10, 0.5f, sparkColor);
                        GeneralParticleHandler.SpawnParticle(sparke);
                    }
                }
            }

            // Scuttler's Jewel projectiles can spawn either on-hit or on-kill, but only spawn once per projectile.
            if (projectile.owner == Main.myPlayer && !projectile.npcProj && !projectile.trap && projectile.CountsAsClass<RogueDamageClass>() && !CannotProc)
            {
                if (modPlayer.scuttlersJewel && stealthStrike && !JewelSpikeSpawned)
                {
                    int damage = (int)player.GetTotalDamage<RogueDamageClass>().ApplyTo(16);

                    int spike = Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Vector2.Zero, ProjectileType<JewelSpike>(), damage, projectile.knockBack, projectile.owner);
                    Main.projectile[spike].frame = 4;
                    JewelSpikeSpawned = true;
                }
                if (modPlayer.etherealExtorter && extorterBoost && Main.player[projectile.owner].ownedProjectileCounts[ProjectileType<LostSoulFriendly>()] < 5)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);

                        int damage = (int)player.GetTotalDamage<RogueDamageClass>().ApplyTo(EtherealExtorter.soulDamage);

                        int soul = Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, velocity, ProjectileType<LostSoulFriendly>(), damage, 0f, projectile.owner);
                        Main.projectile[soul].tileCollide = false;
                        if (soul.WithinBounds(Main.maxProjectiles))
                            Main.projectile[soul].DamageType = DamageClass.Generic;
                            Main.projectile[soul].ArmorPenetration = 20;
                    }  
                    extorterBoost = false;
                }
            }


        }
        #endregion

        #region Modify Hit Player
        public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers)
        {                
            modifiers.FinalDamage.Flat -= flatDR;
            modifiers.FinalDamage *= 1f - multiplicativeDR;

            // Reduce projectile damage if the enemy is inflicted with Whispering Death
            if (ParentNPCIndex != -1)
            {
                NPC npc = Main.npc[(int)ParentNPCIndex];
                if (npc.active && npc.Calamity().whisperingDeath)
                    modifiers.FinalDamage *= 1f - WhisperingDeath.EnemyDamageReduction;
            }
        }
        #endregion

        #region On Hit NPC
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            //Manage Hybrid iframes
            if ((projectile.usesLocalNPCImmunity && projectile.usesIDStaticNPCImmunity) && (projectile.penetrate != 1 || projectile.appliesImmunityTimeOnSingleHits))
            {
                projectile.localNPCImmunity[target.whoAmI] = projectile.localNPCHitCooldown;
                Projectile.perIDStaticNPCImmunity[projectile.type][target.whoAmI] = Main.GameUpdateCount + (uint)projectile.idStaticNPCHitCooldown;
            }
            if (BloodstoneOrbValue > 0)
                Projectile.NewProjectile(projectile.GetSource_OnHit(target), projectile.Center, projectile.velocity.SafeNormalize(Vector2.Zero) * Math.Min(((projectile.velocity.Length() * projectile.MaxUpdates) / 4f), 4f) * Main.rand.NextFloat(0.75f, 1.25f), ModContent.ProjectileType<BloodstoneHealOrb>(), BloodstoneOrbValue, 0f, Main.player[projectile.owner].whoAmI);
            //Mana Burn
            if (Main.player[projectile.owner].statMana < 0 && Main.player[projectile.owner].Calamity().ChaosStone)
            {
                var player = Main.player[projectile.owner];
                float burnRatio = (-player.statMana / 100) * ChaosStone.DamageMultPer100Mana;
                var dripPlayer = player.GetModPlayer<IVDripPlayer>();
                target.Calamity().manaBurn += damageDone * burnRatio * (dripPlayer.HasAlcohol(AlcoholType.OldFashioned) ? OldFashioned.DamageBoostMultiplier : 1) * (dripPlayer.HasAlcohol(AlcoholType.OldFashioned) ? OldFashioned.DamageBoostMultiplier : 1) * (player.Calamity().vodka ? 1 + Vodka.DebuffBoost : 1);
                target.Calamity().playerManaBurnIntensity = -player.statMana / (float)player.statManaMax2;
                if (Main.netMode != NetmodeID.SinglePlayer)
                    ManaBurnSyncPacket.Send(target);
            }
            // Hyperius Overflow
            if (projectile.type != ProjectileType<HyperiusBulletProj>() && projectile.type != ProjectileType<HyperiusSplit>() && projectile.type != ProjectileType<HyperiusDamage>() && projectile.type != ProjectileType<HyperiusBleed>() && target.Calamity().hyperiusMarked)
            {
                int damageDealt = (int)(damageDone * HyperiusBullet.overflowAppliedMult);
                int damage = (int)MathF.Min(target.Calamity().hyperiusDamage / HyperiusBullet.overflowEfficency, damageDealt);

                target.Calamity().hyperiusDamage -= (int)(damage * HyperiusBullet.overflowEfficency);

                // Spawn overflow hit
                Projectile overflow = Projectile.NewProjectileDirect(target.GetSource_FromThis(), target.Center, Vector2.Zero, ProjectileType<HyperiusDamage>(), damage, 0, projectile.owner, target.whoAmI);
                overflow.DamageType = projectile.DamageType;
                overflow.ArmorPenetration = projectile.ArmorPenetration; // Takes the armor pen from what did the hit

                if (target.Calamity().hyperiusDamage <= 0)
                {
                    target.Calamity().hyperiusDamage = 0;
                    target.Calamity().hyperiusMarked = false;
                }
            }
            // Crystal Darts reset iframes on bouncing off an enemy
            if (projectile.type == ProjectileID.CrystalDart)
            {
                projectile.ResetLocalNPCHitImmunity();
                projectile.localNPCImmunity[target.whoAmI] = -1;
            }
            // Implementation of shared static iframes.
            // If this projectile does not use static iframes, or is not registered to share them, then do nothing.
            if (!projectile.usesIDStaticNPCImmunity || CalamityProjectileSets.SharedIDStaticIFrames[projectile.type] == -1)
                return;

            // Apply the appropriate shared static iframes to all projectile types with which it is shared.
            for (int proj = 0; proj < CalamityProjectileSets.SharedIDStaticIFrames.Length; proj++)
            {
                if (CalamityProjectileSets.SharedIDStaticIFrames[proj] == CalamityProjectileSets.SharedIDStaticIFrames[projectile.type])
                    Projectile.perIDStaticNPCImmunity[proj][target.whoAmI] = Main.GameUpdateCount + (uint)projectile.idStaticNPCHitCooldown;
            }
        }
        #endregion

        #region Can Damage + Can Hit
        public override bool? CanDamage(Projectile projectile)
        {
            if (projectile.hostile && (projectile.damage - flatDR <= 0))
                return false;

            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            switch (projectile.type)
            {
                // Rev+ Deerclops ice spikes can only deal damage while they're not fading out
                case ProjectileID.DeerclopsIceSpike:
                    if (CalamityWorld.revenge || BossRushEvent.BossRushActive)
                    {
                        float fadeInTime = 25f;
                        float fadeOutGateValue = death ? 90f : 65f;
                        return (projectile.ai[0] >= fadeInTime && projectile.ai[0] < fadeOutGateValue);
                    }
                    break;

                // Rev+ Deerclops rubble doesn't deal damage while it's not flying upwards
                case ProjectileID.DeerclopsRangedProjectile:
                    if (CalamityWorld.revenge || BossRushEvent.BossRushActive)
                        return projectile.ai[0] > projectile.ai[2];
                    break;

                // Additional slams in the Death Mode shockwave have a delay before expanding and dealing damage
                case ProjectileID.QueenSlimeSmash:
                    if (death)
                        return projectile.ai[0] > 0f;
                    break;

                // Storm Weaver frost waves don't deal damage unless they're at their max velocity
                case ProjectileID.FrostWave:
                    if (projectile.ai[1] > 0f)
                        return projectile.velocity.Length() >= projectile.ai[1];
                    break;

                // Duke Fishron tornadoes deal no damage for 1 second after spawning
                case ProjectileID.Sharknado:
                    if (projectile.timeLeft > FishronSharknadoTotalDuration - FishronTornadoTimeBeforeDealingDamage)
                        return false;
                    break;

                case ProjectileID.Cthulunado:
                    if (projectile.timeLeft > FishronCthulhunadoTotalDuration - FishronTornadoTimeBeforeDealingDamage)
                        return false;
                    break;

                // Empress Rainbow Streaks deal no damage if they haven't started homing
                case ProjectileID.HallowBossRainbowStreak:
                    if (projectile.hostile)
                        return projectile.timeLeft <= EmpressRainbowStreakSpreadOutCutoff;
                    break;

                // Empress Lasting Rainbows deal no damage for 1 second after spawning
                case ProjectileID.HallowBossLastingRainbow:
                    if (projectile.timeLeft > EmpressLastingRainbowTotalDuration - EmpressLastingRainbowTimeBeforeDealingDamage)
                        return false;
                    break;

                default:
                    break;
            }
            return null;
        }

        // Cultist lightning orbs cannot hit players specifically. This could probably be switched to CanDamage?
        public override bool CanHitPlayer(Projectile projectile, Player target)
        {
            if (projectile.type == ProjectileID.CultistBossLightningOrb)
            {
                return false;
            }
            return true;
        }

        public override bool? CanHitNPC(Projectile projectile, NPC target)
        {

            if ((projectile.usesLocalNPCImmunity && projectile.usesIDStaticNPCImmunity) && (projectile.localNPCImmunity[target.whoAmI] != 0 || Projectile.perIDStaticNPCImmunity[projectile.type][target.whoAmI] > Main.GameUpdateCount))
                return false;
            if (target.Calamity().IsArmored() && HomingTarget > -1 && HomingTarget != target.whoAmI)
                return false;
            return null;
        }
        #endregion

        #region TileCollide
        public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
        {
            //Crystal Darts use -1 local that needs to reset whenever they bounce
            if (projectile.type == ProjectileID.CrystalDart)
            {
                projectile.ResetLocalNPCHitImmunity();
            }
            return base.OnTileCollide(projectile, oldVelocity);
        }
        #endregion

        #region Drawing
        public override Color? GetAlpha(Projectile projectile, Color lightColor)
        {
            if (Main.LocalPlayer.Calamity().trippy)
                return new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, Main.DiscoR);

            if (Main.LocalPlayer.Calamity().omniscience && projectile.hostile && projectile.damage > 0 && projectile.alpha < 255)
            {
                if (projectile.ModProjectile is null || (projectile.ModProjectile != null && projectile.ModProjectile.CanHitPlayer(Main.LocalPlayer) && (projectile.ModProjectile.CanDamage() ?? true)))
                {
                    Color mainColor = Color.Lerp(Color.Crimson with { A = 0 }, Color.OrangeRed with { A = 0 }, (Main.GlobalTimeWrappedHourly * 2) % 1f);
                    Color actualDisplayColor = new Color(Math.Max(mainColor.R, lightColor.R), Math.Max(mainColor.G, lightColor.G), Math.Max(mainColor.B, lightColor.B));
                    return actualDisplayColor;
                }
            }

            if (projectile.type == ProjectileID.Skull && (projectile.ai[0] == -1f || projectile.ai[0] == -3f))
            {
                float homingTime = CalamityWorld.death ? 105f : 90f;
                if (projectile.ai[0] == -3f)
                    homingTime += 60f;

                return projectile.ai[1] >= homingTime ? new Color(184, 140, 255, projectile.alpha) : new Color(255, 255, 255, (int)Utils.WrappedLerp(0f, 255f, (float)(projectile.timeLeft % 40) / 40f));
            }

            if (projectile.type == ProjectileID.BloodNautilusShot)
                return new Color(200, 0, 0, projectile.alpha);

            if (projectile.type == ProjectileID.Stinger)
                return new Color(200, 200, 0, projectile.alpha);

            if (projectile.type == ProjectileID.QueenBeeStinger)
                return new Color(250, 250, 0, projectile.alpha);

            if (projectile.type == ProjectileID.QueenSlimeGelAttack || projectile.type == ProjectileID.QueenSlimeMinionBlueSpike || projectile.type == ProjectileID.QueenSlimeMinionPinkBall)
                return new Color(255, 255, 255, projectile.alpha);

            if (projectile.type == ProjectileID.PinkLaser)
            {
                if (projectile.alpha < 200)
                    return new Color(255 - projectile.alpha, 255 - projectile.alpha, 255 - projectile.alpha, 0);

                return Color.Transparent;
            }

            if (projectile.ai[1] > 0f && projectile.type == ProjectileID.FrostWave)
            {
                if (projectile.velocity.Length() < projectile.ai[1])
                {
                    float minVelocity = projectile.ai[1] * 0.5f;
                    float velocityRatio = (projectile.velocity.Length() - minVelocity) / minVelocity;
                    byte b2 = (byte)(velocityRatio * 200);
                    byte a2 = (byte)(b2 / 200f * 255f);
                    return new Color(b2, b2, b2, a2);
                }
                return new Color(200, 200, 200, projectile.alpha);
            }

            if (projectile.type == ProjectileID.ThornBall)
            {
                float startWarningColorGateValue = 420f;
                float timeToReachFullIntensity = 180f;
                float timeToReachThornExplosion = startWarningColorGateValue + timeToReachFullIntensity;
                Color initialColor = lightColor;
                Color finalColor = Color.Lerp(new Color(125, 75, 75), Color.Red, (float)Math.Abs(Math.Sin((projectile.ai[1] - startWarningColorGateValue) * (MathHelper.Pi / 45))));
                finalColor.A = (byte)(255 - projectile.alpha);
                if (projectile.ai[1] > startWarningColorGateValue)
                {
                    float colorTransitionRatio = (projectile.ai[1] - startWarningColorGateValue) / timeToReachFullIntensity;
                    Color dehydratedColor = Color.Lerp(initialColor, finalColor, colorTransitionRatio);
                    return dehydratedColor;
                }
                else
                    return initialColor;
            }

            if (projectile.type == ProjectileID.SeedPlantera || projectile.type == ProjectileID.PoisonSeedPlantera ||
                projectile.type == ProjectileID.CultistBossFireBallClone || projectile.type == ProjectileID.AncientDoomProjectile)
            {
                if (projectile.timeLeft < 85)
                {
                    byte b2 = (byte)(projectile.timeLeft * 3);
                    byte a2 = (byte)(projectile.alpha * (b2 / 255f));
                    return new Color(b2, b2, b2, a2);
                }
                return new Color(255, 255, 255, projectile.alpha);
            }

            return null;
        }

        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            bool shouldDrawBool = true;

            #region Vanilla Summons Drawing Changes

            //
            // MINION AI CHANGES:
            //

            if (projectile.type == ProjectileID.Raven)
            {
                RavenMinionAI.DoRavenMinionDrawing(projectile, ref lightColor);
                shouldDrawBool = false;
            }
            #endregion

            if (projectile.type == ProjectileID.DeerclopsIceSpike && (CalamityWorld.revenge || BossRushEvent.BossRushActive))
            {
                bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

                Texture2D texture = TextureAssets.Projectile[projectile.type].Value;
                Rectangle value26 = texture.Frame(1, 5, 0, projectile.frame);
                Vector2 origin12 = new Vector2(16f, value26.Height / 2);
                Color alpha5 = projectile.GetAlpha(lightColor);
                Vector2 vector39 = new Vector2(projectile.scale);
                float fadeOutGateValue = death ? 90f : 65f;
                float killGateValue = death ? 100f : 75f;
                float lerpValue5 = Utils.GetLerpValue(killGateValue, killGateValue - 50f, projectile.ai[0], clamped: true);
                vector39.Y *= lerpValue5;
                Vector4 vector40 = lightColor.ToVector4();
                Vector4 vector41 = new Color(67, 17, 17).ToVector4();
                vector41 *= vector40;

                SpriteEffects spriteEffects = SpriteEffects.None;
                if (projectile.spriteDirection == -1)
                    spriteEffects = SpriteEffects.FlipHorizontally;

                Main.EntitySpriteDraw(TextureAssets.Extra[ExtrasID.SharpTears].Value, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY) - projectile.velocity * projectile.scale * 0.5f, null, projectile.GetAlpha(new Color(vector41.X, vector41.Y, vector41.Z, vector41.W)), projectile.rotation + MathHelper.PiOver2, TextureAssets.Extra[ExtrasID.SharpTears].Value.Size() / 2f, projectile.scale * 0.9f, spriteEffects);
                Color color49 = projectile.GetAlpha(Color.White) * Utils.Remap(projectile.ai[0], 0f, killGateValue, 0.5f, 0f);
                color49.A = 0;

                for (int i = 0; i < 4; i++)
                    Main.EntitySpriteDraw(texture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY) + projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver2 * (float)i) * 2f * vector39, value26, color49, projectile.rotation, origin12, vector39, spriteEffects);

                Main.EntitySpriteDraw(texture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), value26, alpha5, projectile.rotation, origin12, vector39, spriteEffects);

                shouldDrawBool = false;
            }

            if (projectile.type == ProjectileID.Skull && projectile.ai[0] == -2f)
            {
                Main.instance.LoadProjectile(ProjectileID.BoneGloveProj);
                Texture2D crossbone = TextureAssets.Projectile[ProjectileID.BoneGloveProj].Value;

                Main.spriteBatch.Draw(crossbone, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, crossbone.Size() / 2f, projectile.scale, projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
                return false;
            }

            if (projectile.type == ProjectileID.DemonSickle)
            {
                if (!(Main.wofNPCIndex < 0 || !Main.npc[Main.wofNPCIndex].active || Main.npc[Main.wofNPCIndex].life <= 0 || projectile.tileCollide))
                {
                    Texture2D texture = ExtraTextureRefs.WallOfFleshDemonSickleTexture.Value;
                    int frameHeight = texture.Height / Main.projFrames[projectile.type];
                    int frameY = frameHeight * projectile.frame;
                    Rectangle rectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
                    Vector2 origin = rectangle.Size() / 2f;

                    SpriteEffects spriteEffects = SpriteEffects.None;
                    if (projectile.spriteDirection == -1)
                        spriteEffects = SpriteEffects.FlipHorizontally;

                    Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), rectangle, projectile.GetAlpha(lightColor), projectile.rotation, origin, projectile.scale, spriteEffects, 0f);
                    return false;
                }
            }

            // Manual drawing to adjust for scale change
            if (projectile.type == ProjectileID.Terragrim || projectile.type == ProjectileID.Arkhalis)
            {
                Texture2D tex = TextureAssets.Projectile[projectile.type].Value;
                Rectangle frame = tex.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
                Vector2 origin = frame.Size() / 2f;
                SpriteEffects spriteEffects = projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                Main.spriteBatch.Draw(tex, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), frame, projectile.GetAlpha(lightColor), projectile.rotation, origin, projectile.scale, spriteEffects, 0f);
                shouldDrawBool = false;
            }

            if (Main.zenithWorld)
            {
                if (NPC.AnyNPCs(NPCType<NPCs.CeaselessVoid.CeaselessVoid>()))
                {
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
                    var midnightShader = Terraria.Graphics.Shaders.GameShaders.Armor.GetShaderFromItemId(ItemID.MidnightRainbowDye);
                    midnightShader.Apply();
                }
            }

            return shouldDrawBool;
        }
        #endregion

        #region Pre Kill
        public override bool PreKill(Projectile projectile, int timeLeft)
        {
            bool revQueenBeeBeeHive = projectile.type == ProjectileID.BeeHive && (CalamityWorld.revenge || BossRushEvent.BossRushActive) && (projectile.ai[2] == 1f || CalamityWorld.death);

            if (revQueenBeeBeeHive)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath1, projectile.Center);
                for (int num573 = 0; num573 < 30; num573++)
                {
                    int num574 = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.t_Honey);
                    if (Main.rand.NextBool())
                    {
                        Dust dust2 = Main.dust[num574];
                        dust2.scale *= 1.4f;
                    }

                    projectile.velocity *= 1.9f;
                }
            }

            if (projectile.owner == Main.myPlayer)
            {
                if (revQueenBeeBeeHive)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int beeAmt = Main.rand.Next(2, 5 + 1);
                        int totalBeesAlive = NPC.CountNPCS(NPCID.Bee) + NPC.CountNPCS(NPCID.BeeSmall);
                        int finalBeeAmtToSpawn = NPC.AnyNPCs(NPCID.QueenBee) ? Math.Min(beeAmt, (CalamityWorld.death ? 9 : 15) - totalBeesAlive) : NPC.GetAvailableAmountOfNPCsToSpawnUpToSlot(beeAmt);
                        for (int i = 0; i < finalBeeAmtToSpawn; i++)
                        {
                            int beeType = Main.rand.NextBool() ? NPCID.Bee : NPCID.BeeSmall;
                            if (Main.zenithWorld)
                            {
                                beeType = Main.rand.NextBool(3) ? NPCType<PlagueChargerLarge>() : NPCType<PlagueCharger>();
                            }
                            else if (CalamityWorld.death || BossRushEvent.BossRushActive)
                            {
                                switch (Main.rand.Next(12))
                                {
                                    default:
                                    case 0:
                                    case 1:
                                    case 2:
                                    case 3:
                                    case 4:
                                    case 5:
                                        break;

                                    case 6:
                                    case 7:
                                    case 8:
                                        beeType = NPCID.LittleHornetHoney;
                                        break;

                                    case 9:
                                    case 10:
                                        beeType = NPCID.HornetHoney;
                                        break;

                                    case 11:
                                        beeType = NPCID.BigHornetHoney;
                                        break;
                                }
                            }

                            int beeSpawn = NPC.NewNPC(projectile.GetSource_FromThis(), (int)projectile.Center.X, (int)projectile.Center.Y, beeType, 1);
                            Main.npc[beeSpawn].velocity.X = (float)Main.rand.Next(-200, 201) * 0.002f;
                            Main.npc[beeSpawn].velocity.Y = (float)Main.rand.Next(-200, 201) * 0.002f;
                            Main.npc[beeSpawn].ai[3] = 1f;
                            Main.npc[beeSpawn].timeLeft = 600;
                            Main.npc[beeSpawn].netUpdate = true;
                        }
                    }

                    if (Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendData(MessageID.KillProjectile, -1, -1, null, projectile.identity, projectile.owner);
                }
            }

            if (revQueenBeeBeeHive)
            {
                projectile.active = false;
                return false;
            }

            return true;
        }
        #endregion

        #region Kill
        public override void OnKill(Projectile projectile, int timeLeft)
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (projectile.owner == Main.myPlayer)
            {
                if (!projectile.npcProj && !projectile.trap)
                {
                    if (projectile.CountsAsClass<RogueDamageClass>())
                    {
                        if (modPlayer.etherealExtorter && extorterBoost && Main.player[projectile.owner].ownedProjectileCounts[ProjectileType<LostSoulFriendly>()] < 5)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);

                                int damage = (int)player.GetTotalDamage<RogueDamageClass>().ApplyTo(EtherealExtorter.soulDamage);

                                int soul = Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, velocity, ProjectileType<LostSoulFriendly>(), damage, 0f, projectile.owner);
                                Main.projectile[soul].tileCollide = false;
                                if (soul.WithinBounds(Main.maxProjectiles))
                                    Main.projectile[soul].DamageType = DamageClass.Generic;
                            }
                            extorterBoost = false;
                        }

                        // Make sure the spike doesn't spawn again if it's already been spawned by on-hit.
                        if (modPlayer.scuttlersJewel && stealthStrike && !JewelSpikeSpawned && !CannotProc)
                        {
                            int damage = (int)player.GetTotalDamage<RogueDamageClass>().ApplyTo(16);

                            int spike = Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Vector2.Zero, ProjectileType<JewelSpike>(), damage, projectile.knockBack, projectile.owner);
                            Main.projectile[spike].frame = 4;
                            if (spike.WithinBounds(Main.maxProjectiles))
                                Main.projectile[spike].DamageType = DamageClass.Generic;
                        }
                    }
                }
            }
        }
        #endregion
    }
}
