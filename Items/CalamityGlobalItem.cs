using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using CalamityMod.Items.CalamityCustomThrowingDamage;
using CalamityMod.World;
using CalamityMod.CalPlayer;
using CalamityMod.Utilities;

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

		public int postMoonLordRarity = 0;

		#region SetDefaults
		public override void SetDefaults(Item item)
		{
			if (postMoonLordRarity != 0 && item.rare != 10)
				item.rare = 10;

			if (item.maxStack == 99 || item.type == ItemID.Dynamite || item.type == ItemID.StickyDynamite ||
				item.type == ItemID.BouncyDynamite || item.type == ItemID.StickyBomb || item.type == ItemID.BouncyBomb)
				item.maxStack = 999;

			if (item.type >= ItemID.GreenSolution && item.type <= ItemID.RedSolution)
				item.value = Item.buyPrice(0, 0, 5, 0);

			if (CalamityMod.weaponAutoreuseList.Contains(item.type))
				item.autoReuse = true;

			if (item.type == ItemID.PsychoKnife)
				item.damage *= 4;
			else if (item.type == ItemID.SpectreStaff)
				item.damage *= 3;
			else if (CalamityMod.doubleDamageBuffList.Contains(item.type))
				item.damage *= 2;
			else if (item.type == ItemID.RainbowRod)
				item.damage = (int)((double)item.damage * 1.75);
			else if (CalamityMod.sixtySixDamageBuffList.Contains(item.type))
				item.damage = (int)((double)item.damage * 1.66);
			else if (CalamityMod.fiftyDamageBuffList.Contains(item.type))
				item.damage = (int)((double)item.damage * 1.5);
			else if (CalamityMod.thirtyThreeDamageBuffList.Contains(item.type))
				item.damage = (int)((double)item.damage * 1.33);
			else if (CalamityMod.twentyFiveDamageBuffList.Contains(item.type))
				item.damage = (int)((double)item.damage * 1.25);
			else if (CalamityMod.twentyDamageBuffList.Contains(item.type))
				item.damage = (int)((double)item.damage * 1.2);
			else if (item.type == ItemID.Frostbrand || item.type == ItemID.MagnetSphere)
				item.damage = (int)((double)item.damage * 1.1);
			else if (item.type == ItemID.Razorpine)
				item.damage = (int)((double)item.damage * 0.95);
			else if (item.type == ItemID.LastPrism)
				item.damage = (int)((double)item.damage * 0.85);
			else if (CalamityMod.quarterDamageNerfList.Contains(item.type))
				item.damage = (int)((double)item.damage * 0.75);

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
		}
		#endregion

		#region Shoot
		public override bool Shoot(Item item, Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			if (rogue)
			{
				speedX *= player.Calamity().throwingVelocity;
				speedY *= player.Calamity().throwingVelocity;
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
					if (item.melee && item.type != mod.ItemType("Murasama"))
						Projectile.NewProjectile(position.X, position.Y, speedX * 0.5f, speedY * 0.5f, mod.ProjectileType("LuxorsGiftMelee"), (int)(newDamage * 0.6), 0f, player.whoAmI, 0f, 0f);

					else if (rogue)
						Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("LuxorsGiftRogue"), (int)(newDamage * 0.5), 0f, player.whoAmI, 0f, 0f);

					else if (item.ranged)
						Projectile.NewProjectile(position.X, position.Y, speedX * 1.5f, speedY * 1.5f, mod.ProjectileType("LuxorsGiftRanged"), (int)(newDamage * 0.4), 0f, player.whoAmI, 0f, 0f);

					else if (item.magic)
						Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("LuxorsGiftMagic"), (int)(newDamage * 0.8), 0f, player.whoAmI, 0f, 0f);

					else if (item.summon && player.ownedProjectileCounts[mod.ProjectileType("LuxorsGiftSummon")] < 1)
						Projectile.NewProjectile(position.X, position.Y, 0f, 0f, mod.ProjectileType("LuxorsGiftSummon"), damage, 0f, player.whoAmI, 0f, 0f);
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
						Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("GhostlyBolt"), (int)((double)damage * (player.Calamity().auricSet ? 4.2 : 2.6)), 1f, player.whoAmI, 0f, 0f);
					}
				}
			}
			if (player.Calamity().bloodflareRanged) //0 - 99
			{
				if (item.ranged && !rogue && Main.rand.Next(0, 100) >= 98)
				{
					if (player.whoAmI == Main.myPlayer)
					{
						Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("BloodBomb"), (int)((double)damage * (player.Calamity().auricSet ? 2.2 : 1.6)), 2f, player.whoAmI, 0f, 0f);
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
						Projectile.NewProjectile(position.X, position.Y, speedX * 1.25f, speedY * 1.25f, mod.ProjectileType("ChaosFlare"), (int)((double)damage * 0.25), 2f, player.whoAmI, 0f, 0f);
					}
				}
			}
			if (player.Calamity().godSlayerRanged) //0 - 99
			{
				if (item.ranged && !rogue && Main.rand.Next(0, 100) >= 95)
				{
					if (player.whoAmI == Main.myPlayer)
					{
						Projectile.NewProjectile(position.X, position.Y, speedX * 1.25f, speedY * 1.25f, mod.ProjectileType("GodSlayerShrapnelRound"), (int)((double)damage * (player.Calamity().auricSet ? 3.2 : 2.1)), 2f, player.whoAmI, 0f, 0f);
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
							offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i;
							Projectile.NewProjectile(vector2.X, vector2.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), mod.ProjectileType("ChaosFlare2"), (int)((double)damage * 0.5), 1.25f, player.whoAmI, 0f, 0f);
							Projectile.NewProjectile(vector2.X, vector2.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), mod.ProjectileType("ChaosFlare2"), (int)((double)damage * 0.5), 1.25f, player.whoAmI, 0f, 0f);
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
						Projectile.NewProjectile(position.X, position.Y, speedX * 1.25f, speedY * 1.25f, mod.ProjectileType("MiniRocket"), (int)((double)damage * 1.3), 2f, player.whoAmI, 0f, 0f);
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
						Projectile.NewProjectile(position.X, position.Y, speedX * 1.25f, speedY * 1.25f, mod.ProjectileType("Seashell"), damage * 2, 1f, player.whoAmI, 0f, 0f);
					}
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
				{
					"rogue", rogue
				},
				{
					"timesUsed", timesUsed
				},
				{
					"rarity", postMoonLordRarity
				}
			};
		}

		public override void Load(Item item, TagCompound tag)
		{
			rogue = tag.GetBool("rogue");
			timesUsed = tag.GetInt("timesUsed");
			postMoonLordRarity = tag.GetInt("rarity");
		}

		public override void LoadLegacy(Item item, BinaryReader reader)
		{
			int loadVersion = reader.ReadInt32();
			postMoonLordRarity = reader.ReadInt32();
			timesUsed = reader.ReadInt32();

			if (loadVersion == 0)
			{
				BitsByte flags = reader.ReadByte();
				rogue = flags[0];
			}
			else
			{
				CalamityMod.Instance.Logger.Error("Unknown loadVersion: " + loadVersion);
			}
		}

		public override void NetSend(Item item, BinaryWriter writer)
		{
			BitsByte flags = new BitsByte();
			flags[0] = rogue;

			writer.Write(flags);
			writer.Write(postMoonLordRarity);
			writer.Write(timesUsed);
		}

		public override void NetReceive(Item item, BinaryReader reader)
		{
			BitsByte flags = reader.ReadByte();
			rogue = flags[0];

			postMoonLordRarity = reader.ReadInt32();
			timesUsed = reader.ReadInt32();
		}
		#endregion

		#region Pickup Item Changes
		public override bool OnPickup(Item item, Player player)
		{
			if (item.type == ItemID.Heart || item.type == ItemID.CandyApple || item.type == ItemID.CandyCane)
			{
				bool boostedHeart = player.Calamity().photosynthesis;
				if (NPC.AnyNPCs(mod.NPCType("SupremeCalamitas")))
				{
					player.statLife -= (boostedHeart ? 5 : 10);
					if (Main.myPlayer == player.whoAmI)
					{
						player.HealEffect((boostedHeart ? -5 : -10), true);
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

		#region Use Item Changes
		public override bool CanUseItem(Item item, Player player)
		{
			CalamityPlayer modPlayer = player.Calamity();
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
			if (item.type == ItemID.SpaceGun && player.spaceGun)
			{
				if (player.statMana >= (int)((float)3 * player.manaCost))
				{
					player.manaRegenDelay = (int)player.maxRegenDelay;
					player.statMana -= (int)((float)3 * player.manaCost);
				}
				else if (player.manaFlower)
				{
					player.QuickMana();
					if (player.statMana >= (int)((float)3 * player.manaCost))
					{
						player.manaRegenDelay = (int)player.maxRegenDelay;
						player.statMana -= (int)((float)3 * player.manaCost);
					}
					else
					{
						return false;
					}
				}
				else
				{
					return false;
				}
				return true;
			}
			if ((item.type == ItemID.RegenerationPotion || item.type == ItemID.LifeforcePotion) && player.FindBuffIndex(mod.BuffType("Cadence")) > -1)
			{
				return false;
			}
			if (item.type == mod.ItemType("CrumblingPotion") && player.FindBuffIndex(mod.BuffType("ArmorShattering")) > -1)
			{
				return false;
			}
			if (item.type == ItemID.WrathPotion && player.FindBuffIndex(mod.BuffType("HolyWrathBuff")) > -1)
			{
				return false;
			}
			if (item.type == ItemID.RagePotion && player.FindBuffIndex(mod.BuffType("ProfanedRageBuff")) > -1)
			{
				return false;
			}
			if ((item.type == ItemID.SuperAbsorbantSponge || item.type == ItemID.EmptyBucket) && modPlayer.ZoneAbyss)
			{
				return false;
			}
			if (item.type == ItemID.MagicMirror || item.type == ItemID.IceMirror || item.type == ItemID.CellPhone || item.type == ItemID.RecallPotion)
			{
				return !NPC.AnyNPCs(mod.NPCType("SupremeCalamitas"));
			}
			if (item.type == ItemID.RodofDiscord)
			{
				if (modPlayer.scarfCooldown)
				{
					player.AddBuff(BuffID.ChaosState, 720);
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
				switch (postMoonLordRarity)
				{
					case 12:
						tt2.overrideColor = new Color(0, 255, 200);
						break;
					case 13:
						tt2.overrideColor = new Color(0, 255, 0);
						break;
					case 14:
						tt2.overrideColor = new Color(43, 96, 222);
						break;
					case 15:
						tt2.overrideColor = new Color(108, 45, 199);
						break;
					case 16:
						tt2.overrideColor = new Color(255, 0, 255);
						break;
					case 17: //Legendary Weapons
						if (item.type == mod.ItemType("Malachite"))
							tt2.overrideColor = new Color(Main.DiscoR, 203, 103);
						if (item.type == mod.ItemType("AegisBlade") || item.type == mod.ItemType("YharimsCrystal"))
							tt2.overrideColor = new Color(255, Main.DiscoG, 53);
						if (item.type == mod.ItemType("BrinyBaron"))
							tt2.overrideColor = new Color(53, Main.DiscoG, 255);
						if (item.type == mod.ItemType("CosmicDischarge"))
							tt2.overrideColor = new Color(150, Main.DiscoG, 255);
						if (item.type == mod.ItemType("BlossomFlux"))
							tt2.overrideColor = new Color(Main.DiscoR, 203, 103);
						if (item.type == mod.ItemType("SHPC"))
							tt2.overrideColor = new Color(255, Main.DiscoG, 155);
						if (item.type == mod.ItemType("Vesuvius"))
							tt2.overrideColor = new Color(255, Main.DiscoG, 0);
						break;
					case 18: //Fabstaff
						tt2.overrideColor = new Color(Main.DiscoR, 100, 255);
						break;
					case 19: //Blushie Staff
						tt2.overrideColor = new Color(0, 0, 255);
						break;
					case 20: //Non-Expert Rainbow
						tt2.overrideColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
						break;
					case 21: //Patreon
						tt2.overrideColor = new Color(139, 0, 0);
						break;
					case 22: //Rare Variants
						tt2.overrideColor = new Color(255, 140, 0);
						break;
					default:
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
							"Makes you immune to the Chilled, Frozen, and Glacial State debuffs";
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
						line2.text = "14% increased melee damage and speed";
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
			if (item.type == ItemID.ApprenticeHat)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Increases your max number of sentries and reduces mana costs by 10%\n" +
							"30% increased minion damage";
					}
				}
			}
			if (item.type == ItemID.ApprenticeRobe)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "10% increased magic damage";
					}
				}
			}
			if (item.type == ItemID.ApprenticeTrousers)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "20% increased movement speed";
					}
				}
			}
			if (item.type == ItemID.SquireGreatHelm)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Increases your max number of sentries and increases your life regeneration\n" +
							"30% increased minion damage and 20% increased melee critical strike chance";
					}
				}
			}
			if (item.type == ItemID.SquirePlating)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "15% increased melee damage";
					}
				}
			}
			if (item.type == ItemID.SquireGreaves)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "20% increased movement speed";
					}
				}
			}
			if (item.type == ItemID.HuntressWig)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Increases your max number of sentries and increases ranged critical strike chance by 10%\n" +
							"30% increased minion damage";
					}
				}
			}
			if (item.type == ItemID.HuntressJerkin)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "20% increased ranged damage";
					}
				}
			}
			if (item.type == ItemID.HuntressPants)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "20% increased movement speed";
					}
				}
			}
			if (item.type == ItemID.MonkBrows)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Increases your max number of sentries and increases melee attack speed by 20%\n" +
							"30% increased minion damage and 10% increased melee critical strike chance";
					}
				}
			}
			if (item.type == ItemID.MonkShirt)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "20% increased melee damage";
					}
				}
			}
			if (item.type == ItemID.MonkPants)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "20% increased movement speed";
					}
					if (line2.mod == "Terraria" && line2.Name == "Tooltip1")
					{
						line2.text = "";
					}
				}
			}
			if (item.type == ItemID.ApprenticeAltHead)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip1")
					{
						line2.text = "60% increased minion damage and 25% increased magic damage and critical strike chance";
					}
				}
			}
			if (item.type == ItemID.ApprenticeAltShirt)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "";
					}
				}
			}
			if (item.type == ItemID.ApprenticeAltPants)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "";
					}
				}
			}
			if (item.type == ItemID.MonkAltHead)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Increases your max number of sentries\n" +
							"Increases melee damage, speed, and critical strike chance by 20% and minion damage by 60%";
					}
				}
			}
			if (item.type == ItemID.MonkAltShirt)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "";
					}
				}
			}
			if (item.type == ItemID.MonkAltPants)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "20% increased movement speed";
					}
				}
			}
			if (item.type == ItemID.HuntressAltHead)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip1")
					{
						line2.text = "60% increased minion damage and 10% increased ranged critical strike chance\n" +
							"25% increased ranged damage";
					}
				}
			}
			if (item.type == ItemID.HuntressAltShirt)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "";
					}
				}
			}
			if (item.type == ItemID.HuntressAltPants)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "20% increased movement speed";
					}
				}
			}
			if (item.type == ItemID.SquireAltHead)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Increases your max number of sentries and grants you 60% minion damage\n" +
							"20% increased melee critical strike chance";
					}
				}
			}
			if (item.type == ItemID.SquireAltShirt)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Massively increased life regeneration";
					}
				}
			}
			if (item.type == ItemID.SquireAltPants)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "30% increased movement speed";
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
							"Reduces lava burn damage";
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
							"Greatly reduces lava burn damage";
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
			if (item.type == ItemID.IronHelmet || item.type == ItemID.IronChainmail || item.type == ItemID.IronGreaves)
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
			if (item.type == ItemID.GoldHelmet || item.type == ItemID.GoldChainmail || item.type == ItemID.GoldGreaves)
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
			if (item.type == ItemID.GladiatorHelmet)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Defense")
					{
						line2.text = "4 defense\n" +
							"5% increased rogue damage";
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
							"5% increased rogue critical strike chance";
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
							"5% increased rogue velocity";
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
							"5% increased rogue damage";
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
							"5% increased rogue critical strike chance";
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
							"5% increased rogue velocity";
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
							"7% increased melee and ranged damage\n" +
							"and 3% increased melee and ranged critical strike chance\n" +
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
							"Flight time: 160";
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
							"10% increased movement speed and 100% increased jump speed\n" +
							"+20 defense, 15% increased damage, 7% increased critical strike chance\n" +
							"and 10% increased movement speed at night";
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
							"+1 max minion and 10% increased minion damage while wearing the Spooky Armor";
					}
				}
			}
			if (item.type == ItemID.Hoverboard)
			{
				foreach (TooltipLine line2 in tooltips)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Allows flight and slow fall\n" +
							"Horizontal speed: 6.25\n" +
							"Acceleration multiplier: 1\n" +
							"Average vertical speed\n" +
							"Flight time: 170";
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
							"+10 defense, 10% increased damage," +
							"5% increased critical strike chance," +
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
							"Horizontal speed: 6\n" +
							"Acceleration multiplier: 2.5\n" +
							"Good vertical speed\n" +
							"Flight time: 150";
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
					foreach (TooltipLine line2 in tooltips)
					{
						if (line2.mod == "Terraria" && line2.Name == "PrefixAccDefense")
						{
							line2.text = "+1 defense\n" +
								"+0.25% damage reduction";
						}
					}
				}
				if (item.prefix == 63)
				{
					foreach (TooltipLine line2 in tooltips)
					{
						if (line2.mod == "Terraria" && line2.Name == "PrefixAccDefense")
						{
							line2.text = "+2 defense\n" +
								"+0.5% damage reduction";
						}
					}
				}
				if (item.prefix == 64)
				{
					foreach (TooltipLine line2 in tooltips)
					{
						if (line2.mod == "Terraria" && line2.Name == "PrefixAccDefense")
						{
							line2.text = "+3 defense\n" +
								"+0.75% damage reduction";
						}
					}
				}
				if (item.prefix == 65)
				{
					foreach (TooltipLine line2 in tooltips)
					{
						if (line2.mod == "Terraria" && line2.Name == "PrefixAccDefense")
						{
							line2.text = "+4 defense\n" +
								"+1% damage reduction";
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
			if (context == "bossBag")
			{
				// Give a chance for Laudanum, Stress Pills and Heart of Darkness from every boss bag
				DropHelper.DropRevBagAccessories(player);

				switch (arg)
				{
					// King Slime
					case ItemID.KingSlimeBossBag:
						DropHelper.DropItemCondition(player, mod.ItemType("CrownJewel"), CalamityWorld.revenge);
						break;

					// Eye of Cthulhu
					case ItemID.EyeOfCthulhuBossBag:
						DropHelper.DropItem(player, mod.ItemType("VictoryShard"), 3, 5);
						DropHelper.DropItemChance(player, mod.ItemType("TeardropCleaver"), 3);
						DropHelper.DropItemCondition(player, mod.ItemType("CounterScarf"), CalamityWorld.revenge);
						break;

					// Skeletron
					case ItemID.SkeletronBossBag:
						DropHelper.DropItemChance(player, mod.ItemType("ClothiersWrath"), DropHelper.RareVariantDropRateInt);
						break;

					// Wall of Flesh
					case ItemID.WallOfFleshBossBag:
						DropHelper.DropItemChance(player, mod.ItemType("Meowthrower"), 3);
						DropHelper.DropItemChance(player, mod.ItemType("BlackHawkRemote"), 3);
						DropHelper.DropItemChance(player, mod.ItemType("RogueEmblem"), 4);
						DropHelper.DropItemFromSetChance(player, 5, ItemID.CorruptionKey, ItemID.CrimsonKey);
						DropHelper.DropItem(player, mod.ItemType("MLGRune")); // Demon Trophy
						break;

					// Destroyer
					case ItemID.DestroyerBossBag:
						float shpcChance = CalamityWorld.defiled ? DropHelper.DefiledDropRateFloat : DropHelper.LegendaryDropRateFloat;
						DropHelper.DropItemCondition(player, mod.ItemType("SHPC"), CalamityWorld.revenge, shpcChance);
						break;

					// Plantera
					case ItemID.PlanteraBossBag:
						DropHelper.DropItem(player, mod.ItemType("LivingShard"), 8, 11);
						float bFluxChance = CalamityWorld.defiled ? DropHelper.DefiledDropRateFloat : DropHelper.LegendaryDropRateFloat;
						DropHelper.DropItemCondition(player, mod.ItemType("BlossomFlux"), CalamityWorld.revenge, bFluxChance);
						DropHelper.DropItemChance(player, ItemID.JungleKey, 5);
						break;

					// Golem
					case ItemID.GolemBossBag:
						float aegisChance = CalamityWorld.defiled ? DropHelper.DefiledDropRateFloat : DropHelper.LegendaryDropRateFloat;
						DropHelper.DropItemCondition(player, mod.ItemType("AegisBlade"), CalamityWorld.revenge, aegisChance);
						break;

					// Duke Fishron
					case ItemID.FishronBossBag:
						float baronChance = CalamityWorld.defiled ? DropHelper.DefiledDropRateFloat : DropHelper.LegendaryDropRateFloat;
						DropHelper.DropItemCondition(player, mod.ItemType("BrinyBaron"), CalamityWorld.revenge, baronChance);
						break;

					// Betsy
					case ItemID.BossBagBetsy:
						float vesuviusChance = CalamityWorld.defiled ? DropHelper.DefiledDropRateFloat : DropHelper.LegendaryDropRateFloat;
						DropHelper.DropItemCondition(player, mod.ItemType("Vesuvius"), CalamityWorld.revenge, vesuviusChance);
						break;

					// Moon Lord
					case ItemID.MoonLordBossBag:
						DropHelper.DropItem(player, mod.ItemType("MLGRune2")); // Celestial Onion
						DropHelper.DropItemChance(player, mod.ItemType("UtensilPoker"), 8);
						DropHelper.DropItemChance(player, mod.ItemType("GrandDad"), DropHelper.RareVariantDropRateInt);
						DropHelper.DropItemChance(player, mod.ItemType("Infinity"), DropHelper.RareVariantDropRateInt);
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
			if (head.type == ItemID.IronHelmet && body.type == ItemID.IronChainmail && legs.type == ItemID.IronGreaves)
				return "Iron";
			if (head.type == ItemID.LeadHelmet && body.type == ItemID.LeadChainmail && legs.type == ItemID.LeadGreaves)
				return "Lead";
			if (head.type == ItemID.SilverHelmet && body.type == ItemID.SilverChainmail && legs.type == ItemID.SilverGreaves)
				return "Silver";
			if (head.type == ItemID.TungstenHelmet && body.type == ItemID.TungstenChainmail && legs.type == ItemID.TungstenGreaves)
				return "Tungsten";
			if (head.type == ItemID.GoldHelmet && body.type == ItemID.GoldChainmail && legs.type == ItemID.GoldGreaves)
				return "Gold";
			if (head.type == ItemID.PlatinumHelmet && body.type == ItemID.PlatinumChainmail && legs.type == ItemID.PlatinumGreaves)
				return "Platinum";
			if (head.type == ItemID.GladiatorHelmet && body.type == ItemID.GladiatorBreastplate && legs.type == ItemID.GladiatorLeggings)
				return "Gladiator";
			if (head.type == ItemID.ObsidianHelm && body.type == ItemID.ObsidianShirt && legs.type == ItemID.ObsidianPants)
				return "Obsidian";
			return "";
		}

		public override void UpdateArmorSet(Player player, string set)
		{
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
				CalamityPlayer modPlayer = player.Calamity();
				modPlayer.rogueStealthMax = 1f;
                modPlayer.wearingRogueArmor = true;
				player.Calamity().throwingDamage += 0.1f;
				player.Calamity().throwingVelocity += 0.1f;
				player.statDefense += 5;
				player.setBonus = "+5 defense\n" +
							"10% increased rogue damage and velocity\n" +
							"Rogue stealth builds while not attacking and not moving, up to a max of 100\n" +
							"Rogue stealth only reduces when you attack, it does not reduce while moving\n" +
							"The higher your rogue stealth the higher your rogue damage, crit, and movement speed";
			}
			else if (set == "Obsidian")
			{
				CalamityPlayer modPlayer = player.Calamity();
				modPlayer.rogueStealthMax = 1f;
                modPlayer.wearingRogueArmor = true;
				player.Calamity().throwingDamage += 0.05f;
				player.Calamity().throwingCrit += 5;
				player.statDefense += 2;
				player.fireWalk = true;
				player.lavaMax += 180;
				player.setBonus = "+2 defense\n" +
							"5% increased rogue damage and critical strike chance\n" +
							"Grants immunity to fire blocks and temporary immunity to lava\n" +
							"Rogue stealth builds while not attacking and not moving, up to a max of 100\n" +
							"Rogue stealth only reduces when you attack, it does not reduce while moving\n" +
							"The higher your rogue stealth the higher your rogue damage, crit, and movement speed";
			}
		}
		#endregion

		#region Equip Changes
		public override void UpdateEquip(Item item, Player player)
		{
			if (item.type == ItemID.FireGauntlet)
			{
				player.meleeDamage += 0.04f;
				player.meleeSpeed += 0.04f;
			}

			#region Head
			if (item.type == ItemID.SpectreHood)
				player.magicDamage += 0.2f;
			else if (item.type == ItemID.GladiatorHelmet || item.type == ItemID.ObsidianHelm)
				player.Calamity().throwingDamage += 0.05f;
			else if (item.type == ItemID.ApprenticeHat)
				player.minionDamage += 0.3f;
			else if (item.type == ItemID.SquireGreatHelm)
			{
				player.minionDamage += 0.3f;
				player.meleeCrit += 20;
			}
			else if (item.type == ItemID.HuntressWig)
				player.minionDamage += 0.3f;
			else if (item.type == ItemID.MonkBrows)
			{
				player.minionDamage += 0.3f;
				player.meleeCrit += 10;
			}
			else if (item.type == ItemID.ApprenticeAltHead)
			{
				player.minionDamage += 0.5f;
				player.magicDamage += 0.15f;
				player.magicCrit += 25;
			}
			else if (item.type == ItemID.MonkAltHead)
			{
				player.minionDamage += 0.4f;
				player.meleeSpeed += 0.2f;
				player.meleeCrit += 20;
			}
			else if (item.type == ItemID.HuntressAltHead)
			{
				player.minionDamage += 0.5f;
				player.rangedDamage += 0.25f;
			}
			else if (item.type == ItemID.SquireAltHead)
			{
				player.minionDamage += 0.5f;
				player.meleeCrit += 20;
			}
			#endregion

			#region Body
			if (item.type == ItemID.GladiatorBreastplate || item.type == ItemID.ObsidianShirt)
				player.Calamity().throwingCrit += 5;
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
			else if (item.type == ItemID.ApprenticeRobe)
				player.minionDamage -= 0.2f;
			else if (item.type == ItemID.SquirePlating)
				player.minionDamage -= 0.15f;
			else if (item.type == ItemID.HuntressJerkin)
				player.minionDamage -= 0.2f;
			else if (item.type == ItemID.MonkShirt)
				player.minionDamage -= 0.2f;
			else if (item.type == ItemID.ApprenticeAltShirt)
			{
				player.minionDamage -= 0.3f;
				player.magicDamage -= 0.15f;
			}
			else if (item.type == ItemID.MonkAltShirt)
			{
				player.minionDamage -= 0.2f;
				player.meleeSpeed -= 0.2f;
			}
			else if (item.type == ItemID.HuntressAltShirt)
			{
				player.minionDamage -= 0.25f;
				player.meleeSpeed -= 0.2f;
			}
			else if (item.type == ItemID.SquireAltShirt)
				player.minionDamage -= 0.3f;
			#endregion

			#region Legs
			if (item.type == ItemID.GladiatorLeggings || item.type == ItemID.ObsidianPants)
				player.Calamity().throwingVelocity += 0.05f;
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
			else if (item.type == ItemID.ApprenticeTrousers)
			{
				player.magicCrit -= 20;
				player.moveSpeed += 0.2f;
			}
			else if (item.type == ItemID.SquireGreaves)
			{
				player.minionDamage -= 0.15f;
				player.meleeCrit -= 20;
			}
			else if (item.type == ItemID.HuntressPants)
				player.minionDamage -= 0.1f;
			else if (item.type == ItemID.MonkPants)
			{
				player.minionDamage -= 0.1f;
				player.meleeCrit -= 10;
			}
			else if (item.type == ItemID.ApprenticeAltPants)
			{
				player.minionDamage -= 0.2f;
				player.magicCrit -= 25;
			}
			else if (item.type == ItemID.MonkAltPants)
			{
				player.minionDamage -= 0.2f;
				player.meleeCrit -= 20;
			}
			else if (item.type == ItemID.HuntressAltPants)
				player.minionDamage -= 0.25f;
			if (item.type == ItemID.SquireAltPants)
			{
				player.minionDamage -= 0.2f;
				player.meleeCrit -= 20;
			}
			#endregion
		}
		#endregion

		#region Accessory Changes
		public override void UpdateAccessory(Item item, Player player, bool hideVisual)
		{
			CalamityPlayer modPlayer = player.Calamity();

			if (item.type == ItemID.AngelWings) // Boost to max life, defense, and life regen
			{
				player.statLifeMax2 += 20;
				player.statDefense += 15;
				player.lifeRegen += 3;
			}
			else if (item.type == ItemID.DemonWings) // Boost to all damage and crit
			{
				player.allDamage += 0.1f;
				modPlayer.AllCritBoost(10);
			}
			else if (item.type == ItemID.FinWings) // Boosted water abilities, faster fall in water
			{
				player.moveSpeed += 0.2f;
				player.jumpSpeedBoost += 1.8f;
				player.gills = true;
				player.ignoreWater = true;
				if (!player.mount.Active)
				{
					if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
						player.maxFallSpeed = 12f;
				}
			}
			else if (item.type == ItemID.BeeWings) // Honey buff
			{
				player.AddBuff(BuffID.Honey, 2);
			}
			else if (item.type == ItemID.ButterflyWings) // Boost to magic stats
			{
				player.statManaMax2 += 50;
				player.magicDamage += 0.1f;
				player.manaCost *= 0.95f;
				player.magicCrit += 5;
			}
			else if (item.type == ItemID.FairyWings) // Boost to max life
			{
				player.statLifeMax2 += 80;
			}
			else if (item.type == ItemID.BatWings) // Stronger at night
			{
				player.moveSpeed += 0.1f;
				player.jumpSpeedBoost += 1.0f;
				if (!Main.dayTime)
				{
					player.statDefense += 20;
					player.allDamage += 0.15f;
					modPlayer.AllCritBoost(7);
					player.moveSpeed += 0.1f;
				}
			}
			else if (item.type == ItemID.HarpyWings)
			{
				player.moveSpeed += 0.3f;
			}
			else if (item.type == ItemID.BoneWings) // Bonus to ranged and defense stats while wearing necro armor
			{
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
			}
			else if (item.type == ItemID.FrozenWings) // Bonus to melee and ranged stats while wearing frost armor
			{
				if (player.head == ArmorIDs.Head.FrostHelmet && player.body == ArmorIDs.Body.FrostBreastplate && player.legs == ArmorIDs.Legs.FrostLeggings)
				{
					player.meleeDamage += 0.07f;
					player.rangedDamage += 0.07f;
					player.meleeCrit += 3;
					player.rangedCrit += 3;
				}
			}
			else if (item.type == ItemID.FlameWings) // Bonus to melee stats
			{
				player.meleeDamage += 0.1f;
				player.meleeCrit += 5;
			}
			else if (item.type == ItemID.GhostWings) // Bonus to mage stats while wearing spectre armor
			{
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
			else if (item.type == ItemID.FestiveWings) // Drop powerful homing christmas tree bulbs while in flight
			{
				player.statLifeMax2 += 50;
			}
			else if (item.type == ItemID.SpookyWings) // Bonus to summon stats while wearing spooky armor
			{
				if (player.head == ArmorIDs.Head.SpookyHelmet && player.body == ArmorIDs.Body.SpookyBreastplate && player.legs == ArmorIDs.Legs.SpookyLeggings)
				{
					player.maxMinions++;
					player.minionDamage += 0.1f;
				}
			}
			else if (item.type == ItemID.TatteredFairyWings)
			{
				player.allDamage += 0.05f;
				modPlayer.AllCritBoost(5);
			}
			else if (item.type == ItemID.SteampunkWings)
			{
				player.statDefense += 10;
				player.allDamage += 0.05f;
				modPlayer.AllCritBoost(5);
				player.moveSpeed += 0.1f;
			}
			else if (item.type == ItemID.WingsSolar) // Bonus to melee stats while wearing solar flare armor
			{
				if (player.head == ArmorIDs.Head.SolarFlareHelmet && player.body == ArmorIDs.Body.SolarFlareBreastplate && player.legs == ArmorIDs.Legs.SolarFlareLeggings)
				{
					player.meleeDamage += 0.07f;
					player.meleeCrit += 3;
				}
			}
			else if (item.type == ItemID.WingsVortex) // Bonus to ranged stats while wearing vortex armor
			{
				if (player.head == ArmorIDs.Head.VortexHelmet && player.body == ArmorIDs.Body.VortexBreastplate && player.legs == ArmorIDs.Legs.VortexLeggings)
				{
					player.rangedDamage += 0.03f;
					player.rangedCrit += 7;
				}
			}
			else if (item.type == ItemID.WingsNebula) // Bonus to magic stats while wearing nebula armor
			{
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
				if (player.head == ArmorIDs.Head.StardustHelmet && player.body == ArmorIDs.Body.StardustPlate && player.legs == ArmorIDs.Legs.StardustLeggings)
				{
					player.maxMinions++;
					player.minionDamage += 0.05f;
				}
			}

			if (item.type == ItemID.JellyfishNecklace || item.type == ItemID.JellyfishDivingGear || item.type == ItemID.ArcticDivingGear)
				modPlayer.jellyfishNecklace = true;

			if (item.type == ItemID.WormScarf)
				player.endurance -= 0.07f;

			if (item.type == ItemID.RoyalGel)
				modPlayer.royalGel = true;

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
			if (item.prefix == 62)
				player.endurance += 0.0025f;
			if (item.prefix == 63)
				player.endurance += 0.005f;
			if (item.prefix == 64)
				player.endurance += 0.0075f;
			if (item.prefix == 65)
				player.endurance += 0.01f;

			// Precise only gives 1% crit and Lucky only gives 3% crit
			if (item.prefix == 67 || item.prefix == 68)
			{
                player.meleeCrit -= 1;
				player.rangedCrit -= 1;
				player.magicCrit -= 1;
                player.thrownCrit -= 1;
				player.Calamity().throwingCrit += (item.prefix == 68 ? 3 : 1);
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
				(modPlayer.draconicSurge ? 0.15f : 0f);
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
				if (player.inventory[i].ammo == item.useAmmo && player.inventory[i].stack >= ammoConsumed)
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
					if (player.inventory[j].ammo == item.useAmmo && player.inventory[j].stack >= ammoConsumed)
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
            if (NPC.AnyNPCs(mod.NPCType("Bandit")))
            {
                int value = item.value;
                ItemLoader.ReforgePrice(item, ref value, ref Main.LocalPlayer.discount);
                if (Main.LocalPlayer.GetModPlayer<CalamityPlayer>().reforges <= 9) //to be reset later
                {
                    Main.LocalPlayer.GetModPlayer<CalamityPlayer>().moneyStolenByBandit += value / 5;
                    Main.LocalPlayer.GetModPlayer<CalamityPlayer>().reforges++;
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
