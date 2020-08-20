using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.AcidRain
{
    public class SulfurousSkater : ModNPC
    {
        public bool Flying = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sulphurous Skater");
            Main.npcFrameCount[npc.type] = 5;
            NPCID.Sets.TrailingMode[npc.type] = 1;
            NPCID.Sets.TrailCacheLength[npc.type] = 6;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Flying);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Flying = reader.ReadBoolean();
        }
        public override void SetDefaults()
        {
            npc.width = 48;
            npc.height = 48;

            npc.damage = 70;
            npc.lifeMax = 520;
            npc.defense = 3;

            if (CalamityWorld.downedPolterghast)
            {
                npc.damage = 100;
                npc.lifeMax = 5000;
                npc.defense = 33;
            }

            npc.knockBackResist = 0.8f;
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
            if (!Flying)
            {
				npc.knockBackResist = 0.8f;
                npc.DR_NERD(0.35f);
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
                    {
                        npc.velocity.Y = -3f;
                    }
                }

                if (closestBubble != null && minimumDistance < 200f)
                {
                    npc.velocity.Y += 0.2f;

                    if (closestBubble.Hitbox.Intersects(npc.Hitbox))
                    {
                        Flying = true;
                        closestBubble.Kill();
                        npc.netSpam = 0;
                        npc.netUpdate = true;
                    }
                }
                if (npc.velocity.Y == 0f || npc.wet)
                {
                    npc.TargetClosest(false);
                    npc.velocity.X *= 0.85f;
                    npc.ai[1]++;
                    float lungeForwardSpeed = 15f;
                    float jumpSpeed = 4f;
                    if (Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                    {
                        lungeForwardSpeed *= 1.2f;
                    }
                    if (npc.ai[1] >= 17)
                    {
                        npc.ai[1] = 0f;
                        npc.velocity.Y -= jumpSpeed;
                        npc.velocity.X = lungeForwardSpeed * (npc.Center.X - destination.X < 0).ToDirectionInt();
                        npc.spriteDirection = (npc.Center.X - destination.X > 0).ToDirectionInt();
                        npc.netSpam = 0;
                        npc.netUpdate = true;
                    }
                }
                else
                {
                    npc.knockBackResist = 0f;
                }
            }
            else
            {
				npc.knockBackResist = 0.5f;
                npc.DR_NERD(0f);
                float speed = CalamityWorld.downedPolterghast ? 17f : 14f;
                float inertia = CalamityWorld.downedPolterghast ? 20f : 24.5f;
                if (npc.Distance(player.Center) < 200f)
                    inertia *= 0.667f;
                npc.velocity = (npc.velocity * inertia + npc.DirectionTo(player.Center) * speed) / (inertia + 1f);
                npc.spriteDirection = (npc.velocity.X < 0).ToDirectionInt();
                if (npc.Distance(player.Center) < player.Size.Length())
                {
                    Flying = false;
                    npc.netSpam = 0;
                    npc.netUpdate = true;
                }
            }
        }
        public override void FindFrame(int frameHeight)
        {
            if (!Flying)
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
        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            CalamityGlobalNPC.DrawGlowmask(npc, spriteBatch, ModContent.GetTexture(Texture + "Glow"), true, Vector2.UnitY * 4f);
            CalamityGlobalNPC.DrawAfterimage(npc, spriteBatch, drawColor, Color.Transparent, directioning: true, invertedDirection: true);
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.85f);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/SulfurousSkaterGore"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/SulfurousSkaterGore2"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/SulfurousSkaterGore3"), npc.scale);
            }
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(npc, ModContent.ItemType<CorrodedFossil>(), 3 * (CalamityWorld.downedPolterghast ? 5 : 1), 1, 3);
            DropHelper.DropItemChance(npc, ModContent.ItemType<SulphurousGrabber>(), 20);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 120);
        }
    }
}
