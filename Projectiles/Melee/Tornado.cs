using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Melee
{
    public class Tornado : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/TornadoProj";

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {
            //only 3 tornado can exist at a time
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
                if (projCount > 3)
                {
                    Main.projectile[oldestTornado].netUpdate = true;
                    Main.projectile[oldestTornado].ai[0] = 36000f;
                    Main.projectile[oldestTornado].damage = 0;
                    return;
                }
            }

            float projTimer = 900f;
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = -1;
                SoundEngine.PlaySound(SoundID.Item122, Projectile.Center);
            }
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= projTimer)
            {
                Projectile.Kill();
            }
            if (Projectile.localAI[0] >= 30f)
            {
                Projectile.damage = 0;
                if (Projectile.ai[0] < projTimer - 120f)
                {
                    float timeModulo = Projectile.ai[0] % 60f;
                    Projectile.ai[0] = projTimer - 120f + timeModulo;
                    Projectile.netUpdate = true;
                }
            }
            float projX = Projectile.Center.X;
            float projY = Projectile.Center.Y;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy(Projectile, false) && Collision.CanHit(Projectile.Center, 1, 1, npc.Center, 1, 1) && !CalamityPlayer.areThereAnyDamnBosses)
                {
                    float npcCenterX = npc.position.X + (float)(npc.width / 2);
                    float npcCenterY = npc.position.Y + (float)(npc.height / 2);
                    float npcDist = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - npcCenterX) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - npcCenterY);
                    if (npcDist < 600f)
                    {
                        if (npc.position.X < projX)
                        {
                            npc.velocity.X += 0.02f;
                        }
                        else
                        {
                            npc.velocity.X -= 0.02f;
                        }
                        if (npc.position.Y < projY)
                        {
                            npc.velocity.Y += 0.02f;
                        }
                        else
                        {
                            npc.velocity.Y -= 0.02f;
                        }
                    }
                }
            }
            Point projCenterTile = Projectile.Center.ToTileCoordinates();
            int sizeMod;
            int sizeMod2;
            Collision.ExpandVertically(projCenterTile.X, projCenterTile.Y, out sizeMod, out sizeMod2, 15, 15);
            sizeMod++;
            sizeMod2--;
            Vector2 sizeModVector = new Vector2((float)projCenterTile.X, (float)sizeMod) * 16f + new Vector2(8f);
            Vector2 sizeModVector2 = new Vector2((float)projCenterTile.X, (float)sizeMod2) * 16f + new Vector2(8f);
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
                if (!breakFlag && Projectile.ai[0] < projTimer - 120f)
                {
                    float aiDecrement = Projectile.ai[0] % 60f;
                    Projectile.ai[0] = projTimer - 120f + aiDecrement;
                    Projectile.netUpdate = true;
                }
            }
            if (Projectile.ai[0] < projTimer - 120f)
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
            Color cloudColor = new Color(225, 225, 225);
            for (float j = (float)(int)sizeModdingVector2.Y; j > (float)(int)sizeModdingVector.Y; j -= increment)
            {
                incrementStorage += increment;
                float colorChanger = incrementStorage / sizeModdingPos.Y;
                float incStorageMult = incrementStorage * 6.28318548f / -20f;
                float lowerColorChanger = colorChanger - 0.15f;
                Vector2 spinArea = spinningpoint.RotatedBy((double)incStorageMult, default);
                Vector2 colorChangeVector = new Vector2(0f, colorChanger + 1f);
                colorChangeVector.X = colorChangeVector.Y * vectorMult;
                Color newCloudColor = Microsoft.Xna.Framework.Color.Lerp(Microsoft.Xna.Framework.Color.Transparent, cloudColor, colorChanger * 2f);
                if (colorChanger > 0.5f)
                {
                    newCloudColor = Microsoft.Xna.Framework.Color.Lerp(Microsoft.Xna.Framework.Color.Transparent, cloudColor, 2f - colorChanger * 2f);
                }
                newCloudColor.A = (byte)((float)newCloudColor.A * 0.5f);
                newCloudColor *= trackerClamp;
                spinArea *= colorChangeVector * 100f;
                spinArea.Y = 0f;
                spinArea.X = 0f;
                spinArea += new Vector2(sizeModdingVector2.X, j) - Main.screenPosition;
                Main.spriteBatch.Draw(texture2D23, spinArea, new Microsoft.Xna.Framework.Rectangle?(drawRectangle), newCloudColor, aiTrackMult + incStorageMult, smallRect, 1f + lowerColorChanger, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}
