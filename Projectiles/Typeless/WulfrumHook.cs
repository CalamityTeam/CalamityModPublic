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
using System.Collections.Generic;
using System.Linq;
using CalamityMod.Graphics.Primitives;

namespace CalamityMod.Projectiles.Typeless
{
    public class WulfrumHook : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
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
            Grappling = 3 //Making this value "3" is important here, as it makes it so that i can put this projectile in the player grapple list while also never having it considered as "grappling" (aka ai[0] = 2)
        }

        public static float MaxReach = 600;

        public override void SetStaticDefaults()
        {
            //Expand the draw distance. Should never happen really , but just in case the player basically walks away from the hook.
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 3000;
        }

        public override void SetDefaults()
        {
            Projectile.width = 3;
            Projectile.height = 3;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = WulfrumPackPlayer.HookUpdates;
            Projectile.netImportant = true;
            Projectile.aiStyle = ProjAIStyleID.Hook; //The projectile uses entirely custom AI, but for some reason terraria's only way to distinguish what is and isnt a hook is its ai style.
        }

        public override bool? CanDamage() => false;

        public override bool PreAI() => false;
        public override void PostAI()
        {
            Lighting.AddLight(Projectile.Center, Color.DeepSkyBlue.ToVector3());
            Vector2 BetweenOwner = Owner.Center - Projectile.Center;

            if (Owner.dead || Owner.stoned || Owner.webbed || Owner.frozen || BetweenOwner.Length() > 1500)
            {
                Projectile.Kill();
                return;
            }

            Projectile.rotation = BetweenOwner.ToRotation() - MathHelper.PiOver2;

            if (Owner.GetModPlayer<WulfrumPackPlayer>().WulfrumPackEquipped)
            {
                Projectile.timeLeft = 2;
            }

            if (State == HookState.Thrown)
            {
                //Retract if too far.
                if (MaxReach < BetweenOwner.Length())
                    State = HookState.Retracting;

                float fallSpeed = Projectile.velocity.Y;

                if (Timer > 15 * Projectile.extraUpdates)
                    Projectile.velocity += Vector2.UnitY * 0.5f * (1 - Math.Clamp((Timer - 15) / 35f, 0f, 1f)) / Projectile.extraUpdates;

                Projectile.velocity *= 0.98f;

                if (Projectile.velocity.Y + 0.001 > 0)
                    Projectile.velocity.Y = Math.Clamp(Projectile.velocity.Y, 0, Math.Max(18f, fallSpeed));

                if (Projectile.velocity.Length() < 1f)
                    State = HookState.Retracting;

                CheckForGrapplableTiles();

            }

            else if (State == HookState.Retracting)
            {
                Projectile.velocity = BetweenOwner.SafeNormalize(Vector2.One) * WulfrumPackPlayer.ReturnVelocity;
                Projectile.Center += Vector2.UnitY * 0.5f;

                if (BetweenOwner.Length() < 25f)
                    Projectile.Kill();
            }

            else
            {
                float LengthToOwner = BetweenOwner.Length();

                if (LengthToOwner > Owner.GetModPlayer<WulfrumPackPlayer>().SwingLength + 60f)
                {
                    State = HookState.Retracting;
                }

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

                    OnGrapple(worldPos, x, y);
                    
                    break;
                }

                if (State == HookState.Grappling)
                    break;
            }
        }

        public void OnGrapple(Vector2 grapplePos, int x, int y)
        {
            WulfrumPackPlayer mp = Owner.GetModPlayer<WulfrumPackPlayer>();
            //Clear previous grapple
            if (Main.projectile[mp.Grapple].active && Main.projectile[mp.Grapple].ModProjectile is WulfrumHook hook && hook.State == WulfrumHook.HookState.Grappling)
                Main.projectile[mp.Grapple].Kill();

            //Hook onto the tile
            Projectile.velocity = Vector2.Zero;
            State = HookState.Grappling;
            Projectile.Center = grapplePos + Vector2.One * 8f;
            //effects
            WorldGen.KillTile(x, y, fail: true, effectOnly: true);
            SoundEngine.PlaySound(SoundID.Dig, grapplePos);
            SoundEngine.PlaySound(WulfrumAcrobaticsPack.GrabSound, grapplePos);

            if (Owner.grapCount < 10)
            {
                Owner.grappling[Owner.grapCount] = Projectile.whoAmI;
                Owner.grapCount++;
                //Owner.velocity = Vector2.Zero;
            }

            mp.SwingLength = (Owner.Center - Projectile.Center).Length();
            mp.OldPosition = Owner.Center - Owner.velocity;
            mp.SetSegments(Projectile.Center);
            mp.Grapple = Projectile.whoAmI;

            Rectangle? tileVisualHitbox = WorldGen.GetTileVisualHitbox(x, y);
            if (tileVisualHitbox.HasValue)
                Projectile.Center = tileVisualHitbox.Value.Center.ToVector2();

            Projectile.netUpdate = true;
            NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, Owner.whoAmI);
        }


        public override void OnKill(int timeLeft)
        {
            if (Owner.grappling[0] == Projectile.whoAmI)
            {
                Owner.grappling[0] = -1;
                Owner.grapCount--;
            }
        }

        public float PrimWidthFunction(float completionRatio)
        {
            return 1.6f;
        }

        public Color PrimColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.DeepSkyBlue, Color.GreenYellow, (float)Math.Pow(completionRatio, 1.5D));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2[] segmentPositions = new Vector2[] {Projectile.Center, Owner.Center };

            if (State == HookState.Grappling)
                segmentPositions = Owner.GetModPlayer<WulfrumPackPlayer>().Segments.Select(x => x.position).ToArray();
            PrimitiveSet.Prepare(new List<Vector2>(segmentPositions) { Owner.Center }, new(PrimWidthFunction, PrimColorFunction), 30);

            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() / 2f, Projectile.scale, 0, 0);
            return false;
        }

        public override bool PreDrawExtras() => false; //Prevents vanilla chain drawing from taking place
    }
}
