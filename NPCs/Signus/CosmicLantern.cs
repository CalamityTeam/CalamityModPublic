using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.Signus
{
    public class CosmicLantern : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmic Lantern");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            aiType = -1;
            npc.damage = 110;
            npc.width = 25;
            npc.height = 25;
            npc.defense = 50;
            npc.lifeMax = 25;
            npc.alpha = 255;
            npc.knockBackResist = 0.85f;
            npc.noGravity = true;
            npc.dontTakeDamage = true;
            npc.chaseable = false;
            npc.canGhostHeal = false;
            npc.noTileCollide = true;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.HitSound = SoundID.NPCHit53;
            npc.DeathSound = SoundID.NPCDeath44;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.dontTakeDamage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.dontTakeDamage = reader.ReadBoolean();
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
			if (CalamityGlobalNPC.signus < 0 || !Main.npc[CalamityGlobalNPC.signus].active)
			{
				npc.active = false;
				npc.netUpdate = true;
				return;
			}

			Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.75f, 0.75f, 0.75f);

			npc.alpha -= 3;
            if (npc.alpha < 0)
            {
                npc.alpha = 0;
                int num1262 = Dust.NewDust(npc.position, npc.width, npc.height, 204, 0f, 0f, 0, default, 0.25f);
                Main.dust[num1262].velocity *= 0.1f;
                Main.dust[num1262].noGravity = true;
            }

            npc.rotation = npc.velocity.X * 0.08f;
            npc.spriteDirection = (npc.direction > 0) ? 1 : -1;
            npc.TargetClosest(true);

			bool revenge = CalamityWorld.revenge;
			float playerDistNormMult = revenge ? 24f : 22f;
			if (CalamityWorld.bossRushActive)
				playerDistNormMult = 30f;

            Vector2 vector145 = new Vector2(npc.Center.X, npc.Center.Y);
            float playerDistX = Main.player[npc.target].Center.X - vector145.X;
            float playerDistY = Main.player[npc.target].Center.Y - vector145.Y;
            float playerDistMagnitude = (float)Math.Sqrt((double)(playerDistX * playerDistX + playerDistY * playerDistY));

            if (npc.localAI[0] < 85f)
            {
                playerDistNormMult = 0.1f;
                playerDistMagnitude = playerDistNormMult / playerDistMagnitude;
                playerDistX *= playerDistMagnitude;
                playerDistY *= playerDistMagnitude;
                npc.velocity = (npc.velocity * 100f + new Vector2(playerDistX, playerDistY)) / 101f;
                npc.localAI[0] += 1f;
                return;
            }

            npc.dontTakeDamage = false;
            npc.chaseable = true;

            playerDistMagnitude = playerDistNormMult / playerDistMagnitude;
            playerDistX *= playerDistMagnitude;
            playerDistY *= playerDistMagnitude;
            npc.velocity = (npc.velocity * 100f + new Vector2(playerDistX, playerDistY)) / 101f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            if (CalamityWorld.revenge)
            {
                player.AddBuff(ModContent.BuffType<MarkedforDeath>(), 180);
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 0;
            return npc.alpha == 0;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 204, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
