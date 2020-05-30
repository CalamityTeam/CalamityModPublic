using CalamityMod.Dusts;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.AstrumDeus
{
    public class AstrumDeusProbe2 : ModNPC
    {
        public int timer = 0;
        public bool start = true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astrum Deus Probe");
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            aiType = -1;
            npc.width = 30;
            npc.height = 30;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.chaseable = false;
            npc.dontTakeDamage = true;
            npc.damage = 0;
            npc.lifeMax = 100;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
        }

        public override bool PreAI()
        {
            bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;
            if (start)
            {
                for (int num621 = 0; num621 < 5; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 182, 0f, 0f, 100, default, 2f);
                }
                npc.ai[1] = npc.ai[0];
                start = false;
            }
            npc.TargetClosest(true);
            Vector2 direction = Main.player[npc.target].Center - npc.Center;
            direction.Normalize();
            direction *= 40f;
            npc.rotation = direction.ToRotation();
            npc.localAI[0] += 1f;
            if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[0] >= 360f)
            {
                npc.localAI[0] = 0f;
                int seeProjFileforDmg = 0;
                Projectile.NewProjectile(npc.Center.X, npc.Center.Y, direction.X, direction.Y, ModContent.ProjectileType<DeusMine>(), seeProjFileforDmg, 0f, Main.myPlayer, 0f, 0f);
            }
            bool anySmallDeusHeads = NPC.AnyNPCs(ModContent.NPCType<AstrumDeusHead>());
            if (CalamityGlobalNPC.astrumDeusHeadMain < 0 || !Main.npc[CalamityGlobalNPC.astrumDeusHeadMain].active)
            {
                npc.active = false;
                npc.netUpdate = true;
                return false;
            }
            Player player = Main.player[npc.target];
            int npcType = anySmallDeusHeads ? ModContent.NPCType<AstrumDeusHead>() : ModContent.NPCType<AstrumDeusHeadSpectral>();
            NPC parent = Main.npc[NPC.FindFirstNPC(npcType)];
            double deg = (double)npc.ai[1];
            double rad = deg * (Math.PI / 180);
            double dist = 150;
            npc.position.X = parent.Center.X - (int)(Math.Cos(rad) * dist) - npc.width / 2;
            npc.position.Y = parent.Center.Y - (int)(Math.Sin(rad) * dist) - npc.height / 2;
            npc.ai[1] += 2f;
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                npc.position.X = npc.position.X + (float)(npc.width / 2);
                npc.position.Y = npc.position.Y + (float)(npc.height / 2);
                npc.width = 50;
                npc.height = 50;
                npc.position.X = npc.position.X - (float)(npc.width / 2);
                npc.position.Y = npc.position.Y - (float)(npc.height / 2);
                for (int num621 = 0; num621 < 5; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 10; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			Texture2D texture2D15 = Main.npcTexture[npc.type];
			Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / 2));
			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height)) * npc.scale / 2f;
			vector43 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);

			spriteBatch.Draw(texture2D15, vector43, npc.frame, npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			texture2D15 = ModContent.GetTexture("CalamityMod/NPCs/Astral/AstralProbeGlow");

			spriteBatch.Draw(texture2D15, vector43, npc.frame, Color.White * 0.6f, npc.rotation, vector11, npc.scale, npc.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

			return false;
		}
	}
}
