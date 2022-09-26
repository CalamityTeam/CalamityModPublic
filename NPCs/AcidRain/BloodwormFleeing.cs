using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.AcidRain
{
    public class BloodwormFleeing : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloodworm");
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.NPCBestiaryDrawModifiers bestiaryData = new NPCID.Sets.NPCBestiaryDrawModifiers(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, bestiaryData);
        }

        public override void SetDefaults()
        {
            NPC.damage = 0;
            NPC.width = 12;
            NPC.height = 42;
            NPC.defense = 0;
            NPC.lifeMax = 5;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit20;
            NPC.DeathSound = SoundID.NPCDeath12;
        }

        public override void AI()
        {
            Player player = Main.player[Player.FindClosest(NPC.Center, 1, 1)];
            if (NPC.velocity == Vector2.Zero)
                NPC.velocity = Vector2.UnitY * 12f;

            float intertia = 24f;

            // Attempt to flee from the nearest player.
            NPC.velocity = (NPC.velocity * intertia - NPC.SafeDirectionTo(player.Center) * 12f) / (intertia + 1f);

            // But always dig downward.
            NPC.velocity.Y = Math.Abs(NPC.velocity.Y);
            NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 5)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= Main.npcFrameCount[NPC.type] * frameHeight)
                    NPC.frame.Y = 0;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
        }
    }
}
