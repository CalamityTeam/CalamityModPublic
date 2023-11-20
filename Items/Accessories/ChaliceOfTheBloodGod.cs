using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Particles;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [LegacyName("CoreOfTheBloodGod")]
    public class ChaliceOfTheBloodGod : ModItem, ILocalizedModType
    {
        // This value serves three purposes.
        // 1 - If the player takes less damage than this post-mitigation, CotBG refuses to activate and will not make that damage a bleedout.
        // 2 - CotBG reduces incoming damage to this value when it does activate.
        // 3 - If there is less bleedout left than this number, the remainder is dealt instantly in a single frame.
        //
        // Dying to this sudden last few points of health loss has a special death message.
        internal static readonly int MinAllowedDamage = 5;

        // Drinking a healing potion clears 50% of the bleedout buffer.
        internal const float HealingPotionBufferClear = 0.5f;

        // This is per frame. The ideal is 50% per second, so it's 0.83% per frame.
        internal static readonly double BleedoutExponentialDecay = 0.0083333333333;
        internal static readonly Color BleedoutBufferDamageTextColor = new(230, 40, 100);
        
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 12));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 32;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.rare = ModContent.RarityType<DarkBlue>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();

            // This applies the +25% health boost and the bleedout buffer effect.
            // Health boost intentionally stacks with Blood Pact.
            modPlayer.chaliceOfTheBloodGod = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BloodPact>().
                AddIngredient<BloodstoneCore>(5).
                AddIngredient<AscendantSpiritEssence>(4).
                AddTile<CosmicAnvil>().
                Register();
        }

        internal static void HandleBleedout(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();

            // If the player has at least a few more points of damage to take, they are bleeding.
            // This doesn't count down to exactly 1 point of damage because that could take an annoyingly long time.
            if (modPlayer.chaliceOfTheBloodGod && modPlayer.chaliceBleedoutBuffer > MinAllowedDamage)
            {
                double amountBledThisFrame = modPlayer.chaliceBleedoutBuffer * BleedoutExponentialDecay;
                modPlayer.chaliceDamagePointPartialProgress += amountBledThisFrame;

                int healthToLose = (int)modPlayer.chaliceDamagePointPartialProgress;

                // If more than one damage point is done this frame, reduce health appropriately
                if (healthToLose > 0)
                {
                    player.statLife -= healthToLose;

                    // Take away the integer part of the partial progress when this occurs.
                    modPlayer.chaliceDamagePointPartialProgress -= healthToLose;

                    // If this reduces the player's health to zero, make sure they actually die.
                    if (player.statLife <= 0)
                    {
                        player.KillMe(PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.ChaliceOfTheBloodGod" + Main.rand.Next(1, 4 + 1)).Format(player.name)), modPlayer.chaliceBleedoutBuffer, 0, false);
                    }
                }

                // Particles are spawned even if the bleed damage this frame is zero, so there's always a continuous trickle of blood.
                SpawnBloodParticles(player, healthToLose);

                // Actually reduce the remaining buffer.
                modPlayer.chaliceBleedoutBuffer *= 1f - BleedoutExponentialDecay;
            }

            // This else is reached in 3 cases.
            //
            // 1. The player unequips Chalice of the Blood God while still bleeding.
            //   In this case, the rest of the bleed is dealt in a single frame as punishment.
            //   This prevents hotswapping accessories in while still benefitting from the bleedout survivability.
            //
            // 2. The player has Chalice of the Blood God equipped, but there are very few points of damage left to take.
            //   In this case, the rest of the bleed is dealt in a single frame for convenience.
            //
            // 3. The player doesn't have the accessory equipped at all, and isn't bleeding.
            //   In this case, nothing happens, because there's no bleed damage to take.
            else
            {
                // Deal the entire remaining buffer as damage in a single frame.
                if (modPlayer.chaliceBleedoutBuffer > 0)
                {
                    int remainingDamage = (int)modPlayer.chaliceBleedoutBuffer + 1;
                    player.statLife -= remainingDamage;

                    // Intentionally produces an extra burst of particles so it's more obvious when bleeding ends.
                    SpawnBloodParticles(player, remainingDamage * 4);

                    // If this reduces the player's health to zero, make sure they actually die.
                    if (player.statLife <= 0)
                    {
                        // Death message changes depending on whether it was the last few drips of an "honored" bleed, or attempting to cheat by removing the accessory.
                        string deathMessageKey = modPlayer.chaliceOfTheBloodGod ? "Status.Death.ChaliceOfTheBloodGodClose" : "Status.Death.ChaliceOfTheBloodGodUnequip";
                        player.KillMe(PlayerDeathReason.ByCustomReason(CalamityUtils.GetText(deathMessageKey).Format(player.name)), modPlayer.chaliceBleedoutBuffer, 0, false);
                    }
                }

                // Regardless of what caused the buffer to flush, reset both variables.
                modPlayer.chaliceBleedoutBuffer = 0D;
                modPlayer.chaliceDamagePointPartialProgress = 0D;
            }
        }

        private static void SpawnBloodParticles(Player player, int bleedDamage)
        {
            CalamityPlayer modPlayer = player.Calamity();

            // Spawn blood particles based on the amount of damage taken this frame.
            // Some particles are always spawned even when bleeding slowly (aka not actually taking damage this frame).
            float roughBloodCount = bleedDamage == 0 ? 0.4f : bleedDamage;
            int exactBloodCount = (int)roughBloodCount;

            // Fractional chance is taken as a chance to spawn one final blood particle.
            if (Main.rand.NextFloat() < roughBloodCount - exactBloodCount)
                ++exactBloodCount;

            // Particles count is capped. This is primarily there to prevent particle spam when unequipping the accessory.
            if (exactBloodCount > 18)
                exactBloodCount = 18;

            // Code copied from Violence.
            float bloodVelMult = 0.6f + MathHelper.Clamp((float)modPlayer.chaliceBleedoutBuffer * 0.01f, 0f, 3f);
            for (int i = 0; i < exactBloodCount; ++i)
            {
                int bloodLifetime = Main.rand.Next(22, 36);
                float bloodScale = Main.rand.NextFloat(0.6f, 0.8f);
                Color bloodColor = Color.Lerp(Color.Red, Color.DarkRed, Main.rand.NextFloat());
                bloodColor = Color.Lerp(bloodColor, new Color(51, 22, 94), Main.rand.NextFloat(0.65f));

                if (Main.rand.NextBool(20))
                    bloodScale *= 2f;

                float randomSpeedMultiplier = Main.rand.NextFloat(1.25f, 2.25f);
                Vector2 bloodVelocity = Main.rand.NextVector2Unit() * bloodVelMult * randomSpeedMultiplier;
                bloodVelocity.Y -= 5f;
                BloodParticle blood = new BloodParticle(player.Center, bloodVelocity, bloodLifetime, bloodScale, bloodColor);
                GeneralParticleHandler.SpawnParticle(blood);
            }
            for (int i = 0; i < exactBloodCount / 3; ++i)
            {
                float bloodScale = Main.rand.NextFloat(0.2f, 0.33f);
                Color bloodColor = Color.Lerp(Color.Red, Color.DarkRed, Main.rand.NextFloat(0.5f, 1f));
                Vector2 bloodVelocity = Main.rand.NextVector2Unit() * bloodVelMult * Main.rand.NextFloat(1f, 2f);
                bloodVelocity.Y -= 2.3f;
                BloodParticle2 blood = new BloodParticle2(player.Center, bloodVelocity, 20, bloodScale, bloodColor);
                GeneralParticleHandler.SpawnParticle(blood);
            }
        }
    }
}
