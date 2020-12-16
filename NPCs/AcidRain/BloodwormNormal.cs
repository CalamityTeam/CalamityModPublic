using CalamityMod.Items.SummonItems;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.AcidRain
{
    public class BloodwormNormal : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloodworm");
            Main.npcFrameCount[npc.type] = 7;
            Main.npcCatchable[npc.type] = true;
        }

        public override void SetDefaults()
        {
            npc.damage = 0;
            npc.width = 36;
            npc.height = 16;
            npc.defense = 0;
            npc.lifeMax = 5;
            npc.knockBackResist = 0f;
            npc.lavaImmune = false;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.catchItem = (short)ModContent.ItemType<BloodwormItem>();
            npc.dontTakeDamageFromHostiles = true;
			npc.rarity = 4;
        }

        public override void AI()
        {
			if (npc.collideY)
            {
                if (npc.ai[0] == 0f)
                {
                    npc.ai[0] = Main.rand.NextBool(2).ToDirectionInt();
                    npc.netUpdate = true;
                }
                if (npc.collideX)
                {
                    npc.ai[0] *= -1;
                }
            }
            float xSpeed = 3f;
            npc.velocity.X = xSpeed * npc.ai[0];
            npc.spriteDirection = (int)npc.ai[0];
			bool flee = false;
			for (int i = 0; i < Main.player.Length; i++)
			{
				Player player = Main.player[i];
				if (player.active && !player.dead && Vector2.Distance(player.Center, npc.Center) <= 220f)
				{
					flee = true;
					break;
				}
			}
			int timeBeforeFlee = 60;
			if (flee && npc.ai[1] < timeBeforeFlee)
			{
				npc.ai[1] += 1f;
			}
			if (npc.ai[1] == timeBeforeFlee && Main.netMode != NetmodeID.MultiplayerClient)
			{
				npc.position.Y += 16f;
				npc.Transform(ModContent.NPCType<BloodwormFleeing>());
				npc.netUpdate = true;
				return;
			}
		}

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter >= 6)
            {
                npc.frameCounter = 0;
                npc.frame.Y += frameHeight;
                if (npc.frame.Y >= Main.npcFrameCount[npc.type] * frameHeight)
                {
                    npc.frame.Y = 0;
                }
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return (spawnInfo.player.Calamity().ZoneSulphur && CalamityWorld.encounteredOldDuke && !CalamityWorld.rainingAcid) ? SpawnCondition.WormCritter.Chance * 2.569f : 0f;
        }

        public override void OnCatchNPC(Player player, Item item)
        {
            try
            {

            }
            catch
            {
                return;
            }
        }
    }
}
