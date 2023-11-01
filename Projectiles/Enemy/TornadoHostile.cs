using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Enemy
{
    public class TornadoHostile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Enemy";
        public override string Texture => "CalamityMod/Projectiles/TornadoProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1200;
        }

        public override void AI()
        {
            float projTimer = Projectile.ai[1] == 1f ? 900f : 600f;
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = -1;
                SoundEngine.PlaySound(SoundID.Item122, Projectile.position);
            }
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= projTimer)
            {
                Projectile.Kill();
            }
            float expandSizeX = Projectile.ai[1] == 1f ? 90f : 15f;
            float expandSizeY = Projectile.ai[1] == 1f ? 90f : 15f;
            Point projCenterTile = Projectile.Center.ToTileCoordinates();
            int sizeMod;
            int sizeMod2;
            Collision.ExpandVertically(projCenterTile.X, projCenterTile.Y, out sizeMod, out sizeMod2, (int)expandSizeX, (int)expandSizeY);
            sizeMod++;
            sizeMod2--;
            Vector2 sizeModVector = new Vector2((float)projCenterTile.X, (float)sizeMod) * 16f + new Vector2(8f);
            Vector2 sizeModVector2 = new Vector2((float)projCenterTile.X, (float)sizeMod2) * 16f + new Vector2(8f);
            Vector2 centering = Vector2.Lerp(sizeModVector, sizeModVector2, 0.5f);
            Vector2 sizeModPos = new Vector2(0f, sizeModVector2.Y - sizeModVector.Y);
            sizeModPos.X = sizeModPos.Y * 0.2f;
            Projectile.width = (int)(sizeModPos.X * (Projectile.ai[1] == 1f ? 0.167f : 0.65f));
            Projectile.height = (int)sizeModPos.Y;
            Projectile.Center = centering;
            if (Projectile.owner == Main.myPlayer)
            {
                bool breakFlag = false;
                Vector2 playerCenter = Main.player[Projectile.owner].Center;
                Vector2 top = Main.player[Projectile.owner].Top;
                for (float i = 0f; i < 1f; i += 0.05f)
                {
                    Vector2 position2 = Vector2.Lerp(sizeModVector, sizeModVector2, i);
                    if (Collision.CanHitLine(position2, 0, 0, playerCenter, 0, 0) || Collision.CanHitLine(position2, 0, 0, top, 0, 0))
                    {
                        breakFlag = true;
                        break;
                    }
                }
                if (!breakFlag && Projectile.ai[0] < projTimer - 120f)
                {
                    float aiDecrement = Projectile.ai[0] % 60f;
                    Projectile.ai[0] = projTimer - 120f + aiDecrement;
                    Projectile.netUpdate = true;
                }
            }
            if (Projectile.ai[0] < projTimer - 120f)
            {
                for (int j = 0; j < 1; j++)
                {
                    float lerpRandomizer = Main.rand.NextFloat();
                    Vector2 dustVelocity = new Vector2(MathHelper.Lerp(0.1f, 1f, Main.rand.NextFloat()), MathHelper.Lerp(-0.5f, 0.9f, lerpRandomizer));
                    dustVelocity.X *= MathHelper.Lerp(2.2f, 0.6f, lerpRandomizer);
                    dustVelocity.X *= -1f;
                    Vector2 dustCustomData = new Vector2(6f, 10f);
                    Vector2 dustPosition = centering + sizeModPos * dustVelocity * 0.5f + dustCustomData;
                    Dust cloudDust = Main.dust[Dust.NewDust(dustPosition, 0, 0, 16, 0f, 0f, 0, default, 1.5f)];
                    cloudDust.position = dustPosition;
                    cloudDust.customData = centering + dustCustomData;
                    cloudDust.fadeIn = 1f;
                    cloudDust.scale = 0.3f;
                    if (dustVelocity.X > -1.2f)
                    {
                        cloudDust.velocity.X = 1f + Main.rand.NextFloat();
                    }
                    cloudDust.velocity.Y = Main.rand.NextFloat() * -0.5f - 1f;
                }
            }
        }

        public override bool CanHitPlayer(Player target) => Projectile.ai[0] >= 60f && Projectile.ai[0] <= (Projectile.ai[1] == 1f ? 840f : 540f);

        public override bool PreDraw(ref Color lightColor)
        {
            float aiTrackCheck = Projectile.ai[1] == 1f ? 900f : 600f;
            float expandSizeX2 = Projectile.ai[1] == 1f ? 90f : 15f;
            float expandSizeY2 = Projectile.ai[1] == 1f ? 90f : 15f;
            float aiTracker = Projectile.ai[0];
            float trackerClamp = MathHelper.Clamp(aiTracker / 30f, 0f, 1f);
            if (aiTracker > aiTrackCheck - 60f)
            {
                trackerClamp = MathHelper.Lerp(1f, 0f, (aiTracker - (aiTrackCheck - 60f)) / 60f);
            }
            Point centerPoint = Projectile.Center.ToTileCoordinates();
            int sizeModding;
            int sizeModding2;
            Collision.ExpandVertically(centerPoint.X, centerPoint.Y, out sizeModding, out sizeModding2, (int)expandSizeX2, (int)expandSizeY2);
            sizeModding++;
            sizeModding2--;
            float vectorMult = 0.2f;
            Vector2 sizeModdingVector = new Vector2((float)centerPoint.X, (float)sizeModding) * 16f + new Vector2(8f);
            Vector2 sizeModdingVector2 = new Vector2((float)centerPoint.X, (float)sizeModding2) * 16f + new Vector2(8f);
            Vector2.Lerp(sizeModdingVector, sizeModdingVector2, 0.5f);
            Vector2 sizeModdingPos = new Vector2(0f, sizeModdingVector2.Y - sizeModdingVector.Y);
            sizeModdingPos.X = sizeModdingPos.Y * vectorMult;
            new Vector2(sizeModdingVector.X - sizeModdingPos.X / 2f, sizeModdingVector.Y);
            Texture2D texture2D23 = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle drawRectangle = texture2D23.Frame(1, 1, 0, 0);
            Vector2 smallRect = drawRectangle.Size() / 2f;
            float aiTrackMult = -0.06283186f * aiTracker;
            Vector2 spinningpoint2 = Vector2.UnitY.RotatedBy((double)(aiTracker * 0.1f), default);
            float incrementStorage = 0f;
            float increment = 5.1f;
            Color cloudColor = new Color(225, 225, 225);
            for (float k = (float)(int)sizeModdingVector2.Y; k > (float)(int)sizeModdingVector.Y; k -= increment)
            {
                incrementStorage += increment;
                float colorChanger = incrementStorage / sizeModdingPos.Y;
                float incStorageMult = incrementStorage * 6.28318548f / -20f;
                float lowerColorChanger = colorChanger - 0.15f;
                Vector2 spinArea = spinningpoint2.RotatedBy((double)incStorageMult, default);
                Vector2 colorChangeVector = new Vector2(0f, colorChanger + 1f);
                colorChangeVector.X = colorChangeVector.Y * vectorMult;
                Color newCloudColor = Color.Lerp(Color.Transparent, cloudColor, colorChanger * 2f);
                if (colorChanger > 0.5f)
                {
                    newCloudColor = Color.Lerp(Color.Transparent, cloudColor, 2f - colorChanger * 2f);
                }
                newCloudColor.A = (byte)((float)newCloudColor.A * 0.5f);
                newCloudColor *= trackerClamp;
                spinArea *= colorChangeVector * 100f;
                spinArea.Y = 0f;
                spinArea.X = 0f;
                spinArea += new Vector2(sizeModdingVector2.X, k) - Main.screenPosition;
                Main.spriteBatch.Draw(texture2D23, spinArea, new Microsoft.Xna.Framework.Rectangle?(drawRectangle), newCloudColor, aiTrackMult + incStorageMult, smallRect, 1f + lowerColorChanger, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}
