using CalamityMod.CalPlayer;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class AngelicAllianceArchangel : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        private int lifeSpan = 900;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 68;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void SendExtraAI(BinaryWriter writer) => writer.Write(lifeSpan);

        public override void ReceiveExtraAI(BinaryReader reader) => lifeSpan = reader.ReadInt32();

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            if (!modPlayer.divineBless || player.dead || !player.active)
            {
                lifeSpan = 0;
            }

            // Initialization and dust
            if (Projectile.localAI[0] == 0f)
            {
                for (int i = 0; i < 10; i++)
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, (int)CalamityDusts.ProfanedFire, 0f, 0f, 100, default, 2f);
                }
                Projectile.localAI[0] += 1f;
            }
            // Rotate around the player
            double deg = Projectile.ai[1];
            double rad = deg * (Math.PI / 180);
            double dist = 300;
            Projectile.position.X = player.Center.X - (int)(Math.Cos(rad) * dist) - Projectile.width / 2;
            Projectile.position.Y = player.Center.Y - (int)(Math.Sin(rad) * dist) - Projectile.height / 2;
            Projectile.ai[1] += 1f;

            if (!Projectile.FinalExtraUpdate())
                return;

            lifeSpan--;
            if (lifeSpan <= 0)
            {
                Projectile.alpha += 30;
                if (Projectile.alpha >= 255)
                {
                    Projectile.Kill();
                    return;
                }
            }

            // Give off some light
            float lightScalar = Main.rand.NextFloat(0.9f, 1.1f) * Main.essScale;
            Lighting.AddLight(Projectile.Center, 0.3f * lightScalar, 0.26f * lightScalar, 0.15f * lightScalar);

            // Get a target
            NPC target = Projectile.Center.MinionHoming(2000f, player, false, true);

            // Shoot the target
            if (target != null)
            {
                Vector2 direction = target.Center - Projectile.Center;
                direction.Normalize();
                direction *= 6f;
                if (direction.X >= 0.25f)
                {
                    Projectile.direction = -1;
                }
                else if (direction.X < -0.25f)
                {
                    Projectile.direction = 1;
                }
                Projectile.ai[0]++;
                int timerLimit = 180;
                if (Projectile.ai[0] > timerLimit && Projectile.alpha < 50)
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        int type = ModContent.ProjectileType<AngelRay>();
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, direction * 0.5f, type, Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                Vector2 direction = player.Center - Projectile.Center;
                direction.Normalize();
                direction *= 6f;
                if (direction.X >= 0.25f)
                {
                    Projectile.direction = -1;
                }
                else if (direction.X < -0.25f)
                {
                    Projectile.direction = 1;
                }
            }
            Projectile.spriteDirection = Projectile.direction;

            // Frames
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 7)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int frameY = frameHeight * Projectile.frame;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, frameY, texture.Width, frameHeight)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2((float)texture.Width / 2f, (float)frameHeight / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool? CanDamage() => false;
    }
}
