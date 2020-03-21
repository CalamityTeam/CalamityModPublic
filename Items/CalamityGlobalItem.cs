using CalamityMod.Buffs.Potions;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.DifficultyItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Potions;
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
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
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

        public int timesUsed = 0;

        // Rarity is provided both as the classic int and the new enum.
        public CalamityRarity customRarity = CalamityRarity.NoEffect;
        public int postMoonLordRarity 
        {
            get => (int)customRarity;
            set => customRarity = (CalamityRarity)value;
        }

        #region SetDefaults
        public override void SetDefaults(Item item)
        {
            if (customRarity.IsPostML() && item.rare != 10)
                item.rare = 10;

            if (item.maxStack == 99 || item.type == ItemID.Dynamite || item.type == ItemID.StickyDynamite ||
                item.type == ItemID.BouncyDynamite || item.type == ItemID.StickyBomb || item.type == ItemID.BouncyBomb)
                item.maxStack = 999;

            if (item.type == ItemID.PirateMap || item.type == ItemID.SnowGlobe)
                item.maxStack = 20;

            if (item.type >= ItemID.GreenSolution && item.type <= ItemID.RedSolution)
                item.value = Item.buyPrice(0, 0, 5, 0);

            if (CalamityMod.weaponAutoreuseList?.Contains(item.type) ?? false)
                item.autoReuse = true;

            if (item.type == ItemID.PsychoKnife)
                item.damage *= 4;
            else if (item.type == ItemID.SpectreStaff)
                item.damage *= 3;
            else if (CalamityMod.doubleDamageBuffList?.Contains(item.type) ?? false)
                item.damage *= 2;
            else if (item.type == ItemID.RainbowRod)
                item.damage = (int)((double)item.damage * 1.75);
            else if (CalamityMod.sixtySixDamageBuffList?.Contains(item.type) ?? false)
                item.damage = (int)((double)item.damage * 1.66);
            else if (CalamityMod.fiftyDamageBuffList?.Contains(item.type) ?? false)
                item.damage = (int)((double)item.damage * 1.5);
            else if (CalamityMod.thirtyThreeDamageBuffList?.Contains(item.type) ?? false)
                item.damage = (int)((double)item.damage * 1.33);
            else if (CalamityMod.twentyFiveDamageBuffList?.Contains(item.type) ?? false)
                item.damage = (int)((double)item.damage * 1.25);
            else if (CalamityMod.twentyDamageBuffList?.Contains(item.type) ?? false)
                item.damage = (int)((double)item.damage * 1.2);
            else if (item.type == ItemID.Frostbrand || item.type == ItemID.MagnetSphere)
                item.damage = (int)((double)item.damage * 1.1);
            else if (item.type == ItemID.Razorpine)
                item.damage = (int)((double)item.damage * 0.95);
            else if (item.type == ItemID.Phantasm)
                item.damage = (int)((double)item.damage * 0.9);
            else if (item.type == ItemID.LastPrism)
                item.damage = (int)((double)item.damage * 0.85);
            else if (CalamityMod.quarterDamageNerfList?.Contains(item.type) ?? false)
                item.damage = (int)((double)item.damage * 0.75);
            else if (item.type == ItemID.StardustDragonStaff)
                item.damage = (int)((double)item.damage * 0.5);

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
            else if (item.type == ItemID.GladiatorHelmet) //total defense pre-buff = 7 post-buff = 21
                item.defense = 4; //2 more defense
            else if (item.type == ItemID.GladiatorBreastplate)
                item.defense = 7; //4 more defense
            else if (item.type == ItemID.GladiatorLeggings)
                item.defense = 5; //3 more defense
            else if (item.type == ItemID.HallowedPlateMail) //total defense pre-buff = 31, 50, 35 post-buff = 36, 55, 40
                item.defense = 18; //3 more defense
            else if (item.type == ItemID.HallowedGreaves)
                item.defense = 13; //2 more defense

			if (CalamityMod.noGravityList.Contains(item.type))
				ItemID.Sets.ItemNoGravity[item.type] = true;
			if (CalamityMod.lavaFishList.Contains(item.type))
				ItemID.Sets.CanFishInLava[item.type] = true;

			// not expert because ML drops it in normal so that it can be used with the lore item
            if (item.type == ItemID.GravityGlobe)
			{
				item.expert = false;
				item.rare = 10;
			}
            
            if(item.type == ItemID.SuspiciousLookingTentacle)
                item.expert = true;
        }
        #endregion

        #region Shoot
        public override bool Shoot(Item item, Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (rogue)
            {
                speedX *= player.Calamity().throwingVelocity;
                speedY *= player.Calamity().throwingVelocity;
                if (player.Calamity().gloveOfRecklessness)
                {
                    Vector2 rotated = new Vector2(speedX, speedY);
                    rotated = rotated.RotatedByRandom(MathHelper.ToRadians(10f));
                    speedX = rotated.X;
                    speedY = rotated.Y;
                }
            }
            if (player.Calamity().luxorsGift)
            {
                // useTime 9 = 0.9 useTime 2 = 0.2
                double damageMult = 1.0;
                if (item.useTime < 10)
                    damageMult -= (double)(10 - item.useTime) / 10.0;

                double newDamage = (double)damage * damageMult;

                if (player.whoAmI == Main.myPlayer)
                {
                    if (item.melee)
                        Projectile.NewProjectile(position.X, position.Y, speedX * 0.5f, speedY * 0.5f, ModContent.ProjectileType<LuxorsGiftMelee>(), (int)(newDamage * 0.6), 0f, player.whoAmI, 0f, 0f);

                    else if (rogue)
                        Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<LuxorsGiftRogue>(), (int)(newDamage * 0.5), 0f, player.whoAmI, 0f, 0f);

                    else if (item.ranged)
                        Projectile.NewProjectile(position.X, position.Y, speedX * 1.5f, speedY * 1.5f, ModContent.ProjectileType<LuxorsGiftRanged>(), (int)(newDamage * 0.4), 0f, player.whoAmI, 0f, 0f);

                    else if (item.magic)
                        Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<LuxorsGiftMagic>(), (int)(newDamage * 0.8), 0f, player.whoAmI, 0f, 0f);

                    else if (item.summon && player.ownedProjectileCounts[ModContent.ProjectileType<LuxorsGiftSummon>()] < 1)
                        Projectile.NewProjectile(position.X, position.Y, 0f, 0f, ModContent.ProjectileType<LuxorsGiftSummon>(), damage, 0f, player.whoAmI, 0f, 0f);
                }
            }
            if (player.Calamity().eArtifact && item.ranged && !rogue)
            {
                speedX *= 1.25f;
                speedY *= 1.25f;
            }
            if (player.Calamity().bloodflareMage) //0 - 99
            {
                if (item.magic && Main.rand.Next(0, 100) >= 95)
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<GhostlyBolt>(), (int)((double)damage * (player.Calamity().auricSet ? 4.2 : 2.6)), 1f, player.whoAmI, 0f, 0f);
                    }
                }
            }
            if (player.Calamity().bloodflareRanged) //0 - 99
            {
                if (item.ranged && !rogue && Main.rand.Next(0, 100) >= 98)
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<BloodBomb>(), (int)((double)damage * (player.Calamity().auricSet ? 2.2 : 1.6)), 2f, player.whoAmI, 0f, 0f);
                    }
                }
            }
            if (player.Calamity().tarraMage)
            {
                if (player.Calamity().tarraCrits >= 5 && player.whoAmI == Main.myPlayer)
                {
                    player.Calamity().tarraCrits = 0;
                    int num106 = 9 + Main.rand.Next(3);
                    for (int num107 = 0; num107 < num106; num107++)
                    {
                        float num110 = 0.025f * (float)num107;
                        float hardar = speedX + (float)Main.rand.Next(-25, 26) * num110;
                        float hordor = speedY + (float)Main.rand.Next(-25, 26) * num110;
                        float num84 = (float)Math.Sqrt((double)(speedX * speedX + speedY * speedY));
                        num84 = item.shootSpeed / num84;
                        hardar *= num84;
                        hordor *= num84;
                        Projectile.NewProjectile(position.X, position.Y, hardar, hordor, 206, (int)((double)damage * 0.2), knockBack, player.whoAmI, 0.0f, 0.0f);
                    }
                }
            }
            if (player.Calamity().ataxiaBolt)
            {
                if (item.ranged && !rogue && Main.rand.NextBool(2))
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        Projectile.NewProjectile(position.X, position.Y, speedX * 1.25f, speedY * 1.25f, ModContent.ProjectileType<ChaosFlare>(), (int)((double)damage * 0.25), 2f, player.whoAmI, 0f, 0f);
                    }
                }
            }
            if (player.Calamity().godSlayerRanged) //0 - 99
            {
                if (item.ranged && !rogue && Main.rand.Next(0, 100) >= 95)
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        Projectile.NewProjectile(position.X, position.Y, speedX * 1.25f, speedY * 1.25f, ModContent.ProjectileType<GodSlayerShrapnelRound>(), (int)((double)damage * (player.Calamity().auricSet ? 3.2 : 2.1)), 2f, player.whoAmI, 0f, 0f);
                    }
                }
            }
            if (player.Calamity().ataxiaVolley)
            {
                if (rogue && Main.rand.NextBool(10))
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 20);
                        float spread = 45f * 0.0174f;
                        double startAngle = Math.Atan2(player.velocity.X, player.velocity.Y) - spread / 2;
                        double deltaAngle = spread / 8f;
                        double offsetAngle;
                        int i;
                        for (i = 0; i < 4; i++)
                        {
                            Vector2 vector2 = new Vector2(player.Center.X, player.Center.Y);
                            offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                            Projectile.NewProjectile(vector2.X, vector2.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<ChaosFlare2>(), (int)((double)damage * 0.5), 1.25f, player.whoAmI, 0f, 0f);
                            Projectile.NewProjectile(vector2.X, vector2.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<ChaosFlare2>(), (int)((double)damage * 0.5), 1.25f, player.whoAmI, 0f, 0f);
                        }
                    }
                }
            }
            if (player.Calamity().reaverDoubleTap) //0 - 99
            {
                if (item.ranged && !rogue && Main.rand.Next(0, 100) >= 90)
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        Projectile.NewProjectile(position.X, position.Y, speedX * 1.25f, speedY * 1.25f, ModContent.ProjectileType<MiniRocket>(), (int)((double)damage * 1.3), 2f, player.whoAmI, 0f, 0f);
                    }
                }
            }
            if (player.Calamity().victideSet)
            {
                if ((item.ranged || item.melee || item.magic ||
                    rogue || item.summon) && item.rare < 8 && Main.rand.NextBool(10))
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        Projectile.NewProjectile(position.X, position.Y, speedX * 1.25f, speedY * 1.25f, ModContent.ProjectileType<Seashell>(), damage * 2, 1f, player.whoAmI, 0f, 0f);
                    }
                }
            }
            if (player.Calamity().dynamoStemCells)
            {
                if (item.ranged && !rogue && Main.rand.Next(0, 100) >= 80)
                {
					double damageMult = 1.0;
					damageMult = (double)(item.useTime) / 30.0;
					if (damageMult < 0.35)
						damageMult = 0.35;

					double newDamage = (double)damage * 2 * damageMult;

                    if (player.whoAmI == Main.myPlayer)
                    {
                        Projectile.NewProjectile(position.X, position.Y, speedX * 1.25f, speedY * 1.25f, ModContent.ProjectileType<Minibirb>(), (int)newDamage, 2f, player.whoAmI, 0f, 0f);
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
				Main.PlaySound(SoundID.Item11.WithPitchVariance(0.05f), position); // <--- This is optional; if using, add "item.UseSound = null" to GlobalItem.SetDefaults when checking for the Star Cannon's ID
				return false;
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
                {
                    "rogue", rogue
                },
                {
                    "timesUsed", timesUsed
                },
                {
                    "rarity", (int)customRarity
                }
            };
        }

        public override void Load(Item item, TagCompound tag)
        {
            rogue = tag.GetBool("rogue");
            timesUsed = tag.GetInt("timesUsed");
            customRarity = (CalamityRarity)tag.GetInt("rarity");
        }

        public override void LoadLegacy(Item item, BinaryReader reader)
        {
            int loadVersion = reader.ReadInt32();
            customRarity = (CalamityRarity)reader.ReadInt32();
            timesUsed = reader.ReadInt32();

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
        }

        public override void NetReceive(Item item, BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            rogue = flags[0];

            customRarity = (CalamityRarity)reader.ReadInt32();
            timesUsed = reader.ReadInt32();
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
                bool enrage = player.statLife <= (int)((double)player.statLifeMax2 * 0.5);
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

                                Projectile.NewProjectile(player.Center.X, player.Center.Y - 10, perturbedspeed.X, perturbedspeed.Y, ModContent.ProjectileType<ProfanedCrystalMeleeSpear>(), (int)((shouldNerf ? 1000 : 1750) * (player.allDamage + player.minionDamage - 1f)), 1f, player.whoAmI, Main.rand.NextBool(player.Calamity().profanedSoulWeaponUsage == 4 ? 5 : 7) ? 1f : 0f);
                                spread -= Main.rand.Next(2, 4);
                                Main.PlaySound(SoundID.Item20, player.Center);
                            }
                            player.Calamity().profanedSoulWeaponUsage = 0;
                        }
                        else
                        {
                            Projectile.NewProjectile(player.Center, correctedVelocity * 6.9f, ModContent.ProjectileType<ProfanedCrystalMeleeSpear>(), (int)((shouldNerf ? 500 : 1250) * (player.allDamage + player.minionDamage - 1f)), 1f, player.whoAmI, Main.rand.NextBool(player.Calamity().profanedSoulWeaponUsage == 4 ? 5 : 7) ? 1f : 0f, 1f);
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
                        int dam = (int)((shouldNerf ? 500 : 1000) * (player.allDamage + player.minionDamage - 1f));
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
                    int manaCost = (int)((float)100 * player.manaCost);
                    if (player.statMana < manaCost && player.Calamity().profanedSoulWeaponUsage == 0)
                    {
                        if (player.manaFlower)
                        {
                            player.QuickMana();
                        }
                    }
                    if (player.statMana >= manaCost && player.Calamity().profanedSoulWeaponUsage == 0)
                    {
                        player.manaRegenDelay = (int)player.maxRegenDelay;
                        player.statMana -= manaCost;
                        correctedVelocity *= 25f;
                        Main.PlaySound(SoundID.Item20, player.Center);
                        int dam = (int)((shouldNerf ? 1800 : 4500) * (player.allDamage + player.minionDamage - 1f));
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
                    if (player.Calamity().profanedSoulWeaponUsage >= (enrage ? 120 : 180))
                    {
                        float crystalCount = 36f;
                        for (float i = 0; i < crystalCount; i++)
                        {
                            float angle = MathHelper.TwoPi / crystalCount * i;
                            int proj = Projectile.NewProjectile(player.Center, angle.ToRotationVector2() * 8f, ModContent.ProjectileType<ProfanedCrystalRogueShard>(), (int)((shouldNerf ? 169 : 880) * (player.allDamage + player.minionDamage - 1f)), 1f, player.whoAmI, 0f, 0f);
                            Main.projectile[proj].Calamity().forceMinion = true;
                            Main.PlaySound(SoundID.Item20, player.Center);
                        }
                        player.Calamity().profanedSoulWeaponUsage = 0;
                    }
                    else if (player.Calamity().profanedSoulWeaponUsage % (enrage ? 5 : 10) == 0)
                    {
                        float angle = MathHelper.TwoPi / (enrage ? 9 : 18) * (player.Calamity().profanedSoulWeaponUsage / (enrage ? 1 : 10));
                        int proj = Projectile.NewProjectile(player.Center, angle.ToRotationVector2() * 8f, ModContent.ProjectileType<ProfanedCrystalRogueShard>(), (int)((shouldNerf ? 220 : 1100) * (player.allDamage + player.minionDamage - 1f)), 1f, player.whoAmI, 1f, 0f);
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

        #region Use Item Changes

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
            if (player.HeldItem.type == ModContent.ItemType<IgneousExaltation>())
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
                            Main.projectile[i].velocity = Main.projectile[i].DirectionTo(Main.MouseWorld) * 16f;
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
            return base.AltFunctionUse(item, player);
        }

        public override bool CanUseItem(Item item, Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
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
                for (int i = 0; i < 1000; ++i)
                {
                    if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == item.shoot)
                    {
                        return false;
                    }
                }
                return true;
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
					if (!Collision.SolidCollision(teleportLocation, player.width, player.height))
					{
						if (modPlayer.scarfCooldown)
							player.AddBuff(BuffID.ChaosState, CalamityPlayer.chaosStateDuration * 2, true);
						else
							player.AddBuff(BuffID.ChaosState, CalamityPlayer.chaosStateDuration, true);
					}
				}
			}
            return true;
        }
        #endregion

        #region Modify Tooltips
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine tt2 = tooltips.FirstOrDefault(x => x.Name == "ItemName" && x.mod == "Terraria");
            if (tt2 != null)
            {
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
                    case CalamityRarity.RareVariant:
                        tt2.overrideColor = new Color(255, 140, 0);
                        break;
                    case CalamityRarity.Dedicated:
                        tt2.overrideColor = new Color(139, 0, 0);
                        break;

                    case CalamityRarity.ItemSpecific:
                        // Uniquely colored developer weapons
                        if(item.type == ModContent.ItemType<Fabstaff>())
                            tt2.overrideColor = new Color(Main.DiscoR, 100, 255);
                        if(item.type == ModContent.ItemType<BlushieStaff>())
                            tt2.overrideColor = new Color(0, 0, 255);
                        if (item.type == ModContent.ItemType<NanoblackReaperMelee>() || item.type == ModContent.ItemType<NanoblackReaperRogue>())
                            tt2.overrideColor = new Color(0.34f, 0.34f + 0.66f * Main.DiscoG / 255f, 0.34f + 0.5f * Main.DiscoG / 255f);
                        if (item.type == ModContent.ItemType<ProfanedSoulCrystal>())
                            tt2.overrideColor = new Color(255 - Main.DiscoG < 80 ? 80 : Main.DiscoG < 50 ? 255 : 255 - Main.DiscoG, Main.DiscoG < 126 ? 126 : Main.DiscoG, 0); //alternates between emerald green and amber (BanditHueh)

                        // Uniquely colored legendary weapons  and Yharim's Crystal
                        if (item.type == ModContent.ItemType<AegisBlade>() || item.type == ModContent.ItemType<YharimsCrystal>())
                            tt2.overrideColor = new Color(255, Main.DiscoG, 53);
                        if (item.type == ModContent.ItemType<BlossomFlux>())
                            tt2.overrideColor = new Color(Main.DiscoR, 203, 103);
                        if (item.type == ModContent.ItemType<BrinyBaron>())
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
                            tt2.overrideColor = new Color(255, 255, Main.DiscoB);
                        break;
                }
            }

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

			if (item.type == ItemID.RodofDiscord)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Teleports you to the position of the mouse\n" +
							"Teleportation is disabled while Chaos State is active";
					}
				}
			}
			if (item.type == ItemID.SuperAbsorbantSponge)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Capable of soaking up an endless amount of water\n" +
                            "Cannot be used in the Abyss";
                    }
                }
            }
            if (item.type == ItemID.EmptyBucket)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Defense")
                    {
                        line2.text = "1 defense\n" +
                            "Cannot be used in the Abyss";
                    }
                }
            }
            if (item.type == ItemID.CrimsonHeart)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Summons a heart to provide light\n" +
                            "Provides a small amount of light in the abyss";
                    }
                }
            }
            if (item.type == ItemID.ShadowOrb)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Creates a magical shadow orb\n" +
                            "Provides a small amount of light in the abyss";
                    }
                }
            }
            if (item.type == ItemID.MagicLantern)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Summons a magic lantern that exposes nearby treasure\n" +
                            "Provides a small amount of light in the abyss";
                    }
                }
            }
            if (item.type == ItemID.ArcticDivingGear)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip1")
                    {
                        line2.text = "Provides light under water and extra mobility on ice\n" +
                            "Provides a small amount of light in the abyss\n" +
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
                        line2.text = "Provides light under water\n" +
                            "Provides a small amount of light in the abyss";
                    }
                }
            }
            if (item.type == ItemID.JellyfishDivingGear)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip1")
                    {
                        line2.text = "Provides light under water\n" +
                            "Provides a small amount of light in the abyss";
                    }
                }
            }
            if (item.type == ItemID.FairyBell)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Summons a magical fairy\n" +
                            "Provides a moderate amount of light in the abyss";
                    }
                }
            }
            if (item.type == ItemID.DD2PetGhost)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Summons a pet flickerwick to provide light\n" +
                            "Provides a moderate amount of light in the abyss";
                    }
                }
            }
            if (item.type == ItemID.ShinePotion)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "BuffTime")
                    {
                        line2.text = "5 minute duration\n" +
                            "Provides a moderate amount of light in the abyss";
                    }
                }
            }
            if (item.type == ItemID.WispinaBottle)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Summons a Wisp to provide light\n" +
                            "Provides a large amount of light in the abyss";
                    }
                }
            }
            if (item.type == ItemID.SuspiciousLookingTentacle)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip1")
                    {
                        line2.text = "'I know what you're thinking...'\n" +
                            "Provides a large amount of light in the abyss";
                    }
                }
            }
            if (item.type == ItemID.GillsPotion)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "BuffTime")
                    {
                        line2.text = "2 minute duration\n" +
                            "Greatly reduces breath loss in the abyss";
                    }
                }
            }
            if (item.type == ItemID.DivingHelmet)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Greatly extends underwater breathing\n" +
                            "Moderately reduces breath loss in the abyss";
                    }
                }
            }
            if (item.type == ItemID.NeptunesShell)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Transforms the holder into merfolk when entering water\n" +
                            "Greatly reduces breath loss in the abyss";
                    }
                }
            }
            if (item.type == ItemID.MoonShell)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Turns the holder into a werewolf at night and a merfolk when entering water\n" +
                            "Greatly reduces breath loss in the abyss";
                    }
                }
            }
            if (item.type == ItemID.CelestialShell)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip1")
                    {
                        line2.text = "Minor increases to all stats\n" +
                            "Greatly reduces breath loss in the abyss";
                    }
                }
            }
            if (item.type == ItemID.WarmthPotion)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Reduces damage from cold sources\n" +
                            "Makes you immune to the Chilled, Frozen, and Glacial State debuffs\n" +
							"Provides cold protection in Death Mode";
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
            if (item.type == ItemID.FireGauntlet)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip1")
                    {
                        line2.text = "14% increased melee damage and speed\n" +
							"Provides heat and cold protection in Death Mode";
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
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Provides immunity to direct damage from touching lava\n" +
                            "Provides temporary immunity to lava burn damage\n" +
                            "Reduces lava burn damage\n" +
							"Provides heat protection in Death Mode";
                    }
                }
            }
            if (item.type == ItemID.ObsidianRose)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Reduced direct damage from touching lava\n" +
                            "Greatly reduces lava burn damage\n" +
							"Provides heat protection in Death Mode";
                    }
                }
            }
            if (item.type == ItemID.MagmaStone)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Inflicts fire damage on attack\n" +
							"Provides heat and cold protection in Death Mode";
                    }
                }
            }
            if (item.type == ItemID.LavaCharm)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Provides 7 seconds of immunity to lava\n" +
							"Provides heat protection in Death Mode";
                    }
                }
            }
            if (item.type == ItemID.LavaWaders)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip1")
                    {
                        line2.text = "Grants immunity to fire blocks and 7 seconds of immunity to lava\n" +
							"Provides heat protection in Death Mode";
                    }
                }
            }
            if (item.type == ItemID.InvisibilityPotion)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Grants invisibility\n" +
                            "Boosts certain stats when holding certain types of rogue weapons";
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
                        line2.text = "Capable of mining Lihzahrd Bricks\n" +
                            "Also capable of mining Chaotic Ore found in the Abyss";
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
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "SetBonus")
                    {
                        line2.text = "Set Bonus: Double tap " + (Main.ReversedUpDownArmorSetBonuses ? "UP" : "DOWN") + " to call an ancient storm to the cursor location\n" +
							"Minions deal full damage while wielding weaponry";
                    }
                }
            }
            if (item.type == ItemID.GladiatorHelmet)
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
            if (item.type == ItemID.GladiatorBreastplate)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Defense")
                    {
                        line2.text = "7 defense\n" +
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
                        line2.text = "5 defense\n" +
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
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "SetBonus")
                    {
                        line2.text = @"Set Bonus: 17% extra melee damage
Grants immunity to fire blocks, and temporary immunity to lava
Provides heat and cold protection in Death Mode";
                    }
                }
            }
            if (item.type == ItemID.FrostHelmet || item.type == ItemID.FrostBreastplate || item.type == ItemID.FrostLeggings)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "SetBonus")
                    {
                        line2.text = @"Set Bonus: Melee and ranged attacks cause frostburn
Provides heat and cold protection in Death Mode";
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
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Provides immunity to chilling and freezing effects\n" +
							"Provides a regeneration boost while wearing the Eskimo armor";
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
                            "30% increased movement speed";
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
                            "20% increased movement speed and 180% increased jump speed";
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
                            "+10 defense, 10% increased movement speed,\n" +
                            "and 5% increased damage and critical strike chance";
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
							"10% increased movement speed, 100% increased jump speed,\n" +
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
                            "+1 max minion and 5% increased minion damage while wearing the Spooky Armor";
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
                            "+50 max life";
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
                            "+10 defense, 10% increased damage, " +
                            "5% increased critical strike chance, " +
                            "10% increased movement speed and 120% increased jump speed";
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
                if (item.prefix == 67)
                {
                    foreach (TooltipLine line2 in tooltips)
                    {
                        if (line2.mod == "Terraria" && line2.Name == "PrefixAccCritChance")
                        {
                            line2.text = "+1% critical strike chance";
                        }
                    }
                }
                if (item.prefix == 68)
                {
                    foreach (TooltipLine line2 in tooltips)
                    {
                        if (line2.mod == "Terraria" && line2.Name == "PrefixAccCritChance")
                        {
                            line2.text = "+3% critical strike chance";
                        }
                    }
                }
                if (item.prefix == 62)
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
                if (item.prefix == 63)
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
                if (item.prefix == 64)
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
                if (item.prefix == 65)
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
                        DropHelper.DropItemFromSetChance(player, 5, ItemID.CorruptionKey, ItemID.CrimsonKey);
                        DropHelper.DropItemCondition(player, ModContent.ItemType<MLGRune>(), !CalamityWorld.demonMode); // Demon Trophy
                        break;

                    // Destroyer
                    case ItemID.DestroyerBossBag:
                        float shpcChance = DropHelper.LegendaryDropRateFloat;
                        DropHelper.DropItemCondition(player, ModContent.ItemType<SHPC>(), CalamityWorld.revenge, shpcChance);
                        break;

                    // Plantera
                    case ItemID.PlanteraBossBag:
                        DropHelper.DropItem(player, ModContent.ItemType<LivingShard>(), 8, 11);
                        float bFluxChance = DropHelper.LegendaryDropRateFloat;
                        DropHelper.DropItemCondition(player, ModContent.ItemType<BlossomFlux>(), CalamityWorld.revenge, bFluxChance);
                        DropHelper.DropItemChance(player, ItemID.JungleKey, 5);
                        break;

                    // Golem
                    case ItemID.GolemBossBag:
                        float aegisChance = DropHelper.LegendaryDropRateFloat;
                        DropHelper.DropItemCondition(player, ModContent.ItemType<AegisBlade>(), CalamityWorld.revenge, aegisChance);
                        DropHelper.DropItem(player, ModContent.ItemType<EssenceofCinder>(), 8, 13);
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
                player.statDefense += 5;
                player.setBonus = "+5 defense\n" +
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
                player.setBonus = "+2 defense\n" +
                            "5% increased rogue damage and critical strike chance\n" +
                            "Grants immunity to fire blocks and temporary immunity to lava\n" +
                            "Rogue stealth builds while not attacking and not moving, up to a max of 80\n" +
                            "Once you have built max stealth, you will be able to perform a Stealth Strike\n" +
                            "Rogue stealth only reduces when you attack, it does not reduce while moving\n" +
                            "The higher your rogue stealth the higher your rogue damage, crit, and movement speed\n" +
							"Provides heat protection in Death Mode";
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
                player.setBonus = "All ice-themed weapons receive a 10% damage bonus\n" +
				"Cold enemies will deal reduced contact damage to the player\n" +
				"Provides immunity to the Frostburn and Glacial State debuffs\n" +
				"Provides cold immunity in Death Mode";
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
            else if (item.type == ItemID.PalladiumBreastplate)
                player.Calamity().throwingCrit += 2;
            else if (item.type == ItemID.CobaltBreastplate)
                player.Calamity().throwingCrit += 3;
            else if (item.type == ItemID.OrichalcumBreastplate)
                player.Calamity().throwingCrit += 6;
            else if (item.type == ItemID.TitaniumBreastplate)
                player.Calamity().throwingCrit += 3;
            else if (item.type == ItemID.HallowedPlateMail)
                player.Calamity().throwingCrit += 7;
            else if (item.type == ItemID.ChlorophytePlateMail)
                player.Calamity().throwingCrit += 7;
            else if (item.type == ItemID.Gi)
                player.Calamity().throwingCrit += 5;
            #endregion

            #region Legs
            if (item.type == ItemID.GladiatorLeggings || item.type == ItemID.ObsidianPants)
                player.Calamity().throwingVelocity += 0.03f;
            else if (item.type == ItemID.PalladiumLeggings)
                player.Calamity().throwingCrit += 1;
            else if (item.type == ItemID.MythrilGreaves)
                player.Calamity().throwingCrit += 3;
            else if (item.type == ItemID.AdamantiteLeggings)
                player.Calamity().throwingCrit += 4;
            else if (item.type == ItemID.TitaniumLeggings)
                player.Calamity().throwingCrit += 3;
            else if (item.type == ItemID.ChlorophyteGreaves)
                player.Calamity().throwingCrit += 8;
            #endregion
        }
        #endregion

        #region Accessory Changes
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();

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
                player.statDefense += 10;
                player.allDamage += 0.1f;
                modPlayer.AllCritBoost(5);
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
            }
            else if (item.type == ItemID.SpookyWings) // Bonus to summon stats while wearing spooky armor
            {
                player.noFallDmg = true;
                if (player.head == ArmorIDs.Head.SpookyHelmet && player.body == ArmorIDs.Body.SpookyBreastplate && player.legs == ArmorIDs.Legs.SpookyLeggings)
                {
                    player.maxMinions++;
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
                player.statDefense += 10;
                player.allDamage += 0.05f;
                modPlayer.AllCritBoost(5);
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

            if (item.type == ItemID.CelestialStone || item.type == ItemID.CelestialShell || (item.type == ItemID.MoonStone && !Main.dayTime) ||
                (item.type == ItemID.SunStone && Main.dayTime))
            {
                player.Calamity().throwingCrit += 2;
            }
            if (item.type == ItemID.DestroyerEmblem)
                player.Calamity().throwingCrit += 8;
            if (item.type == ItemID.EyeoftheGolem)
                player.Calamity().throwingCrit += 10;
            if (item.type == ItemID.PutridScent)
                player.Calamity().throwingCrit += 5;

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
            if (item.prefix == (PrefixID.Precise | PrefixID.Lucky))
            {
                player.meleeCrit -= 1;
                player.rangedCrit -= 1;
                player.magicCrit -= 1;
                player.thrownCrit -= 1;
                player.Calamity().throwingCrit += item.prefix == PrefixID.Lucky ? 3 : 1;
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

            for (int i = 54; i < 58; i++)
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

            for (int i = 54; i < 58; i++)
            {
                if (player.inventory[i].ammo == item.useAmmo && player.inventory[i].stack >= ammoConsumed)
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
                    if (player.inventory[j].ammo == item.useAmmo && player.inventory[j].stack >= ammoConsumed)
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

        #region Goblin Money Theft (PostReforge)
        public override void PostReforge(Item item)
        {
            if (NPC.AnyNPCs(ModContent.NPCType<THIEF>()))
            {
                int value = item.value;
                ItemLoader.ReforgePrice(item, ref value, ref Main.LocalPlayer.discount);
                if (Main.LocalPlayer.Calamity().reforges <= 9) //to be reset later
                {
                    Main.LocalPlayer.Calamity().moneyStolenByBandit += value / 5;
                    Main.LocalPlayer.Calamity().reforges++;
                }
            }
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
