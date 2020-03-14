using CalamityMod.Dusts;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Projectiles.Enemy;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using CalamityMod.Buffs.StatDebuffs;
namespace CalamityMod.NPCs.AcidRain
{
    public class Orthocera : ModNPC
    {
        public const float MaxSpeed = 10.5f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Orthocera");
            Main.npcFrameCount[npc.type] = 5;
        }

        public override void SetDefaults()
        {
            npc.width = 62;
            npc.height = 34;
            npc.defense = 10;

            npc.damage = Main.hardMode ? 70 : 60;
            npc.lifeMax = Main.hardMode ? 420 : 280;

            npc.knockBackResist = 0f;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.value = Item.buyPrice(0, 0, 4, 20);
            npc.lavaImmune = false;
            npc.noGravity = true;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit41;
            npc.DeathSound = SoundID.NPCDeath13;
            banner = npc.type;
            bannerItem = ModContent.ItemType<OrthoceraBanner>();
        }
        public override void AI()
        {
            npc.TargetClosest(false);
            npc.ai[1] += 1f;
            if (npc.target >= 0 && npc.target < 255)
            {
                Player player = Main.player[npc.target];
                // Swim
                if (npc.ai[1] % 250f < 180f)
                {
                    if (npc.wet)
                    {
                        // Reset so we can jum again.
                        npc.ai[3] = 0f;

                        // Swim towards the player if we're not too close.
                        // If we're close, simply resume our current movement.
                        if (npc.Distance(player.Center) > 150f)
                        {
                            npc.velocity = (npc.velocity * 17f + npc.DirectionTo(player.Center) * MaxSpeed) / 18f;
                            npc.ai[0] = 12f;
                        }
                        // Variable for X movement later.
                        // It seems to slow down for some dumb reason in the jump phase without this value
                        npc.ai[2] = npc.velocity.X;

                        // Make sure the value isn't 0 since we're relying on multiplication
                        if (npc.ai[2] == 0f)
                            npc.ai[2] = 0.1f;

                        if (Math.Abs(npc.ai[2]) < 7f)
                            npc.ai[2] = Math.Abs(npc.ai[2]) * 7f;
                        if (Math.Abs(npc.ai[2]) > 16f)
                            npc.ai[2] = Math.Abs(npc.ai[2]) * 16f;
                        npc.ai[2] = Math.Abs(npc.ai[2]) * (player.Center.X - npc.Center.X > 0).ToDirectionInt();
                    }
                    else
                    {
                        if (npc.ai[0] <= 0f)
                            npc.velocity.Y += 0.2f;
                        else
                            npc.ai[0] -= 1f;
                    }
                    npc.direction = npc.spriteDirection = (npc.velocity.X > 0).ToDirectionInt();
                }
                // And jump/shoot
                else if (npc.ai[1] % 220f > 180f)
                {                    
                    if (npc.ai[1] % 220f < 200f)
                    {
                        npc.velocity.Y -= 0.05f;
                    }
                    else
                    {
                        npc.velocity.Y += 0.05f;
                    }
                    if (npc.ai[1] % 220f == 219f)
                        npc.ai[3] = 1f;
                    if (!npc.wet)
                        npc.velocity.X = npc.ai[2];
                }
                // Don't jump mid-air
                if (npc.ai[1] % 220f > 180f && npc.ai[3] == 1f)
                    npc.ai[1] = 0f;
                npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver4 + MathHelper.PiOver2 + MathHelper.Pi;

                if (npc.spriteDirection == -1)
                    npc.rotation -= MathHelper.PiOver2;
                // If sitting on land, slow down and, if in the middle of a jump, release a stream of acid.
                if (!npc.wet)
                {
                    npc.velocity.X *= 0.92f;
                    // Spit out a stream of acid based on our rotation
                    if (npc.ai[1] % 220f == 195f)
                    {
                        float rotation = npc.rotation - MathHelper.Pi - MathHelper.PiOver2 - MathHelper.PiOver4;
                        if (npc.spriteDirection == -1)
                            rotation += MathHelper.PiOver2;
                        int damage = Main.hardMode ? 26 : 18;
                        Projectile.NewProjectile(npc.Center, rotation.ToRotationVector2() * 12f, ModContent.ProjectileType<OrthoceraStream>(), damage, 2f);
                    }
                }
                // Prevent yeeting into the sky at the speed of light
                npc.velocity = Vector2.Clamp(npc.velocity, new Vector2(-MaxSpeed), new Vector2(MaxSpeed));
            }
        }
        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter >= 6)
            {
                npc.frameCounter = 0;
                npc.frame.Y += frameHeight;
                if (npc.frame.Y >= Main.npcFrameCount[npc.type] * frameHeight)
                {
                    npc.frame.Y = 0;
                }
            }
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.damage = Main.hardMode ? 80 : 69;
            npc.lifeMax = Main.hardMode ? 480 : 335;
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/OrthoceraGore"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/OrthoceraGore2"), npc.scale);
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }
    }
}
