using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Utilities;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using CalamityMod;
using CalamityMod.NPCs;
using CalamityMod.Items.CalamityCustomThrowingDamage;

namespace CalamityMod.Items
{
	public class CalamityGlobalItem : GlobalItem
	{
        public override void SetDefaults(Item item)
        {
            if (item.maxStack == 99 || item.type == ItemID.Dynamite || item.type == ItemID.StickyDynamite ||
                item.type == ItemID.BouncyDynamite || item.type == ItemID.StickyBomb || item.type == ItemID.BouncyBomb)
                item.maxStack = 999;

            if (item.type >= ItemID.GreenSolution && item.type <= ItemID.RedSolution)
                item.value = Item.buyPrice(0, 0, 5, 0);

            if (CalamityMod.weaponAutoreuseList.Contains(item.type))
                item.autoReuse = true;

            if (item.type == ItemID.PsychoKnife)
                item.damage *= 4;
            else if (CalamityMod.doubleDamageBuffList.Contains(item.type))
                item.damage *= 2;
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

        public override bool OnPickup(Item item, Player player)
        {
            if (item.type == ItemID.Heart || item.type == ItemID.CandyApple || item.type == ItemID.CandyCane)
            {
                if (NPC.AnyNPCs(mod.NPCType("SupremeCalamitas")))
                {
                    player.statLife -= 10;
                    if (Main.myPlayer == player.whoAmI)
                    {
                        player.HealEffect(-10, true);
                    }
                }
            }
            return true;
        }

        public override bool CanUseItem(Item item, Player player)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
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
            if ((item.type == ItemID.SuperAbsorbantSponge || item.type == ItemID.EmptyBucket) && modPlayer.ZoneAbyss)
            {
                return false;
            }
            if (item.type == ItemID.MagicMirror || item.type == ItemID.IceMirror || item.type == ItemID.CellPhone || item.type == ItemID.RecallPotion)
            {
                return !NPC.AnyNPCs(mod.NPCType("SupremeCalamitas"));
            }
            if (item.type == ItemID.RodofDiscord && modPlayer.scarfCooldown)
            {
                player.AddBuff(BuffID.ChaosState, 720);
            }
            return true;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
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
            if (item.type == ItemID.AngelWings || item.type == ItemID.DemonWings)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Allows flight and slow fall\n" +
                            "Horizontal speed: 6.25\n" +
                            "Acceleration multiplier: 1\n" +
                            "Average vertical speed\n" +
                            "Flight time: 100";
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
            if (item.type == ItemID.ButterflyWings || item.type == ItemID.FairyWings || item.type == ItemID.BeeWings)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Allows flight and slow fall\n" +
                            "Horizontal speed: 6.75\n" +
                            "Acceleration multiplier: 1\n" +
                            "Average vertical speed\n" +
                            "Flight time: 130";
                    }
                }
            }
            if (item.type == ItemID.HarpyWings || item.type == ItemID.BoneWings)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Allows flight and slow fall\n" +
                            "Horizontal speed: 7\n" +
                            "Acceleration multiplier: 1\n" +
                            "Average vertical speed\n" +
                            "Flight time: 140";
                    }
                }
            }
            if (item.type == ItemID.FlameWings || item.type == ItemID.FrozenWings || item.type == ItemID.GhostWings || 
                item.type == ItemID.BeetleWings)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Allows flight and slow fall\n" +
                            "Horizontal speed: 7.5\n" +
                            "Acceleration multiplier: 1\n" +
                            "Average vertical speed\n" +
                            "Flight time: 160";
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
                            "Flight time: 100";
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
                            "Flight time: 180";
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
                            "Flight time: 140";
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
            if (item.type == ItemID.TatteredFairyWings || item.type == ItemID.SpookyWings)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Allows flight and slow fall\n" +
                            "Horizontal speed: 7.5\n" +
                            "Acceleration multiplier: 1\n" +
                            "Average vertical speed\n" +
                            "Flight time: 180";
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
                            "Flight time: 170";
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
                            "Flight time: 160";
                    }
                }
            }
            if (item.type == ItemID.WingsSolar || item.type == ItemID.WingsStardust)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Allows flight and slow fall\n" +
                            "Horizontal speed: 9\n" +
                            "Acceleration multiplier: 2.5\n" +
                            "Great vertical speed\n" +
                            "Flight time: 180";
                    }
                }
            }
            if (item.type == ItemID.WingsVortex || item.type == ItemID.WingsNebula)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Allows flight and slow fall\n" +
                            "Horizontal speed: 6.5\n" +
                            "Acceleration multiplier: 1.5\n" +
                            "Good vertical speed\n" +
                            "Flight time: 160";
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
                            line2.text = "+2% critical strike chance";
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
            if (head.type == ItemID.PumpkinHelmet && body.type == ItemID.PumpkinBreastplate && legs.type == ItemID.PumpkinLeggings)
                return "PumpMyAssFull";
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
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.1f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingVelocity += 0.1f;
                player.statDefense += 5;
                player.setBonus = "+5 defense\n" +
                            "10% increased rogue damage and velocity";
            }
            else if (set == "Obsidian")
            {
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.1f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 10;
                player.statDefense += 3;
                player.fireWalk = true;
                player.lavaMax += 300;
                player.setBonus = "+3 defense\n" +
                            "10% increased rogue damage and critical strike chance\n" +
                            "Grants immunity to fire blocks and temporary immunity to lava";
            }
            else if (set == "PumpMyAssFull")
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.1f;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            if (item.type == ItemID.SpectreHood)
                player.magicDamage += 0.2f;
            else if (item.type == ItemID.GladiatorHelmet || item.type == ItemID.ObsidianHelm)
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.05f;
            else if (item.type == ItemID.CrimsonHelmet)
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.02f;

            if (item.type == ItemID.GladiatorBreastplate || item.type == ItemID.ObsidianShirt)
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 5;
            else if (item.type == ItemID.CrimsonScalemail)
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.02f;
            else if (item.type == ItemID.PalladiumBreastplate)
            {
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.03f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 2;
            }
            else if (item.type == ItemID.CobaltBreastplate)
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 3;
            else if (item.type == ItemID.MythrilChainmail)
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.05f;
            else if (item.type == ItemID.OrichalcumBreastplate)
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 6;
            else if (item.type == ItemID.AdamantiteBreastplate)
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.06f;
            else if (item.type == ItemID.TitaniumBreastplate)
            {
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.04f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 3;
            }
            else if (item.type == ItemID.HallowedPlateMail)
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 7;
            else if (item.type == ItemID.ChlorophytePlateMail)
            {
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.05f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 7;
            }
            else if (item.type == ItemID.Gi)
            {
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.05f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 5;
            }

            if (item.type == ItemID.GladiatorLeggings || item.type == ItemID.ObsidianPants)
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingVelocity += 0.05f;
            else if (item.type == ItemID.CrimsonGreaves)
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.02f;
            else if (item.type == ItemID.PalladiumLeggings)
            {
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.02f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 1;
            }
            else if (item.type == ItemID.MythrilGreaves)
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 3;
            else if (item.type == ItemID.AdamantiteLeggings)
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 4;
            else if (item.type == ItemID.TitaniumLeggings)
            {
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.03f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 3;
            }
            else if (item.type == ItemID.HallowedGreaves)
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.07f;
            else if (item.type == ItemID.ChlorophyteGreaves)
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 8;
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);

            if (item.type == ItemID.JellyfishNecklace || item.type == ItemID.JellyfishDivingGear || item.type == ItemID.ArcticDivingGear)
                modPlayer.jellyfishNecklace = true;

            if (item.type == ItemID.WormScarf)
                player.endurance -= 0.07f;

            if (item.type == ItemID.AvengerEmblem)
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.12f;
            if (item.type == ItemID.CelestialStone || item.type == ItemID.CelestialShell || (item.type == ItemID.MoonStone && !Main.dayTime) ||
                (item.type == ItemID.SunStone && Main.dayTime))
            {
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.1f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 2;
            }
            if (item.type == ItemID.DestroyerEmblem)
            {
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.1f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 8;
            }
            if (item.type == ItemID.EyeoftheGolem)
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 10;
            if (item.type == ItemID.PutridScent)
            {
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.05f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 5;
            }

            if (item.prefix == 62)
                player.endurance += 0.0025f;
            if (item.prefix == 63)
                player.endurance += 0.005f;
            if (item.prefix == 64)
                player.endurance += 0.0075f;
            if (item.prefix == 65)
                player.endurance += 0.01f;
            if (item.prefix == 67)
            {
                player.meleeCrit -= 1;
                player.rangedCrit -= 1;
                player.magicCrit -= 1;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 1;
            }
            if (item.prefix == 68)
            {
                player.meleeCrit -= 2;
                player.rangedCrit -= 2;
                player.magicCrit -= 2;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 2;
            }
            if (item.prefix == 69)
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.01f;
            if (item.prefix == 70)
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.02f;
            if (item.prefix == 71)
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.03f;
            if (item.prefix == 72)
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.04f;
        }

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
