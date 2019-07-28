using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class PhantomSpiritL : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Phantom Spirit");
			Main.npcFrameCount[npc.type] = 4;
		}

		public override void SetDefaults()
		{
			npc.aiStyle = -1;
			npc.damage = 100;
			npc.width = 32; //324
			npc.height = 32; //216
            npc.scale = 1.2f;
			npc.defense = 100;
			npc.lifeMax = 9000;
			npc.knockBackResist = 0f;
			aiType = -1;
			npc.value = Item.buyPrice(0, 0, 60, 0);
			npc.HitSound = SoundID.NPCHit36;
			npc.DeathSound = SoundID.NPCDeath39;
			npc.noGravity = true;
			npc.noTileCollide = true;
			banner = mod.NPCType("PhantomSpirit");
			bannerItem = mod.ItemType("PhantomSpiritBanner");
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
			npc.TargetClosest(true);
			Vector2 vector102 = new Vector2(npc.Center.X, npc.Center.Y);
			float num818 = Main.player[npc.target].Center.X - vector102.X;
			float num819 = Main.player[npc.target].Center.Y - vector102.Y;
			float num820 = (float)Math.Sqrt((double)(num818 * num818 + num819 * num819));
			float num821 = 12f;
			num820 = num821 / num820;
			num818 *= num820;
			num819 *= num820;
			npc.velocity.X = (npc.velocity.X * 100f + num818) / 101f;
			npc.velocity.Y = (npc.velocity.Y * 100f + num819) / 101f;
			npc.rotation = (float)Math.Atan2((double)num819, (double)num818) - 1.57f;
			int num822 = Dust.NewDust(npc.position, npc.width, npc.height, 60, 0f, 0f, 0, default(Color), 1f);
			Dust dust = Main.dust[num822];
			dust.velocity *= 0.1f;
			Main.dust[num822].scale = 1.3f;
			Main.dust[num822].noGravity = true;
            Vector2 vector17 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
            float num147 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector17.X;
            float num148 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector17.Y;
            float num149 = (float)Math.Sqrt((double)(num147 * num147 + num148 * num148));
            num149 = 4f / num149;
            num147 *= num149;
            num148 *= num149;
            vector17 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
            num147 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector17.X;
            num148 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector17.Y;
            num149 = (float)Math.Sqrt((double)(num147 * num147 + num148 * num148));
            if (num149 > 800f)
            {
                npc.ai[2] = 0f;
                npc.ai[3] = 0f;
                return;
            }
            npc.ai[2] += 1f;
            if (npc.ai[3] == 0f)
            {
                if (npc.ai[2] > 120f)
                {
                    npc.ai[2] = 0f;
                    npc.ai[3] = 1f;
                    npc.netUpdate = true;
                    return;
                }
            }
            else
            {
                if (npc.ai[2] > 40f)
                {
                    npc.ai[3] = 0f;
                }
                if (Main.netMode != 1 && npc.ai[2] == 20f)
                {
                    float num151 = 5f;
                    int num152 = 60;
                    int num153 = mod.ProjectileType("PhantomGhostShot");
                    num149 = num151 / num149;
                    num147 *= num149;
                    num148 *= num149;
                    int num154 = Projectile.NewProjectile(vector17.X, vector17.Y, num147, num148, num153, num152, 0f, Main.myPlayer, 0f, 0f);
                }
            }
		}

		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			if (CalamityWorld.revenge)
			{
				player.AddBuff(mod.BuffType("MarkedforDeath"), 180);
			}
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 60, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				for (int num288 = 0; num288 < 50; num288++)
				{
					int num289 = Dust.NewDust(npc.position, npc.width, npc.height, 60, npc.velocity.X, npc.velocity.Y, 0, default(Color), 1f);
					Dust dust = Main.dust[num289];
					dust.velocity *= 2f;
					Main.dust[num289].noGravity = true;
					Main.dust[num289].scale = 1.4f;
				}
			}
		}

		public override Color? GetAlpha(Color drawColor)
		{
			return new Color(200, 200, 200, 0);
		}

		public override void NPCLoot()
		{
			Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Phantoplasm"), Main.rand.Next(2, 5));
		}
	}
}
