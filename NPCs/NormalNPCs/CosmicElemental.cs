using CalamityMod.Items.Placeables.Banners;
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
            Main.npcFrameCount[NPC.type] = 11;
        }

        public override void SetDefaults()
        {
            NPC.npcSlots = 0.5f;
            NPC.aiStyle = 91;
            NPC.damage = 20;
            NPC.width = NPC.height = 30;
            NPC.defense = 10;
            NPC.lifeMax = 25;
            NPC.knockBackResist = 0.5f;
            NPC.value = Item.buyPrice(0, 0, 3, 0);
            NPC.HitSound = SoundID.NPCHit7;
            NPC.DeathSound = SoundID.NPCDeath6;
            banner = NPC.type;
            bannerItem = ModContent.ItemType<CosmicElementalBanner>();
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 6)
            {
                NPC.frame.Y += frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.ai[0] == -1f)
            {
                if (NPC.frame.Y >= frameHeight * 11)
                    NPC.frame.Y = frameHeight * 10;
                else if (NPC.frame.Y <= frameHeight * 5)
                    NPC.frame.Y = frameHeight * 6;
                NPC.rotation += NPC.velocity.X * 0.2f;
            }
            else
            {
                if (NPC.frame.Y >= frameHeight * 6)
                    NPC.frame.Y = 0;
                NPC.rotation = NPC.velocity.X * 0.1f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = Main.npcTexture[NPC.type];
            int height = texture.Height / Main.npcFrameCount[NPC.type];
            int width = texture.Width;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Main.spriteBatch.Draw(texture, NPC.Center - Main.screenPosition + new Vector2(0f, NPC.gfxOffY), NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, new Vector2(width / 2f, height / 2f), NPC.scale, spriteEffects, 0f);
            return false;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || spawnInfo.Player.Calamity().ZoneAbyss || spawnInfo.Player.Calamity().ZoneSunkenSea)
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
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 70, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 70, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void NPCLoot()
        {
            // 10% combined chance to drop any sword. Individual drop rates are:
            // Bone Sword: 5%
            // Starfury: 2%
            // Enchanted Sword: 2%
            // Arkhalis: 1%
            // Only one sword will ever drop at a time.
            if (Main.rand.NextBool(10))
            {
                DropHelper.DropItemFromWeightedSet(NPC,
                    DropHelper.WeightStack(ItemID.BoneSword, 5f, 1),
                    DropHelper.WeightStack(ItemID.Starfury, 2f, 1),
                    DropHelper.WeightStack(ItemID.EnchantedSword, 2f, 1),
                    DropHelper.WeightStack(ItemID.Arkhalis, 1f, 1));
            }
        }
    }
}
