using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class PhantomSpiritL : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantom Spirit");
            Main.npcFrameCount[npc.type] = 5;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            npc.damage = 100;
            npc.width = 32;
            npc.height = 80;
            npc.scale = 1.2f;
            npc.defense = 30;
            npc.lifeMax = 3000;
            npc.knockBackResist = 0f;
            aiType = -1;
            npc.value = Item.buyPrice(0, 0, 60, 0);
            npc.HitSound = SoundID.NPCHit36;
            npc.DeathSound = SoundID.NPCDeath39;
            npc.noGravity = true;
            npc.noTileCollide = true;
            banner = ModContent.NPCType<PhantomSpirit>();
            bannerItem = ModContent.ItemType<PhantomSpiritBanner>();
			npc.Calamity().VulnerableToSickness = false;
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
            float speed = (CalamityWorld.death || BossRushEvent.BossRushActive) ? 16f : 12f;
            CalamityAI.DungeonSpiritAI(npc, mod, speed, -MathHelper.PiOver2);
            int num822 = Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.Phantoplasm, 0f, 0f, 0, default, 1f);
            Dust dust = Main.dust[num822];
            dust.velocity *= 0.1f;
            dust.scale = 1.3f;
            dust.noGravity = true;
            Vector2 vector17 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
            float num147 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector17.X;
            float num148 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector17.Y;
            float num149 = (float)Math.Sqrt((double)(num147 * num147 + num148 * num148));
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
                if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] == 20f)
                {
                    float num151 = 10f;
					int type = ModContent.ProjectileType<PhantomGhostShot>();
					int damage = npc.GetProjectileDamage(type);
					num149 = num151 / num149;
                    num147 *= num149;
                    num148 *= num149;
                    Projectile.NewProjectile(vector17.X, vector17.Y, num147, num148, type, damage, 0f, Main.myPlayer, 0f, 0f);
                }
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.Phantoplasm, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int num288 = 0; num288 < 50; num288++)
                {
                    int num289 = Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.Phantoplasm, npc.velocity.X, npc.velocity.Y, 0, default, 1f);
                    Dust dust = Main.dust[num289];
                    dust.velocity *= 2f;
                    dust.noGravity = true;
                    dust.scale = 1.4f;
                }
            }
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return new Color(200, 200, 200, 0);
        }

        public override void NPCLoot()
        {
			DropHelper.DropItem(npc, ModContent.ItemType<Phantoplasm>(), 2, 4);
        }
    }
}
