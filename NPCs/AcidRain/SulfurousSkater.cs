using CalamityMod.Dusts;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.StatDebuffs;

namespace CalamityMod.NPCs.AcidRain
{
    public class SulfurousSkater : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sulphurous Skater");
            Main.npcFrameCount[npc.type] = 5;
        }

        public override void SetDefaults()
        {
            npc.width = 48;
            npc.height = 48;

            npc.damage = 70;
            npc.lifeMax = 710;
            npc.defense = 3;

            if (CalamityWorld.downedPolterghast)
            {
                npc.damage = 210;
                npc.lifeMax = 6600;
                npc.defense = 33;
            }

            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 0, 5, 25);
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.lavaImmune = false;
            npc.noGravity = true;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            banner = npc.type;
            bannerItem = ModContent.ItemType<SulfurousSkaterBanner>();

            npc.aiStyle = aiType = -1;
        }
        public override void AI()
        {
            npc.TargetClosest(false);
            Player player = Main.player[npc.target];
            if (npc.ai[0] == 0f)
            {
                npc.Calamity().DR = 0.35f;
                npc.noGravity = false;
                float minimumDistance = float.PositiveInfinity;
                Projectile closestBubble = null;
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    if (Main.projectile[i].type == ModContent.ProjectileType<SulphuricAcidBubble>() && Main.projectile[i].active)
                    {
                        if (Math.Abs(npc.Center.X - Main.projectile[i].Center.X) < minimumDistance &&
                            Collision.CanHit(npc.position, npc.width, npc.height, Main.projectile[i].position, Main.projectile[i].width, Main.projectile[i].height) &&
                            Main.projectile[i].Center.Y > npc.Bottom.Y)
                        {
                            minimumDistance = npc.Distance(Main.projectile[i].Center);
                            closestBubble = Main.projectile[i];
                        }
                    }
                }
                if (minimumDistance >= 2400f)
                {
                    closestBubble = null;
                }

                Vector2 destination = player.Center;

                if (closestBubble != null)
                {
                    destination = closestBubble.Center;
                }
                // Stay on water instead of falling into it
                if (npc.wet)
                {
                    if (npc.velocity.Y >= 0f)
                        npc.velocity.Y = -3f;
                }

                if (closestBubble != null && minimumDistance < 200f)
                {
                    npc.velocity.Y += 0.2f;

                    if (closestBubble.Hitbox.Intersects(npc.Hitbox))
                    {
                        npc.ai[0] = 1f;
                        npc.netUpdate = true;
                        closestBubble.Kill();
                    }
                }
                if (npc.velocity.Y == 0f || npc.wet)
                {
                    npc.TargetClosest(false);
                    npc.velocity.X *= 0.85f;
                    npc.ai[1]++;
                    float lungeForwardSpeed = 13f;
                    float jumpSpeed = 3f;
                    if (Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                    {
                        lungeForwardSpeed *= 1.2f;
                    }
                    if (npc.ai[1] >= 10)
                    {
                        npc.ai[1] = 0f;
                        npc.velocity.Y -= jumpSpeed;
                        npc.velocity.X = lungeForwardSpeed * (npc.Center.X - destination.X < 0).ToDirectionInt();
                        npc.spriteDirection = (npc.Center.X - destination.X < 0).ToDirectionInt();
                    }
                }
                else
                {
                    npc.knockBackResist = 0f;
                }
            }
            else
            {
                npc.Calamity().DR = 0f;
                float speed = CalamityWorld.downedPolterghast ? 24f : 16f;
                float inertia = CalamityWorld.downedPolterghast ? 16f : 28f;
                if (npc.Distance(player.Center) < 200f)
                    inertia *= 0.667f;
                npc.velocity = (npc.velocity * inertia + npc.DirectionTo(player.Center) * speed) / (inertia + 1f);

                float SAImovement = 0.5f;
                for (int index = 0; index < Main.npc.Length; index++)
                {
                    NPC other = Main.npc[index];
                    if (index != npc.whoAmI &&
                        other.active &&
                        other.type == npc.type && Math.Abs(npc.position.X - other.position.X) + Math.Abs(npc.position.Y - other.position.Y) < npc.width)
                    {
                        if (npc.position.X < other.position.X)
                        {
                            npc.velocity.X -= SAImovement;
                        }
                        else
                        {
                            npc.velocity.X += SAImovement;
                        }
                        if (npc.position.Y < other.position.Y)
                        {
                            npc.velocity.Y -= SAImovement;
                        }
                        else
                        {
                            npc.velocity.Y += SAImovement;
                        }
                    }
                }
                npc.spriteDirection = (npc.velocity.X > 0).ToDirectionInt();
            }
        }
        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            if (npc.ai[0] != 0f)
                npc.ai[0] = 0f;
        }
        public override void FindFrame(int frameHeight)
        {
            if (npc.ai[0] == 0f)
                npc.frame.Y = 0;
            else
            {
                npc.frameCounter++;
                if (npc.frameCounter >= 4)
                {
                    npc.frameCounter = 0;
                    npc.frame.Y += frameHeight;
                    if (npc.frame.Y >= Main.npcFrameCount[npc.type] * frameHeight)
                    {
                        npc.frame.Y = frameHeight;
                    }
                }
            }
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.85f);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 8; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
            }
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 120);
        }
    }
}
