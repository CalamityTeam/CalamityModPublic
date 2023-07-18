using CalamityMod.Buffs.StatBuffs;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    //Developer item, dedicatee: Mishiro Usui/Amber Sienna
    public class ProfanedSoulCrystal : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        /**
         * Notes: Drops from providence if the only damage source during the fight is from typeless damage or the profaned soul and the owners of those babs do not have profaned crystal.
         * All projectiles are in ProfanedSoulCrystalProjectiles.cs in the summon projectile directory
         * The projectiles being created/fired on click happens in CalamityGlobalItem (there's a region specially for it so ctrl + f is your friend)
         * the day/night buffs are in calamityplayermisceffects
         * the bab projectiles are the same, just refactored ai to be more adhering to DRY principle
         * bab spears being fired happens at the bottom of calplayer
         * Animation of legs is postupdate, animation of wings is frameeffects.
         * Projectiles transformed are ONLY affected by alldamage and summon damage bonuses, likewise the weapon's base damage/usetime is NOT taken into account.
         * You enrage below or at 50% hp.
         */
        public override void Load()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Accessories/ProfanedSoulTransHead", EquipType.Head, this);
            EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Accessories/ProfanedSoulTransBody", EquipType.Body, this);
            EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Accessories/ProfanedSoulTransLegs", EquipType.Legs, this);
            EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Accessories/Wings/ProfanedSoulTransWings", EquipType.Wings, this);
        }

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(8, 4));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<ProfanedSoulArtifact>();

            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlotBody = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
            ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
            ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;

            int equipSlotLegs = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);
            ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegs] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.Calamity().devItem = true;
        }

        public override bool CanEquipAccessory(Player player, int slot, bool modded)
        {
            return !player.Calamity().pArtifact;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            bool scal = DownedBossSystem.downedCalamitas;
            bool draedon = DownedBossSystem.downedExoMechs;
            if (!scal || !draedon)
            {
                string reject = this.GetLocalizedValue(!draedon ? "ExoMechsLock" : "CalamitasLock") + "\n" + this.GetLocalizedValue("Reject");
                tooltips.FindAndReplace("[STATUS]", reject);

                TooltipLine linePrice = tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Sell price");
                if (linePrice != null)
                    linePrice.Text = "";
            }
            else
            {
                string manaCost = (100 * Main.player[Main.myPlayer].manaCost).ToString("N0");
                string full = this.GetLocalization("FullTooltip").Format(manaCost);
                tooltips.FindAndReplace("[STATUS]", full);
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();

            modPlayer.pArtifact = true;
            modPlayer.profanedCrystal = true;

            if (hideVisual)
                modPlayer.profanedCrystalHide = true;
        }

        public override void UpdateVanity(Player player)
        {
            player.Calamity().profanedCrystalHide = false;
            player.Calamity().profanedCrystalForce = true;
        }

        internal static void DetermineTransformationEligibility(Player player)
        {
            if (DownedBossSystem.downedCalamitas && DownedBossSystem.downedExoMechs && (player.maxMinions - player.slotsMinions) >= 10 && !player.Calamity().profanedCrystalForce && player.HasBuff<ProfanedCrystalBuff>())
            {
                player.Calamity().profanedCrystalBuffs = true;
            }
        }

        // Moved from CalamityGlobalItem since it's just a function called in one place.
        internal static bool TransformItemUsage(Item item, Player player)
        {
            if (player.whoAmI != Main.myPlayer)
                return false;

            var source = player.GetSource_ItemUse(item);
            int weaponType = item.CountsAsClass<MeleeDamageClass>() ? 1 : 
                item.CountsAsClass<RangedDamageClass>() ? 2 : 
                item.CountsAsClass<MagicDamageClass>() ? 3 :
                item.CountsAsClass<ThrowingDamageClass>() ? 4 : -1;
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
                if (item.CountsAsClass<MeleeDamageClass>())
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

                                int spearBaseDamage = shouldNerf ? 175 : 350;
                                int spearDamage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(spearBaseDamage);
                                int proj = Projectile.NewProjectile(source, player.Center.X, player.Center.Y - 10, perturbedspeed.X, perturbedspeed.Y, ModContent.ProjectileType<ProfanedCrystalMeleeSpear>(), spearDamage, 1f, player.whoAmI, Main.rand.NextBool(player.Calamity().profanedSoulWeaponUsage == 4 ? 5 : 7) ? 1f : 0f);
                                if (proj.WithinBounds(Main.maxProjectiles))
                                {
                                    Main.projectile[proj].DamageType = DamageClass.Summon;
                                    Main.projectile[proj].originalDamage = spearBaseDamage;
                                }
                                spread -= Main.rand.Next(2, 4);
                                SoundEngine.PlaySound(SoundID.Item20, player.Center);
                            }
                            player.Calamity().profanedSoulWeaponUsage = 0;
                        }
                        else
                        {
                            int spearBaseDamage = shouldNerf ? 125 : 250;
                            int spearDamage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(spearBaseDamage);
                            int proj = Projectile.NewProjectile(source, player.Center, correctedVelocity * 6.9f, ModContent.ProjectileType<ProfanedCrystalMeleeSpear>(), spearDamage, 1f, player.whoAmI, Main.rand.NextBool(player.Calamity().profanedSoulWeaponUsage == 4 ? 5 : 7) ? 1f : 0f, 1f);
                            if (proj.WithinBounds(Main.maxProjectiles))
                            {
                                Main.projectile[proj].DamageType = DamageClass.Summon;
                                Main.projectile[proj].originalDamage = spearBaseDamage;
                            }
                            SoundEngine.PlaySound(SoundID.Item20, player.Center);
                        }

                    }
                    player.Calamity().profanedSoulWeaponUsage++;

                }
                else if (item.CountsAsClass<RangedDamageClass>())
                {
                    if (enrage || Main.rand.NextBool(2)) //100% chance if 50% or lower, else 1 in 2 chance
                    {
                        correctedVelocity *= 20f;
                        Vector2 perturbedspeed = new Vector2(correctedVelocity.X + Main.rand.Next(-3, 4), correctedVelocity.Y + Main.rand.Next(-3, 4)).RotatedBy(MathHelper.ToRadians(3));
                        bool isSmallBoomer = Main.rand.NextDouble() <= (enrage ? 0.2 : 0.3); // 20% chance if enraged, else 30% This is intentional due to literally doubling the amount of projectiles fired.
                        bool isThiccBoomer = isSmallBoomer && Main.rand.NextDouble() <= 0.05; // 5%
                        int projType = isSmallBoomer ? isThiccBoomer ? 1 : 2 : 3;
                        int boomBaseDamage = shouldNerf ? 100 : 200;
                        int boomDamage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(boomBaseDamage);
                        switch (projType)
                        {
                            case 1: //big boomer
                            case 2: //boomer
                                int proj = Projectile.NewProjectile(source, player.Center, perturbedspeed, ModContent.ProjectileType<ProfanedCrystalRangedHuges>(), boomDamage, 0f, player.whoAmI, projType == 1 ? 1f : 0f);
                                if (proj.WithinBounds(Main.maxProjectiles))
                                {
                                    Main.projectile[proj].DamageType = DamageClass.Summon;
                                    Main.projectile[proj].originalDamage = boomBaseDamage;
                                }
                                break;
                            case 3: //bab boomer
                                int proj2 = Projectile.NewProjectile(source, player.Center, perturbedspeed, ModContent.ProjectileType<ProfanedCrystalRangedSmalls>(), boomDamage, 0f, player.whoAmI, 0f);
                                if (proj2.WithinBounds(Main.maxProjectiles))
                                {
                                    Main.projectile[proj2].DamageType = DamageClass.Summon;
                                    Main.projectile[proj2].originalDamage = boomBaseDamage;
                                }
                                break;
                        }
                        if (projType > 1)
                        {
                            SoundEngine.PlaySound(SoundID.Item20, player.Center);
                        }
                    }
                }
                else if (item.CountsAsClass<MagicDamageClass>())
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
                        SoundEngine.PlaySound(SoundID.Item20, player.Center);
                        int magefireBaseDamage = shouldNerf ? 450 : 900;
                        int magefireDamage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(magefireBaseDamage);
                        if (player.HasBuff(BuffID.ManaSickness))
                        {
                            int sickPenalty = (int)(magefireDamage * (0.05f * ((player.buffTime[player.FindBuffIndex(BuffID.ManaSickness)] + 60) / 60)));
                            magefireDamage -= sickPenalty;
                        }
                        int proj = Projectile.NewProjectile(source, player.position, correctedVelocity, ModContent.ProjectileType<ProfanedCrystalMageFireball>(), magefireDamage, 1f, player.whoAmI, enrage ? 1f : 0f);
                        if (proj.WithinBounds(Main.maxProjectiles))
                        {
                            Main.projectile[proj].DamageType = DamageClass.Summon;
                            Main.projectile[proj].originalDamage = magefireBaseDamage;
                        }
                        player.Calamity().profanedSoulWeaponUsage = enrage ? 20 : 25;
                    }
                    if (player.Calamity().profanedSoulWeaponUsage > 0)
                        player.Calamity().profanedSoulWeaponUsage--;
                }
                else if (item.CountsAsClass<ThrowingDamageClass>())
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
                            int shardBaseDamage = shouldNerf ? 88 : 176;
                            int shardDamage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(shardBaseDamage);
                            int proj = Projectile.NewProjectile(source, player.Center, angle.ToRotationVector2() * 8f, ModContent.ProjectileType<ProfanedCrystalRogueShard>(), shardDamage, 1f, player.whoAmI, 0f, 0f);
                            if (proj.WithinBounds(Main.maxProjectiles))
                            {
                                Main.projectile[proj].DamageType = DamageClass.Summon;
                                Main.projectile[proj].originalDamage = shardBaseDamage;
                            }
                            SoundEngine.PlaySound(SoundID.Item20, player.Center);
                        }
                        player.Calamity().profanedSoulWeaponUsage = 0;
                    }
                    else if (player.Calamity().profanedSoulWeaponUsage % (enrage ? 5 : 10) == 0)
                    {
                        float angle = MathHelper.TwoPi / (enrage ? 9 : 18) * (player.Calamity().profanedSoulWeaponUsage / (enrage ? 1 : 10));
                        int shardBaseDamage = shouldNerf ? 110 : 220;
                        int shardDamage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(shardBaseDamage);
                        int proj = Projectile.NewProjectile(source, player.Center, angle.ToRotationVector2() * 8f, ModContent.ProjectileType<ProfanedCrystalRogueShard>(), shardDamage, 1f, player.whoAmI, 1f, 0f);
                        if (proj.WithinBounds(Main.maxProjectiles))
                        {
                            Main.projectile[proj].DamageType = DamageClass.Summon;
                            Main.projectile[proj].originalDamage = shardBaseDamage;
                        }
                        SoundEngine.PlaySound(SoundID.Item20, player.Center);
                    }
                    player.Calamity().profanedSoulWeaponUsage += enrage ? 1 : 2;
                    if (!enrage && player.Calamity().profanedSoulWeaponUsage % 2 != 0)
                        player.Calamity().profanedSoulWeaponUsage--;
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ProfanedSoulArtifact>().
                AddIngredient<DivineGeode>(50).
                AddIngredient<UnholyEssence>(100).
                AddIngredient<ShadowspecBar>(5).
                AddTile<ProfanedCrucible>().
                Register();
        }
    }
}
