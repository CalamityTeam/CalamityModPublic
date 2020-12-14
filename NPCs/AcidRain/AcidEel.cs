using CalamityMod.Dusts;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
            NPCID.Sets.TrailingMode[npc.type] = 1;
            NPCID.Sets.TrailCacheLength[npc.type] = 7;
        }

        public override void SetDefaults()
        {
            npc.width = 72;
            npc.height = 18;

            npc.damage = 20;
            npc.lifeMax = 80;
            npc.defense = 4;
            npc.knockBackResist = 0.9f;

            if (CalamityWorld.downedPolterghast)
            {
				npc.DR_NERD(0.05f);
                npc.damage = 100;
                npc.lifeMax = 6000;
                npc.defense = 20;
				npc.knockBackResist = 0.7f;
            }
            else if (CalamityWorld.downedAquaticScourge)
            {
                npc.damage = 50;
                npc.lifeMax = 240;
            }

            npc.value = Item.buyPrice(0, 0, 3, 32);
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

            if (npc.ai[2] == 0f && !npc.wet)
            {
                npc.netUpdate = true;
                npc.ai[2] = 1f;
            }
            if (npc.ai[2] == 1f && npc.wet)
            {
                npc.netUpdate = true;
                npc.ai[2] = 0f;
            }

            if (npc.ai[2] == 0f)
            {
                npc.ai[1] += 1f;
                if (npc.ai[1] % 150f == 0f || npc.direction == 0)
                {
                    npc.direction = (Main.player[npc.target].position.X > npc.position.X).ToDirectionInt();
                }
                float acceleration = 0.3f;
                float yAcceleration = 0.08f;
                float maxSpeedX = 15f;
                float maxSpeedY = 4f;
                if (CalamityWorld.downedPolterghast)
                {
                    acceleration = 0.6f;
                    yAcceleration = 0.15f;
                    maxSpeedX = 20f;
                    maxSpeedY = 7f;
                }
                npc.velocity.X += npc.direction * acceleration;

                if (npc.collideX)
                    npc.direction *= -1;

                npc.spriteDirection = npc.direction;

                npc.velocity.Y += (Main.player[npc.target].position.Y > npc.position.Y).ToDirectionInt() * yAcceleration;
                if (npc.velocity.X > maxSpeedX)
                {
                    npc.velocity.X = maxSpeedX;
                    npc.netUpdate = true;
                }
                if (npc.velocity.X < -maxSpeedX)
                {
                    npc.velocity.X = -maxSpeedX;
                    npc.netUpdate = true;
                }
                if (npc.velocity.Y > maxSpeedY)
                {
                    npc.velocity.Y = maxSpeedY;
                    npc.netUpdate = true;
                }
                if (npc.velocity.Y < -maxSpeedY)
                {
                    npc.velocity.Y = -maxSpeedY;
                    npc.netUpdate = true;
                }
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

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(npc, ModContent.ItemType<SulfuricScale>(), 2 * (CalamityWorld.downedAquaticScourge ? 6 : 1), 1, 3);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<SlitheringEels>(), CalamityWorld.downedAquaticScourge, 0.05f);
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

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            CalamityGlobalNPC.DrawGlowmask(npc, spriteBatch, ModContent.GetTexture(Texture + "Glow"));
            if (npc.velocity.Length() > 1.5f)
            {
                CalamityGlobalNPC.DrawAfterimage(npc, spriteBatch, drawColor, Color.Transparent, directioning: true);
            }
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
