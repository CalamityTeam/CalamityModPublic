using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Projectiles.Enemy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Abyss
{
    public class ChaoticPuffer : ModNPC
    {
        public bool puffedUp = false;
        public bool puffing = false;
        public bool unpuffing = false;
        public int puffTimer = 0;
        public int puffingTimer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chaotic Puffer");
            Main.npcFrameCount[npc.type] = 11;
        }

        public override void SetDefaults()
        {
            npc.noGravity = true;
            npc.lavaImmune = true;
            npc.width = 78;
            npc.height = 70;
            npc.defense = 50;
            npc.lifeMax = 5625;
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 0, 30, 0);
            npc.HitSound = SoundID.NPCHit23;
            npc.DeathSound = SoundID.NPCDeath28;
            banner = npc.type;
            bannerItem = ModContent.ItemType<ChaoticPufferBanner>();
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToSickness = true;
            npc.Calamity().VulnerableToElectricity = true;
            npc.Calamity().VulnerableToWater = false;
        }

        public override void AI()
        {
            npc.TargetClosest(true);
            npc.velocity.X = npc.velocity.X + (float)npc.direction * 0.03f;
            npc.velocity.Y = npc.velocity.Y + (float)npc.directionY * 0.03f;

            npc.damage = puffedUp ? (Main.expertMode ? 175 : 100) : 0;


            if (!puffing || !unpuffing)
            {
                ++puffTimer;
            }

            if (puffTimer >= 300)
            {

                if (!puffedUp)
                {
                    puffing = true;
                }
                else
                {
                    unpuffing = true;
                }

                puffTimer = 0;

            }
            else if (puffing || unpuffing)
            {

                ++puffingTimer;

                if (puffingTimer > 16 && puffing)
                {

                    puffing = false;
                    puffedUp = true;
                    puffingTimer = 0;

                }
                else if (puffingTimer > 16 && unpuffing)
                {

                    unpuffing = false;
                    puffedUp = false;
                    puffingTimer = 0;

                }

            }

            if (npc.velocity.X >= 1 || npc.velocity.X <= -1)
            {

                npc.velocity.X = npc.velocity.X * 0.97f;

            }

            if (npc.velocity.Y >= 1 || npc.velocity.Y <= -1)
            {

                npc.velocity.Y = npc.velocity.Y * 0.97f;

            }

            npc.rotation += npc.velocity.X * 0.05f;

        }

        public void Boom()
        {
            Main.PlaySound(SoundID.NPCDeath14, (int)npc.position.X, (int)npc.position.Y);
            if (Main.netMode != NetmodeID.MultiplayerClient && puffedUp)
            {
                int damageBoom = 45;
                int projectileType = ModContent.ProjectileType<PufferExplosion>();
                int boom = Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0, 0, projectileType, damageBoom, 0f, Main.myPlayer, 0f, 0f);
            }
            npc.netUpdate = true;
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
            vector -= new Vector2((float)ModContent.GetTexture("CalamityMod/NPCs/Abyss/ChaoticPufferGlow").Width, (float)(ModContent.GetTexture("CalamityMod/NPCs/Abyss/ChaoticPufferGlow").Height / Main.npcFrameCount[npc.type])) * 1f / 2f;
            vector += vector11 * 1f + new Vector2(0f, 4f + npc.gfxOffY);
            Color color = new Color(127 - npc.alpha, 127 - npc.alpha, 127 - npc.alpha, 0).MultiplyRGBA(Microsoft.Xna.Framework.Color.Yellow);
            Main.spriteBatch.Draw(ModContent.GetTexture("CalamityMod/NPCs/Abyss/ChaoticPufferGlow"), vector,
                new Microsoft.Xna.Framework.Rectangle?(npc.frame), color, npc.rotation, vector11, 1f, spriteEffects, 0f);
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 1.0;
            if (npc.frameCounter > 6.0)
            {
                npc.frameCounter = 0.0;
                if (!unpuffing)
                {
                    npc.frame.Y = npc.frame.Y + frameHeight;
                }
                else
                {
                    npc.frame.Y = npc.frame.Y - frameHeight;
                }
            }
            if (puffing)
            {
                if (npc.frame.Y < frameHeight * 3)
                {
                    npc.frame.Y = frameHeight * 3;
                }
                if (npc.frame.Y > frameHeight * 6)
                {
                    npc.frame.Y = frameHeight * 3;
                }
            }
            else if (unpuffing)
            {
                if (npc.frame.Y > frameHeight * 6)
                {
                    npc.frame.Y = frameHeight * 6;
                }
                if (npc.frame.Y < frameHeight * 3)
                {
                    npc.frame.Y = frameHeight * 6;
                }
            }
            else if (!puffedUp)
            {
                if (npc.frame.Y > frameHeight * 3)
                {
                    npc.frame.Y = 0;
                }
            }
            else
            {
                if (npc.frame.Y < frameHeight * 7)
                {
                    npc.frame.Y = frameHeight * 7;
                }
                if (npc.frame.Y > frameHeight * 10)
                {
                    npc.frame.Y = frameHeight * 7;
                }
            }

        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.player.Calamity().ZoneAbyssLayer2 && spawnInfo.water)
            {
                return SpawnCondition.CaveJellyfish.Chance * 0.4f;
            }
            if (spawnInfo.player.Calamity().ZoneAbyssLayer3 && spawnInfo.water)
            {
                return SpawnCondition.CaveJellyfish.Chance * 0.6f;
            }
            return 0f;
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemCondition(npc, ModContent.ItemType<ChaoticOre>(), NPC.downedGolemBoss, 1f, 10, 26);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 180);

            if (puffedUp)
            {
                Boom();
                npc.active = false;
            }
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            npc.velocity.X = projectile.velocity.X;
            npc.velocity.Y = projectile.velocity.Y;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                Boom();

                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
