using CalamityMod.Events;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.Leviathan
{
	public class Parasea : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Parasea");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            aiType = -1;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.canGhostHeal = false;
			npc.GetNPCDamage();
			npc.width = npc.height = 30;
            npc.defense = 8;
            npc.lifeMax = 650;
            if (BossRushEvent.BossRushActive)
            {
                npc.lifeMax = 5000;
            }
            npc.knockBackResist = 0f;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            banner = npc.type;
            bannerItem = ModContent.ItemType<ParaseaBanner>();
        }

        public override void AI()
        {
            bool revenge = CalamityWorld.revenge;
            float speed = revenge ? 16f : 13f;
            if (BossRushEvent.BossRushActive || CalamityWorld.malice)
                speed = 24f;
            CalamityAI.DungeonSpiritAI(npc, mod, speed, MathHelper.Pi);
        }

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.playerSafe || spawnInfo.player.Calamity().ZoneSulphur || (!NPC.downedPlantBoss && !CalamityWorld.downedCalamitas))
			{
				return 0f;
			}
			return SpawnCondition.OceanMonster.Chance * 0.06f;
		}

		public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = Main.npcTexture[npc.type];
            int height = texture.Height / Main.npcFrameCount[npc.type];
            int width = texture.Width;
			SpriteEffects spriteEffects = SpriteEffects.FlipHorizontally;
			if (npc.spriteDirection == -1)
				spriteEffects = SpriteEffects.None;
            Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), npc.frame, npc.GetAlpha(drawColor), npc.rotation, new Vector2((float)width / 2f, (float)height / 2f), npc.scale, spriteEffects, 0f);
            return false;
        }

		public override bool PreNPCLoot()
		{
			if (!CalamityWorld.malice && !CalamityWorld.revenge)
			{
				int closestPlayer = Player.FindClosest(npc.Center, 1, 1);
				if (Main.rand.Next(8) == 0 && Main.player[closestPlayer].statLife < Main.player[closestPlayer].statLifeMax2)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Heart);
			}

			return false;
		}

		public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Bleeding, 60, true);
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
            }
        }
    }
}
