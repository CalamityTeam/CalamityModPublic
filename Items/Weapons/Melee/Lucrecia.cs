using CalamityMod.Cooldowns;
using CalamityMod.Items.Materials;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Lucrecia : BaseSwordHoldoutItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";

        public static int MaxEnergy = 100;

        public override int ProjectileType => ModContent.ProjectileType<LucreciaHoldout>();

        public override void SetDefaults()
        {
            Item.width = 54;
            Item.height = 54;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.damage = 132;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 34;
            Item.shootSpeed = 10f;
            Item.knockBack = 8.25f;
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.rare = ItemRarityID.Yellow;

            Item.autoReuse = true;
            Item.channel = true;
            // Item.noMelee = true;
            // Item.noUseGraphic = true;
            base.SetDefaults();

        }

        public override bool AltFunctionUse(Player player) => player.GetModPlayer<LucreciaPlayer>().darklightEnergy >= MaxEnergy;

        public override void HoldItem(Player player)
        {
            if (player.Calamity().cooldowns.TryGetValue(DarklightEnergy.ID, out var cooldown))
            {
                cooldown.timeLeft = player.GetModPlayer<LucreciaPlayer>().darklightEnergy;
            }
            else
            {
                player.AddCooldown(DarklightEnergy.ID, 0);
            }
        }

        // public override bool MeleePrefix() => true;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<LifeAlloy>(5).
                AddIngredient(ItemID.FallenStar, 10).
                AddIngredient(ItemID.SoulofLight, 5).
                AddIngredient(ItemID.SoulofNight, 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }

    public class LucreciaPlayer : ModPlayer
    {
        private int lucreciaParticleTimer = 0;
        public int darklightEnergy = 0;
        public int darklightEnergyPrevious = 0;
        public int darklightEnergyTimer = 0;
        public bool darklightEnergyPaused = false;
        public bool darklightEnergyMaxFXPlayed = false;


        public override void PostUpdateMiscEffects()
        {
            if (Player.HeldItem.type == ModContent.ItemType<Lucrecia>())
            {
                if (darklightEnergy == 0)
                    darklightEnergyPrevious = 0;

                if (darklightEnergy > 0)
                {
                    // If the player's energy has increased since last tick
                    if (darklightEnergy > darklightEnergyPrevious)
                    {
                        darklightEnergyPaused = true;
                        darklightEnergyTimer = 0;
                    }

                    darklightEnergyTimer++;

                    // Pause for 180 ticks
                    if (darklightEnergyPaused)
                    {
                        if (darklightEnergyTimer >= 1 && darklightEnergy == Lucrecia.MaxEnergy && !darklightEnergyMaxFXPlayed)
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
                            darklightEnergyMaxFXPlayed = true;
                        }

                        // If the pause is Done, resume decrementing.
                        if (darklightEnergyTimer >= 180)
                        {
                            darklightEnergyPaused = false;
                            darklightEnergyTimer = 0;
                            darklightEnergy--;
                        }
                    }
                    else // Decrementing
                    {
                        darklightEnergyMaxFXPlayed = false;
                        // Once every 20 ticks
                        if (darklightEnergyTimer >= 20)
                        {
                            darklightEnergy--;
                            darklightEnergyTimer = 0;
                        }
                    }
                }

                if (darklightEnergy != Lucrecia.MaxEnergy)
                    darklightEnergyMaxFXPlayed = false;

                // Update for next loop.
                darklightEnergyPrevious = darklightEnergy;

            }
            else
            {
                // Reset all energy variables when not holding the weapon
                darklightEnergyTimer = 0;
                darklightEnergyPaused = false;
                darklightEnergy = 0;
            }


            if (Player.HeldItem.type == ModContent.ItemType<Lucrecia>() && darklightEnergy > 0)
            {
                lucreciaParticleTimer--;

                // If the timer is at or below zero, spawn a particle.
                if (lucreciaParticleTimer <= 0)
                {
                    // Reset the timer (spawn rate depends on energy)
                    lucreciaParticleTimer = (int)(20 - 15 * (darklightEnergy / 60));

                    float radius = Main.rand.NextFloat(160f, 190f); // Distance from center
                    float spawnAngle = Main.rand.NextFloat(MathHelper.TwoPi); // Random angle along the whole radius

                    Vector2 spawnPosition = Player.Center + spawnAngle.ToRotationVector2() * radius;

                    // Scale opacity with energy
                    float opacity = darklightEnergy / (float)Lucrecia.MaxEnergy;
                    Color color = Main.rand.NextBool() ? Color.MediumPurple : Color.CornflowerBlue;
                    color *= opacity * 0.5f;

                    if (darklightEnergy >= 100)
                    {
                        color *= 2.4f; // Way brighter
                    }

                    Vector2 dummyVelocity = Vector2.Zero; // We dont want the argument where this is used to have influence on the actual path
                    float rotationSpeed = 0.04f;

                    var particle = new RoundedStarParticle(spawnPosition, dummyVelocity, color with { A = 0 }, Main.rand.NextFloat(0.05f, 0.065f), Main.rand.Next(30, 60), rotationSpeed, 1f, true, Player.Center, Player.whoAmI);
                    GeneralParticleHandler.SpawnParticle(particle);
                }
            }
        }

        public override void UpdateDead()
        {
            darklightEnergy = 0;
            darklightEnergyPrevious = 0;
        }
    }
}
