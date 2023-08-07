using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Items.Armor.GemTech;

namespace CalamityMod.DataStructures
{
    public class GemTechArmorState
    {
        public int OwnerIndex;
        public int RedGemRegenerationCountdown;
        public int YellowGemRegenerationCountdown;
        public int GreenGemRegenerationCountdown;
        public int BlueGemRegenerationCountdown;
        public int PurpleGemRegenerationCountdown;
        public int PinkGemRegenerationCountdown;
        public int MeleeCrystalCountdown;
        public int LifeRegenBonusCountdown;
        public int MultiWeaponLifeRegenBonusCountdown;
        public GemTechArmorGemType GemThatShouldBeLost = GemTechArmorGemType.Base;
        public GemTechArmorGemType? PreviouslyUsedGem = null;

        public Player Owner => Main.player[OwnerIndex];
        public bool HasInvalidOwner => OwnerIndex < 0 || OwnerIndex >= Main.maxPlayers || !Owner.active;

        public bool IsRedGemActive => RedGemRegenerationCountdown <= 0;
        public bool IsYellowGemActive => YellowGemRegenerationCountdown <= 0;
        public bool IsGreenGemActive => GreenGemRegenerationCountdown <= 0;
        public bool IsBlueGemActive => BlueGemRegenerationCountdown <= 0;
        public bool IsPurpleGemActive => PurpleGemRegenerationCountdown <= 0;
        public bool IsPinkGemActive => PinkGemRegenerationCountdown <= 0;
        public bool AllGemsActive => IsRedGemActive && IsYellowGemActive && IsGreenGemActive && IsBlueGemActive && IsPurpleGemActive && IsPinkGemActive;

        public GemTechArmorState(int ownerIndex)
        {
            OwnerIndex = ownerIndex;
        }

        public bool GemIsActive(GemTechArmorGemType gemType)
        {
            if (gemType == GemTechArmorGemType.Melee)
                return IsYellowGemActive;
            if (gemType == GemTechArmorGemType.Ranged)
                return IsGreenGemActive;
            if (gemType == GemTechArmorGemType.Magic)
                return IsPurpleGemActive;
            if (gemType == GemTechArmorGemType.Summoner)
                return IsBlueGemActive;
            if (gemType == GemTechArmorGemType.Rogue)
                return IsRedGemActive;
            if (gemType == GemTechArmorGemType.Base)
                return IsPinkGemActive;

            return false;
        }

        public void Update()
        {
            // Calculate the gem that should be lost.
            // The accessory check is to catch the edge-case of the Shield of Cthulhu
            // having melee damage for some godforsaken reason.
            if (!Owner.ActiveItem().IsAir && !Owner.ActiveItem().accessory)
            {
                if (Owner.ActiveItem().CountsAsClass<MeleeDamageClass>())
                    GemThatShouldBeLost = GemTechArmorGemType.Melee;
                if (Owner.ActiveItem().CountsAsClass<RangedDamageClass>())
                    GemThatShouldBeLost = GemTechArmorGemType.Ranged;
                if (Owner.ActiveItem().CountsAsClass<MagicDamageClass>())
                    GemThatShouldBeLost = GemTechArmorGemType.Magic;
                if (Owner.ActiveItem().CountsAsClass<SummonDamageClass>())
                    GemThatShouldBeLost = GemTechArmorGemType.Summoner;
                if (Owner.ActiveItem().CountsAsClass<ThrowingDamageClass>())
                    GemThatShouldBeLost = GemTechArmorGemType.Rogue;
            }

            // Mark the base gem for loss if the gem for the previously used item's class is not present.
            if (!GemIsActive(GemThatShouldBeLost))
                GemThatShouldBeLost = GemTechArmorGemType.Base;

            // Decrement countdowns.
            if (RedGemRegenerationCountdown > 0)
            {
                RedGemRegenerationCountdown--;
                if (RedGemRegenerationCountdown == 0)
                    CreateRegenerationEffect(GemTechArmorGemType.Rogue);
            }
            if (YellowGemRegenerationCountdown > 0)
            {
                YellowGemRegenerationCountdown--;
                if (YellowGemRegenerationCountdown == 0)
                    CreateRegenerationEffect(GemTechArmorGemType.Melee);
            }
            if (GreenGemRegenerationCountdown > 0)
            {
                GreenGemRegenerationCountdown--;
                if (GreenGemRegenerationCountdown == 0)
                    CreateRegenerationEffect(GemTechArmorGemType.Ranged);
            }
            if (BlueGemRegenerationCountdown > 0)
            {
                BlueGemRegenerationCountdown--;
                if (BlueGemRegenerationCountdown == 0)
                    CreateRegenerationEffect(GemTechArmorGemType.Summoner);
            }
            if (PurpleGemRegenerationCountdown > 0)
            {
                PurpleGemRegenerationCountdown--;
                if (PurpleGemRegenerationCountdown == 0)
                    CreateRegenerationEffect(GemTechArmorGemType.Magic);
            }
            if (PinkGemRegenerationCountdown > 0)
            {
                PinkGemRegenerationCountdown--;
                if (PinkGemRegenerationCountdown == 0)
                    CreateRegenerationEffect(GemTechArmorGemType.Base);
            }

            if (MeleeCrystalCountdown > 0)
            {
                MeleeCrystalCountdown--;

                // Make the crystal fire countdown go down faster if holding a true melee item.
                if (Owner.ActiveItem().IsTrueMelee())
                    MeleeCrystalCountdown--;

                // Ensure the crystal cooldown does not go below 0 due to the second decrement.
                if (MeleeCrystalCountdown < 0)
                    MeleeCrystalCountdown = 0;
            }

            if (LifeRegenBonusCountdown > 0)
                LifeRegenBonusCountdown--;

            if (MultiWeaponLifeRegenBonusCountdown > 0)
                MultiWeaponLifeRegenBonusCountdown = 0;
        }

