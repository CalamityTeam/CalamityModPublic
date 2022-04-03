using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Melee
{
    public class CrystylCrusherRay : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Magic/YharimsCrystalBeam";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystyl Crusher Ray");
        }

        // Use a different style for constant so it is very clear in code when a constant is used

        // The maximum charge value
        private const float MAX_CHARGE = 100f;
        //The distance charge particle from the player center
        private const float MOVE_DISTANCE = 100f;

        // The actual distance is stored in the ai0 field
        // By making a property to handle this it makes our life easier, and the accessibility more readable
        public float Distance
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        // The actual charge value is stored in the localAI0 field
        public float Charge
        {
            get => Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }

        // Are we at max charge? With c#6 you can simply use => which indicates this is a get only property
        public bool IsAtMaxCharge => Charge == MAX_CHARGE;

        private bool playedSound = false;

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.scale = 1.5f;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.hide = true;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // We start drawing the laser if we have charged up
            if (IsAtMaxCharge)
            {
                Vector2 maxLength = Main.MouseWorld - Main.player[Projectile.owner].Center;

                DrawLaser(ModContent.Request<Texture2D>(Texture).Value, Main.player[Projectile.owner].Center,
                    Projectile.velocity, 15f, Projectile.damage, -1.57f, Projectile.scale, maxLength.Length(), new Color(Main.DiscoR, 0, 255), (int)MOVE_DISTANCE);
            }
            return false;
        }

        // The core function of drawing a laser
        public void DrawLaser(Texture2D texture, Vector2 start, Vector2 unit, float step, int damage, float rotation = 0f, float scale = 1f, float maxDist = 2000f, Color color = default(Color), int transDist = 50)
        {
            float r = unit.ToRotation() + rotation;

            // Draws the laser 'body'
            for (float i = transDist; i <= Distance; i += step)
            {
                Color c = new Color(Main.DiscoR, 0, 255);
                var origin = start + i * unit;
                Main.EntitySpriteDraw(texture, origin - Main.screenPosition,
                    new Rectangle(0, 26, 28, 26), i < transDist ? Color.Transparent : c, r,
                    new Vector2(28 * .5f, 26 * .5f), scale, 0, 0);
            }

            // Draws the laser 'tail'
            Main.EntitySpriteDraw(texture, start + unit * (transDist - step) - Main.screenPosition,
                new Rectangle(0, 0, 28, 26), new Color(Main.DiscoR, 0, 255), r, new Vector2(28 * .5f, 26 * .5f), scale, 0, 0);

            // Draws the laser 'head'
            Main.EntitySpriteDraw(texture, start + (Distance + step) * unit - Main.screenPosition,
                new Rectangle(0, 52, 28, 26), new Color(Main.DiscoR, 0, 255), r, new Vector2(28 * .5f, 26 * .5f), scale, 0, 0);
        }

        // Change the way of collision check of the projectile
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // We can only collide if we are at max charge, which is when the laser is actually fired
            if (!IsAtMaxCharge) return false;

            Player player = Main.player[Projectile.owner];
            Vector2 unit = Projectile.velocity;
            float point = 0f;
            // Run an AABB versus Line check to look for collisions, look up AABB collision first to see how it works
            // It will look for collisions on the given line using AABB
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), player.Center,
                player.Center + unit * Distance, 22, ref point);
        }

        // The AI of the projectile
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (Projectile.timeLeft == 300)
                SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Item/CrystylCharge"), player.Center);

            Projectile.position = player.Center + Projectile.velocity * MOVE_DISTANCE;
            Projectile.timeLeft = 2;

            // By separating large AI into methods it becomes very easy to see the flow of the AI in a broader sense
            // First we update player variables that are needed to channel the laser
            // Then we run our charging laser logic
            // If we are fully charged, we proceed to update the laser's position

            UpdatePlayer(player);
            ChargeLaser(player);

            // If laser is not charged yet, stop the AI here.
            if (Charge < MAX_CHARGE) return;

            //Play cool sound when fully charged
            if (playedSound == false)
            {
                SoundEngine.PlaySound(SoundID.Item68, Projectile.Center);
                playedSound = true;
            }

            SetLaserPosition(player);
            CastLights();
            DestroyTiles();
        }

        /*
         * Sets the end of the laser position based on where it collides with something
         */
        private void SetLaserPosition(Player player)
        {
            Distance = MathHelper.Max(Main.player[Projectile.owner].Distance(Main.MouseWorld) - 20f, MOVE_DISTANCE + 5f);
        }

        private void ChargeLaser(Player player)
        {
            // Kill the projectile if the player stops channeling
            if (!player.channel)
            {
                Projectile.Kill();
            }
            else
            {
                Vector2 offset = Projectile.velocity;
                offset *= MOVE_DISTANCE - 20;
                Vector2 pos = player.Center + offset - new Vector2(15f, 15f);
                if (Charge < MAX_CHARGE)
                {
                    Charge++;
                }
                int chargeFact = (int)(Charge / 20f);
                Vector2 dustVelocity = Vector2.UnitX * 18f;
                dustVelocity = dustVelocity.RotatedBy(Projectile.rotation - 1.57f);
                Vector2 spawnPos = Projectile.Center + dustVelocity;
                for (int k = 0; k < chargeFact + 1; k++)
                {
                    int dustType = Main.rand.Next(3);
                    switch (dustType)
                    {
                        case 0:
                            dustType = 173;
                            break;
                        case 1:
                            dustType = 57;
                            break;
                        case 2:
                            dustType = 58;
                            break;
                        default:
                            break;
                    }
                    Vector2 spawn = spawnPos + ((float)Main.rand.NextDouble() * 6.28f).ToRotationVector2() * (12f - chargeFact * 2);
                    Dust dust = Main.dust[Dust.NewDust(pos, 20, 20, dustType, Projectile.velocity.X / 2f, Projectile.velocity.Y / 2f)];
                    dust.velocity = Vector2.Normalize(spawnPos - spawn) * 1.5f * (10f - chargeFact * 2f) / 10f;
                    dust.noGravity = true;
                    dust.color = new Color(Main.DiscoR, 0, 255);
                    dust.scale = Main.rand.Next(10, 20) * 0.05f;
                }
            }
        }

        private void UpdatePlayer(Player player)
        {
            // Multiplayer support here, only run this code if the client running it is the owner of the projectile
            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 diff = Main.MouseWorld - player.Center;
                diff.Normalize();
                Projectile.velocity = diff;
                Projectile.direction = Main.MouseWorld.X > player.position.X ? 1 : -1;
                Projectile.netUpdate = true;
            }
            int dir = Projectile.direction;
            player.ChangeDir(dir); // Set player direction to where we are shooting
            player.heldProj = Projectile.whoAmI; // Update player's held projectile
            player.itemTime = 2; // Set item time to 2 frames while we are used
            player.itemAnimation = 2; // Set item animation time to 2 frames while we are used
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * dir, Projectile.velocity.X * dir); // Set the item rotation to where we are shooting
        }

        private void CastLights()
        {
            // Cast a light along the line of the laser
            DelegateMethods.v3_1 = new Vector3(0.8f, 0.8f, 1f);
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * (Distance - MOVE_DISTANCE), 26, DelegateMethods.CastLight);
        }

        public override bool ShouldUpdatePosition() => false;

        /*
         * Update CutTiles so the laser will cut tiles (like grass)
         */
        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 unit = Projectile.velocity;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + unit * Distance, Projectile.width + 16, DelegateMethods.CutTiles);
        }

        private void DestroyTiles()
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 destroyVector = Projectile.Center + Projectile.velocity * (Distance - MOVE_DISTANCE);
                int num814 = 3;
                int num815 = (int)(destroyVector.X / 16f - num814);
                int num816 = (int)(destroyVector.X / 16f + num814);
                int num817 = (int)(destroyVector.Y / 16f - num814);
                int num818 = (int)(destroyVector.Y / 16f + num814);
                if (num815 < 0)
                {
                    num815 = 0;
                }
                if (num816 > Main.maxTilesX)
                {
                    num816 = Main.maxTilesX;
                }
                if (num817 < 0)
                {
                    num817 = 0;
                }
                if (num818 > Main.maxTilesY)
                {
                    num818 = Main.maxTilesY;
                }
                AchievementsHelper.CurrentlyMining = true;
                for (int num824 = num815; num824 <= num816; num824++)
                {
                    for (int num825 = num817; num825 <= num818; num825++)
                    {
                        float num826 = Math.Abs(num824 - destroyVector.X / 16f);
                        float num827 = Math.Abs(num825 - destroyVector.Y / 16f);
                        double num828 = Math.Sqrt(num826 * num826 + num827 * num827);
                        if (num828 < num814)
                        {
                            if (Main.tile[num824, num825] != null && Main.tile[num824, num825].HasTile)
                            {
                                WorldGen.KillTile(num824, num825, false, false, false);
                                DustExplosion(destroyVector);
                                if (!Main.tile[num824, num825].HasTile && Main.netMode != NetmodeID.SinglePlayer)
                                {
                                    NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, num824, num825, 0f, 0, 0, 0);
                                }
                            }
                        }
                    }
                }
                AchievementsHelper.CurrentlyMining = false;
            }
        }

        private void DustExplosion(Vector2 vector)
        {
            int num226 = 12;
            for (int num227 = 0; num227 < num226; num227++)
            {
                int dustType = Main.rand.Next(3);
                switch (dustType)
                {
                    case 0:
                        dustType = 173;
                        break;
                    case 1:
                        dustType = 57;
                        break;
                    case 2:
                        dustType = 58;
                        break;
                    default:
                        break;
                }
                Vector2 vector6 = Vector2.Normalize(Projectile.velocity) * new Vector2(Projectile.width / 2f, Projectile.height) * 0.75f;
                vector6 = vector6.RotatedBy((num227 - (num226 / 2 - 1)) * MathHelper.TwoPi / num226, default) + vector;
                Vector2 vector7 = vector6 - vector;
                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, dustType, vector7.X * 0.5f, vector7.Y * 0.5f, 100, new Color(Main.DiscoR, 0, 255), 1f);
                Main.dust[num228].noGravity = true;
                Main.dust[num228].noLight = true;
                Main.dust[num228].velocity = vector7;
            }
        }
    }
}
