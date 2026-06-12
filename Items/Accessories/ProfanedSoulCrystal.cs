using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Buffs.Summon.Whips;
using CalamityMod.CalPlayer;
using CalamityMod.DataStructures;
using CalamityMod.Items.BaseItems;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    //Developer item, dedicatee: Mishiro Usui/Amber Sienna
    public class ProfanedSoulCrystal : TransformationAccessory, ILocalizedModType, IDyeableShaderRenderer
    {
        public static string[] contributorNames = new[] { "IbanPlay", "Chen", "Nincity", "Amber", "Mishiro", "LordMetarex" };
        public static int ShieldDurabilityMax = 100;
        public new string LocalizationCategory => "Items.Accessories";

        public static int ShieldRechargeDelay = CalamityUtils.SecondsToFrames(5);
        public static int TotalShieldRechargeTime = CalamityUtils.SecondsToFrames(4);

        public const int maxMinionRequirement = 10;
        public const int maxPscAnimTime = 120;
        public static SummonTag SummonTag = new() {
            MultiplicativeTagDamage = 0.2f,
            TagModifyHitEffects = ApplyTagModifyHit,
            AutoDrawTooltip = false
        };

        public static void ApplyTagModifyHit(Projectile proj, NPC npc, ref NPC.HitModifiers modifiers, ref float tagDamageMult, ref float critChance)
        {
            if (Main.player[proj.owner].Calamity().pscState >= (int)ProfanedSoulCrystalState.Buffs)
            {
                var empowered = Main.player[proj.owner].Calamity().pscState == (int)ProfanedSoulCrystalState.Empowered;
                //20% is balanced for non empowered, while 40% helps ensure psc remains balanced at empowered tier
                //Some PSC projectiles receive a reduced amount of benefit from this, for balancing purposes
                modifiers.ScalingBonusDamage += (empowered ? 0.4f : SummonTag.MultiplicativeTagDamage) * tagDamageMult;
                if (!Main.dedServ)
                {
                    var color = ProvUtils.GetColorBasedOnEnrage(!Main.dayTime, 0);
                    float power = Math.Min(npc.height / 100f, 3f);
                    var position = new Vector2(Main.rand.NextFloat(npc.Left.X, npc.Right.X), Main.rand.NextFloat(npc.Top.Y, npc.Bottom.Y));
                    var particle = new FlameParticle(position, 50, 0.25f, power, color * (Main.dayTime ? 1f : 1.25f), color * (Main.dayTime ? 1.25f : 1f));
                    GeneralParticleHandler.SpawnParticle(particle);
                }
            }
        }

        // Interface stuff.
        public int OwnerPlayer { get; set; }
        public float RenderDepth => IDyeableShaderRenderer.ProfanedSoulShieldDepth;
        public bool ShaderIsDyeable => false;

        public bool ShouldDrawDyeableShader
        {
            get
            {
                if (CalamityClientConfig.Instance.EnergyShieldOpacity <= 0.0f)
                    return false;

                if (OwnerPlayer < 0 || OwnerPlayer >= Main.maxPlayers)
                    return false;

                var player = Main.player[OwnerPlayer];
                if (player is null)
                    return false;

                if (player.outOfRange || player.dead)
                    return false;

                CalamityPlayer modPlayer = player.Calamity();
                if (modPlayer.drawingParameters.ProfanedShieldCharge <= 0.0f)
                    return false;

                return true;
            }
        }

        public enum ProfanedSoulCrystalState
        {
            Vanity, //pre scal/draedon or insufficient minion slots
            Buffs, //regular psc, offense guardian functionality
            Enraged, //psc but night, some attacks are faster
            Empowered //psc but no other minions, healer guardian functionality, inherits all other functionality (except vanity) and goes even further beyond, any remaining attack changes are here
        }

        public void DrawDyeableShader(SpriteBatch spriteBatch) => ProfanedSoulArtifact.DrawProfanedSoulShields(OwnerPlayer);

        internal static ProfanedSoulCrystalState GetPscStateFor(Player player, bool ignoreNoBuffs = false)
        {
            if (!player.Calamity().profanedCrystalBuffs && !ignoreNoBuffs)
                return ProfanedSoulCrystalState.Vanity; //vanity if no buffs

            //vanity check during animation
            if (ignoreNoBuffs &&
                (!DownedBossSystem.downedCalamitas || !DownedBossSystem.downedExoMechs ||
                 (player.maxMinions - player.slotsMinions) < maxMinionRequirement) ||
                player.Transformation().Type == ModContent.ItemType<ProfanedSoulCrystal>() || !player.HasBuff<ProfanedCrystalBuff>())
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
        public override string AssetPath => "CalamityMod/Items/Accessories/";

        public override (EquipType Type, string AssetName, string EquipName)[] EquipSlots =>
        [
            (EquipType.Head, "ProfanedSoulTrans", null),
            (EquipType.Body, "ProfanedSoulTrans", null),
            (EquipType.Legs, "ProfanedSoulTrans", null),
            (EquipType.Wings, "ProfanedSoulTrans", null),
            (EquipType.Head, "ProfanedSoulTransNight", "PscNightHead"),
            (EquipType.Legs, "ProfanedSoulTransNight", "PscNightLegs"),
            (EquipType.Wings, "ProfanedSoulTransNight", "PscNightWings"),
            (EquipType.Face, null, null), //results in setting this equip slot to -1
        ];

        public override void ArmorIDSets()
        {
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

        public override (SoundStyle sound, int delay)? HurtSound(Player p) => (p.Calamity().pSoulShieldDurability > 0 ? ProfanedGuardianDefender.ShieldDeathSound : Providence.HurtSound, 20);

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(8, 4));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<HolyFlames>()];
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            SummonTag.TagItem = Type;
            SummonTag.TagTexture = TextureAssets.Item[Type];
            Item.width = 50;
            Item.height = 50;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.Calamity().devItem = true;
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player) => incomingItem.type != ModContent.ItemType<ProfanedSoulArtifact>();

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            bool scal = DownedBossSystem.downedCalamitas;
            bool draedon = DownedBossSystem.downedExoMechs;
            if (!scal && !draedon)
            {
                string reject = this.GetLocalization("LockedBoth").Format(this.GetLocalizedValue("ExoMechsLock"), this.GetLocalizedValue("CalamitasLock")) + "\n" + this.GetLocalizedValue("Reject");
                tooltips.FindAndReplace("[STATUS]", reject);

                TooltipLine linePrice = tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Price");
                if (linePrice != null)
                    linePrice.Text = "";
            }
            else if (!scal || !draedon)
            {
                string reject = this.GetLocalization("Locked").Format(!draedon ? this.GetLocalizedValue("ExoMechsLock") : this.GetLocalizedValue("CalamitasLock")) + "\n" + this.GetLocalizedValue("Reject");
                tooltips.FindAndReplace("[STATUS]", reject);

                TooltipLine linePrice = tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Price");
                if (linePrice != null)
                    linePrice.Text = "";
            }
            else
            {
                string manaCost = (100 * Main.LocalPlayer.manaCost).ToString("N0");
                string full = this.GetLocalization("FullTooltip").Format(maxMinionRequirement, manaCost);
                tooltips.FindAndReplace("[STATUS]", full);
            }
        }

        public override bool CustomSetEquipType(Player player, EquipType type, Mod mod, string name)
        {
            switch (type)
            {
                case EquipType.Legs:
                    player.legs = EquipLoader.GetEquipSlot(Mod, Main.dayTime ? "ProfanedSoulCrystal" : "PscNightLegs", type);
                    return true;
                case EquipType.Head:
                    player.head = EquipLoader.GetEquipSlot(Mod, Main.dayTime ? "ProfanedSoulCrystal" : "PscNightHead", type);
                    return true;
                case EquipType.Wings:
                    player.wings = EquipLoader.GetEquipSlot(Mod, Main.dayTime ? "ProfanedSoulCrystal" : "PscNightWings", type);
                    return true;
            }
            return false;
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
            if (DownedBossSystem.downedCalamitas && DownedBossSystem.downedExoMechs)
                player.Calamity().profanedSoulRelicBuff = true;

            modPlayer.pSoulShieldVisible = !hideVisual;

            DetermineTransformationEligibility(player);
        }

        internal static void DetermineTransformationEligibility(Player player)
        {
            //short circuit immediately if profanedcrystalbuffs has already been set
            if (!player.Calamity().profanedCrystalBuffs && player.Calamity().profanedCrystalAnim == -1 && DownedBossSystem.downedCalamitas && DownedBossSystem.downedExoMechs && (player.maxMinions - player.slotsMinions) >= maxMinionRequirement && player.HasBuff<ProfanedCrystalBuff>())
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

                // 14NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
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
                    if (player.Calamity().profanedSoulWeaponUsage == 0 && !player.silence && player.CheckMana(manaCost, true))
                    {
                        player.manaRegenDelay = (int)player.maxRegenDelay;
                        correctedVelocity *= 25f;
                        SoundEngine.PlaySound(SoundID.Item20, player.Center);
                        int magefireBaseDamage = 900;
                        int mageFireDamage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(magefireBaseDamage);
                        if (player.HasBuff(BuffID.ManaSickness))
                        {
                            int sickPenalty = (int)(mageFireDamage * (0.05f * ((player.buffTime[player.FindBuffIndex(BuffID.ManaSickness)] + 60) / 60)));
                            mageFireDamage -= sickPenalty;
                        }

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

        public override void TransformFrameEffects(Player player)
        {
            bool enrage = player.Calamity().pscState >= (int)ProfanedSoulCrystal.ProfanedSoulCrystalState.Enraged;

            if (profanedCrystalWingCounter.Value == 0)
            {
                int key = profanedCrystalWingCounter.Key;
                profanedCrystalWingCounter = new KeyValuePair<int, int>(key == 3 ? 0 : key + 1, enrage ? 5 : 8);
            }

            player.wingFrame = profanedCrystalWingCounter.Key;
            profanedCrystalWingCounter = new KeyValuePair<int, int>(profanedCrystalWingCounter.Key, profanedCrystalWingCounter.Value - 1);
            player.armorEffectDrawOutlines = true;
            if (player.Calamity().profanedCrystalBuffs)
            {
                player.armorEffectDrawShadow = true;
                if (enrage)
                {
                    player.armorEffectDrawOutlinesForbidden = true;
                }
            }
        }

        public KeyValuePair<int, int> profanedCrystalWingCounter = new KeyValuePair<int, int>(1, 10);
        public KeyValuePair<int, int> profanedCrystalAnimCounter = new KeyValuePair<int, int>(0, 10);

        private bool IsValidTransitionFrame(AnimationType currentAnim, AnimationType newAnim, int frame, int counter) //this exists so it doesn't loop through the entire walk/idle anim just to find one frame for switching.
        {
            bool result = newAnim != AnimationType.Jump && currentAnim != AnimationType.Jump;
            if (currentAnim == AnimationType.Walk && newAnim == AnimationType.Idle)
            {
                result = counter <= 0 && (frame == 11 || frame == 15 || frame == 19);
            }
            else if (currentAnim == AnimationType.Idle && newAnim == AnimationType.Walk)
            {
                result = counter <= 0 && (frame == 2 || frame == 6);
            }
            return currentAnim != newAnim && result; //swapping to jumps should be instant, no need to check the counter here
        }

        private int HandlePSCAnimationFrames(Player player, AnimationType newType)
        {
            int key = profanedCrystalAnimCounter.Key; //0-based indexing
            int value = profanedCrystalAnimCounter.Value - 1;
            AnimationType currentType = key < 8 ? AnimationType.Idle : key == 8 ? AnimationType.Jump : AnimationType.Walk;

            bool isInvalidTransFrame = !IsValidTransitionFrame(currentType, newType, key, value); //to make the transition between walk and idle frames less jarring and smoother
            AnimationType type = isInvalidTransFrame ? newType : currentType;
            int frameCount = type == AnimationType.Walk || (player.Calamity().profanedCrystal && player.statLife <= (int)(player.statLifeMax2 * 0.5)) ? 7 : 10;
            int lowerRange = type == AnimationType.Idle ? 0 : type == AnimationType.Jump ? 8 : 9;
            int upperRange = type == AnimationType.Idle ? 7 : type == AnimationType.Jump ? 8 : 22;
            if (value <= 0 || !isInvalidTransFrame)
            {
                value = frameCount;
                if (key >= lowerRange && key < upperRange)
                    key++;
                else
                    key = lowerRange;
            }
            profanedCrystalAnimCounter = new KeyValuePair<int, int>(key, value);
            return profanedCrystalAnimCounter.Key;
        }

        public override void TransformPostUpdate(Player player)
        {
            bool validEquipSlot = player.legs == EquipLoader.GetEquipSlot(Mod, "ProfanedSoulCrystal", EquipType.Legs) ||
                                  player.legs == EquipLoader.GetEquipSlot(Mod, "PscNightLegs", EquipType.Legs);
            if (player.Transformation().Type == ModContent.ItemType<ProfanedSoulCrystal>() && validEquipSlot)
            {
                bool usingCarpet = player.carpetTime > 0 && player.controlJump; //doesn't make sense for carpet to use jump frame since you have solid ground
                AnimationType animType = AnimationType.Walk;
                if ((player.sliding || player.velocity.Y != 0 || player.mount.Active || (player.grappling[0] != -1 || !player.CheckSolidGround()) || player.GoingDownWithGrapple) && !usingCarpet)
                    animType = AnimationType.Jump;
                else if (player.velocity.X == 0 || usingCarpet)
                    animType = AnimationType.Idle;
                int frame = HandlePSCAnimationFrames(player, animType);
                player.legFrame.Y = player.legFrame.Height * frame;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ProfanedSoulArtifact>().
                AddIngredient<ShadowspecBar>(5).
                AddIngredient<DivineGeode>(50).
                AddIngredient<UnholyEssence>(100).
                AddTile<ProfanedCrucible>().
                AddDecraftCondition(CalamityConditions.DownedSupremeCalamitas, CalamityConditions.DownedExoMechs).
                Register();
        }
    }
}
