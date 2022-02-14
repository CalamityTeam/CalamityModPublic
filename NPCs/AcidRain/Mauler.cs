using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.World.Generation;

namespace CalamityMod.NPCs.AcidRain
{
    public class Mauler : ModNPC
    {
        public enum MaulerAttackState
        {
            BubbleBursts,
            HorizontalAcidCharge,
            BigChargeupDash
        }

        public bool InWater => Collision.DrownCollision(npc.Center, 1, 1);
        public Player Target => Main.player[npc.target];
        public MaulerAttackState CurrentAttack
        {
            get => (MaulerAttackState)(int)npc.ai[0];
            set => npc.ai[0] = (int)value;
        }
        public ref float AttackTimer => ref npc.ai[1];
        public int CurrentFrame
        {
            get => (int)npc.localAI[0];
            set => npc.localAI[0] = value;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mauler");
            Main.npcFrameCount[npc.type] = 8;
        }

        public override void SetDefaults()
        {
            npc.noGravity = true;
            npc.damage = 135;
            npc.width = 180;
            npc.height = 90;
            npc.defense = 50;
            npc.DR_NERD(0.05f);
            npc.lifeMax = 88500;
            npc.aiStyle = -1;
            aiType = -1;
            npc.value = Item.buyPrice(0, 25, 0, 0);
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath60;
            npc.knockBackResist = 0f;
            npc.rarity = 2;
            banner = npc.type;
            bannerItem = ModContent.ItemType<MaulerBanner>();
            npc.RemoveWaterSlowness();
            npc.Calamity().canBreakPlayerDefense = true;
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToSickness = false;
            npc.Calamity().VulnerableToElectricity = true;
            npc.Calamity().VulnerableToWater = false;
        }

        public override void AI()
        {
            npc.TargetClosest(false);

            // Reset things every frame. They may be changed in the attack states below.
            npc.noTileCollide = false;
            npc.noGravity = InWater;

            switch (CurrentAttack)
            {
                case MaulerAttackState.BubbleBursts:
                    DoBehavior_BubbleBursts();
                    break;
                case MaulerAttackState.HorizontalAcidCharge:
                    DoBehavior_HorizontalAcidCharge();
                    break;
                case MaulerAttackState.BigChargeupDash:
                    DoBehavior_BigChargeupDash();
                    break;
            }

            // Emit radioactive light.
            Vector2 left = npc.Center - Vector2.UnitX * 15f;
            Vector2 right = npc.Center + Vector2.UnitX * 15f;
            DelegateMethods.v3_1 = Color.Lime.ToVector3();
            Utils.PlotTileLine(left, right, 10f, DelegateMethods.CastLightOpen);

            // Drift away from the world border.
            if (npc.Center.X < (Main.offLimitBorderTiles + 6f) * 16f || npc.Center.X >= (Main.maxTilesX - Main.offLimitBorderTiles - 6f) * 16f)
            {
                npc.velocity *= 0.9f;
                npc.Center = npc.Center.MoveTowards(Target.Center, 12f);
            }

            AttackTimer++;
        }

