using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class DarkSparkPrism : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Items/Weapons/Magic/DarkSpark";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.NeedsUUID[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            float piConditional = 0f;
            if (Projectile.spriteDirection == -1)
                piConditional = MathHelper.Pi;

            Vector2 playerRotation = player.RotatedRelativePoint(player.MountedCenter);

            float hitCooldown = 30f;
            if (Projectile.ai[0] > 360f)
                hitCooldown = 15f;
            if (Projectile.ai[0] > 480f)
                hitCooldown = 5f;

            Projectile.damage = player.ActiveItem() is null ? 0 : player.GetWeaponDamage(player.ActiveItem());

            Projectile.ai[0] += 1f;
            Projectile.ai[1] += 1f;

            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                Projectile.localAI[1] = 255f;
            }

            if (Projectile.localAI[1] > 0f)
                Projectile.localAI[1] -= 1f;

            bool shouldHitNotCharged = false;
            if (Projectile.ai[0] % hitCooldown == 0f)
                shouldHitNotCharged = true;

            bool shouldHitChargedUp = false;
            if (Projectile.ai[0] % hitCooldown == 0f)
                shouldHitChargedUp = true;

            if (Projectile.ai[1] >= 1f)
            {
                Projectile.ai[1] = 0f;
                shouldHitChargedUp = true;

                if (Main.myPlayer == Projectile.owner)
                {
                    float scaleFactor5 = player.ActiveItem().shootSpeed * Projectile.scale;
                    Vector2 projRotation = playerRotation;
                    Vector2 gravityAdjustedRotation = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY) - projRotation;
                    if (player.gravDir == -1f)
                        gravityAdjustedRotation.Y = Main.screenHeight - Main.mouseY + Main.screenPosition.Y - projRotation.Y;

                    Vector2 prismDirection = Vector2.Normalize(gravityAdjustedRotation);
                    if (float.IsNaN(prismDirection.X) || float.IsNaN(prismDirection.Y))
                        prismDirection = -Vector2.UnitY;

                    prismDirection = Vector2.Normalize(Vector2.Lerp(prismDirection, Vector2.Normalize(Projectile.velocity), 0.92f));
                    prismDirection *= scaleFactor5;
                    if (prismDirection.X != Projectile.velocity.X || prismDirection.Y != Projectile.velocity.Y)
                        Projectile.netUpdate = true;

                    Projectile.velocity = prismDirection;
                }
            }

            Projectile.frameCounter++;
            int framing = (Projectile.ai[0] < 480f) ? 3 : 1;
            if (Projectile.frameCounter >= framing)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                    Projectile.frame = 0;
            }
            if (Projectile.soundDelay <= 0)
            {
                Projectile.soundDelay = 10;
                Projectile.soundDelay *= 2;
                if (Projectile.ai[0] != 1f)
                    SoundEngine.PlaySound(SoundID.Item15, Projectile.position);
            }

            if (shouldHitChargedUp && Main.myPlayer == Projectile.owner)
            {
                bool hasMana = !shouldHitNotCharged || player.CheckMana(player.ActiveItem(), -1, true, false);
                bool canUseItem = player.channel && hasMana && !player.noItems && !player.CCed;
                if (canUseItem)
                {
                    if (Projectile.ai[0] == 1f)
                    {
                        Vector2 projCenter = Projectile.Center;
                        Vector2 beamDirection = Vector2.Normalize(Projectile.velocity);
                        if (float.IsNaN(beamDirection.X) || float.IsNaN(beamDirection.Y))
                            beamDirection = -Vector2.UnitY;

                        for (int l = 0; l < 7; l++)
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), projCenter, beamDirection, ModContent.ProjectileType<DarkSparkBeam>(), Projectile.damage, Projectile.knockBack, Projectile.owner, l, Projectile.GetByUUID(Projectile.owner, Projectile.whoAmI));

                        Projectile.netUpdate = true;
                    }
                }
                else
                    Projectile.Kill();
            }

            Projectile.position = player.RotatedRelativePoint(player.MountedCenter, true) - Projectile.Size / 2f;
            Projectile.rotation = Projectile.velocity.ToRotation() + piConditional;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 mountedCenter = Main.player[Projectile.owner].MountedCenter;
            Color prismColorArea = Lighting.GetColor((int)((double)Projectile.position.X + (double)Projectile.width * 0.5) / 16, (int)(((double)Projectile.position.Y + (double)Projectile.height * 0.5) / 16.0));
            if (Projectile.hide && !ProjectileID.Sets.DontAttachHideToAlpha[Projectile.type])
            {
                prismColorArea = Lighting.GetColor((int)mountedCenter.X / 16, (int)(mountedCenter.Y / 16f));
            }
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Texture2D texture2D14 = ModContent.Request<Texture2D>(Texture).Value;
            int framing = ModContent.Request<Texture2D>(Texture).Value.Height / Main.projFrames[Projectile.type];
            int y7 = framing * Projectile.frame;
            Vector2 drawStart = (Projectile.position + new Vector2((float)Projectile.width, (float)Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition).Floor();
            if (Main.player[Projectile.owner].shroomiteStealth && Main.player[Projectile.owner].inventory[Main.player[Projectile.owner].selectedItem].CountsAsClass<RangedDamageClass>())
            {
                float playerStealth = Main.player[Projectile.owner].stealth;
                if ((double)playerStealth < 0.03)
                {
                    playerStealth = 0.03f;
                }
                float arg_97B3_0 = (1f + playerStealth * 10f) / 11f;
                prismColorArea *= playerStealth;
            }
            if (Main.player[Projectile.owner].setVortex && Main.player[Projectile.owner].inventory[Main.player[Projectile.owner].selectedItem].CountsAsClass<RangedDamageClass>())
            {
                float playerStealthAgain = Main.player[Projectile.owner].stealth;
                if ((double)playerStealthAgain < 0.03)
                {
                    playerStealthAgain = 0.03f;
                }
                float arg_9854_0 = (1f + playerStealthAgain * 10f) / 11f;
                prismColorArea = prismColorArea.MultiplyRGBA(new Color(Vector4.Lerp(Vector4.One, new Vector4(0.16f, 0.12f, 0f, 0f), 1f - playerStealthAgain)));
            }
            Main.spriteBatch.Draw(texture2D14, drawStart, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y7, texture2D14.Width, framing)), Projectile.GetAlpha(prismColorArea), Projectile.rotation, new Vector2((float)texture2D14.Width / 2f, (float)framing / 2f), Projectile.scale, spriteEffects, 0);
            float scaleFactor2 = (float)Math.Cos((double)(6.28318548f * (Projectile.ai[0] / 120f))) * 2f + 2f;
            if (Projectile.ai[0] > 480f)
            {
                scaleFactor2 = 4f;
            }
            for (float i = 0f; i < 4f; i += 1f)
            {
                Main.spriteBatch.Draw(texture2D14, drawStart + Vector2.UnitY.RotatedBy((double)(i * 6.28318548f / 4f), default) * scaleFactor2, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y7, texture2D14.Width, framing)), Projectile.GetAlpha(prismColorArea).MultiplyRGBA(new Color(255, 255, 255, 0)) * 0.03f, Projectile.rotation, new Vector2((float)texture2D14.Width / 2f, (float)framing / 2f), Projectile.scale, spriteEffects, 0);
            }
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.ai[0] < 255f)
                return new Color((int)Projectile.ai[0], (int)Projectile.ai[0], (int)Projectile.ai[0], (int)Projectile.localAI[1]);
            else
                return new Color(255, 255, 255, 0);
        }
    }
}
