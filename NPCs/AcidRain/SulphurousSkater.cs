using CalamityMod.BiomeManagers;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.Enemy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.AcidRain
{
    public class SulphurousSkater : ModNPC
    {
        public bool Flying = false;
        
        public Player Target => Main.player[NPC.target];

        public ref float JumpTimer => ref NPC.ai[0];

        public const int JumpDelay = 64;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.TrailCacheLength[NPC.type] = 6;
        }

        public override void SetDefaults()
        {
            NPC.width = 48;
            NPC.height = 48;

            NPC.damage = 48;
            NPC.lifeMax = 280;
            NPC.defense = 3;

            if (DownedBossSystem.downedPolterghast)
            {
                NPC.damage = 85;
                NPC.lifeMax = 3850;
                NPC.defense = 15;
            }

            NPC.knockBackResist = 0.8f;
            NPC.value = Item.buyPrice(0, 0, 5, 25);
            NPC.lavaImmune = false;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<SulphurousSkaterBanner>();

            NPC.aiStyle = AIType = -1;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<AcidRainBiome>().Type };
        }

        public override void SendExtraAI(BinaryWriter writer) => writer.Write(Flying);

        public override void ReceiveExtraAI(BinaryReader reader) => Flying = reader.ReadBoolean();

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.SulphurousSkater")
            });
        }

        public override void AI()
        {
            NPC.TargetClosest(false);
            if (!Flying)
                JumpToDestination();
            else
                DoFlyMovement();
        }

        public void JumpToDestination()
        {
            NPC.knockBackResist = 0.8f;
            NPC.DR_NERD(0.35f);
            NPC.noGravity = false;
            Projectile closestBubble = SearchForNearestBubble(out float distanceToBubbele);

            Vector2 destination = Target.Center;

            // Jump towards any nearby bubbles if they exist.
            if (closestBubble != null)
                destination = closestBubble.Center;

            // Stay on water instead of falling into it
            if (NPC.wet && NPC.velocity.Y >= 0f)
                NPC.velocity.Y = -3f;

            // If close to the bubble, try to fall onto it.
            if (closestBubble != null && distanceToBubbele < 200f)
            {
                NPC.velocity.Y += 0.2f;

                if (closestBubble.Hitbox.Intersects(NPC.Hitbox))
                {
                    Flying = true;
                    NPC.netSpam = 0;
                    NPC.netUpdate = true;
                    closestBubble.Kill();
                }
            }

            // Wait for a small amount of time and jump if there is little motion.
            if (NPC.velocity.Y == 0f || NPC.wet)
            {
                NPC.TargetClosest(false);

                // Rapidly zero out horizontal movement.
                NPC.velocity.X *= 0.85f;

                JumpTimer++;
                float lungeForwardSpeed = 12f;
                float jumpSpeed = 4f;
                if (Collision.CanHit(NPC.Center, 1, 1, Target.Center, 1, 1))
                    lungeForwardSpeed *= 1.2f;

                // Jump after a short amount of time.
                if (JumpTimer >= JumpDelay)
                {
                    JumpTimer = 0f;
                    NPC.velocity.Y -= jumpSpeed;
                    NPC.velocity.X = lungeForwardSpeed * (NPC.Center.X - destination.X < 0).ToDirectionInt();
                    NPC.spriteDirection = (NPC.Center.X - destination.X > 0).ToDirectionInt();
                    NPC.netSpam = 0;
                    NPC.netUpdate = true;
                }
            }
            else
                NPC.knockBackResist = 0f;
        }

        public void DoFlyMovement()
        {
            NPC.knockBackResist = 0.5f;
            NPC.DR_NERD(0f);

            float flySpeed = DownedBossSystem.downedPolterghast ? 17f : 14f;
            float flyInertia = DownedBossSystem.downedPolterghast ? 20f : 24.5f;

            // Fly more sharply if close to the target.
            if (NPC.WithinRange(Target.Center, 200f))
                flyInertia *= 0.667f;
            NPC.velocity = (NPC.velocity * flyInertia + NPC.SafeDirectionTo(Target.Center, Vector2.UnitY) * flySpeed) / (flyInertia + 1f);
            NPC.spriteDirection = (NPC.velocity.X < 0).ToDirectionInt();

            // Have the bubble pop and stop flying if within the circular hitbox area of the player.
            if (NPC.WithinRange(Target.Center, Target.Size.Length()))
            {
                Flying = false;
                NPC.netSpam = 0;
                NPC.netUpdate = true;
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

                if (Math.Abs(NPC.Center.X - Main.projectile[i].Center.X) >= minimumDistance ||
                    Main.projectile[i].Center.Y <= NPC.Bottom.Y ||
                    !Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.projectile[i].position, Main.projectile[i].width, Main.projectile[i].height))
                {
                    continue;
                }

                minimumDistance = NPC.Distance(Main.projectile[i].Center);
                closestBubble = Main.projectile[i];
            }

            distanceToBubble = minimumDistance;
            return closestBubble;
        }

        public override void FindFrame(int frameHeight)
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                SpriteDirection = 1
            };
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            if (!Flying && !NPC.IsABestiaryIconDummy)
                NPC.frame.Y = 0;
            else
            {
                NPC.frameCounter++;
                if (NPC.frameCounter >= 4)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y >= Main.npcFrameCount[NPC.type] * frameHeight)
                        NPC.frame.Y = frameHeight;
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!NPC.IsABestiaryIconDummy)
            {
                CalamityGlobalNPC.DrawAfterimage(NPC, spriteBatch, drawColor, Color.Transparent, directioning: true, invertedDirection: true);
                CalamityGlobalNPC.DrawGlowmask(NPC, spriteBatch, ModContent.Request<Texture2D>(Texture + "Glow").Value, true);
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.SulfurousSeaAcid, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("SulfurousSkaterGore").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("SulfurousSkaterGore2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("SulfurousSkaterGore3").Type, NPC.scale);
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ModContent.ItemType<SulphurousGrabber>(), 20);
            LeadingConditionRule postPolter = npcLoot.DefineConditionalDropSet(() => DownedBossSystem.downedPolterghast);
            postPolter.Add(ModContent.ItemType<CorrodedFossil>(), 15, 1, 3, !DownedBossSystem.downedPolterghast);
            postPolter.AddFail(ModContent.ItemType<CorrodedFossil>(), 3, 1, 3, DownedBossSystem.downedPolterghast);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<Irradiated>(), 120);
        }
    }
}
