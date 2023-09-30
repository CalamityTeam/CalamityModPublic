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

namespace CalamityMod.Projectiles.Typeless
{
    public class WulfrumDiggingTurtleProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public static readonly SoundStyle IdleSound = new("CalamityMod/Sounds/Custom/WulfrumSawIdle") { IsLooped = true, Volume = 0.8f, MaxInstances = 0 };
        public static readonly SoundStyle CuttingSound = new("CalamityMod/Sounds/Custom/WulfrumSawCutting") { IsLooped = true, Volume = 0.7f, MaxInstances = 0 };
        public static readonly SoundStyle BreakingSound = new("CalamityMod/Sounds/Custom/WulfrumMachineBreak");

        private SlotId CuttingSoundSlot;
        private SlotId IdlingSoundSlot;

        public Player Owner => Main.player[Projectile.owner];
        public bool Diggging
        {
            get => Projectile.ai[0] == 1;
            set { Projectile.ai[0] = value ? 1f : 0f; }
        }

        public bool HasDug
        {
            get => Projectile.ai[1] == 1;
            set { Projectile.ai[1] = value ? 1f : 0f; }
        }

        public float CuttingVolume
        {
            get => Projectile.localAI[0];
            set { Projectile.localAI[0] = Math.Clamp(value, 0f, 1f); }
        }

        public override string Texture => "CalamityMod/Items/Tools/WulfrumDiggingTurtle";
        public static Texture2D SmallGearTexture;
        public static Texture2D GearTexture;

