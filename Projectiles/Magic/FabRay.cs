using CalamityMod.Buffs.Alcohol;
using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class FabRay : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 54;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 70;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = 60 * Projectile.extraUpdates;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.2f, 0.01f, 0.1f);
            Projectile.ai[0] += 1f;
            Projectile.Opacity = Utils.GetLerpValue(0f, 10f * Projectile.MaxUpdates, Projectile.timeLeft, true);

            int shootRate = Projectile.npcProj ? 40 : 12;
            if (Projectile.owner == Main.myPlayer && Projectile.ai[0] % shootRate == 0f)
            {
                NPC potentialTarget = Projectile.Center.ClosestNPCAt(180f, false);
                if (potentialTarget != null)
                {
                    Vector2 shootVelocity = Projectile.SafeDirectionTo(potentialTarget.Center) * 13f;
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center - shootVelocity * 2.5f, shootVelocity, ModContent.ProjectileType<FabBolt>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    if (p.WithinBounds(Main.maxProjectiles))
                    {
                        if (Projectile.hostile)
                        {
                            Main.projectile[p].hostile = true;
                            Main.projectile[p].friendly = false;
                            Main.projectile[p].DamageType = DamageClass.Default;
                        }
                    }
                }

                for (int i = 0; i < Projectile.oldPos.Length / 4; i += 3)
                {
                    potentialTarget = Projectile.oldPos[i].ClosestNPCAt(280f, false);
                    if (potentialTarget != null)
                    {
                        Vector2 shootVelocity = (potentialTarget.Center - Projectile.oldPos[i]).SafeNormalize(Vector2.UnitY) * 13f;
                        int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.oldPos[i] - shootVelocity * 2.5f, shootVelocity, ModContent.ProjectileType<FabBolt>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        if (p.WithinBounds(Main.maxProjectiles))
                        {
                            if (Projectile.hostile)
                            {
                                Main.projectile[p].hostile = true;
                                Main.projectile[p].friendly = false;
                                Main.projectile[p].DamageType = DamageClass.Default;
                            }
                        }
                        break;
                    }
                }
            }

            // Emit light.
            if (!Projectile.hostile)
                Lighting.AddLight(Projectile.Center, Vector3.One * Projectile.Opacity * 0.7f);
        }

        internal Color ColorFunction(float completionRatio)
        {
            float fadeToEnd = MathHelper.Lerp(0.65f, 1f, (float)Math.Cos(-Main.GlobalTimeWrappedHourly * 3f) * 0.5f + 0.5f);
            float fadeOpacity = Utils.GetLerpValue(1f, 0.64f, completionRatio, true) * Projectile.Opacity;
            Color endColor = Color.Lerp(Color.Cyan, Color.HotPink, (float)Math.Sin(completionRatio * MathHelper.Pi * 1.6f - Main.GlobalTimeWrappedHourly * 4f) * 0.5f + 0.5f);
            return Color.Lerp(Color.White, endColor, fadeToEnd) * fadeOpacity;
        }

        internal float WidthFunction(float completionRatio)
        {
            float expansionCompletion = 1f - (float)Math.Pow(1f - Utils.GetLerpValue(0f, 0.3f, completionRatio, true), 2D);
            return MathHelper.Lerp(0f, 32f * Projectile.Opacity, expansionCompletion);
        }

        internal Vector2 OffsetFunction(float completionRatio) => Projectile.Size * 0.5f;

        public override bool PreDraw(ref Color lightColor)
        {
            GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/ScarletDevilStreak"));
            PrimitiveSet.Prepare(Projectile.oldPos, new PrimitiveSettings(WidthFunction, ColorFunction, OffsetFunction, pixelate: false, shader: GameShaders.Misc["CalamityMod:TrailStreak"]), 32);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
            {
                Projectile.Kill();
            }
            else
            {
                if (Projectile.velocity.X != oldVelocity.X)
                {
                    Projectile.velocity.X = -oldVelocity.X;
                }
                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.velocity.Y = -oldVelocity.Y;
                }
            }
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Projectile.hostile)
                target.AddBuff(ModContent.BuffType<FabsolVodkaBuff>(), 54000);
        }
    }
}
