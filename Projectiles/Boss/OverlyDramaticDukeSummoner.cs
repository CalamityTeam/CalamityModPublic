using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class OverlyDramaticDukeSummoner : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/OldDukeVortex";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Old Duke Summoner");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1;
            Projectile.scale = 0.004f;
            Projectile.hostile = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 1800;
        }
        private static void ExpandVertically(int startX, int startY, out int topY, out int bottomY, int maxExpandUp = 100, int maxExpandDown = 100)
        {
            topY = startY;
            bottomY = startY;
            if (!WorldGen.InWorld(startX, startY, 10))
            {
                return;
            }
            int yUp = 0;
            while (yUp < maxExpandUp && topY > 0 && topY >= 10 && Main.tile[startX, topY] != null)
            {
                topY--;
                yUp++;
            }
            int yDown = 0;
            while (yDown < maxExpandDown && bottomY < Main.maxTilesY - 10 && bottomY <= Main.maxTilesY - 10)
            {
                if (Main.tile[startX, bottomY] == null)
                {
                    return;
                }
                bottomY++;
                yDown++;
            }
        }

        public override void AI()
        {
            Projectile.rotation -= 0.15f * (float)(1D - (Projectile.alpha / 255D));
            Projectile.ai[0]++;

            float totalTilesToExpand = 1600f * Projectile.scale / 16;

            Point centerAsTileCoords = Projectile.Center.ToTileCoordinates();
            ExpandVertically(centerAsTileCoords.X, centerAsTileCoords.Y, out int topY, out int bottomY, (int)(totalTilesToExpand / 2), (int)(totalTilesToExpand / 2));
            topY++;
            bottomY--;
            Vector2 topVector = new Vector2(centerAsTileCoords.X, topY) * 16f + new Vector2(8f);
            Vector2 bottomVector = new Vector2(centerAsTileCoords.X, bottomY) * 16f + new Vector2(8f);
            Vector2 centerVector = Vector2.Lerp(topVector, bottomVector, 0.5f);
            Projectile.width = (int)(208 * Projectile.scale);
            Projectile.height = (int)(bottomVector.Y - topVector.Y);
            Projectile.Center = centerVector;

            if (Projectile.ai[0] < 90f)
            {
                Projectile.alpha = (int)MathHelper.Lerp(255f, 0f, Projectile.ai[0] / 90f);
                Projectile.scale = MathHelper.Lerp(0.004f, 1f, Projectile.ai[0] / 90f);
            }
            // Spray gore and acid everywhere
            else if (Projectile.ai[0] < 480f)
            {
                if (Projectile.ai[0] % 10f == 9f)
                {
                    Vector2 velocity = new Vector2(0f, -18f).RotatedByRandom(0.7f);
                    Projectile.NewProjectile(Projectile.Top + new Vector2(Main.rand.NextFloat(-80f, 80f), 100f), velocity,
                        ModContent.ProjectileType<OldDukeSummonDrop>(), 65, 2f);
                }
                if (Projectile.ai[0] % 35f == 34f)
                {
                    Vector2 velocity = new Vector2(Main.rand.NextFloat(-3f, 3f), -7f - Main.rand.NextFloat(4f, 12f)).RotatedByRandom(0.5f);
                    Projectile.NewProjectile(Projectile.Top + new Vector2(Main.rand.NextFloat(-30f, 30f), 100f), velocity,
                        ModContent.ProjectileType<OldDukeGore>(), 65, 2f);
                }
            }

            // Fade out and die
            if (Projectile.ai[0] >= 600f)
            {
                Projectile.alpha = (int)MathHelper.Lerp(0f, 255f, (Projectile.ai[0] - 600f) / 120f);

                bool canSpawnBoomer = false;
                for (int i = 0; i < Main.player.Length; i++)
                {
                    if (!Main.player[i].dead && Projectile.Distance(Main.player[i].Center) < 12000f)
                    {
                        canSpawnBoomer = true;
                        break;
                    }
                }

                // Summon the boomer duke
                if (Projectile.ai[0] == 660f)
                {
                    if (canSpawnBoomer)
                    {
                        for (int i = 0; i < 160; i++)
                        {
                            Dust dust = Dust.NewDustPerfect(Projectile.Top + Vector2.UnitY * 100f, (int)CalamityDusts.SulfurousSeaAcid);
                            dust.velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(5f, 23f);
                            dust.noGravity = true;
                            dust.scale = 3f;
                        }
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int boomer = NPC.NewNPC((int)Projectile.Top.X, (int)Projectile.Top.Y + 100, ModContent.NPCType<OldDuke>());
                            string boomerName = Main.npc[boomer].TypeName;

                            if (Main.netMode == NetmodeID.SinglePlayer)
                            {
                                Main.NewText(Language.GetTextValue("Announcement.HasAwoken", boomerName), new Color(175, 75, 255));
                                return;
                            }

                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.BroadcastChatMessage(NetworkText.FromKey("Announcement.HasAwoken", new object[]
                                {
                            Main.npc[boomer].GetTypeNetName()
                                }), new Color(175, 75, 255));
                                return;
                            }

                            CalamityUtils.BossAwakenMessage(boomer);

                            Main.npc[boomer].velocity = Vector2.UnitY * -12f;
                            Main.npc[boomer].alpha = 255;
                            Main.npc[boomer].Calamity().newAI[3] = 1f; // To signal that Old Duke should not deccelerate as it normally would
                            Main.npc[boomer].netUpdate = true;
                            CalamityWorld.triedToSummonOldDuke = true;
                            CalamityWorld.encounteredOldDuke = true;
                            AcidRainEvent.UpdateInvasion(false);
                        }
                    }
                    else
                    {
                        CalamityWorld.acidRainPoints = 0;
                        CalamityWorld.triedToSummonOldDuke = false;
                        AcidRainEvent.UpdateInvasion(false);
                    }
                }
            }
            if (Projectile.ai[0] >= 720f)
            {
                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float totalTilesToExpand = 1600f * Projectile.scale / 16;
            Point centerAsTileCoords = Projectile.Center.ToTileCoordinates();
            ExpandVertically(centerAsTileCoords.X, centerAsTileCoords.Y, out _, out int bottomY, (int)(totalTilesToExpand / 2), (int)(totalTilesToExpand / 2));
            bottomY--;
            Vector2 bottomVector = new Vector2(centerAsTileCoords.X, bottomY) * 16f + new Vector2(8f);

            Texture2D texture = ModContent.Request<Texture2D>(Texture);
            float yMax = 1600f * Projectile.scale;
            for (int y = 0; y < yMax; y += 30)
            {
                float thetaDelta = MathHelper.Pi * y / yMax;
                float scale = Projectile.scale * MathHelper.Lerp(0.3f, 1.4f, y / yMax);
                float alphaMultiplier = MathHelper.Lerp(1f, 0.6f, y / yMax);
                Main.EntitySpriteDraw(texture,
                                 bottomVector - Main.screenPosition - Vector2.UnitY * y,
                                 null,
                                 Color.White * Projectile.Opacity * 0.3f * alphaMultiplier,
                                 Projectile.rotation + thetaDelta,
                                 texture.Size() * 0.5f,
                                 scale,
                                 SpriteEffects.None,
                                 0f);
            }
            return false;
        }

        public override bool CanHitPlayer(Player target) => Projectile.ai[0] >= 90f;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 240f * Projectile.scale, targetHitbox);

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 420);
        }
    }
}
