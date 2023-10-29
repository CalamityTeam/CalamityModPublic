using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class SilvaCrystal : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public int dust = 3;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 52;
            Projectile.ignoreWater = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.timeLeft = 18000;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            bool isMinion = Projectile.type == ModContent.ProjectileType<SilvaCrystal>();
            if (!modPlayer.silvaSummon)
            {
                Projectile.active = false;
                return;
            }
            if (isMinion)
            {
                if (player.dead)
                {
                    modPlayer.sCrystal = false;
                }
                if (modPlayer.sCrystal)
                {
                    Projectile.timeLeft = 2;
                }
            }
            Projectile.Center = player.Center + Vector2.UnitY * (player.gfxOffY - 60f);
            if (player.gravDir == -1f)
            {
                Projectile.position.Y += 120f;
                Projectile.rotation = MathHelper.Pi;
            }
            else
            {
                Projectile.rotation = 0f;
            }
            Projectile.position.X = (int)Projectile.position.X;
            Projectile.position.Y = (int)Projectile.position.Y;
            Projectile.velocity = Vector2.Zero;
            Projectile.alpha -= 5;
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }
            if (Projectile.direction == 0)
            {
                Projectile.direction = Main.player[Projectile.owner].direction;
            }
            if (Projectile.alpha == 0 && Main.rand.NextBool(15))
            {
                Dust silvaDust = Main.dust[Dust.NewDust(Projectile.Top, 0, 0, 267, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 1f)];
                silvaDust.velocity.X = 0f;
                silvaDust.noGravity = true;
                silvaDust.fadeIn = 1f;
                silvaDust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(6.2831854820251465) * (4f * Main.rand.NextFloat() + 26f);
                silvaDust.scale = 0.5f;
            }
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= 60f)
            {
                Projectile.localAI[0] = 0f;
            }
            if (Projectile.ai[0] < 0f)
            {
                Projectile.ai[0] += 1f;
            }
            if (Projectile.ai[0] == 0f)
            {
                int targetID = -1;
                float attackRange = 1500f;
                if (player.HasMinionAttackTargetNPC)
                {
                    NPC npc = Main.npc[player.MinionAttackTargetNPC];
                    if (npc.CanBeChasedBy(Projectile, false))
                    {
                        float targetDist = Projectile.Distance(npc.Center);
                        if (targetDist < attackRange && Collision.CanHitLine(Projectile.Center, 0, 0, npc.Center, 0, 0))
                        {
                            attackRange = targetDist;
                            targetID = npc.whoAmI;
                        }
                    }
                }
                if (targetID < 0)
                {
                    int targetIDCopy;
                    for (int i = 0; i < Main.maxNPCs; i = targetIDCopy + 1)
                    {
                        NPC target = Main.npc[i];
                        if (target.CanBeChasedBy(Projectile, false))
                        {
                            float targetDistance = Projectile.Distance(target.Center);
                            if (targetDistance < attackRange && Collision.CanHitLine(Projectile.Center, 0, 0, target.Center, 0, 0))
                            {
                                attackRange = targetDistance;
                                targetID = i;
                            }
                        }
                        targetIDCopy = i;
                    }
                }
                if (targetID != -1)
                {
                    Projectile.ai[0] = 1f;
                    Projectile.ai[1] = (float)targetID;
                    Projectile.netUpdate = true;
                    return;
                }
            }
            if (Projectile.ai[0] > 0f)
            {
                int npcTrack = (int)Projectile.ai[1];
                if (!Main.npc[npcTrack].CanBeChasedBy(Projectile, false))
                {
                    Projectile.ai[0] = 0f;
                    Projectile.ai[1] = 0f;
                    Projectile.netUpdate = true;
                    return;
                }
                Projectile.ai[0] += 1f;
                if (Projectile.ai[0] >= 5f)
                {
                    int projXDirection = (Projectile.SafeDirectionTo(Main.npc[npcTrack].Center, Vector2.UnitY).X > 0f) ? 1 : -1;
                    Projectile.direction = projXDirection;
                    Projectile.ai[0] = -20f;
                    Projectile.netUpdate = true;
                    if (Projectile.owner == Main.myPlayer)
                    {
                        Vector2 attackPos = Main.npc[npcTrack].position + Main.npc[npcTrack].Size * Utils.RandomVector2(Main.rand, 0f, 1f) - Projectile.Center;
                        int attackAmt;
                        for (int j = 0; j < 3; j = attackAmt + 1)
                        {
                            Vector2 finalAttackPos = Projectile.Center + attackPos;
                            if (j > 0)
                            {
                                finalAttackPos = Projectile.Center + attackPos.RotatedByRandom(0.78539818525314331) * (Main.rand.NextFloat() * 0.5f + 0.75f);
                            }
                            float x4 = Main.rgbToHsl(new Color(Main.DiscoR, 203, 103)).X;
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), finalAttackPos, Vector2.Zero, ModContent.ProjectileType<SilvaCrystalExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner, x4, (float)Projectile.whoAmI);
                            attackAmt = j;
                        }
                        return;
                    }
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 127 - Projectile.alpha / 2);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Color colorArea = Lighting.GetColor((int)((double)Projectile.position.X + (double)Projectile.width * 0.5) / 16, (int)(((double)Projectile.position.Y + (double)Projectile.height * 0.5) / 16.0));
            Vector2 projPos = Projectile.position + new Vector2((float)Projectile.width, (float)Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
            Texture2D texture2D34 = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle rectangular = texture2D34.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Color colorAlpha = Projectile.GetAlpha(colorArea);
            Vector2 halfRectangle = rectangular.Size() / 2f;
            float scaleFactor = (float)Math.Cos((double)(6.28318548f * (Projectile.localAI[0] / 60f))) + 3f + 3f;
            for (float k = 0f; k < 4f; k += 1f)
            {
                SpriteBatch spritebatch = Main.spriteBatch;
                double arg_F8BE_1 = (double)(k * MathHelper.PiOver2);
                Vector2 center = default;
                Main.EntitySpriteDraw(texture2D34, projPos + Vector2.UnitY.RotatedBy(arg_F8BE_1, center) * scaleFactor, new Microsoft.Xna.Framework.Rectangle?(rectangular), colorAlpha * 0.2f, Projectile.rotation, halfRectangle, Projectile.scale, SpriteEffects.None, 0);
            }
            return false;
        }

        public override bool? CanDamage() => false;
    }
}
