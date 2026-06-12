using System.IO;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.Summon;
using CalamityMod.Items.Weapons.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class FieryDraconid : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public Player Owner => Main.player[Projectile.owner];
        public ref float AttackTimer => ref Projectile.ai[0];
        public ref float RamCountdown => ref Projectile.ai[1];
        public ref float RamReboundCountdown => ref Projectile.localAI[1];
        public const int FireballShootRate = 20;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 10;
            Main.projPet[Type] = true;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.extraUpdates = 1;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = Projectile.MaxUpdates * 9;
            if (!Main.gameMenu && Main.player[Projectile.owner].Calamity().fadedIdolatry)
                Projectile.minionSlots = 4f;
            else
                Projectile.minionSlots = 5f;
            Projectile.timeLeft = 90000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
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
            if (Projectile.localAI[0] == 0f)
                PerformInitialization();
            Projectile.localAI[0]++;

            // Perform minion checks.
            PerformMinionChecks();

            //Update slots to match Faded Idolatry equip state
            if (Main.player[Projectile.owner].Calamity().fadedIdolatry)
                Projectile.minionSlots = 4f;
            else
                Projectile.minionSlots = 5f;

            // Handle frame logic.
            if (Projectile.FinalExtraUpdate())
                Projectile.frameCounter++;
            if (Projectile.frameCounter % 8 == 0)
            {
                Projectile.frame++;
            }
            if (RamCountdown > 0f || RamReboundCountdown > 0f)
            {
                if (Projectile.frame < 6)
                {
                    Projectile.frame = 6;
                }
                if (Projectile.frame >= Main.projFrames[Type])
                {
                    Projectile.frame = 6;
                }
            }
            else
            {
                if (Projectile.frame >= 6)
                {
                    Projectile.frame = 0;
                }
            }

            NPC potentialTarget = Projectile.Center.MinionHoming(2000f, Owner);

            // Teleport to the player if extremely far away.
            if (!Projectile.WithinRange(Owner.Center, 4000f))
            {
                Projectile.Center = Owner.Center + Main.rand.NextVector2Circular(16f, 16f);
                Projectile.netUpdate = true;
                return;
            }

            // Rebound for a bit after a ram.
            if (RamReboundCountdown > 0f)
            {
                Projectile.velocity *= 0.97f;
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
            for (int i = 0; i < 45; i++)
            {
                Dust fire = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(55f, 55f), DustID.CopperCoin);
                fire.velocity *= 2f;
                fire.scale *= 1.15f;
            }
        }

        public void PerformMinionChecks()
        {
            bool correctMinion = Projectile.type == ModContent.ProjectileType<FieryDraconid>();
            Owner.AddBuff(ModContent.BuffType<FieryDraconidBuff>(), 3600);
            if (!correctMinion)
                return;

            if (Owner.dead)
                Owner.Calamity().aChicken = false;

            if (Owner.Calamity().aChicken)
                Projectile.timeLeft = 2;
        }

        public void DoPlayerHoverMovement()
        {
            // Move away from other minions of the same type.
            Projectile.MinionAntiClump(0.1f);

            Vector2 hoverDestination = Owner.Top - Vector2.UnitY * 50f;
            if (!Projectile.WithinRange(hoverDestination, 60f))
                Projectile.velocity = (Projectile.velocity * 19f + Projectile.SafeDirectionTo(hoverDestination) * 11f) / 20f;

            // Adjust the sprite direction to point towards the hover destination. This does not happen if already really close
            // horizontally to the destination, to prevent direction changing spam.
            if (MathHelper.Distance(Projectile.Center.X, hoverDestination.X) > 45f)
                Projectile.spriteDirection = (Projectile.Center.X - hoverDestination.X > 0).ToDirectionInt();
        }

        public void AttackTarget(NPC target)
        {
            float distanceFromTarget = Projectile.Distance(target.Center);

            // Approach the target quickly if far enough away.
            // Movement becomes sharper the farther away from the target the minion is.
            // Also release fireballs.
            if (distanceFromTarget > 220f)
            {
                float interpolantToIdealVelocity = MathHelper.Lerp(0.05f, 0.3f, Utils.GetLerpValue(300f, 560f, distanceFromTarget, true));
                Vector2 idealVelocity = Projectile.SafeDirectionTo(target.Center) * MathHelper.Min(distanceFromTarget, 14f);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, idealVelocity, interpolantToIdealVelocity);

                // Adjust the sprite direction to point towards the hover destination.
                Projectile.spriteDirection = (Projectile.Center.X - target.Center.X > 0).ToDirectionInt();

                // Periodically release fireballs at the target.
                // They move faster the farther away from the target the minion is.
                if (AttackTimer % FireballShootRate == FireballShootRate - 1f)
                {
                    float shootSpeed = (distanceFromTarget - 220f) * 0.015f + 45f;
                    SoundEngine.PlaySound(SoundID.Item73, Projectile.Center);
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Vector2 shootPosition = Projectile.Center + Vector2.UnitX * Projectile.spriteDirection * 10f;
                        Vector2 shootVelocity = (target.Center - shootPosition).SafeNormalize(Vector2.UnitX * Projectile.spriteDirection) * shootSpeed;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), shootPosition, shootVelocity, ModContent.ProjectileType<YharonMinionFireball>(), Projectile.damage, Projectile.knockBack * 0.5f, Projectile.owner);
                    }
                }
            }

            // Otherwise ram at the target.
            else if (RamCountdown <= 0f)
            {
                RamCountdown = 60f;

                Projectile.velocity = Projectile.SafeDirectionTo(target.Center, -Vector2.UnitY) * 23f;
                Projectile.netUpdate = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Dragonfire>(), 180);

            // Create an explosion and rebound on hitting an NPC if a ram is happening.
            if (RamCountdown > 0f)
            {
                RamCountdown = 0f;
                RamReboundCountdown = 30f;
                Projectile.velocity *= -0.6f;

                // Create an explosion of dust.
                for (int i = 0; i < 150; i++)
                {
                    Dust fire = Dust.NewDustPerfect(Projectile.Center, DustID.CopperCoin);
                    fire.velocity = Main.rand.NextVector2Circular(20f, 20f);
                    fire.scale *= 3f;
                    fire.noGravity = true;

                    fire = Dust.NewDustPerfect(Projectile.Center, DustID.CopperCoin);
                    fire.velocity *= Main.rand.NextVector2Circular(8f, 8f);
                    fire.scale *= 2f;
                }

                Projectile.netUpdate = true;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // Do much more damage during a ram than otherwise.
            if (RamCountdown > 0f)
                modifiers.FinalDamage *= YharonsKindleStaff.ReboundRamDamageFactor;
        }

        public override bool MinionContactDamage() => Projectile.localAI[0] > 1f;

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            int framing = Terraria.GameContent.TextureAssets.Projectile[Type].Value.Height / Main.projFrames[Type];
            int y6 = framing * Projectile.frame;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle(0, y6, texture2D13.Width, framing), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture2D13.Width / 2f, framing / 2f), Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}
