using CalamityMod.BiomeManagers;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.WorldBuilding;
using Terraria.Audio;

namespace CalamityMod.NPCs.AcidRain
{
    [AutoloadBossHead]
    public class Mauler : ModNPC
    {
        // TODO -- Potentially add another attack in higher difficulty modes at some point?
        public enum MaulerAttackState
        {
            BubbleBursts,
            HorizontalAcidCharge,
            BigChargeupDash
        }

        public bool InWater => Collision.DrownCollision(NPC.Center, 1, 1);
        public Player Target => Main.player[NPC.target];
        public MaulerAttackState CurrentAttack
        {
            get => (MaulerAttackState)(int)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }
        public ref float AttackTimer => ref NPC.ai[1];
        public int CurrentFrame
        {
            get => (int)NPC.localAI[0];
            set => NPC.localAI[0] = value;
        }

        public static readonly SoundStyle RoarSound = new("CalamityMod/Sounds/Custom/MaulerRoar");

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Scale = 0.425f,
                PortraitScale = 0.9f,
                SpriteDirection = 1
            };
            value.Position.X += 10f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.noGravity = true;
            NPC.damage = 135;
            NPC.width = 180;
            NPC.height = 90;
            NPC.defense = 50;
            NPC.DR_NERD(0.05f);
            NPC.lifeMax = 90000;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.value = Item.buyPrice(0, 25, 0, 0);
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath60;
            NPC.knockBackResist = 0f;
            NPC.waterMovementSpeed = 1f;
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<AcidRainBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Mauler")
            });
        }

        public override void AI()
        {
            NPC.TargetClosest(false);

            // Reset things every frame. They may be changed in the attack states below.
            NPC.noTileCollide = false;
            NPC.noGravity = InWater;

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
            Vector2 left = NPC.Center - Vector2.UnitX * 15f;
            Vector2 right = NPC.Center + Vector2.UnitX * 15f;
            DelegateMethods.v3_1 = Color.Lime.ToVector3();
            Utils.PlotTileLine(left, right, 10f, DelegateMethods.CastLightOpen);

            // Drift away from the world border.
            if (NPC.Center.X < (Main.offLimitBorderTiles + 6f) * 16f || NPC.Center.X >= (Main.maxTilesX - Main.offLimitBorderTiles - 6f) * 16f)
            {
                NPC.velocity *= 0.9f;
                NPC.Center = NPC.Center.MoveTowards(Target.Center, 12f);
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
            NPC.velocity *= 0.94f;
            if (!InWater)
            {
                NPC.noTileCollide = true;
                NPC.position.Y += 6f;
            }

            // Roar prior to releasing bubbles.
            if (AttackTimer <= roarDelay)
                CurrentFrame = (int)Math.Round(Utils.GetLerpValue(0f, roarDelay, AttackTimer, true) * 5f);

            // Animate as usual a bit after roaring.
            else if (AttackTimer >= roarDelay + shootDelay)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter >= 5)
                {
                    CurrentFrame = (CurrentFrame + 1) % Main.npcFrameCount[NPC.type];
                    NPC.frameCounter = 0;
                }

                // Open the mouth prior to creating bubbles.
                if (wrappedAttackTimer >= burstShootRate * 0.7f)
                    CurrentFrame = 4;
            }
            if (AttackTimer == roarDelay)
                SoundEngine.PlaySound(SoundID.Zombie97, Target.Center);

            // Rotate towards the target.
            int previousSpriteDirection = NPC.spriteDirection;
            float idealRotation = NPC.AngleTo(Target.Center);
            if (previousSpriteDirection == 1)
                idealRotation += MathHelper.Pi;
            NPC.spriteDirection = (Target.Center.X < NPC.Center.X).ToDirectionInt();
            if (previousSpriteDirection != NPC.spriteDirection)
                NPC.rotation += MathHelper.Pi;

            NPC.rotation = NPC.rotation.AngleTowards(idealRotation, 0.125f).AngleLerp(idealRotation, 0.075f);

            // Fire the bubble bursts.
            bool shouldShootBubbles = AttackTimer >= roarDelay + shootDelay && wrappedAttackTimer == burstShootRate - 1f;
            if (shouldShootBubbles)
            {
                Vector2 mouthPosition = NPC.Center + new Vector2(NPC.spriteDirection * -56f, 22f).RotatedBy(NPC.rotation);
                SoundEngine.PlaySound(SoundID.Item96, mouthPosition);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int bubbleShootType = ModContent.ProjectileType<MaulerAcidBubble>();
                    int bubbleDamage = NPC.GetProjectileDamage(bubbleShootType);
                    Vector2 baseBubbleShootVelocity = NPC.SafeDirectionTo(mouthPosition) * 13.5f;
                    for (int i = 0; i < bubblesPerBurst; i++)
                    {
                        Vector2 bubbleShootVelocity = baseBubbleShootVelocity + Main.rand.NextVector2Circular(4f, 4f);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), mouthPosition, bubbleShootVelocity, bubbleShootType, bubbleDamage, 0f);
                    }

                    // Get launched back after firing. This only happens if in water and there's no obstacles behind.
                    if (InWater && Collision.CanHit(NPC.Center, 1, 1, NPC.Center - baseBubbleShootVelocity.SafeNormalize(Vector2.Zero) * 100f, 1, 1))
                        NPC.velocity -= baseBubbleShootVelocity * 0.62f;
                    NPC.netUpdate = true;
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
            ref float stuckTimer = ref NPC.ai[2];
            ref float chargeDirection = ref NPC.ai[3];

            // Select frames.
            NPC.frameCounter++;
            if (NPC.frameCounter >= 5)
            {
                CurrentFrame = (CurrentFrame + 1) % Main.npcFrameCount[NPC.type];
                NPC.frameCounter = 0;
            }

            // Decide the rotation and direction.
            NPC.rotation = NPC.velocity.X * 0.0125f;
            if (Math.Abs(NPC.velocity.X) > 0.1f)
                NPC.spriteDirection = (NPC.velocity.X < 0f).ToDirectionInt();
            if (AttackTimer < chargeDelay + chargePreparationTime)
                NPC.spriteDirection = (Target.Center.X < NPC.Center.X).ToDirectionInt();

            // Move through tiles and attempt to reach water if stuck on land.
            Vector2 idealPosition = Target.Center + Vector2.UnitX * (Target.Center.X < NPC.Center.X).ToDirectionInt() * 480f;
            if (!InWater && AttackTimer < chargeDelay)
            {
                NPC.noTileCollide = true;
                NPC.noGravity = true;
                AttackTimer = 0f;

                if (WorldUtils.Find(idealPosition.ToTileCoordinates(), Searches.Chain(new Searches.Down(1500), new CustomConditions.IsWater()), out Point hitPoint))
                    idealPosition = hitPoint.ToWorldCoordinates();
                Vector2 idealVelocity = NPC.SafeDirectionTo(idealPosition) * 20f;

                // Move towards the ideal position.
                if (!NPC.WithinRange(idealPosition, 120f))
                    NPC.velocity = Vector2.Lerp(NPC.velocity, idealVelocity, 0.05f);

                // Give up and do another attack if unable to find water.
                stuckTimer++;
                if (stuckTimer >= stuckGiveupTime)
                {
                    NPC.Opacity = 1f;
                    SelectNextAttack();
                }
            }
            else
                stuckTimer = 0f;

            // Handle opacity. When not in water, become almost completely transparent.
            NPC.Opacity = MathHelper.Clamp(NPC.Opacity + InWater.ToDirectionInt() * 0.04f, 0.33f, 1f);

            // Charge and release homing acid upward.
            if (AttackTimer >= chargeDelay && AttackTimer < chargeDelay + chargePreparationTime)
            {
                // Define the charge direction.
                if (chargeDirection == 0f)
                {
                    chargeDirection = (Target.Center.X > NPC.Center.X).ToDirectionInt();
                    NPC.netUpdate = true;
                }
                NPC.position.Y += NPC.SafeDirectionTo(Target.Center).Y * 8f;
                NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.UnitX * chargeDirection * chargeSpeed, 0.075f);
            }

            bool isCharging = AttackTimer >= chargeDelay + chargePreparationTime && AttackTimer < chargeDelay + chargePreparationTime + chargeTime;
            bool shouldShootAcid = isCharging && AttackTimer % acidShootRate == acidShootRate - 1f;
            if (shouldShootAcid)
            {
                SoundEngine.PlaySound(SoundID.Item95, NPC.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int acidShootType = ModContent.ProjectileType<MaulerAcidDrop>();
                    int acidDamage = NPC.GetProjectileDamage(acidShootType);
                    Vector2 acidSpawnPosition = NPC.Center + Main.rand.NextVector2Circular(30f, 10f).RotatedBy(NPC.rotation);
                    Vector2 acidShootVelocity = -Vector2.UnitY.RotatedByRandom(0.33f) * Main.rand.NextFloat(8f, 10.5f);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), acidSpawnPosition, acidShootVelocity, acidShootType, acidDamage, 0f);
                    NPC.netUpdate = true;
                }
            }

            if (NPC.velocity.Length() < 0.4f)
                NPC.velocity = Vector2.Zero;

            Vector2 aheadPosition = NPC.position + NPC.velocity.SafeNormalize(Vector2.UnitX * -NPC.spriteDirection) * 150f;
            bool stuckWhileCharging = !Collision.CanHit(NPC.position, NPC.width, NPC.height, aheadPosition, NPC.width, NPC.height) ||
                Collision.SolidCollision(NPC.position, NPC.width, NPC.height);
            stuckWhileCharging &= isCharging;
            if (AttackTimer >= chargeDelay + chargePreparationTime + chargeTime || stuckWhileCharging)
            {
                NPC.velocity *= 0.5f;
                SelectNextAttack();
            }
        }

        public void DoBehavior_BigChargeupDash()
        {
            int chargeDelay = 105;
            int telegraphTime = 60;
            int chargeTime = 50;
            float chargeSpeed = MathHelper.Clamp(NPC.Distance(Target.Center) / chargeTime * 1.35f, 29f, 45f);
            bool hasCharged = AttackTimer >= chargeDelay;

            // Slow down at first. If necessary, fall.
            if (!hasCharged)
            {
                NPC.velocity *= 0.94f;
                NPC.noGravity = true;
            }

            // Select frames.
            NPC.frameCounter++;
            if (NPC.frameCounter >= (hasCharged ? 4 : 7))
            {
                CurrentFrame = (CurrentFrame + 1) % Main.npcFrameCount[NPC.type];
                NPC.frameCounter = 0;
            }

            // Open the mouth if close to the target after charging.
            if (CurrentFrame < 4 && NPC.WithinRange(Target.Center, 200f) && AttackTimer > chargeDelay)
                CurrentFrame = 4;

            // Rotate towards the target at first.
            int previousSpriteDirection = NPC.spriteDirection;
            float idealRotation = NPC.AngleTo(Target.Center);
            if (previousSpriteDirection == 1)
                idealRotation += MathHelper.Pi;

            // Roar as a telegraph. The placement of the sound is offset such that the player and ideally determine the determine where the charge will come from.
            if (AttackTimer == chargeDelay - telegraphTime)
                SoundEngine.PlaySound(RoarSound, Target.Center + Target.SafeDirectionTo(NPC.Center) * 300f);

            // Charge.
            if (AttackTimer == chargeDelay)
            {
                SoundEngine.PlaySound(SoundID.DD2_FlameburstTowerShot, Target.Center);
                NPC.velocity = NPC.SafeDirectionTo(Target.Center) * chargeSpeed;
                NPC.netUpdate = true;
            }
            if (!hasCharged)
            {
                NPC.spriteDirection = (Target.Center.X < NPC.Center.X).ToDirectionInt();
                if (previousSpriteDirection != NPC.spriteDirection)
                    NPC.rotation += MathHelper.Pi;

                NPC.rotation = NPC.rotation.AngleTowards(idealRotation, 0.18f).AngleLerp(idealRotation, 0.08f);
            }
            else
            {
                NPC.noGravity = NPC.noTileCollide = true;
                NPC.rotation = NPC.velocity.ToRotation();
                if (previousSpriteDirection == 1)
                    NPC.rotation += MathHelper.Pi;
            }

            if (AttackTimer >= chargeDelay + chargeTime)
                SelectNextAttack();
        }

        public void SelectNextAttack()
        {
            // Reset the attack timer and optional attack variables.
            AttackTimer = 0f;
            NPC.ai[2] = 0f;
            NPC.ai[3] = 0f;

            MaulerAttackState oldAttack = CurrentAttack;

            bool targetDefinitelyNotNearWater = false;
            bool isInCrampedPosition = Collision.SolidCollision(NPC.Center - Vector2.One * 150f, 300, 300) && InWater;
            if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
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

            NPC.netUpdate = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D glowmask = ModContent.Request<Texture2D>("CalamityMod/NPCs/AcidRain/MaulerGlowmask").Value;
            Vector2 drawPosition = NPC.Center - screenPos + Vector2.UnitY * NPC.gfxOffY;
            Vector2 origin = NPC.frame.Size() * 0.5f;
            SpriteEffects direction = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            spriteBatch.Draw(texture, drawPosition, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, direction, 0f);
            spriteBatch.Draw(glowmask, drawPosition, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, origin, NPC.scale, direction, 0f);
            return false;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Y = CurrentFrame * frameHeight;
            if (NPC.IsABestiaryIconDummy)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter >= 5)
                {
                    CurrentFrame = (CurrentFrame + 1) % Main.npcFrameCount[NPC.type];
                    NPC.frameCounter = 0;
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<Irradiated>(), 420);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemID.SharkFin, 1, 2, 4);
            npcLoot.Add(ModContent.ItemType<SulphuricAcidCannon>(), 3);
            npcLoot.Add(ModContent.ItemType<MaulerTrophy>(), 10);
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<MaulerRelic>());
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);

            if (NPC.life <= 0)
            {
                for (int k = 0; k < 30; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);

                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Mauler").Type, NPC.scale);
                    for (int i = 2; i <= 5; i++)
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>($"Mauler{i}").Type, NPC.scale);
                }
            }
        }

        public override void OnKill()
        {
            if (Main.zenithWorld)
            {
                Vector2 valueBoom = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                float spreadBoom = 15f * 0.0174f;
                double startAngleBoom = Math.Atan2(NPC.velocity.X, NPC.velocity.Y) - spreadBoom / 2;
                double deltaAngleBoom = spreadBoom / 8f;
                double offsetAngleBoom;
                int iBoom;
                int damageBoom = 200;
                for (iBoom = 0; iBoom < 25; iBoom++)
                {
                    int projectileType = Main.rand.NextBool() ? ModContent.ProjectileType<SulphuricAcidMist>() : ModContent.ProjectileType<SulphuricAcidBubble>();
                    offsetAngleBoom = startAngleBoom + deltaAngleBoom * (iBoom + iBoom * iBoom) / 2f + 32f * iBoom;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                    int boom1 = Projectile.NewProjectile(NPC.GetSource_Death(), valueBoom.X, valueBoom.Y, (float)(Math.Sin(offsetAngleBoom) * 6f), (float)(Math.Cos(offsetAngleBoom) * 6f), projectileType, damageBoom, 0f, Main.myPlayer, 0f, 0f);
                    int boom2 = Projectile.NewProjectile(NPC.GetSource_Death(), valueBoom.X, valueBoom.Y, (float)(-Math.Sin(offsetAngleBoom) * 6f), (float)(-Math.Cos(offsetAngleBoom) * 6f), projectileType, damageBoom, 0f, Main.myPlayer, 0f, 0f);
                    }
                }
                for (int i = 0; i < 25; i++)
                {
                    int deathDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, 31, 0f, 0f, 100, default, 2f);
                    Main.dust[deathDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[deathDust].scale = 0.5f;
                        Main.dust[deathDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                    Main.dust[deathDust].noGravity = true;
                }
                for (int j = 0; j < 50; j++)
                {
                    int deathDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, 5, 0f, 0f, 100, default, 3f);
                    Main.dust[deathDust2].noGravity = true;
                    Main.dust[deathDust2].velocity *= 5f;
                    deathDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, 5, 0f, 0f, 100, default, 2f);
                    Main.dust[deathDust2].velocity *= 2f;
                    Main.dust[deathDust2].noGravity = true;
                }
                NPC.netUpdate = true;
            }
            // Mark Mauler as dead
            DownedBossSystem.downedMauler = true;
            CalamityNetcode.SyncWorld();
        }
    }
}
