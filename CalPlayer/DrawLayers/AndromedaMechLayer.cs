using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.DrawLayers
{
    public class AndromedaMechLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.BackAcc);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow != 0f)
                return false;

            return drawInfo.drawPlayer.Calamity().andromedaState != AndromedaPlayerState.Inactive;
        }

        public static void DrawTheStupidFuckingRobot(ref PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            drawInfo.hidesBottomSkin = true;
            drawInfo.hidesTopSkin = true;
            drawInfo.armorHidesArms = true;
            drawInfo.armorHidesHands = true;
            drawInfo.cShield = 0;
            drawInfo.hideCompositeShoulders = true;

            // Clear all old draw data and draw the robot on top.
            drawInfo.DrawDataCache.Clear();

            int robot = -1;
            int andromedaMechID = ModContent.ProjectileType<GiantIbanRobotOfDoom>();
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].type == andromedaMechID && Main.projectile[i].owner == drawPlayer.whoAmI)
                {
                    robot = i;
                    break;
                }
            }
            if (robot == -1)
            {
                drawPlayer.Calamity().andromedaState = AndromedaPlayerState.Inactive;
                return;
            }

            SpriteEffects direction = Main.projectile[robot].spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (drawPlayer.gravDir == -1f)
                direction |= SpriteEffects.FlipVertically;

            GiantIbanRobotOfDoom robotEntityInstance = (GiantIbanRobotOfDoom)Main.projectile[robot].ModProjectile;
            switch (drawPlayer.Calamity().andromedaState)
            {
                case AndromedaPlayerState.SpecialAttack:
                    Texture2D dashTexture = ModContent.Request<Texture2D>("CalamityMod/CalPlayer/DrawLayers/AndromedaBolt").Value;
                    Rectangle frame = dashTexture.Frame(1, 4, 0, robotEntityInstance.RightIconCooldown / 4 % 4);

                    DrawData drawData = new DrawData(dashTexture,
                                     drawPlayer.Center + new Vector2(0f, drawPlayer.gravDir * -8f) - Main.screenPosition,
                                     frame,
                                     Color.White,
                                     Main.projectile[robot].rotation,
                                     drawPlayer.Size / 2,
                                     1f,
                                     direction,
                                     1);
                    drawData.shader = drawPlayer.cBody;

                    drawInfo.DrawDataCache.Add(drawData);
                    break;
                case AndromedaPlayerState.LargeRobot:
                    Texture2D robotTexture = ModContent.Request<Texture2D>(robotEntityInstance.Texture).Value;
                    frame = new Rectangle(robotEntityInstance.FrameX * robotTexture.Width / 3, robotEntityInstance.FrameY * robotTexture.Height / 7, robotTexture.Width / 3, robotTexture.Height / 7);

                    drawData = new DrawData(ModContent.Request<Texture2D>(Main.projectile[robot].ModProjectile.Texture).Value,
                                     Main.projectile[robot].Center + Vector2.UnitY * drawPlayer.gravDir * 6f - Main.screenPosition,
                                     frame,
                                     Color.White,
                                     Main.projectile[robot].rotation,
                                     Main.projectile[robot].Size / 2,
                                     1f,
                                     direction,
                                     1);
                    drawData.shader = drawPlayer.cBody;

                    drawInfo.DrawDataCache.Add(drawData);
                    break;
                case AndromedaPlayerState.SmallRobot:
                    robotTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/AndromedaSmall").Value;
                    frame = new Rectangle(0, robotEntityInstance.CurrentFrame * 54, robotTexture.Width, robotTexture.Height / 21);
                    drawData = new DrawData(robotTexture,
                                     drawPlayer.Center + new Vector2(drawPlayer.direction == 1 ? -24 : -10, drawPlayer.gravDir * -8f) - Main.screenPosition,
                                     frame,
                                     Color.White,
                                     0f,
                                     drawPlayer.Size / 2,
                                     1f,
                                     direction,
                                     1);
                    drawData.shader = drawPlayer.cBody;

                    drawInfo.DrawDataCache.Add(drawData);
                    break;
            }
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            DrawTheStupidFuckingRobot(ref drawInfo);
        }
    }
}
