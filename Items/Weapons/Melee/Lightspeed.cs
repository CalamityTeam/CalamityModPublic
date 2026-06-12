using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Cooldowns;
using CalamityMod.Items.Materials;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    [LegacyName("ElementalShortsword", "ElementalShiv")]
    public class Lightspeed : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";

        public static int MaxEnergy = 100;
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<ElementalMix>()];
        }

        public override void SetDefaults()
        {
            Item.width = 74;
            Item.height = 94;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.damage = 200;
            Item.DamageType = TrueMeleeDamageClass.Instance;
            Item.useAnimation = Item.useTime = 20;
            Item.shootSpeed = 10f;
            Item.knockBack = 2f;
            Item.value = CalamityGlobalItem.RarityPurpleBuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.shoot = ModContent.ProjectileType<LightspeedHoldout>();

            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.autoReuse = true;
            Item.channel = true;
            base.SetDefaults();
        }

        // You can only use the right-click if you have sufficient Elemental Mastery
        public override bool AltFunctionUse(Player player) => player.GetModPlayer<LightspeedPlayer>().elementalMastery >= MaxEnergy;

        public override void HoldItem(Player player)
        {
            if (player.Calamity().cooldowns.TryGetValue(ElementalMastery.ID, out var cooldown))
            {
                cooldown.timeLeft = player.GetModPlayer<LightspeedPlayer>().elementalMastery;
            }
            else
            {
                player.AddCooldown(ElementalMastery.ID, 0);
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.PiercingStarlight).
                AddIngredient<Lucrecia>().
                AddIngredient(ItemID.LunarBar, 5).
                AddIngredient<LifeAlloy>(5).
                AddIngredient(ItemID.FragmentSolar, 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }

    public class LightspeedPlayer : ModPlayer
    {
        public int elementalMastery = 0;
        public int elementalMasteryPrevious = 0;
        public int elementalMasteryTimer = 0;
        public bool elementalMasteryPaused = false;
        public bool elementalMasteryMaxFXPlayed = false;
        private int lucreciaParticleTimer = 0;

        public override void PostUpdateMiscEffects()
        {
            if (Player.HeldItem.type == ModContent.ItemType<Lightspeed>() && elementalMastery > 0)
            {
                lucreciaParticleTimer--;

                // If the timer is at or below zero, spawn a particle.
                if (lucreciaParticleTimer <= 0)
                {
                    // Reset the timer (spawn rate depends on energy)
                    lucreciaParticleTimer = (int)(20 - 15 * (elementalMastery / 60));

                    float radius = Main.rand.NextFloat(160f, 190f); // Distance from center
                    float spawnAngle = Main.rand.NextFloat(MathHelper.TwoPi); // Random angle along the whole radius

                    Vector2 spawnPosition = Player.Center + spawnAngle.ToRotationVector2() * radius;

                    // Scale opacity with energy
                    float opacity = elementalMastery / (float)Lightspeed.MaxEnergy;
                    Color color = Main.rand.NextBool() ? Color.Aqua : Color.OrangeRed;
                    color *= opacity * 0.5f;

                    if (elementalMastery >= 100)
                    {
                        color *= 2.4f; // Way brighter
                    }

                    Vector2 dummyVelocity = Vector2.Zero; // We dont want the argument where this is used to have influence on the actual path
                    float rotationSpeed = 0.04f;

                    var particle = new RoundedStarParticle(spawnPosition, dummyVelocity, color with { A = 0 }, Main.rand.NextFloat(0.05f, 0.065f), Main.rand.Next(30, 60), rotationSpeed, 1f, true, Player.Center, Player.whoAmI);
                    GeneralParticleHandler.SpawnParticle(particle);
                }
            }


            if (Player.HeldItem.type == ModContent.ItemType<Lightspeed>())
            {
                if (elementalMastery == 0)
                    elementalMasteryPrevious = 0;

                if (elementalMastery > 0)
                {
                    if (elementalMastery > elementalMasteryPrevious)
                    {
                        elementalMasteryPaused = true;
                        elementalMasteryTimer = 0;
                    }

                    elementalMasteryTimer++;

                    // Pause for 180 ticks
                    if (elementalMasteryPaused)
                    {
                        if (elementalMasteryTimer >= 1 && elementalMastery == Lightspeed.MaxEnergy && !elementalMasteryMaxFXPlayed)
                        {
                            SoundStyle maxEnergyReached = new("CalamityMod/Sounds/Custom/AbilitySounds/DarklightEnergyCharged");
                            SoundEngine.PlaySound(maxEnergyReached with { Volume = 0.9f }, Player.Center);

                            for (int i = 0; i < 10; i++) // Circular ring of particles burst from player
                            {
                                float angle = MathHelper.TwoPi * (i / 10f);
                                Vector2 spawnDirection = angle.ToRotationVector2();
                                Vector2 velocity = spawnDirection * 14f;

                                CritSpark spark = new CritSpark(Player.Center + spawnDirection * 3f, velocity, Color.Lerp(Color.CornflowerBlue, Color.MediumPurple, Main.rand.NextFloat(1f)), Color.White * 0.33f, 1.2f, 12, 0.3f, 1.2f);
                                GeneralParticleHandler.SpawnParticle(spark);
                            }
                            elementalMasteryMaxFXPlayed = true;
                        }

                        // If the pause is Done, resume decrementing.
                        if (elementalMasteryTimer >= 180)
                        {
                            elementalMasteryTimer = 0;
                            elementalMastery--;
                        }
                    }
                    else // Decrementing
                    {
                        // Once every 30 ticks
                        if (elementalMasteryTimer >= 30)
                        {
                            elementalMastery--;
                            elementalMasteryTimer = 0;
                        }
                    }

                    if (elementalMastery != Lightspeed.MaxEnergy)
                        elementalMasteryMaxFXPlayed = false;
                }

                // Update for next loop.
                elementalMasteryPrevious = elementalMastery;

            }
            else
            {
                // Reset all EM variables when not holding the weapon
                elementalMasteryTimer = 0;
                elementalMasteryPaused = false;
                elementalMastery = 0;
            }
        }

        public override void UpdateDead()
        {
            elementalMastery = 0;
            elementalMasteryPrevious = 0;
        }
    }
}
