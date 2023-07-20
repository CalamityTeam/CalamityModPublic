using CalamityMod.BiomeManagers;
using CalamityMod.Items.Critters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.DraedonLabThings
{
    public class Nanodroid : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;
            NPCID.Sets.CountsAsCritter[NPC.type] = true;
            Main.npcCatchable[NPC.type] = true;
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.LightningBug);
            NPC.width = 16;
            NPC.height = 12;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath44;
            NPC.catchItem = (short)ModContent.ItemType<NanodroidItem>();
            SpawnModBiomes = new int[1] { ModContent.GetInstance<ArsenalLabBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Nanodroid")
            });
        }

        public override void AI()
        {
            Lighting.AddLight(NPC.Center, 0.2f, 0.2f, 0.4f);
        }

        public override bool? CanBeHitByItem(Player player, Item item) => null;

        public override bool? CanBeHitByProjectile(Projectile projectile) => null;

        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;
            NPC.frameCounter += 0.3f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int i = 0; i < 6; i++)
                Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, 226);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D critterTexture = TextureAssets.Npc[NPC.type].Value;
            Texture2D glowmask = ModContent.Request<Texture2D>("CalamityMod/NPCs/DraedonLabThings/Nanodroid_Glow").Value;
            Vector2 drawPosition = NPC.Center - screenPos + Vector2.UnitY * NPC.gfxOffY;
            SpriteEffects direction = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(critterTexture, drawPosition, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, direction, 0f);
            spriteBatch.Draw(glowmask, drawPosition, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, direction, 0f);
            return false;
        }
    }
}
