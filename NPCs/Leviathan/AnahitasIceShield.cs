using CalamityMod.Events;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Leviathan
{
    public class AnahitasIceShield : ModNPC
    {
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.canGhostHeal = false;
            NPC.noTileCollide = true;
            NPC.coldDamage = true;
            NPC.GetNPCDamage();
            NPC.width = 100;
            NPC.height = 100;
            NPC.defense = 10;
            NPC.DR_NERD(0.5f);
            NPC.lifeMax = 650;
            if (BossRushEvent.BossRushActive)
            {
                NPC.lifeMax = 1000;
            }
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.alpha = 255;
            NPC.HitSound = SoundID.NPCHit5;
            NPC.DeathSound = SoundID.NPCDeath7;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = false;
            NPC.Calamity().VulnerableToSickness = false;

            if (Main.getGoodWorld)
                NPC.scale *= 0.8f;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.dontTakeDamage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.dontTakeDamage = reader.ReadBoolean();
        }

        public override void AI()
        {
            int anahitaID = (int)NPC.ai[0];
            if (Main.npc[anahitaID].active && Main.npc[anahitaID].type == ModContent.NPCType<Anahita>())
            {
                if (NPC.alpha > 100 && NPC.ai[1] == 0f)
                    NPC.alpha -= 2;

                if (Main.npc[anahitaID].damage == 0)
                    NPC.ai[1] = 1f;
                else
                    NPC.ai[1] = 0f;

                if (NPC.ai[1] == 1f)
                    NPC.alpha = Main.npc[anahitaID].alpha;

                NPC.dontTakeDamage = Main.npc[anahitaID].damage == 0;
                NPC.rotation = Main.npc[anahitaID].rotation;
                NPC.spriteDirection = Main.npc[anahitaID].direction;
                NPC.velocity = Vector2.Zero;
                NPC.position = Main.npc[anahitaID].Center;
                NPC.position.X = NPC.position.X - (NPC.width / 2) + ((NPC.spriteDirection == 1) ? -20f : 20f) * NPC.scale;
                NPC.position.Y = NPC.position.Y - (NPC.height / 2) - (int)(30 * NPC.scale);
                NPC.gfxOffY = Main.npc[anahitaID].gfxOffY;
                Lighting.AddLight((int)NPC.Center.X / 16, (int)NPC.Center.Y / 16, 0f, 0.8f, 1.1f);
                return;
            }

            NPC.dontTakeDamage = false;
            NPC.life = 0;
            NPC.HitEffect();
            NPC.active = false;
            NPC.netUpdate = true;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => NPC.ai[1] == 0f;

        public override Color? GetAlpha(Color drawColor) => NPC.ai[1] == 1f ? Color.Transparent : new Color(200, 200, 200, drawColor.A) * NPC.Opacity;

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (NPC.ai[1] == 1f || hurtInfo.Damage <= 0)
                return;

            target.AddBuff(BuffID.Frostburn, 240, true);
        }

        public override bool CheckActive() => false;

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 67, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 25; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 67, hit.HitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
