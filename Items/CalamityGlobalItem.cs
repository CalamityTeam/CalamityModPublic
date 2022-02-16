using CalamityMod.Buffs.Potions;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Potions;
using CalamityMod.Items.VanillaArmorChanges;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.Other;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.UI;
using CalamityMod.UI.CalamitasEnchants;
using CalamityMod.UI.CooldownIndicators;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityMod.Items
{
    public partial class CalamityGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;

        public bool rogue = false;
        public float StealthGenBonus;

        #region Chargeable Item Variables
        public bool UsesCharge = false;
        public float Charge = 0f;
        public float MaxCharge = 1f;
        public float ChargePerUse = 0f;
        // If left at the default value of -1, ChargePerUse is automatically used for alt fire.
        // If you want a different amount of charge used for alt fire, then set a different value here.
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
        public bool CannotBeEnchanted = false;
        public Enchantment? AppliedEnchantment = null;
        public float DischargeEnchantExhaustion = 0;
        public float DischargeExhaustionRatio
        {
            get
            {
                float ratio = DischargeEnchantExhaustion / DischargeEnchantExhaustionCap;
                return float.IsNaN(ratio) || float.IsInfinity(ratio) ? 0f : MathHelper.Clamp(ratio, 0f, 1f);
            }
        }
        public const float DischargeEnchantExhaustionCap = 1600f;
        public const float DischargeEnchantMinDamageFactor = 0.77f;
        public const float DischargeEnchantMaxDamageFactor = 1.26f;
        #endregion

        // Miscellaneous stuff
        public CalamityRarity customRarity = CalamityRarity.NoEffect;
        public int timesUsed = 0;
        public int reforgeTier = 0;
        public bool donorItem = false;
        public bool devItem = false;
        public bool canFirePointBlankShots = false;
        public bool trueMelee = false;

        public static readonly Color ExhumedTooltipColor = new Color(198, 27, 64);

        // See RogueWeapon.cs for rogue modifier shit
        #region Modifiers
        public CalamityGlobalItem()
        {
            StealthGenBonus = 1f;
        }

        public override GlobalItem Clone(Item item, Item itemClone)
        {
            CalamityGlobalItem myClone = (CalamityGlobalItem)base.Clone(item, itemClone);
            myClone.StealthGenBonus = StealthGenBonus;
            myClone.DischargeEnchantExhaustion = DischargeEnchantExhaustion;
            myClone.Charge = Charge;
            return myClone;
        }

        public override bool NewPreReforge(Item item)
        {
            StealthGenBonus = 1f;
            return true;
        }
        #endregion

        #region SetDefaults
        public override void SetDefaults(Item item)
        {
            // TODO -- Remove this in 1.4 with ModRarity.
            // TODO -- Remove all instances of manually setting the rarity of postML items to Red.
            if (customRarity.IsPostML() && item.rare != ItemRarityID.Purple)
                item.rare = ItemRarityID.Purple;

            // All items that stack to 30, 50, 75, 99 , or 999 now stack to 9999 instead.
            if (item.maxStack == 30 || item.maxStack == 50 || item.maxStack == 75 || item.maxStack == 99 || item.maxStack == 999)
                item.maxStack = 9999;

            // Shield of Cthulhu cannot be enchanted (it is an accessory with a damage value).
            // TODO -- there are better ways to do this. Just stop letting accessories be enchanted, even if they do have a damage value.
            if (item.type == ItemID.EoCShield)
                CannotBeEnchanted = true;

            // Star Cannon no longer makes noise.
            if (item.type == ItemID.StarCannon)
                item.UseSound = null;

            // Fix Bones being attracted to the player when you have open ammo slots
            if (item.type == ItemID.Bone)
                item.notAmmo = true;

            // Modified Pearlwood items are now Light Red
            if (item.type == ItemID.PearlwoodBow || item.type == ItemID.PearlwoodHammer || item.type == ItemID.PearlwoodSword)
                item.rare = ItemRarityID.LightRed;

            // Make most expert items no longer expert because they drop in all modes now.
            switch (item.type)
            {
                case ItemID.RoyalGel:
                case ItemID.EoCShield:
                case ItemID.WormScarf:
                case ItemID.BrainOfConfusion:
                case ItemID.HiveBackpack:
                case ItemID.BoneGlove:
                case ItemID.MechanicalBatteryPiece:
                case ItemID.MechanicalWagonPiece:
                case ItemID.MechanicalWheelPiece:
                case ItemID.MinecartMech:
                case ItemID.SporeSac:
                case ItemID.ShinyStone:
                case ItemID.ShrimpyTruffle:
                case ItemID.GravityGlobe:
                case ItemID.SuspiciousLookingTentacle:
                    item.expert = false;
                    break;
            }

            SetDefaults_ApplyTweaks(item);

            // TODO -- these properties should be some sort of dictionary.
            // Perhaps the solution here is to just apply all changes of all kinds using the "Balance" system
            if (CalamityLists.noGravityList.Contains(item.type))
                ItemID.Sets.ItemNoGravity[item.type] = true;

            if (CalamityLists.lavaFishList.Contains(item.type))
                ItemID.Sets.CanFishInLava[item.type] = true;
        }
        #endregion

        #region Shoot
        public override bool Shoot(Item item, Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (Main.myPlayer == player.whoAmI && player.Calamity().cursedSummonsEnchant && NPC.CountNPCS(ModContent.NPCType<CalamitasEnchantDemon>()) < 2)
            {
                CalamityNetcode.NewNPC_ClientSide(Main.MouseWorld, ModContent.NPCType<CalamitasEnchantDemon>(), player);
                Main.PlaySound(SoundID.DD2_DarkMageSummonSkeleton, Main.MouseWorld);
            }

            bool belowHalfMana = player.statMana < player.statManaMax2 * 0.5f;
            if (Main.myPlayer == player.whoAmI && player.Calamity().manaMonsterEnchant && Main.rand.NextBool(12) && player.ownedProjectileCounts[ModContent.ProjectileType<ManaMonster>()] <= 0 && belowHalfMana)
            {
                int monsterDamage = (int)(165000 * player.MagicDamage());
                Vector2 shootVelocity = player.SafeDirectionTo(Main.MouseWorld, -Vector2.UnitY).RotatedByRandom(0.07f) * Main.rand.NextFloat(4f, 5f);
                Projectile.NewProjectile(player.Center + shootVelocity, shootVelocity, ModContent.ProjectileType<ManaMonster>(), monsterDamage, 0f, player.whoAmI);
            }

            if (rogue)
            {
                speedX *= modPlayer.throwingVelocity;
                speedY *= modPlayer.throwingVelocity;
                if (modPlayer.gloveOfRecklessness && item.useTime == item.useAnimation)
                {
                    Vector2 rotated = new Vector2(speedX, speedY);
                    rotated = rotated.RotatedByRandom(MathHelper.ToRadians(6f));
                    speedX = rotated.X;
                    speedY = rotated.Y;
                }
            }
            if (modPlayer.eArtifact && item.ranged)
            {
                speedX *= 1.25f;
                speedY *= 1.25f;
            }
            Vector2 velocity = new Vector2(speedX, speedY);
            if (modPlayer.luxorsGift)
            {
                // useTime 9 = 0.9 useTime 2 = 0.2
                double damageMult = 1.0;
                if (item.useTime < 10)
                    damageMult -= (10 - item.useTime) / 10.0;

                double newDamage = damage * damageMult;

                if (player.whoAmI == Main.myPlayer)
                {
                    if (item.melee)
                    {
                        int projectile = Projectile.NewProjectile(position, velocity * 0.5f, ModContent.ProjectileType<LuxorsGiftMelee>(), (int)(newDamage * 0.25), 0f, player.whoAmI);
                        if (projectile.WithinBounds(Main.maxProjectiles))
                            Main.projectile[projectile].Calamity().forceTypeless = true;
                    }
                    else if (rogue)
                    {
                        int projectile = Projectile.NewProjectile(position, velocity, ModContent.ProjectileType<LuxorsGiftRogue>(), (int)(newDamage * 0.2), 0f, player.whoAmI);
                        if (projectile.WithinBounds(Main.maxProjectiles))
                            Main.projectile[projectile].Calamity().forceTypeless = true;
                    }
                    else if (item.ranged)
                    {
                        int projectile = Projectile.NewProjectile(position, velocity * 1.5f, ModContent.ProjectileType<LuxorsGiftRanged>(), (int)(newDamage * 0.15), 0f, player.whoAmI);
                        if (projectile.WithinBounds(Main.maxProjectiles))
                            Main.projectile[projectile].Calamity().forceTypeless = true;
                    }
                    else if (item.magic)
                    {
                        int projectile = Projectile.NewProjectile(position, velocity, ModContent.ProjectileType<LuxorsGiftMagic>(), (int)(newDamage * 0.3), 0f, player.whoAmI);
                        if (projectile.WithinBounds(Main.maxProjectiles))
                            Main.projectile[projectile].Calamity().forceTypeless = true;
                    }
                    else if (item.summon && player.ownedProjectileCounts[ModContent.ProjectileType<LuxorsGiftSummon>()] < 1)
                    {
                        int projectile = Projectile.NewProjectile(position, Vector2.Zero, ModContent.ProjectileType<LuxorsGiftSummon>(), damage, 0f, player.whoAmI);
                        if (projectile.WithinBounds(Main.maxProjectiles))
                            Main.projectile[projectile].Calamity().forceTypeless = true;
                    }
                }
            }
            if (modPlayer.bloodflareMage && modPlayer.canFireBloodflareMageProjectile)
            {
                if (item.magic)
                {
                    modPlayer.canFireBloodflareMageProjectile = false;
                    if (player.whoAmI == Main.myPlayer)
                    {
                        // Bloodflare Mage Bolt: 130%, soft cap starts at 2000 base damage
                        int bloodflareBoltDamage = CalamityUtils.DamageSoftCap(damage * 1.3, 2600);
                        Projectile.NewProjectile(position, velocity, ModContent.ProjectileType<GhostlyBolt>(), bloodflareBoltDamage, 1f, player.whoAmI);
                    }
                }
            }
            if (modPlayer.bloodflareRanged && modPlayer.canFireBloodflareRangedProjectile)
            {
                if (item.ranged)
                {
                    modPlayer.canFireBloodflareRangedProjectile = false;
                    if (player.whoAmI == Main.myPlayer)
                    {
                        // Bloodflare Ranged Bloodsplosion: 80%, soft cap starts at 150 base damage
                        // This is intentionally extremely low because this effect can be grossly overpowered with sniper rifles and the like.
                        int bloodsplosionDamage = CalamityUtils.DamageSoftCap(damage * 0.8, 120);
                        Projectile.NewProjectile(position, velocity, ModContent.ProjectileType<BloodBomb>(), bloodsplosionDamage, 2f, player.whoAmI);
                    }
                }
            }
            if (modPlayer.tarraMage)
            {
                if (modPlayer.tarraCrits >= 5 && player.whoAmI == Main.myPlayer)
                {
                    modPlayer.tarraCrits = 0;
                    // Tarragon Mage Leaves: (8-10) x 20%, soft cap starts at 200 base damage
                    int leafAmt = 8 + Main.rand.Next(3); // 8, 9, or 10
                    int leafDamage = (int)(damage * 0.2);

                    for (int l = 0; l < leafAmt; l++)
                    {
                        float spreadMult = 0.025f * l;
                        float xDiff = speedX + Main.rand.Next(-25, 26) * spreadMult;
                        float yDiff = speedY + Main.rand.Next(-25, 26) * spreadMult;
                        float speed = (float)Math.Sqrt(speedX * speedX + speedY * speedY);
                        speed = item.shootSpeed / speed;
                        xDiff *= speed;
                        yDiff *= speed;
                        int projectile = Projectile.NewProjectile(position, new Vector2(xDiff, yDiff), ProjectileID.Leaf, leafDamage, knockBack, player.whoAmI);
                        if (projectile.WithinBounds(Main.maxProjectiles))
                            Main.projectile[projectile].Calamity().forceTypeless = true;
                    }
                }
            }
            if (modPlayer.ataxiaBolt && modPlayer.canFireAtaxiaRangedProjectile)
            {
                if (item.ranged)
                {
                    modPlayer.canFireAtaxiaRangedProjectile = false;
                    if (player.whoAmI == Main.myPlayer)
                    {
                        int ataxiaFlareDamage = (int)(damage * 0.25);
                        Projectile.NewProjectile(position, velocity * 1.25f, ModContent.ProjectileType<ChaosFlare>(), ataxiaFlareDamage, 2f, player.whoAmI);
                    }
                }
            }
            if (modPlayer.godSlayerRanged && modPlayer.canFireGodSlayerRangedProjectile)
            {
                if (item.ranged)
                {
                    modPlayer.canFireGodSlayerRangedProjectile = false;
                    if (player.whoAmI == Main.myPlayer)
                    {
                        // God Slayer Ranged Shrapnel: 100%, soft cap starts at 800 base damage
                        int shrapnelRoundDamage = CalamityUtils.DamageSoftCap(damage, 800);
                        Projectile.NewProjectile(position, velocity * 1.25f, ModContent.ProjectileType<GodSlayerShrapnelRound>(), shrapnelRoundDamage, 2f, player.whoAmI);
                    }
                }
            }
            if (modPlayer.ataxiaVolley && modPlayer.canFireAtaxiaRogueProjectile)
            {
                if (rogue)
                {
                    modPlayer.canFireAtaxiaRogueProjectile = false;
                    int flareID = ModContent.ProjectileType<ChaosFlare2>();

                    // Ataxia Rogue Flares: 8 x 50%, soft cap starts at 200 base damage
                    int flareDamage = CalamityUtils.DamageSoftCap(damage * 0.5, 100);
                    if (player.whoAmI == Main.myPlayer)
                    {
                        Main.PlaySound(SoundID.Item20, player.Center);
                        float spread = 45f * 0.0174f;
                        double startAngle = Math.Atan2(player.velocity.X, player.velocity.Y) - spread / 2;
                        double deltaAngle = spread / 8f;
                        double offsetAngle;
                        for (int i = 0; i < 4; i++)
                        {
                            offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                            Projectile.NewProjectile(player.Center.X, player.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), flareID, flareDamage, 1f, player.whoAmI);
                            Projectile.NewProjectile(player.Center.X, player.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), flareID, flareDamage, 1f, player.whoAmI);
                        }
                    }
                }
            }
            if (modPlayer.victideSet)
            {
                if ((item.ranged || item.melee || item.magic || item.thrown || rogue || item.summon) && Main.rand.NextBool(10))
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        // Victide All-class Seashells: 200%, soft cap starts at 23 base damage
                        int seashellDamage = CalamityUtils.DamageSoftCap(damage * 2, 46);
                        Projectile.NewProjectile(position, velocity * 1.25f, ModContent.ProjectileType<Seashell>(), seashellDamage, 1f, player.whoAmI);
                    }
                }
            }
            if (modPlayer.dynamoStemCells)
            {
                if (item.ranged && Main.rand.NextBool(20))
                {
                    double damageMult = item.useTime / 30D;
                    if (damageMult < 0.35)
                        damageMult = 0.35;

                    int newDamage = (int)(damage * 2 * damageMult);

                    if (player.whoAmI == Main.myPlayer)
                    {
                        int projectile = Projectile.NewProjectile(position, velocity * 1.25f, ModContent.ProjectileType<Minibirb>(), newDamage, 2f, player.whoAmI);
                        if (projectile.WithinBounds(Main.maxProjectiles))
                            Main.projectile[projectile].Calamity().forceTypeless = true;
                    }
                }
            }
            if (modPlayer.prismaticRegalia)
            {
                if (item.magic && Main.rand.NextBool(20))
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        for (int i = -5; i <= 5; i += 5)
                        {
                            if (i != 0)
                            {
                                Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.ToRadians(i));
                                int rocket = Projectile.NewProjectile(position, perturbedSpeed, ModContent.ProjectileType<MiniRocket>(), (int)(damage * 0.25), 2f, player.whoAmI);
                                if (rocket.WithinBounds(Main.maxProjectiles))
                                    Main.projectile[rocket].Calamity().forceTypeless = true;
                            }
                        }
                    }
                }
            }
            if (modPlayer.harpyWingBoost && modPlayer.harpyRing)
            {
                if (Main.rand.NextBool(5))
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        float spreadX = speedX + Main.rand.NextFloat(-0.75f, 0.75f);
                        float spreadY = speedY + Main.rand.NextFloat(-0.75f, 0.75f);
                        int feather = Projectile.NewProjectile(position, new Vector2(spreadX, spreadY) * 1.25f, ModContent.ProjectileType<TradewindsProjectile>(), (int)(damage * 0.3), 2f, player.whoAmI);
                        if (feather.WithinBounds(Main.maxProjectiles))
                        {
                            Main.projectile[feather].usesLocalNPCImmunity = true;
                            Main.projectile[feather].localNPCHitCooldown = 10;
                            Main.projectile[feather].Calamity().forceTypeless = true;
                        }
                    }
                }
            }
            if (item.type == ItemID.StarCannon)
            {
                Vector2 muzzleOffset = Vector2.Normalize(new Vector2(speedX, speedY)) * 5f;
                if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                {
                    position += muzzleOffset;
                }
                Projectile.NewProjectile(position, velocity, ModContent.ProjectileType<FallenStarProj>(), damage, knockBack, player.whoAmI);
                Main.PlaySound(SoundID.Item11.WithPitchVariance(0.05f), position);
                return false;
            }
            if (item.type == ItemID.PearlwoodBow)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && (Main.projectile[i].type == ModContent.ProjectileType<RainbowFront>() || Main.projectile[i].type == ModContent.ProjectileType<RainbowTrail>()) && Main.projectile[i].owner == player.whoAmI)
                    {
                        Main.projectile[i].Kill();
                    }
                }
                for (int i = -8; i <= 8; i += 8)
                {
                    Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.ToRadians(i));
                    Projectile.NewProjectile(position, perturbedSpeed, ModContent.ProjectileType<RainbowFront>(), damage, 0f, player.whoAmI);
                }
            }
            return true;
        }
        #endregion

        #region Saving And Loading
        public override bool NeedsSaving(Item item)
        {
            return rogue || canFirePointBlankShots || trueMelee || timesUsed != 0 || customRarity != 0 || Charge != 0 || reforgeTier != 0 || AppliedEnchantment.HasValue || DischargeEnchantExhaustion != 0;
        }

        public override TagCompound Save(Item item)
        {
            return new TagCompound
            {
                ["rogue"] = rogue,
                ["timesUsed"] = timesUsed,
                ["rarity"] = (int)customRarity,
                ["charge"] = Charge,
                ["reforgeTier"] = reforgeTier,
                ["enchantmentID"] = AppliedEnchantment.HasValue ? AppliedEnchantment.Value.ID : 0,
                ["DischargeEnchantExhaustion"] = DischargeEnchantExhaustion,
                ["canFirePointBlankShots"] = canFirePointBlankShots,
                ["trueMelee"] = trueMelee
            };
        }

        public override void Load(Item item, TagCompound tag)
        {
            rogue = tag.GetBool("rogue");
            canFirePointBlankShots = tag.GetBool("canFirePointBlankShots");
            trueMelee = tag.GetBool("trueMelee");
            timesUsed = tag.GetInt("timesUsed");
            customRarity = (CalamityRarity)tag.GetInt("rarity");

            // Changed charge from int to float. If an old charge int is present, load that instead.
            if (tag.ContainsKey("Charge"))
                Charge = tag.GetInt("Charge");
            else
                Charge = tag.GetFloat("charge");

            DischargeEnchantExhaustion = tag.GetFloat("DischargeEnchantExhaustion");
            reforgeTier = tag.GetInt("reforgeTimer");
            Enchantment? savedEnchantment = EnchantmentManager.FindByID(tag.GetInt("enchantmentID"));
            if (savedEnchantment.HasValue)
            {
                AppliedEnchantment = savedEnchantment.Value;
                bool hasCreationEffect = AppliedEnchantment.Value.CreationEffect != null;
                item.Calamity().AppliedEnchantment.Value.CreationEffect?.Invoke(item);
            }
        }

        public override void LoadLegacy(Item item, BinaryReader reader)
        {
            int loadVersion = reader.ReadInt32();
            customRarity = (CalamityRarity)reader.ReadInt32();
            timesUsed = reader.ReadInt32();
            Charge = reader.ReadSingle();
            reforgeTier = reader.ReadInt32();

            if (loadVersion == 0)
            {
                BitsByte flags = reader.ReadByte();
                rogue = flags[0];
                canFirePointBlankShots = flags[1];
                trueMelee = flags[2];
            }
            else
            {
                ModContent.GetInstance<CalamityMod>().Logger.Error("Unknown loadVersion: " + loadVersion);
            }
        }

        public override void NetSend(Item item, BinaryWriter writer)
        {
            BitsByte flags = new BitsByte();
            flags[0] = rogue;
            flags[1] = canFirePointBlankShots;
            flags[2] = trueMelee;

            writer.Write(flags);
            writer.Write((int)customRarity);
            writer.Write(timesUsed);
            writer.Write(Charge);
            writer.Write(reforgeTier);
            writer.Write(AppliedEnchantment.HasValue ? AppliedEnchantment.Value.ID : 0);
            writer.Write(DischargeEnchantExhaustion);
        }

        public override void NetReceive(Item item, BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            rogue = flags[0];
            canFirePointBlankShots = flags[1];
            trueMelee = flags[2];

            customRarity = (CalamityRarity)reader.ReadInt32();
            timesUsed = reader.ReadInt32();
            Charge = reader.ReadSingle();
            reforgeTier = reader.ReadInt32();

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
        public override bool OnPickup(Item item, Player player)
        {
            if (item.type == ItemID.Heart || item.type == ItemID.CandyApple || item.type == ItemID.CandyCane)
            {
                bool boostedHeart = player.Calamity().photosynthesis;
                if (NPC.AnyNPCs(ModContent.NPCType<SupremeCalamitas>()))
                {
                    player.statLife -= boostedHeart ? 5 : 10;
                    if (Main.myPlayer == player.whoAmI)
                        player.HealEffect(boostedHeart ? -5 : -10, true);
                }
                else if (boostedHeart)
                {
                    player.statLife += 5;
                    if (Main.myPlayer == player.whoAmI)
                        player.HealEffect(5, true);
                }
            }
            return true;
        }
        #endregion

        #region Use Item Changes
        public override bool UseItem(Item item, Player player)
        {
            if (player.Calamity().evilSmasherBoost > 0)
            {
                if (item.type != ModContent.ItemType<EvilSmasher>())
                    player.Calamity().evilSmasherBoost = 0;
            }

            if (player.HasBuff(BuffID.ParryDamageBuff))
            {
                if (item.type != ItemID.DD2SquireDemonSword)
                {
                    player.parryDamageBuff = false;
                    player.ClearBuff(BuffID.ParryDamageBuff);
                }
            }

            // Give 2 minutes of Honey buff when drinking Bottled Honey.
            if (item.type == ItemID.BottledHoney)
                player.AddBuff(BuffID.Honey, 7200);

            // Moon Lord instantly spawns when Celestial Sigil is used.
            if (item.type == ItemID.CelestialSigil)
            {
                NPC.MoonLordCountdown = 1;
                NetMessage.SendData(MessageID.MoonlordCountdown, -1, -1, null, NPC.MoonLordCountdown);
            }

            return base.UseItem(item, player);
        }

        public override bool AltFunctionUse(Item item, Player player)
        {
            if (player.Calamity().profanedCrystalBuffs && item.pick == 0 && item.axe == 0 && item.hammer == 0 && item.autoReuse && (item.Calamity().rogue || item.magic || item.ranged || item.melee))
            {
                NPC closest = Main.MouseWorld.ClosestNPCAt(1000f, true);
                if (closest != null)
                {
                    player.MinionAttackTargetNPC = closest.whoAmI;
                    player.UpdateMinionTarget();
                }
                return false;
            }
            if (player.ActiveItem().type == ModContent.ItemType<ViridVanguard>())
            {
                int bladeCount = 0;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active &&
                        Main.projectile[i].type == ModContent.ProjectileType<ViridVanguardBlade>() &&
                        Main.projectile[i].owner == player.whoAmI &&
                        Main.projectile[i].ModProjectile<ViridVanguardBlade>().FiringTime <= 0f)
                    {
                        bladeCount++;
                    }
                }
                if (bladeCount > 0)
                {
                    int bladeIndex = 0;
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        if (Main.projectile[i].modProjectile is ViridVanguardBlade)
                        {
                            if (Main.projectile[i].ModProjectile<ViridVanguardBlade>().FiringTime > 0f)
                                continue;
                        }
                        if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<ViridVanguardBlade>() && Main.projectile[i].owner == player.whoAmI)
                        {
                            Main.projectile[i].ModProjectile<ViridVanguardBlade>().FiringTime = 240f;
                            Main.projectile[i].ModProjectile<ViridVanguardBlade>().RedirectAngle = MathHelper.Lerp(0f, MathHelper.TwoPi, bladeIndex / (float)bladeCount);
                            Main.projectile[i].netUpdate = true;
                            bladeIndex++;
                        }
                    }
                }
                return false;
            }
            if (player.ActiveItem().type == ModContent.ItemType<IgneousExaltation>())
            {
                bool hasBlades = false;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<IgneousBlade>() && Main.projectile[i].owner == player.whoAmI && Main.projectile[i].localAI[1] == 0f)
                    {
                        hasBlades = true;
                        break;
                    }
                }
                if (hasBlades)
                {
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        if (Main.projectile[i].modProjectile is IgneousBlade)
                        {
                            if (Main.projectile[i].ModProjectile<IgneousBlade>().Firing)
                                continue;
                        }
                        if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<IgneousBlade>() && Main.projectile[i].owner == player.whoAmI && Main.projectile[i].localAI[1] == 0f)
                        {
                            Main.projectile[i].rotation = MathHelper.PiOver2 + MathHelper.PiOver4;
                            Main.projectile[i].velocity = Main.projectile[i].SafeDirectionTo(Main.MouseWorld, Vector2.UnitY) * 22f;
                            Main.projectile[i].rotation += Main.projectile[i].velocity.ToRotation();
                            Main.projectile[i].ai[0] = 180f;
                            Main.projectile[i].ModProjectile<IgneousBlade>().Firing = true;
                            Main.projectile[i].tileCollide = true;
                            Main.projectile[i].netUpdate = true;
                        }
                    }
                }
                return false;
            }
            if (player.ActiveItem().type == ModContent.ItemType<VoidConcentrationStaff>() && player.ownedProjectileCounts[ModContent.ProjectileType<VoidConcentrationBlackhole>()] == 0)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].modProjectile is VoidConcentrationAura)
                    {
                        if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI)
                        {
                            Main.projectile[i].ModProjectile<VoidConcentrationAura>().HandleRightClick();
                            break;
                        }
                    }
                }
                return false;
            }
            if (player.ActiveItem().type == ModContent.ItemType<ColdDivinity>())
            {
                bool canContinue = true;
                int count = 0;
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<ColdDivinityPointyThing>() && Main.projectile[i].owner == player.whoAmI)
                    {
                        if (Main.projectile[i].ai[1] > 1f)
                        {
                            canContinue = false;
                            break;
                        }
                        else if (Main.projectile[i].ai[1] == 0f)
                        {
                            if (((ColdDivinityPointyThing)Main.projectile[i].modProjectile).circlingPlayer)
                                count++;
                        }
                    }
                }
                if (canContinue && count > 0)
                {
                    NPC unluckyTarget = CalamityUtils.MinionHoming(Main.MouseWorld, 1000f, player);
                    if (unluckyTarget != null)
                    {
                        int height = unluckyTarget.getRect().Height;
                        int width = unluckyTarget.getRect().Width;
                        int pointyThingyAmount = count;
                        float angleVariance = MathHelper.TwoPi / pointyThingyAmount;
                        float angle = 0f;

                        for (int i = 0; i < pointyThingyAmount; i++)
                        {
                            if (Main.projectile.Length == Main.maxProjectiles)
                                break;
                            int projj = Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<ColdDivinityPointyThing>(), (int)(80 * player.MinionDamage()), 1f, player.whoAmI, angle, 2f);
                            angle += angleVariance;
                            for (int j = 0; j < 22; j++)
                            {
                                Dust dust = Dust.NewDustDirect(Main.projectile[projj].position, Main.projectile[projj].width, Main.projectile[projj].height, DustID.Ice);
                                dust.velocity = Vector2.UnitY * Main.rand.NextFloat(3f, 5.5f) * Main.rand.NextBool(2).ToDirectionInt();
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

            if (player.ownedProjectileCounts[ModContent.ProjectileType<RelicOfDeliveranceSpear>()] > 0 &&
                (item.damage > 0 || item.ammo != AmmoID.None))
            {
                return false; // Don't use weapons if you're charging with a spear
            }

            // Conversion for Andromeda
            if (player.ownedProjectileCounts[ModContent.ProjectileType<GiantIbanRobotOfDoom>()] > 0)
            {
                if (item.type == ItemID.WireKite)
                    return false;
                if (item.pick > 0 || item.axe > 0 || item.hammer > 0 || item.fishingPole > 0)
                    return false;
                // compiler optimization: && short-circuits, so if altFunctionUse != 0, Andromeda code is never called.
                if (item.Calamity().rogue || item.magic || item.ranged || item.melee)
                    return player.altFunctionUse == 0 && PrototypeAndromechaRing.TransformItemUsage(item, player);
            }

            // Conversion for Profaned Soul Crystal
            if (modPlayer.profanedCrystalBuffs && item.pick == 0 && item.axe == 0 && item.hammer == 0 && item.autoReuse && (modItem.rogue || item.magic || item.ranged || item.melee))
                return player.altFunctionUse == 0 ? ProfanedSoulCrystal.TransformItemUsage(item, player) : AltFunctionUse(item, player);

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
                    if (player.CheckMana(item) && item.modItem.CanUseItem(player))
                        Charge -= chargeNeeded;
                }
            }

            // Handle general use-item effects for the Gem Tech Armor.
            player.Calamity().GemTechState.OnItemUseEffects(item);

            if (item.type == ItemID.MonkStaffT1 || CalamityLists.spearAutoreuseList.Contains(item.type))
            {
                return player.ownedProjectileCounts[item.shoot] <= 0;
            }
            if (item.type == ItemID.InvisibilityPotion && player.FindBuffIndex(ModContent.BuffType<ShadowBuff>()) > -1)
            {
                return false;
            }
            if ((item.type == ItemID.RegenerationPotion || item.type == ItemID.LifeforcePotion) && player.FindBuffIndex(ModContent.BuffType<Cadence>()) > -1)
            {
                return false;
            }
            if (item.type == ModContent.ItemType<CrumblingPotion>() && player.FindBuffIndex(ModContent.BuffType<ArmorShattering>()) > -1)
            {
                return false;
            }
            if (item.type == ItemID.WrathPotion && player.FindBuffIndex(ModContent.BuffType<HolyWrathBuff>()) > -1)
            {
                return false;
            }
            if (item.type == ItemID.RagePotion && player.FindBuffIndex(ModContent.BuffType<ProfanedRageBuff>()) > -1)
            {
                return false;
            }
            if ((item.type == ItemID.SuperAbsorbantSponge || item.type == ItemID.EmptyBucket) && modPlayer.ZoneAbyss)
            {
                return false;
            }
            if (item.type == ItemID.MagicMirror || item.type == ItemID.IceMirror || item.type == ItemID.CellPhone || item.type == ItemID.RecallPotion)
            {
                return !player.HasBuff(ModContent.BuffType<BossZen>());
            }
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
                    bool templeCheck = Main.tile[x, y].wall != WallID.LihzahrdBrickUnsafe || y <= Main.worldSurface || NPC.downedPlantBoss;
                    if (templeCheck && !Collision.SolidCollision(teleportLocation, player.width, player.height))
                    {
                        int duration = CalamityPlayer.chaosStateDuration;
                        if (CalamityPlayer.areThereAnyDamnBosses || CalamityPlayer.areThereAnyDamnEvents)
                            duration = CalamityPlayer.chaosStateDurationBoss;
                        if (modPlayer.Cooldowns.Exists(cooldown => cooldown.GetType() == typeof(EvasionScarfCooldown)))
                            duration = (int)(duration * 1.5);
                        else if (modPlayer.Cooldowns.Exists(cooldown => cooldown.GetType() == typeof(CounterScarfCooldown)))
                            duration *= 2;
                        player.AddBuff(BuffID.ChaosState, duration, true);
                    }
                }
            }
            if (item.type == ItemID.SuspiciousLookingEye || item.type == ItemID.WormFood || item.type == ItemID.BloodySpine || item.type == ItemID.SlimeCrown || item.type == ItemID.Abeemination || item.type == ItemID.MechanicalEye || item.type == ItemID.MechanicalWorm || item.type == ItemID.MechanicalSkull || item.type == ItemID.CelestialSigil)
            {
                return !BossRushEvent.BossRushActive;
            }
            return true;
        }
        #endregion

        #region Modify Weapon Damage
        public override void ModifyWeaponDamage(Item item, Player player, ref float add, ref float mult, ref float flat)
        {
            // Nerf yoyo glove and bag because it's bad and stupid and dumb and bad.
            if (player.yoyoGlove && ItemID.Sets.Yoyo[item.type])
                mult *= 0.66f;

            // Nerf archery potion damage buff from 1.2x to 1.05x.
            if (item.useAmmo == AmmoID.Arrow && player.archery)
                mult *= 0.875f;

            if (item.type < ItemID.Count)
                return;

            // Summon weapons specifically do not have their damage affected by charge. They still require charge to function however.
            CalamityGlobalItem modItem = item.Calamity();

            if (!item.summon && modItem.DischargeEnchantExhaustion > 0f)
                mult *= DischargeEnchantmentDamageFormula();

            if (!item.summon && (modItem?.UsesCharge ?? false))
            {
                // At exactly zero charge, do not perform any multiplication.
                // This makes charge-using weapons show up at full damage when previewed in crafting, Recipe Browser, etc.
                if (Charge == 0f)
                    return;
                mult *= ChargeDamageFormula();
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

        // This formula gives a slightly higher value than 1.0 above 85% charge, and a slightly lower value than 0.0 at 0% charge.
        // Specifically, it gives 0.0 or less at 0.36% charge or lower. This is fine because the result is immediately clamped.
        internal float ChargeDamageFormula()
        {
            float x = MathHelper.Clamp(ChargeRatio, 0f, 1f);
            float y = 1.087f - 0.08f / (x + 0.07f);
            return MathHelper.Clamp(y, 0f, 1f);
        }
        #endregion

        #region Armor Set Changes
        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            string managedArmorName = VanillaArmorChangeManager.GetSetBonusName(Main.player[head.owner]);
            if (!string.IsNullOrEmpty(managedArmorName))
                return managedArmorName;

            if (head.type == ItemID.SquireGreatHelm && body.type == ItemID.SquirePlating && legs.type == ItemID.SquireGreaves)
                return "SquireTier2";
            if (head.type == ItemID.HuntressWig && body.type == ItemID.HuntressJerkin && legs.type == ItemID.HuntressPants)
                return "HuntressTier2";
            if (head.type == ItemID.ApprenticeHat && body.type == ItemID.ApprenticeRobe && legs.type == ItemID.ApprenticeTrousers)
                return "ApprenticeTier2";
            if (head.type == ItemID.MonkAltHead && body.type == ItemID.MonkAltShirt && legs.type == ItemID.MonkAltPants)
                return "MonkTier3";
            if (head.type == ItemID.SquireAltHead && body.type == ItemID.SquireAltShirt && legs.type == ItemID.SquireAltPants)
                return "SquireTier3";
            if (head.type == ItemID.HuntressAltHead && body.type == ItemID.HuntressAltShirt && legs.type == ItemID.HuntressAltPants)
                return "HuntressTier3";
            if (head.type == ItemID.ApprenticeAltHead && body.type == ItemID.ApprenticeAltShirt && legs.type == ItemID.ApprenticeAltPants)
                return "ApprenticeTier3";
            if (head.type == ItemID.SolarFlareHelmet && body.type == ItemID.SolarFlareBreastplate && legs.type == ItemID.SolarFlareLeggings)
                return "SolarFlare";
            return "";
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            CalamityPlayer modPlayer = player.Calamity();
            VanillaArmorChangeManager.CreateTooltipManuallyAsNecessary(player);

            if (set == "SquireTier2")
            {
                player.lifeRegen += 3;
                player.minionDamage += 0.15f;
                player.meleeCrit += 15;
                player.setBonus += "\nIncreases your life regeneration\n" +
                            "15% increased minion damage and melee critical strike chance";
            }
            else if (set == "HuntressTier2")
            {
                player.minionDamage += 0.1f;
                player.rangedDamage += 0.1f;
                player.setBonus += "\n10% increased minion and ranged damage";
            }
            else if (set == "ApprenticeTier2")
            {
                player.minionDamage += 0.05f;
                player.magicCrit += 15;
                player.setBonus += "\n5% increased minion damage and 15% increased magic critical strike chance";
            }
            else if (set == "MonkTier3")
            {
                player.minionDamage += 0.3f;
                player.meleeSpeed += 0.1f;
                player.meleeDamage += 0.1f;
                player.meleeCrit += 10;
                player.setBonus += "\n10% increased melee damage, melee critical strike chance and melee speed\n" +
                            "30% increased minion damage";
            }
            else if (set == "SquireTier3")
            {
                player.lifeRegen += 6;
                player.minionDamage += 0.1f;
                player.meleeCrit += 10;
                player.setBonus += "\nMassively increased life regeneration\n" +
                            "10% increased minion damage and melee critical strike chance";
            }
            else if (set == "HuntressTier3")
            {
                player.minionDamage += 0.1f;
                player.rangedDamage += 0.1f;
                player.setBonus += "\n10% increased minion and ranged damage";
            }
            else if (set == "ApprenticeTier3")
            {
                player.minionDamage += 0.1f;
                player.magicCrit += 15;
                player.setBonus += "\n10% increased minion damage and 15% increased magic critical strike chance";
            }
            else if (set == "SolarFlare")
            {
                modPlayer.dashMod = 0;
            }
        }
        #endregion

        #region Equip Changes
        public override void UpdateEquip(Item item, Player player)
        {
            VanillaArmorChangeManager.ApplyPotentialEffectsTo(player);

            switch (item.type)
            {
                case ItemID.MagicHat:
                    player.magicDamage -= 0.02f;
                    player.magicCrit -= 2;
                    break;

                case ItemID.WizardHat:
                    player.magicDamage -= 0.1f;
                    break;

                case ItemID.SpectreHood:
                    player.magicDamage += 0.2f;
                    break;

                case ItemID.SquireGreatHelm:
                    player.lifeRegen -= 7;
                    break;
                case ItemID.SquirePlating:
                    player.minionDamage -= 0.05f;
                    player.meleeDamage -= 0.05f;
                    break;
                case ItemID.SquireGreaves:
                    player.minionDamage -= 0.1f;
                    player.meleeCrit -= 10;
                    break;

                case ItemID.HuntressJerkin:
                    player.minionDamage -= 0.1f;
                    player.rangedDamage -= 0.1f;
                    break;

                case ItemID.ApprenticeTrousers:
                    player.minionDamage -= 0.05f;
                    player.magicCrit -= 15;
                    break;

                case ItemID.SquireAltShirt:
                    player.lifeRegen -= 14;
                    break;
                case ItemID.SquireAltPants:
                    player.minionDamage -= 0.1f;
                    player.meleeCrit -= 10;
                    break;

                case ItemID.MonkAltHead:
                    player.minionDamage -= 0.1f;
                    player.meleeDamage -= 0.1f;
                    break;
                case ItemID.MonkAltShirt:
                    player.minionDamage -= 0.1f;
                    player.meleeSpeed -= 0.1f;
                    break;
                case ItemID.MonkAltPants:
                    player.minionDamage -= 0.1f;
                    player.meleeCrit -= 10;
                    break;

                case ItemID.HuntressAltShirt:
                    player.minionDamage -= 0.1f;
                    player.rangedDamage -= 0.1f;
                    break;

                case ItemID.ApprenticeAltPants:
                    player.minionDamage -= 0.1f;
                    player.magicCrit -= 15;
                    break;
            }
        }
        #endregion

        #region Accessory Changes
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();

            if (item.prefix > 0)
            {
                float stealthGenBoost = item.Calamity().StealthGenBonus - 1f;
                if (stealthGenBoost > 0)
                {
                    modPlayer.accStealthGenBoost += stealthGenBoost;
                }
            }

            // Obsidian Skull and its upgrades make you immune to On Fire!
            if (item.type == ItemID.ObsidianSkull || item.type == ItemID.ObsidianHorseshoe || item.type == ItemID.ObsidianShield || item.type == ItemID.AnkhShield || item.type == ItemID.ObsidianWaterWalkingBoots || item.type == ItemID.LavaWaders)
                player.buffImmune[BuffID.OnFire] = true;

            if (item.type == ItemID.FrogLeg)
                player.jumpSpeedBoost -= 1.2f;

            if (item.type == ItemID.FireGauntlet)
            {
                player.meleeDamage += 0.04f;
                player.meleeSpeed += 0.04f;
            }

            if (item.type == ItemID.AngelWings) // Boost to max life, defense, and life regen
            {
                player.statLifeMax2 += 20;
                player.statDefense += 10;
                player.lifeRegen += 2;
                player.noFallDmg = true;
            }
            else if (item.type == ItemID.DemonWings) // Boost to all damage and crit
            {
                player.allDamage += 0.05f;
                modPlayer.AllCritBoost(5);
                player.noFallDmg = true;
            }
            else if (item.type == ItemID.FinWings) // Boosted water abilities, faster fall in water
            {
                player.moveSpeed += 0.15f;
                player.jumpSpeedBoost += 0.9f;
                player.gills = true;
                player.ignoreWater = true;
                player.noFallDmg = true;
                if (!player.mount.Active)
                {
                    if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
                        player.maxFallSpeed = 12f;
                }
            }
            else if (item.type == ItemID.BeeWings) // Honey buff
            {
                player.AddBuff(BuffID.Honey, 2);
                player.noFallDmg = true;
            }
            else if (item.type == ItemID.ButterflyWings) // Boost to magic stats
            {
                player.statManaMax2 += 20;
                player.magicDamage += 0.05f;
                player.manaCost *= 0.95f;
                player.magicCrit += 5;
                player.noFallDmg = true;
            }
            else if (item.type == ItemID.FairyWings) // Boost to max life
            {
                player.statLifeMax2 += 60;
                player.noFallDmg = true;
            }
            else if (item.type == ItemID.BatWings) // Stronger at night
            {
                player.noFallDmg = true;
                if (!Main.dayTime || Main.eclipse)
                {
                    player.jumpSpeedBoost += 0.5f;
                    player.allDamage += 0.07f;
                    modPlayer.AllCritBoost(3);
                    player.moveSpeed += 0.1f;
                }
            }
            else if (item.type == ItemID.HarpyWings)
            {
                modPlayer.harpyWingBoost = true;
                player.moveSpeed += 0.2f;
                player.noFallDmg = true;
            }
            else if (item.type == ItemID.BoneWings) // Bonus to ranged and defense stats while wearing necro armor
            {
                player.noFallDmg = true;
                if ((player.head == ArmorIDs.Head.NecroHelmet || player.head == ArmorIDs.Head.AncientNecroHelmet) &&
                    player.body == ArmorIDs.Body.NecroBreastplate && player.legs == ArmorIDs.Legs.NecroGreaves)
                {
                    player.moveSpeed += 0.1f;
                    player.rangedDamage += 0.1f;
                    player.rangedCrit += 10;
                    player.statDefense += 30;
                }
            }
            else if (item.type == ItemID.MothronWings) // Spawn baby mothrons over time to attack enemies, max of 3
            {
                player.statDefense += 5;
                player.allDamage += 0.05f;
                player.moveSpeed += 0.1f;
                player.jumpSpeedBoost += 0.6f;
                player.noFallDmg = true;
            }
            else if (item.type == ItemID.FrozenWings) // Bonus to melee and ranged stats while wearing frost armor
            {
                player.noFallDmg = true;
                if (player.head == ArmorIDs.Head.FrostHelmet && player.body == ArmorIDs.Body.FrostBreastplate && player.legs == ArmorIDs.Legs.FrostLeggings)
                {
                    player.meleeDamage += 0.02f;
                    player.rangedDamage += 0.02f;
                    player.meleeCrit += 1;
                    player.rangedCrit += 1;
                }
            }
            else if (item.type == ItemID.FlameWings) // Bonus to melee stats
            {
                player.meleeDamage += 0.05f;
                player.meleeCrit += 5;
                player.noFallDmg = true;
            }
            else if (item.type == ItemID.GhostWings) // Bonus to mage stats while wearing spectre armor
            {
                player.noFallDmg = true;
                if (player.body == ArmorIDs.Body.SpectreRobe && player.legs == ArmorIDs.Legs.SpectrePants)
                {
                    if (player.head == ArmorIDs.Head.SpectreHood)
                    {
                        player.statDefense += 10;
                        player.endurance += 0.05f;
                    }
                    else if (player.head == ArmorIDs.Head.SpectreMask)
                    {
                        player.magicDamage += 0.05f;
                        player.magicCrit += 5;
                    }
                }
            }
            else if (item.type == ItemID.BeetleWings) // Boosted defense and melee stats while wearing beetle armor
            {
                player.noFallDmg = true;
                if (player.head == ArmorIDs.Head.BeetleHelmet && player.legs == ArmorIDs.Legs.BeetleLeggings)
                {
                    if (player.body == ArmorIDs.Body.BeetleShell)
                    {
                        player.statDefense += 10;
                        player.endurance += 0.05f;
                    }
                    else if (player.body == ArmorIDs.Body.BeetleScaleMail)
                    {
                        player.meleeDamage += 0.05f;
                        player.meleeCrit += 5;
                    }
                }
            }
            else if (item.type == ItemID.Hoverboard) // Boosted ranged stats while wearing shroomite armor
            {
                player.noFallDmg = true;
                if (player.body == ArmorIDs.Body.ShroomiteBreastplate && player.legs == ArmorIDs.Legs.ShroomiteLeggings)
                {
                    if (player.head == ArmorIDs.Head.ShroomiteHeadgear) //arrows
                    {
                        player.arrowDamage += 0.05f;
                    }
                    else if (player.head == ArmorIDs.Head.ShroomiteMask) //bullets
                    {
                        player.bulletDamage += 0.05f;
                    }
                    else if (player.head == ArmorIDs.Head.ShroomiteHelmet) //rockets
                    {
                        player.rocketDamage += 0.05f;
                    }
                    else if (player.Calamity().flamethrowerBoost) //flamethrowers
                    {
                        player.Calamity().hoverboardBoost = true;
                    }
                }
            }
            else if (item.type == ItemID.LeafWings) // Bonus to defensive stats while wearing tiki armor
            {
                player.noFallDmg = true;
                if (player.head == ArmorIDs.Head.TikiMask && player.body == ArmorIDs.Body.TikiShirt && player.legs == ArmorIDs.Legs.TikiPants)
                {
                    player.statDefense += 5;
                    player.endurance += 0.05f;
                    player.AddBuff(BuffID.DryadsWard, 5, true); // Dryad's Blessing
                }
            }
            else if (item.type == ItemID.FestiveWings) // Drop powerful homing christmas tree bulbs while in flight
            {
                player.noFallDmg = true;
                player.statLifeMax2 += 40;
                if (modPlayer.icicleCooldown <= 0)
                {
                    if (player.controlJump && !player.jumpAgainCloud && player.jump == 0 && player.velocity.Y != 0f && !player.mount.Active && !player.mount.Cart)
                    {
                        int p = Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 2f, ProjectileID.OrnamentFriendly, (int)(100 * player.AverageDamage()), 5f, player.whoAmI);
                        if (p.WithinBounds(Main.maxProjectiles))
                        {
                            Main.projectile[p].Calamity().forceTypeless = true;
                            Main.projectile[p].Calamity().lineColor = 1;
                            modPlayer.icicleCooldown = 10;
                        }
                    }
                }
            }
            else if (item.type == ItemID.SpookyWings) // Bonus to summon stats while wearing spooky armor
            {
                player.noFallDmg = true;
                if (player.head == ArmorIDs.Head.SpookyHelmet && player.body == ArmorIDs.Body.SpookyBreastplate && player.legs == ArmorIDs.Legs.SpookyLeggings)
                {
                    player.minionKB += 2f;
                    player.minionDamage += 0.05f;
                }
            }
            else if (item.type == ItemID.TatteredFairyWings)
            {
                player.allDamage += 0.05f;
                modPlayer.AllCritBoost(5);
                player.noFallDmg = true;
            }
            else if (item.type == ItemID.SteampunkWings)
            {
                player.statDefense += 8;
                player.allDamage += 0.04f;
                modPlayer.AllCritBoost(2);
                player.moveSpeed += 0.1f;
                player.noFallDmg = true;
            }
            else if (item.type == ItemID.WingsSolar) // Bonus to melee stats while wearing solar flare armor
            {
                player.noFallDmg = true;
                if (player.head == ArmorIDs.Head.SolarFlareHelmet && player.body == ArmorIDs.Body.SolarFlareBreastplate && player.legs == ArmorIDs.Legs.SolarFlareLeggings)
                {
                    player.meleeDamage += 0.07f;
                    player.meleeCrit += 3;
                }
            }
            else if (item.type == ItemID.WingsVortex) // Bonus to ranged stats while wearing vortex armor
            {
                player.noFallDmg = true;
                if (player.head == ArmorIDs.Head.VortexHelmet && player.body == ArmorIDs.Body.VortexBreastplate && player.legs == ArmorIDs.Legs.VortexLeggings)
                {
                    player.rangedDamage += 0.03f;
                    player.rangedCrit += 7;
                }
            }
            else if (item.type == ItemID.WingsNebula) // Bonus to magic stats while wearing nebula armor
            {
                player.noFallDmg = true;
                if (player.head == ArmorIDs.Head.NebulaHelmet && player.body == ArmorIDs.Body.NebulaBreastplate && player.legs == ArmorIDs.Legs.NebulaLeggings)
                {
                    player.magicDamage += 0.05f;
                    player.magicCrit += 5;
                    player.statManaMax2 += 20;
                    player.manaCost *= 0.95f;
                }
            }
            else if (item.type == ItemID.WingsStardust) // Bonus to summon stats while wearing stardust armor
            {
                player.noFallDmg = true;
                if (player.head == ArmorIDs.Head.StardustHelmet && player.body == ArmorIDs.Body.StardustPlate && player.legs == ArmorIDs.Legs.StardustLeggings)
                {
                    player.maxMinions++;
                    player.minionDamage += 0.05f;
                }
            }
            else if (item.type == ItemID.FishronWings || item.type == ItemID.BetsyWings || item.type == ItemID.Yoraiz0rWings ||
                item.type == ItemID.JimsWings || item.type == ItemID.SkiphsWings || item.type == ItemID.LokisWings ||
                item.type == ItemID.ArkhalisWings || item.type == ItemID.LeinforsWings || item.type == ItemID.BejeweledValkyrieWing ||
                item.type == ItemID.RedsWings || item.type == ItemID.DTownsWings || item.type == ItemID.WillsWings ||
                item.type == ItemID.CrownosWings || item.type == ItemID.CenxsWings)
            {
                player.noFallDmg = true;
            }

            if (item.type == ItemID.JellyfishNecklace || item.type == ItemID.JellyfishDivingGear || item.type == ItemID.ArcticDivingGear)
                modPlayer.jellyfishNecklace = true;

            if (item.type == ItemID.FleshKnuckles)
                modPlayer.fleshKnuckles = true;

            if (item.type == ItemID.WormScarf)
                player.endurance -= 0.07f;

            if (item.type == ItemID.RoyalGel)
                modPlayer.royalGel = true;

            if (item.type == ItemID.HandWarmer)
                modPlayer.handWarmer = true;

            if (item.type == ItemID.EoCShield || item.type == ItemID.Tabi || item.type == ItemID.MasterNinjaGear)
                modPlayer.dashMod = 0;

            // Hard / Guarding / Armored / Warding give 0.25% / 0.5% / 0.75% / 1% DR
            if (item.prefix == PrefixID.Hard)
            {
                /* Prehardmode = 1
                 * Hardmode = 2
                 * Post-Golem = 2
                 * Post-Moon Lord = 2
                 * Post-Provi = 2
                 * Post-Polter = 3
                 * Post-DoG = 3
                 * Post-Yharon = 4
                 */
                
                if (CalamityWorld.downedYharon)
                    player.statDefense += 3;
                else if (CalamityWorld.downedPolterghast || CalamityWorld.downedDoG)
                    player.statDefense += 2;
                else if (Main.hardMode || NPC.downedGolemBoss || NPC.downedMoonlord || CalamityWorld.downedProvidence)
                    player.statDefense += 1;

                player.endurance += 0.0025f;
            }
            if (item.prefix == PrefixID.Guarding)
            {
                /* Prehardmode = 2
                 * Hardmode = 3
                 * Post-Golem = 4
                 * Post-Moon Lord = 4
                 * Post-Provi = 5
                 * Post-Polter = 5
                 * Post-DoG = 6
                 * Post-Yharon = 8
                 */
                
                if (CalamityWorld.downedYharon)
                    player.statDefense += 6;
                else if (CalamityWorld.downedDoG)
                    player.statDefense += 4;
                else if (CalamityWorld.downedProvidence || CalamityWorld.downedPolterghast)
                    player.statDefense += 3;
                else if (NPC.downedGolemBoss || NPC.downedMoonlord)
                    player.statDefense += 2;
                else if (Main.hardMode)
                    player.statDefense += 1;

                player.endurance += 0.005f;
            }
            if (item.prefix == PrefixID.Armored)
            {
                /* Prehardmode = 3
                 * Hardmode = 5
                 * Post-Golem = 5
                 * Post-Moon Lord = 6
                 * Post-Provi = 7
                 * Post-Polter = 8
                 * Post-DoG = 9
                 * Post-Yharon = 11
                 */
                
                if (CalamityWorld.downedYharon)
                    player.statDefense += 8;
                else if (CalamityWorld.downedDoG)
                    player.statDefense += 6;
                else if (CalamityWorld.downedPolterghast)
                    player.statDefense += 5;
                else if (CalamityWorld.downedProvidence)
                    player.statDefense += 4;
                else if (NPC.downedMoonlord)
                    player.statDefense += 3;
                else if (Main.hardMode || NPC.downedGolemBoss)
                    player.statDefense += 2;

                player.endurance += 0.0075f;
            }
            if (item.prefix == PrefixID.Warding)
            {
                /* Prehardmode = 4
                 * Hardmode = 6
                 * Post-Golem = 7
                 * Post-Moon Lord = 8
                 * Post-Provi = 9
                 * Post-Polter = 10
                 * Post-DoG = 12
                 * Post-Yharon = 15
                 */
                
                if (CalamityWorld.downedYharon)
                    player.statDefense += 11;
                else if (CalamityWorld.downedDoG)
                    player.statDefense += 8;
                else if (CalamityWorld.downedPolterghast)
                    player.statDefense += 6;
                else if (CalamityWorld.downedProvidence)
                    player.statDefense += 5;
                else if (NPC.downedMoonlord)
                    player.statDefense += 4;
                else if (NPC.downedGolemBoss)
                    player.statDefense += 3;
                else if (Main.hardMode)
                    player.statDefense += 2;

                player.endurance += 0.01f;
            }

            // TODO -- make all prefixes relevant. Details in Ozz's todo.
            /*
            if (item.prefix == PrefixID.Precise || item.prefix == PrefixID.Lucky)
                player.armorPenetration += 1;
            */
        }
        #endregion

        #region WingChanges
        public override void HorizontalWingSpeeds(Item item, Player player, ref float speed, ref float acceleration)
        {
            CalamityPlayer modPlayer = player.Calamity();
            float moveSpeedBoost = modPlayer.moveSpeedStat * 0.001f;

            float flightSpeedMult = 1f +
                (modPlayer.soaring ? 0.1f : 0f) +
                (modPlayer.profanedRage ? 0.05f : 0f) +
                (modPlayer.draconicSurge ? 0.1f : 0f) +
                (modPlayer.reaverSpeed ? 0.1f : 0f) +
                moveSpeedBoost;

            float flightAccMult = 1f +
                (modPlayer.draconicSurge ? 0.1f : 0f) +
                moveSpeedBoost;

            flightSpeedMult = MathHelper.Clamp(flightSpeedMult, 0.5f, 1.5f);
            speed *= flightSpeedMult;

            flightAccMult = MathHelper.Clamp(flightAccMult, 0.5f, 1.5f);
            acceleration *= flightAccMult;
        }
        #endregion

        #region GrabChanges
        public override void GrabRange(Item item, Player player, ref int grabRange)
        {
            CalamityPlayer modPlayer = player.Calamity();
            int itemGrabRangeBoost = 0 +
                (modPlayer.reaverExplore ? 20 : 0);

            grabRange += itemGrabRangeBoost;
        }
        #endregion

        #region Ammo
        public override bool ConsumeAmmo(Item item, Player player) => Main.rand.NextFloat() <= player.Calamity().rangedAmmoCost;

        public static bool HasEnoughAmmo(Player player, Item item, int ammoConsumed)
        {
            bool flag = false;
            bool canShoot = false;

            for (int i = 54; i < Main.maxInventory; i++)
            {
                if (player.inventory[i].ammo == item.useAmmo && (player.inventory[i].stack >= ammoConsumed || !player.inventory[i].consumable))
                {
                    canShoot = true;
                    flag = true;
                    break;
                }
            }

            if (!flag)
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
            bool flag = false;
            bool dontConsumeAmmo = false;

            for (int i = 54; i < Main.maxInventory; i++)
            {
                if (player.inventory[i].ammo == item.useAmmo && (player.inventory[i].stack >= ammoConsumed || !player.inventory[i].consumable))
                {
                    itemAmmo = player.inventory[i];
                    flag = true;
                    break;
                }
            }

            if (!flag)
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

            if (player.ammoBox && Main.rand.NextBool(5))
                dontConsumeAmmo = true;
            if (player.ammoPotion && Main.rand.NextBool(5))
                dontConsumeAmmo = true;
            if (player.ammoCost80 && Main.rand.NextBool(5))
                dontConsumeAmmo = true;
            if (player.ammoCost75 && Main.rand.NextBool(4))
                dontConsumeAmmo = true;
            if (Main.rand.NextFloat() > player.Calamity().rangedAmmoCost)
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
            if (CalamityLists.forceItemList?.Contains(item.type) ?? false)
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
            Texture2D itemTexture = Main.itemTexture[item.type];
            Rectangle itemFrame = (Main.itemAnimations[item.type] == null) ? itemTexture.Frame() : Main.itemAnimations[item.type].GetFrame(itemTexture);

            if (!EnchantmentManager.ItemUpgradeRelationship.ContainsKey(item.type) || !Main.LocalPlayer.InventoryHas(ModContent.ItemType<BrimstoneLocus>()))
                return true;

            // Draw all particles.
            float currentPower = 0f;
            int calamitasNPCIndex = NPC.FindFirstNPC(ModContent.NPCType<WITCH>());
            if (calamitasNPCIndex != -1)
                currentPower = Utils.InverseLerp(11750f, 1000f, Main.LocalPlayer.Distance(Main.npc[calamitasNPCIndex].Center), true);

            Vector2 particleDrawCenter = position + new Vector2(12f, 16f) * Main.inventoryScale;

            EnchantmentEnergyParticles.InterpolationSpeed = MathHelper.Lerp(0.035f, 0.1f, currentPower);
            EnchantmentEnergyParticles.DrawSet(particleDrawCenter + Main.screenPosition);
            spriteBatch.Draw(itemTexture, position, itemFrame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);

            return false;
        }
        #endregion

        #region Reforging
        private int NewPrefixType(Item item)
        {
            int prefix = -2;
            if (item.accessory)
            {
                if (Main.player[Main.myPlayer].Calamity().finalTierAccessoryReforge)
                {
                    // Warding = 18, Menacing = 19, Quick = 20, Violent = 21, Lucky = 22, Silent = 23
                    prefix = Main.rand.Next(18, 24);
                }
                else
                {
                    switch (reforgeTier)
                    {
                        case 0:
                            break;
                        case 1:
                            // Hard = 1, Jagged = 2, Brisk = 3, Wild = 4, Quiet = 5
                            prefix = Main.rand.Next(1, 6);
                            break;
                        case 2:
                            // Guarding = 6, Spiked = 7, Fleeting = 8, Rash = 9, Cloaked = 10
                            prefix = Main.rand.Next(6, 11);
                            break;
                        case 3:
                            // Precise = 11, Arcane = 12
                            prefix = Main.rand.Next(11, 13);
                            break;
                        case 4:
                            // Armored = 13, Angry = 14, Hasty = 15, Intrepid = 16, Camouflaged = 17
                            prefix = Main.rand.Next(13, 18);
                            break;
                        case 5:
                        case 6:
                            // Warding = 18, Menacing = 19, Quick = 20, Violent = 21, Lucky = 22, Silent = 23
                            prefix = Main.rand.Next(18, 24);
                            break;
                    }
                }
                switch (prefix)
                {
                    case -2:
                        break;
                    case 1:
                        prefix = PrefixID.Hard;
                        break;
                    case 2:
                        prefix = PrefixID.Jagged;
                        break;
                    case 3:
                        prefix = PrefixID.Brisk;
                        break;
                    case 4:
                        prefix = PrefixID.Wild;
                        break;
                    case 5:
                        prefix = mod.PrefixType("Quiet");
                        break;
                    case 6:
                        prefix = PrefixID.Guarding;
                        break;
                    case 7:
                        prefix = PrefixID.Spiked;
                        break;
                    case 8:
                        prefix = PrefixID.Fleeting;
                        break;
                    case 9:
                        prefix = PrefixID.Rash;
                        break;
                    case 10:
                        prefix = mod.PrefixType("Cloaked");
                        break;
                    case 11:
                        prefix = PrefixID.Precise;
                        break;
                    case 12:
                        prefix = PrefixID.Arcane;
                        break;
                    case 13:
                        prefix = PrefixID.Armored;
                        break;
                    case 14:
                        prefix = PrefixID.Angry;
                        break;
                    case 15:
                        prefix = PrefixID.Hasty2;
                        break;
                    case 16:
                        prefix = PrefixID.Intrepid;
                        break;
                    case 17:
                        prefix = mod.PrefixType("Camouflaged");
                        break;
                    case 18:
                        prefix = PrefixID.Warding;
                        break;
                    case 19:
                        prefix = PrefixID.Menacing;
                        break;
                    case 20:
                        prefix = PrefixID.Quick2;
                        break;
                    case 21:
                        prefix = PrefixID.Violent;
                        break;
                    case 22:
                        prefix = PrefixID.Lucky;
                        break;
                    case 23:
                        prefix = mod.PrefixType("Silent");
                        break;
                }
            }
            else if (item.melee)
            {
                // Yoyos, Flails, Spears, etc.
                if (item.channel || item.noMelee)
                {
                    switch (reforgeTier)
                    {
                        case 0:
                            break;
                        case 1:
                            // Keen = 1, Ruthless = 2
                            prefix = Main.rand.Next(1, 3);
                            break;
                        case 2:
                            // Hurtful = 3, Zealous = 4
                            prefix = Main.rand.Next(3, 5);
                            break;
                        case 3:
                            // Forceful = 5, Strong = 6
                            prefix = Main.rand.Next(5, 7);
                            break;
                        case 4:
                            // Demonic = 7
                            prefix = 7;
                            break;
                        case 5:
                            // Superior = 8
                            prefix = 8;
                            break;
                        case 6:
                            // Godly = 9
                            prefix = 9;
                            break;
                    }
                    switch (prefix)
                    {
                        case -2:
                            break;
                        case 1:
                            prefix = PrefixID.Keen;
                            break;
                        case 2:
                            prefix = PrefixID.Ruthless;
                            break;
                        case 3:
                            prefix = PrefixID.Hurtful;
                            break;
                        case 4:
                            prefix = PrefixID.Zealous;
                            break;
                        case 5:
                            prefix = PrefixID.Forceful;
                            break;
                        case 6:
                            prefix = PrefixID.Strong;
                            break;
                        case 7:
                            prefix = PrefixID.Demonic;
                            break;
                        case 8:
                            prefix = PrefixID.Superior;
                            break;
                        case 9:
                            prefix = PrefixID.Godly;
                            break;
                    }
                }
                // All other melee weapons
                else
                {
                    switch (reforgeTier)
                    {
                        case 0:
                            break;
                        case 1:
                            // Keen = 1, Ruthless = 2, Nimble = 3, Nasty = 4, Heavy = 5, Light = 6
                            prefix = Main.rand.Next(1, 7);
                            break;
                        case 2:
                            // Hurtful = 7, Zealous = 8, Quick = 9, Pointy = 10, Bulky = 11
                            prefix = Main.rand.Next(7, 12);
                            break;
                        case 3:
                            // Forceful = 12, Strong = 13, Agile = 14, Large = 15, Dangerous = 16, Sharp = 17
                            prefix = Main.rand.Next(12, 18);
                            break;
                        case 4:
                            // Murderous = 18, Massive = 19, Unpleasant = 20, Deadly2 = 21
                            prefix = Main.rand.Next(18, 22);
                            break;
                        case 5:
                            // Superior = 22, Demonic = 23, Savage = 24
                            prefix = Main.rand.Next(22, 25);
                            break;
                        case 6:
                            // Legendary = 25, Light = 6 ~We are tool friendly (=
                            if (item.pick > 0 || item.axe > 0 || item.hammer > 0)
                                prefix = Main.rand.NextBool() ? 25 : 6;
                            else
                                prefix = 25;
                            break;
                    }
                    switch (prefix)
                    {
                        case -2:
                            break;
                        case 1:
                            prefix = PrefixID.Keen;
                            break;
                        case 2:
                            prefix = PrefixID.Ruthless;
                            break;
                        case 3:
                            prefix = PrefixID.Nimble;
                            break;
                        case 4:
                            prefix = PrefixID.Nasty;
                            break;
                        case 5:
                            prefix = PrefixID.Heavy;
                            break;
                        case 6:
                            prefix = PrefixID.Light;
                            break;
                        case 7:
                            prefix = PrefixID.Hurtful;
                            break;
                        case 8:
                            prefix = PrefixID.Zealous;
                            break;
                        case 9:
                            prefix = PrefixID.Quick;
                            break;
                        case 10:
                            prefix = PrefixID.Pointy;
                            break;
                        case 11:
                            prefix = PrefixID.Bulky;
                            break;
                        case 12:
                            prefix = PrefixID.Forceful;
                            break;
                        case 13:
                            prefix = PrefixID.Strong;
                            break;
                        case 14:
                            prefix = PrefixID.Agile;
                            break;
                        case 15:
                            prefix = PrefixID.Large;
                            break;
                        case 16:
                            prefix = PrefixID.Dangerous;
                            break;
                        case 17:
                            prefix = PrefixID.Sharp;
                            break;
                        case 18:
                            prefix = PrefixID.Murderous;
                            break;
                        case 19:
                            prefix = PrefixID.Massive;
                            break;
                        case 20:
                            prefix = PrefixID.Unpleasant;
                            break;
                        case 21:
                            prefix = PrefixID.Deadly2;
                            break;
                        case 22:
                            prefix = PrefixID.Superior;
                            break;
                        case 23:
                            prefix = PrefixID.Demonic;
                            break;
                        case 24:
                            prefix = PrefixID.Savage;
                            break;
                        case 25:
                            prefix = PrefixID.Legendary;
                            break;
                    }
                }
            }
            else if (item.ranged)
            {
                switch (reforgeTier)
                {
                    case 0:
                        break;
                    case 1:
                        // Keen = 1, Ruthless = 2, Nimble = 3, Nasty = 4, Powerful = 5
                        prefix = Main.rand.Next(1, 6);
                        break;
                    case 2:
                        // Hurtful = 6, Zealous = 7, Quick = 8
                        prefix = Main.rand.Next(6, 9);
                        break;
                    case 3:
                        // Forceful = 9, Strong = 10, Agile = 11, Sighted = 12, Murderous = 13
                        prefix = Main.rand.Next(9, 14);
                        break;
                    case 4:
                        // Superior = 14, Demonic = 15, Deadly2 = 16, Intimidating = 17, Unpleasant = 18
                        prefix = Main.rand.Next(14, 19);
                        break;
                    case 5:
                        // Godly = 19, Rapid = 20, Hasty = 21, Deadly = 22, Staunch = 23
                        prefix = Main.rand.Next(19, 24);
                        break;
                    case 6:
                        // Unreal = 24
                        prefix = 24;
                        break;
                }
                switch (prefix)
                {
                    case -2:
                        break;
                    case 1:
                        prefix = PrefixID.Keen;
                        break;
                    case 2:
                        prefix = PrefixID.Ruthless;
                        break;
                    case 3:
                        prefix = PrefixID.Nimble;
                        break;
                    case 4:
                        prefix = PrefixID.Nasty;
                        break;
                    case 5:
                        prefix = PrefixID.Powerful;
                        break;
                    case 6:
                        prefix = PrefixID.Hurtful;
                        break;
                    case 7:
                        prefix = PrefixID.Zealous;
                        break;
                    case 8:
                        prefix = PrefixID.Quick;
                        break;
                    case 9:
                        prefix = PrefixID.Forceful;
                        break;
                    case 10:
                        prefix = PrefixID.Strong;
                        break;
                    case 11:
                        prefix = PrefixID.Agile;
                        break;
                    case 12:
                        prefix = PrefixID.Sighted;
                        break;
                    case 13:
                        prefix = PrefixID.Murderous;
                        break;
                    case 14:
                        prefix = PrefixID.Superior;
                        break;
                    case 15:
                        prefix = PrefixID.Demonic;
                        break;
                    case 16:
                        prefix = PrefixID.Deadly2;
                        break;
                    case 17:
                        prefix = PrefixID.Intimidating;
                        break;
                    case 18:
                        prefix = PrefixID.Unpleasant;
                        break;
                    case 19:
                        prefix = PrefixID.Godly;
                        break;
                    case 20:
                        prefix = PrefixID.Rapid;
                        break;
                    case 21:
                        prefix = PrefixID.Hasty;
                        break;
                    case 22:
                        prefix = PrefixID.Deadly;
                        break;
                    case 23:
                        prefix = PrefixID.Staunch;
                        break;
                    case 24:
                        prefix = PrefixID.Unreal;
                        break;
                }
            }
            else if (item.magic)
            {
                switch (reforgeTier)
                {
                    case 0:
                        break;
                    case 1:
                        // Keen = 1, Ruthless = 2, Nimble = 3, Nasty = 4, Furious = 5
                        prefix = Main.rand.Next(1, 6);
                        break;
                    case 2:
                        // Hurtful = 6, Zealous = 7, Quick = 8, Taboo = 9, Manic = 10
                        prefix = Main.rand.Next(6, 11);
                        break;
                    case 3:
                        // Forceful = 11, Strong = 12, Agile = 13, Murderous = 14, Adept = 15, Celestial = 16
                        prefix = Main.rand.Next(11, 17);
                        break;
                    case 4:
                        // Superior = 17, Demonic = 18, Deadly2 = 19, Mystic = 20
                        prefix = Main.rand.Next(17, 21);
                        break;
                    case 5:
                        // Godly = 21, Masterful = 22
                        prefix = Main.rand.Next(21, 23);
                        break;
                    case 6:
                        // Mythical = 23
                        prefix = 23;
                        break;
                }
                switch (prefix)
                {
                    case -2:
                        break;
                    case 1:
                        prefix = PrefixID.Keen;
                        break;
                    case 2:
                        prefix = PrefixID.Ruthless;
                        break;
                    case 3:
                        prefix = PrefixID.Nimble;
                        break;
                    case 4:
                        prefix = PrefixID.Nasty;
                        break;
                    case 5:
                        prefix = PrefixID.Furious;
                        break;
                    case 6:
                        prefix = PrefixID.Hurtful;
                        break;
                    case 7:
                        prefix = PrefixID.Zealous;
                        break;
                    case 8:
                        prefix = PrefixID.Quick;
                        break;
                    case 9:
                        prefix = PrefixID.Taboo;
                        break;
                    case 10:
                        prefix = PrefixID.Manic;
                        break;
                    case 11:
                        prefix = PrefixID.Forceful;
                        break;
                    case 12:
                        prefix = PrefixID.Strong;
                        break;
                    case 13:
                        prefix = PrefixID.Agile;
                        break;
                    case 14:
                        prefix = PrefixID.Murderous;
                        break;
                    case 15:
                        prefix = PrefixID.Adept;
                        break;
                    case 16:
                        prefix = PrefixID.Celestial;
                        break;
                    case 17:
                        prefix = PrefixID.Superior;
                        break;
                    case 18:
                        prefix = PrefixID.Demonic;
                        break;
                    case 19:
                        prefix = PrefixID.Deadly2;
                        break;
                    case 20:
                        prefix = PrefixID.Mystic;
                        break;
                    case 21:
                        prefix = PrefixID.Godly;
                        break;
                    case 22:
                        prefix = PrefixID.Masterful;
                        break;
                    case 23:
                        prefix = PrefixID.Mythical;
                        break;
                }
            }
            else if (item.summon)
            {
                switch (reforgeTier)
                {
                    case 0:
                        break;
                    case 1:
                        // Nimble = 1, Furious = 2
                        prefix = Main.rand.Next(1, 3);
                        break;
                    case 2:
                        // Hurtful = 3, Quick = 4, Taboo = 5, Manic = 6
                        prefix = Main.rand.Next(3, 7);
                        break;
                    case 3:
                        // Forceful = 7, Strong = 8, Adept = 9, Celestial = 10
                        prefix = Main.rand.Next(7, 11);
                        break;
                    case 4:
                        // Deadly2 = 11, Mystic = 12, Superior = 13, Demonic = 14
                        prefix = Main.rand.Next(11, 15);
                        break;
                    case 5:
                        // Masterful = 15, Mythical = 16, Godly = 17
                        prefix = Main.rand.Next(15, 18);
                        break;
                    case 6:
                        // Ruthless = 18
                        prefix = 18;
                        break;
                }
                switch (prefix)
                {
                    case -2:
                        break;
                    case 1:
                        prefix = PrefixID.Nimble;
                        break;
                    case 2:
                        prefix = PrefixID.Furious;
                        break;
                    case 3:
                        prefix = PrefixID.Hurtful;
                        break;
                    case 4:
                        prefix = PrefixID.Quick;
                        break;
                    case 5:
                        prefix = PrefixID.Taboo;
                        break;
                    case 6:
                        prefix = PrefixID.Manic;
                        break;
                    case 7:
                        prefix = PrefixID.Forceful;
                        break;
                    case 8:
                        prefix = PrefixID.Strong;
                        break;
                    case 9:
                        prefix = PrefixID.Adept;
                        break;
                    case 10:
                        prefix = PrefixID.Celestial;
                        break;
                    case 11:
                        prefix = PrefixID.Deadly2;
                        break;
                    case 12:
                        prefix = PrefixID.Mystic;
                        break;
                    case 13:
                        prefix = PrefixID.Superior;
                        break;
                    case 14:
                        prefix = PrefixID.Demonic;
                        break;
                    case 15:
                        prefix = PrefixID.Masterful;
                        break;
                    case 16:
                        prefix = PrefixID.Mythical;
                        break;
                    case 17:
                        prefix = PrefixID.Godly;
                        break;
                    case 18:
                        prefix = PrefixID.Ruthless;
                        break;
                }
            }
            else if (item.Calamity().rogue)
            {
                switch (reforgeTier)
                {
                    case 0:
                        break;
                    case 1:
                        // Radical = 1, Pointy = 2
                        prefix = Main.rand.Next(1, 3);
                        break;
                    case 2:
                        // Sharp = 3, Glorious = 4
                        prefix = Main.rand.Next(3, 5);
                        break;
                    case 3:
                        // Feathered = 5, Sleek = 6, Hefty = 7
                        prefix = Main.rand.Next(5, 8);
                        break;
                    case 4:
                        // Mighty = 8, Serrated = 9
                        prefix = Main.rand.Next(8, 10);
                        break;
                    case 5:
                        // Vicious = 10, Lethal = 11
                        prefix = Main.rand.Next(10, 12);
                        break;
                    case 6:
                        // Flawless = 12
                        prefix = 12;
                        break;
                }
                switch (prefix)
                {
                    case -2:
                        break;
                    case 1:
                        prefix = mod.PrefixType("Radical");
                        break;
                    case 2:
                        prefix = mod.PrefixType("Pointy");
                        break;
                    case 3:
                        prefix = mod.PrefixType("Sharp");
                        break;
                    case 4:
                        prefix = mod.PrefixType("Glorious");
                        break;
                    case 5:
                        prefix = mod.PrefixType("Feathered");
                        break;
                    case 6:
                        prefix = mod.PrefixType("Sleek");
                        break;
                    case 7:
                        prefix = mod.PrefixType("Hefty");
                        break;
                    case 8:
                        prefix = mod.PrefixType("Mighty");
                        break;
                    case 9:
                        prefix = mod.PrefixType("Serrated");
                        break;
                    case 10:
                        prefix = mod.PrefixType("Vicious");
                        break;
                    case 11:
                        prefix = mod.PrefixType("Lethal");
                        break;
                    case 12:
                        prefix = mod.PrefixType("Flawless");
                        break;
                }
            }
            return prefix;
        }

        // Cut price in half because fuck the goblin
        public override bool ReforgePrice(Item item, ref int reforgePrice, ref bool canApplyDiscount)
        {
            reforgePrice /= 2;
            return true;
        }

        // Get cash from dickhead goblin fuck
        public override void PostReforge(Item item)
        {
            CalamityPlayer modPlayer = Main.player[Main.myPlayer].Calamity();
            if (modPlayer.itemTypeLastReforged != item.type)
            {
                modPlayer.reforgeTierSafety = 0;
                modPlayer.finalTierAccessoryReforge = false;
            }

            modPlayer.itemTypeLastReforged = item.type;

            if (modPlayer.reforgeTierSafety > 6)
            {
                modPlayer.reforgeTierSafety = 0;
                modPlayer.finalTierAccessoryReforge = true;
            }

            modPlayer.reforgeTierSafety++;
            reforgeTier = modPlayer.reforgeTierSafety;

            bool favorited = item.favorited;
            item.netDefaults(item.netID);
            item.Prefix(NewPrefixType(item));

            item.Center = Main.player[Main.myPlayer].Center;
            item.favorited = favorited;

            if (NPC.AnyNPCs(ModContent.NPCType<THIEF>()))
            {
                int value = item.value;
                ItemLoader.ReforgePrice(item, ref value, ref Main.LocalPlayer.discount);
                CalamityWorld.MoneyStolenByBandit += value / 5;
                CalamityWorld.Reforges++;
            }
        }
        #endregion

        #region Money From Rarity
        public static readonly int Rarity0BuyPrice = Item.buyPrice(0, 0, 50, 0);
        public static readonly int Rarity1BuyPrice = Item.buyPrice(0, 1, 0, 0);
        public static readonly int Rarity2BuyPrice = Item.buyPrice(0, 2, 0, 0);
        public static readonly int Rarity3BuyPrice = Item.buyPrice(0, 4, 0, 0);
        public static readonly int Rarity4BuyPrice = Item.buyPrice(0, 12, 0, 0);
        public static readonly int Rarity5BuyPrice = Item.buyPrice(0, 24, 0, 0);
        public static readonly int Rarity6BuyPrice = Item.buyPrice(0, 36, 0, 0);
        public static readonly int Rarity7BuyPrice = Item.buyPrice(0, 48, 0, 0);
        public static readonly int Rarity8BuyPrice = Item.buyPrice(0, 60, 0, 0);
        public static readonly int Rarity9BuyPrice = Item.buyPrice(0, 80, 0, 0);
        public static readonly int Rarity10BuyPrice = Item.buyPrice(1, 0, 0, 0);
        public static readonly int Rarity11BuyPrice = Item.buyPrice(1, 10, 0, 0);
        public static readonly int RarityTurquoiseBuyPrice = Item.buyPrice(1, 20, 0, 0);
        public static readonly int RarityPureGreenBuyPrice = Item.buyPrice(1, 30, 0, 0);
        public static readonly int RarityDarkBlueBuyPrice = Item.buyPrice(1, 40, 0, 0);
        public static readonly int RarityVioletBuyPrice = Item.buyPrice(1, 50, 0, 0);
        public static readonly int RarityHotPinkBuyPrice = Item.buyPrice(2, 0, 0, 0);

        //These duplicates are for my sanity...
        public static readonly int Rarity12BuyPrice = Item.buyPrice(1, 20, 0, 0);
        public static readonly int Rarity13BuyPrice = Item.buyPrice(1, 30, 0, 0);
        public static readonly int Rarity14BuyPrice = Item.buyPrice(1, 40, 0, 0);
        public static readonly int Rarity15BuyPrice = Item.buyPrice(1, 50, 0, 0);
        public static readonly int Rarity16BuyPrice = Item.buyPrice(2, 0, 0, 0);

        public static int GetBuyPrice(int rarity)
        {
            switch (rarity)
            {
                case 0:
                    return Rarity0BuyPrice;
                case 1:
                    return Rarity1BuyPrice;
                case 2:
                    return Rarity2BuyPrice;
                case 3:
                    return Rarity3BuyPrice;
                case 4:
                    return Rarity4BuyPrice;
                case 5:
                    return Rarity5BuyPrice;
                case 6:
                    return Rarity6BuyPrice;
                case 7:
                    return Rarity7BuyPrice;
                case 8:
                    return Rarity8BuyPrice;
                case 9:
                    return Rarity9BuyPrice;
                case 10:
                    return Rarity10BuyPrice;
                case 11:
                    return Rarity11BuyPrice;
                case (int)CalamityRarity.Turquoise:
                    return RarityTurquoiseBuyPrice;
                case (int)CalamityRarity.PureGreen:
                    return RarityPureGreenBuyPrice;
                case (int)CalamityRarity.DarkBlue:
                    return RarityDarkBlueBuyPrice;
                case (int)CalamityRarity.Violet:
                    return RarityVioletBuyPrice;
                case (int)CalamityRarity.HotPink:
                    return RarityHotPinkBuyPrice;
            }
            return 0;
        }
        public static int GetBuyPrice(Item item)
        {
            return GetBuyPrice(item.rare);
        }
        #endregion
    }
}
