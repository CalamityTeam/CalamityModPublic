using System;
using CalamityMod.BiomeManagers;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Sounds;
using CalamityMod.World;
using CalamityMod.BiomeManagers.BestiaryCategories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Astral
{
    public class Hiveling : ModNPC
    {
        public static Asset<Texture2D> glowmask;

        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                glowmask = ModContent.Request<Texture2D>("CalamityMod/NPCs/Astral/HivelingGlow", AssetRequestMode.AsyncLoad);
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers();
            value.Position.Y -= 8;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.width = 50;
            NPC.height = 40;
            NPC.aiStyle = -1;
            NPC.damage = 30;
            NPC.defense = 0;
            NPC.lifeMax = 150;
            NPC.DeathSound = CommonCalamitySounds.AstralNPCDeathSound;
            NPC.knockBackResist = 0.5f;
            NPC.noGravity = true;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            if (DownedBossSystem.downedAstrumAureus)
            {
                NPC.damage = 50;
                NPC.defense = 8;
                NPC.knockBackResist = 0.4f;
                NPC.lifeMax = 220;
            }
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = false;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<AstralUnderground>().Type };

            // Scale stats in Expert and Master
            CalamityGlobalNPC.AdjustExpertModeStatScaling(NPC);
            CalamityGlobalNPC.AdjustMasterModeStatScaling(NPC);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Hiveling")
            });
        }

        public override void AI()
        {
            if (NPC.ai[1] == 0f)
            {
                NPC.velocity *= 0.97f;

                NPC.TargetClosest(false);
                if (Main.player[NPC.target].dead)
                {
                    NPC.TargetClosest(false);
                }
                Player targ = Main.player[NPC.target];

                if (Collision.CanHit(NPC.position, NPC.width, NPC.height, targ.position, targ.width, targ.height) || Vector2.Distance(NPC.Center, targ.MountedCenter) < 320f)
                {
                    NPC.ai[1] = 1f;
                }
            }
            else
            {
                CalamityGlobalNPC.DoFlyingAI(NPC, (CalamityWorld.death ? 5f : CalamityWorld.revenge ? 4f : 3f), (CalamityWorld.death ? 0.08f : CalamityWorld.revenge ? 0.065f : 0.05f), 200f);
                Player myTarget = Main.player[NPC.target];
                Vector2 toTarget = myTarget.Center - NPC.Center;
                if (!myTarget.dead && myTarget.active)
                {
                    NPC.spriteDirection = NPC.direction = (toTarget.X > 0).ToDirectionInt();
                }
                else
                {
                    NPC.spriteDirection = NPC.direction = (NPC.velocity.X > 0).ToDirectionInt();
                }
                if (NPC.spriteDirection == 1)
                    NPC.rotation += MathHelper.Pi;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                NPC.frameCounter += 2;
            }
            else
            {
                NPC.frameCounter += 0.05f + NPC.velocity.Length() * 0.667f;
            }
            if (NPC.frameCounter >= 8)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > NPC.height * 2)
                {
                    NPC.frame.Y = 0;
                }
            }

            //DO DUST
            Dust d = CalamityGlobalNPC.SpawnDustOnNPC(NPC, 30, frameHeight, ModContent.DustType<AstralOrange>(), new Rectangle(16, 8, 6, 6), Vector2.Zero, 0.3f, true);
            if (d != null)
            {
                d.customData = 0.04f;
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //draw glowmask
            spriteBatch.Draw(glowmask.Value, NPC.Center - screenPos + new Vector2(0, 12), NPC.frame, Color.White * 0.6f, NPC.rotation, new Vector2(15, 10), 1f, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.soundDelay == 0)
            {
                NPC.soundDelay = 15;
                SoundEngine.PlaySound(CommonCalamitySounds.AstralNPCHitSound, NPC.Center);
            }

            CalamityGlobalNPC.DoHitDust(NPC, hit.HitDirection, (Main.rand.Next(0, Math.Max(0, NPC.life)) == 0) ? 5 : ModContent.DustType<AstralEnemy>(), 1f, 3, 20);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return 0f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 25, true);
        }
    }
}
