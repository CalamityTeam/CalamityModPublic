using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.SunkenSea
{
    public class BlindedAngler : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blinded Angler");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            npc.noGravity = true;
            npc.damage = 150;
            npc.width = 56;
            npc.height = 44;
            npc.defense = 30;
            npc.DR_NERD(0.2f);
            npc.lifeMax = 750;
            npc.aiStyle = -1;
            aiType = -1;
            npc.value = Item.buyPrice(0, 0, 40, 0);
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.knockBackResist = 0.1f;
            banner = npc.type;
            bannerItem = ModContent.ItemType<BlindedAnglerBanner>();
            npc.chaseable = false;
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToSickness = true;
            npc.Calamity().VulnerableToElectricity = true;
            npc.Calamity().VulnerableToWater = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.chaseable);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.chaseable = reader.ReadBoolean();
        }

        public override void AI()
        {
            Lighting.AddLight(npc.Center, (255 - npc.alpha) * 0f / 255f, (255 - npc.alpha) * 0.75f / 255f, (255 - npc.alpha) * 0.75f / 255f);
            CalamityAI.PassiveSwimmingAI(npc, mod, 1, 100f, 0.1f, 0.1f, 3f, 3f, 0.1f);
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += npc.wet ? 0.15f : 0f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (npc.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Vector2 center = new Vector2(npc.Center.X, npc.Center.Y);
            Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2));
            Vector2 vector = center - Main.screenPosition;
            vector -= new Vector2((float)ModContent.GetTexture("CalamityMod/NPCs/SunkenSea/BlindedAnglerGlow").Width, (float)(ModContent.GetTexture("CalamityMod/NPCs/SunkenSea/BlindedAnglerGlow").Height / Main.npcFrameCount[npc.type])) * 1f / 2f;
            vector += vector11 * 1f + new Vector2(0f, 4f + npc.gfxOffY);
            Color color = new Color(127 - npc.alpha, 127 - npc.alpha, 127 - npc.alpha, 0).MultiplyRGBA(Microsoft.Xna.Framework.Color.LightBlue);
            Main.spriteBatch.Draw(ModContent.GetTexture("CalamityMod/NPCs/SunkenSea/BlindedAnglerGlow"), vector,
                new Microsoft.Xna.Framework.Rectangle?(npc.frame), color, npc.rotation, vector11, 1f, spriteEffects, 0f);
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.minion && !projectile.Calamity().overridesMinionDamagePrevention)
            {
                return npc.chaseable;
            }
            return null;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (Main.hardMode && spawnInfo.player.Calamity().ZoneSunkenSea && spawnInfo.water && !spawnInfo.player.Calamity().clamity)
            {
                return SpawnCondition.CaveJellyfish.Chance * 0.45f;
            }
            return 0f;
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(npc, ModContent.ItemType<EutrophicScimitar>(), 0.25f);
            DropHelper.DropItem(npc, ModContent.ItemType<PrismShard>(), 5, 9);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 68, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 25; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 68, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/BlindedAngler/BlindAnglerGore1"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/BlindedAngler/BlindAnglerGore2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/BlindedAngler/BlindAnglerGore3"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/BlindedAngler/BlindAnglerGore4"), 1f);
            }
        }
    }
}