        public void DoBehavior_BubbleBursts()
        {
            int roarDelay = 32;
            int shootDelay = 24;
            int bubblesPerBurst = 7;
            int burstShootRate = 45;
            int burstCount = 3;
            float wrappedAttackTimer = (AttackTimer - roarDelay - shootDelay) % burstShootRate;

            // Slow down. If necessary, fall.
            npc.velocity *= 0.94f;
            if (!InWater)
            {
                npc.noTileCollide = true;
                npc.position.Y += 6f;
            }

            // Roar prior to releasing bubbles.
            if (AttackTimer <= roarDelay)
                CurrentFrame = (int)Math.Round(Utils.InverseLerp(0f, roarDelay, AttackTimer, true) * 5f);

            // Animate as usual a bit after roaring.
            else if (AttackTimer >= roarDelay + shootDelay)
            {
                npc.frameCounter++;
                if (npc.frameCounter >= 5)
                {
                    CurrentFrame = (CurrentFrame + 1) % Main.npcFrameCount[npc.type];
                    npc.frameCounter = 0;
                }

                // Open the mouth prior to creating bubbles.
                if (wrappedAttackTimer >= burstShootRate * 0.7f)
                    CurrentFrame = 4;
            }
            if (AttackTimer == roarDelay)
                Main.PlaySound(SoundID.Zombie, Target.Center, 97);

            // Rotate towards the target.
            int previousSpriteDirection = npc.spriteDirection;
            float idealRotation = npc.AngleTo(Target.Center);
            if (previousSpriteDirection == 1)
                idealRotation += MathHelper.Pi;
            npc.spriteDirection = (Target.Center.X < npc.Center.X).ToDirectionInt();
            if (previousSpriteDirection != npc.spriteDirection)
                npc.rotation += MathHelper.Pi;

            npc.rotation = npc.rotation.AngleTowards(idealRotation, 0.125f).AngleLerp(idealRotation, 0.075f);

            // Fire the bubble bursts.
            bool shouldShootBubbles = AttackTimer >= roarDelay + shootDelay && wrappedAttackTimer == burstShootRate - 1f;
            if (shouldShootBubbles)
            {
                Vector2 mouthPosition = npc.Center + new Vector2(npc.spriteDirection * -56f, 22f).RotatedBy(npc.rotation);
                Main.PlaySound(SoundID.Item96, mouthPosition);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int bubbleShootType = ModContent.ProjectileType<MaulerAcidBubble>();
                    int bubbleDamage = npc.GetProjectileDamage(bubbleShootType);
                    Vector2 baseBubbleShootVelocity = npc.SafeDirectionTo(mouthPosition) * 13.5f;
                    for (int i = 0; i < bubblesPerBurst; i++)
                    {
                        Vector2 bubbleShootVelocity = baseBubbleShootVelocity + Main.rand.NextVector2Circular(4f, 4f);
                        Projectile.NewProjectile(mouthPosition, bubbleShootVelocity, bubbleShootType, bubbleDamage, 0f);
                    }

                    // Get launched back after firing. This only happens if in water and there's no obstacles behind.
                    if (InWater && Collision.CanHit(npc.Center, 1, 1, npc.Center - baseBubbleShootVelocity.SafeNormalize(Vector2.Zero) * 100f, 1, 1))
                        npc.velocity -= baseBubbleShootVelocity * 0.62f;
                    npc.netUpdate = true;
                }
            }

            if (AttackTimer >= roarDelay + shootDelay + burstShootRate * burstCount)
                SelectNextAttack();
        }

