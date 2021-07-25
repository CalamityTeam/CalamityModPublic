using CalamityMod.Dusts;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.AcidRain
{
    public class Skyfin : ModNPC
    {
        public bool Flying = false;
        public Player Target => Main.player[npc.target];
        public ref float Time => ref npc.ai[0];
        public ref float DiveTimer => ref npc.ai[1];
        public ref float HorizontalDiveSpeed => ref npc.ai[2];
        public ref float WaitBeforeFlyingMovement => ref npc.ai[3];
        public const float DiveDelay = 120f;
        public const float DiveTime = 90f;
        public const float TotalDiveTime = DiveDelay + DiveTime;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Skyfin");
            Main.npcFrameCount[npc.type] = 5;
        }

        public override void SetDefaults()
        {
            npc.width = 46;
            npc.height = 22;
            npc.aiStyle = aiType = -1;

            npc.damage = 12;
            npc.lifeMax = 70;
            npc.defense = 6;
            npc.knockBackResist = 1f;

            if (CalamityWorld.downedPolterghast)
            {
				npc.knockBackResist = 0.8f;
                npc.damage = 88;
                npc.lifeMax = 3025;
                npc.defense = 18;
				npc.DR_NERD(0.05f);
            }
            else if (CalamityWorld.downedAquaticScourge)
            {
                npc.damage = 38;
                npc.lifeMax = 220;
				npc.DR_NERD(0.05f);
            }

            npc.value = Item.buyPrice(0, 0, 3, 65);
            npc.lavaImmune = false;
            npc.noGravity = true;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            banner = npc.type;
            bannerItem = ModContent.ItemType<SkyfinBanner>();
        }

        public override void SendExtraAI(BinaryWriter writer) => writer.Write(Flying);

        public override void ReceiveExtraAI(BinaryReader reader) => Flying = reader.ReadBoolean();

        public override void AI()
        {
            npc.TargetClosest(false);

            if (Flying)
                FlyTowardsTarget();
            else
                DoSwimMovement();
            int idealDirection = (npc.velocity.X > 0).ToDirectionInt();
            npc.direction = npc.spriteDirection = idealDirection;
            if (idealDirection != npc.direction)
                npc.netUpdate = true;

            npc.rotation = npc.velocity.ToRotation() + (npc.direction > 0).ToInt() * MathHelper.Pi;
        }

        public void FlyTowardsTarget()
		{
            npc.noTileCollide = true;
            Time++;
            if (Time % 300f >= 180f)
            {
                // Rise upward.
                if (Time % 300f <= 205f)
                    npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, -6.5f, 0.07f);

                // And fly towards the target.
                if (Time % 300f == 235f)
                {
                    float chargeSpeed = 8f;
                    if (CalamityWorld.downedAquaticScourge)
                        chargeSpeed = 14f;
                    if (CalamityWorld.downedPolterghast)
                        chargeSpeed = 18f;
                    npc.velocity = npc.SafeDirectionTo(Target.Center, Vector2.UnitY) * chargeSpeed;
                }
            }
            else
            {
                if (Math.Abs(Target.Center.X - npc.Center.X) > 320f)
                    npc.velocity.X = MathHelper.Lerp(npc.velocity.X, (Target.Center.X - npc.Center.X > 0).ToDirectionInt() * 10f, 0.05f);
                if (Math.Abs(Target.Center.Y - npc.Center.Y) > 50f)
                    npc.velocity.Y = npc.SafeDirectionTo(Target.Center).Y * 9f;
            }
        }

        public void DoSwimMovement()
		{
            Time++;
            if (DiveTimer > 0f)
                DiveTimer -= 1f;
            if (npc.wet)
            {
                // Swim around, moving towards the player.
                bool canSwimToPlayer = Collision.CanHit(npc.position, npc.width, npc.height, Target.position, Target.width, Target.height);
                if (canSwimToPlayer)
                {
                    if (Time % 55f == 54f)
                    {
                        float horizontalSchoolingSpeed = 9f;
                        if (CalamityWorld.downedAquaticScourge)
                            horizontalSchoolingSpeed = 15f;
                        if (CalamityWorld.downedPolterghast)
                            horizontalSchoolingSpeed = 24f;
                        npc.velocity = Vector2.UnitX * (Target.Center.X - npc.Center.X > 0).ToDirectionInt() * horizontalSchoolingSpeed;
                    }
                    if ((Math.Abs(Target.Center.Y - npc.Center.Y) > 50f && Target.wet) || (!Target.wet && DiveTimer <= 0f))
                    {
                        float verticalSwimSpeed = CalamityWorld.downedPolterghast ? 10f : 6f;
                        npc.velocity.Y = (Target.Center.Y - npc.Center.Y > 0).ToDirectionInt() * verticalSwimSpeed;
                    }

                    // Speed up if there is little horizontal movement.
                    if (Math.Abs(npc.velocity.X) < 6f)
                        npc.velocity.X *= 1.04f;
                }

                // Slow down increasingly vertically if it's already low.
                else if (!canSwimToPlayer && Math.Abs(npc.velocity.Y) < 4f)
                    npc.velocity.Y *= 0.97f;

                // Turn around if we hit a tile on the X axis
                if (!canSwimToPlayer && npc.collideX)
                    npc.velocity.X *= -1f;

                // Check if a dive is possible.
                if (Target.Center.Y < npc.Top.Y - 10f && DiveTimer <= 0f)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(10))
                    {
                        DiveTimer = TotalDiveTime;
                        npc.netUpdate = true;
                    }

                    HorizontalDiveSpeed = (Target.Center.X - npc.Center.X > 0).ToDirectionInt() * 10f;
                }
            }
            else
            {
                // Dive upward in an attempt to hit to the player.
                if (DiveTimer > TotalDiveTime - DiveTime)
                {
                    npc.velocity.X = HorizontalDiveSpeed;
                    if (DiveTimer > TotalDiveTime - DiveTime * 0.5f)
                    {
                        float upwardDiveSpeed = CalamityWorld.downedAquaticScourge ? 0.115f : 0.085f;
                        if (CalamityWorld.downedPolterghast)
                            upwardDiveSpeed = 0.135f;
                        npc.velocity.Y -= upwardDiveSpeed;
                    }
                    else
                    {
                        DiveTimer = TotalDiveTime - DiveTime;
                        npc.velocity.Y += 0.2f;
                    }
                }
                else
                {
                    // Don't fall too fast because of wings
                    DiveTimer = TotalDiveTime - DiveTime;
                    npc.velocity.Y += 0.1f;
                    WaitBeforeFlyingMovement++;

                    // Consistently sync the enemy.
                    if (WaitBeforeFlyingMovement % 40f == 39f)
                        npc.netUpdate = true;

                    if (WaitBeforeFlyingMovement > 180f)
                    {
                        Time = DiveTimer = HorizontalDiveSpeed = WaitBeforeFlyingMovement = 0f;
                        Flying = true;
                        npc.netUpdate = true;
                    }
                }
            }
            // If sitting on land, rotate in a way that looks like we're stuck on the ground
            if (!npc.wet)
            {
                npc.velocity.X *= 0.92f;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter >= 5)
            {
                npc.frameCounter = 0;
                npc.frame.Y += frameHeight;
                if (npc.frame.Y >= Main.npcFrameCount[npc.type] * frameHeight)
                    npc.frame.Y = 0;
            }
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(npc, ModContent.ItemType<SulfuricScale>(), 2 * (CalamityWorld.downedAquaticScourge ? 6 : 1), 1, 3);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<SkyfinBombers>(), CalamityWorld.downedAquaticScourge, 0.05f);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 8; k++)
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/SkyfinGore"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/SkyfinGore2"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/SkyfinGore3"), npc.scale);
                for (int k = 0; k < 20; k++)
                    Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit) => target.AddBuff(ModContent.BuffType<Irradiated>(), 120);
    }
}
