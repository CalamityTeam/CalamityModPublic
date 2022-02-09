using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.AquaticScourge
{
	public class AquaticScourgeTail : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aquatic Scourge");
        }

        public override void SetDefaults()
        {
			npc.GetNPCDamage();
			npc.width = 32;
            npc.height = 32;
            npc.defense = 25;
			npc.DR_NERD(0.15f);
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.alpha = 255;
            npc.LifeMaxNERB(77000, 92000, 1000000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.netAlways = true;
            npc.dontCountMe = true;
            npc.chaseable = false;
            npc.canGhostHeal = false;

            if (CalamityWorld.malice || BossRushEvent.BossRushActive)
                npc.scale = 1.25f;
            else if (CalamityWorld.death)
                npc.scale = 1.2f;
            else if (CalamityWorld.revenge)
                npc.scale = 1.15f;
            else if (Main.expertMode)
                npc.scale = 1.1f;

            npc.Calamity().VulnerableToHeat = false;
			npc.Calamity().VulnerableToSickness = false;
			npc.Calamity().VulnerableToElectricity = true;
			npc.Calamity().VulnerableToWater = false;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(npc.chaseable);
			for (int i = 0; i < 4; i++)
				writer.Write(npc.Calamity().newAI[i]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			npc.chaseable = reader.ReadBoolean();
			for (int i = 0; i < 4; i++)
				npc.Calamity().newAI[i] = reader.ReadSingle();
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void AI()
        {
			CalamityAI.AquaticScourgeAI(npc, mod, false);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			Texture2D texture2D15 = Main.npcTexture[npc.type];
			Vector2 vector11 = new Vector2(Main.npcTexture[npc.type].Width / 2, Main.npcTexture[npc.type].Height / 2);

			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2(texture2D15.Width, texture2D15.Height) * npc.scale / 2f;
			vector43 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
			Color color = npc.GetAlpha(lightColor);

			if (Main.npc[(int)npc.ai[2]].Calamity().newAI[3] > 480f && (CalamityWorld.revenge || BossRushEvent.BossRushActive))
				color = Color.Lerp(color, Color.SandyBrown, MathHelper.Clamp((Main.npc[(int)npc.ai[2]].Calamity().newAI[3] - 480f) / 180f, 0f, 1f));

			spriteBatch.Draw(texture2D15, vector43, npc.frame, color, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			return false;
		}

		public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.minion)
            {
                return npc.Calamity().newAI[0] == 1f;
            }
            return null;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreNPCLoot()
        {
            return false;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * npc.GetExpertDamageMultiplier());
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AquaticScourgeGores/ASTail"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AquaticScourgeGores/ASTail2"), 1f);
            }
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<Irradiated>(), 120, true);
        }
    }
}
