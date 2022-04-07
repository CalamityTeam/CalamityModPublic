using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.Astral
{
    public class AstralachneaWall : ModNPC
    {
        private static Texture2D glowmask;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astralachnea");

            Main.npcFrameCount[NPC.type] = 4;

            if (!Main.dedServ)
                glowmask = ModContent.Request<Texture2D>("CalamityMod/NPCs/Astral/AstralachneaWallGlow").Value;

            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            NPC.width = 60;
            NPC.height = 60;
            NPC.aiStyle = -1;
            NPC.damage = 55;
            NPC.defense = 20;
            NPC.DR_NERD(0.15f);
            NPC.lifeMax = 500;
            NPC.DeathSound = Mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/AstralEnemyDeath");
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.value = Item.buyPrice(0, 0, 20, 0);
            NPC.timeLeft = NPC.activeTime * 2;
            animationType = NPCID.BlackRecluseWall;
            Banner = ModContent.NPCType<AstralachneaGround>();
            BannerItem = ModContent.ItemType<AstralachneaBanner>();
            if (DownedBossSystem.downedAstrumAureus)
            {
                NPC.damage = 90;
                NPC.defense = 30;
                NPC.lifeMax = 750;
            }
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void AI()
        {
            CalamityGlobalNPC.DoSpiderWallAI(NPC, ModContent.NPCType<AstralachneaGround>(), (CalamityWorld.death ? 3.6f : 2.4f), (CalamityWorld.death ? 0.15f : 0.1f));
        }

        public override void FindFrame(int frameHeight)
        {
            //DO DUST
            int frame = NPC.frame.Y / frameHeight;
            Rectangle rect = new Rectangle(12, 24, 18, 10);
            Rectangle rect2 = new Rectangle(12, 44, 18, 10);
            switch (frame)
            {
                case 1:
                    rect = new Rectangle(6, 26, 28, 8);
                    rect2 = new Rectangle(6, 44, 28, 8);
                    break;
                case 2:
                    rect = new Rectangle(12, 26, 18, 8);
                    rect2 = new Rectangle(12, 44, 18, 8);
                    break;
                case 3:
                    rect = new Rectangle(16, 24, 16, 10);
                    rect2 = new Rectangle(16, 44, 16, 10);
                    break;
            }
            Dust d = CalamityGlobalNPC.SpawnDustOnNPC(NPC, 80, frameHeight, ModContent.DustType<AstralOrange>(), rect, Vector2.Zero, 0.225f, true);
            Dust d2 = CalamityGlobalNPC.SpawnDustOnNPC(NPC, 80, frameHeight, ModContent.DustType<AstralOrange>(), rect2, Vector2.Zero, 0.225f, true);
            if (d != null)
            {
                d.customData = 0.04f;
            }
            if (d2 != null)
            {
                d2.customData = 0.04f;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.soundDelay == 0)
            {
                NPC.soundDelay = 15;
                switch (Main.rand.Next(3))
                {
                    case 0:
                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/NPCHit/AstralEnemyHit"), NPC.Center);
                        break;
                    case 1:
                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/NPCHit/AstralEnemyHit2"), NPC.Center);
                        break;
                    case 2:
                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/NPCHit/AstralEnemyHit3"), NPC.Center);
                        break;
                }
            }

            CalamityGlobalNPC.DoHitDust(NPC, hitDirection, (Main.rand.Next(0, Math.Max(0, NPC.life)) == 0) ? 5 : ModContent.DustType<AstralEnemy>(), 1f, 4, 22);

            //if dead do gores
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        Gore.NewGore(NPC.Center, NPC.velocity * 0.3f, Mod.Find<ModGore>("Gores/Astralachnea/AstralachneaGore" + i).Type);
                    }
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 origin = new Vector2(40f, 40f);
            spriteBatch.Draw(glowmask, NPC.Center - screenPos - new Vector2(0, 8f), NPC.frame, Color.White * 0.6f, NPC.rotation, origin, 1f, SpriteEffects.None, 0);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120, true);
        }

        public override void NPCLoot()
        {
            DropHelper.DropItem(NPC, ModContent.ItemType<Stardust>(), 2, 3);
            DropHelper.DropItemCondition(NPC, ModContent.ItemType<Stardust>(), Main.expertMode);
            DropHelper.DropItemCondition(NPC, ModContent.ItemType<AstralachneaStaff>(), DownedBossSystem.downedAstrumAureus, 7, 1, 1);
        }
    }
}
