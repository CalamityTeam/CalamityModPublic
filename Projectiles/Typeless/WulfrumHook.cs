using System;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using CalamityMod.Items.Materials;
using Terraria.DataStructures;
using ReLogic.Utilities;
using CalamityMod.Items.Accessories;

namespace CalamityMod.Projectiles.Typeless
{
    public class WulfrumHook : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];

        public HookState State
        {
            get => (HookState)(int)Projectile.ai[0];
            set { Projectile.ai[0] = (int)value; }
        }

        public ref float Timer => ref Projectile.ai[1];

        public enum HookState
        {
            Thrown,
            Retracting,
            Unused, //Fuck off vanilla code. This makes it so that i can put this projectile in the player grapple list while also never having it considered as "grappling" (aka ai[0] = 2)
            Grappling
        }

        public static float MaxReach = 600;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Grappling Hook");
        }

        public override void SetDefaults()
        {
            Projectile.width = 3;
            Projectile.height = 3;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false;
            //Projectile.aiStyle = 7; Can i set its ai style to 7 wihle also making it not do anthing related to ai 7 whatsoever and use my own custom hook ai?
            //That would bne great because fsr the onyl way to distinguish if a projectile is a hook or not is by checking its ai style.
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.DeepSkyBlue.ToVector3());
            Vector2 BetweenOwner = Owner.Center - Projectile.Center;

            if (Owner.dead || Owner.stoned || Owner.webbed || Owner.frozen || BetweenOwner.Length() > 2500)
            {
                Projectile.Kill();
                return;
            }

            Projectile.rotation = BetweenOwner.ToRotation() - MathHelper.PiOver2;

            if (Owner.GetModPlayer<WulfrumPackPlayer>().WulfrumPackEquipped)
            {
                Projectile.timeLeft = 2;
                Owner.GetModPlayer<WulfrumPackPlayer>().Grapple = Projectile.whoAmI;
            }

            if (State == HookState.Thrown)
            {
                //Retract if too far.
                if (MaxReach < BetweenOwner.Length())
                    State = HookState.Retracting;

                float fallSpeed = Projectile.velocity.Y;

                if (Timer > 15)
                    Projectile.velocity += Vector2.UnitY * 0.5f * (1 - Math.Clamp((Timer - 15) / 35f, 0f, 1f));

                Projectile.velocity *= 0.98f;

                if (Projectile.velocity.Y > 0)
                    Projectile.velocity.Y = Math.Clamp(Projectile.velocity.Y, 0, Math.Max(18f, fallSpeed));


                CheckForGrapplableTiles();
            }

            else if (State == HookState.Retracting)
            {
                Projectile.velocity = BetweenOwner.SafeNormalize(Vector2.One) * 13f;
                Projectile.Center += Vector2.UnitY * 0.5f;

                if (BetweenOwner.Length() < 25f)
                    Projectile.Kill();
            }

            else
            {
                Point tilePos = Projectile.Center.ToTileCoordinates();
                Tile tile = Main.tile[tilePos];
                if (!tile.HasUnactuatedTile || !tile.CanTileBeLatchedOnTo() || Owner.IsBlacklistedForGrappling(tilePos))
                    State = HookState.Retracting;

                Projectile.velocity = Vector2.Zero;

                if (Owner.grapCount < 10)
                {
                    Owner.grappling[Owner.grapCount] = Projectile.whoAmI;
                    Owner.grapCount++;
                }
            }

            Timer++;
        }

        public void CheckForGrapplableTiles()
        {
            Vector2 hitboxStart = Projectile.Center - new Vector2(5f);
            Vector2 hitboxEnd = Projectile.Center + new Vector2(5f);

            Point topLeftTile = (hitboxStart - new Vector2(16f)).ToTileCoordinates();
            Point bottomRightTile = (hitboxEnd + new Vector2(32f)).ToTileCoordinates();

            for (int x = topLeftTile.X; x < bottomRightTile.X; x++)
            {
                for (int y = topLeftTile.Y; y < bottomRightTile.Y; y++)
                {
                    Vector2 worldPos = new Vector2(x * 16f, y * 16f);
                    Point tilePos = new Point(x, y);

                    //Ignore tiles that arent being collided with
                    if (!(hitboxStart.X + 10f > worldPos.X) || !(hitboxStart.X < worldPos.X + 16f) || !(hitboxStart.Y + 10f > worldPos.Y) || !(hitboxStart.Y < worldPos.Y + 16f))
                        continue;

                    Tile tile = Main.tile[tilePos];

                    if (!tile.HasUnactuatedTile || !tile.CanTileBeLatchedOnTo() || Owner.IsBlacklistedForGrappling(tilePos))
                        continue;
                    if (Main.myPlayer != Owner.whoAmI)
                        continue;

                    //Hook onto the tile
                    Projectile.velocity = Vector2.Zero;
                    State = HookState.Grappling;
                    Projectile.Center = worldPos + Vector2.One * 8f;
                    //effects
                    WorldGen.KillTile(x, y, fail: true, effectOnly: true);
                    SoundEngine.PlaySound(SoundID.Dig, worldPos);
                    
                    if (Owner.grapCount < 10)
                    {
                        Owner.grappling[Owner.grapCount] = Projectile.whoAmI;
                        Owner.grapCount++;
                        //Owner.velocity = Vector2.Zero;
                    }

                    WulfrumPackPlayer mp = Owner.GetModPlayer<WulfrumPackPlayer>();

                    mp.SwingLenght = (Owner.Center - Projectile.Center).Length();
                    mp.OldPosition = Owner.Center - Owner.velocity;
                    mp.SetSegments(Projectile.Center);


                    Rectangle? tileVisualHitbox = WorldGen.GetTileVisualHitbox(x, y);
                    if (tileVisualHitbox.HasValue)
                        Projectile.Center = tileVisualHitbox.Value.Center.ToVector2();

                    Projectile.netUpdate = true;
                    NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, Owner.whoAmI);
                    break;
                }

                if (State == HookState.Grappling)
                    break;
            }
        }

        public override void Kill(int timeLeft)
        {
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() / 2f, Projectile.scale, 0, 0);
            return false;
        }
    }
}
