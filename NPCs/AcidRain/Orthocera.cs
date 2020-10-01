using CalamityMod.Dusts;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.World;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using CalamityMod.Buffs.StatDebuffs;
namespace CalamityMod.NPCs.AcidRain
{
    public class Orthocera : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Orthocera");
            Main.npcFrameCount[npc.type] = 5;
        }

        public override void SetDefaults()
        {
            npc.width = 62;
            npc.height = 34;
            npc.aiStyle = aiType = -1;

            npc.damage = 45;
            npc.lifeMax = 280;
            npc.defense = 15;
			npc.DR_NERD(0.075f);

            if (CalamityWorld.downedPolterghast)
            {
                npc.damage = 120;
                npc.lifeMax = 7000;
                npc.defense = 35;
				npc.DR_NERD(0.15f);
            }

            npc.knockBackResist = 0.6f;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.value = Item.buyPrice(0, 0, 4, 20);
            npc.lavaImmune = false;
            npc.noGravity = true;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit41;
            npc.DeathSound = SoundID.NPCDeath13;
            banner = npc.type;
            bannerItem = ModContent.ItemType<OrthoceraBanner>();
        }

        public override void AI()
        {
            npc.TargetClosest(false);
            npc.ai[1] += 1f;
            float maxSpeed = CalamityWorld.downedPolterghast ? 12.8f : 10.5f;
            if (npc.target >= 0 && npc.target < 255)
            {
                Player player = Main.player[npc.target];
                // Swim
                if (npc.ai[1] % 250f < 180f)
                {
                    if (npc.wet)
                    {
                        // Reset so we can jum again.
                        npc.ai[3] = 0f;

                        // Swim towards the player if we're not too close.
                        // If we're close, simply resume our current movement.
                        if (npc.Distance(player.Center) > 150f)
                        {
                            npc.velocity = (npc.velocity * 17f + npc.DirectionTo(player.Center) * maxSpeed) / 18f;
                            if (npc.ai[0] != 12f)
                            {
                                npc.ai[0] = 12f;
                                npc.netUpdate = true;
                            }
                        }
                        // Variable for X movement later.
                        // It seems to slow down for some dumb reason in the jump phase without this value
                        npc.ai[2] = npc.velocity.X;

                        // Make sure the value isn't 0 since we're relying on multiplication
                        if (npc.ai[2] == 0f)
                            npc.ai[2] = 0.1f;

                        if (Math.Abs(npc.ai[2]) < 7f)
                            npc.ai[2] = Math.Abs(npc.ai[2]) * 7f;
                        if (Math.Abs(npc.ai[2]) > 16f)
                            npc.ai[2] = Math.Abs(npc.ai[2]) * 16f;
                        npc.ai[2] = Math.Abs(npc.ai[2]) * (player.Center.X - npc.Center.X > 0).ToDirectionInt();
                    }
                    else
                    {
                        if (npc.ai[0] <= 0f)
                            npc.velocity.Y += 0.2f;
                        else
                            npc.ai[0] -= 1f;
                    }
                    npc.direction = npc.spriteDirection = (npc.velocity.X > 0).ToDirectionInt();
                }
                // And jump/shoot
                else if (npc.ai[1] % 220f > 180f)
                {
                    float yAcceleration = CalamityWorld.downedPolterghast ? 0.07f : 0.05f;
                    if (npc.ai[1] % 220f < 200f)
                    {
                        npc.velocity.Y -= yAcceleration;
                    }
                    else
                    {
                        npc.velocity.Y += yAcceleration;
                    }
                    if (npc.ai[1] % 220f == 219f)
                        npc.ai[3] = 1f;
                    if (!npc.wet)
                        npc.velocity.X = npc.ai[2];
                }
                // Don't jump mid-air
                if (npc.ai[1] % 220f > 180f && npc.ai[3] == 1f)
                {
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                }
                npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver4 + MathHelper.PiOver2 + MathHelper.Pi;

                if (npc.spriteDirection == -1)
                    npc.rotation -= MathHelper.PiOver2;
                // If sitting on land, slow down and, if in the middle of a jump, release a stream of acid.
                if (!npc.wet)
                {
                    npc.velocity.X *= 0.92f;
                    // Spit out a stream of acid based on our rotation
                    if (npc.ai[1] % 220f == 195f)
                    {
                        float rotation = npc.rotation - MathHelper.Pi - MathHelper.PiOver2 - MathHelper.PiOver4;
                        if (npc.spriteDirection == -1)
                            rotation += MathHelper.PiOver2;

                        int damage = CalamityWorld.downedPolterghast ? 40 : CalamityWorld.downedAquaticScourge ? 26 : 18;
						if (Main.expertMode)
							damage = (int)Math.Round(damage * 0.8);

                        if (CalamityWorld.downedPolterghast)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                float angle = MathHelper.Lerp(-0.3f, 0.3f, i / 2f);
                                Projectile.NewProjectile(npc.Center, (rotation + angle).ToRotationVector2() * 10f, ModContent.ProjectileType<OrthoceraStream>(), damage, 2f);
                            }
                        }
                        Projectile.NewProjectile(npc.Center, rotation.ToRotationVector2() * 12f, ModContent.ProjectileType<OrthoceraStream>(), damage, 2f);
                    }
                }
                // Prevent yeeting into the sky at the speed of light
                npc.velocity = Vector2.Clamp(npc.velocity, new Vector2(-maxSpeed), new Vector2(maxSpeed));
            }
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(npc, ModContent.ItemType<CorrodedFossil>(), 3 * (CalamityWorld.downedPolterghast ? 5 : 1), 1, 3);
            DropHelper.DropItemChance(npc, ModContent.ItemType<OrthoceraShell>(), 20);
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter >= 6)
            {
                npc.frameCounter = 0;
                npc.frame.Y += frameHeight;
                if (npc.frame.Y >= Main.npcFrameCount[npc.type] * frameHeight)
                {
                    npc.frame.Y = 0;
                }
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/OrthoceraGore"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/OrthoceraGore2"), npc.scale);
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }
    }
}
