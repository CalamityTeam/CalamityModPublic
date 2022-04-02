using CalamityMod.Dusts;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.StatDebuffs;
namespace CalamityMod.NPCs.AcidRain
{
    public class Trilobite : ModNPC
    {
        public Player Target => Main.player[npc.target];
        public ref float SpikeShootCountdown => ref npc.ai[0];
        public const float MinSpeedLungePrompt = 0.5f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Trilobite");
            Main.npcFrameCount[npc.type] = 8;
            NPCID.Sets.TrailingMode[npc.type] = 1;
            NPCID.Sets.TrailCacheLength[npc.type] = 5;
        }

        public override void SetDefaults()
        {
            npc.width = 36;
            npc.height = 38;
            npc.aiStyle = aiType = -1;

            npc.damage = 45;
            npc.lifeMax = 300;
            npc.defense = 15;
            npc.DR_NERD(0.25f);

            if (CalamityWorld.downedPolterghast)
            {
                npc.damage = 80;
                npc.lifeMax = 4125;
                npc.defense = 30;
            }

            npc.knockBackResist = 0.2f;
            npc.value = Item.buyPrice(0, 0, 4, 0);
            npc.lavaImmune = false;
            npc.noGravity = true;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit42;
            npc.DeathSound = SoundID.NPCDeath27;
            banner = npc.type;
            bannerItem = ModContent.ItemType<TrilobiteBanner>();
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToSickness = false;
            npc.Calamity().VulnerableToElectricity = true;
            npc.Calamity().VulnerableToWater = false;
        }

        public override void AI()
        {
            npc.TargetClosest(false);
            if (!npc.wet)
            {
                GetStuckOnLand();
                return;
            }

            // If this NPC doesn't have enough momentum, lunge towards its target.
            if (npc.velocity.Length() < MinSpeedLungePrompt)
            {
                npc.TargetClosest();
                float lungeSpeed = CalamityWorld.downedPolterghast ? 18.5f : 15f;

                npc.velocity = npc.SafeDirectionTo(Target.Center, -Vector2.UnitY) * lungeSpeed;
                npc.velocity.X *= 1.6f;
                npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver2;
                npc.netUpdate = true;
                return;
            }

            // Accelerate quickly through the water horizontally.
            if (Math.Abs(npc.velocity.X) < 20f)
                npc.velocity.X += npc.direction * 0.02f;

            npc.rotation = npc.velocity.X * 0.4f;

            // Slow down in terms of horizontal speed if there is little vertical speed.
            if (Math.Abs(npc.velocity.Y) < 0.9f)
                npc.velocity.X *= 0.96f;

            // Lunge towards the target if there is little horizontal speed but also a little bit of vertical speed.
            else if (Main.netMode != NetmodeID.MultiplayerClient && Math.Abs(npc.velocity.X) < 3.5f)
            {
                float speedX = 18f;
                float speedY = 9f;
                if (CalamityWorld.downedPolterghast)
                {
                    speedX = 22f;
                    speedY = 11f;
                }
                npc.velocity = npc.SafeDirectionTo(Target.Center, -Vector2.UnitY) * new Vector2(speedX, speedY);
                npc.netUpdate = true;
            }
            npc.velocity.Y *= 0.98f;
        }

        public void GetStuckOnLand()
        {
            if (SpikeShootCountdown > 0)
                SpikeShootCountdown--;
            npc.rotation += npc.velocity.X * 0.1f;

            // If not moving vertically, slow down hotizontally.
            if (npc.velocity.Y == 0f)
            {
                npc.velocity.X *= 0.99f;
                if (Math.Abs(npc.velocity.X) < 0.01f)
                    npc.velocity.X = 0f;
            }

            // Fall and cap the vertical speed.
            if (npc.velocity.Y > 13f)
            {
                npc.velocity.Y = 13f;
                npc.netUpdate = true;
            }
            else
                npc.velocity.Y += 0.3f;
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(npc, ModContent.ItemType<CorrodedFossil>(), 3 * (CalamityWorld.downedPolterghast ? 5 : 1), 1, 3);
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            if (npc.velocity.Length() > 0.5f)
                CalamityGlobalNPC.DrawAfterimage(npc, spriteBatch, drawColor, Color.Transparent, directioning: true);
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            // No need for a netmode check- this code is only called by 1 client, the owner of the projectile that hit the NPC.
            if (SpikeShootCountdown <= 0f)
            {
                Main.PlaySound(SoundID.NPCDeath11, npc.Center);
                int projDamage = CalamityWorld.downedPolterghast ? 35 : CalamityWorld.downedAquaticScourge ? 29 : 21;
                if (Main.expertMode)
                    projDamage = (int)Math.Round(projDamage * 0.8);

                Vector2 spikeVelocity = -npc.velocity.RotatedByRandom(0.18f);
                Projectile.NewProjectile(npc.Center + Main.rand.NextVector2Unit() * npc.Size * 0.7f, spikeVelocity, ModContent.ProjectileType<TrilobiteSpike>(), projDamage, 3f);
                SpikeShootCountdown = Main.rand.Next(50, 65);
                npc.netUpdate = true;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter >= 4)
            {
                npc.frameCounter = 0;
                npc.frame.Y += frameHeight;
                if (npc.frame.Y >= Main.npcFrameCount[npc.type] * frameHeight)
                    npc.frame.Y = 0;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/TrilobiteGore"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/TrilobiteGore2"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/TrilobiteGore3"), npc.scale);
                for (int k = 0; k < 30; k++)
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