        public void MeleeOnHitEffects(NPC target)
        {
            if (!Owner.Calamity().GemTechSet || !IsYellowGemActive || Main.myPlayer != OwnerIndex || MeleeCrystalCountdown > 0)
                return;

            int damage = (int)Owner.GetTotalDamage<MeleeDamageClass>().ApplyTo(GemTechHeadgear.MeleeShardBaseDamage);
            for (int i = 0; i < 14; i++)
            {
                Vector2 shootVelocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.5f, 3.25f);
                Projectile.NewProjectile(Owner.GetSource_ItemUse(Owner.ActiveItem()), target.Center, shootVelocity, ModContent.ProjectileType<GemTechYellowShard>(), damage, 0f, OwnerIndex);
            }

            MeleeCrystalCountdown = GemTechHeadgear.MeleeShardDelay;
        }

        public void RangedOnHitEffects(NPC target, int hitDamage)
        {
            bool hasReachedProjCountLimit = Owner.ownedProjectileCounts[ModContent.ProjectileType<GemTechGreenFlechette>()] > GemTechHeadgear.MaxFlechettes;
            if (!Owner.Calamity().GemTechSet || !IsGreenGemActive || Main.myPlayer != OwnerIndex || hasReachedProjCountLimit)
                return;

            int damage = CalamityUtils.DamageSoftCap((int)(hitDamage * 0.32f), 400);
            Vector2 spawnPosition = Owner.Center + Main.rand.NextVector2Circular(Owner.width, Owner.height) * 1.35f;
            Vector2 shootVelocity = (target.Center - spawnPosition) * 0.04f;
            if (shootVelocity.Length() < 6f)
                shootVelocity = shootVelocity.SafeNormalize(Vector2.UnitY) * 6f;

            spawnPosition -= shootVelocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(15f, 50f);
            Projectile.NewProjectile(Owner.GetSource_ItemUse(Owner.ActiveItem()), spawnPosition, shootVelocity, ModContent.ProjectileType<GemTechGreenFlechette>(), damage, 0f, OwnerIndex);
        }

        public void OnItemUseEffects(Item item)
        {
            if (item.IsAir || item.damage <= 0)
                return;

            LifeRegenBonusCountdown = GemTechHeadgear.AllGemsLifeRegenBoostTime;
            GemTechArmorGemType? usedGemType = null;

            if (item.CountsAsClass<MeleeDamageClass>())
                usedGemType = GemTechArmorGemType.Melee;
            if (item.CountsAsClass<RangedDamageClass>())
                usedGemType = GemTechArmorGemType.Ranged;
            if (item.CountsAsClass<MagicDamageClass>())
                usedGemType = GemTechArmorGemType.Magic;
            if (item.CountsAsClass<SummonDamageClass>())
                usedGemType = GemTechArmorGemType.Summoner;
            if (item.CountsAsClass<ThrowingDamageClass>())
                usedGemType = GemTechArmorGemType.Rogue;

            // The previously used gem has been defined and is different than the currently used one, set the
            // extra life regen bonus for using a different class.
            if (PreviouslyUsedGem != null && usedGemType != PreviouslyUsedGem)
                MultiWeaponLifeRegenBonusCountdown = GemTechHeadgear.AllGemsMultiWeaponLifeRegenBoostTime;

            if (usedGemType.HasValue)
                PreviouslyUsedGem = usedGemType;
        }

