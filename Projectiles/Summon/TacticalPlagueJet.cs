using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class TacticalPlagueJet : ModProjectile
    {
        public static Item FalseGun = null;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tactical Plague Jet");
            Main.projFrames[projectile.type] = 3;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 52;
            projectile.height = 32;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.minionSlots = 1;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            projectile.tileCollide = false;
            projectile.extraUpdates = 1;
        }

        // Defines an Item which is a hacked clone of a P90, edited to be summon class instead of ranged.
        // The false gun's damage is changed to the appropriate value every time a Tactical Plague Jet wants to fire a bullet.
        private static void DefineFalseGun()
        {
            int p90ID = ModContent.ItemType<Items.Weapons.Ranged.P90>();
            FalseGun = new Item();
            FalseGun.SetDefaults(p90ID, true);
            FalseGun.ranged = false;
            FalseGun.summon = true;
        }

        public override bool CanDamage() => false;

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            // Frame 1 spawning effects.
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;

                // Spawn dust.
                for (int i = 0; i < 45; i++)
                {
                    float angle = MathHelper.TwoPi / 45f * i;
                    Vector2 velocity = angle.ToRotationVector2() * 4f;
                    Dust dust = Dust.NewDustPerfect(projectile.Center + velocity * 2f, (int)CalamityDusts.Plague, velocity);
                    dust.noGravity = true;
                }

                // Construct a fake item to use with vanilla code for the sake of firing bullets.
                if (FalseGun is null)
                    DefineFalseGun();
                projectile.localAI[0] = 1f;
            }

            // Animation handling.
            if (projectile.frameCounter++ > 6f)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
                projectile.frame = 0;

            // Correct for minion damage changes every single frame.
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = trueDamage;
            }

            player.AddBuff(ModContent.BuffType<TacticalPlagueEngineBuff>(), 3600);
            bool isCorrectProjectile = projectile.type == ModContent.ProjectileType<TacticalPlagueJet>();
            if (isCorrectProjectile)
            {
                if (player.dead)
                    modPlayer.plagueEngine = false;
                if (modPlayer.plagueEngine)
                    projectile.timeLeft = 2;
            }

            // Pick a target and possibly fire at them.
            NPC potentialTarget = projectile.Center.MinionHoming(1560f, player);

            // If the jet has no target or is out of ammo, passively fly around the player.
            if (potentialTarget is null || !player.HasAmmo(FalseGun, false))
            {
                float distanceToOwner = (player.Center - projectile.Center).Length();
                float acceleration = 0.1f;

                // Reduce acceleration significantly if close to the player.
                if (distanceToOwner < 140f)
                    acceleration = 0.035f;
                else if (distanceToOwner < 200f)
                    acceleration = 0.07f;

                // Push away from the player constantly, but only if more than 100 pixels away. (What???)
                if (distanceToOwner > 100f)
                {
                    if (Math.Abs(player.Center.X - projectile.Center.X) > 20f)
                        projectile.velocity.X += acceleration * Math.Sign(player.Center.X - projectile.Center.X);
                    if (Math.Abs(player.Center.Y - projectile.Center.Y) > 10f)
                        projectile.velocity.Y += acceleration * Math.Sign(player.Center.Y - projectile.Center.Y);
                }

                // If the jet is moving too fast, start giving it air friction.
                else if (projectile.velocity.Length() > 4f)
                    projectile.velocity *= 0.95f;

                // The jet vertically sticks to the player.
                if (Math.Abs(projectile.velocity.Y) < 2f)
                    projectile.velocity.Y += 0.1f * Math.Sign(player.Center.Y - projectile.Center.Y);

                // If the jet is moving WAY too fast, clamp its velocity.
                if (projectile.velocity.Length() > 9f)
                    projectile.velocity = Vector2.Normalize(projectile.velocity) * 9f;

                // Change the sprite direction of the jet if it's moving more than a tiny bit in either direction.
                if (projectile.velocity.X > 0.25f)
                    projectile.spriteDirection = 1;
                else if (projectile.velocity.X < -0.25f)
                    projectile.spriteDirection = -1;

                // Angle the jet as it moves.
                projectile.rotation = projectile.rotation.AngleTowards(0f, 0.2f);

                // If the jet is absurdly far away, teleport directly onto the player.
                if (distanceToOwner > 2700f)
                {
                    projectile.Center = player.Center;
                    projectile.netUpdate = true;
                }
            }

            // The jet has ammo and has a target. Open fire.
            else
            {
                projectile.spriteDirection = 1;
                projectile.velocity = Vector2.Lerp(projectile.velocity, projectile.SafeDirectionTo(potentialTarget.Center - Vector2.UnitY * 195f) * 17f, 0.035f);
                projectile.rotation = projectile.rotation.AngleTowards(projectile.AngleTo(potentialTarget.Center), 0.1f);

                if (projectile.ai[0]++ % 75f == 24f)
                {
                    int projID = ProjectileID.Bullet;
                    float shootSpeed = FalseGun.shootSpeed;
                    bool canShoot = true;
                    int damage = FalseGun.damage = projectile.damage;
                    float kb = projectile.knockBack;
                    bool shootRocket = ++projectile.ai[1] % 20f == 0f;
                    // Rockets never consume ammo.
                    bool dontConsumeAmmo = Main.rand.NextBool() || shootRocket;
                    int projIndex;

                    // Vanilla function tricked into using a fake gun item with the appropriate base damage as the "firing item".
                    player.PickAmmo(FalseGun, ref projID, ref shootSpeed, ref canShoot, ref damage, ref kb, dontConsumeAmmo);

                    // One in every 20 shots is a rocket which deals 1.5x total damage and extreme knockback.
                    if (shootRocket)
                    {
                        int rocketDamage = (int)(damage * 1.5f);
                        float rocketKB = kb + 5f;
                        projIndex = Projectile.NewProjectile(projectile.Center, projectile.SafeDirectionTo(potentialTarget.Center) * 18f, ModContent.ProjectileType<MK2RocketHoming>(), rocketDamage, rocketKB, projectile.owner);
                    }

                    // Fire the selected bullet, nothing special.
                    else
                        projIndex = Projectile.NewProjectile(projectile.Center, projectile.SafeDirectionTo(potentialTarget.Center) * shootSpeed, projID, damage, kb, projectile.owner);

                    // Regardless of what was fired, force it to be a summon projectile so that summon accessories work.
                    if (projIndex.WithinBounds(Main.maxProjectiles))
                        Main.projectile[projIndex].Calamity().forceMinion = true;
                }

                // Prevent minion clumping while firing.
                projectile.MinionAntiClump(0.25f);
            }
        }
    }
}
