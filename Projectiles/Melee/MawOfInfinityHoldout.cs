using System;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Projectiles.Boss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class MawOfInfinityHoldout : BaseSwordHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override bool useAttackSpeed => true;
        public override bool useMeleeSize => true;
        public override int swingWidth => 270;
        public override Item BaseItem => ModContent.GetModItem(ModContent.ItemType<MawOfInfinity>()).Item;
        public override int AfterImageLength => 0;

        public override int StartupTime { get; set; }
        public override int CooldownTime { get; set; }

        public override string Texture => ModContent.GetModItem(BaseItem.type).Texture;

        public override float lineCollisionLength => 232;
        public override bool AlternateSwings => true;

        public override void Defaults()
        {
            Projectile.width = 78;
            Projectile.height = 94;
            Projectile.extraUpdates = 5; //ExtraUpdates help make collision more accurate
            Projectile.noEnchantmentVisuals = true;
        }
        public override void Spawn()
        {
            var player = Main.player[Projectile.owner];
            var modplayer = player.GetModPlayer<BaseSwordHoldoutPlayer>();
            StartupTime = 10;
            CooldownTime = 10;
            swingTime -= StartupTime + CooldownTime;

            if (Main.myPlayer == Projectile.owner)
                modplayer.swingNum = modplayer.swingNum++ % 3;
            OffsetDistance = 70;
            RotateInStartup = 0.2f;
            RotateInCooldown = 0f;

            UseSound = SoundID.DD2_MonkStaffSwing;
        }

        public override void AdditionalAI()
        {
            var player = Main.player[Projectile.owner];
            var modplayer = player.GetModPlayer<BaseSwordHoldoutPlayer>();
            switch (modplayer.swingNum)
            {
                case 1:
                    if (inSwing)
                    {
                        var veloc = oldPlayerOffset - (Projectile.Center - Main.player[Projectile.owner].Center);
                        veloc.Normalize();
                        int sparkLifetime = Main.rand.Next(15, 23);

                        Vector2 sparkVel = Vector2.UnitY * -9f;
                        float maxRotationDeviance = 0.4f;
                        float rotationAngle = Main.rand.NextFloat(-maxRotationDeviance, maxRotationDeviance);
                        sparkVel = sparkVel.RotatedBy(rotationAngle) * Main.rand.NextFloat(0.3f, 1.0f);

                        float sparkScale = Main.rand.NextFloat(0.007f, 0.015f);

                        Vector2 compensatedSparkVel = veloc.RotatedBy(MathHelper.PiOver4 * 0.5f * Projectile.spriteDirection) * Main.rand.NextFloat(2, 5);
                        Particle spark = new GlowSparkParticle(Projectile.Center + new Vector2(-angle.X.DirectionalSign(), Main.rand.NextFloat(-0.05f, 0.05f)).RotatedBy(Projectile.rotation - 0.7f * Projectile.spriteDirection) * Main.rand.NextFloat(-20, -108) * Projectile.scale, compensatedSparkVel, false, sparkLifetime, sparkScale, Main.rand.NextBool() ? Color.Fuchsia : Color.HotPink, new Vector2(0.5f, 1.3f));
                        GeneralParticleHandler.SpawnParticle(spark);
                    }
                    break;
                case 2:
                    if (swingTimer == (int)(swingTime*0.5f))
                    {
                        if (Main.myPlayer == Projectile.owner)
                        {
                            for (var i = -1; i < 2; i++)
                            {
                                var p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center, -angle.RotatedBy(0.2f * i) * 16, ModContent.ProjectileType<DoGFire>(), Projectile.damage, Projectile.knockBack, player.whoAmI, 2);
                                if (Main.projectile.IndexInRange(p))
                                {
                                    Main.projectile[p].hostile = false;
                                    Main.projectile[p].friendly = true;
                                    Main.projectile[p].DamageType = DamageClass.Melee;
                                    Main.projectile[p].timeLeft = 120;
                                    Main.projectile[p].netUpdate = true;
                                }
                            }
                        }
                    }
                    if (inSwing)
                    {
                        var veloc = oldPlayerOffset - (Projectile.Center - Main.player[Projectile.owner].Center);
                        veloc.Normalize();
                        int sparkLifetime = Main.rand.Next(15, 23);

                        Vector2 sparkVel = Vector2.UnitY * -9f;
                        float maxRotationDeviance = 0.4f;
                        float rotationAngle = Main.rand.NextFloat(-maxRotationDeviance, maxRotationDeviance);
                        sparkVel = sparkVel.RotatedBy(rotationAngle) * Main.rand.NextFloat(0.3f, 1.0f);

                        float sparkScale = Main.rand.NextFloat(0.007f, 0.015f);

                        Vector2 compensatedSparkVel = veloc.RotatedBy(MathHelper.PiOver4 * 0.5f * Projectile.spriteDirection) * Main.rand.NextFloat(2, 5);
                        Particle spark = new GlowSparkParticle(Projectile.Center + new Vector2(-angle.X.DirectionalSign(), Main.rand.NextFloat(-0.05f, 0.05f)).RotatedBy(Projectile.rotation - 0.7f * Projectile.spriteDirection) * Main.rand.NextFloat(20, 108) * Projectile.scale, compensatedSparkVel, false, sparkLifetime, sparkScale, Main.rand.NextBool() ? Color.Cyan : Color.Aqua, new Vector2(0.5f, 1.3f));
                        GeneralParticleHandler.SpawnParticle(spark);
                    }
                    break;
                case 0:
                    if (Main.myPlayer == Projectile.owner)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center, -angle * 3, ModContent.ProjectileType<MawOfInfinityJaws>(), Projectile.damage * 2, Projectile.knockBack, player.whoAmI);
                        player.itemAnimation = BaseItem.useAnimation;
                        player.itemTime = BaseItem.useTime;
                    Projectile.Kill();
                    return;
            }
        }

        public override float SwingFunction()
        {
            if (inStartup)
                return MathHelper.ToRadians(MathHelper.SmoothStep(-swingWidth * 0.6f, -swingWidth * 0.5f, MathF.Pow(StartupCompletion, 2f)));
            if (inCooldown)
                return MathHelper.ToRadians(MathHelper.Lerp(swingWidth * 0.5f, swingWidth * 0.6f, 1 - MathF.Pow(1 - CooldownCompletion, 2f)));
            return MathHelper.ToRadians(MathHelper.SmoothStep(-swingWidth * 0.5f, swingWidth * 0.5f, SwingCompletion));
        }

        public override void PostDraw(Color lightColor)
        {
            var player = Main.player[Projectile.owner];
            var modplayer = player.GetModPlayer<BaseSwordHoldoutPlayer>();
            if (modplayer.swingNum == 0)
                return;
            var tex = ModContent.Request<Texture2D>("CalamityMod/Particles/Jaws").Value;
            float jawScaleMult = 1f;
            if (inStartup)
                jawScaleMult = StartupCompletion;
            if (inCooldown)
                jawScaleMult = 1-CooldownCompletion;
            jawScaleMult = MathF.Pow(jawScaleMult, 3);
            float rotation = Projectile.rotation - (Projectile.spriteDirection == -1 ? MathHelper.PiOver2: 0);
            Vector2 DrawPos = Projectile.Center + Projectile.scale * new Vector2(50,10 * Projectile.spriteDirection).RotatedBy(Projectile.rotation - MathHelper.PiOver2 + (Projectile.spriteDirection == -1 ? -MathHelper.PiOver4 : MathHelper.PiOver4));
            
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            Main.spriteBatch.Draw(tex, DrawPos - Main.screenPosition, tex.Frame(2,1,0,0), modplayer.swingNum == 1 ? Color.Fuchsia : Color.Cyan, rotation + MathHelper.PiOver4, new Vector2(tex.Width * 0.25f,tex.Height * 0.5f), Projectile.scale * jawScaleMult, Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
        }
    }
}
