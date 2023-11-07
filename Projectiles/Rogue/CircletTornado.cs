using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class CircletTornado : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Projectiles/TornadoProj";

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1200;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        public override void AI()
        {
            //only 1 tornado can exist at a time
            Projectile.localAI[1] += 1f;
            if (Projectile.localAI[1] >= 10f)
            {
                Projectile.localAI[1] = 0f;
                int projCount = 0;
                int oldestTornado = 0;
                float tornadoAge = 0f;
                int projType = Projectile.type;
                for (int projIndex = 0; projIndex < Main.maxProjectiles; projIndex++)
                {
                    Projectile proj = Main.projectile[projIndex];
                    if (proj.active && proj.owner == Projectile.owner && proj.type == projType && proj.ai[0] < 900f)
                    {
                        projCount++;
                        if (proj.ai[0] > tornadoAge)
                        {
                            oldestTornado = projIndex;
                            tornadoAge = proj.ai[0];
                        }
                    }
                }
                if (projCount > 1)
                {
                    Main.projectile[oldestTornado].netUpdate = true;
                    Main.projectile[oldestTornado].ai[0] = 36000f;
                    return;
                }
            }

            float lifeSpan = 900f;
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = -1;
                SoundEngine.PlaySound(SoundID.Item122, Projectile.Center);
            }
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= lifeSpan)
            {
                Projectile.Kill();
            }
            if (Projectile.localAI[0] >= 30f)
            {
                Projectile.damage = 0;
                if (Projectile.ai[0] < lifeSpan - 120f)
                {
                    float aiDecrement = Projectile.ai[0] % 60f;
                    Projectile.ai[0] = lifeSpan - 120f + aiDecrement;
                    Projectile.netUpdate = true;
                }
            }
            Point point8 = Projectile.Center.ToTileCoordinates();
            int sizeMod;
            int sizeMod2;
            Collision.ExpandVertically(point8.X, point8.Y, out sizeMod, out sizeMod2, 15, 15);
            sizeMod++;
            sizeMod2--;
            Vector2 sizeModVector = new Vector2((float)point8.X, (float)sizeMod) * 16f + new Vector2(8f);
            Vector2 sizeModVector2 = new Vector2((float)point8.X, (float)sizeMod2) * 16f + new Vector2(8f);
            Vector2 centering = Vector2.Lerp(sizeModVector, sizeModVector2, 0.5f);
            Vector2 sizeModPos = new Vector2(0f, sizeModVector2.Y - sizeModVector.Y);
            sizeModPos.X = sizeModPos.Y * 0.2f;
            Projectile.width = (int)(sizeModPos.X * 0.65f);
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
                if (!breakFlag && Projectile.ai[0] < lifeSpan - 120f)
                {
                    float aiDecrement2 = Projectile.ai[0] % 60f;
                    Projectile.ai[0] = lifeSpan - 120f + aiDecrement2;
                    Projectile.netUpdate = true;
                }
            }
            if (Projectile.ai[0] < lifeSpan - 120f)
            {
                return;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float aiTracker = Projectile.ai[0];
            float trackerClamp = MathHelper.Clamp(aiTracker / 30f, 0f, 1f);
            if (aiTracker > 540f)
            {
                trackerClamp = MathHelper.Lerp(1f, 0f, (aiTracker - 540f) / 60f);
            }
            Point centerPoint = Projectile.Center.ToTileCoordinates();
            int sizeModding;
            int sizeModding2;
            Collision.ExpandVertically(centerPoint.X, centerPoint.Y, out sizeModding, out sizeModding2, 15, 15);
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
            Vector2 spinningpoint = Vector2.UnitY.RotatedBy((double)(aiTracker * 0.1f), default);
            float incrementStorage = 0f;
            float increment = 5.1f;
            Color sandYellow = new Color(225, 225, 100);
            for (float k = (float)(int)sizeModdingVector2.Y; k > (float)(int)sizeModdingVector.Y; k -= increment)
            {
                incrementStorage += increment;
                float colorChanger = incrementStorage / sizeModdingPos.Y;
                float incStorageMult = incrementStorage * 6.28318548f / -20f;
                float lowerColorChanger = colorChanger - 0.15f;
                Vector2 spinArea = spinningpoint.RotatedBy((double)incStorageMult, default);
                Vector2 colorChangeVector = new Vector2(0f, colorChanger + 1f);
                colorChangeVector.X = colorChangeVector.Y * vectorMult;
                Color newSandYellow = Microsoft.Xna.Framework.Color.Lerp(Microsoft.Xna.Framework.Color.Transparent, sandYellow, colorChanger * 2f);
                if (colorChanger > 0.5f)
                {
                    newSandYellow = Microsoft.Xna.Framework.Color.Lerp(Microsoft.Xna.Framework.Color.Transparent, sandYellow, 2f - colorChanger * 2f);
                }
                newSandYellow.A = (byte)((float)newSandYellow.A * 0.5f);
                newSandYellow *= trackerClamp;
                spinArea *= colorChangeVector * 100f;
                spinArea.Y = 0f;
                spinArea.X = 0f;
                spinArea += new Vector2(sizeModdingVector2.X, k) - Main.screenPosition;
                Main.spriteBatch.Draw(texture2D23, spinArea, new Microsoft.Xna.Framework.Rectangle?(drawRectangle), newSandYellow, aiTrackMult + incStorageMult, smallRect, 1f + lowerColorChanger, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}