        public void DoBehavior_HorizontalAcidCharge()
        {
            int stuckGiveupTime = 360;
            int chargeDelay = 45;
            int chargePreparationTime = 15;
            int chargeTime = 72;
            int acidShootRate = 4;
            float chargeSpeed = 24.5f;
            ref float stuckTimer = ref npc.ai[2];
            ref float chargeDirection = ref npc.ai[3];

            // Select frames.
            npc.frameCounter++;
            if (npc.frameCounter >= 5)
            {
                CurrentFrame = (CurrentFrame + 1) % Main.npcFrameCount[npc.type];
                npc.frameCounter = 0;
            }

            // Decide the rotation and direction.
            npc.rotation = npc.velocity.X * 0.0125f;
            if (Math.Abs(npc.velocity.X) > 0.1f)
                npc.spriteDirection = (npc.velocity.X < 0f).ToDirectionInt();
            if (AttackTimer < chargeDelay + chargePreparationTime)
                npc.spriteDirection = (Target.Center.X < npc.Center.X).ToDirectionInt();

            // Move through tiles and attempt to reach water if stuck on land.
            Vector2 idealPosition = Target.Center + Vector2.UnitX * (Target.Center.X < npc.Center.X).ToDirectionInt() * 480f;
            if (!InWater && AttackTimer < chargeDelay)
            {
                npc.noTileCollide = true;
                npc.noGravity = true;
                AttackTimer = 0f;

                if (WorldUtils.Find(idealPosition.ToTileCoordinates(), Searches.Chain(new Searches.Down(1500), new CustomConditions.IsWater()), out Point hitPoint))
                    idealPosition = hitPoint.ToWorldCoordinates();
                Vector2 idealVelocity = npc.SafeDirectionTo(idealPosition) * 20f;

                // Move towards the ideal position.
                if (!npc.WithinRange(idealPosition, 120f))
                    npc.velocity = Vector2.Lerp(npc.velocity, idealVelocity, 0.05f);

                // Give up and do another attack if unable to find water.
                stuckTimer++;
                if (stuckTimer >= stuckGiveupTime)
                {
                    npc.Opacity = 1f;
                    SelectNextAttack();
                }
            }
            else
                stuckTimer = 0f;

            // Handle opacity. When not in water, become almost completely transparent.
            npc.Opacity = MathHelper.Clamp(npc.Opacity + InWater.ToDirectionInt() * 0.04f, 0.33f, 1f);

            // Charge and release homing acid upward.
            if (AttackTimer >= chargeDelay && AttackTimer < chargeDelay + chargePreparationTime)
            {
                // Define the charge direction.
                if (chargeDirection == 0f)
                {
                    chargeDirection = (Target.Center.X > npc.Center.X).ToDirectionInt();
                    npc.netUpdate = true;
                }
                npc.position.Y += npc.SafeDirectionTo(Target.Center).Y * 8f;
                npc.velocity = Vector2.Lerp(npc.velocity, Vector2.UnitX * chargeDirection * chargeSpeed, 0.075f);
            }

            bool isCharging = AttackTimer >= chargeDelay + chargePreparationTime && AttackTimer < chargeDelay + chargePreparationTime + chargeTime;
            bool shouldShootAcid = isCharging && AttackTimer % acidShootRate == acidShootRate - 1f;
            if (shouldShootAcid)
            {
                Main.PlaySound(SoundID.Item95, npc.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int acidShootType = ModContent.ProjectileType<MaulerAcidDrop>();
                    int acidDamage = npc.GetProjectileDamage(acidShootType);
                    Vector2 acidSpawnPosition = npc.Center + Main.rand.NextVector2Circular(30f, 10f).RotatedBy(npc.rotation);
                    Vector2 acidShootVelocity = -Vector2.UnitY.RotatedByRandom(0.33f) * Main.rand.NextFloat(8f, 10.5f);
                    Projectile.NewProjectile(acidSpawnPosition, acidShootVelocity, acidShootType, acidDamage, 0f);
                    npc.netUpdate = true;
                }
            }

            if (npc.velocity.Length() < 0.4f)
                npc.velocity = Vector2.Zero;

            Vector2 aheadPosition = npc.position + npc.velocity.SafeNormalize(Vector2.UnitX * -npc.spriteDirection) * 150f;
            bool stuckWhileCharging = !Collision.CanHit(npc.position, npc.width, npc.height, aheadPosition, npc.width, npc.height) ||
                Collision.SolidCollision(npc.position, npc.width, npc.height);
            stuckWhileCharging &= isCharging;
            if (AttackTimer >= chargeDelay + chargePreparationTime + chargeTime || stuckWhileCharging)
            {
                npc.velocity *= 0.5f;
                SelectNextAttack();
            }
        }

        public void DoBehavior_BigChargeupDash()
        {
            int chargeDelay = 105;
            int telegraphTime = 60;
            int chargeTime = 50;
            float chargeSpeed = MathHelper.Clamp(npc.Distance(Target.Center) / chargeTime * 1.35f, 29f, 45f);
            bool hasCharged = AttackTimer >= chargeDelay;

            // Slow down at first. If necessary, fall.
            if (!hasCharged)
            {
                npc.velocity *= 0.94f;
                npc.noGravity = true;
            }

            // Select frames.
            npc.frameCounter++;
            if (npc.frameCounter >= (hasCharged ? 4 : 7))
            {
                CurrentFrame = (CurrentFrame + 1) % Main.npcFrameCount[npc.type];
                npc.frameCounter = 0;
            }

            // Open the mouth if close to the target after charging.
            if (CurrentFrame < 4 && npc.WithinRange(Target.Center, 200f) && AttackTimer > chargeDelay)
                CurrentFrame = 4;

            // Rotate towards the target at first.
            int previousSpriteDirection = npc.spriteDirection;
            float idealRotation = npc.AngleTo(Target.Center);
            if (previousSpriteDirection == 1)
                idealRotation += MathHelper.Pi;

            // Roar as a telegraph. The placement of the sound is offset such that the player and ideally determine the determine where the charge will come from.
            if (AttackTimer == chargeDelay - telegraphTime)
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/MaulerRoar"), Target.Center + Target.SafeDirectionTo(npc.Center) * 300f);

            // Charge.
            if (AttackTimer == chargeDelay)
            {
                Main.PlaySound(SoundID.DD2_FlameburstTowerShot, Target.Center);
                npc.velocity = npc.SafeDirectionTo(Target.Center) * chargeSpeed;
                npc.netUpdate = true;
            }
            if (!hasCharged)
            {
                npc.spriteDirection = (Target.Center.X < npc.Center.X).ToDirectionInt();
                if (previousSpriteDirection != npc.spriteDirection)
                    npc.rotation += MathHelper.Pi;

                npc.rotation = npc.rotation.AngleTowards(idealRotation, 0.18f).AngleLerp(idealRotation, 0.08f);
            }
            else
            {
                npc.noGravity = npc.noTileCollide = true;
                npc.rotation = npc.velocity.ToRotation();
                if (previousSpriteDirection == 1)
                    npc.rotation += MathHelper.Pi;
            }

            if (AttackTimer >= chargeDelay + chargeTime)
                SelectNextAttack();
        }