        public void PlayerOnHitEffects(int hitDamage)
        {
            // Don't do anything if the player is not wearing the Gem Tech set.
            if (!Owner.Calamity().GemTechSet)
                return;

            bool gemWasLost = false;
            int gemDamage = 0;
            if (hitDamage >= GemTechHeadgear.GemBreakDamageLowerBound)
            {
                // Destroy the rogue gem.
                if (GemIsActive(GemTechArmorGemType.Rogue) && GemThatShouldBeLost == GemTechArmorGemType.Rogue)
                {
                    RedGemRegenerationCountdown = GemTechHeadgear.GemRegenTime;
                    gemDamage = (int)Owner.GetTotalDamage<ThrowingDamageClass>().ApplyTo(GemTechHeadgear.GemDamage);
                    gemWasLost = true;
                }

                // Destroy the melee gem.
                if (GemIsActive(GemTechArmorGemType.Melee) && GemThatShouldBeLost == GemTechArmorGemType.Melee)
                {
                    YellowGemRegenerationCountdown = GemTechHeadgear.GemRegenTime;
                    gemDamage = (int)Owner.GetTotalDamage<MeleeDamageClass>().ApplyTo(GemTechHeadgear.GemDamage);
                    gemWasLost = true;
                }

                // Destroy the ranged gem.
                if (GemIsActive(GemTechArmorGemType.Ranged) && GemThatShouldBeLost == GemTechArmorGemType.Ranged)
                {
                    GreenGemRegenerationCountdown = GemTechHeadgear.GemRegenTime;
                    gemDamage = (int)Owner.GetTotalDamage<RangedDamageClass>().ApplyTo(GemTechHeadgear.GemDamage);
                    gemWasLost = true;
                }

                // Destroy the summoner gem.
                if (GemIsActive(GemTechArmorGemType.Summoner) && GemThatShouldBeLost == GemTechArmorGemType.Summoner)
                {
                    BlueGemRegenerationCountdown = GemTechHeadgear.GemRegenTime;
                    gemDamage = (int)Owner.GetTotalDamage<SummonDamageClass>().ApplyTo(GemTechHeadgear.GemDamage);
                    gemWasLost = true;
                }

                // Destroy the magic gem.
                if (GemIsActive(GemTechArmorGemType.Magic) && GemThatShouldBeLost == GemTechArmorGemType.Magic)
                {
                    PurpleGemRegenerationCountdown = GemTechHeadgear.GemRegenTime;
                    gemDamage = (int)Owner.GetTotalDamage<MagicDamageClass>().ApplyTo(GemTechHeadgear.GemDamage);
                    gemWasLost = true;
                }

                // Destroy the base gem.
                if (GemIsActive(GemTechArmorGemType.Base) && GemThatShouldBeLost == GemTechArmorGemType.Base)
                {
                    PinkGemRegenerationCountdown = GemTechHeadgear.GemRegenTime;
                    gemDamage = (int)Owner.GetTotalDamage<GenericDamageClass>().ApplyTo(GemTechHeadgear.GemDamage);
                    gemWasLost = true;
                }
            }

            if (gemWasLost)
            {
                SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact);
                Vector2 gemPosition = CalculateGemPosition(GemThatShouldBeLost);

                // Softcap the damage. This is done primarily to dampen stealth interactions.
                gemDamage = CalamityUtils.DamageSoftCap(gemDamage, GemTechHeadgear.GemDamageSoftcapThreshold);

                if (Main.myPlayer == OwnerIndex)
                    Projectile.NewProjectile(Owner.GetSource_ItemUse(Owner.ActiveItem()), gemPosition, Vector2.Zero, ModContent.ProjectileType<GemTechArmorGem>(), gemDamage, 0f, OwnerIndex, 0f, (int)GemThatShouldBeLost);
            }
        }

        public void OnDeathEffects()
        {
            // Reset countdowns.
            RedGemRegenerationCountdown = 0;
            YellowGemRegenerationCountdown = 0;
            GreenGemRegenerationCountdown = 0;
            BlueGemRegenerationCountdown = 0;
            PurpleGemRegenerationCountdown = 0;
            PinkGemRegenerationCountdown = 0;
        }

        public void ProvideGemBoosts()
        {
            // Don't apply benefits if the armor set isn't being worn.
            if (!Owner.Calamity().GemTechSet)
                return;

            // The rogue stealth max is present in the Gem Tech Headgear file.
            if (IsRedGemActive)
            {
                Owner.GetCritChance<ThrowingDamageClass>() += (int)(GemTechHeadgear.RogueCritBoost * 100f);
                Owner.GetDamage<ThrowingDamageClass>() += GemTechHeadgear.RogueDamageBoost;
            }

            // The melee speed boost code is present in the Player Misc Effects file.
            if (IsYellowGemActive)
            {
                Owner.GetCritChance<MeleeDamageClass>() += (int)(GemTechHeadgear.MeleeCritBoost * 100f);
                Owner.GetDamage<MeleeDamageClass>() += GemTechHeadgear.MeleeDamageBoost;
            }

            if (IsGreenGemActive)
            {
                Owner.GetCritChance<RangedDamageClass>() += (int)(GemTechHeadgear.RangedCritBoost * 100f);
                Owner.GetDamage<RangedDamageClass>() += GemTechHeadgear.RangedDamageBoost;
            }

            if (IsBlueGemActive)
            {
                Owner.maxMinions += GemTechHeadgear.SummonMinionCountBoost;
                Owner.GetDamage<SummonDamageClass>() += GemTechHeadgear.SummonDamageBoost;
            }

            if (IsPurpleGemActive)
            {
                Owner.statManaMax2 += GemTechHeadgear.MagicManaBoost;
                Owner.GetCritChance<MagicDamageClass>() += (int)(GemTechHeadgear.MagicCritBoost * 100f);
                Owner.GetDamage<MagicDamageClass>() += GemTechHeadgear.MagicDamageBoost;
            }

            if (IsPinkGemActive)
            {
                Owner.statDefense += GemTechHeadgear.BaseGemDefenseBoost;
                Owner.lifeRegen += GemTechHeadgear.BaseGemLifeRegenBoost;
                Owner.moveSpeed += GemTechHeadgear.BaseGemMovementSpeedBoost;
                Owner.jumpSpeedBoost += GemTechHeadgear.BaseGemJumpSpeedBoost;
                Owner.endurance += GemTechHeadgear.BaseGemDRBoost;
            }

            if (LifeRegenBonusCountdown > 0 && AllGemsActive)
            {
                if (MultiWeaponLifeRegenBonusCountdown > 0)
                    Owner.lifeRegen += GemTechHeadgear.AllGemsMultiWeaponUseLifeRegenBoost;
                else
                    Owner.lifeRegen += GemTechHeadgear.AllGemsWeaponUseLifeRegenBoost;
            }

            if (!Owner.ActiveItem().IsAir && !Owner.ActiveItem().CountsAsClass<MagicDamageClass>())
                Owner.manaRegen += GemTechHeadgear.NonMagicItemManaRegenBoost;
        }

        public float CalculateGemOffsetAngle(GemTechArmorGemType gemType, float time)
        {
            return MathHelper.TwoPi * (int)gemType / 6f + time;
        }

        public Vector2 CalculateGemPosition(GemTechArmorGemType gemType)
        {
            float gemTime = Main.GlobalTimeWrappedHourly * 3.41f;
            Vector2 baseDrawOffsetDirection = CalculateGemOffsetAngle(gemType, gemTime).ToRotationVector2() * new Vector2(1f, 0.2f);
            Vector2 gemPosition = Owner.Center + baseDrawOffsetDirection * Owner.width * 1.25f;
            gemPosition.Y += Owner.gfxOffY;
            if (Owner.mount?.Active ?? false)
                gemPosition.Y += Owner.mount.YOffset;
            return gemPosition;
        }

        public void CreateRegenerationEffect(GemTechArmorGemType gemType)
        {
            // Create a little visual effect to accompany the return of the gem and play a magic sound.
            SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact, Owner.Center);

            for (int i = 0; i < 12; i++)
            {
                Dust energyPuff = Dust.NewDustPerfect(CalculateGemPosition(gemType), 267);
                energyPuff.velocity = (MathHelper.TwoPi * i / 12f).ToRotationVector2() * 5f;
                energyPuff.color = GetColorFromGemType(gemType);
                energyPuff.scale = 1.125f;
                energyPuff.alpha = 175;
                energyPuff.noGravity = true;
            }
        }

        public static Color GetColorFromGemType(GemTechArmorGemType gemType)
        {
            switch (gemType)
            {
                case GemTechArmorGemType.Rogue:
                    return new Color(224, 24, 0);
                case GemTechArmorGemType.Melee:
                    return new Color(237, 170, 43);
                case GemTechArmorGemType.Ranged:
                    return new Color(37, 188, 108);
                case GemTechArmorGemType.Summoner:
                    return new Color(37, 119, 206);
                case GemTechArmorGemType.Magic:
                    return new Color(200, 58, 209);
                case GemTechArmorGemType.Base:
                    return new Color(255, 115, 206);
            }

            // Return a transparent color as a fallback.
            return Color.Transparent;
        }
    }
}
