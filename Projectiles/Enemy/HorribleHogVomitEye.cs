using CalamityMod.Graphics.Metaballs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Enemy
{
    public class HorribleHogVomitEye : ModProjectile, ILocalizedModType
    {
        public static int PlayerSearchArea => 128;

        public ref float LockOnTime => ref Projectile.ai[0];

        public ref float Timer => ref Projectile.ai[1];

        public new string LocalizationCategory => "Projectiles.Enemy";

        public override string Texture => "Terraria/Images/NPC_" + NPCID.DemonEye;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 480;
            Projectile.scale = 0f;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
        }

        public override bool CanHitPlayer(Player target) => Projectile.scale > 0.15f;

        public override void AI()
        {
            Projectile.scale = MathHelper.Lerp(Projectile.scale, 0.75f, 0.125f);
            Projectile.ExpandHitboxBy(Projectile.scale);

            int targetIndex = Player.FindClosest(Projectile.Center, 1, 1);
            if (Main.player[targetIndex].dead || !Main.player.IndexInRange(targetIndex))
            {
                Projectile.velocity *= 1.12f;
                Projectile.tileCollide = true;
                Projectile.rotation = Projectile.rotation.AngleLerp(Projectile.velocity.ToRotation() + MathHelper.Pi, 0.15f);
                return;
            }

            Player target = Main.player[targetIndex];
            if (Timer >= LockOnTime)
            {
                if (Timer <= LockOnTime + 45)
                {
                    Projectile.velocity *= 0.94f;
                    Projectile.rotation = Projectile.rotation.AngleLerp(Projectile.AngleTo(target.Center) + MathHelper.Pi, 0.15f);
                }
                else
                {
                    float turnResistance = 30f;
                    Vector2 directionToPoint = target.Center - Projectile.Center;
                    directionToPoint *= 30f / directionToPoint.Length();
                    Vector2 idealVelocity = (Projectile.velocity * turnResistance + directionToPoint) / (turnResistance + 1f);
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, idealVelocity, 0.35f);

                    Projectile.rotation = Projectile.rotation.AngleLerp(Projectile.velocity.ToRotation() + MathHelper.Pi, 0.15f);
                    Projectile.tileCollide = true;
                }
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 4)
            {
                Projectile.frame++;
                Projectile.frame %= 2;
                Projectile.frameCounter = 0;
            }

            Timer++;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            int targetIndex = Player.FindClosest(Projectile.Center, 1, 1);
            Player target = Main.player[targetIndex];
            if (Projectile.Center.Y > target.Center.Y - 48f)
                fallThrough = false;

            return true;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);
            int bloodAmt = Main.rand.Next(14, 19);
            for (int i = 0; i < bloodAmt; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(8f, 8f);
                Dust.NewDust(Projectile.Center, 1, 1, DustID.Blood, velocity.X, velocity.Y, Scale: Main.rand.NextFloat(1.2f, 1.4f));
            }

            if (!Main.dedServ)
            {
                Gore.NewGore(Projectile.Center, Projectile.oldVelocity * -0.8f, 1, 0.75f);
                Gore.NewGore(Projectile.Center, Projectile.oldVelocity * -0.8f, 2, 0.75f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;

            if (CalamityClientConfig.Instance.Afterimages)
            {
                for (int i = 0; i < Projectile.oldPos.Length; ++i)
                {
                    Vector2 afterimageDrawPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
                    float trailLengthInterpolant = ((float)(Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length);
                    Color afterimgeColor = Color.Red * trailLengthInterpolant;

                    Main.spriteBatch.Draw(texture, afterimageDrawPosition, frame, Projectile.GetAlpha(afterimgeColor) * 0.6f, Projectile.oldRot[i], origin, Projectile.scale * trailLengthInterpolant * 1.25f, 0, 0f);
                }
            }

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, 0, 0f);
            return false;
        }
    }
}