        public void SelectNextAttack()
        {
            // Reset the attack timer and optional attack variables.
            AttackTimer = 0f;
            npc.ai[2] = 0f;
            npc.ai[3] = 0f;

            MaulerAttackState oldAttack = CurrentAttack;

            bool targetDefinitelyNotNearWater = false;
            bool isInCrampedPosition = Collision.SolidCollision(npc.Center - Vector2.One * 150f, 300, 300) && InWater;
            if (Collision.SolidCollision(npc.position, npc.width, npc.height))
                isInCrampedPosition = true;

            // Register the target as not near water if they have left the ocean.
            if (Target.position.X > 8000f && Target.position.X < Main.maxTilesX * 16f - 8000f)
                targetDefinitelyNotNearWater = true;

            // Register the target as not near water if there's no water below the target's position or it's very far away.
            if (WorldUtils.Find(Target.Center.ToTileCoordinates(), Searches.Chain(new Searches.Down(750), new CustomConditions.IsWater()), out Point hitPoint))
            {
                if (Target.Distance(hitPoint.ToWorldCoordinates()) > 6400f)
                    targetDefinitelyNotNearWater = true;
            }
            else
                targetDefinitelyNotNearWater = true;

            WeightedRandom<MaulerAttackState> attackSelector = new WeightedRandom<MaulerAttackState>(Main.rand);
            attackSelector.Add(MaulerAttackState.BubbleBursts);
            attackSelector.Add(MaulerAttackState.HorizontalAcidCharge);
            attackSelector.Add(MaulerAttackState.BigChargeupDash);

            // Select the new attack.
            // It is random, but will never perform the same attack twice in succession.
            do
                CurrentAttack = attackSelector.Get();
            while (oldAttack == CurrentAttack);

            // Bypass the above check and use an attack that doesn't require water if there's none nearby.
            if (targetDefinitelyNotNearWater || isInCrampedPosition)
                CurrentAttack = MaulerAttackState.BigChargeupDash;

            npc.netUpdate = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = Main.npcTexture[npc.type];
            Texture2D glowmask = ModContent.GetTexture("CalamityMod/NPCs/AcidRain/MaulerGlowmask");
            Vector2 drawPosition = npc.Center - Main.screenPosition + Vector2.UnitY * npc.gfxOffY;
            Vector2 origin = npc.frame.Size() * 0.5f;
            SpriteEffects direction = npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            spriteBatch.Draw(texture, drawPosition, npc.frame, npc.GetAlpha(drawColor), npc.rotation, origin, npc.scale, direction, 0f);
            spriteBatch.Draw(glowmask, drawPosition, npc.frame, npc.GetAlpha(Color.White), npc.rotation, origin, npc.scale, direction, 0f);
            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frame.Y = CurrentFrame * frameHeight;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<Irradiated>(), 420);
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(npc, ModContent.ItemType<SulphuricAcidCannon>(), 3);

            // Drop shark fins. They are specially colored to match the color of maulers.
            int item = Item.NewItem(npc.Center, npc.Size, ItemID.SharkFin, Main.rand.Next(2, 5), false, 0, false, false);
            Main.item[item].color = new Color(151, 115, 57, 255);
            NetMessage.SendData(MessageID.ItemTweaker, -1, -1, null, item, 1f, 0f, 0f, 0, 0, 0);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);

            if (npc.life <= 0)
            {
                for (int k = 0; k < 30; k++)
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);

                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Mauler"), npc.scale);
                for (int i = 2; i <= 5; i++)
                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot($"Gores/Mauler{i}"), npc.scale);
            }
        }
    }
}
