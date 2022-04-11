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
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 52;
            Projectile.height = 32;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.minionSlots = 1;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
        }

        // Defines an Item which is a hacked clone of a P90, edited to be summon class instead of ranged.
        // The false gun's damage is changed to the appropriate value every time a Tactical Plague Jet wants to fire a bullet.
        private static void DefineFalseGun()
        {
            int p90ID = ModContent.ItemType<Items.Weapons.Ranged.P90>();
            FalseGun = new Item();
            FalseGun.SetDefaults(p90ID, true);
            // FalseGun.ranged = false /* tModPorter - this is redundant, for more info see https://github.com/tModLoader/tModLoader/wiki/Update-Migration-Guide#damage-classes */ ;
            FalseGun.DamageType = DamageClass.Summon;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            // Frame 1 spawning effects.
            if (Projectile.localAI[0] == 0f)
            {
                // Spawn dust.
                for (int i = 0; i < 45; i++)
                {
                    float angle = MathHelper.TwoPi / 45f * i;
                    Vector2 velocity = angle.ToRotationVector2() * 4f;
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + velocity * 2f, (int)CalamityDusts.Plague, velocity);
                    dust.noGravity = true;
                }

                // Construct a fake item to use with vanilla code for the sake of firing bullets.
                if (FalseGun is null)
                    DefineFalseGun();
                Projectile.localAI[0] = 1f;
            }

            // Animation handling.
            if (Projectile.frameCounter++ > 6f)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
                Projectile.frame = 0;

            player.AddBuff(ModContent.BuffType<TacticalPlagueEngineBuff>(), 3600);
            bool isCorrectProjectile = Projectile.type == ModContent.ProjectileType<TacticalPlagueJet>();
            if (isCorrectProjectile)
            {
                if (player.dead)
                    modPlayer.plagueEngine = false;
                if (modPlayer.plagueEngine)
                    Projectile.timeLeft = 2;
            }

            // Pick a target and possibly fire at them.
            NPC potentialTarget = Projectile.Center.MinionHoming(1560f, player);

            // If the jet has no target or is out of ammo, passively fly around the player.
            if (potentialTarget is null || !player.HasAmmo(FalseGun, false))
            {
                float distanceToOwner = (player.Center - Projectile.Center).Length();
                float acceleration = 0.1f;

                // Reduce acceleration significantly if close to the player.
                if (distanceToOwner < 140f)
                    acceleration = 0.035f;
                else if (distanceToOwner < 200f)
                    acceleration = 0.07f;

                // Push away from the player constantly, but only if more than 100 pixels away. (What???)
                if (distanceToOwner > 100f)
                {
                    if (Math.Abs(player.Center.X - Projectile.Center.X) > 20f)
                        Projectile.velocity.X += acceleration * Math.Sign(player.Center.X - Projectile.Center.X);
                    if (Math.Abs(player.Center.Y - Projectile.Center.Y) > 10f)
                        Projectile.velocity.Y += acceleration * Math.Sign(player.Center.Y - Projectile.Center.Y);
                }

                // If the jet is moving too fast, start giving it air friction.
                else if (Projectile.velocity.Length() > 4f)
                    Projectile.velocity *= 0.95f;

                // The jet vertically sticks to the player.
                if (Math.Abs(Projectile.velocity.Y) < 2f)
                    Projectile.velocity.Y += 0.1f * Math.Sign(player.Center.Y - Projectile.Center.Y);

                // If the jet is moving WAY too fast, clamp its velocity.
                if (Projectile.velocity.Length() > 9f)
                    Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 9f;

                // Change the sprite direction of the jet if it's moving more than a tiny bit in either direction.
                if (Projectile.velocity.X > 0.25f)
                    Projectile.spriteDirection = 1;
                else if (Projectile.velocity.X < -0.25f)
                    Projectile.spriteDirection = -1;

                // Angle the jet as it moves.
                Projectile.rotation = Projectile.rotation.AngleTowards(0f, 0.2f);

                // If the jet is absurdly far away, teleport directly onto the player.
                if (distanceToOwner > 2700f)
                {
                    Projectile.Center = player.Center;
                    Projectile.netUpdate = true;
                }
            }

            // The jet has ammo and has a target. Open fire.
            else
            {
                Projectile.spriteDirection = 1;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(potentialTarget.Center - Vector2.UnitY * 195f) * 17f, 0.035f);
                Projectile.rotation = Projectile.rotation.AngleTowards(Projectile.AngleTo(potentialTarget.Center), 0.1f);

                if (Projectile.ai[0]++ % 75f == 24f)
                {
                    int projID = ProjectileID.Bullet;
                    float shootSpeed = FalseGun.shootSpeed;
                    bool canShoot = true;
                    int damage = FalseGun.damage = Projectile.damage;
                    float kb = Projectile.knockBack;
                    bool shootRocket = ++Projectile.ai[1] % 20f == 0f;
                    // Rockets never consume ammo.
                    bool dontConsumeAmmo = Main.rand.NextBool() || shootRocket;
                    int projIndex;

                    // Vanilla function tricked into using a fake gun item with the appropriate base damage as the "firing item".
                    player.PickAmmo(FalseGun, ref projID, ref shootSpeed, ref canShoot, ref damage, ref kb, out _, dontConsumeAmmo);

                    // One in every 20 shots is a rocket which deals 1.5x total damage and extreme knockback.
                    if (shootRocket)
                    {
                        int rocketDamage = (int)(damage * 1.5f);
                        float rocketKB = kb + 5f;
                        projIndex = Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, Projectile.SafeDirectionTo(potentialTarget.Center) * 18f, ModContent.ProjectileType<MK2RocketHoming>(), rocketDamage, rocketKB, Projectile.owner);
                    }

                    // Fire the selected bullet, nothing special.
                    else
                        projIndex = Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, Projectile.SafeDirectionTo(potentialTarget.Center) * shootSpeed, projID, damage, kb, Projectile.owner);

                    // Regardless of what was fired, force it to be a summon projectile so that summon accessories work.
                    if (projIndex.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[projIndex].Calamity().forceMinion = true;
                        Main.projectile[projIndex].originalDamage = Projectile.originalDamage;
                    }
                }

                // Prevent minion clumping while firing.
                Projectile.MinionAntiClump(0.25f);
            }
        }
    }
}
