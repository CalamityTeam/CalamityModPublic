using CalamityMod.BiomeManagers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Linq;
using CalamityMod.Items.Critters;
using CalamityMod.Items.Placeables.Banners;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.Audio;
using Terraria.DataStructures;

namespace CalamityMod.NPCs.Abyss
{
    public class BabyCannonballJellyfish : ModNPC
    {
        public bool hasBeenHit = false;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 7;
            //Main.npcCatchable[NPC.type] = true;
            NPCID.Sets.CountsAsCritter[NPC.type] = true;
        }

        public override void SetDefaults()
        {
            NPC.npcSlots = 0.1f;
            NPC.noGravity = true;
            NPC.chaseable = false;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.damage = 30;
            NPC.width = 28;
            NPC.height = 36;
            NPC.defense = 0;
            NPC.lifeMax = 5;
            NPC.knockBackResist = 1f;
            NPC.alpha = 100;
            NPC.HitSound = SoundID.NPCHit25;
            NPC.DeathSound = SoundID.NPCDeath28;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<BabyCannonballJellyfishBanner>();
            NPC.catchItem = (short)ModContent.ItemType<BabyCannonballJellyfishItem>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<AbyssLayer1Biome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.BabyCannonballJellyfish")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.chaseable);
            writer.Write(hasBeenHit);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.chaseable = reader.ReadBoolean();
            hasBeenHit = reader.ReadBoolean();
        }

        public override void AI()
        {
            if (NPC.localAI[0] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.localAI[0] = 1f;
                NPC.velocity.Y = -3f;
                NPC.netUpdate = true;
            }
            Lighting.AddLight(NPC.Center, (67 - NPC.alpha) * 1f / 255f, (218 - NPC.alpha) * 1f / 255f, (166 - NPC.alpha) * 1f / 255f);
            if (NPC.wet)
            {
                NPC.noGravity = true;
                if (NPC.velocity.Y < 0f)
                {
                    NPC.velocity.Y += 0.1f;
                }
                if (NPC.velocity.Y > 0f)
                {
                    NPC.velocity.Y = 0f;
                }
            }
            else
            {
                NPC.noGravity = false;
            }
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.minion && !projectile.Calamity().overridesMinionDamagePrevention)
            {
                return hasBeenHit;
            }
            return null;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.Calamity().ZoneAbyssLayer1 && spawnInfo.Water)
            {
                return SpawnCondition.CaveJellyfish.Chance;
            }
            return 0f;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Vector2 center = new Vector2(NPC.Center.X, NPC.Center.Y);
            Vector2 halfSizeTexture = new Vector2((float)(TextureAssets.Npc[NPC.type].Value.Width / 2), (float)(TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2));
            Vector2 vector = center - screenPos;
            vector -= new Vector2((float)ModContent.Request<Texture2D>("CalamityMod/NPCs/Abyss/BabyCannonballJellyfishGlow").Value.Width, (float)(ModContent.Request<Texture2D>("CalamityMod/NPCs/Abyss/BabyCannonballJellyfishGlow").Value.Height / Main.npcFrameCount[NPC.type])) * 1f / 2f;
            vector += halfSizeTexture * 1f + new Vector2(0f, 4f + NPC.gfxOffY);
            Color color = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(new Color(67, 218, 166));
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>("CalamityMod/NPCs/Abyss/BabyCannonballJellyfishGlow").Value, vector,
                new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, halfSizeTexture, 1f, spriteEffects, 0f);
        }

        public override bool CheckDead()
        {
            Explode();
            return false; //custom death handling
        }

        private void Explode()
        {
            NPC.position = NPC.Center;
            NPC.width = (int)(NPC.width * 3f);
            NPC.height = (int)(NPC.height * 3f);
            NPC.position -= NPC.Size * 0.5f; //hitbox expansion + adjustments
            SoundEngine.PlaySound(SoundID.Item14, NPC.Center); //bomb sound
            foreach (Player player in Main.player.Where(player => player.active && !player.dead && NPC.Hitbox.Intersects(player.Hitbox)))
                player.Hurt(PlayerDeathReason.ByNPC(NPC.whoAmI), NPC.damage, NPC.direction);
            for (int k = 0; k < 8; k++)
            {
                int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.MoonBoulder, 0f, -1f, 0, default, 1f);
                if (Main.dust.IndexInRange(dust))
                    Main.dust[dust].noGravity = true;
            }
            NPC.active = false;
            NPC.NPCLoot();
            NPC.netUpdate = true;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => NPC.life <= 0;

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 2; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.MoonBoulder, hit.HitDirection, -1f, 0, default, 1f);
            }
        }
    }
}
