using CalamityMod.Dusts;
using CalamityMod.Items.Placeables.Banners;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.StatDebuffs;
namespace CalamityMod.NPCs.AcidRain
{
    public class AcidEel : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Acid Eel");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            npc.width = 72;
            npc.height = 18;
            npc.defense = 4;

            npc.damage = Main.hardMode ? 58 : 41;
            npc.lifeMax = Main.hardMode ? 430 : 180;

            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 0, 3, 32);
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.aiStyle = -1;
            aiType = -1;
            npc.lavaImmune = false;
            npc.noGravity = true;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            banner = npc.type;
            bannerItem = ModContent.ItemType<AcidEelBanner>();
        }
        public override void AI()
        {
            npc.TargetClosest(false);

            if (Main.rand.NextBool(480))
                Main.PlaySound(SoundID.Zombie, npc.Center, 32); // Slither sound

            if (npc.wet)
            {
                npc.ai[1] += 1f;
                if (npc.ai[1] % 150f == 0f || npc.direction == 0)
                {
                    npc.direction = (Main.player[npc.target].position.X > npc.position.X).ToDirectionInt();
                }
                npc.velocity.X += npc.direction * 0.3f;

                if (npc.collideX)
                    npc.direction *= -1;

                npc.spriteDirection = npc.direction;

                npc.velocity.Y += (Main.player[npc.target].position.Y > npc.position.Y).ToDirectionInt() * 0.075f;
                npc.velocity = Vector2.Clamp(npc.velocity, new Vector2(-15f, -3f), new Vector2(15f, 3f));
                npc.rotation = npc.velocity.X * 0.02f;
            }
            else
            {
                npc.rotation = npc.rotation.AngleLerp(0f, 0.1f);
                npc.velocity.X *= 0.95f;
                if (npc.velocity.Y < 14f)
                    npc.velocity.Y += 0.15f;
            }
        }
        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter >= 5)
            {
                npc.frameCounter = 0;
                npc.frame.Y += frameHeight;
                if (npc.frame.Y >= Main.npcFrameCount[npc.type] * frameHeight)
                {
                    npc.frame.Y = 0;
                }
            }
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.damage = (int)(npc.damage * 1.2);
            npc.lifeMax = Main.hardMode ? 500 : 230;
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 8; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/AcidEelGore"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/AcidEelGore2"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/AcidEelGore3"), npc.scale);
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }
    }
}
