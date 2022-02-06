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
        public PrimitiveTrail ChainDrawer = null;
        public int ChainHeartIndex => (int)npc.ai[0];
        public List<Vector2> ChainEndpoints = new List<Vector2>();
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimstone Heart");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            npc.damage = 0;
            npc.width = 24;
            npc.height = 24;
            npc.defense = 0;
			npc.LifeMaxNERB(24550, 29440, 10000);
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.alpha = 255;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.canGhostHeal = false;
            npc.hide = true;
            npc.HitSound = SoundID.NPCHit13;
            npc.DeathSound = SoundID.NPCDeath1;
			npc.Calamity().VulnerableToHeat = false;
			npc.Calamity().VulnerableToCold = true;
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
            if (CalamityGlobalNPC.SCal < 0 || !Main.npc[CalamityGlobalNPC.SCal].active)
            {
                npc.active = false;
                npc.netUpdate = true;
                return;
            }

            npc.alpha -= 42;
            if (npc.alpha < 0)
                npc.alpha = 0;
        }

		public override void NPCLoot()
		{
			if (!CalamityWorld.revenge)
			{
				int closestPlayer = Player.FindClosest(npc.Center, 1, 1);
				if (Main.rand.Next(4) == 0 && Main.player[closestPlayer].statLife < Main.player[closestPlayer].statLifeMax2)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Heart);
			}
		}

		public float PrimitiveWidthFunction(float completionRatio)
        {
            float widthInterpolant = Utils.InverseLerp(0f, 0.16f, completionRatio, true) * Utils.InverseLerp(1f, 0.84f, completionRatio, true);
            widthInterpolant = (float)Math.Pow(widthInterpolant, 8D);
            float baseWidth = MathHelper.Lerp(4f, 1f, widthInterpolant);
            float pulseWidth = MathHelper.Lerp(0f, 3.2f, (float)Math.Pow(Math.Sin(Main.GlobalTime * 2.6f + npc.whoAmI * 1.3f + completionRatio), 16D));
            return baseWidth + pulseWidth;
        }

        public Color PrimitiveColorFunction(float completionRatio)
        {
            float colorInterpolant = MathHelper.SmoothStep(0f, 1f, Utils.InverseLerp(0f, 0.34f, completionRatio, true) * Utils.InverseLerp(1.07f, 0.66f, completionRatio, true));
            return Color.Lerp(Color.DarkRed * 0.7f, Color.Red, colorInterpolant) * 0.425f;
        }

        public override void FindFrame(int frameHeight)
        {
            int frame = (int)Math.Round((float)Math.Pow(Math.Sin(Main.GlobalTime * 2.6f + npc.whoAmI * 1.3f), 6D) * Main.npcFrameCount[npc.type]);
            if (frame >= Main.npcFrameCount[npc.type])
                frame = Main.npcFrameCount[npc.type] - 1;
            npc.frame.Y = frame * frameHeight;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            if (ChainDrawer is null)
                ChainDrawer = new PrimitiveTrail(PrimitiveWidthFunction, PrimitiveColorFunction);
            for (int i = 0; i < ChainEndpoints.Count; i++)
            {
                List<Vector2> points = new List<Vector2>()
                {
                    npc.Center,
                    ChainEndpoints[i] + npc.DirectionTo(ChainEndpoints[i]) * 25f
                };
                ChainDrawer.Draw(points, -Main.screenPosition, 40);
            }

            return true;
        }

        public void TendrilDestructionEffects(int tendrilIndex)
        {
            for (int i = 0; i < 70; i++)
            {
                Vector2 dustSpawnPosition = Vector2.Lerp(npc.Center, ChainEndpoints[tendrilIndex], i / 70f);
                Dust blood = Dust.NewDustDirect(dustSpawnPosition, 4, 4, DustID.Blood);
                blood.velocity = Main.rand.NextVector2Circular(3f, 3f);
                blood.scale = Main.rand.NextFloat(1f, 1.4f);
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int i = 1; i <= 2; i++)
                {
                    Vector2 heartGoreVelocity = new Vector2((i == 1).ToDirectionInt() * 3f, Main.rand.NextFloat(-2f, 0f));
                    Gore.NewGorePerfect(npc.Center, heartGoreVelocity, mod.GetGoreSlot($"Gores/SupremeCalamitas/BrimstoneHeart_Gore{i}"), npc.scale);
                }
                for (int i = 0; i < ChainEndpoints.Count; i++)
                    TendrilDestructionEffects(i);
            }
        }

        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index);
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (projectile.type == ModContent.ProjectileType<Celestus2>())
                damage = (int)(damage * 0.66);
        }

		public override bool CheckActive() => false;
	}
}
