using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Buffs.Summon.Whips;
using CalamityMod.CalPlayer;
using CalamityMod.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
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
    public class ProfanedSoulCrystal : ModItem, ILocalizedModType, IDyeableShaderRenderer
    {
        public static string[] contributorNames = new[] { "IbanPlay", "Chen", "Nincity", "Amber", "Mishiro", "LordMetarex", "Memes" };
        public static int ShieldDurabilityMax = 125;
        public new string LocalizationCategory => "Items.Accessories";
        
        public static int ShieldRechargeDelay = CalamityUtils.SecondsToFrames(5);
        public static int TotalShieldRechargeTime = CalamityUtils.SecondsToFrames(4);
        
        public const int maxMinionRequirement = 10;
        public const int maxPscAnimTime = 120;

        // Interface stuff.
        public float RenderDepth => IDyeableShaderRenderer.ProfanedSoulShieldDepth;

        public bool ShaderIsDyeable => false;

        public bool ShouldDrawDyeableShader
        {
            get
            {
                bool result = false;
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player player = Main.player[i];
                    if (player is null || !player.active || player.outOfRange || player.dead)
                        continue;

                    CalamityPlayer modPlayer = player.Calamity();

                    // Do not render the shield if its visibility is off (or it does not exist)
                    bool isVanityOnly = modPlayer.pSoulShieldVisible && !modPlayer.pSoulArtifact;
                    bool shouldNotDraw = modPlayer.andromedaState >= AndromedaPlayerState.LargeRobot; //I am not dealing with drawing that :taxevasion:
                    bool shieldExists = isVanityOnly || modPlayer.pSoulShieldDurability > 0;
                    bool shouldntDraw = !modPlayer.pSoulShieldVisible || modPlayer.drawnAnyShieldThisFrame || shouldNotDraw || !shieldExists;
                    result |= !shouldntDraw;
                }
                return result;
            }
        }

        public enum ProfanedSoulCrystalState
        {
            Vanity, //pre scal/draedon or insufficient minion slots
            Buffs, //regular psc, offense guardian functionality
            Enraged, //psc but night, some attacks are faster
            Empowered //psc but no other minions, healer guardian functionality, inherits all other functionality (except vanity) and goes even further beyond, any remaining attack changes are here
        }

        public void DrawDyeableShader(SpriteBatch spriteBatch) => ProfanedSoulArtifact.DrawProfanedSoulShields();

        internal static ProfanedSoulCrystalState GetPscStateFor(Player player, bool ignoreNoBuffs = false)
        {
            if (!player.Calamity().profanedCrystalBuffs && !ignoreNoBuffs)
                return ProfanedSoulCrystalState.Vanity; //vanity if no buffs
            
            //vanity check during animation
            if (ignoreNoBuffs &&
                (!DownedBossSystem.downedCalamitas || !DownedBossSystem.downedExoMechs ||
                 (player.maxMinions - player.slotsMinions) < maxMinionRequirement) ||
                player.Calamity().profanedCrystalForce || !player.HasBuff<ProfanedCrystalBuff>())
            {
                return ProfanedSoulCrystalState.Vanity; //failsafe for vanity
            }
            
            var noMinions = player.slotsMinions == 0;
            var noSentries = !Main.projectile.Any(proj => proj.active && proj.owner == player.whoAmI && proj.sentry);
            if (noMinions && noSentries)
                return ProfanedSoulCrystalState.Empowered; //immediately check for empowered as it overrides everything else
            
            return !Main.dayTime ? ProfanedSoulCrystalState.Enraged : //check for enrage
                ProfanedSoulCrystalState.Buffs; //return buffs as the sole remaining option
        }

        internal static Color GetColorForPsc(int pscState, bool day, int alpha = 0)
        {
            return ((ProfanedSoulCrystalState)pscState) switch
            {
                ProfanedSoulCrystalState.Vanity => new Color(231, 160, 56, alpha),
                ProfanedSoulCrystalState.Buffs => new Color(255, 110, 56, alpha),
                ProfanedSoulCrystalState.Enraged => new Color(145, 208, 188, alpha),
                ProfanedSoulCrystalState.Empowered => day ? new Color(255, 75, 13, alpha) : new Color(84, 186, 163, alpha),
                _ => Color.White //defaults to white, should not be white
            };
        }

        internal static Color GetLerpedColorForPsc(CalamityPlayer calPlayer)
        {
            if (calPlayer.pscLerpColor != Color.White)
                return calPlayer.pscLerpColor; //already set the lerp colour this frame, calculating it again is redundant
            
            bool day = Main.dayTime;
            double totalTime = Main.dayTime ? Main.dayLength : Main.dayLength + Main.nightLength;
            double currentTime = Main.time;
            double midday = Main.dayLength / 2;
            double midnight = Main.nightLength / 2;
            Color dayColor = GetColorForPsc(calPlayer.pscState, day);
            Color nightColor = GetColorForPsc(calPlayer.pscState > (int)ProfanedSoulCrystalState.Enraged
                        ? (int)ProfanedSoulCrystalState.Empowered
                        : (int)ProfanedSoulCrystalState.Enraged, false);
            var targetColor = Main.dayTime ? dayColor : nightColor;
            var nonTargetColor = Main.dayTime ? nightColor : dayColor;
            var targetTime = Main.dayTime ? midday : midnight;
            var interpolant = Utils.GetLerpValue(totalTime, targetTime, currentTime, false);
            var result = Color.White;
            if (!Main.dayTime && Main.time > midnight)
            {
                result = Color.Lerp(nightColor, dayColor, 2f - (float)interpolant);
            }

            else if (Main.dayTime && Main.time > midday)
            {
                result = Color.Lerp(nightColor, dayColor, (float)interpolant);
            }

            if (result == Color.White)
            {
                result = Color.Lerp(nonTargetColor, targetColor, (Main.time < midday ? 2f : 0f) - (float)interpolant);
            }
                

            calPlayer.pscLerpColor = result;
            return result;
        }
        
        /**
         * Notes: Drops from providence if the only damage source during the fight is from typeless damage or the profaned soul and the owners of those babs do not have profaned crystal.
         * All projectiles are in ProfanedSoulCrystalProjectiles.cs in the summon projectile directory
         * the day/night buffs are in calamityplayermisceffects
         * bab spears being fired happens at the bottom of calplayer
         * Animation of legs is postupdate, animation of wings is frameeffects.
         * Projectiles transformed are ONLY affected by alldamage and summon damage bonuses, likewise the weapon's base damage/usetime is NOT taken into account.
         * The on-hit effect for the whip is in globalnpc
         */
        public override void Load()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Accessories/ProfanedSoulTransHead", EquipType.Head, this);
            EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Accessories/ProfanedSoulTransHeadNight", EquipType.Head, this, "PscNightHead");
            EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Accessories/ProfanedSoulTransBody", EquipType.Body, this);
            EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Accessories/ProfanedSoulTransLegs", EquipType.Legs, this);
            EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Accessories/ProfanedSoulTransLegsNight", EquipType.Legs, this, "PscNightLegs");
            EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Accessories/Wings/ProfanedSoulTransWings", EquipType.Wings, this);
            EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Accessories/Wings/ProfanedSoulTransWingsNight", EquipType.Wings, this, "PscNightWings");
        }

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(8, 4));
            ItemID.Sets.AnimatesAsSoul[Type] = true;

            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlotBody = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
            ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
            ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;

            int equipSlotLegs = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);
            ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegs] = true;
            ArmorIDs.Legs.Sets.OverridesLegs[equipSlotLegs] = true;

            int equipSlotNightLegs = EquipLoader.GetEquipSlot(Mod, "PscNightLegs", EquipType.Legs);
            ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotNightLegs] = true;
            ArmorIDs.Legs.Sets.OverridesLegs[equipSlotNightLegs] = true;
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

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return incomingItem.type != ModContent.ItemType<ProfanedSoulArtifact>();
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
                string full = this.GetLocalization("FullTooltip").Format(maxMinionRequirement, manaCost);
                tooltips.FindAndReplace("[STATUS]", full);
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();

            modPlayer.pSoulArtifact = true;
            modPlayer.profanedCrystal = true;
            if (!modPlayer.profanedCrystalPrevious && player.ownedProjectileCounts[ModContent.ProjectileType<PscTransformAnimation>()] == 0)
            {
                modPlayer.pSoulShieldDurability = 1;
                modPlayer.profanedCrystalAnim = maxPscAnimTime;
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<PscTransformAnimation>(), 0, 0f, player.whoAmI);
            }
                

            modPlayer.profanedCrystalHide = hideVisual || modPlayer.profanedCrystalAnim > 0;
            modPlayer.pSoulShieldVisible = !hideVisual;
            
            DetermineTransformationEligibility(player);
        }

        public override void UpdateVanity(Player player)
        {
            player.Calamity().profanedCrystalHide = false;
            player.Calamity().profanedCrystalForce = true;
        }

        internal static void DetermineTransformationEligibility(Player player)
        {
            //short circuit immediately if profanedcrystalbuffs has already been set
            
            if (!player.Calamity().profanedCrystalBuffs && player.Calamity().profanedCrystalAnim == -1 && DownedBossSystem.downedCalamitas && DownedBossSystem.downedExoMechs && (player.maxMinions - player.slotsMinions) >= maxMinionRequirement && !player.Calamity().profanedCrystalForce && player.HasBuff<ProfanedCrystalBuff>())
            {
                player.Calamity().profanedCrystalBuffs = true;
                player.Calamity().pscState = (int)GetPscStateFor(player); //update psc state, default is 0 which is the same as the int value of vanity
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
                item.CountsAsClass<ThrowingDamageClass>() ? 4 : 
                item.CountsAsClass<SummonMeleeSpeedDamageClass>() ? 5 : -1;
            if (weaponType > 0)
            {
                if (player.Calamity().profanedSoulWeaponType != weaponType || player.Calamity().profanedSoulWeaponUsage >= 370)
                {
                    player.Calamity().profanedSoulWeaponType = weaponType;
                    player.Calamity().profanedSoulWeaponUsage = 0;
                }
                Vector2 correctedVelocity = Main.MouseWorld - player.Center;
                correctedVelocity.Normalize();
                bool empowered = player.Calamity().pscState == (int)ProfanedSoulCrystalState.Empowered;
                bool enraged = player.Calamity().pscState >= (int)ProfanedSoulCrystalState.Enraged;
                if (item.CountsAsClass<MeleeDamageClass>())
                {
                    if (player.Calamity().profanedSoulWeaponUsage % (enraged ? 4 : 6) == 0)
                    {
                        if (player.Calamity().profanedSoulWeaponUsage > 0 && player.Calamity().profanedSoulWeaponUsage % (enraged ? 20 : 30) == 0) //every 5 shots is a shotgun spread
                        {
                            int numProj = 5;

                            correctedVelocity *= 20f;
                            int spread = -6;
                            for (int i = 0; i < numProj; i++)
                            {
                                Vector2 perturbedspeed = new Vector2(correctedVelocity.X, correctedVelocity.Y).RotatedBy(MathHelper.ToRadians(spread));
                                int separation = (i * 4) - 8;
                                int spearBaseDamage = 350;
                                int spearDamage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(spearBaseDamage);
                                spearDamage = player.ApplyArmorAccDamageBonusesTo(spearDamage);

                                int proj = Projectile.NewProjectile(source, player.Center.X, player.Center.Y - separation, perturbedspeed.X, perturbedspeed.Y, ModContent.ProjectileType<ProfanedCrystalMeleeSpear>(), spearDamage, 1f, player.whoAmI, Main.rand.NextBool(player.Calamity().profanedSoulWeaponUsage == 4 ? 5 : 7) ? 1f : 0f);
                                if (proj.WithinBounds(Main.maxProjectiles))
                                {
                                    Main.projectile[proj].DamageType = DamageClass.Summon;
                                    Main.projectile[proj].originalDamage = spearBaseDamage;
                                }
                                spread += 3;
                                SoundEngine.PlaySound(SoundID.Item20, player.Center);
                            }
                            player.Calamity().profanedSoulWeaponUsage = 0;
                        }
                        else
                        {
                            int spearBaseDamage = 250;
                            int spearDamage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(spearBaseDamage);
                            spearDamage = player.ApplyArmorAccDamageBonusesTo(spearDamage);

                            int proj = Projectile.NewProjectile(source, player.Center, correctedVelocity * 14f, ModContent.ProjectileType<ProfanedCrystalMeleeSpear>(), spearDamage, 1f, player.whoAmI, Main.rand.NextBool(player.Calamity().profanedSoulWeaponUsage == 4 ? 5 : 7) ? 1f : 0f, 1f);
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
                    if (enraged || Main.rand.NextBool()) //100% chance if 50% or lower, else 1 in 2 chance
                    {
                        correctedVelocity *= 20f;
                        Vector2 perturbedspeed = new Vector2(correctedVelocity.X + Main.rand.Next(-3, 4), correctedVelocity.Y + Main.rand.Next(-3, 4)).RotatedBy(MathHelper.ToRadians(3));
                        bool isSmallBoomer = Main.rand.NextDouble() <= (enraged && !empowered ? 0.2 : 0.3); // 20% chance if enraged, else 30% This is intentional due to literally doubling the amount of projectiles fired.
                        bool isThiccBoomer = isSmallBoomer && Main.rand.NextDouble() <= 0.05; // 5%
                        int projType = isSmallBoomer ? isThiccBoomer ? 1 : 2 : 3;
                        int boomBaseDamage = 200;
                        int boomDamage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(boomBaseDamage);
                        boomDamage = player.ApplyArmorAccDamageBonusesTo(boomDamage);

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
                        int magefireBaseDamage = 900;
                        int mageFireDamage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(magefireBaseDamage);
                        if (player.HasBuff(BuffID.ManaSickness))
                        {
                            int sickPenalty = (int)(mageFireDamage * (0.05f * ((player.buffTime[player.FindBuffIndex(BuffID.ManaSickness)] + 60) / 60)));
                            mageFireDamage -= sickPenalty;
                        }
                        mageFireDamage = player.ApplyArmorAccDamageBonusesTo(mageFireDamage);

                        int proj = Projectile.NewProjectile(source, player.position, correctedVelocity, ModContent.ProjectileType<ProfanedCrystalMageFireball>(), mageFireDamage, 1f, player.whoAmI, empowered ? 1f : 0f);
                        if (proj.WithinBounds(Main.maxProjectiles))
                        {
                            Main.projectile[proj].DamageType = DamageClass.Summon;
                            Main.projectile[proj].originalDamage = magefireBaseDamage;
                        }
                        player.Calamity().profanedSoulWeaponUsage = enraged ? 20 : 25;
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
                    
                    if (player.Calamity().profanedSoulWeaponUsage >= (empowered ? 120 : 360))
                    {
                        float crystalCount = 36f;
                        for (float i = 0; i < crystalCount; i++)
                        {
                            float angle = MathHelper.TwoPi / crystalCount * i;
                            int shardBaseDamage = 176;
                            int shardDamage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(shardBaseDamage);
                            shardDamage = player.ApplyArmorAccDamageBonusesTo(shardDamage);

                            int proj = Projectile.NewProjectile(source, player.Center, angle.ToRotationVector2() * 12f, ModContent.ProjectileType<ProfanedCrystalRogueShard>(), shardDamage, 1f, player.whoAmI, 0f, 0f);
                            if (proj.WithinBounds(Main.maxProjectiles))
                            {
                                Main.projectile[proj].DamageType = DamageClass.Summon;
                                Main.projectile[proj].originalDamage = shardBaseDamage;
                            }
                            SoundEngine.PlaySound(SoundID.Item20, player.Center);
                        }
                        player.Calamity().profanedSoulWeaponUsage = 0;
                    }
                    else if (player.Calamity().profanedSoulWeaponUsage % (empowered ? 5 : 10) == 0)
                    {
                        int chains = empowered ? 3 : 1;
                        int totalShardProjectiles = empowered ? 360 / 5 : 360 / 10;
                        int shardBaseDamage = empowered ? 125 : 220;
                        int shardDamage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(shardBaseDamage);
                        shardDamage = player.ApplyArmorAccDamageBonusesTo(shardDamage);

                        float interval = totalShardProjectiles / chains * (empowered ? 5f : 10f);
                        double patternInterval = Math.Floor(player.Calamity().profanedSoulWeaponUsage / interval);
                        if (patternInterval % 2 == 0)
                        {
                            double radians = MathHelper.TwoPi / chains;
                            double angleA = radians * 0.5;
                            double angleB = MathHelper.ToRadians(90f) - angleA;
                            float velocityX = (float)(2f * Math.Sin(angleA) / Math.Sin(angleB));
                            Vector2 spinningPoint = new Vector2(velocityX, -2f);
                            for (int i = 0; i < chains; i++)
                            {
                                Vector2 vector2 = spinningPoint.RotatedBy(radians * i + MathHelper.ToRadians(player.Calamity().profanedSoulWeaponUsage));
                                vector2.Normalize();
                                int proj = Projectile.NewProjectile(source, player.Center, vector2 * 12f, ModContent.ProjectileType<ProfanedCrystalRogueShard>(), shardDamage, 1f, player.whoAmI, 1f, 0f);
                                if (proj.WithinBounds(Main.maxProjectiles))
                                {
                                    Main.projectile[proj].DamageType = DamageClass.Summon;
                                    Main.projectile[proj].originalDamage = shardBaseDamage;
                                }
                            }

                            SoundEngine.PlaySound(SoundID.Item20, player.Center);
                        }
                    }
                    player.Calamity().profanedSoulWeaponUsage += !empowered ? 2 : 1;

                }
                else if (item.CountsAsClass<SummonMeleeSpeedDamageClass>())
                {
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<ProfanedCrystalWhip>()] == 0)
                    {
                        player.Calamity().profanedSoulWeaponUsage = 0;
                    }

                    if (player.Calamity().profanedSoulWeaponUsage == 0)
                    {
                        int whipBaseDamage = 250;
                        int whipDamage = (int)player.GetTotalDamage<SummonMeleeSpeedDamageClass>().ApplyTo(whipBaseDamage);
                        whipDamage = player.ApplyArmorAccDamageBonusesTo(whipDamage);

                        var buffed = player.HasBuff<ProfanedCrystalWhipBuff>();
                        correctedVelocity *= buffed ? 10f : 8f;
                        int permittedDistance = player.HasBuff<ProfanedCrystalWhipBuff>() ? 10 : 8;
                        correctedVelocity.X = Math.Clamp(correctedVelocity.X, -permittedDistance, permittedDistance);
                        correctedVelocity.Y = Math.Clamp(correctedVelocity.Y, -permittedDistance, permittedDistance);

                        player.ChangeDir(MathF.Sign(correctedVelocity.X));
                        Projectile.NewProjectile(source, player.Center, correctedVelocity, ModContent.ProjectileType<ProfanedCrystalWhip>(), whipDamage, 1f, player.whoAmI);
                        player.Calamity().profanedSoulWeaponUsage = 10;
                    }

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
