using System;
using CalamityMod.Buffs.Pets;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Pets
{
    public class Beldum : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Pets";
        public override void SetStaticDefaults()
        {
            Main.projPet[Type] = true;
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.CharacterPreviewAnimations[Type] = ProjectileID.Sets.SimpleLoop(0, 0, 1)
            .WithOffset(-8f, -20f).WithSpriteDirection(-1).WhenNotSelected(0, 0);
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 beldumcenter = Projectile.Center;
            Vector2 vectorbeldum = player.Center - beldumcenter;
            float playerdistance = vectorbeldum.Length();
            if (!player.active)
            {
                Projectile.active = false;
                return;
            }

            //Delete the projectile if the player doesnt have the buff or is very far away (dunno if this needs to be deleted)
            if (!player.HasBuff(ModContent.BuffType<BeldumBuff>()) || playerdistance >= 4000f)
            {
                Projectile.Kill();
            }

            CalamityPlayer modPlayer = player.Calamity();
            if (player.dead)
            {
                modPlayer.beldum = false;
            }
            if (modPlayer.beldum)
            {
                Projectile.timeLeft = 2;
            }

            if (player.InInteractionRange((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16, TileReachCheckSettings.Simple))
            {
                if (Projectile.Hitbox.Contains(player.ClampedMouseWorld().ToPoint()))
                {
                    if (player.controlUseItem && player.velocity == Vector2.Zero)
                    {
                        Projectile.ai[1] = Projectile.ai[1] == 1 ? 0 : 1;
                    }
                    else if (player.controlUseTile)
                    {
                        Projectile.ai[1] = Projectile.ai[1] == 2 ? 0 : 2;
                    }
                }
            }

            // Normal behaviour
            if (Projectile.ai[1] == 0)
            {
                Projectile.FloatingPetAI(false, 0);
            }
            // Being pet
            else if (Projectile.ai[1] == 1)
            {
                if (player.velocity != Vector2.Zero || player.HasIFrames())
                {
                    Projectile.ai[1] = 0;
                }
                else
                {
                    Projectile.Center = player.Center + new Vector2(40 * player.direction, 0);
                    Projectile.velocity = Vector2.Zero;
                    PlayerPetting();
                }
            }
            // Shoulder
            else
            {
                Projectile.Center = player.Center + new Vector2(0, -20 * player.gravDir) + Vector2.UnitY * player.gfxOffY;
            }

            if (Projectile.ai[1] != 2)
            {
                Projectile.ai[0]++;
                if (Projectile.ai[1] == 0)
                {
                    Projectile.spriteDirection = -Projectile.velocity.X.DirectionalSign();
                    Projectile.rotation = MathF.Sin(Projectile.ai[0] * 0.05f) * MathHelper.ToRadians(20);
                }
                else
                {
                    Projectile.spriteDirection = -Projectile.DirectionTo(player.Center).X.DirectionalSign();
                    Projectile.rotation = MathF.Sin(Projectile.ai[0] * 0.05f) * MathHelper.ToRadians(5);
                }
            }
            else
            {
                Projectile.ai[0] = 0;
                Projectile.rotation = 0;
                Projectile.spriteDirection = -player.direction;
            }

            if (Main.rand.NextBool(600) && Projectile.ai[2] == 0)
            {
                Projectile.ai[2] = 1;
            }

            if (Projectile.ai[2] == 1)
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter > 3)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                }

                if (Projectile.frame > 3)
                {
                    Projectile.frame = 0;
                    Projectile.ai[2] = 0;
                }
            }
        }

        public void PlayerPetting()
        {
            Player player = Main.player[Projectile.owner];
            int targetDirection = (Projectile.Center.X > player.Center.X) ? 1 : (-1);
            player.StopVanityActions();
            player.ChangeDir(targetDirection);
            player.gravDir = 1f;
            int completion = player.miscCounter % 14 / 7;
            Player.CompositeArmStretchAmount stretch = Player.CompositeArmStretchAmount.ThreeQuarters;
            if (completion == 1)
            {
                stretch = Player.CompositeArmStretchAmount.Full;
            }
            player.SetCompositeArmBack(enabled: true, stretch, player.DirectionTo(Projectile.Center).ToRotation() - MathHelper.PiOver2);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            SpriteEffects fx = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (Main.player[Projectile.owner].gravDir == -1)
                fx |= SpriteEffects.FlipVertically;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition + Vector2.UnitY * MathF.Cos(Projectile.ai[0] * 0.05f) * (Projectile.ai[1] == 0 ? 10 : 2), tex.Frame(1, 4, 0, Projectile.frame), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(tex.Width / 2, tex.Height / Main.projFrames[Type] / 2), Projectile.scale, fx);
            return false;
        }
    }
}
