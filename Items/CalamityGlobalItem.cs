using System;
using System.IO;
using CalamityMod.Balancing;
using CalamityMod.Buffs.Potions;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.Enums;
using CalamityMod.Events;
using CalamityMod.ExtraJumps;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Ammo;
using CalamityMod.Items.Armor.Bloodflare;
using CalamityMod.Items.Armor.GodSlayer;
using CalamityMod.Items.Armor.Hydrothermic;
using CalamityMod.Items.Armor.Prismatic;
using CalamityMod.Items.Armor.Reaver;
using CalamityMod.Items.Armor.Tarragon;
using CalamityMod.Items.Armor.Victide;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.Items.Tools;
using CalamityMod.Items.VanillaArmorChanges;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.Other;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Healing;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.UI;
using CalamityMod.UI.CalamitasEnchants;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items
{
    public partial class CalamityGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        private BitsByte flag0 = 0;

        #region Chargeable Item Variables
        /// <summary>
        /// If set to true, this item will consume <see cref="Charge"/> on use.<br/>
        /// Be sure to also set <see cref="MaxCharge"/> and <see cref="ChargePerUse"/>.
        /// </summary>
        public bool UsesCharge
        {
            get => flag0[0];
            set => flag0[0] = value;
        }
        /// <summary> The current charge value of this item. </summary>
        public float Charge = 0f;
        /// <summary> The maximum charge value of this item. Should only be set if <see cref="UsesCharge"/> is set to true. </summary>
        public float MaxCharge = 1f;
        /// <summary> The charge consumed on each use of this item. Should only be set if <see cref="UsesCharge"/> is set to true. </summary>
        public float ChargePerUse = 0f;
        /// <summary>
        /// By default, the right-click use of an item will use <see cref="ChargePerUse"/> to determine how much charge to consume.<br/>
        /// Set to a value other than -1 to make the right-click use a different amount of charge.
        /// </summary>
        public float ChargePerAltUse = -1f;
        public float ChargeRatio
        {
            get
            {
                float ratio = Charge / MaxCharge;
                return float.IsNaN(ratio) || float.IsInfinity(ratio) ? 0f : MathHelper.Clamp(ratio, 0f, 1f);
            }
        }
        #endregion

        #region Enchantment Variables
        /// <summary> If set to true, this item cannot receive enchantments from the Brimstone Witch. </summary>
        public bool CannotBeEnchanted
        {
            get => flag0[1];
            set => flag0[1] = value;
        }
        /// <summary> Stores the current enchantment placed on this item. If set to null, this item has no enchantment. </summary>
        public Enchantment? AppliedEnchantment = null;
        /// <summary>
        /// Stores the "exhaustion" value of this item for the Ephemeral enchantment.<br/>
        /// The ratio of this value to the maximum value is used as a lerp value for the damage multiplier.
        /// </summary>
        public float DischargeEnchantExhaustion = 0;
        public float DischargeExhaustionRatio
        {
            get
            {
                float ratio = DischargeEnchantExhaustion / DischargeEnchantExhaustionCap;
                return float.IsNaN(ratio) || float.IsInfinity(ratio) ? 0f : MathHelper.Clamp(ratio, 0f, 1f);
            }
        }
        /// <summary> Constant value storing the maximum value for the Ephemeral enchantment's "exhaustion" value. </summary>
        public const float DischargeEnchantExhaustionCap = 1600f;
        /// <summary> The minimum damage multiplier for weapons with the Ephemeral enchantment. </summary>
        public const float DischargeEnchantMinDamageFactor = 0.77f;
        /// <summary> The maximum damage multiplier for weapons with the Ephemeral enchantment. </summary>
        public const float DischargeEnchantMaxDamageFactor = 1.26f;
        #endregion

        // Miscellaneous stuff
        /// <summary>
        /// Set to true if this item can only be obtained in Revengeance Mode.<br/>
        /// Adds "Revengeance" to the bottom of the item's tooltip.
        /// </summary>
        public bool revengeanceItem
        {
            get => flag0[2];
            set => flag0[2] = value;
        }
        /// <summary>
        /// Set to true if this item is dedicated to a Patreon donator.<br/>
        /// Adds "- Donor Item -" to the bottom of the item's tooltip.
        /// </summary>
        public bool donorItem
        {
            get => flag0[3];
            set => flag0[3] = value;
        }
        /// <summary>
        /// Set to true if this item is dedicated to a Calamity developer.<br/>
        /// Adds "- Dev Item -" to the bottom of the item's tooltip.
        /// </summary>
        public bool devItem
        {
            get => flag0[4];
            set => flag0[4] = value;
        }

        public static readonly Color ExhumedTooltipColor = new Color(198, 27, 64);

        // Ozzatron 21MAY2022: This function is required by TML 1.4's new clone behavior.
        // This behavior is sadly mandatory because there are a few places in vanilla Terraria which use cloning.
        // Most notably: reforging and item tooltips.
        //
        // It manually copies everything because I don't trust the base clone behavior after seeing the insane bugs.
        //
        // ANY TIME YOU ADD A VARIABLE TO CalamityGlobalItem, IT MUST BE COPIED IN THIS FUNCTION.
        public override GlobalItem Clone(Item item, Item itemClone)
        {
            CalamityGlobalItem myClone = (CalamityGlobalItem)base.Clone(item, itemClone);

            // BitFlags
            myClone.flag0 = flag0;

            // Charge (Draedon's Arsenal)
            myClone.UsesCharge = UsesCharge;
            myClone.Charge = Charge;
            myClone.MaxCharge = MaxCharge;
            myClone.ChargePerUse = ChargePerUse;
            myClone.ChargePerAltUse = ChargePerAltUse;

            // Enchantments
            myClone.AppliedEnchantment = AppliedEnchantment.HasValue ? AppliedEnchantment.Value : null;
            myClone.DischargeEnchantExhaustion = DischargeEnchantExhaustion;

            return myClone;
        }

        #region SetDefaults
        public override void SetStaticDefaults()
        {
            SetStaticDefaults_ShimmerRecipes();

            #region Vanilla Wing Tweaks
            // Shorthand to make looking at this easier
            WingStats[] stats = ArmorIDs.Wing.Sets.Stats;

            // 130 -> 160 flight time, 6.75 -> 7.5 horizontal speed
            stats[(int)VanillaWingID.HarpyWings].FlyTime = 160;
            stats[(int)VanillaWingID.HarpyWings].AccRunSpeedOverride = 7.5f;
            // 6.75 -> 9 horizontal speed, 1 -> 1.5 acceleration multiplier
            stats[(int)VanillaWingID.FrozenWings].AccRunSpeedOverride = 9f;
            stats[(int)VanillaWingID.FrozenWings].AccRunAccelerationMult = 1.5f;
            // 160 -> 130 flight time
            stats[(int)VanillaWingID.FlameWings].FlyTime = 130;
            // 160 -> 180 flight time, 7.5 -> 6.75 horizontal speed
            stats[(int)VanillaWingID.BatWings].FlyTime = 180;
            stats[(int)VanillaWingID.BatWings].AccRunSpeedOverride = 6.75f;
            // 1 -> 1.5 acceleration multiplier
            stats[(int)VanillaWingID.ButterflyWings].AccRunAccelerationMult = 1.5f;

            // 170 -> 240 flight time
            stats[(int)VanillaWingID.BoneWings].FlyTime = 240;
            // 160 -> 170 flight time, 7.5 -> 9 horizontal speed, 1 -> 2 acceleration multiplier
            stats[(int)VanillaWingID.LeafWings].FlyTime = 170;
            stats[(int)VanillaWingID.LeafWings].AccRunSpeedOverride = 9f;
            stats[(int)VanillaWingID.LeafWings].AccRunAccelerationMult = 2f;
            // (Spectre Wings) 1 -> 1.5 acceleration multiplier
            stats[(int)VanillaWingID.GhostWings].AccRunAccelerationMult = 1.5f;

            // 170 -> 210 flight time
            stats[(int)VanillaWingID.BeetleWings].FlyTime = 210;
            // 180 -> 300 flight time
            stats[(int)VanillaWingID.TatteredFairyWings].FlyTime = 300;
            // (Empress Wings) 150 -> 120 flight time
            stats[(int)VanillaWingID.RainbowWings].FlyTime = 120;

            // 12 -> 10.8 hover stats
            stats[(int)VanillaWingID.BejeweledValkyrieWing].DownHoverSpeedOverride = 10.8f; // (Lazure)
            stats[(int)VanillaWingID.BejeweledValkyrieWing].DownHoverAccelerationMult = 10.8f;
            stats[(int)VanillaWingID.Yoraiz0rWings].DownHoverSpeedOverride = 10.8f;
            stats[(int)VanillaWingID.Yoraiz0rWings].DownHoverAccelerationMult = 10.8f;
            stats[(int)VanillaWingID.SkiphsWings].DownHoverSpeedOverride = 10.8f;
            stats[(int)VanillaWingID.SkiphsWings].DownHoverAccelerationMult = 10.8f;
            stats[(int)VanillaWingID.BetsyWings].DownHoverSpeedOverride = 10.8f;
            stats[(int)VanillaWingID.BetsyWings].DownHoverAccelerationMult = 10.8f;

            // (Celestial Starboard) 4.5 -> 2.75 acceleration multiplier, 16 -> 12 hover stats
            stats[(int)VanillaWingID.LongRainbowTrailWings].AccRunAccelerationMult = 2.75f;
            stats[(int)VanillaWingID.LongRainbowTrailWings].DownHoverSpeedOverride = 12f;
            stats[(int)VanillaWingID.LongRainbowTrailWings].DownHoverAccelerationMult = 12f;
            #endregion
        }

        public override void SetDefaults(Item item)
        {
            // Accessories can never be enchanted, to prevent Shield of Cthulhu & High Ruler from being enchantable
            if (item.accessory)
                CannotBeEnchanted = true;

            // Music boxes are pre-boss and sold from the Merchant, so they should now use the blue rarity.
            if (item.type == ItemID.MusicBox || item.createTile == TileID.MusicBoxes || ItemID.Sets.ShimmerTransformToItem[item.type] == ItemID.MusicBox)
                item.rare = ItemRarityID.Blue;

            // Modified Pearlwood items are now Light Red.
            if (item.type == ItemID.PearlwoodBow || item.type == ItemID.PearlwoodHammer || item.type == ItemID.PearlwoodSword)
                item.rare = ItemRarityID.LightRed;

            //Flame Waker Boots are no longer just vanity and therefore lose the vanity tooltip.
            if (item.type == ItemID.FlameWakerBoots)
                item.vanity = false;

            // Volatile Gelatin is pre-mech post-WoF so it should use the pink rarity.
            if (item.type == ItemID.VolatileGelatin)
                item.rare = ItemRarityID.Pink;

            // Soaring Insignia is post-Golem so it should use the yellow rarity.
            if (item.type == ItemID.EmpressFlightBooster)
                item.rare = ItemRarityID.Yellow;

            // Zenith rarity
            if (item.type == ItemID.Zenith)
                item.rare = RarityType<BurnishedAuric>();

            // CIT 16OCT2025: Following overwhelming dev vote, Expert+ drops are NOT available in Classic
            /*switch (item.type)
            {
                case ItemID.RoyalGel:
                case ItemID.EoCShield:
                case ItemID.WormScarf:
                case ItemID.BrainOfConfusion:
                case ItemID.HiveBackpack:
                case ItemID.BoneHelm:
                case ItemID.BoneGlove:
                // case ItemID.DemonHeart:
                case ItemID.VolatileGelatin:
                case ItemID.MechanicalBatteryPiece:
                case ItemID.MechanicalWagonPiece:
                case ItemID.MechanicalWheelPiece:
                case ItemID.MinecartMech:
                case ItemID.SporeSac:
                case ItemID.WitchBroom:
                case ItemID.EmpressFlightBooster:
                case ItemID.ShinyStone:
                case ItemID.ShrimpyTruffle:
                case ItemID.GravityGlobe:
                case ItemID.SuspiciousLookingTentacle:
                case ItemID.LongRainbowTrailWings:
                    item.expert = false;
                    break;
            }*/

            // Allow Souls to be used as ammo for SHPC.
            if (item.type == ItemID.SoulofLight || item.type == ItemID.SoulofNight || item.type == ItemID.SoulofFlight || item.type == ItemID.SoulofMight || item.type == ItemID.SoulofSight || item.type == ItemID.SoulofFright)
            {
                item.ammo = ItemID.SoulofLight;
                item.notAmmo = true; // Prevents them from showing an "Ammo" tooltip or going to ammo slots.
            }

            // Increase how much health Mushrooms heal.
            if (item.type == ItemID.Mushroom && item.healLife < 25)
                item.healLife = 25;

            // Allow Beam Sword to change direction when it fires, because vanilla disables it for some reason.
            if (item.type == ItemID.BeamSword)
                item.ChangePlayerDirectionOnShoot = true;

            // Apply Calamity Global Item Tweaks.
            SetDefaults_ApplyTweaks(item);

            // Items which are "classic true melee" (melee items with no fired projectile) are automatically reclassed as True Melee class.
            if (item.shoot == ProjectileID.None)
            {
                if (item.DamageType == DamageClass.Melee)
                    item.DamageType = TrueMeleeDamageClass.Instance;
                else if (item.DamageType == DamageClass.MeleeNoSpeed)
                    item.DamageType = TrueMeleeNoSpeedDamageClass.Instance;
            }
        }
        #endregion

        #region Shoot
        public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockBack)
        {
            CalamityPlayer modPlayer = player.Calamity();

            if (item.CountsAsClass<RogueDamageClass>())
            {
                velocity *= modPlayer.rogueVelocity;
                if (modPlayer.gloveOfRecklessness)
                    velocity = velocity.RotatedByRandom(MathHelper.ToRadians(12f));
            }
        }

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockBack)
        {
            CalamityPlayer modPlayer = player.Calamity();
            var playerSource = player.GetSource_FromThis();
            Vector2 mouse = player.ClampedMouseWorld();

            if (Main.myPlayer == player.whoAmI && player.Calamity().cursedSummonsEnchant)
            {
                if (NPC.CountNPCS(NPCType<CalamitasEnchantDemon>()) < 2)
                {
                    CalamityNetcode.NewNPC_ClientSide(mouse, NPCType<CalamitasEnchantDemon>(), player);
                    SoundEngine.PlaySound(SoundID.DD2_DarkMageSummonSkeleton, mouse);
                }
            }

            // Traitorous enchantment implementation
            // Previously, this enchant was a 1/12 chance to fire a projectile for 165,000 base damage (yes, 165,000) when below half mana
            // This was so unbelievably overpowered that with RNG it was possible to kill Exo Mechs in 20 seconds
            // 
            // Traitorous has been reworked to be a guaranteed effect below 25% mana, which removes all your remaining mana
            bool belowManaThreshold = player.statMana < player.statManaMax2 * 0.25f;
            bool traitorousAlreadyInPlay = player.ownedProjectileCounts[ProjectileType<ManaMonster>()] > 0;
            if (Main.myPlayer == player.whoAmI && player.Calamity().manaMonsterEnchant && !traitorousAlreadyInPlay && belowManaThreshold)
            {
                // Calculate how much damage to deal based on how much mana was consumed
                int remainingMana = player.statMana;
                int damagePerManaConsumed = 80;
                int monsterDamage = (int)player.GetTotalDamage<MagicDamageClass>().ApplyTo(remainingMana * damagePerManaConsumed);

                // Spawn the Mana Monster
                Vector2 shootVelocity = player.SafeDirectionTo(mouse, -Vector2.UnitY).RotatedByRandom(0.07f) * Main.rand.NextFloat(4f, 5f);
                Projectile.NewProjectile(source, player.Center + shootVelocity, shootVelocity, ProjectileType<ManaMonster>(), monsterDamage, 0f, player.whoAmI);

                // Set the player's mana to zero.
                player.statMana = 0;
            }

            if (modPlayer.bloodflareMage && modPlayer.canFireBloodflareMageProjectile)
            {
                if (item.CountsAsClass<MagicDamageClass>() && !item.channel)
                {
                    modPlayer.canFireBloodflareMageProjectile = false;
                    if (player.whoAmI == Main.myPlayer)
                    {
                        int bloodflareBoltDamage = CalamityUtils.DamageSoftCap(damage * BloodflareHeadMagic.GhostBoltDamageRatio, BloodflareHeadMagic.GhostBoltonDamageSoftcap);
                        Projectile.NewProjectile(playerSource, position, velocity, ProjectileType<GhostlyBolt>(), bloodflareBoltDamage, 1f, player.whoAmI);
                    }
                }
            }
            if (modPlayer.bloodflareRanged && modPlayer.canFireBloodflareRangedProjectile)
            {
                if (item.CountsAsClass<RangedDamageClass>() && !item.channel)
                {
                    modPlayer.canFireBloodflareRangedProjectile = false;
                    if (player.whoAmI == Main.myPlayer)
                    {
                        int bloodsplosionDamage = CalamityUtils.DamageSoftCap(damage * BloodflareHeadRanged.BloodBombDamageRatio, BloodflareHeadRanged.BloodBombDamageSoftcap);
                        Projectile.NewProjectile(playerSource, position, velocity, ProjectileType<BloodBomb>(), bloodsplosionDamage, 2f, player.whoAmI);
                    }
                }
            }
            if (modPlayer.tarraMage && !item.channel)
            {
                if (modPlayer.tarraCrits >= TarragonHeadMagic.CritsToSpawnLeaves && player.whoAmI == Main.myPlayer)
                {
                    modPlayer.tarraCrits = 0;
                    // Tarragon Mage Leaves: (8-10) x 20%, soft cap starts at 200 base damage
                    int leafAmt = 8 + Main.rand.Next(3); // 8, 9, or 10
                    int leafDamage = (int)(damage * TarragonHeadMagic.LeafDamageRatio);

                    for (int l = 0; l < leafAmt; l++)
                    {
                        float spreadMult = 0.025f * l;
                        float xDiff = velocity.X + Main.rand.Next(-25, 26) * spreadMult;
                        float yDiff = velocity.Y + Main.rand.Next(-25, 26) * spreadMult;
                        float speed = velocity.Length();
                        speed = item.shootSpeed / speed;
                        xDiff *= speed;
                        yDiff *= speed;
                        int projectile = Projectile.NewProjectile(playerSource, position, new Vector2(xDiff, yDiff), ProjectileID.Leaf, leafDamage, knockBack, player.whoAmI);
                        if (projectile.WithinBounds(Main.maxProjectiles))
                            Main.projectile[projectile].DamageType = DamageClass.Generic;
                    }
                }
            }
            if (modPlayer.ataxiaBolt && modPlayer.canFireAtaxiaRangedProjectile)
            {
                if (item.CountsAsClass<RangedDamageClass>() && !item.channel)
                {
                    modPlayer.canFireAtaxiaRangedProjectile = false;
                    if (player.whoAmI == Main.myPlayer)
                    {
                        int ataxiaFlareDamage = (int)(damage * HydrothermicHeadRanged.FlareDamageRatio);
                        Projectile.NewProjectile(playerSource, position, velocity * 1.25f, ProjectileType<HydrothermicFlare>(), ataxiaFlareDamage, 2f, player.whoAmI);
                    }
                }
            }
            if (modPlayer.godSlayerRanged && modPlayer.canFireGodSlayerRangedProjectile)
            {
                if (item.CountsAsClass<RangedDamageClass>() && !item.channel)
                {
                    modPlayer.canFireGodSlayerRangedProjectile = false;
                    if (player.whoAmI == Main.myPlayer)
                    {
                        int shrapnelRoundDamage = CalamityUtils.DamageSoftCap(damage * GodSlayerHeadRanged.ShrapnelRoundDamageRatio, GodSlayerHeadRanged.ShrapnelRoundDamageSoftcap);
                        Projectile.NewProjectile(playerSource, position, velocity * 1.25f, ProjectileType<GodSlayerShrapnelRound>(), shrapnelRoundDamage, 2f, player.whoAmI);
                    }
                }
            }
            if (modPlayer.ataxiaVolley && modPlayer.canFireAtaxiaRogueProjectile)
            {
                if (item.CountsAsClass<ThrowingDamageClass>() && !item.channel)
                {
                    modPlayer.canFireAtaxiaRogueProjectile = false;
                    int flareID = ProjectileType<HydrothermicFlareRogue>();
                    int flareDamage = CalamityUtils.DamageSoftCap(HydrothermicHeadRogue.VolleyDamage + damage * HydrothermicHeadRogue.VolleyDamageRatio, HydrothermicHeadRogue.VolleyDamageSoftcap);

                    if (player.whoAmI == Main.myPlayer)
                    {
                        SoundEngine.PlaySound(SoundID.Item20, player.Center);
                        for (int i = 0; i < 6; i++)
                        {
                            Vector2 circleVel = (MathHelper.TwoPi * i / 6f + velocity.ToRotation()).ToRotationVector2() * 5f;
                            Projectile.NewProjectile(playerSource, player.Center, circleVel, flareID, flareDamage, 1f, player.whoAmI);
                        }
                    }
                }
            }
            if (modPlayer.prismaticRegalia)
            {
                if (item.CountsAsClass<MagicDamageClass>() && Main.rand.NextBool(PrismaticRegalia.RocketChanceDenominator) && !item.channel)
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        for (int i = -5; i <= 5; i += 5)
                        {
                            if (i != 0)
                            {
                                Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.ToRadians(i));
                                int rocket = Projectile.NewProjectile(playerSource, position, perturbedSpeed, ProjectileType<ScorpioRocket>(), (int)(damage * PrismaticRegalia.RocketDamageRatio), 2f, player.whoAmI, 0, 12f);
                                //First extra value is rocket type which I just used 0 to get the default, second is the velocity I went with my gut feeling and got quite close to Scorpio's velocity with Rockets I
                                if (rocket.WithinBounds(Main.maxProjectiles))
                                    Main.projectile[rocket].DamageType = DamageClass.Generic;
                            }
                        }
                    }
                }
            }
            if (modPlayer.victideSet)
            {
                if ((item.CountsAsClass<RangedDamageClass>() || item.CountsAsClass<MeleeDamageClass>() || item.CountsAsClass<MagicDamageClass>() ||
                    item.CountsAsClass<ThrowingDamageClass>() || item.CountsAsClass<SummonDamageClass>()) &&
                    Main.rand.NextBool(10) && !item.channel)
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        // Victide All-class Seashells: 200%, soft cap starts at 46 base damage
                        int seashellDamage = CalamityUtils.DamageSoftCap(damage * 2, 46);

                        Projectile.NewProjectile(source, position, velocity * 1.25f, ModContent.ProjectileType<Seashell>(), seashellDamage, 1f, player.whoAmI);
                    }
                }
            }

            return true;
        }
        #endregion

        #region Saving And Loading
        public override void SaveData(Item item, TagCompound tag)
        {
            tag.Add("charge", Charge);
            tag.Add("enchantmentID", AppliedEnchantment.HasValue ? AppliedEnchantment.Value.ID : 0);
            tag.Add("DischargeEnchantExhaustion", DischargeEnchantExhaustion);
        }

        public override void LoadData(Item item, TagCompound tag)
        {
            // Changed charge from int to float. If an old charge int is present, load that instead.
            if (tag.ContainsKey("Charge"))
                Charge = tag.GetInt("Charge");
            else
                Charge = tag.GetFloat("charge");

            DischargeEnchantExhaustion = tag.GetFloat("DischargeEnchantExhaustion");
            Enchantment? savedEnchantment = EnchantmentManager.FindByID(tag.GetInt("enchantmentID"));
            if (savedEnchantment.HasValue)
            {
                AppliedEnchantment = savedEnchantment.Value;
                bool hasCreationEffect = AppliedEnchantment.Value.CreationEffect != null;
                item.Calamity().AppliedEnchantment.Value.CreationEffect?.Invoke(item);
            }
        }

        public override void NetSend(Item item, BinaryWriter writer)
        {
            writer.Write(Charge);
            writer.Write(AppliedEnchantment.HasValue ? AppliedEnchantment.Value.ID : 0);
            writer.Write(DischargeEnchantExhaustion);
        }

        public override void NetReceive(Item item, BinaryReader reader)
        {
            Charge = reader.ReadSingle();

            Enchantment? savedEnchantment = EnchantmentManager.FindByID(reader.ReadInt32());
            if (savedEnchantment.HasValue)
            {
                AppliedEnchantment = savedEnchantment.Value;
                bool hasCreationEffect = AppliedEnchantment.Value.CreationEffect != null;
                if (hasCreationEffect)
                    item.Calamity().AppliedEnchantment.Value.CreationEffect(item);
            }
            DischargeEnchantExhaustion = reader.ReadSingle();
        }
        #endregion

        #region Pickup Item Changes
        public override bool CanPickup(Item item, Player player)
        {
            // Prevent Mana Stars from being picked up while wielding Ion Blaster or Apoctosis Array
            if (item.type == ItemID.Star || item.type == ItemID.SoulCake || item.type == ItemID.SugarPlum)
            {
                if (player.HeldItem.type == ItemType<IonBlaster>() || player.HeldItem.type == ItemType<ApoctosisArray>())
                    return false;
            }
            return base.CanPickup(item, player);
        }

        public override bool OnPickup(Item item, Player player)
        {
            if (item.type == ItemID.Heart || item.type == ItemID.CandyApple || item.type == ItemID.CandyCane) // On heart pickup
            {
                if (player.Calamity().photosynthesis)
                    player.HealPlayer(PhotosynthesisPotion.IncreasedHeartHeal);
            }
            return true;
        }
        #endregion

        #region Use Item Changes
        public override void HoldItem(Item item, Player player)
        {
            if (player.Calamity().ChaosStone && item.mana == 0 && !player.ItemTimeIsZero)
            {
                player.manaRegenDelay = player.maxRegenDelay;
            }
        }

        public override bool? UseItem(Item item, Player player)
        {
            var modPlayer = player.Calamity();

            if (Main.zenithWorld && item.type == ItemID.RodOfHarmony)
            {
                if (NPC.AnyNPCs(NPCType<THELORDE>()))
                {
                    //one hour of NOU when using rod of harmony while LORDE is alive
                    player.AddBuff(BuffType<NOU>(), 3600 * 60);
                }
            }

            // Give 1 minute of Mushy buff when consuming Mushrooms with Fungal Symbiote equipped.
            if (item.type == ItemID.Mushroom && player.Calamity().fungalSymbiote)
                player.AddBuff(BuffType<Mushy>(), 3600);

            // Healing item interactions
            if (item.healLife > 0)
            {
                // Jelly aura spawning
                // This runs twice per use for whatever reason so we need a stopping player variable
                if (player.whoAmI == Main.myPlayer && !modPlayer.spawnedJellyAura)
                {
                    if (modPlayer.absorber)
                        Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ProjectileType<AbsorberAura>(), 0, 0, player.whoAmI);
                    else if (modPlayer.GrandGelatin)
                        Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ProjectileType<GreenJellyAura>(), 0, 0, player.whoAmI);
                    else
                    {
                        if (modPlayer.cleansingjelly)
                            Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ProjectileType<BlueJellyAura>(), 0, 0, player.whoAmI);
                        if (modPlayer.lifejelly)
                            Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ProjectileType<PinkJellyAura>(), 0, 0, player.whoAmI);
                    }
                    modPlayer.spawnedJellyAura = true;
                }

                // Trigger Bloom Stone's heal over time from healing items
                if (modPlayer.bloomStone)
                {
                    // Temporarily disable Bloom Stone so that GetHealLife doesn't return 0
                    modPlayer.bloomStone = false;
                    modPlayer.bloomStoneTotalHeal = modPlayer.bloomStoneHealPool = player.GetHealLife(item);
                    modPlayer.bloomStone = true;
                }
            }

            //Mana Potion interactions
            if (item.healMana > 0 || player.HasBuff(ModContent.BuffType<AstralInjectionBuff>()))
            {
                //If mana potion used, kill all active Crown projectiles 
                if ((modPlayer.moonCrown || modPlayer.featherCrown) && modPlayer.mageCrownCount > 0)
                {
                    modPlayer.mageCrownTimer = 300;
                    modPlayer.mageCrownCount = 0;
                }
            }

            // Staff/Axe of Regrowth growing Calamity grass
            if (item.type == ItemID.StaffofRegrowth || item.type == ItemID.AcornAxe)
            {
                Tile tile = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY);
                Tile tileAbove = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY - 1);

                if (tile.HasTile && !tileAbove.HasTile && tileAbove.LiquidAmount == 0 && tile.TileType == TileType<Tiles.Crags.ScorchedRemains>() && player.IsInTileInteractionRange(Player.tileTargetX, Player.tileTargetY, TileReachCheckSettings.Simple))
                {
                    Main.tile[Player.tileTargetX, Player.tileTargetY].TileType = (ushort)TileType<Tiles.Crags.ScorchedRemainsGrass>();

                    SoundEngine.PlaySound(SoundID.Dig, player.Center);
                    return true;
                }
                else if (tile.HasTile && tile.TileType == TileType<Tiles.Astral.AstralDirt>() && player.IsInTileInteractionRange(Player.tileTargetX, Player.tileTargetY, TileReachCheckSettings.Simple))
                {
                    Main.tile[Player.tileTargetX, Player.tileTargetY].TileType = (ushort)TileType<Tiles.Astral.AstralGrass>();

                    SoundEngine.PlaySound(SoundID.Dig, player.Center);
                    return true;
                }
            }

            return base.UseItem(item, player);
        }

        public override bool AltFunctionUse(Item item, Player player)
        {
            if (player.Calamity().profanedCrystalBuffs && item.pick == 0 && item.axe == 0 && item.hammer == 0 && item.autoReuse && (item.CountsAsClass<ThrowingDamageClass>() || item.CountsAsClass<MagicDamageClass>() || item.CountsAsClass<RangedDamageClass>() || item.CountsAsClass<MeleeDamageClass>() || item.CountsAsClass<SummonMeleeSpeedDamageClass>()))
            {
                return false;
            }
            if (player.HeldItem.type == ItemType<GlacialEmbrace>())
            {
                bool canContinue = true;
                int count = 0;
                foreach (Projectile p in Main.ActiveProjectiles)
                {
                    if (p.type == ProjectileType<GlacialEmbracePointyThing>() && p.owner == player.whoAmI)
                    {
                        if (p.ai[1] > 1f)
                        {
                            canContinue = false;
                            break;
                        }
                        else if (p.ai[1] == 0f)
                        {
                            if (((GlacialEmbracePointyThing)p.ModProjectile).circlingPlayer)
                                count++;
                        }
                    }
                }
                if (canContinue && count > 0)
                {
                    Vector2 mouse = player.ClampedMouseWorld();
                    NPC unluckyTarget = CalamityUtils.MinionHoming(mouse, 1000f, player);
                    if (unluckyTarget != null)
                    {
                        int pointyThingyAmount = count;
                        float angleVariance = MathHelper.TwoPi / pointyThingyAmount;
                        float angle = 0f;

                        var source = player.GetSource_ItemUse(player.HeldItem);
                        for (int i = 0; i < pointyThingyAmount; i++)
                        {
                            if (Main.projectile.Length == Main.maxProjectiles)
                                break;
                            int GlacialEmbraceDamage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(80);
                            int projj = Projectile.NewProjectile(source, mouse, Vector2.Zero, ProjectileType<GlacialEmbracePointyThing>(), GlacialEmbraceDamage, 1f, player.whoAmI, angle, 2f);
                            Main.projectile[projj].originalDamage = 80;

                            angle += angleVariance;
                            for (int j = 0; j < 22; j++)
                            {
                                Dust dust = Dust.NewDustDirect(Main.projectile[projj].position, Main.projectile[projj].width, Main.projectile[projj].height, DustID.Ice);
                                dust.velocity = Vector2.UnitY * Main.rand.NextFloat(3f, 5.5f) * Main.rand.NextBool().ToDirectionInt();
                                dust.noGravity = true;
                            }
                        }
                    }
                }
                return false;
            }
            return base.AltFunctionUse(item, player);
        }

        public override bool CanUseItem(Item item, Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            CalamityGlobalItem modItem = item.Calamity();

            // Restrict behavior when reading Dreadon's Log.
            if (PopupGUIManager.AnyGUIsActive)
                return false;

            if (player.ownedProjectileCounts[ProjectileType<RelicOfDeliveranceSpear>()] > 0 &&
                (item.damage > 0 || item.ammo != AmmoID.None))
            {
                return false; // Don't use weapons if you're charging with a spear
            }

            // If the player if using the Drill Containment Unit, ignore all this.
            // It will start to check for everything below EVERY FRAME, including attacking with PSC or using Charge
            if (player.mount.Type == MountID.Drill)
                return base.CanUseItem(item, player);

            // Conversion for Andromeda
            if (player.ownedProjectileCounts[ProjectileType<GiantIbanRobotOfDoom>()] > 0)
            {
                if (item.type == ItemID.WireKite)
                    return false;
                if (item.pick > 0 || item.axe > 0 || item.hammer > 0 || item.fishingPole > 0)
                    return false;
                // compiler optimization: && short-circuits, so if altFunctionUse != 0, Andromeda code is never called.
                if (item.CountsAsClass<ThrowingDamageClass>() || item.CountsAsClass<MagicDamageClass>() || item.CountsAsClass<RangedDamageClass>() || item.CountsAsClass<MeleeDamageClass>())
                    return player.altFunctionUse == 0 && FlamsteedRing.TransformItemUsage(item, player);
            }

            // Conversion for Profaned Soul Crystal
            bool autoreuse = item.autoReuse || item.CountsAsClass<SummonMeleeSpeedDamageClass>();
            if (modPlayer.profanedCrystalBuffs && item.pick == 0 && item.axe == 0 && item.hammer == 0 && autoreuse && (item.CountsAsClass<ThrowingDamageClass>() || item.CountsAsClass<MagicDamageClass>() || item.CountsAsClass<RangedDamageClass>() || item.CountsAsClass<MeleeDamageClass>() || item.CountsAsClass<SummonMeleeSpeedDamageClass>()))
                return player.altFunctionUse == 0 ? ProfanedSoulCrystal.TransformItemUsage(item, player) : AltFunctionUse(item, player);


            //TODO - This souldn't be here!
            if (!item.IsAir)
            {
                // Exhaust the weapon if it has the necessary enchant.
                if (modPlayer.dischargingItemEnchant)
                {
                    float exhaustionCost = item.useTime * 2.25f;
                    if (exhaustionCost < 10f)
                        exhaustionCost = 10f;
                    DischargeEnchantExhaustion = MathHelper.Clamp(DischargeEnchantExhaustion - exhaustionCost, 0.001f, DischargeEnchantExhaustionCap);
                }

                // Otherwise, if it doesn't, clear exhaustion.
                else
                    DischargeEnchantExhaustion = 0;
            }

            // Check for sufficient charge if this item uses charge.
            if (item.type >= ItemID.Count && modItem.UsesCharge)
            {
                // If attempting to use alt fire, and alt fire charge is defined, require that charge. Otherwise require normal charge per use.
                float chargeNeeded = (player.altFunctionUse == 2 && modItem.ChargePerAltUse != -1f) ? modItem.ChargePerAltUse : modItem.ChargePerUse;

                // If the amount of charge needed is zero or less, ignore the charge requirement entirely (e.g. summon staff right click).
                if (chargeNeeded > 0f)
                {
                    if (modItem.Charge < chargeNeeded)
                        return false;

                    // If you have enough charge, decrement charge on the spot because this hook runs exactly once every time you use an item.
                    // Mana has to be checked separately or you'll fail to use the weapon on a mana check later and still have consumed charge.
                    if (player.CheckMana(item) && item.ModItem.CanUseItem(player))
                        Charge -= chargeNeeded;
                }
            }

            // Handle general use-item effects for the Gem Tech Armor.
            player.Calamity().GemTechState.OnItemUseEffects(item);

            if (item.type == ItemID.RodofDiscord)
            {
                if (player.chaosState)
                    return false;

                Vector2 teleportLocation;
                teleportLocation.X = (float)Main.mouseX + Main.screenPosition.X;
                if (player.gravDir == 1f)
                {
                    teleportLocation.Y = (float)Main.mouseY + Main.screenPosition.Y - (float)player.height;
                }
                else
                {
                    teleportLocation.Y = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY;
                }
                teleportLocation.X -= (float)(player.width / 2);
                if (teleportLocation.X > 50f && teleportLocation.X < (float)(Main.maxTilesX * 16 - 50) && teleportLocation.Y > 50f && teleportLocation.Y < (float)(Main.maxTilesY * 16 - 50))
                {
                    int x = (int)teleportLocation.X / 16;
                    int y = (int)teleportLocation.Y / 16;
                    bool templeCheck = Main.tile[x, y].WallType != WallID.LihzahrdBrickUnsafe || y <= Main.worldSurface || NPC.downedPlantBoss;
                    if (templeCheck && !Collision.SolidCollision(teleportLocation, player.width, player.height))
                    {
                        int duration = CalamityPlayer.areThereAnyDamnBosses ? CalamityPlayer.chaosStateDuration : 360;
                        player.AddBuff(BuffID.ChaosState, duration, true);
                    }
                }
            }
            if (item.type == ItemID.SuspiciousLookingEye || item.type == ItemID.WormFood || item.type == ItemID.BloodySpine || item.type == ItemID.SlimeCrown || item.type == ItemID.BloodMoonStarter || item.type == ItemID.Abeemination || item.type == ItemID.DeerThing || item.type == ItemID.QueenSlimeCrystal || item.type == ItemID.MechanicalEye || item.type == ItemID.MechanicalWorm || item.type == ItemID.MechanicalSkull || item.type == ItemID.CelestialSigil)
            {
                return !BossRushEvent.BossRushActive;
            }
            return true;
        }
        #endregion

        #region Modify Weapon Damage
        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            if (item.type < ItemID.Count)
                return;

            // Summon weapons specifically do not have their damage affected by charge. They still require charge to function however.
            CalamityGlobalItem modItem = item.Calamity();

            if (!item.CountsAsClass<SummonDamageClass>() && modItem.DischargeEnchantExhaustion > 0f)
                damage *= DischargeEnchantmentDamageFormula();

            if (!item.CountsAsClass<SummonDamageClass>() && (modItem?.UsesCharge ?? false))
            {
                // At exactly zero charge, do not perform any multiplication.
                // This makes charge-using weapons show up at full damage when previewed in crafting, Recipe Browser, etc.
                if (Charge == 0f)
                    return;
                damage *= ChargeDamageFormula();
            }
        }

        internal float DischargeEnchantmentDamageFormula()
        {
            // This exponential has the properties of beginning at 0 and ending at 1, yet also has their signature rising curve.
            // It is therefore perfect for a potential interpolant.
            float interpolant = (float)Math.Pow(2D, DischargeExhaustionRatio) - 1f;

            // No further smoothening is required in the form of a Smoothstep remap.
            // A linear interpolation works fine; the exponential already has the desired curve shape.
            return MathHelper.Lerp(DischargeEnchantMinDamageFactor, DischargeEnchantMaxDamageFactor, interpolant);
        }

        // 07MAY2024: Ozzatron: adjusted charge formula again to more closely match previous behavior
        // old formula: 1.087 - 0.08 / (x + 0.07)
        // new formula: 1.08 - 0.04 / (x + 0.06)
        //
        // Intended behavior: Any charge above 50% guarantees 100% damage, so charge weapons are easy to use
        // Actual behavior:
        // 44%+ charge = 100% damage
        // 20%  charge = 92.6% damage
        // 10%  charge = 83% damage
        // 0%   charge = 41.33% damage
        //
        internal float ChargeDamageFormula()
        {
            float x = MathHelper.Clamp(ChargeRatio, 0f, 1f);
            float y = 1.08f - 0.04f / (x + 0.06f);
            return MathHelper.Clamp(y, 0f, 1f);
        }
        #endregion

        #region Hit NPC
        public override void ModifyHitNPC(Item item, Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            // This assume all items with a damage hit is a weapon. There appears to be no edge cases for this thus far
            if (player.Calamity().oldFashioned)
                modifiers.SourceDamage *= OldFashioned.DamageReductionMultiplier;

            var dripPlayer = player.GetModPlayer<IVDripPlayer>();
            if (dripPlayer.HasAlcohol(AlcoholType.OldFashioned))
                modifiers.SourceDamage *= OldFashioned.DamageReductionMultiplier;
        }

        public override void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Hyperius Overflow
            if (target.Calamity().hyperiusMarked)
            {
                int damageDealt = (int)(damageDone * HyperiusBullet.overflowAppliedMult);
                int damage = (int)Math.Min(target.Calamity().hyperiusDamage / HyperiusBullet.overflowEfficency, damageDealt);

                target.Calamity().hyperiusDamage -= (int)(damage * HyperiusBullet.overflowEfficency);

                // Spawn overflow hit
                Projectile overflow = Projectile.NewProjectileDirect(target.GetSource_FromThis(), target.Center, Vector2.Zero, ProjectileType<HyperiusDamage>(), damage, 0, player.whoAmI, target.whoAmI);
                overflow.DamageType = item.DamageType;
                overflow.ArmorPenetration = item.ArmorPenetration; // Takes the armor pen from what did the hit

                if (target.Calamity().hyperiusDamage <= 0)
                {
                    target.Calamity().hyperiusDamage = 0;
                    target.Calamity().hyperiusMarked = false;
                }
            }
        }
        #endregion

        #region Armor Set Changes
        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            string managedArmorSetName = VanillaArmorChangeManager.GetSetBonusName(Main.player[head.playerIndexTheItemIsReservedFor]);
            if (!string.IsNullOrEmpty(managedArmorSetName))
                return managedArmorSetName;

            if (head.type == ItemID.WizardHat && (body.type == ItemID.AmethystRobe || body.type == ItemID.TopazRobe || body.type == ItemID.SapphireRobe || body.type == ItemID.EmeraldRobe || body.type == ItemID.RubyRobe || body.type == ItemID.DiamondRobe || body.type == ItemID.AmberRobe))
                return "WizardHat";
            if (head.type == ItemID.MagicHat && (body.type == ItemID.AmethystRobe || body.type == ItemID.TopazRobe || body.type == ItemID.SapphireRobe || body.type == ItemID.EmeraldRobe || body.type == ItemID.RubyRobe || body.type == ItemID.DiamondRobe || body.type == ItemID.AmberRobe))
                return "MagicHat";
            if (head.type == ItemID.CrystalNinjaHelmet && body.type == ItemID.CrystalNinjaChestplate && legs.type == ItemID.CrystalNinjaLeggings)
                return "CrystalAssassin";
            if (head.type == ItemID.SpectreHood && body.type == ItemID.SpectreRobe && legs.type == ItemID.SpectrePants)
                return "SpectreHealing";
            if (head.type == ItemID.SolarFlareHelmet && body.type == ItemID.SolarFlareBreastplate && legs.type == ItemID.SolarFlareLeggings)
                return "SolarFlare";
            return "";
        }

        public override void ModifyItemScale(Item item, Player player, ref float scale)
        {
            // Xyk 3MARCH2026: Doesn't work on any non use style 1 items currently, Doze will fix it
            if (item.CountsAsClass<MeleeDamageClass>() && player.HasBuff(BuffID.Tipsy))
                scale += 0.25f;
            if (item.CountsAsClass<MeleeDamageClass>() && (player.GetModPlayer<IVDripPlayer>().HasAlcohol(AlcoholType.Ale) || player.GetModPlayer<IVDripPlayer>().HasAlcohol(AlcoholType.Sake)))
                scale += 0.25f;
        }
        public override void UpdateArmorSet(Player player, string set)
        {
            CalamityPlayer modPlayer = player.Calamity();
            VanillaArmorChangeManager.CreateTooltipManuallyAsNecessary(player);

            if (set == "WizardHat")
            {
                player.GetCritChance<MagicDamageClass>() -= 6;
                player.setBonus = CalamityUtils.GetTextValue("Vanilla.Armor.SetBonus.Wizard");
            }
            if (set == "MagicHat")
            {
                player.statManaMax2 -= 20;
                player.setBonus = CalamityUtils.GetTextValue("Vanilla.Armor.SetBonus.MagicHat");
            }
            if (set == "CrystalAssassin")
            {
                player.setBonus = CalamityUtils.GetTextValue("Vanilla.Armor.SetBonus.CrystalAssassin");
                modPlayer.DashID = string.Empty;
                modPlayer.rogueStealthMax += 0.9f;
                modPlayer.wearingRogueArmor = true;
            }
            else if (set == "SpectreHealing")
            {
                player.GetDamage<MagicDamageClass>() += 0.2f;
                player.setBonus = CalamityUtils.GetTextValue("Vanilla.Armor.SetBonus.SpectreHealing");
            }
            else if (set == "SolarFlare")
            {
                // Cancel out the base 12% DR
                player.endurance -= 0.12f;

                // Solar Flare armor dash overrides modded dashes by default
                if (player.solarShields > 0)
                    modPlayer.DashID = string.Empty;
            }
        }
        #endregion

        #region Equip Changes
        public override void UpdateEquip(Item item, Player player)
        {
            switch (item.type)
            {
                case ItemID.MagicHat:
                    player.GetDamage<MagicDamageClass>() -= 0.06f;
                    break;

                case ItemID.AmethystRobe:
                    player.manaCost += 0.01f; // 5% to 4%
                    break;

                case ItemID.TopazRobe:
                    player.statManaMax2 -= 20;
                    player.manaCost += 0.02f; // 7% to 5%
                    break;

                case ItemID.SapphireRobe:
                    player.manaCost += 0.03f; // 9% to 6%
                    break;

                case ItemID.EmeraldRobe:
                    player.statManaMax2 -= 20;
                    player.manaCost += 0.04f; // 11% to 7%
                    break;

                case ItemID.RubyRobe:
                case ItemID.AmberRobe:
                    player.manaCost += 0.05f; // 13% to 8%
                    break;

                case ItemID.DiamondRobe:
                    player.statManaMax2 -= 20;
                    player.manaCost += 0.06f; // 15% to 9%
                    break;

                case ItemID.Gi:
                    player.GetAttackSpeed<MeleeDamageClass>() -= 0.1f;
                    player.jumpSpeedBoost += 0.5f;
                    break;

                case ItemID.TitaniumMask:
                    player.GetAttackSpeed<MeleeDamageClass>() += 0.05f;
                    break;





                case ItemID.ShroomiteBreastplate:
                    player.GetDamage<RangedDamageClass>() -= 0.05f;
                    player.GetCritChance<RangedDamageClass>() -= 5;
                    break;

                case ItemID.SquireAltHead:
                    player.GetDamage<MeleeDamageClass>() += 0.05f;
                    player.GetDamage<SummonDamageClass>() += 0.05f;
                    break;

                case ItemID.SquireAltShirt:
                    player.GetDamage<SummonDamageClass>() -= 0.1f;
                    break;

                case ItemID.SquireAltPants:
                    player.GetCritChance<MeleeDamageClass>() -= 5;
                    player.GetDamage<SummonDamageClass>() -= 0.05f;
                    break;

                case ItemID.SolarFlareHelmet:
                    player.GetCritChance<MeleeDamageClass>() -= 6;
                    break;

                case ItemID.VortexHelmet:
                    player.GetDamage<RangedDamageClass>() -= 0.06f;
                    player.GetCritChance<RangedDamageClass>() -= 2;
                    break;
            }
        }
        #endregion

        #region Accessory Changes
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();

            // Ankh Shield Mighty Wind immunity.
            if (item.type == ItemID.AnkhShield)
                player.buffImmune[BuffID.WindPushed] = true;

            if (item.type == ItemID.FlameWakerBoots)
            {
                modPlayer.flameWakerBoots = true;
                if (modPlayer.bootLevel < 1)
                    modPlayer.bootLevel = 1;
            }

            if (item.type == ItemID.HellfireTreads)
            {
                modPlayer.hellfireTreads = true;
                if (modPlayer.bootLevel < 2)
                    modPlayer.bootLevel = 2;
                player.buffImmune[BuffID.OnFire] = true;
            }

            if (item.type == ItemID.FairyBoots)
                modPlayer.fairyBoots = true;

            // Mana Flower tinker buffs
            if (item.type == ItemID.ArcaneFlower)
                player.GetDamage<MagicDamageClass>() += 0.05f;

            if (item.type == ItemID.EyeoftheGolem)
            {
                player.Calamity().critDamage += 0.15f;
            }
            if (item.type == ItemID.ReconScope)
            {
                player.GetDamage<RangedDamageClass>() += 0.05f; //Total 15% damage
                player.GetCritChance<RangedDamageClass>() -= 5; //Total 5% crit
            }
            if (item.type == ItemID.SniperScope)
            {
                player.GetDamage<RangedDamageClass>() -= 0.1f; //Total 0% damage
                player.GetCritChance<RangedDamageClass>() += 2; //Total 12% crit
                player.Calamity().critDamage += 0.15f;
            }

            if (item.type == ItemID.FireGauntlet)
            {
                player.GetDamage<MeleeDamageClass>() += 0.02f;
            }
            if (item.type == ItemID.FireGauntlet || item.type == ItemID.MagmaStone)
            {
                modPlayer.magmaStoneVisuals = !hideVisual; // hides the fire dust when hiding the accessory
            }

            // The Frog Leg line is prevented from stacking.
            // Additionally, Amphibian boots are directly nerfed so they aren't the best in slot boots at all times.
            //
            // 21MAY2024: Ozzatron: Disabled this code. Frog Leg is allowed to stack. Amphibian Boots specific nerf is applied below.
            /*
            switch (item.type)
            {
                default:
                    break;
                case ItemID.AmphibianBoots:
                    if (modPlayer.alreadyHasFrogLeg)
                        player.jumpSpeedBoost -= BalancingConstants.VanillaFrogLegJumpSpeedBoost;
                    else
                        player.jumpSpeedBoost += BalancingConstants.AmphibianBootsJumpSpeedBoost - BalancingConstants.VanillaFrogLegJumpSpeedBoost;
                    modPlayer.alreadyHasFrogLeg = true;
                    break;
                case ItemID.FrogLeg:
                case ItemID.FrogFlipper:
                case ItemID.FrogGear:
                case ItemID.FrogWebbing:
                    if (modPlayer.alreadyHasFrogLeg)
                        player.jumpSpeedBoost -= BalancingConstants.VanillaFrogLegJumpSpeedBoost;
                    modPlayer.alreadyHasFrogLeg = true;
                    break;
            }
            */
            if (item.type == ItemID.AmphibianBoots)
                player.jumpSpeedBoost += BalancingConstants.AmphibianBootsJumpSpeedBoost - BalancingConstants.VanillaFrogLegJumpSpeedBoost;

            // Feral Claws line melee speed adjustments and nonstacking
            // First removes all their melee speed so it can be given based on which you wear without stacking
            if (item.type == ItemID.FeralClaws)
            {
                player.GetAttackSpeed<MeleeDamageClass>() -= 0.12f; // Feral Claws 10%
                if (modPlayer.gloveLevel < 1)
                    modPlayer.gloveLevel = 1;
            }
            if (item.type == ItemID.PowerGlove || item.type == ItemID.BerserkerGlove)
            {
                player.GetAttackSpeed<MeleeDamageClass>() -= 0.12f; // Power/Berserker Glove 12%
                if (modPlayer.gloveLevel < 2)
                    modPlayer.gloveLevel = 2;
            }
            if (item.type == ItemID.MechanicalGlove)
            {
                player.GetAttackSpeed<MeleeDamageClass>() -= 0.12f; // Mechanical Glove 12%
                if (modPlayer.gloveLevel < 3)
                    modPlayer.gloveLevel = 3;
            }
            if (item.type == ItemID.FireGauntlet)
            {
                player.GetAttackSpeed<MeleeDamageClass>() -= 0.12f; // Fire Gauntlet 14%
                if (modPlayer.gloveLevel < 4)
                    modPlayer.gloveLevel = 4;
            }
            if (modPlayer.eGauntlet && modPlayer.gloveLevel < 5) // Elemental Gauntlet 15%
                modPlayer.gloveLevel = 5;

            //Celestial Stone line melee speed removal
            if (item.type == ItemID.SunStone)
            {
                if (Main.dayTime)
                    player.GetAttackSpeed<MeleeDamageClass>() -= 0.1f;
            }

            if (item.type == ItemID.MoonStone)
            {
                if (!Main.dayTime || Main.eclipse)
                    player.GetAttackSpeed<MeleeDamageClass>() -= 0.1f;
            }

            if (item.type == ItemID.CelestialStone || item.type == ItemID.CelestialShell)
            {
                player.GetAttackSpeed<MeleeDamageClass>() -= 0.1f;
            }

            if (item.type == ItemID.GravityGlobe)
            {
                player.noFallDmg = true;
                player.GetJumpState<GravityJump>().Enable();
                if (player.Calamity().justChangedGravity)
                {
                    player.GetJumpState<GravityJump>().Available = true;
                }
                if (player.wingsLogic <= 0 && player.velocity.Y != 0 && player.maxRunSpeed < 8)
                {
                    player.maxRunSpeed = 5f;
                }
                player.jumpSpeedBoost += 1.6f;
                if (player.controlDown)
                    player.maxFallSpeed *= 1.5f;
                else
                    player.maxFallSpeed *= 1.2f;
            }

            if (item.type == ItemID.DemonWings && !player.mount.Active)
                player.maxFallSpeed *= 1.2f;

            if (item.type == ItemID.BeeWings && !player.mount.Active && !player.controlDown)
            {
                player.gravity *= 0.75f;
                player.maxFallSpeed *= 0.75f;
            }

            if (item.type == ItemID.FinWings)
                player.ignoreWater = true;

            // Spawns ornaments which refreshes flight time upon pickup
            else if (item.type == ItemID.FestiveWings)
            {
                if (modPlayer.wingProjectileCooldown <= 0)
                {
                    var source = player.GetSource_Accessory(item);
                    if (player.controlJump && player.jump == 0 && player.velocity.Y != 0f && player.wingTime > 0f && !player.mount.Active && !player.mount.Cart)
                    {
                        Vector2 ornamentPos = player.Center + Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(105f)) * Main.rand.NextFloat(-512f, -320f);

                        int p = Projectile.NewProjectile(source, ornamentPos, Vector2.Zero, ProjectileType<FestiveWingsOrnament>(), 0, 0f, player.whoAmI);
                        if (p.WithinBounds(Main.maxProjectiles))
                            modPlayer.wingProjectileCooldown = 90;
                    }
                }
            }
            // Leaves a trail of black fairy dust which reduces flight time to any player that touches it
            else if (item.type == ItemID.TatteredFairyWings)
            {
                if (modPlayer.wingProjectileCooldown <= 0)
                {
                    var source = player.GetSource_Accessory(item);
                    if (player.controlJump && player.jump == 0 && player.velocity.Y != 0f && player.wingTime > 0f && !player.mount.Active && !player.mount.Cart)
                    {
                        Vector2 fairyDustVel = Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * Main.rand.NextFloat(0.08f, 0.2f);

                        int p = Projectile.NewProjectile(source, player.Center, fairyDustVel, ProjectileType<TatteredFairyDust>(), 0, 0f, player.whoAmI);
                        if (p.WithinBounds(Main.maxProjectiles))
                            modPlayer.wingProjectileCooldown = 8;
                    }
                }
            }

            if (item.type == ItemID.JellyfishNecklace || item.type == ItemID.JellyfishDivingGear || item.type == ItemID.ArcticDivingGear)
                modPlayer.jellyfishNecklace = true;

            if (item.type == ItemID.FleshKnuckles || item.type == ItemID.BerserkerGlove || item.type == ItemID.HeroShield)
                modPlayer.fleshKnuckles = true;

            if (item.type == ItemID.RoyalGel)
                modPlayer.royalGel = true;

            if (item.type == ItemID.HandWarmer)
                modPlayer.handWarmer = true;

            if (item.type == ItemID.EoCShield || item.type == ItemID.Tabi || item.type == ItemID.MasterNinjaGear)
                modPlayer.DashID = string.Empty;
        }
        #endregion

        #region WingChanges
        public override void HorizontalWingSpeeds(Item item, Player player, ref float speed, ref float acceleration)
        {
            CalamityPlayer modPlayer = player.Calamity();
            float moveSpeedBoost = modPlayer.moveSpeedBonus * 0.06f;

            float flightSpeedMult = 1f +
                (modPlayer.soaring ? SoaringPotion.FlightBoost : 0f) +
                (modPlayer.reaverSpeed ? ReaverHeadMobility.SetBonusFlightBoost : 0f) +
                moveSpeedBoost;

            float flightAccMult = 1f + moveSpeedBoost;

            flightSpeedMult = MathHelper.Clamp(flightSpeedMult, 0.5f, 1.5f);
            speed *= flightSpeedMult;

            flightAccMult = MathHelper.Clamp(flightAccMult, 0.5f, 1.5f);
            acceleration *= flightAccMult;
        }

        public override void VerticalWingSpeeds(Item item, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            switch (item.type)
            {
                case ItemID.AngelWings:
                    maxAscentMultiplier *= 1.2f;
                    constantAscend *= 1.35f;
                    break;
                case ItemID.DemonWings:
                    ascentWhenFalling *= 2f;
                    break;
                case ItemID.FlameWings:
                    maxAscentMultiplier *= 1.1067f;
                    constantAscend *= 1.25f;
                    break;
                case ItemID.ButterflyWings:
                    maxAscentMultiplier *= 0.6667f;
                    constantAscend *= 5f;
                    break;
                case ItemID.GhostWings:
                    maxAscentMultiplier *= 0.6025f;
                    constantAscend *= 5f;
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region GrabChanges
        public override void GrabRange(Item item, Player player, ref int grabRange)
        {
            // First, apply the grab range multiplier.
            if (item.TryGetGlobalItem<GrabRangeGlobalItem>(out var grabRangeItem) && grabRangeItem.grabRangeMultiplier > 1f)
                grabRange = (int)(grabRangeItem.grabRangeMultiplier * grabRange);

            // Then, apply flat grab range boosts.
            if (player.Calamity().reaverExplore)
                grabRange += ReaverHeadExplore.SetBonusGrabRangeBoost;

            // Nebula boosters have greater pickup range while hovering with Nebula Mantle.
            if (player.wingsLogic == (int)VanillaWingID.WingsNebula && player.wingTime > 0f && player.controlJump && player.TryingToHoverDown && ItemID.Sets.NebulaPickup[item.type])
                grabRange *= 3;
        }
        #endregion

        #region Ammo
        public override bool CanConsumeAmmo(Item weapon, Item ammo, Player player) => Main.rand.NextFloat() <= player.Calamity().ammoCost;

        public static bool HasEnoughAmmo(Player player, Item item, int ammoConsumed)
        {
            bool hasEnoughAmmo = false;
            bool canShoot = false;

            for (int i = 54; i < Main.InventorySlotsTotal; i++)
            {
                if (player.inventory[i].ammo == item.useAmmo && (player.inventory[i].stack >= ammoConsumed || !player.inventory[i].consumable))
                {
                    canShoot = true;
                    hasEnoughAmmo = true;
                    break;
                }
            }

            if (!hasEnoughAmmo)
            {
                for (int j = 0; j < 54; j++)
                {
                    if (player.inventory[j].ammo == item.useAmmo && (player.inventory[j].stack >= ammoConsumed || !player.inventory[j].consumable))
                    {
                        canShoot = true;
                        break;
                    }
                }
            }
            return canShoot;
        }

        public static void ConsumeAdditionalAmmo(Player player, Item item, int ammoConsumed)
        {
            Item itemAmmo = new Item();
            bool hasEnoughAmmo = false;
            bool dontConsumeAmmo = false;

            for (int i = 54; i < Main.InventorySlotsTotal; i++)
            {
                if (player.inventory[i].ammo == item.useAmmo && (player.inventory[i].stack >= ammoConsumed || !player.inventory[i].consumable))
                {
                    itemAmmo = player.inventory[i];
                    hasEnoughAmmo = true;
                    break;
                }
            }

            if (!hasEnoughAmmo)
            {
                for (int j = 0; j < 54; j++)
                {
                    if (player.inventory[j].ammo == item.useAmmo && (player.inventory[j].stack >= ammoConsumed || !player.inventory[j].consumable))
                    {
                        itemAmmo = player.inventory[j];
                        break;
                    }
                }
            }

            if (player.magicQuiver && (item.useAmmo == AmmoID.Arrow || item.useAmmo == AmmoID.Stake) && Main.rand.NextBool(5))
                dontConsumeAmmo = true;
            if (player.huntressAmmoCost90 && Main.rand.NextBool(10))
                dontConsumeAmmo = true;
            if (player.ammoBox && Main.rand.NextBool(5))
                dontConsumeAmmo = true;
            if (player.ammoPotion && Main.rand.NextBool(5))
                dontConsumeAmmo = true;
            if (player.ammoCost80 && Main.rand.NextBool(5))
                dontConsumeAmmo = true;
            if (player.chloroAmmoCost80 && Main.rand.NextBool(5))
                dontConsumeAmmo = true;
            if (player.ammoCost75 && Main.rand.NextBool(4))
                dontConsumeAmmo = true;
            if (Main.rand.NextFloat() > player.Calamity().ammoCost)
                dontConsumeAmmo = true;

            if (!dontConsumeAmmo && itemAmmo.consumable)
            {
                itemAmmo.stack -= ammoConsumed;
                if (itemAmmo.stack <= 0)
                {
                    itemAmmo.active = false;
                    itemAmmo.TurnToAir();
                }
            }
        }
        #endregion

        #region PostUpdate
        public override void PostUpdate(Item item)
        {
            CalamityUtils.ForceItemIntoWorld(item);
        }
        #endregion

        #region Inventory Drawing
        internal static ChargingEnergyParticleSet EnchantmentEnergyParticles = new ChargingEnergyParticleSet(-1, 2, Color.DarkViolet, Color.White, 0.04f, 24f);

        internal static void UpdateAllParticleSets()
        {
            EnchantmentEnergyParticles.Update();
        }

        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            // I want to strangle somebody.
            Texture2D itemTexture = TextureAssets.Item[item.type].Value;
            Rectangle itemFrame = (Main.itemAnimations[item.type] == null) ? itemTexture.Frame() : Main.itemAnimations[item.type].GetFrame(itemTexture);

            if (!EnchantmentManager.ItemUpgradeRelationship.ContainsKey(item.type) || !Main.LocalPlayer.InventoryHas(ItemType<BrimstoneLocus>()))
                return true;

            // Draw all particles.
            float currentPower = 0f;
            int calamitasNPCIndex = NPC.FindFirstNPC(NPCType<BrimstoneWitch>());
            if (calamitasNPCIndex != -1)
                currentPower = Utils.GetLerpValue(11750f, 1000f, Main.LocalPlayer.Distance(Main.npc[calamitasNPCIndex].Center), true);

            Vector2 particleDrawCenter = position + new Vector2(12f, 16f) * Main.inventoryScale - itemFrame.Size() * 0.25f;

            EnchantmentEnergyParticles.InterpolationSpeed = MathHelper.Lerp(0.035f, 0.1f, currentPower);
            EnchantmentEnergyParticles.DrawSet(particleDrawCenter + Main.screenPosition);
            spriteBatch.Draw(itemTexture, position, itemFrame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);

            return false;
        }
        #endregion     

        #region On Create
        private static int cachedForgeID = -1;
        public override void OnCreated(Item item, ItemCreationContext context)
        {
            // 05JUL2024: Ozzatron: Register the usage of Draedon's Forge for the purposes of his dialogue.
            // This was moved out of an On edit in the DraedonsForge item for Magic Storage compatibility.
            Player p = Main.LocalPlayer;
            if (cachedForgeID < 0)
                cachedForgeID = TileType<DraedonsForge>();
            if (context is RecipeItemCreationContext && p.adjTile[cachedForgeID])
                p.Calamity().HasCraftedDraedonsForge = true;
        }
        #endregion

        #region Rarity Price Table
        // Base numeric rarity pricing guide.
        private static readonly int Rarity0BuyPrice = Item.buyPrice(0, 0, 50, 0);
        private static readonly int Rarity1BuyPrice = Item.buyPrice(0, 1, 0, 0);
        private static readonly int Rarity2BuyPrice = Item.buyPrice(0, 2, 0, 0);
        private static readonly int Rarity3BuyPrice = Item.buyPrice(0, 5, 0, 0);
        private static readonly int Rarity4BuyPrice = Item.buyPrice(0, 10, 0, 0);
        private static readonly int Rarity5BuyPrice = Item.buyPrice(0, 20, 0, 0);
        private static readonly int Rarity6BuyPrice = Item.buyPrice(0, 35, 0, 0);
        private static readonly int Rarity7BuyPrice = Item.buyPrice(0, 45, 0, 0);
        private static readonly int Rarity8BuyPrice = Item.buyPrice(0, 60, 0, 0);
        private static readonly int Rarity9BuyPrice = Item.buyPrice(0, 80, 0, 0);
        private static readonly int Rarity10BuyPrice = Item.buyPrice(1, 0, 0, 0); // Highest raw rarity used by vanilla items (ML drops)
        private static readonly int Rarity11BuyPrice = Item.buyPrice(1, 20, 0, 0); // End of vanilla rarities
        private static readonly int Rarity12BuyPrice = Item.buyPrice(1, 50, 0, 0);
        private static readonly int Rarity13BuyPrice = Item.buyPrice(1, 75, 0, 0);
        private static readonly int Rarity14BuyPrice = Item.buyPrice(2, 0, 0, 0);
        private static readonly int Rarity15BuyPrice = Item.buyPrice(2, 40, 0, 0);
        private static readonly int Rarity16BuyPrice = Item.buyPrice(2, 80, 0, 0);
        private static readonly int Rarity17BuyPrice = Item.buyPrice(3, 20, 0, 0); // This is Calamity's "plus" rarity (similar to vanilla 11 / Purple). Nothing uses it.

        private static readonly int[] RarityBuyPriceArray = new int[] {
            Rarity0BuyPrice,
            Rarity1BuyPrice,
            Rarity2BuyPrice,
            Rarity3BuyPrice,
            Rarity4BuyPrice,
            Rarity5BuyPrice,
            Rarity6BuyPrice,
            Rarity7BuyPrice,
            Rarity8BuyPrice,
            Rarity9BuyPrice,
            Rarity10BuyPrice,
            Rarity11BuyPrice,
            Rarity12BuyPrice,
            Rarity13BuyPrice,
            Rarity14BuyPrice,
            Rarity15BuyPrice,
            Rarity16BuyPrice,
            Rarity17BuyPrice,
        };

        // Canonical names which are implemented as properties that reference the base numeric rarity prices.
        // Also serves as a convenient counter for the number of items Calamity adds of each rarity.
        public static int RarityWhiteBuyPrice => Rarity0BuyPrice;
        public static int RarityBlueBuyPrice => Rarity1BuyPrice;
        public static int RarityGreenBuyPrice => Rarity2BuyPrice;
        public static int RarityOrangeBuyPrice => Rarity3BuyPrice;
        public static int RarityLightRedBuyPrice => Rarity4BuyPrice;
        public static int RarityPinkBuyPrice => Rarity5BuyPrice;
        public static int RarityLightPurpleBuyPrice => Rarity6BuyPrice;
        public static int RarityLimeBuyPrice => Rarity7BuyPrice;
        public static int RarityYellowBuyPrice => Rarity8BuyPrice;
        public static int RarityCyanBuyPrice => Rarity9BuyPrice;
        public static int RarityRedBuyPrice => Rarity10BuyPrice;
        public static int RarityPurpleBuyPrice => Rarity11BuyPrice;
        public static int RarityTurquoiseBuyPrice => Rarity12BuyPrice;
        public static int RarityPureGreenBuyPrice => Rarity13BuyPrice;
        public static int RarityDarkBlueBuyPrice => Rarity14BuyPrice;
        public static int RarityVioletBuyPrice => Rarity15BuyPrice;
        public static int RarityHotPinkBuyPrice => Rarity16BuyPrice;
        public static int RarityCalamityRedBuyPrice => Rarity17BuyPrice;
        #endregion

        //
        // !! WARNING !!
        //
        // 17APR2024: Ozzatron:
        // THESE FUNCTIONS MAY SHOW ZERO REFERENCES BUT ARE ACTIVELY USED BY MULTIPLE CALAMITY ADDONS, INCLUDING CATALYST.
        // DO NOT TOUCH. IF YOU DO, THESE ADDONS WILL BREAK!
        //
        #region Rarity / Price Helper Functions
        public static int GetBuyPrice(int rarity)
        {
            // Vanilla rarities go directly to the array.
            if (rarity >= ItemRarityID.White && rarity <= ItemRarityID.Purple)
                return RarityBuyPriceArray[rarity];

            // Calamity rarities aren't guaranteed to have the monotonic IDs, so they're handled directly.
            if (rarity == RarityType<Turquoise>())
                return RarityTurquoiseBuyPrice;
            if (rarity == RarityType<PureGreen>())
                return RarityPureGreenBuyPrice;
            if (rarity == RarityType<CosmicPurple>())
                return RarityDarkBlueBuyPrice;
            if (rarity == RarityType<BurnishedAuric>())
                return RarityVioletBuyPrice;
            if (rarity == RarityType<HotPink>())
                return RarityHotPinkBuyPrice;
            if (rarity == RarityType<CalamityRed>())
                return RarityCalamityRedBuyPrice;

            // Return 0 if it's not a progression based or other mod's rarity
            return 0;
        }

        public static int GetBuyPrice(Item item) => GetBuyPrice(item.rare);
        #endregion
    }
}
