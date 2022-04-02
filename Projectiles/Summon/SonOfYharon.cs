using CalamityMod.Buffs.Summon;
using CalamityMod.Items.Weapons.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class SonOfYharon : ModProjectile
    {
        public Player Owner => Main.player[projectile.owner];
        public ref float AttackTimer => ref projectile.ai[0];
        public ref float RamCountdown => ref projectile.ai[1];
        public ref float RamReboundCountdown => ref projectile.localAI[1];
        public const int FireballShootRate = 20;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Draconid");
            Main.projFrames[projectile.type] = 10;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 100;
            projectile.height = 100;
            projectile.extraUpdates = 1;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = projectile.MaxUpdates * 9;
            projectile.minionSlots = 5f;
            projectile.timeLeft = 90000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.minion = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(RamReboundCountdown);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            RamReboundCountdown = reader.ReadSingle();
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0f)
                PerformInitialization();
            projectile.localAI[0]++;

            // Perform minion checks.
            PerformMinionChecks();

            // Dynamically adjust damage if it changes.
            if (Owner.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)(projectile.Calamity().spawnedPlayerMinionProjectileDamageValue / projectile.Calamity().spawnedPlayerMinionDamageValue * Owner.MinionDamage());
                projectile.damage = trueDamage;
            }

            // Handle frame logic.
            if (projectile.FinalExtraUpdate())
                projectile.frameCounter++;
            if (projectile.frameCounter % 8 == 0)
            {
                projectile.frame++;
            }
            if (RamCountdown > 0f || RamReboundCountdown > 0f)
            {
                if (projectile.frame < 6)
                {
                    projectile.frame = 6;
                }
                if (projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame = 6;
                }
            }
            else
            {
                if (projectile.frame >= 6)
                {
                    projectile.frame = 0;
                }
            }

            NPC potentialTarget = projectile.Center.MinionHoming(2000f, Owner);

            // Teleport to the player if extremely far away.
            if (!projectile.WithinRange(Owner.Center, 4000f))
            {
                projectile.Center = Owner.Center + Main.rand.NextVector2Circular(16f, 16f);
                projectile.netUpdate = true;
                return;
            }

            // Rebound for a bit after a ram.
            if (RamReboundCountdown > 0f)
            {
                projectile.velocity *= 0.97f;
                RamReboundCountdown--;
                return;
            }

            if (potentialTarget is null)
            {
                DoPlayerHoverMovement();
                AttackTimer = 0f;
            }
            else
            {
                AttackTarget(potentialTarget);
                AttackTimer++;
            }
        }

        public void PerformInitialization()
        {
            projectile.Calamity().spawnedPlayerMinionDamageValue = Owner.MinionDamage();
            projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;

            for (int i = 0; i < 45; i++)
            {
                Dust fire = Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Circular(55f, 55f), 244);
                fire.velocity *= 2f;
                fire.scale *= 1.15f;
            }
        }

        public void PerformMinionChecks()
        {
            bool correctMinion = projectile.type == ModContent.ProjectileType<SonOfYharon>();
            Owner.AddBuff(ModContent.BuffType<YharonKindleBuff>(), 3600);
            if (!correctMinion)
                return;

            if (Owner.dead)
                Owner.Calamity().aChicken = false;

            if (Owner.Calamity().aChicken)
                projectile.timeLeft = 2;
        }

        public void DoPlayerHoverMovement()
        {
            // Move away from other minions of the same type.
            projectile.MinionAntiClump(0.1f);

            Vector2 hoverDestination = Owner.Top - Vector2.UnitY * 50f;
            if (!projectile.WithinRange(hoverDestination, 60f))
                projectile.velocity = (projectile.velocity * 19f + projectile.SafeDirectionTo(hoverDestination) * 11f) / 20f;

            // Adjust the sprite direction to point towards the hover destination. This does not happen if already really close
            // horizontally to the destination, to prevent direction changing spam.
            if (MathHelper.Distance(projectile.Center.X, hoverDestination.X) > 45f)
                projectile.spriteDirection = (projectile.Center.X - hoverDestination.X > 0).ToDirectionInt();
        }

        public void AttackTarget(NPC target)
        {
            float distanceFromTarget = projectile.Distance(target.Center);

            // Approach the target quickly if far enough away.
            // Movement becomes sharper the farther away from the target the minion is.
            // Also release fireballs.
            if (distanceFromTarget > 220f)
            {
                float interpolantToIdealVelocity = MathHelper.Lerp(0.05f, 0.3f, Utils.InverseLerp(300f, 560f, distanceFromTarget, true));
                Vector2 idealVelocity = projectile.SafeDirectionTo(target.Center) * MathHelper.Min(distanceFromTarget, 14f);
                projectile.velocity = Vector2.Lerp(projectile.velocity, idealVelocity, interpolantToIdealVelocity);

                // Adjust the sprite direction to point towards the hover destination.
                projectile.spriteDirection = (projectile.Center.X - target.Center.X > 0).ToDirectionInt();

                // Periodically release fireballs at the target.
                // They move faster the farther away from the target the minion is.
                if (AttackTimer % FireballShootRate == FireballShootRate - 1f)
                {
                    float shootSpeed = (distanceFromTarget - 220f) * 0.015f + 45f;
                    Main.PlaySound(SoundID.Item73, projectile.Center);
                    if (Main.myPlayer == projectile.owner)
                    {
                        Vector2 shootPosition = projectile.Center + Vector2.UnitX * projectile.spriteDirection * 10f;
                        Vector2 shootVelocity = (target.Center - shootPosition).SafeNormalize(Vector2.UnitX * projectile.spriteDirection) * shootSpeed;
                        Projectile.NewProjectile(shootPosition, shootVelocity, ModContent.ProjectileType<YharonMinionFireball>(), projectile.damage, projectile.knockBack * 0.5f, projectile.owner);
                    }
                }
            }

            // Otherwise ram at the target.
            else if (RamCountdown <= 0f)
            {
                RamCountdown = 60f;

                projectile.velocity = projectile.SafeDirectionTo(target.Center, -Vector2.UnitY) * 23f;
                projectile.netUpdate = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // Create an explosion and rebound on hitting an NPC if a ram is happening.
            if (RamCountdown > 0f)
            {
                RamCountdown = 0f;
                RamReboundCountdown = 30f;
                projectile.velocity *= -0.6f;

                // Create an explosion of dust.
                for (int i = 0; i < 150; i++)
                {
                    Dust fire = Dust.NewDustPerfect(projectile.Center, 244);
                    fire.velocity = Main.rand.NextVector2Circular(20f, 20f);
                    fire.scale *= 3f;
                    fire.noGravity = true;

                    fire = Dust.NewDustPerfect(projectile.Center, 244);
                    fire.velocity *= Main.rand.NextVector2Circular(8f, 8f);
                    fire.scale *= 2f;
                }

                projectile.netUpdate = true;
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            // Do much more damage during a ram than otherwise.
            if (RamCountdown > 0f)
                damage = (int)(damage * AngryChickenStaff.ReboundRamDamageFactor);
        }

        public override bool CanDamage() => projectile.localAI[0] > 1f;

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y6 = num214 * projectile.frame;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Rectangle(0, y6, texture2D13.Width, num214), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2(texture2D13.Width / 2f, num214 / 2f), projectile.scale, spriteEffects, 0f);
            return false;
        }
    }
}
