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
        public Player Target => Main.player[npc.target];
        public ref float JumpTimer => ref npc.ai[0];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sulphurous Skater");
            Main.npcFrameCount[npc.type] = 5;
            NPCID.Sets.TrailingMode[npc.type] = 1;
            NPCID.Sets.TrailCacheLength[npc.type] = 6;
        }

        public override void SendExtraAI(BinaryWriter writer) => writer.Write(Flying);

        public override void ReceiveExtraAI(BinaryReader reader) => Flying = reader.ReadBoolean();

        public override void SetDefaults()
        {
            npc.width = 48;
            npc.height = 48;

            npc.damage = 48;
            npc.lifeMax = 280;
            npc.defense = 3;

            if (CalamityWorld.downedPolterghast)
            {
                npc.damage = 85;
                npc.lifeMax = 3850;
                npc.defense = 15;
            }

            npc.knockBackResist = 0.8f;
            npc.value = Item.buyPrice(0, 0, 5, 25);
            npc.lavaImmune = false;
            npc.noGravity = true;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            banner = npc.type;
            bannerItem = ModContent.ItemType<SulfurousSkaterBanner>();

            npc.aiStyle = aiType = -1;
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToSickness = false;
            npc.Calamity().VulnerableToElectricity = true;
            npc.Calamity().VulnerableToWater = false;
        }

        public override void AI()
        {
            npc.TargetClosest(false);
            if (!Flying)
                JumpToDestination();
            else
                DoFlyMovement();
        }

        public void JumpToDestination()
        {
            npc.knockBackResist = 0.8f;
            npc.DR_NERD(0.35f);
            npc.noGravity = false;
            Projectile closestBubble = SearchForNearestBubble(out float distanceToBubbele);

            Vector2 destination = Target.Center;

            // Jump towards any nearby bubbles if they exist.
            if (closestBubble != null)
                destination = closestBubble.Center;

            // Stay on water instead of falling into it
            if (npc.wet && npc.velocity.Y >= 0f)
                npc.velocity.Y = -3f;

            // If close to the bubble, try to fall onto it.
            if (closestBubble != null && distanceToBubbele < 200f)
            {
                npc.velocity.Y += 0.2f;

                if (closestBubble.Hitbox.Intersects(npc.Hitbox))
                {
                    Flying = true;
                    npc.netSpam = 0;
                    npc.netUpdate = true;
                    closestBubble.Kill();
                }
            }

            // Wait for a small amount of time and jump if there is little motion.
            if (npc.velocity.Y == 0f || npc.wet)
            {
                npc.TargetClosest(false);

                // Rapidly zero out horizontal movement.
                npc.velocity.X *= 0.85f;

                JumpTimer++;
                float lungeForwardSpeed = 15f;
                float jumpSpeed = 4f;
                if (Collision.CanHit(npc.Center, 1, 1, Target.Center, 1, 1))
                    lungeForwardSpeed *= 1.2f;

                // Jump after a short amount of time.
                if (JumpTimer >= 17)
                {
                    JumpTimer = 0f;
                    npc.velocity.Y -= jumpSpeed;
                    npc.velocity.X = lungeForwardSpeed * (npc.Center.X - destination.X < 0).ToDirectionInt();
                    npc.spriteDirection = (npc.Center.X - destination.X > 0).ToDirectionInt();
                    npc.netSpam = 0;
                    npc.netUpdate = true;
                }
            }
            else
                npc.knockBackResist = 0f;
        }

        public void DoFlyMovement()
        {
            npc.knockBackResist = 0.5f;
            npc.DR_NERD(0f);

            float flySpeed = CalamityWorld.downedPolterghast ? 17f : 14f;
            float flyInertia = CalamityWorld.downedPolterghast ? 20f : 24.5f;

            // Fly more sharply if close to the target.
            if (npc.WithinRange(Target.Center, 200f))
                flyInertia *= 0.667f;
            npc.velocity = (npc.velocity * flyInertia + npc.SafeDirectionTo(Target.Center, Vector2.UnitY) * flySpeed) / (flyInertia + 1f);
            npc.spriteDirection = (npc.velocity.X < 0).ToDirectionInt();

            // Have the bubble pop and stop flying if within the circular hitbox area of the player.
            if (npc.WithinRange(Target.Center, Target.Size.Length()))
            {
                Flying = false;
                npc.netSpam = 0;
                npc.netUpdate = true;
            }
        }

        public Projectile SearchForNearestBubble(out float distanceToBubble)
        {
            int bubbleType = ModContent.ProjectileType<SulphuricAcidBubble>();
            float minimumDistance = 2400f;
            Projectile closestBubble = null;

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].type != bubbleType || !Main.projectile[i].active)
                    continue;

                if (Math.Abs(npc.Center.X - Main.projectile[i].Center.X) >= minimumDistance ||
                    Main.projectile[i].Center.Y <= npc.Bottom.Y ||
                    !Collision.CanHit(npc.position, npc.width, npc.height, Main.projectile[i].position, Main.projectile[i].width, Main.projectile[i].height))
                {
                    continue;
                }

                minimumDistance = npc.Distance(Main.projectile[i].Center);
                closestBubble = Main.projectile[i];
            }

            distanceToBubble = minimumDistance;
            return closestBubble;
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
                        npc.frame.Y = frameHeight;
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            CalamityGlobalNPC.DrawGlowmask(npc, spriteBatch, ModContent.GetTexture(Texture + "Glow"), true, Vector2.UnitY * 4f);
            CalamityGlobalNPC.DrawAfterimage(npc, spriteBatch, drawColor, Color.Transparent, directioning: true, invertedDirection: true);
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

        public override void OnHitPlayer(Player target, int damage, bool crit) => target.AddBuff(ModContent.BuffType<Irradiated>(), 120);
    }
}
