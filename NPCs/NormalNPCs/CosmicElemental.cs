using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.NormalNPCs
{
    public class CosmicElemental : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmic Elemental");
            Main.npcFrameCount[npc.type] = 11;
        }

        public override void SetDefaults()
        {
            npc.npcSlots = 0.5f;
            npc.aiStyle = 91;
            npc.damage = 20;
            npc.width = npc.height = 30;
            npc.defense = 10;
            npc.lifeMax = 25;
            npc.knockBackResist = 0.5f;
            npc.value = Item.buyPrice(0, 0, 3, 0);
            npc.HitSound = SoundID.NPCHit7;
            npc.DeathSound = SoundID.NPCDeath6;
            banner = npc.type;
            bannerItem = ModContent.ItemType<CosmicElementalBanner>();
        }

		public override void FindFrame(int frameHeight)
		{
			npc.frameCounter++;
			if (npc.frameCounter > 6)
			{
				npc.frame.Y += frameHeight;
				npc.frameCounter = 0;
			}
			if (npc.ai[0] == -1f)
			{
				if (npc.frame.Y >= frameHeight * 11)
					npc.frame.Y = frameHeight * 10;
				else if (npc.frame.Y <= frameHeight * 5)
					npc.frame.Y = frameHeight * 6;
				npc.rotation += npc.velocity.X * 0.2f;
			}
			else
			{
				if (npc.frame.Y >= frameHeight * 6)
					npc.frame.Y = 0;
				npc.rotation = npc.velocity.X * 0.1f;
			}
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = Main.npcTexture[npc.type];
            int height = texture.Height / Main.npcFrameCount[npc.type];
            int width = texture.Width;
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == -1)
			{
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
            Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), npc.frame, npc.GetAlpha(drawColor), npc.rotation, new Vector2(width / 2f, height / 2f), npc.scale, spriteEffects, 0f);
            return false;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || spawnInfo.player.Calamity().ZoneAbyss || spawnInfo.player.Calamity().ZoneSunkenSea)
            {
                return 0f;
            }
            return SpawnCondition.Cavern.Chance * 0.01f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Confused, 180, true);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 70, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 70, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void NPCLoot()
        {
			DropHelper.DropItemChance(npc, ItemID.BoneSword, CalamityWorld.defiled ? DropHelper.DefiledDropRateInt : 25);
			DropHelper.DropItemChance(npc, ItemID.Starfury, CalamityWorld.defiled ? DropHelper.DefiledDropRateInt : 50);
			DropHelper.DropItemChance(npc, ItemID.EnchantedSword, CalamityWorld.defiled ? DropHelper.DefiledDropRateInt : 100);
			DropHelper.DropItemChance(npc, ItemID.Arkhalis, CalamityWorld.defiled ? DropHelper.DefiledDropRateInt : 200);
        }
    }
}
