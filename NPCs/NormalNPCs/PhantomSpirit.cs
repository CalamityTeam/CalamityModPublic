using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.NormalNPCs
{
    public class PhantomSpirit : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantom Spirit");
            Main.npcFrameCount[npc.type] = 3;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            npc.damage = 70;
            npc.width = 16;
            npc.height = 16;
            npc.defense = 10;
            npc.lifeMax = 1000;
            npc.knockBackResist = 0.1f;
            animationType = 288;
            aiType = -1;
            npc.value = Item.buyPrice(0, 0, 20, 0);
            npc.HitSound = SoundID.NPCHit36;
            npc.DeathSound = SoundID.NPCDeath39;
            npc.noGravity = true;
            npc.noTileCollide = true;
            banner = npc.type;
            bannerItem = ModContent.ItemType<PhantomSpiritBanner>();
        }

        public override void AI()
        {
            npc.TargetClosest(true);
            Vector2 vector102 = new Vector2(npc.Center.X, npc.Center.Y);
            float num818 = Main.player[npc.target].Center.X - vector102.X;
            float num819 = Main.player[npc.target].Center.Y - vector102.Y;
            float num820 = (float)Math.Sqrt((double)(num818 * num818 + num819 * num819));
            float num821 = 17f;
            num820 = num821 / num820;
            num818 *= num820;
            num819 *= num820;
            npc.velocity.X = (npc.velocity.X * 100f + num818) / 101f;
            npc.velocity.Y = (npc.velocity.Y * 100f + num819) / 101f;
            npc.rotation = (float)Math.Atan2((double)num819, (double)num818) - 1.57f;
            int num822 = Dust.NewDust(npc.position, npc.width, npc.height, 60, 0f, 0f, 0, default, 1f);
            Dust dust = Main.dust[num822];
            dust.velocity *= 0.1f;
            Main.dust[num822].scale = 1.3f;
            Main.dust[num822].noGravity = true;
            return;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            if (CalamityWorld.revenge)
            {
                player.AddBuff(ModContent.BuffType<MarkedforDeath>(), 120);
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 60, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int num288 = 0; num288 < 50; num288++)
                {
                    int num289 = Dust.NewDust(npc.position, npc.width, npc.height, 60, npc.velocity.X, npc.velocity.Y, 0, default, 1f);
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
            Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<Phantoplasm>());
        }
    }
}
