using CalamityMod.Buffs.Potions;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.DifficultyItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Placeables.Furniture.Fountains;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Tools;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.UI;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityMod.Items
{
	public class CalamityGlobalItem : GlobalItem
    {
        #region Instances
        public override bool InstancePerEntity
        {
            get
            {
                return true;
            }
        }

        public override bool CloneNewInstances
        {
            get
            {
                return true;
            }
        }
        #endregion

		public bool rogue = false;
		public float StealthGenBonus;

		public int timesUsed = 0;

        // The damage modifications for the item are handled in player files.
        public int CurrentCharge;
        public bool Chargeable = false;
        public int ChargeMax;
        public const float ChargeDamageMinMultiplier = 0.75f;
        public const float ChargeDamageReductionThreshold = 0.75f;

        // Rarity is provided both as the classic int and the new enum.
        public CalamityRarity customRarity = CalamityRarity.NoEffect;
		public int postMoonLordRarity 
		{
			get => (int)customRarity;
			set => customRarity = (CalamityRarity)value;
		}

		///See RogueWeapon.cs for rogue modifier shit
		#region Modifiers
		public CalamityGlobalItem()
		{
			StealthGenBonus = 1f;
		}

		public override GlobalItem Clone(Item item, Item itemClone)
		{
			CalamityGlobalItem myClone = (CalamityGlobalItem)base.Clone(item, itemClone);
			myClone.StealthGenBonus = StealthGenBonus;
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
            item.Calamity().ChargeMax = ChargeMax;
            if (customRarity.IsPostML() && item.rare != 10)
                item.rare = 10;

            if (item.maxStack == 99 || item.type == ItemID.Dynamite || item.type == ItemID.StickyDynamite ||
                item.type == ItemID.BouncyDynamite || item.type == ItemID.StickyBomb || item.type == ItemID.BouncyBomb)
                item.maxStack = 999;

            if (item.type == ItemID.PirateMap || item.type == ItemID.SnowGlobe)
                item.maxStack = 20;

            if (item.type >= ItemID.GreenSolution && item.type <= ItemID.RedSolution)
                item.value = Item.buyPrice(0, 0, 5, 0);

            if (CalamityLists.weaponAutoreuseList?.Contains(item.type) ?? false)
                item.autoReuse = true;

            if (item.type == ItemID.PsychoKnife || item.type == ItemID.TaxCollectorsStickOfDoom)
                item.damage *= 4;
            else if (item.type == ItemID.SpectreStaff)
                item.damage *= 3;
            else if (CalamityLists.doubleDamageBuffList?.Contains(item.type) ?? false)
                item.damage *= 2;
            else if (item.type == ItemID.RainbowRod)
                item.damage = (int)(item.damage * 1.75);
            else if (CalamityLists.sixtySixDamageBuffList?.Contains(item.type) ?? false)
                item.damage = (int)(item.damage * 1.66);
            else if (CalamityLists.fiftyDamageBuffList?.Contains(item.type) ?? false)
                item.damage = (int)(item.damage * 1.5);
            else if (CalamityLists.thirtyThreeDamageBuffList?.Contains(item.type) ?? false)
                item.damage = (int)(item.damage * 1.33);
            else if (CalamityLists.twentyFiveDamageBuffList?.Contains(item.type) ?? false)
                item.damage = (int)(item.damage * 1.25);
            else if (CalamityLists.twentyDamageBuffList?.Contains(item.type) ?? false)
                item.damage = (int)(item.damage * 1.2);
            else if (CalamityLists.tenDamageBuffList?.Contains(item.type) ?? false)
                item.damage = (int)(item.damage * 1.1);
            else if (CalamityLists.tenDamageNerfList?.Contains(item.type) ?? false)
                item.damage = (int)(item.damage * 0.9);
            else if (CalamityLists.quarterDamageNerfList?.Contains(item.type) ?? false)
                item.damage = (int)(item.damage * 0.75);
            else if (item.type == ItemID.BlizzardStaff)
                item.damage = (int)(item.damage * 0.7);
            else if (item.type == ItemID.LaserMachinegun)
                item.damage = (int)(item.damage * 0.65);
            else if (item.type == ItemID.StardustDragonStaff)
                item.damage = (int)(item.damage * 0.5);

            if (item.type == ItemID.BookStaff)
                item.mana = 10;
            else if (item.type == ItemID.UnholyTrident)
                item.mana = 14;
            else if (item.type == ItemID.FrostStaff)
                item.mana = 9;
            else if (item.type == ItemID.BookofSkulls)
                item.mana = 12;
            else if (item.type == ItemID.BlizzardStaff)
                item.mana = 7;
            else if (item.type == ItemID.SolarFlareHelmet) //total defense pre-buff = 78 post-buff = 94
                item.defense = 29; //5 more defense
            else if (item.type == ItemID.SolarFlareBreastplate)
                item.defense = 41; //7 more defense
            else if (item.type == ItemID.SolarFlareLeggings)
                item.defense = 24; //4 more defense
            else if (item.type == ItemID.GladiatorHelmet) //total defense pre-buff = 7 post-buff = 15
                item.defense = 3; //1 more defense
            else if (item.type == ItemID.GladiatorBreastplate)
                item.defense = 5; //2 more defense
            else if (item.type == ItemID.GladiatorLeggings)
                item.defense = 4; //2 more defense
            else if (item.type == ItemID.HallowedPlateMail) //total defense pre-buff = 31, 50, 35 post-buff = 36, 55, 40
                item.defense = 18; //3 more defense
            else if (item.type == ItemID.HallowedGreaves)
                item.defense = 13; //2 more defense

			if (CalamityLists.noGravityList.Contains(item.type))
				ItemID.Sets.ItemNoGravity[item.type] = true;
			if (CalamityLists.lavaFishList.Contains(item.type))
				ItemID.Sets.CanFishInLava[item.type] = true;

			// not expert because ML drops it in normal so that it can be used with the lore item
            if (item.type == ItemID.GravityGlobe)
			{
				item.expert = false;
				item.rare = 10;
			}
            
            if (item.type == ItemID.SuspiciousLookingTentacle)
                item.expert = true;

            if (item.type == ItemID.PearlwoodHammer)
			{
                item.hammer += 35; //80% hammer power
				item.useAnimation = 20;
				item.useTime = 15;
				item.damage *= 4;
				item.tileBoost += 1;
				item.rare = 4;
			}
            if (item.type == ItemID.PearlwoodBow)
			{
				item.useAnimation += 8; //35
				item.useTime += 8; //35
				item.shootSpeed += 3.4f; //10f
				item.knockBack += 1f; //1f
				item.rare = 4;
				item.damage = (int)(item.damage * 2.1);
			}
            if (item.type == ItemID.PearlwoodSword)
			{
				item.damage *= 4;
				item.rare = 4;
			}
			if (item.type == ItemID.StarCannon)
				item.UseSound = null;
        }
        #endregion

        #region Shoot
        public override bool Shoot(Item item, Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (item.type >= ItemID.Count && item.Calamity().Chargeable && item.Calamity().CurrentCharge > 0 && Main.rand.NextBool(120 / (int)MathHelper.Max(1, item.useAnimation)))
            {
                CurrentCharge--;
            }
			CalamityPlayer modPlayer = player.Calamity();
            if (rogue)
            {
                speedX *= modPlayer.throwingVelocity;
                speedY *= modPlayer.throwingVelocity;
                if (modPlayer.gloveOfRecklessness)
                {
                    Vector2 rotated = new Vector2(speedX, speedY);
                    rotated = rotated.RotatedByRandom(MathHelper.ToRadians(10f));
                    speedX = rotated.X;
                    speedY = rotated.Y;
                }
            }
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
                        Projectile.NewProjectile(position, new Vector2(speedX, speedY) * 0.5f, ModContent.ProjectileType<LuxorsGiftMelee>(), CalamityUtils.DamageSoftCap(newDamage * 0.6, 60), 0f, player.whoAmI, 0f, 0f);

                    else if (rogue)
                        Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<LuxorsGiftRogue>(), CalamityUtils.DamageSoftCap(newDamage * 0.5, 50), 0f, player.whoAmI, 0f, 0f);

                    else if (item.ranged)
                        Projectile.NewProjectile(position, new Vector2(speedX, speedY) * 1.5f, ModContent.ProjectileType<LuxorsGiftRanged>(), CalamityUtils.DamageSoftCap(newDamage * 0.4, 40), 0f, player.whoAmI, 0f, 0f);

                    else if (item.magic)
                        Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<LuxorsGiftMagic>(), CalamityUtils.DamageSoftCap(newDamage * 0.8, 80), 0f, player.whoAmI, 0f, 0f);

                    else if (item.summon && player.ownedProjectileCounts[ModContent.ProjectileType<LuxorsGiftSummon>()] < 1)
                        Projectile.NewProjectile(position, Vector2.Zero, ModContent.ProjectileType<LuxorsGiftSummon>(), damage, 0f, player.whoAmI, 0f, 0f);
                }
            }
            if (modPlayer.eArtifact && item.ranged)
            {
                speedX *= 1.25f;
                speedY *= 1.25f;
            }
            if (modPlayer.bloodflareMage && modPlayer.canFireBloodflareMageProjectile)
            {
                if (item.magic)
                {
					modPlayer.canFireBloodflareMageProjectile = false;
					if (player.whoAmI == Main.myPlayer)
                    {
                        Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<GhostlyBolt>(), CalamityUtils.DamageSoftCap(damage * 1.3, 250), 1f, player.whoAmI, 0f, 0f);
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
                        Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<BloodBomb>(), CalamityUtils.DamageSoftCap(damage * 0.8, 150), 2f, player.whoAmI, 0f, 0f);
                    }
                }
            }
            if (modPlayer.tarraMage)
            {
                if (modPlayer.tarraCrits >= 5 && player.whoAmI == Main.myPlayer)
                {
                    modPlayer.tarraCrits = 0;
                    int num106 = 9 + Main.rand.Next(3);
                    for (int num107 = 0; num107 < num106; num107++)
                    {
                        float num110 = 0.025f * num107;
                        float hardar = speedX + Main.rand.Next(-25, 26) * num110;
                        float hordor = speedY + Main.rand.Next(-25, 26) * num110;
                        float num84 = (float)Math.Sqrt(speedX * speedX + speedY * speedY);
                        num84 = item.shootSpeed / num84;
                        hardar *= num84;
                        hordor *= num84;
                        Projectile.NewProjectile(position, new Vector2(hardar, hordor), ProjectileID.Leaf, CalamityUtils.DamageSoftCap(damage * 0.2, 50), knockBack, player.whoAmI, 0f, 0f);
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
                        Projectile.NewProjectile(position, new Vector2(speedX, speedY) * 1.25f, ModContent.ProjectileType<ChaosFlare>(), CalamityUtils.DamageSoftCap(damage * 0.25, 50), 2f, player.whoAmI, 0f, 0f);
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
                        Projectile.NewProjectile(position, new Vector2(speedX, speedY) * 1.25f, ModContent.ProjectileType<GodSlayerShrapnelRound>(), CalamityUtils.DamageSoftCap(damage, 200), 2f, player.whoAmI, 0f, 0f);
                    }
                }
            }
            if (modPlayer.ataxiaVolley && modPlayer.canFireAtaxiaRogueProjectile)
            {
                if (rogue)
                {
					modPlayer.canFireAtaxiaRogueProjectile = false;
					if (player.whoAmI == Main.myPlayer)
                    {
                        Main.PlaySound(SoundID.Item20, (int)player.position.X, (int)player.position.Y);
                        float spread = 45f * 0.0174f;
                        double startAngle = Math.Atan2(player.velocity.X, player.velocity.Y) - spread / 2;
                        double deltaAngle = spread / 8f;
                        double offsetAngle;
                        int i;
                        for (i = 0; i < 4; i++)
                        {
                            Vector2 vector2 = new Vector2(player.Center.X, player.Center.Y);
                            offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                            Projectile.NewProjectile(vector2.X, vector2.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<ChaosFlare2>(), CalamityUtils.DamageSoftCap(damage * 0.5, 100), 1f, player.whoAmI, 0f, 0f);
                            Projectile.NewProjectile(vector2.X, vector2.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<ChaosFlare2>(), CalamityUtils.DamageSoftCap(damage * 0.5, 100), 1f, player.whoAmI, 0f, 0f);
                        }
                    }
                }
            }
            if (modPlayer.reaverDoubleTap && modPlayer.canFireReaverRangedProjectile)
            {
                if (item.ranged)
                {
					modPlayer.canFireReaverRangedProjectile = false;
					if (player.whoAmI == Main.myPlayer)
                    {
                        Projectile.NewProjectile(position, new Vector2(speedX, speedY) * 1.25f, ModContent.ProjectileType<MiniRocket>(), CalamityUtils.DamageSoftCap(damage * 1.3, 150), 2f, player.whoAmI, 0f, 0f);
                    }
                }
            }
            if (modPlayer.victideSet)
            {
                if ((item.ranged || item.melee || item.magic || item.thrown || rogue || item.summon) && item.rare < ItemRarityID.Yellow && Main.rand.NextBool(10))
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        Projectile.NewProjectile(position, new Vector2(speedX, speedY) * 1.25f, ModContent.ProjectileType<Seashell>(), CalamityUtils.DamageSoftCap(damage * 2, 60), 1f, player.whoAmI, 0f, 0f);
                    }
                }
            }
            if (modPlayer.dynamoStemCells)
            {
                if (item.ranged && Main.rand.Next(0, 100) >= 80)
                {
					double damageMult = item.useTime / 30D;
					if (damageMult < 0.35)
						damageMult = 0.35;

					int newDamage = (int)(damage * 2 * damageMult);

                    if (player.whoAmI == Main.myPlayer)
                    {
                        Projectile.NewProjectile(position, new Vector2(speedX, speedY) * 1.25f, ModContent.ProjectileType<Minibirb>(), newDamage, 2f, player.whoAmI, 0f, 0f);
                    }
                }
            }
            if (modPlayer.prismaticRegalia)
            {
                if (item.magic && Main.rand.Next(0, 100) >= 95)
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
						for (int i = -5; i <= 5; i += 5)
						{
							if (i != 0)
							{
								Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(i));
								int rocket = Projectile.NewProjectile(position, perturbedSpeed, ModContent.ProjectileType<MiniRocket>(), (int)(damage * 0.8), 2f, player.whoAmI, 0f, 0f);
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
						float spreadX = speedX + Main.rand.Next(-15, 16) * 0.05f;
						float spreadY = speedY + Main.rand.Next(-15, 16) * 0.05f;
                        int feather = Projectile.NewProjectile(position, new Vector2(spreadX, spreadY) * 1.25f, ModContent.ProjectileType<TradewindsProjectile>(), damage / 2, 2f, player.whoAmI, 0f, 0f);
						Main.projectile[feather].usesLocalNPCImmunity = true;
						Main.projectile[feather].localNPCHitCooldown = 10;
						Main.projectile[feather].Calamity().forceTypeless = true;
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
				Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<FallenStarProj>(), damage, knockBack, player.whoAmI);
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
					Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(i));
					Projectile.NewProjectile(position, perturbedSpeed, ModContent.ProjectileType<RainbowFront>(), damage, 0f, player.whoAmI, 0f, 0f);
				}
			}
            return true;
        }
        #endregion

        #region SavingAndLoading
        public override bool NeedsSaving(Item item)
        {
            return true;
        }

        public override TagCompound Save(Item item)
        {
            return new TagCompound
            {
                ["rogue"] = rogue,
                ["timesUsed"] = timesUsed,
                ["rarity"] = (int)customRarity,
                ["Charge"] = CurrentCharge
            };
        }

        public override void Load(Item item, TagCompound tag)
        {
            rogue = tag.GetBool("rogue");
            timesUsed = tag.GetInt("timesUsed");
            customRarity = (CalamityRarity)tag.GetInt("rarity");
            CurrentCharge = tag.GetInt("Charge");
        }

        public override void LoadLegacy(Item item, BinaryReader reader)
        {
            int loadVersion = reader.ReadInt32();
            customRarity = (CalamityRarity)reader.ReadInt32();
            timesUsed = reader.ReadInt32();
            CurrentCharge = reader.ReadInt32();

            if (loadVersion == 0)
            {
                BitsByte flags = reader.ReadByte();
                rogue = flags[0];
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

            writer.Write(flags);
            writer.Write((int)customRarity);
            writer.Write(timesUsed);
            writer.Write(CurrentCharge);
        }

        public override void NetReceive(Item item, BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            rogue = flags[0];

            customRarity = (CalamityRarity)reader.ReadInt32();
            timesUsed = reader.ReadInt32();
            CurrentCharge = reader.ReadInt32();
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
                    {
                        player.HealEffect(boostedHeart ? -5 : -10, true);
                    }
                }
                else if (boostedHeart)
                {
                    player.statLife += 5;
                    if (Main.myPlayer == player.whoAmI)
                    {
                        player.HealEffect(5, true);
                    }
                }
            }
            return true;
        }
        #endregion

        #region Profaned Soul Crystal Attacks

        private bool HandleAttackTransforms(Item item, Player player)
        {
            if (player.whoAmI != Main.myPlayer)
                return false;
            int weaponType = item.melee ? 1 : item.ranged ? 2 : item.magic ? 3 : item.Calamity().rogue ? 4 : -1;
            if (weaponType > 0)
            {
                if (player.Calamity().profanedSoulWeaponType != weaponType || player.Calamity().profanedSoulWeaponUsage >= 300)
                {
                    player.Calamity().profanedSoulWeaponType = weaponType;
                    player.Calamity().profanedSoulWeaponUsage = 0;
                }
                Vector2 correctedVelocity = Main.MouseWorld - player.Center;
                correctedVelocity.Normalize();
                bool shouldNerf = player.Calamity().endoCooper || player.Calamity().magicHat; //No bonkers damage memes thank you very much.
                bool enrage = player.statLife <= (int)(player.statLifeMax2 * 0.5);
                if (item.melee)
                {
                    if (player.Calamity().profanedSoulWeaponUsage % (enrage ? 4 : 6) == 0)
                    {
                        if (player.Calamity().profanedSoulWeaponUsage > 0 && player.Calamity().profanedSoulWeaponUsage % (enrage ? 20 : 30) == 0) //every 5 shots is a shotgun spread
                        {
                            int numProj = 5;

                            correctedVelocity *= 12f;
                            int spread = 3;
                            for (int i = 0; i < numProj; i++)
                            {
                                Vector2 perturbedspeed = new Vector2(correctedVelocity.X, correctedVelocity.Y + Main.rand.Next(-3, 4)).RotatedBy(MathHelper.ToRadians(spread));

                                Projectile.NewProjectile(player.Center.X, player.Center.Y - 10, perturbedspeed.X, perturbedspeed.Y, ModContent.ProjectileType<ProfanedCrystalMeleeSpear>(), (int)((shouldNerf ? 1000 : 1750) * player.MinionDamage()), 1f, player.whoAmI, Main.rand.NextBool(player.Calamity().profanedSoulWeaponUsage == 4 ? 5 : 7) ? 1f : 0f);
                                spread -= Main.rand.Next(2, 4);
                                Main.PlaySound(SoundID.Item20, player.Center);
                            }
                            player.Calamity().profanedSoulWeaponUsage = 0;
                        }
                        else
                        {
                            Projectile.NewProjectile(player.Center, correctedVelocity * 6.9f, ModContent.ProjectileType<ProfanedCrystalMeleeSpear>(), (int)((shouldNerf ? 500 : 1250) * player.MinionDamage()), 1f, player.whoAmI, Main.rand.NextBool(player.Calamity().profanedSoulWeaponUsage == 4 ? 5 : 7) ? 1f : 0f, 1f);
                            Main.PlaySound(SoundID.Item20, player.Center);
                        }

                    }
                    player.Calamity().profanedSoulWeaponUsage++;

                }
                else if (item.ranged)
                {
                    if (enrage || Main.rand.NextBool(2)) //100% chance if 50% or lower, else 1 in 2 chance
                    {
                        correctedVelocity *= 20f;
                        Vector2 perturbedspeed = new Vector2(correctedVelocity.X + Main.rand.Next(-3, 4), correctedVelocity.Y + Main.rand.Next(-3, 4)).RotatedBy(MathHelper.ToRadians(3));
                        bool isSmallBoomer = Main.rand.NextDouble() <= (enrage ? 0.2 : 0.3); // 20% chance if enraged, else 30% This is intentional due to literally doubling the amount of projectiles fired.
                        bool isThiccBoomer = isSmallBoomer && Main.rand.NextDouble() <= 0.05; // 5%
                        int projType = isSmallBoomer ? isThiccBoomer ? 1 : 2 : 3;
                        int dam = (int)((shouldNerf ? 500 : 1000) * player.MinionDamage());
                        switch (projType)
                        {
                            case 1: //big boomer
                            case 2: //boomer
                                int proj = Projectile.NewProjectile(player.Center, perturbedspeed, ModContent.ProjectileType<ProfanedCrystalRangedHuges>(), dam, 0f, player.whoAmI, projType == 1 ? 1f : 0f);
                                Main.projectile[proj].Calamity().forceMinion = true;
                                break;
                            case 3: //bab boomer
                                int proj2 = Projectile.NewProjectile(player.Center, perturbedspeed, ModContent.ProjectileType<ProfanedCrystalRangedSmalls>(), dam, 0f, player.whoAmI, 0f);
                                Main.projectile[proj2].Calamity().forceMinion = true;
                                break;
                        }
                        if (projType > 1)
                        {
                            Main.PlaySound(SoundID.Item20, player.Center);
                        }
                    }
                }
                else if (item.magic)
                {
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<ProfanedCrystalMageFireball>()] == 0 && player.ownedProjectileCounts[ModContent.ProjectileType<ProfanedCrystalMageFireballSplit>()] == 0)
                    {
                        player.Calamity().profanedSoulWeaponUsage = 0;
                    }
                    int manaCost = (int)(100 * player.manaCost);
                    if (player.statMana < manaCost && player.Calamity().profanedSoulWeaponUsage == 0)
                    {
                        if (player.manaFlower)
                        {
                            player.QuickMana();
                        }
                    }
                    if (player.statMana >= manaCost && player.Calamity().profanedSoulWeaponUsage == 0 && !player.silence)
                    {
                        player.manaRegenDelay = (int)player.maxRegenDelay;
                        player.statMana -= manaCost;
                        correctedVelocity *= 25f;
                        Main.PlaySound(SoundID.Item20, player.Center);
                        int dam = (int)((shouldNerf ? 1800 : 4500) * player.MinionDamage());
                        if (player.HasBuff(BuffID.ManaSickness))
                        {
                            int sickPenalty = (int)(dam * (0.05f * ((player.buffTime[player.FindBuffIndex(BuffID.ManaSickness)] + 60) / 60)));
                            dam -= sickPenalty;
                        }
                        int proj = Projectile.NewProjectile(player.position, correctedVelocity, ModContent.ProjectileType<ProfanedCrystalMageFireball>(), dam, 1f, player.whoAmI, enrage ? 1f : 0f);
                        Main.projectile[proj].Calamity().forceMinion = true;
                        player.Calamity().profanedSoulWeaponUsage = enrage ? 20 : 25;
                    }
                    if (player.Calamity().profanedSoulWeaponUsage > 0)
                        player.Calamity().profanedSoulWeaponUsage--;
                }
                else if (item.Calamity().rogue)
                {
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<ProfanedCrystalRogueShard>()] == 0)
                    {
                        player.Calamity().profanedSoulWeaponUsage = 0;
                    }
                    if (player.Calamity().profanedSoulWeaponUsage >= (enrage ? 69 : 180))
                    {
                        float crystalCount = 36f;
                        for (float i = 0; i < crystalCount; i++)
                        {
                            float angle = MathHelper.TwoPi / crystalCount * i;
                            int proj = Projectile.NewProjectile(player.Center, angle.ToRotationVector2() * 8f, ModContent.ProjectileType<ProfanedCrystalRogueShard>(), (int)((shouldNerf ? 300 : 880) * player.MinionDamage()), 1f, player.whoAmI, 0f, 0f);
                            Main.projectile[proj].Calamity().forceMinion = true;
                            Main.PlaySound(SoundID.Item20, player.Center);
                        }
                        player.Calamity().profanedSoulWeaponUsage = 0;
                    }
                    else if (player.Calamity().profanedSoulWeaponUsage % (enrage ? 5 : 10) == 0)
                    {
                        float angle = MathHelper.TwoPi / (enrage ? 9 : 18) * (player.Calamity().profanedSoulWeaponUsage / (enrage ? 1 : 10));
                        int proj = Projectile.NewProjectile(player.Center, angle.ToRotationVector2() * 8f, ModContent.ProjectileType<ProfanedCrystalRogueShard>(), (int)((shouldNerf ? 400 : 1100) * player.MinionDamage()), 1f, player.whoAmI, 1f, 0f);
                        Main.projectile[proj].Calamity().forceMinion = true;
                        Main.PlaySound(SoundID.Item20, player.Center);
                    }
                    player.Calamity().profanedSoulWeaponUsage += enrage ? 1 : 2;
                    if (!enrage && player.Calamity().profanedSoulWeaponUsage % 2 != 0)
                        player.Calamity().profanedSoulWeaponUsage--;
                }
            }
            return false;
        }

        #endregion

        #region Andromeda Dev Item Attacks
        private bool PerformAndromedaAttacks(Item item, Player player)
        {
            if (player.whoAmI != Main.myPlayer)
                return false;

            int robotIndex = -1;

            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].active &&
                    Main.projectile[i].type == ModContent.ProjectileType<GiantIbanRobotOfDoom>() &&
                    Main.projectile[i].owner == player.whoAmI)
                {
                    robotIndex = i;
                    break;
                }
            }
            if (robotIndex != -1)
            {
                Projectile robot = Main.projectile[robotIndex];
                GiantIbanRobotOfDoom robotModProjectile = ((GiantIbanRobotOfDoom)robot.modProjectile);
                if (player.ownedProjectileCounts[ModContent.ProjectileType<AndromedaRegislash>()] <= 0 &&
                    robotModProjectile.TopIconActive &&
                    (robotModProjectile.RightIconCooldown <= GiantIbanRobotOfDoom.RightIconAttackTime ||
                     !robotModProjectile.RightIconActive)) // "Melee" attack
                {
                    int damage = player.Calamity().andromedaState == AndromedaPlayerState.SmallRobot ? GiantIbanRobotOfDoom.RegicideBaseDamageSmall : GiantIbanRobotOfDoom.RegicideBaseDamageLarge;
                    if (item.Calamity().rogue)
                    {
                        damage = (int)(damage * player.RogueDamage());
                    }
                    if (item.melee)
                    {
                        damage = (int)(damage * player.MeleeDamage());
                    }
                    if (item.ranged)
                    {
                        damage = (int)(damage * player.RangedDamage());
                    }
                    if (item.magic)
                    {
                        damage = (int)(damage * player.MagicDamage());
                    }
                    if (item.summon)
                    {
                        damage = (int)(damage * player.MinionDamage());
                    }
                    Projectile blade = Projectile.NewProjectileDirect(robot.Center + (robot.spriteDirection > 0).ToDirectionInt() * robot.width / 2 * Vector2.UnitX, 
                               Vector2.Zero, ModContent.ProjectileType<AndromedaRegislash>(), damage, 15f, player.whoAmI, Projectile.GetByUUID(robot.owner, robot.whoAmI));

                    if (item.Calamity().rogue)
                    {
                        blade.Calamity().forceRogue = true;
                    }
                    if (item.melee)
                    {
                        blade.Calamity().forceMelee = true;
                    }
                    if (item.ranged)
                    {
                        blade.Calamity().forceRanged = true;
                    }
                    if (item.magic)
                    {
                        blade.Calamity().forceMagic = true;
                    }
                    if (item.summon)
                    {
                        blade.Calamity().forceMinion = true;
                    }
                }

                if (!robotModProjectile.TopIconActive &&
                    (robotModProjectile.LeftBracketActive || robotModProjectile.RightBracketActive) &&
                    !robotModProjectile.BottomBracketActive &&
                    robotModProjectile.LaserCooldown <= 0 &&
                    (robotModProjectile.RightIconCooldown <= GiantIbanRobotOfDoom.RightIconAttackTime ||
                     !robotModProjectile.RightIconActive)) // "Ranged" attack
                {
                    robotModProjectile.LaserCooldown = AndromedaDeathRay.TrueTimeLeft * 2;
                    if (player.Calamity().andromedaState == AndromedaPlayerState.SmallRobot)
                    {
                        robotModProjectile.LaserCooldown = AndromedaDeathRay.TrueTimeLeft + 1;
                    }
                }
            }
            return false;
        }
        #endregion

        #region Use Item Changes

        public override bool UseItem(Item item, Player player)
        {
            // Use charge on swing instead of on shoot if the item doesn't shoot anything.
            if (item.type >= ItemID.Count && item.Calamity().Chargeable && item.Calamity().CurrentCharge > 0 && Main.rand.NextBool(120 / (int)MathHelper.Max(1, item.useAnimation)) && item.shoot == ProjectileID.None)
            {
                if (player.itemAnimation == 1)
                    CurrentCharge--;
            }
			if (item.type == ItemID.BottledHoney)
			{
				player.AddBuff(BuffID.Honey, 7200);
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
            if (player.ActiveItem().type == ModContent.ItemType<IgneousExaltation>())
            {
                bool hasBlades = false;
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<IgneousBlade>() && Main.projectile[i].owner == player.whoAmI && Main.projectile[i].localAI[1] == 0f)
                    {
                        hasBlades = true;
                        break;
                    }
                }
                if (hasBlades)
                {
                    for (int i = 0; i < Main.projectile.Length; i++)
                    {
                        if (Main.projectile[i].modProjectile is IgneousBlade)
                        {
                            if ((Main.projectile[i].modProjectile as IgneousBlade).Firing)
                                continue;
                        }
                        if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<IgneousBlade>() && Main.projectile[i].owner == player.whoAmI && Main.projectile[i].localAI[1] == 0f)
                        {
                            Main.projectile[i].rotation = MathHelper.PiOver2 + MathHelper.PiOver4;
                            Main.projectile[i].velocity = Main.projectile[i].DirectionTo(Main.MouseWorld) * 22f;
                            Main.projectile[i].rotation += Main.projectile[i].velocity.ToRotation();
                            Main.projectile[i].ai[0] = 180f;
                            (Main.projectile[i].modProjectile as IgneousBlade).Firing = true;
                            Main.projectile[i].tileCollide = true;
                            Main.projectile[i].netUpdate = true;
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

            // Restrict behavior when reading Dreadon's Log.
            if (PopupGUIManager.AnyGUIsActive)
                return false;

            if (player.ownedProjectileCounts[ModContent.ProjectileType<RelicOfDeliveranceSpear>()] > 0 &&
                (item.damage > 0 || item.ammo != AmmoID.None))
            {
                return false; // Don't use weapons if you're charging with a spear
            }

            if (player.ownedProjectileCounts[ModContent.ProjectileType<GiantIbanRobotOfDoom>()] > 0)
            {
				if (item.type == ItemID.WireKite)
					return false;
                if (item.pick > 0 || item.axe > 0 || item.hammer > 0 || item.fishingPole > 0)
                    return false;
                if (item.Calamity().rogue || item.magic || item.ranged || item.melee)
                {
                    if (player.altFunctionUse == 0)
                        return PerformAndromedaAttacks(item, player);
                    else return false;
                }
            }

            if (modPlayer.profanedCrystalBuffs && item.pick == 0 && item.axe == 0 && item.hammer == 0 && item.autoReuse && (item.Calamity().rogue || item.magic || item.ranged || item.melee))
            {   
                if (player.altFunctionUse == 0)
                {
                    return HandleAttackTransforms(item, player);
                }
                else
                {
                    return AltFunctionUse(item, player);
                }
            }
            else if (modPlayer.profanedCrystalBuffs && item.summon)
            {
                
            }
            if (item.type == ItemID.MonkStaffT1)
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
            if ((item.type == ItemID.ShinePotion || item.type == ItemID.NightOwlPotion) && (modPlayer.etherealExtorter && modPlayer.ZoneAbyss))
            {
                return false;
            }
            if ((item.type == ItemID.SuperAbsorbantSponge || item.type == ItemID.EmptyBucket) && modPlayer.ZoneAbyss)
            {
                return false;
            }
            if (item.type == ItemID.MagicMirror || item.type == ItemID.IceMirror || item.type == ItemID.CellPhone || item.type == ItemID.RecallPotion)
            {
                return !NPC.AnyNPCs(ModContent.NPCType<SupremeCalamitas>());
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
						if (modPlayer.eScarfCooldown)
							duration = (int)(duration * 1.5);
						else if (modPlayer.scarfCooldown)
							duration *= 2;
						player.AddBuff(BuffID.ChaosState, duration, true);
					}
				}
			}
            return true;
        }
        #endregion

        #region Modify Weapon Damage
        public override void ModifyWeaponDamage(Item item, Player player, ref float add, ref float mult, ref float flat)
        {
            if (item.Calamity().Chargeable)
            {
                float chargeDamageMultiplier = 1f;
                float chargeRatio = item.Calamity().CurrentCharge / (float)ChargeMax;
                if (chargeRatio < ChargeDamageReductionThreshold)
                    chargeDamageMultiplier = MathHelper.Lerp(ChargeDamageMinMultiplier, 1f, chargeRatio * ChargeDamageReductionThreshold);
                mult *= chargeDamageMultiplier;
            }
        }
        #endregion

        #region Modify Tooltips
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
			#region Custom Rarities#
            TooltipLine tt2 = tooltips.FirstOrDefault(x => x.Name == "ItemName" && x.mod == "Terraria");
            if (tt2 != null)
            {
                // The special color in the tooltip were overlapping. Placing everything in here, above other things, was the only solution I could find that worked.
                if (item.type == ModContent.ItemType<Eternity>())
                {
                    List<Color> colorSet = new List<Color>()
                    {
                        new Color(188, 192, 193), // white
                        new Color(157, 100, 183), // purple
                        new Color(249, 166, 77), // honey-ish orange
                        new Color(255, 105, 234), // pink
                        new Color(67, 204, 219), // sky blue
                        new Color(249, 245, 99), // bright yellow
                        new Color(236, 168, 247), // purplish pink
                    };
                    if (tt2 != null)
                    {
                        int colorIndex = (int)(Main.GlobalTime / 2 % colorSet.Count);
                        Color currentColor = colorSet[colorIndex];
                        Color nextColor = colorSet[(colorIndex + 1) % colorSet.Count];
                        tt2.overrideColor = Color.Lerp(currentColor, nextColor, Main.GlobalTime % 2f > 1f ? 1f : Main.GlobalTime % 1f);
                    }
                    return;
                }
                switch (customRarity)
                {
                    default:
                        break;
                    case CalamityRarity.Turquoise:
                        tt2.overrideColor = new Color(0, 255, 200);
                        break;
                    case CalamityRarity.PureGreen:
                        tt2.overrideColor = new Color(0, 255, 0);
                        break;
                    case CalamityRarity.DarkBlue:
                        tt2.overrideColor = new Color(43, 96, 222);
                        break;
                    case CalamityRarity.Violet:
                        tt2.overrideColor = new Color(108, 45, 199);
                        break;
                    case CalamityRarity.Developer:
                        tt2.overrideColor = new Color(255, 0, 255);
                        break;

                    case CalamityRarity.Rainbow:
                        tt2.overrideColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
                        break;
                    case CalamityRarity.DraedonRust:
                        tt2.overrideColor = new Color(204, 71, 35);
                        break;
                    case CalamityRarity.RareVariant:
                        tt2.overrideColor = new Color(255, 140, 0);
                        break;
                    case CalamityRarity.Dedicated:
                        tt2.overrideColor = new Color(139, 0, 0);
                        break;

                    case CalamityRarity.ItemSpecific:
                        // Uniquely colored developer weapons
                        if (item.type == ModContent.ItemType<Fabstaff>())
                            tt2.overrideColor = new Color(Main.DiscoR, 100, 255);
                        if (item.type == ModContent.ItemType<BlushieStaff>())
                            tt2.overrideColor = new Color(0, 0, 255);
                        if (item.type == ModContent.ItemType<Judgement>())
                            tt2.overrideColor = Judgement.GetSyncedLightColor();
                        if (item.type == ModContent.ItemType<NanoblackReaperMelee>() || item.type == ModContent.ItemType<NanoblackReaperRogue>())
                            tt2.overrideColor = new Color(0.34f, 0.34f + 0.66f * Main.DiscoG / 255f, 0.34f + 0.5f * Main.DiscoG / 255f);
                        if (item.type == ModContent.ItemType<ProfanedSoulCrystal>())
                            tt2.overrideColor = CalamityUtils.ColorSwap(new Color(255, 166, 0), new Color(25, 250, 25), 4f); //alternates between emerald green and amber (BanditHueh)
                        if (item.type == ModContent.ItemType<BensUmbrella>())
                            tt2.overrideColor = CalamityUtils.ColorSwap(new Color(210, 0, 255), new Color(255, 248, 24), 2f);
                        if (item.type == ModContent.ItemType<Endogenesis>())
                            tt2.overrideColor = CalamityUtils.ColorSwap(new Color(131, 239, 255), new Color(36, 55, 230), 2f);
                        if (item.type == ModContent.ItemType<DraconicDestruction>())
                            tt2.overrideColor = CalamityUtils.ColorSwap(new Color(255, 69, 0), new Color(139, 0, 0), 2f);
                        if (item.type == ModContent.ItemType<ScarletDevil>())
                            tt2.overrideColor = CalamityUtils.ColorSwap(new Color(191, 45, 71), new Color(185, 187, 253), 2f);
                        if (item.type == ModContent.ItemType<GaelsGreatsword>())
                            tt2.overrideColor = new Color(146, 0, 0);
                        if (item.type == ModContent.ItemType<CrystylCrusher>())
                            tt2.overrideColor = new Color(129, 29, 149);
                        if (item.type == ModContent.ItemType<Svantechnical>())
                            tt2.overrideColor = new Color(220, 20, 60);
                        if (item.type == ModContent.ItemType<PrototypeAndromechaRing>())
                        {
                            if (Main.GlobalTime % 1f < 0.6f)
                            {
                                tt2.overrideColor = new Color(89, 229, 255);
                            }
                            else if (Main.GlobalTime % 1f < 0.8f)
                            {
                                tt2.overrideColor = Color.Lerp(new Color(89, 229, 255), Color.White, (Main.GlobalTime % 1f - 0.6f) / 0.2f);
                            }
                            else
                            {
                                tt2.overrideColor = Color.Lerp(Color.White, new Color(89, 229, 255), (Main.GlobalTime % 1f - 0.8f) / 0.2f);
                            }
                        }

                        // Uniquely colored legendary weapons and Yharim's Crystal
                        if (item.type == ModContent.ItemType<AegisBlade>() || item.type == ModContent.ItemType<YharimsCrystal>())
                            tt2.overrideColor = new Color(255, Main.DiscoG, 53);
                        if (item.type == ModContent.ItemType<BlossomFlux>())
                            tt2.overrideColor = new Color(Main.DiscoR, 203, 103);
                        if (item.type == ModContent.ItemType<BrinyBaron>() || item.type == ModContent.ItemType<ColdDivinity>())
                            tt2.overrideColor = new Color(53, Main.DiscoG, 255);
                        if (item.type == ModContent.ItemType<CosmicDischarge>())
                            tt2.overrideColor = new Color(150, Main.DiscoG, 255);
                        if (item.type == ModContent.ItemType<Malachite>())
                            tt2.overrideColor = new Color(Main.DiscoR, 203, 103);
                        if (item.type == ModContent.ItemType<SeasSearing>())
                            tt2.overrideColor = new Color(60, Main.DiscoG, 190);
                        if (item.type == ModContent.ItemType<SHPC>())
                            tt2.overrideColor = new Color(255, Main.DiscoG, 155);
                        if (item.type == ModContent.ItemType<Vesuvius>())
                            tt2.overrideColor = new Color(255, Main.DiscoG, 0);
                        if (item.type == ModContent.ItemType<PristineFury>())
							tt2.overrideColor = CalamityUtils.ColorSwap(new Color(255, 168, 53), new Color(255, 249, 0), 2f);
                        if (item.type == ModContent.ItemType<LeonidProgenitor>())
							tt2.overrideColor = CalamityUtils.ColorSwap(LeonidProgenitor.blueColor, LeonidProgenitor.purpleColor, 3f);
                        break;
                }
            }
			#endregion

			#region Accessory Modifier Display
			if (item.accessory)
			{
				if (!item.social && item.prefix > 0)
				{
					float stealthGenBoost = item.Calamity().StealthGenBonus - 1f;
					if (stealthGenBoost > 0)
					{
						TooltipLine StealthGen = new TooltipLine(mod, "PrefixStealthGenBoost", "+" + Math.Round(stealthGenBoost * 100f) + "% stealth generation")
						{
							isModifier = true
						};
						tooltips.Add(StealthGen);
					}
				}
			}
            #endregion

            /*if (item.ammo == 97)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria")
                    {
                        if (line2.Name == "Damage")
                            line2.text = "";
                    }
                }
            }*/

			if (item.type == ItemID.BottledHoney)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "HealLife")
					{
						line2.text += "\nGrants the Honey buff for 2 minutes";
					}
				}
			}
            if (item.type == ItemID.RodofDiscord)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text += "\nTeleportation is disabled while Chaos State is active";
					}
				}
			}
			if (item.type == ItemID.SuperAbsorbantSponge)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text += "\nCannot be used in the Abyss";
                    }
                }
            }
            if (item.type == ItemID.EmptyBucket)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Defense")
                    {
                        line2.text += "\nCannot be used in the Abyss";
                    }
                }
            }
            if (item.type == ItemID.CrimsonHeart)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text += "\nProvides a small amount of light in the abyss";
                    }
                }
            }
            if (item.type == ItemID.ShadowOrb)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text += "\nProvides a small amount of light in the abyss";
                    }
                }
            }
            if (item.type == ItemID.MagicLantern)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text += "\nProvides a small amount of light in the abyss";
                    }
                }
            }
            if (item.type == ItemID.ArcticDivingGear)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip1")
                    {
                        line2.text += "\nProvides a small amount of light in the abyss\n" +
                            "Moderately reduces breath loss in the abyss";
                    }
                }
            }
            if (item.type == ItemID.JellyfishNecklace)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text += "\nProvides a small amount of light in the abyss";
                    }
                }
            }
            if (item.type == ItemID.JellyfishDivingGear)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip1")
                    {
                        line2.text += "\nProvides a small amount of light in the abyss";
                    }
                }
            }
            if (item.type == ItemID.FairyBell)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text += "\nProvides a moderate amount of light in the abyss";
                    }
                }
            }
            if (item.type == ItemID.DD2PetGhost)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text += "\nProvides a moderate amount of light in the abyss";
                    }
                }
            }
            if (item.type == ItemID.ShinePotion)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "BuffTime")
                    {
                        line2.text += "\nProvides a moderate amount of light in the abyss";
                    }
                }
            }
            if (item.type == ItemID.WispinaBottle)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text += "\nProvides a large amount of light in the abyss";
                    }
                }
            }
            if (item.type == ItemID.SuspiciousLookingTentacle)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip1")
                    {
                        line2.text += "\nProvides a large amount of light in the abyss";
                    }
                }
            }
            if (item.type == ItemID.GillsPotion)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "BuffTime")
                    {
                        line2.text += "\nGreatly reduces breath loss in the abyss";
                    }
                }
            }
            if (item.type == ItemID.DivingHelmet)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text += "\nModerately reduces breath loss in the abyss";
                    }
                }
            }
            if (item.type == ItemID.NeptunesShell || item.type == ItemID.MoonShell)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text += "\nGreatly reduces breath loss in the abyss";
                    }
                }
            }
            if (item.type == ItemID.CelestialShell)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip1")
                    {
                        line2.text += "\nGreatly reduces breath loss in the abyss";
                    }
                }
            }
            if (item.type == ItemID.WarmthPotion)
            {
				if (CalamityWorld.death)
				{
					foreach (TooltipLine line2 in tooltips)
					{
						if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
						{
							line2.text += "\nMakes you immune to the Chilled, Frozen, and Glacial State debuffs\n" +
								"Provides cold protection in Death Mode";
						}
					}
				}
				else
				{
					foreach (TooltipLine line2 in tooltips)
					{
						if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
						{
							line2.text += "\nMakes you immune to the Chilled, Frozen, and Glacial State debuffs";
						}
					}
				}
            }
            if (item.type == ItemID.FlaskofVenom)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Melee and rogue attacks inflict Venom on enemies";
                    }
                }
            }
            if (item.type == ItemID.FlaskofCursedFlames)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Melee and rogue attacks inflict enemies with cursed flames";
                    }
                }
            }
            if (item.type == ItemID.FlaskofFire)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Melee and rogue attacks set enemies on fire";
                    }
                }
            }
            if (item.type == ItemID.FlaskofGold)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Melee and rogue attacks make enemies drop more gold";
                    }
                }
            }
            if (item.type == ItemID.FlaskofIchor)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Melee and rogue attacks decrease enemy defense";
                    }
                }
            }
            if (item.type == ItemID.FlaskofNanites)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Melee and rogue attacks confuse enemies";
                    }
                }
            }
            if (item.type == ItemID.FlaskofParty)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "All attacks cause confetti to appear";
                    }
                }
            }
            if (item.type == ItemID.FlaskofPoison)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Melee and rogue attacks poison enemies";
                    }
                }
            }
            if (item.type == ItemID.WormScarf)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Reduces damage taken by 10%";
                    }
                }
            }
			if (item.type == ItemID.TitanGlove)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text += "\n10% increased true melee damage";
					}
				}
			}
			if (item.type == ItemID.PowerGlove)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip1")
					{
						line2.text += "\n10% increased true melee damage";
					}
				}
			}
			if (item.type == ItemID.MechanicalGlove)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip1")
					{
						line2.text += "\n10% increased true melee damage";
					}
				}
			}
			if (item.type == ItemID.FireGauntlet)
            {
				if (CalamityWorld.death)
				{
					foreach (TooltipLine line2 in tooltips)
					{
						if (line2.mod == "Terraria" && line2.Name == "Tooltip1")
						{
							line2.text = "14% increased melee damage and speed\n" +
								"10% increased true melee damage\n" +
								"Provides heat and cold protection in Death Mode";
						}
					}
				}
				else
				{
					foreach (TooltipLine line2 in tooltips)
					{
						if (line2.mod == "Terraria" && line2.Name == "Tooltip1")
						{
							line2.text = "14% increased melee damage and speed\n" +
								"10% increased true melee damage";
						}
					}
				}
            }
            if (item.type == ItemID.SpectreHood)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "20% decreased magic damage";
                    }
                }
            }
            if (item.type == ItemID.ObsidianSkinPotion)
            {
				string heatImmunity = CalamityWorld.death ? "\nProvides heat protection in Death Mode" : "";
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Provides immunity to direct damage from touching lava\n" +
							"Provides temporary immunity to lava burn damage\n" +
							"Reduces lava burn damage" + heatImmunity;
					}
				}
            }
            if (item.type == ItemID.ObsidianRose)
            {
				string heatImmunity = CalamityWorld.death ? "\nProvides heat protection in Death Mode" : "";
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Reduced direct damage from touching lava\n" +
							"Greatly reduces lava burn damage" + heatImmunity;
					}
				}
            }
            if (item.type == ItemID.MagmaStone && CalamityWorld.death)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text += "\nProvides heat and cold protection in Death Mode";
                    }
                }
            }
            if (item.type == ItemID.LavaCharm && CalamityWorld.death)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text += "\nProvides heat protection in Death Mode";
                    }
                }
            }
            if (item.type == ItemID.LavaWaders && CalamityWorld.death)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip1")
                    {
                        line2.text += "\nProvides heat protection in Death Mode";
                    }
                }
            }
            if (item.type == ItemID.InvisibilityPotion)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text += "\nBoosts certain stats when holding certain types of rogue weapons";
                    }
                }
            }
            if (item.type == ItemID.GoldenFishingRod)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "NeedsBait")
                    {
                        line2.text = "Requires bait to catch fish\n" +
                            "The line will never break";
                    }
                }
            }
            if (item.type == ItemID.Picksaw)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
						// I'm leaving this intentionally ambiguous so that people have to search for Scoria Ore
                        line2.text = "Capable of mining Lihzahrd Bricks and Scoria Ore";
                    }
                }
            }
            if (item.type == ItemID.SolarFlarePickaxe)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Material")
                    {
                        line2.text = "Material\n" +
                            "Can mine Uelibloom Ore";
                    }
                }
            }
            if (item.type == ItemID.VortexPickaxe)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Material")
                    {
                        line2.text = "Material\n" +
                            "Can mine Uelibloom Ore";
                    }
                }
            }
            if (item.type == ItemID.NebulaPickaxe)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Material")
                    {
                        line2.text = "Material\n" +
                            "Can mine Uelibloom Ore";
                    }
                }
            }
            if (item.type == ItemID.StardustPickaxe)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Material")
                    {
                        line2.text = "Material\n" +
                            "Can mine Uelibloom Ore";
                    }
                }
            }

			if (Main.player[Main.myPlayer].Calamity().trueMeleeDamage > 0D)
			{
				if (item.melee && item.damage > 0)
				{
					foreach (TooltipLine line2 in tooltips)
					{
						if (line2.mod == "Terraria" && line2.Name == "Damage")
						{
							line2.text += " : " + string.Concat((int)(Main.player[Main.myPlayer].Calamity().actualMeleeDamageStat * item.damage * 
								(1D + Main.player[Main.myPlayer].Calamity().trueMeleeDamage) + 5E-06f)) + " true melee damage";
						}
					}
				}
			}

			if (item.type == ItemID.MeteorHelmet || item.type == ItemID.MeteorSuit || item.type == ItemID.MeteorLeggings)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "SetBonus")
                    {
                        line2.text = "Set Bonus: Reduces the mana cost of the Space Gun by 50%";
                    }
                }
            }
            if (item.type == ItemID.CopperHelmet || item.type == ItemID.CopperChainmail || item.type == ItemID.CopperGreaves)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "SetBonus")
                    {
                        line2.text = "Set Bonus: +2 defense and 15% increased mining speed";
                    }
                }
            }
            if (item.type == ItemID.TinHelmet || item.type == ItemID.TinChainmail || item.type == ItemID.TinGreaves)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "SetBonus")
                    {
                        line2.text = "Set Bonus: +2 defense and 10% increased mining speed";
                    }
                }
            }
            if (item.type == ItemID.AncientIronHelmet || item.type == ItemID.IronHelmet || item.type == ItemID.IronChainmail || item.type == ItemID.IronGreaves)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "SetBonus")
                    {
                        line2.text = "Set Bonus: +2 defense and 25% increased mining speed";
                    }
                }
            }
            if (item.type == ItemID.LeadHelmet || item.type == ItemID.LeadChainmail || item.type == ItemID.LeadGreaves)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "SetBonus")
                    {
                        line2.text = "Set Bonus: +3 defense and 20% increased mining speed";
                    }
                }
            }
            if (item.type == ItemID.SilverHelmet || item.type == ItemID.SilverChainmail || item.type == ItemID.SilverGreaves)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "SetBonus")
                    {
                        line2.text = "Set Bonus: +3 defense and 35% increased mining speed";
                    }
                }
            }
            if (item.type == ItemID.TungstenHelmet || item.type == ItemID.TungstenChainmail || item.type == ItemID.TungstenGreaves)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "SetBonus")
                    {
                        line2.text = "Set Bonus: +3 defense and 30% increased mining speed";
                    }
                }
            }
            if (item.type == ItemID.AncientGoldHelmet || item.type == ItemID.GoldHelmet || item.type == ItemID.GoldChainmail || item.type == ItemID.GoldGreaves)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "SetBonus")
                    {
                        line2.text = "Set Bonus: +3 defense and 45% increased mining speed";
                    }
                }
            }
            if (item.type == ItemID.PlatinumHelmet || item.type == ItemID.PlatinumChainmail || item.type == ItemID.PlatinumGreaves)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "SetBonus")
                    {
                        line2.text = "Set Bonus: +4 defense and 40% increased mining speed";
                    }
                }
            }
			if (item.type == ItemID.AncientBattleArmorHat || item.type == ItemID.AncientBattleArmorShirt || item.type == ItemID.AncientBattleArmorPants)
            {
				if (!Main.player[Main.myPlayer].Calamity().forbiddenCirclet)
				{
					foreach (TooltipLine line2 in tooltips)
					{
						if (line2.mod == "Terraria" && line2.Name == "SetBonus")
						{
							line2.text += "\nThe minion damage nerf is reduced while wielding magic weapons";
						}
					}
				}
            }
            if (item.type == ItemID.GladiatorHelmet)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Defense")
                    {
                        line2.text = "3 defense\n" +
                            "3% increased rogue damage";
                    }
                }
            }
            if (item.type == ItemID.GladiatorBreastplate)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Defense")
                    {
                        line2.text = "5 defense\n" +
                            "3% increased rogue critical strike chance";
                    }
                }
            }
            if (item.type == ItemID.GladiatorLeggings)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Defense")
                    {
                        line2.text = "4 defense\n" +
                            "3% increased rogue velocity";
                    }
                }
            }
            if (item.type == ItemID.ObsidianHelm)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Defense")
                    {
                        line2.text = "4 defense\n" +
                            "3% increased rogue damage";
                    }
                }
            }
            if (item.type == ItemID.ObsidianShirt)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Defense")
                    {
                        line2.text = "5 defense\n" +
                            "3% increased rogue critical strike chance";
                    }
                }
            }
            if (item.type == ItemID.ObsidianPants)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Defense")
                    {
                        line2.text = "4 defense\n" +
                            "3% increased rogue velocity";
                    }
                }
            }
			if (item.type == ItemID.MoltenHelmet || item.type == ItemID.MoltenBreastplate || item.type == ItemID.MoltenGreaves)
            {
				if (CalamityWorld.death)
				{
					foreach (TooltipLine line2 in tooltips)
					{
						if (line2.mod == "Terraria" && line2.Name == "SetBonus")
						{
							line2.text = @"Set Bonus: 17% extra melee damage
20% extra true melee damage
Grants immunity to fire blocks, and temporary immunity to lava
Provides heat and cold protection in Death Mode";
						}
					}
                }
				else
				{
					foreach (TooltipLine line2 in tooltips)
					{
						if (line2.mod == "Terraria" && line2.Name == "SetBonus")
						{
							line2.text = @"Set Bonus: 17% extra melee damage
20% extra true melee damage
Grants immunity to fire blocks, and temporary immunity to lava";
						}
					}
                }
            }
            if (item.type == ItemID.FrostHelmet || item.type == ItemID.FrostBreastplate || item.type == ItemID.FrostLeggings)
            {
				if (CalamityWorld.death)
				{
					foreach (TooltipLine line2 in tooltips)
					{
						if (line2.mod == "Terraria" && line2.Name == "SetBonus")
						{
							line2.text += "\nProvides heat and cold protection in Death Mode";
						}
					}
				}
            }
            if (item.type == ItemID.MagicQuiver)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Increases arrow damage by 10% and greatly increases arrow speed";
                    }
                }
            }
            if (item.type == ItemID.MiningHelmet)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Provides light when worn\n" +
							"Provides a small amount of light in the Abyss";
                    }
                }
            }
            if (item.type == ItemID.HandWarmer)
            {
				string coldImmunity = CalamityWorld.death ? "\nProvides cold protection in Death Mode" : "";
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Provides immunity to chilling and freezing effects\n" +
							"Provides a regeneration boost while wearing the Eskimo armor" + coldImmunity;
                    }
                }
            }
            if (item.type == ItemID.AngelWings)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Allows flight and slow fall\n" +
                            "Horizontal speed: 6.25\n" +
                            "Acceleration multiplier: 1\n" +
                            "Average vertical speed\n" +
                            "Flight time: 100\n" +
                            "+20 max life, +15 defense and +3 life regen";
                    }
                }
            }
            if (item.type == ItemID.DemonWings)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Allows flight and slow fall\n" +
                            "Horizontal speed: 6.25\n" +
                            "Acceleration multiplier: 1\n" +
                            "Average vertical speed\n" +
                            "Flight time: 100\n" +
                            "10% increased damage and critical strike chance";
                    }
                }
            }
            if (item.type == ItemID.Jetpack)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Allows flight and slow fall\n" +
                            "Horizontal speed: 6.5\n" +
                            "Acceleration multiplier: 1\n" +
                            "Average vertical speed\n" +
                            "Flight time: 115";
                    }
                }
            }
            if (item.type == ItemID.ButterflyWings)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Allows flight and slow fall\n" +
                            "Horizontal speed: 6.75\n" +
                            "Acceleration multiplier: 1\n" +
                            "Average vertical speed\n" +
                            "Flight time: 130\n" +
                            "+50 max mana, 5% decreased mana usage,\n" +
                            "10% increased magic damage and 5% increased magic critical strike chance";
                    }
                }
            }
            if (item.type == ItemID.FairyWings)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Allows flight and slow fall\n" +
                            "Horizontal speed: 6.75\n" +
                            "Acceleration multiplier: 1\n" +
                            "Average vertical speed\n" +
                            "Flight time: 130\n" +
                            "+80 max life";
                    }
                }
            }
            if (item.type == ItemID.BeeWings)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Allows flight and slow fall\n" +
                            "Horizontal speed: 6.75\n" +
                            "Acceleration multiplier: 1\n" +
                            "Average vertical speed\n" +
                            "Flight time: 130\n" +
                            "Honey buff at all times";
                    }
                }
            }
            if (item.type == ItemID.HarpyWings)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Allows flight and slow fall\n" +
                            "Horizontal speed: 7\n" +
                            "Acceleration multiplier: 1\n" +
                            "Average vertical speed\n" +
                            "Flight time: 140\n" +
                            "30% increased movement speed\n" +
							"Most attacks have a chance to fire a feather on swing if Harpy Ring or Angel Treads are equipped";
                    }
                }
            }
            if (item.type == ItemID.BoneWings)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Allows flight and slow fall\n" +
                            "Horizontal speed: 7\n" +
                            "Acceleration multiplier: 1\n" +
                            "Average vertical speed\n" +
                            "Flight time: 140\n" +
                            "15% increased movement speed, 12% increased ranged damage,\n" +
                            "16% increased ranged critical strike chance\n" +
                            "and +30 defense while wearing the Necro Armor";
                    }
                }
            }
            if (item.type == ItemID.FlameWings)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Allows flight and slow fall\n" +
                            "Horizontal speed: 7.5\n" +
                            "Acceleration multiplier: 1\n" +
                            "Average vertical speed\n" +
                            "Flight time: 160\n" +
                            "10% increased melee damage\n" +
                            "and 5% increased melee critical strike chance";
                    }
                }
            }
            if (item.type == ItemID.FrozenWings)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Allows flight and slow fall\n" +
                            "Horizontal speed: 7.5\n" +
                            "Acceleration multiplier: 1\n" +
                            "Average vertical speed\n" +
                            "Flight time: 160\n" +
                            "2% increased melee and ranged damage\n" +
                            "and 1% increased melee and ranged critical strike chance\n" +
                            "while wearing the Frost Armor";
                    }
                }
            }
            if (item.type == ItemID.GhostWings)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Allows flight and slow fall\n" +
                            "Horizontal speed: 7.5\n" +
                            "Acceleration multiplier: 1\n" +
                            "Average vertical speed\n" +
                            "Flight time: 160\n" +
                            "+10 defense and 5% increased damage reduction while wearing the Spectre Armor and Hood\n" +
                            "+20 max mana, 5% increased magic damage and critical strike chance,\n" +
                            "and 5% decreased mana usage while wearing the Spectre Armor and Mask";
                    }
                }
            }
            if (item.type == ItemID.BeetleWings)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Allows flight and slow fall\n" +
                            "Horizontal speed: 7.5\n" +
                            "Acceleration multiplier: 1\n" +
                            "Average vertical speed\n" +
                            "Flight time: 160\n" +
                            "+15 defense and 10% increased damage reduction while wearing the Beetle Armor and Shell\n" +
                            "10% increased melee damage and critical strike chance while wearing the Beetle Armor and Scale Mail";
                    }
                }
            }
            if (item.type == ItemID.FinWings)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Allows flight and slow fall\n" +
                            "Horizontal speed: 0\n" +
                            "Acceleration multiplier: 0\n" +
                            "Average vertical speed\n" +
                            "Flight time: 100\n" +
                            "Gills effect and you can move freely through liquids\n" +
                            "You fall faster while submerged in liquid\n" +
                            "20% increased movement speed and 36% increased jump speed";
                    }
                }
            }
            if (item.type == ItemID.FishronWings)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Allows flight and slow fall\n" +
                            "Horizontal speed: 8\n" +
                            "Acceleration multiplier: 2\n" +
                            "Good vertical speed\n" +
                            "Flight time: 180";
                    }
                }
            }
            if (item.type == ItemID.SteampunkWings)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Allows flight and slow fall\n" +
                            "Horizontal speed: 7.75\n" +
                            "Acceleration multiplier: 1\n" +
                            "Average vertical speed\n" +
                            "Flight time: 180\n" +
                            "+8 defense, 10% increased movement speed,\n" +
                            "4% increased damage, and 2% increased critical strike chance";
                    }
                }
            }
            if (item.type == ItemID.LeafWings)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Allows flight and slow fall\n" +
                            "Horizontal speed: 6.75\n" +
                            "Acceleration multiplier: 1\n" +
                            "Average vertical speed\n" +
                            "Flight time: 160\n" +
                            "+10 defense, 10% increased damage reduction,\n" +
							"and the Dryad's permanent blessing while wearing the Tiki Armor";
                    }
                }
            }
            if (item.type == ItemID.BatWings)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Allows flight and slow fall\n" +
                            "Horizontal speed: 0\n" +
                            "Acceleration multiplier: 0\n" +
                            "Average vertical speed\n" +
                            "Flight time: 140\n" +
                            "At night or during an eclipse, you will gain the following boosts:\n" +
							"10% increased movement speed, 20% increased jump speed,\n" +
                            "+15 defense, 10% increased damage, and 5% increased critical strike chance";
                    }
                }
            }
            if (item.type == ItemID.Yoraiz0rWings || item.type == ItemID.JimsWings || item.type == ItemID.SkiphsWings ||
                item.type == ItemID.LokisWings || item.type == ItemID.ArkhalisWings || item.type == ItemID.LeinforsWings ||
                item.type == ItemID.BejeweledValkyrieWing || item.type == ItemID.RedsWings || item.type == ItemID.DTownsWings ||
                item.type == ItemID.WillsWings || item.type == ItemID.CrownosWings || item.type == ItemID.CenxsWings)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "'Great for impersonating devs!'\n" +
                            "Horizontal speed: 7\n" +
                            "Acceleration multiplier: 1\n" +
                            "Average vertical speed\n" +
                            "Flight time: 150";
                    }
                }
            }
            if (item.type == ItemID.TatteredFairyWings)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Allows flight and slow fall\n" +
                            "Horizontal speed: 7.5\n" +
                            "Acceleration multiplier: 1\n" +
                            "Average vertical speed\n" +
                            "Flight time: 180\n" +
                            "5% increased damage and critical strike chance";
                    }
                }
            }
            if (item.type == ItemID.SpookyWings)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Allows flight and slow fall\n" +
                            "Horizontal speed: 7.5\n" +
                            "Acceleration multiplier: 1\n" +
                            "Average vertical speed\n" +
                            "Flight time: 180\n" +
                            "Increased minion knockback and 5% increased minion damage while wearing the Spooky Armor";
                    }
                }
            }
            if (item.type == ItemID.Hoverboard)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip1")
                    {
                        line2.text = "Hold DOWN and JUMP to hover\n" +
                            "Horizontal speed: 6.25\n" +
                            "Acceleration multiplier: 1\n" +
                            "Average vertical speed\n" +
                            "Flight time: 170\n" +
                            "10% increased damage to bows, guns, rocket launchers, and flamethrowers while wearing the Shroomite Armor\n" +
							"Boosted weapon type depends on the Shroomite Helmet worn";
                    }
                }
            }
            if (item.type == ItemID.FestiveWings)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Allows flight and slow fall\n" +
                            "Horizontal speed: 7.5\n" +
                            "Acceleration multiplier: 1\n" +
                            "Average vertical speed\n" +
                            "Flight time: 170\n" +
                            "+50 max life\n" +
                            "Ornaments rain down as you fly";
                    }
                }
            }
            if (item.type == ItemID.MothronWings)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Allows flight and slow fall\n" +
                            "Horizontal speed: 0\n" +
                            "Acceleration multiplier: 0\n" +
                            "Average vertical speed\n" +
                            "Flight time: 160\n" +
                            "+5 defense, 5% increased damage,\n" +
                            "10% increased movement speed and 24% increased jump speed";
                    }
                }
            }
            if (item.type == ItemID.WingsSolar)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Allows flight and slow fall\n" +
                            "Horizontal speed: 9\n" +
                            "Acceleration multiplier: 2.5\n" +
                            "Great vertical speed\n" +
                            "Flight time: 180\n" +
                            "7% increased melee damage and 3% increased melee critical strike chance\n" +
                            "while wearing the Solar Flare Armor";
                    }
                }
            }
            if (item.type == ItemID.WingsStardust)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Allows flight and slow fall\n" +
                            "Horizontal speed: 9\n" +
                            "Acceleration multiplier: 2.5\n" +
                            "Great vertical speed\n" +
                            "Flight time: 180\n" +
                            "+1 max minion and 5% increased minion damage while wearing the Stardust Armor";
                    }
                }
            }
            if (item.type == ItemID.WingsVortex)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Allows flight and slow fall\n" +
							"Hold DOWN and JUMP to hover\n" +
                            "Horizontal speed: 6.5\n" +
                            "Acceleration multiplier: 1.5\n" +
                            "Good vertical speed\n" +
                            "Flight time: 160\n" +
                            "3% increased ranged damage and 7% increased ranged critical strike chance\n" +
                            "while wearing the Vortex Armor";
                    }
                }
            }
            if (item.type == ItemID.WingsNebula)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Allows flight and slow fall\n" +
							"Hold DOWN and JUMP to hover\n" +
                            "Horizontal speed: 6.5\n" +
                            "Acceleration multiplier: 1.5\n" +
                            "Good vertical speed\n" +
                            "Flight time: 160\n" +
                            "+20 max mana, 5% increased magic damage and critical strike chance,\n" +
                            "and 5% decreased mana usage while wearing the Nebula Armor";
                    }
                }
            }
            if (item.type == ItemID.BetsyWings)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Equipable")
                    {
                        line2.text = "Equipable\n" +
                            "Allows flight and slow fall\n" +
							"Hold DOWN and JUMP to hover\n" +
                            "Horizontal speed: 6\n" +
                            "Acceleration multiplier: 2.5\n" +
                            "Good vertical speed\n" +
                            "Flight time: 150";
                    }
                }
            }
            if (item.type == ItemID.GrapplingHook)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Equipable")
                    {
                        line2.text = "Equipable\n" +
                            "Reach: 18.75\n" +
                            "Launch Velocity: 11.5\n" +
							"Pull Velocity: 11";
                    }
                }
            }
            if (item.type == ItemID.AmethystHook)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Equipable")
                    {
                        line2.text = "Equipable\n" +
                            "Reach: 18.75\n" +
                            "Launch Velocity: 10\n" +
							"Pull Velocity: 11";
                    }
                }
            }
            if (item.type == ItemID.TopazHook)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Equipable")
                    {
                        line2.text = "Equipable\n" +
                            "Reach: 20.625\n" +
                            "Launch Velocity: 10.5\n" +
							"Pull Velocity: 11.75";
                    }
                }
            }
            if (item.type == ItemID.SapphireHook)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Equipable")
                    {
                        line2.text = "Equipable\n" +
                            "Reach: 22.5\n" +
                            "Launch Velocity: 11\n" +
							"Pull Velocity: 12.5";
                    }
                }
            }
            if (item.type == ItemID.EmeraldHook)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Equipable")
                    {
                        line2.text = "Equipable\n" +
                            "Reach: 24.375\n" +
                            "Launch Velocity: 11.5\n" +
							"Pull Velocity: 13.25";
                    }
                }
            }
            if (item.type == ItemID.RubyHook)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Equipable")
                    {
                        line2.text = "Equipable\n" +
                            "Reach: 26.25\n" +
                            "Launch Velocity: 12\n" +
							"Pull Velocity: 14";
                    }
                }
            }
            if (item.type == ItemID.DiamondHook)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Equipable")
                    {
                        line2.text = "Equipable\n" +
                            "Reach: 28.125\n" +
                            "Launch Velocity: 12.5\n" +
							"Pull Velocity: 14.75";
                    }
                }
            }
            if (item.type == ItemID.WebSlinger)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Equipable")
                    {
                        line2.text = "Equipable\n" +
                            "Reach: 15.625\n" +
                            "Launch Velocity: 10\n" +
							"Pull Velocity: 11";
                    }
                }
            }
            if (item.type == ItemID.SkeletronHand)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Equipable")
                    {
                        line2.text = "Equipable\n" +
                            "Reach: 21.875\n" +
                            "Launch Velocity: 15\n" +
							"Pull Velocity: 11";
                    }
                }
            }
            if (item.type == ItemID.SlimeHook)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Equipable")
                    {
                        line2.text = "Equipable\n" +
                            "Reach: 18.75\n" +
                            "Launch Velocity: 13\n" +
							"Pull Velocity: 11";
                    }
                }
            }
            if (item.type == ItemID.FishHook)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Equipable")
                    {
                        line2.text = "Equipable\n" +
                            "Reach: 25\n" +
                            "Launch Velocity: 13\n" +
							"Pull Velocity: 11";
                    }
                }
            }
            if (item.type == ItemID.IvyWhip)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Equipable")
                    {
                        line2.text = "Equipable\n" +
                            "Reach: 25\n" +
                            "Launch Velocity: 13\n" +
							"Pull Velocity: 15";
                    }
                }
            }
            if (item.type == ItemID.BatHook)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Equipable")
                    {
                        line2.text = "Equipable\n" +
                            "Reach: 31.25\n" +
                            "Launch Velocity: 15.5\n" +
							"Pull Velocity: 20";
                    }
                }
            }
            if (item.type == ItemID.CandyCaneHook)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Equipable")
                    {
                        line2.text = "Equipable\n" +
                            "Reach: 25\n" +
                            "Launch Velocity: 11.5\n" +
							"Pull Velocity: 11";
                    }
                }
            }
            if (item.type == ItemID.DualHook)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Equipable")
                    {
                        line2.text = "Equipable\n" +
                            "Reach: 27.5\n" +
                            "Launch Velocity: 14\n" +
							"Pull Velocity: 17";
                    }
                }
            }
            if (item.type == ItemID.ThornHook)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Equipable")
                    {
                        line2.text = "Equipable\n" +
                            "Reach: 30\n" +
                            "Launch Velocity: 15\n" +
							"Pull Velocity: 18";
                    }
                }
            }
            if (item.type == ItemID.WormHook)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Equipable")
                    {
                        line2.text = "Equipable\n" +
                            "Reach: 30\n" +
                            "Launch Velocity: 15\n" +
							"Pull Velocity: 18";
                    }
                }
            }
            if (item.type == ItemID.TendonHook)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Equipable")
                    {
                        line2.text = "Equipable\n" +
                            "Reach: 30\n" +
                            "Launch Velocity: 15\n" +
							"Pull Velocity: 18";
                    }
                }
            }
            if (item.type == ItemID.IlluminantHook)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Equipable")
                    {
                        line2.text = "Equipable\n" +
                            "Reach: 30\n" +
                            "Launch Velocity: 15\n" +
							"Pull Velocity: 18";
                    }
                }
            }
            if (item.type == ItemID.AntiGravityHook)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Equipable")
                    {
                        line2.text = "Equipable\n" +
                            "Reach: 31.25\n" +
                            "Launch Velocity: 14\n" +
							"Pull Velocity: 20";
                    }
                }
            }
            if (item.type == ItemID.SpookyHook)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Equipable")
                    {
                        line2.text = "Equipable\n" +
                            "Reach: 34.375\n" +
                            "Launch Velocity: 15.5\n" +
							"Pull Velocity: 22";
                    }
                }
            }
            if (item.type == ItemID.ChristmasHook)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Equipable")
                    {
                        line2.text = "Equipable\n" +
                            "Reach: 34.375\n" +
                            "Launch Velocity: 15.5\n" +
							"Pull Velocity: 17";
                    }
                }
            }
            if (item.type == ItemID.LunarHook)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Equipable")
                    {
                        line2.text = "Equipable\n" +
                            "Reach: 34.375\n" +
                            "Launch Velocity: 16\n" +
							"Pull Velocity: 24";
                    }
                }
            }
            if (item.type == ItemID.StaticHook)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Equipable")
                    {
                        line2.text = "Equipable\n" +
                            "Reach: 37.5\n" +
                            "Launch Velocity: 16\n" +
							"Pull Velocity: 24";
                    }
                }
            }
            if (item.accessory)
            {
                if (item.prefix == PrefixID.Precise)
                {
                    foreach (TooltipLine line2 in tooltips)
                    {
                        if (line2.mod == "Terraria" && line2.Name == "PrefixAccCritChance")
                        {
                            line2.text = "+1% critical strike chance";
                        }
                    }
                }
                if (item.prefix == PrefixID.Lucky)
                {
                    foreach (TooltipLine line2 in tooltips)
                    {
                        if (line2.mod == "Terraria" && line2.Name == "PrefixAccCritChance")
                        {
                            line2.text = "+3% critical strike chance";
                        }
                    }
                }
                if (item.prefix == PrefixID.Hard)
                {
					string defenseBoost = "+1 defense\n";
					if (NPC.downedMoonlord)
					{
						defenseBoost = "+3 defense\n";
					}
					else if (Main.hardMode)
					{
						defenseBoost = "+2 defense\n";
					}
                    foreach (TooltipLine line2 in tooltips)
                    {
                        if (line2.mod == "Terraria" && line2.Name == "PrefixAccDefense")
                        {
                            line2.text = defenseBoost + "+0.25% damage reduction";
                        }
                    }
                }
                if (item.prefix == PrefixID.Guarding)
                {
					string defenseBoost = "+2 defense\n";
					if (NPC.downedMoonlord)
					{
						defenseBoost = "+4 defense\n";
					}
					else if (Main.hardMode)
					{
						defenseBoost = "+3 defense\n";
					}
					foreach (TooltipLine line2 in tooltips)
                    {
                        if (line2.mod == "Terraria" && line2.Name == "PrefixAccDefense")
                        {
                            line2.text = defenseBoost + "+0.5% damage reduction";
                        }
                    }
                }
                if (item.prefix == PrefixID.Armored)
                {
					string defenseBoost = "+3 defense\n";
					if (NPC.downedMoonlord)
					{
						defenseBoost = "+6 defense\n";
					}
					else if (Main.hardMode)
					{
						defenseBoost = "+4 defense\n";
					}
					foreach (TooltipLine line2 in tooltips)
                    {
                        if (line2.mod == "Terraria" && line2.Name == "PrefixAccDefense")
                        {
                            line2.text = defenseBoost + "+0.75% damage reduction";
                        }
                    }
                }
                if (item.prefix == PrefixID.Warding)
                {
					string defenseBoost = "+4 defense\n";
					if (NPC.downedMoonlord)
					{
						defenseBoost = "+8 defense\n";
					}
					else if (Main.hardMode)
					{
						defenseBoost = "+6 defense\n";
					}
					foreach (TooltipLine line2 in tooltips)
                    {
                        if (line2.mod == "Terraria" && line2.Name == "PrefixAccDefense")
                        {
                            line2.text = defenseBoost + "+1% damage reduction";
                        }
                    }
                }
            }

			Mod fargos = ModLoader.GetMod("Fargowiltas");
			if (fargos != null)
			{
				//Fargo's fountain effects
				if (item.type == ModContent.ItemType<SunkenSeaFountain>())
				{
					TooltipLine line = new TooltipLine(mod, "Tooltip0", "Forces surrounding biome state to Sunken Sea upon activation");
					tooltips.Add(line);
				}
				if (item.type == ModContent.ItemType<SulphurousFountainItem>())
				{
					TooltipLine line = new TooltipLine(mod, "Tooltip0", "Forces surrounding biome state to Sulphurous Sea upon activation");
					tooltips.Add(line);
				}
				if (item.type == ModContent.ItemType<AstralFountainItem>())
				{
					TooltipLine line = new TooltipLine(mod, "Tooltip0", "Forces surrounding biome state to Astral upon activation");
					tooltips.Add(line);
				}
            }

            if (item.type > ItemID.Count && item.Calamity().Chargeable)
            {
                float chargeRatio = item.Calamity().CurrentCharge / (float)ChargeMax;
                chargeRatio *= 100f; // Turn the 0-1 ratio to a 0-100 percentage.
                TooltipLine line = new TooltipLine(mod, "Tooltip0", $"Current Charge: {chargeRatio:N1}%");
                tooltips.Add(line);
            }
        }
		#endregion

		// NOTE: this function applies to all treasure bags, even modded ones (despite the name).
		#region Boss Bag Changes
		public override void OpenVanillaBag(string context, Player player, int arg)
        {
			if (context == "crate")
			{
				switch (arg)
				{
					case ItemID.WoodenCrate:
                        DropHelper.DropItemChance(player, ModContent.ItemType<WulfrumShard>(), 4, 3, 5);
                        break;

					case ItemID.IronCrate:
                        DropHelper.DropItemChance(player, ModContent.ItemType<WulfrumShard>(), 4, 5, 8);
                        DropHelper.DropItemChance(player, ModContent.ItemType<AncientBoneDust>(), 4, 5, 8);
                        break;

					case ItemID.CorruptFishingCrate:
                        DropHelper.DropItemChance(player, ModContent.ItemType<FetidEssence>(), 4, 5, 8);
                        DropHelper.DropItemChance(player, ModContent.ItemType<EbonianGel>(), 4, 5, 8);
                        DropHelper.DropItemChance(player, ModContent.ItemType<MurkySludge>(), 5, 1, 3);
                        break;

					case ItemID.CrimsonFishingCrate:
                        DropHelper.DropItemChance(player, ModContent.ItemType<BloodlettingEssence>(), 4, 5, 8);
                        DropHelper.DropItemChance(player, ModContent.ItemType<EbonianGel>(), 4, 5, 8);
                        DropHelper.DropItemChance(player, ModContent.ItemType<MurkySludge>(), 5, 1, 3);
                        break;

					case ItemID.HallowedFishingCrate:
                        DropHelper.DropItemCondition(player, ModContent.ItemType<UnholyEssence>(), NPC.downedMoonlord, 0.2f, 5, 10);
                        DropHelper.DropItemCondition(player, (WorldGen.crimson ? ModContent.ItemType<ProfanedRagePotion>() : ModContent.ItemType<HolyWrathPotion>()), CalamityWorld.downedProvidence, 0.2f, 1, 3);
                        break;

					case ItemID.DungeonFishingCrate:
                        DropHelper.DropItemCondition(player, ItemID.Ectoplasm, NPC.downedPlantBoss, 0.3f, 5, 10);
                        DropHelper.DropItemCondition(player, ModContent.ItemType<Phantoplasm>(), NPC.downedMoonlord, 0.2f, 5, 10);
                        break;

					case ItemID.JungleFishingCrate:
                        DropHelper.DropItemChance(player, ModContent.ItemType<MurkyPaste>(), 5, 1, 3);
                        DropHelper.DropItemChance(player, ModContent.ItemType<ManeaterBulb>(), 5, 1, 3);
                        DropHelper.DropItemCondition(player, ModContent.ItemType<BeetleJuice>(), Main.hardMode, 0.2f, 1, 3);
                        DropHelper.DropItemCondition(player, ModContent.ItemType<TrapperBulb>(), Main.hardMode, 0.2f, 1, 3);
                        DropHelper.DropItemCondition(player, ItemID.ChlorophyteBar, (CalamityWorld.downedCalamitas || NPC.downedPlantBoss), 0.25f, 5, 10);
                        DropHelper.DropItemCondition(player, ModContent.ItemType<DraedonBar>(), NPC.downedPlantBoss, 0.25f, 5, 10);
                        DropHelper.DropItemCondition(player, ModContent.ItemType<PlagueCellCluster>(), NPC.downedGolemBoss, 0.2f, 3, 6);
                        DropHelper.DropItemCondition(player, ModContent.ItemType<UeliaceBar>(), CalamityWorld.downedProvidence, 0.25f, 5, 10);
                        break;

					case ItemID.FloatingIslandFishingCrate:
                        DropHelper.DropItemCondition(player, ModContent.ItemType<AerialiteBar>(), (CalamityWorld.downedHiveMind || CalamityWorld.downedPerforator), 0.25f, 5, 10);
                        DropHelper.DropItemCondition(player, ModContent.ItemType<EssenceofCinder>(), Main.hardMode, 0.2f, 5, 15);
                        DropHelper.DropItemCondition(player, ModContent.ItemType<GalacticaSingularity>(), NPC.downedMoonlord, 0.1f, 1, 3);
                        break;
				}
			}

            if (context == "bossBag")
            {
                // Give a chance for Laudanum, Stress Pills and Heart of Darkness from every boss bag
                DropHelper.DropRevBagAccessories(player);

                switch (arg)
                {
                    // King Slime
                    case ItemID.KingSlimeBossBag:
                        DropHelper.DropItemCondition(player, ModContent.ItemType<CrownJewel>(), CalamityWorld.revenge);
                        break;

                    // Eye of Cthulhu
                    case ItemID.EyeOfCthulhuBossBag:
                        DropHelper.DropItem(player, ModContent.ItemType<VictoryShard>(), 3, 5);
                        DropHelper.DropItemChance(player, ModContent.ItemType<TeardropCleaver>(), 3);
                        DropHelper.DropItemCondition(player, ModContent.ItemType<CounterScarf>(), CalamityWorld.revenge);
                        break;

                    // Queen Bee
                    case ItemID.QueenBeeBossBag:
                        DropHelper.DropItem(player, ItemID.Stinger, 8, 12);
                        DropHelper.DropItem(player, ModContent.ItemType<HardenedHoneycomb>(), 50, 75);
                        break;

                    // Skeletron
                    case ItemID.SkeletronBossBag:
                        DropHelper.DropItemChance(player, ModContent.ItemType<ClothiersWrath>(), DropHelper.RareVariantDropRateInt);
                        break;

                    // Wall of Flesh
                    case ItemID.WallOfFleshBossBag:
                        DropHelper.DropItemChance(player, ModContent.ItemType<Meowthrower>(), 3);
                        DropHelper.DropItemChance(player, ModContent.ItemType<BlackHawkRemote>(), 3);
                        DropHelper.DropItemChance(player, ModContent.ItemType<BlastBarrel>(), 3);
                        DropHelper.DropItemChance(player, ModContent.ItemType<RogueEmblem>(), 4);
                        DropHelper.DropItemFromSetChance(player, 0.2f, ItemID.CorruptionKey, ItemID.CrimsonKey);
                        DropHelper.DropItemCondition(player, ModContent.ItemType<MLGRune>(), !CalamityWorld.demonMode); // Demon Trophy
                        break;

                    // Destroyer
                    case ItemID.DestroyerBossBag:
                        float shpcChance = DropHelper.LegendaryDropRateFloat;
                        DropHelper.DropItemCondition(player, ModContent.ItemType<SHPC>(), CalamityWorld.revenge, shpcChance);
                        break;

                    // Plantera
                    case ItemID.PlanteraBossBag:
                        DropHelper.DropItem(player, ModContent.ItemType<LivingShard>(), 16, 22);
                        float bFluxChance = DropHelper.LegendaryDropRateFloat;
                        DropHelper.DropItemCondition(player, ModContent.ItemType<BlossomFlux>(), CalamityWorld.revenge, bFluxChance);
                        DropHelper.DropItemChance(player, ItemID.JungleKey, 5);
                        break;

                    // Golem
                    case ItemID.GolemBossBag:
                        float aegisChance = DropHelper.LegendaryDropRateFloat;
                        DropHelper.DropItemCondition(player, ModContent.ItemType<AegisBlade>(), CalamityWorld.revenge, aegisChance);
                        DropHelper.DropItem(player, ModContent.ItemType<EssenceofCinder>(), 8, 13);
						DropHelper.DropItemChance(player, ModContent.ItemType<LeadWizard>(), DropHelper.RareVariantDropRateInt);
                        break;

                    // Duke Fishron
                    case ItemID.FishronBossBag:
                        float baronChance = DropHelper.LegendaryDropRateFloat;
                        DropHelper.DropItemCondition(player, ModContent.ItemType<BrinyBaron>(), CalamityWorld.revenge, baronChance);
                        DropHelper.DropItemChance(player, ModContent.ItemType<DukesDecapitator>(), 4);
                        break;

                    // Betsy
                    case ItemID.BossBagBetsy:
                        float vesuviusChance = DropHelper.LegendaryDropRateFloat;
                        DropHelper.DropItemCondition(player, ModContent.ItemType<Vesuvius>(), CalamityWorld.revenge, vesuviusChance);
                        break;

                    // Moon Lord
                    case ItemID.MoonLordBossBag:
                        DropHelper.DropItem(player, ItemID.LunarOre, 50, 50);
                        DropHelper.DropItem(player, ModContent.ItemType<MLGRune2>()); // Celestial Onion
                        DropHelper.DropItemChance(player, ModContent.ItemType<UtensilPoker>(), 8);
                        DropHelper.DropItemChance(player, ModContent.ItemType<GrandDad>(), DropHelper.RareVariantDropRateInt);
                        DropHelper.DropItemChance(player, ModContent.ItemType<Infinity>(), DropHelper.RareVariantDropRateInt);
                        break;
                }
            }
        }
        #endregion

        #region Armor Set Changes
        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (head.type == ItemID.CopperHelmet && body.type == ItemID.CopperChainmail && legs.type == ItemID.CopperGreaves)
                return "Copper";
            if (head.type == ItemID.TinHelmet && body.type == ItemID.TinChainmail && legs.type == ItemID.TinGreaves)
                return "Tin";
            if ((head.type == ItemID.IronHelmet || head.type == ItemID.AncientIronHelmet) && body.type == ItemID.IronChainmail && legs.type == ItemID.IronGreaves)
                return "Iron";
            if (head.type == ItemID.LeadHelmet && body.type == ItemID.LeadChainmail && legs.type == ItemID.LeadGreaves)
                return "Lead";
            if (head.type == ItemID.SilverHelmet && body.type == ItemID.SilverChainmail && legs.type == ItemID.SilverGreaves)
                return "Silver";
            if (head.type == ItemID.TungstenHelmet && body.type == ItemID.TungstenChainmail && legs.type == ItemID.TungstenGreaves)
                return "Tungsten";
            if ((head.type == ItemID.GoldHelmet || head.type == ItemID.AncientGoldHelmet) && body.type == ItemID.GoldChainmail && legs.type == ItemID.GoldGreaves)
                return "Gold";
            if (head.type == ItemID.PlatinumHelmet && body.type == ItemID.PlatinumChainmail && legs.type == ItemID.PlatinumGreaves)
                return "Platinum";
            if (head.type == ItemID.GladiatorHelmet && body.type == ItemID.GladiatorBreastplate && legs.type == ItemID.GladiatorLeggings)
                return "Gladiator";
            if (head.type == ItemID.ObsidianHelm && body.type == ItemID.ObsidianShirt && legs.type == ItemID.ObsidianPants)
                return "Obsidian";
            if (head.type == ItemID.MoltenHelmet && body.type == ItemID.MoltenBreastplate && legs.type == ItemID.MoltenGreaves)
                return "Molten";
			// Normal and Pink Eskimo set can be mixed and matched
            if ((head.type == ItemID.EskimoHood || head.type == ItemID.PinkEskimoHood) && (body.type == ItemID.EskimoCoat || body.type == ItemID.PinkEskimoCoat) && (legs.type == ItemID.EskimoPants || legs.type == ItemID.PinkEskimoPants))
                return "Eskimo";
			if (head.type == ItemID.MeteorHelmet && body.type == ItemID.MeteorSuit && legs.type == ItemID.MeteorLeggings)
				return "Meteor";
			if (head.type == ItemID.PearlwoodHelmet && body.type == ItemID.PearlwoodBreastplate && legs.type == ItemID.PearlwoodGreaves)
				return "Pearlwood";
            return "";
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (set == "Copper")
                player.pickSpeed -= 0.15f;
            else if (set == "Tin")
                player.pickSpeed -= 0.1f;
            else if (set == "Iron")
                player.pickSpeed -= 0.25f;
            else if (set == "Lead")
                player.pickSpeed -= 0.2f;
            else if (set == "Silver")
                player.pickSpeed -= 0.35f;
            else if (set == "Tungsten")
                player.pickSpeed -= 0.3f;
            else if (set == "Gold")
                player.pickSpeed -= 0.45f;
            else if (set == "Platinum")
                player.pickSpeed -= 0.4f;
            else if (set == "Gladiator")
            {
                modPlayer.rogueStealthMax += 0.7f;
                modPlayer.wearingRogueArmor = true;
                player.Calamity().throwingDamage += 0.05f;
                player.Calamity().throwingVelocity += 0.1f;
                player.statDefense += 3;
                player.setBonus = "+3 defense\n" +
                            "5% increased rogue damage and 10% increased velocity\n" +
                            "Rogue stealth builds while not attacking and not moving, up to a max of 70\n" +
                            "Once you have built max stealth, you will be able to perform a Stealth Strike\n" +
                            "Rogue stealth only reduces when you attack, it does not reduce while moving\n" +
                            "The higher your rogue stealth the higher your rogue damage, crit, and movement speed";
            }
            else if (set == "Obsidian")
            {
                modPlayer.rogueStealthMax += 0.8f;
                modPlayer.wearingRogueArmor = true;
                player.Calamity().throwingDamage += 0.05f;
                player.Calamity().throwingCrit += 5;
                player.statDefense += 2;
                player.fireWalk = true;
                player.lavaMax += 180;
				string heatImmunity = CalamityWorld.death ? "\nProvides heat protection in Death Mode" : "";
				player.setBonus = "+2 defense\n" +
							"5% increased rogue damage and critical strike chance\n" +
							"Grants immunity to fire blocks and temporary immunity to lava\n" +
							"Rogue stealth builds while not attacking and not moving, up to a max of 80\n" +
							"Once you have built max stealth, you will be able to perform a Stealth Strike\n" +
							"Rogue stealth only reduces when you attack, it does not reduce while moving\n" +
							"The higher your rogue stealth the higher your rogue damage, crit, and movement speed" + heatImmunity;
            }
            else if (set == "Molten")
            {
                player.fireWalk = true;
                player.lavaMax += 300;
            }
            else if (set == "Eskimo")
            {
				modPlayer.eskimoSet = true;
				player.buffImmune[BuffID.Frostburn] = true;
				player.buffImmune[ModContent.BuffType<GlacialState>()] = true;
				string coldImmunity = CalamityWorld.death ? "\nProvides cold protection in Death Mode" : "";
				player.setBonus = "All ice-themed weapons receive a 10% damage bonus\n" +
				"Cold enemies will deal reduced contact damage to the player\n" +
				"Provides immunity to the Frostburn and Glacial State debuffs" + coldImmunity;
            }
            else if (set == "Meteor")
            {
                player.spaceGun = false;
                modPlayer.meteorSet = true;
            }
        }
        #endregion

        #region Equip Changes
        public override void UpdateEquip(Item item, Player player)
        {
            #region Head
            if (item.type == ItemID.SpectreHood)
                player.magicDamage += 0.2f;
            else if (item.type == ItemID.GladiatorHelmet || item.type == ItemID.ObsidianHelm)
                player.Calamity().throwingDamage += 0.03f;
            #endregion

            #region Body
            if (item.type == ItemID.GladiatorBreastplate || item.type == ItemID.ObsidianShirt)
                player.Calamity().throwingCrit += 3;
            #endregion

            #region Legs
            if (item.type == ItemID.GladiatorLeggings || item.type == ItemID.ObsidianPants)
                player.Calamity().throwingVelocity += 0.03f;
            #endregion
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

            if (item.type == ItemID.FireGauntlet)
            {
                player.meleeDamage += 0.04f;
                player.meleeSpeed += 0.04f;
            }

            if (item.type == ItemID.AngelWings) // Boost to max life, defense, and life regen
            {
                player.statLifeMax2 += 20;
                player.statDefense += 15;
                player.lifeRegen += 3;
                player.noFallDmg = true;
            }
            else if (item.type == ItemID.DemonWings) // Boost to all damage and crit
            {
                player.allDamage += 0.1f;
                modPlayer.AllCritBoost(10);
                player.noFallDmg = true;
            }
            else if (item.type == ItemID.FinWings) // Boosted water abilities, faster fall in water
            {
                player.moveSpeed += 0.2f;
                player.jumpSpeedBoost += 1.8f;
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
                player.statManaMax2 += 50;
                player.magicDamage += 0.1f;
                player.manaCost *= 0.95f;
                player.magicCrit += 5;
                player.noFallDmg = true;
            }
            else if (item.type == ItemID.FairyWings) // Boost to max life
            {
                player.statLifeMax2 += 80;
                player.noFallDmg = true;
            }
            else if (item.type == ItemID.BatWings) // Stronger at night
            {
                player.noFallDmg = true;
                if (!Main.dayTime || Main.eclipse)
                {
					player.jumpSpeedBoost += 1.0f;
                    player.statDefense += 15;
                    player.allDamage += 0.1f;
                    modPlayer.AllCritBoost(5);
                    player.moveSpeed += 0.1f;
                }
            }
            else if (item.type == ItemID.HarpyWings)
            {
				modPlayer.harpyWingBoost = true;
                player.moveSpeed += 0.3f;
                player.noFallDmg = true;
            }
            else if (item.type == ItemID.BoneWings) // Bonus to ranged and defense stats while wearing necro armor
            {
                player.noFallDmg = true;
                if ((player.head == ArmorIDs.Head.NecroHelmet || player.head == ArmorIDs.Head.AncientNecroHelmet) &&
                    player.body == ArmorIDs.Body.NecroBreastplate && player.legs == ArmorIDs.Legs.NecroGreaves)
                {
                    player.moveSpeed += 0.15f;
                    player.rangedDamage += 0.12f;
                    player.rangedCrit += 16;
                    player.statDefense += 30;
                }
            }
            else if (item.type == ItemID.MothronWings) // Spawn baby mothrons over time to attack enemies, max of 3
            {
                player.statDefense += 5;
                player.allDamage += 0.05f;
                player.moveSpeed += 0.1f;
                player.jumpSpeedBoost += 1.2f;
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
                player.meleeDamage += 0.1f;
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
                        player.statManaMax2 += 20;
                        player.magicDamage += 0.05f;
                        player.manaCost *= 0.95f;
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
                        player.statDefense += 15;
                        player.endurance += 0.1f;
                    }
                    else if (player.body == ArmorIDs.Body.BeetleScaleMail)
                    {
                        player.meleeDamage += 0.1f;
                        player.meleeCrit += 10;
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
					player.statDefense += 10;
                    player.endurance += 0.1f;
					player.AddBuff(BuffID.DryadsWard, 5, true); // Dryad's Blessing
                }
            }
            else if (item.type == ItemID.FestiveWings) // Drop powerful homing christmas tree bulbs while in flight
            {
                player.noFallDmg = true;
                player.statLifeMax2 += 50;
				if (modPlayer.icicleCooldown <= 0)
				{
					if (player.controlJump && !player.jumpAgainCloud && player.jump == 0 && player.velocity.Y != 0f && !player.mount.Active && !player.mount.Cart)
					{
						int p = Projectile.NewProjectile(player.Center.X, player.Center.Y, player.velocity.X * 0f, 2f, ProjectileID.OrnamentFriendly, (int)(100 * player.AverageDamage()), 5f, player.whoAmI);
						Main.projectile[p].Calamity().forceTypeless = true;
						Main.projectile[p].Calamity().lineColor = 1;
						modPlayer.icicleCooldown = 10;
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

            if (item.type == ItemID.WormScarf)
                player.endurance -= 0.07f;

            if (item.type == ItemID.RoyalGel)
                modPlayer.royalGel = true;

            if (item.type == ItemID.HandWarmer)
                modPlayer.handWarmer = true;

			// Hard / Guarding / Armored / Warding give 0.25% / 0.5% / 0.75% / 1% DR
			if (item.prefix == PrefixID.Hard)
			{
				if (NPC.downedMoonlord)
					player.statDefense += 2;
				else if (Main.hardMode)
					player.statDefense += 1;

				player.endurance += 0.0025f;
			}
			if (item.prefix == PrefixID.Guarding)
			{
				if (NPC.downedMoonlord)
					player.statDefense += 2;
				else if (Main.hardMode)
					player.statDefense += 1;

				player.endurance += 0.005f;
			}
			if (item.prefix == PrefixID.Armored)
			{
				if (NPC.downedMoonlord)
					player.statDefense += 3;
				else if (Main.hardMode)
					player.statDefense += 1;

				player.endurance += 0.0075f;
			}
			if (item.prefix == PrefixID.Warding)
			{
				if (NPC.downedMoonlord)
					player.statDefense += 4;
				else if (Main.hardMode)
					player.statDefense += 2;

				player.endurance += 0.01f;
			}

            // Precise only gives 1% crit and Lucky only gives 3% crit
            if ((item.prefix == PrefixID.Precise) || (item.prefix == PrefixID.Lucky))
            {
                player.meleeCrit -= 1;
                player.rangedCrit -= 1;
                player.magicCrit -= 1;
                player.thrownCrit -= 1;
            }
        }
        #endregion

        #region WingChanges
        public override void HorizontalWingSpeeds(Item item, Player player, ref float speed, ref float acceleration)
        {
            CalamityPlayer modPlayer = player.Calamity();
            float flightSpeedMult = 1f +
                (modPlayer.soaring ? 0.1f : 0f) +
                (modPlayer.holyWrath ? 0.05f : 0f) +
                (modPlayer.profanedRage ? 0.05f : 0f) +
                (modPlayer.draconicSurge ? 0.15f : 0f) +
                (modPlayer.etherealExtorter && modPlayer.ZoneAstral ? 0.05f : 0f);
            if (flightSpeedMult > 1.2f)
                flightSpeedMult = 1.2f;

            speed *= flightSpeedMult;

            float flightAccMult = 1f +
                (modPlayer.draconicSurge ? 0.15f : 0f);
            if (flightAccMult > 1.2f)
                flightAccMult = 1.2f;

            acceleration *= flightAccMult;
        }
        #endregion

        #region GrabChanges
        public override void GrabRange(Item item, Player player, ref int grabRange)
        {
            CalamityPlayer modPlayer = player.Calamity();
            int itemGrabRangeBoost = 0 +
                (modPlayer.wallOfFleshLore ? 10 : 0) +
                (modPlayer.planteraLore ? 20 : 0) +
                (modPlayer.polterghastLore ? 30 : 0);

            grabRange += itemGrabRangeBoost;
        }
        #endregion

        #region The Horseman's Blade
        public static void HorsemansBladeOnHit(Player player, int targetIdx, int damage, float knockback, bool hasExtraUpdates)
        {
            int x = Main.rand.Next(100, 300);
            int y = Main.rand.Next(100, 300);

            // Pick a random side: left or right
            if (Main.rand.NextBool(2))
                x -= Main.LogicCheckScreenWidth / 2 + x;
            else
                x += Main.LogicCheckScreenWidth / 2 - x;

            // Pick a random side: top or bottom
            if (Main.rand.NextBool(2))
                y -= Main.LogicCheckScreenHeight / 2 + y;
            else
                y += Main.LogicCheckScreenHeight / 2 - y;

            x += (int)player.position.X;
            y += (int)player.position.Y;
            float speed = 8f;
            Vector2 vector = new Vector2((float)x, (float)y);
            float dx = Main.npc[targetIdx].position.X - vector.X;
            float dy = Main.npc[targetIdx].position.Y - vector.Y;
            float dist = (float)Math.Sqrt(dx * dx + dy * dy);
            dist = speed / dist;
            dx *= dist;
            dy *= dist;
            int projectile = Projectile.NewProjectile(x, y, dx, dy, ProjectileID.FlamingJack, damage, knockback, player.whoAmI, targetIdx, 0f);
            if (hasExtraUpdates)
                Main.projectile[projectile].extraUpdates += 1;
        }
        #endregion

        #region Consume Additional Ammo
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
			{
				CalamityUtils.ForceItemIntoWorld(item);
			}
		}
		#endregion

        #region Goblin Money Theft (PostReforge)
        public override void PostReforge(Item item)
        {
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
        public static readonly int Rarity5BuyPrice = Item.buyPrice(0, 36, 0, 0);
        public static readonly int Rarity6BuyPrice = Item.buyPrice(0, 48, 0, 0);
        public static readonly int Rarity7BuyPrice = Item.buyPrice(0, 60, 0, 0);
        public static readonly int Rarity8BuyPrice = Item.buyPrice(0, 80, 0, 0);
        public static readonly int Rarity9BuyPrice = Item.buyPrice(0, 95, 0, 0);
        public static readonly int Rarity10BuyPrice = Item.buyPrice(1, 0, 0, 0);
        public static readonly int RarityTurquoiseBuyPrice = Item.buyPrice(1, 20, 0, 0);
        public static readonly int RarityPureGreenBuyPrice = Item.buyPrice(1, 40, 0, 0);
        public static readonly int RarityDarkBlueBuyPrice = Item.buyPrice(1, 80, 0, 0);
        public static readonly int RarityVioletBuyPrice = Item.buyPrice(2, 50, 0, 0);
        public static readonly int RarityHotPinkBuyPrice = Item.buyPrice(5, 0, 0, 0);

		//These duplicates are for my sanity...
        public static readonly int Rarity12BuyPrice = Item.buyPrice(1, 20, 0, 0);
        public static readonly int Rarity13BuyPrice = Item.buyPrice(1, 40, 0, 0);
        public static readonly int Rarity14BuyPrice = Item.buyPrice(1, 80, 0, 0);
        public static readonly int Rarity15BuyPrice = Item.buyPrice(2, 50, 0, 0);
        public static readonly int Rarity16BuyPrice = Item.buyPrice(5, 0, 0, 0);

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
                case (int)CalamityRarity.Turquoise:
                    return RarityTurquoiseBuyPrice;
                case (int)CalamityRarity.PureGreen:
                    return RarityPureGreenBuyPrice;
                case (int)CalamityRarity.DarkBlue:
                    return RarityDarkBlueBuyPrice;
                case (int)CalamityRarity.Violet:
                    return RarityVioletBuyPrice;
                case (int)CalamityRarity.Developer:
                    return RarityHotPinkBuyPrice;
            }
            return 0;
        }
        public static int GetBuyPrice(Item item)
        {
            return GetBuyPrice(item.rare);
        }
        #endregion

        /// <summary>
        /// Dust helper to spawn dust for an item. Allows you to specify where on the item to spawn the dust, essentially. (ONLY WORKS FOR SWINGING WEAPONS?)
        /// </summary>
        /// <param name="player">The player using the item.</param>
        /// <param name="dustType">The type of dust to use.</param>
        /// <param name="chancePerFrame">The chance per frame to spawn the dust (0f-1f)</param>
        /// <param name="minDistance">The minimum distance between the player and the dust</param>
        /// <param name="maxDistance">The maximum distance between the player and the dust</param>
        /// <param name="minRandRot">The minimum random rotation offset for the dust</param>
        /// <param name="maxRandRot">The maximum random rotation offset for the dust</param>
        /// <param name="minSpeed">The minimum speed that the dust should travel</param>
        /// <param name="maxSpeed">The maximum speed that the dust should travel</param>
        public static Dust MeleeDustHelper(Player player, int dustType, float chancePerFrame, float minDistance, float maxDistance, float minRandRot = -0.2f, float maxRandRot = 0.2f, float minSpeed = 0.9f, float maxSpeed = 1.1f)
        {
            if (Main.rand.NextFloat(1f) < chancePerFrame)
            {
                //Calculate values
                //distance from player,
                //the vector offset from the player center
                //the vector between the pos and the player
                float distance = Main.rand.NextFloat(minDistance, maxDistance);
                Vector2 offset = (player.itemRotation - (MathHelper.PiOver4 * player.direction) + Main.rand.NextFloat(minRandRot, maxRandRot)).ToRotationVector2() * distance * player.direction;
                Vector2 pos = player.Center + offset;
                Vector2 vec = pos - player.Center;
                //spawn the dust
                Dust d = Dust.NewDustPerfect(pos, dustType);
                //normalise vector and multiply by velocity magnitude
                vec.Normalize();
                d.velocity = vec * Main.rand.NextFloat(minSpeed, maxSpeed);
                return d;
            }
            return null;
        }
    }
}