        public static int Lifetime = 400;
        public static int DigTime = 350;
        public static float DigSpeed = 1.5f;
        public static int MaxPickPower = 160;
        public static float ClearSpaceDiagonal = 50;

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = Lifetime;
            Projectile.netImportant = true;
        }

        public override bool? CanDamage() => false;

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!HasDug)
            {
                HasDug = true;
                Projectile.timeLeft = DigTime;
            }

            Diggging = true;
            Projectile.velocity = oldVelocity.SafeNormalize(Vector2.UnitY) * DigSpeed;

            return false;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.GreenYellow.ToVector3());

            //Idle chainsaw sounds
            if ((!SoundEngine.TryGetActiveSound(IdlingSoundSlot, out var idleSoundOut)))
                IdlingSoundSlot = SoundEngine.PlaySound(IdleSound with { Volume = IdleSound.Volume }, Projectile.Center);
            else if (idleSoundOut != null)
            {
                idleSoundOut.Volume = (1 - CuttingVolume);
                idleSoundOut.Position = Projectile.Center;
            }

            
            //Heavy cutting sound
            if ((!SoundEngine.TryGetActiveSound(CuttingSoundSlot, out var cuttingSoundOut)))
                CuttingSoundSlot = SoundEngine.PlaySound(CuttingSound with { Volume = CuttingSound.Volume  }, Projectile.Center);
            else if (cuttingSoundOut != null)
            {
                cuttingSoundOut.Volume = CuttingVolume;
                cuttingSoundOut.Position = Projectile.Center;
            }
            


            if (Diggging)
            {
                CuttingVolume += 0.1f;
                for (int i = -1; i <= 1; i++)
                {
                    Point tilePos = (Projectile.Center + (Projectile.rotation + MathHelper.PiOver4 * i).ToRotationVector2() * 16).ToTileCoordinates();

                    DigTile(tilePos.X, tilePos.Y);
                }
            }

            //the flung state.
            else
            {
                CuttingVolume -= 0.1f;

                float fallSpeed = Projectile.velocity.Y;
                
                if (Projectile.timeLeft < 345)
                    Projectile.velocity += Vector2.UnitY * 0.5f * (1 - Math.Clamp((Projectile.timeLeft - 310f) / 35f, 0f, 1f));

                Projectile.velocity *= 0.98f;

                if (Projectile.velocity.Y > 0)
                    Projectile.velocity.Y = Math.Clamp(Projectile.velocity.Y, 0,  Math.Max(18f, fallSpeed));

                Projectile.rotation = Projectile.velocity.ToRotation();
            }

            if (!Collision.SolidCollision(Projectile.Center - Vector2.One * ClearSpaceDiagonal * 0.5f, (int)ClearSpaceDiagonal, (int)ClearSpaceDiagonal))
                Diggging = false;
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.myPlayer == Owner.whoAmI)
            {
                if (Main.rand.NextBool() && !Projectile.noDropItem)
                    Item.NewItem(Projectile.GetSource_DropAsItem(), (int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height, ModContent.ItemType<WulfrumMetalScrap>(), 1);
            }

            if (SoundEngine.TryGetActiveSound(CuttingSoundSlot, out var cuttingSoundOut))
                cuttingSoundOut.Stop();

            if (SoundEngine.TryGetActiveSound(IdlingSoundSlot, out var idleSoundOut))
                idleSoundOut.Stop();

            SoundEngine.PlaySound(BreakingSound, Projectile.position);

            int smokeCount = Main.rand.Next(5, 10);
            int sparkCount = Main.rand.Next(4, 8);

            for (int i = 0; i < smokeCount; i++)
            {
                Vector2 velocity = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(3f, 12f);

                Color smokeStart = Main.rand.NextBool() ? Color.GreenYellow : Color.Aqua;
                Color smokeEnd = new Color(60, 60, 60);

                float smokeSize = Main.rand.NextFloat(1.4f, 2.2f);

                Particle smoke = new SmallSmokeParticle(Projectile.Center, velocity, smokeStart, smokeEnd, smokeSize, 135 - Main.rand.Next(30));
                GeneralParticleHandler.SpawnParticle(smoke);
            }

            if (Main.netMode != NetmodeID.Server)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 shrapnelVelocity = Main.rand.NextVector2Circular(9f, 9f);
                    float shrapnelScale = Main.rand.NextFloat(0.8f, 1f);

                    string goreType = i < 2 ? "WulfrumTurtle1" : i < 3 ? "WulfrumTurtle2" : "WulfrumTurtle3";

                    Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, shrapnelVelocity, Mod.Find<ModGore>(goreType).Type, shrapnelScale);
                }


            }

            for (int i = 0; i < sparkCount; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, 226, Main.rand.NextVector2Circular(18f, 18f), Scale: Main.rand.NextFloat(0.4f, 1f));
            }
        }

        public void DigTile(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            if (!tile.HasTile)
                return;

            int pickPower = Math.Min( Owner.GetBestPickPower(), MaxPickPower);
            int pickaxeRequirement = tile.GetRequiredPickPower(x, y);

            bool true_ = true;
            bool false_ = false;

            bool canBreakTileCheck = TileLoader.CanKillTile(x, y, tile.TileType, ref true_) && TileLoader.CanKillTile(x, y, tile.TileType, ref false_);
            bool shouldBreakTile = tile.ShouldBeMined();

            if (!Owner.noBuilding && shouldBreakTile && pickaxeRequirement <= pickPower && canBreakTileCheck)
            {
                WorldGen.KillTile(x, y, false, false, false);
                if (!Main.tile[x, y].HasTile && Main.netMode != NetmodeID.SinglePlayer)
                {
                    NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, x, y, 0f, 0, 0, 0);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            GearTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/WulfrumDiggingTurtle_Gear").Value;

            SmallGearTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/WulfrumDiggingTurtle_SmallGear").Value;


            Vector2 position = Projectile.Center - Main.screenPosition;
            if (Diggging)
                position += Main.rand.NextVector2Circular(2f, 2f);
            float drawRotation = Projectile.rotation + MathHelper.PiOver2;

            for (int i = -1; i <= 1; i += 2)
            {
                Vector2 diggingGearOffset = new Vector2(9 * i, -11).RotatedBy(drawRotation) * Projectile.scale;
                Main.EntitySpriteDraw(SmallGearTexture, position + diggingGearOffset, null, lightColor, drawRotation + Main.GlobalTimeWrappedHourly * -10f * i, SmallGearTexture.Size() / 2f, Projectile.scale * 1.2f, 0, 0);
            }

            Vector2 largeGearOffset = new Vector2(0f, 3f).RotatedBy(drawRotation) * Projectile.scale;
            Main.EntitySpriteDraw(GearTexture, position + largeGearOffset, null, lightColor, Main.GlobalTimeWrappedHourly * 6f, GearTexture.Size() / 2f, Projectile.scale, 0, 0);

            Main.EntitySpriteDraw(texture, position, null, lightColor, drawRotation, texture.Size() / 2f, Projectile.scale, 0, 0);
            return false;
        }
    }
}
