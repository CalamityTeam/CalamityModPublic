using CalamityMod.Graphics.Primitives;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.SupremeCalamitas
{
    public class BrimstoneHeart : ModNPC
    {
        public int ChainHeartIndex => (int)NPC.ai[0];
        public List<Vector2> ChainEndpoints = new();
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            Main.npcFrameCount[NPC.type] = 6;
        }

        public override void SetDefaults()
        {
            NPC.damage = 50;
            NPC.width = 24;
            NPC.height = 24;
            NPC.defense = 0;
            NPC.lifeMax = 15000;
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.alpha = 255;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.canGhostHeal = false;
            NPC.hide = true;
            NPC.HitSound = SoundID.NPCHit13;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(ChainEndpoints.Count);
            for (int i = 0; i < ChainEndpoints.Count; i++)
                writer.WriteVector2(ChainEndpoints[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            ChainEndpoints.Clear();
            int endpointCount = reader.ReadInt32();
            for (int i = 0; i < endpointCount; i++)
                ChainEndpoints.Add(reader.ReadVector2());
        }

        public override void AI()
        {
            // Setting this in SetDefaults will disable expert mode scaling, so put it here instead
            NPC.damage = 0;

            if (CalamityGlobalNPC.SCal < 0 || !Main.npc[CalamityGlobalNPC.SCal].active)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.StrikeInstantKill();

                return;
            }

            NPC.alpha -= 42;
            if (NPC.alpha < 0)
                NPC.alpha = 0;
        }

        public override void OnKill()
        {
            int closestPlayer = Player.FindClosest(NPC.Center, 1, 1);
            if (Main.rand.NextBool(4) && Main.player[closestPlayer].statLife < Main.player[closestPlayer].statLifeMax2)
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.Heart);
        }

        public float PrimitiveWidthFunction(float completionRatio)
        {
            float widthInterpolant = Utils.GetLerpValue(0f, 0.16f, completionRatio, true) * Utils.GetLerpValue(1f, 0.84f, completionRatio, true);
            widthInterpolant = (float)Math.Pow(widthInterpolant, 8D);
            float baseWidth = MathHelper.Lerp(4f, 1f, widthInterpolant);
            float pulseWidth = MathHelper.Lerp(0f, 3.2f, (float)Math.Pow(Math.Sin(Main.GlobalTimeWrappedHourly * 2.6f + NPC.whoAmI * 1.3f + completionRatio), 16D));
            return baseWidth + pulseWidth;
        }

        public Color PrimitiveColorFunction(float completionRatio)
        {
            float colorInterpolant = MathHelper.SmoothStep(0f, 1f, Utils.GetLerpValue(0f, 0.34f, completionRatio, true) * Utils.GetLerpValue(1.07f, 0.66f, completionRatio, true));
            return Color.Lerp(Color.DarkRed * 0.7f, Color.Red, colorInterpolant) * 0.425f;
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.IsABestiaryIconDummy)
                NPC.Opacity = 1f;

            int frame = (int)Math.Round((float)Math.Pow(Math.Sin(Main.GlobalTimeWrappedHourly * 2.6f + NPC.whoAmI * 1.3f), 6D) * Main.npcFrameCount[NPC.type]);
            if (frame >= Main.npcFrameCount[NPC.type])
                frame = Main.npcFrameCount[NPC.type] - 1;
            NPC.frame.Y = frame * frameHeight;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
                return true;

            for (int i = 0; i < ChainEndpoints.Count; i++)
            {
                List<Vector2> points = new List<Vector2>()
                {
                    NPC.Center,
                    ChainEndpoints[i] + NPC.DirectionTo(ChainEndpoints[i]) * 25f
                };
                PrimitiveSet.Prepare(points, new(PrimitiveWidthFunction, PrimitiveColorFunction), 40);
            }

            return true;
        }

        public void TendrilDestructionEffects(int tendrilIndex)
        {
            for (int i = 0; i < 70; i++)
            {
                Vector2 dustSpawnPosition = Vector2.Lerp(NPC.Center, ChainEndpoints[tendrilIndex], i / 70f);
                Dust blood = Dust.NewDustDirect(dustSpawnPosition, 4, 4, DustID.Blood);
                blood.velocity = Main.rand.NextVector2Circular(3f, 3f);
                blood.scale = Main.rand.NextFloat(1f, 1.4f);
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    for (int i = 1; i <= 2; i++)
                    {
                        Vector2 heartGoreVelocity = new Vector2((i == 1).ToDirectionInt() * 3f, Main.rand.NextFloat(-2f, 0f));
                        Gore.NewGorePerfect(NPC.GetSource_Death(), NPC.Center, heartGoreVelocity, Mod.Find<ModGore>($"BrimstoneHeart_Gore{i}").Type, NPC.scale);
                    }
                }
                for (int i = 0; i < ChainEndpoints.Count; i++)
                    TendrilDestructionEffects(i);
            }
        }

        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index);
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (projectile.type == ModContent.ProjectileType<CelestusMiniScythe>())
                modifiers.SourceDamage *= 0.66f;
        }

        public override bool CheckActive() => false;
    }
}
