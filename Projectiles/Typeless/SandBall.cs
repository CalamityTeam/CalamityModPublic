using System;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class AstralSandBallFalling : SandBall
    {
        public override string Texture => "CalamityMod/Projectiles/Typeless/SandBallAstral";
        public override bool Fired => false;
        public override int TileType => ModContent.TileType<Tiles.AstralDesert.AstralSand>();
        public override int ItemType => ModContent.ItemType<Items.Placeables.Astral.AstralSand>();
        public override int DustType => 108;
    }

    public class DunesandBallFalling : SandBall
    {
        public override string Texture => "CalamityMod/Projectiles/Typeless/SandBallDune";
        public override bool Fired => false;
        public override int TileType => ModContent.TileType<Tiles.SunkenSea.Dunesand>();
        public override int ItemType => ModContent.ItemType<Items.Placeables.SunkenSea.Dunesand>();
        public override int DustType => DustID.Hive;
    }

    public class PolypSandBallFalling : SandBall
    {
        public override string Texture => "CalamityMod/Projectiles/Typeless/SandBallPolyp";
        public override bool Fired => false;
        public override int TileType => ModContent.TileType<Tiles.SunkenSea.PolypSand>();
        public override int ItemType => ModContent.ItemType<Items.Placeables.SunkenSea.PolypSand>();
        public override int DustType => DustID.Ice_Red;
    }

    public class VolcanicSandBallFalling : SandBall
    {
        public override string Texture => "CalamityMod/Projectiles/Typeless/SandBallVolcanic";
        public override bool Fired => false;
        public override int TileType => ModContent.TileType<Tiles.SunkenSea.VolcanicSand>();
        public override int ItemType => ModContent.ItemType<Items.Placeables.SunkenSea.VolcanicSand>();
        public override int DustType => DustID.t_PearlWood;
    }

    public class WhitePearlFalling : SandBall
    {
        public override string Texture => "CalamityMod/Projectiles/Typeless/WhitePearl";
        public override bool Fired => false;
        public override int TileType => ModContent.TileType<Tiles.SunkenSea.WhitePearlPile>();
        public override int ItemType => ModContent.ItemType<Items.Placeables.SunkenSea.WhitePearlPile>();
        public override int DustType => DustID.Slush;
    }

    public class PinkPearlFalling : SandBall
    {
        public override string Texture => "CalamityMod/Projectiles/Typeless/PinkPearl";
        public override bool Fired => false;
        public override int TileType => ModContent.TileType<Tiles.SunkenSea.PinkPearlPile>();
        public override int ItemType => ModContent.ItemType<Items.Placeables.SunkenSea.PinkPearlPile>();
        public override int DustType => DustID.Ice_Pink;
    }

    public class BlackPearlFalling : SandBall
    {
        public override string Texture => "CalamityMod/Projectiles/Typeless/BlackPearl";
        public override bool Fired => false;
        public override int TileType => ModContent.TileType<Tiles.SunkenSea.BlackPearlPile>();
        public override int ItemType => ModContent.ItemType<Items.Placeables.SunkenSea.BlackPearlPile>();
        public override int DustType => DustID.Lead;
    }

    public class AstralSandBallGun : SandBall
    {
        public override string Texture => "CalamityMod/Projectiles/Typeless/SandBallAstral";
        public override int TileType => ModContent.TileType<Tiles.AstralDesert.AstralSand>();
        public override int ItemType => ModContent.ItemType<Items.Placeables.Astral.AstralSand>();
        public override int DustType => 108;

        //Doze: I'm giving our sandgun sands actual special effects because it's more interesting
        //This one hits only one target but splits on impact into seeking mini sands

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.penetrate = 1;
            Projectile.localNPCHitCooldown = 10;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.usesIDStaticNPCImmunity = false;
        }
        public override bool PreAI()
        {
            if (Projectile.ai[2] == 1)
            {
                Projectile.scale = 0.75f;
                if (Projectile.ai[1] == 0)
                {
                    Projectile.originalDamage = Projectile.damage;
                    Projectile.damage = 0;
                }
                if (Projectile.ai[1] == 30)
                {
                    Projectile.Calamity().conditionalHomingRange = 600;
                    Projectile.damage = Projectile.originalDamage;
                }
            }
            return base.PreAI();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[2] != 1)
            {
                for (var i = 0; i < 4; i++)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.RotatedByRandom(0.75f) * Main.rand.NextFloat(0.5f,0.75f), ModContent.ProjectileType<AstralSandBallGun>(), (int)(Projectile.damage), Projectile.knockBack, Projectile.owner, ai2: 1);
            }
        }

        public override bool PreKill(int timeLeft)
        {
            if (Projectile.penetrate == 0)
                Projectile.active = false;
            return Projectile.active;
        }

    }

    public class EutrophicSandBallGun : SandBall
    {
        public override string Texture => "CalamityMod/Projectiles/Typeless/SandBallEutrophic";
        public override int TileType => ModContent.TileType<Tiles.SunkenSea.EutrophicSand>();
        public override int ItemType => ModContent.ItemType<Items.Placeables.SunkenSea.EutrophicSand>();
        public override bool DropAsItem => true;
        public override int DustType => 108; // Weirdly same dusts as Astral

        //Doze: I'm giving our sandgun sands actual special effects because it's more interesting
        //This one hits only one target and does 5 less damage, but inflicts a relatively long stun debuff
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.penetrate = 1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Eutrophication>(), 300);
        }
    }

    public class SulphurousSandBallGun : SandBall
    {
        public override string Texture => "CalamityMod/Projectiles/Typeless/SandBallSulphurous";
        public override int TileType => ModContent.TileType<Tiles.Abyss.SulphurousSand>();
        public override int ItemType => ModContent.ItemType<Items.Placeables.Abyss.SulphurousSand>();
        public override bool DropAsItem => true;
        // Uses normal sand dust

        //Doze: I'm giving our sandgun sands actual special effects because it's more interesting
        //This one hits only two targets but inflicts Irradiated
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.penetrate = 2;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 150);
        }
    }
    public class DuneSandBallGun : SandBall
    {
        public override string Texture => "CalamityMod/Projectiles/Typeless/SandBallDune";
        public override int TileType => ModContent.TileType<Tiles.SunkenSea.Dunesand>();
        public override int ItemType => ModContent.ItemType<Items.Placeables.SunkenSea.Dunesand>();
        public override int DustType => DustID.Sand;


        //Doze: I'm giving our sandgun sands actual special effects because it's more interesting
        //This one has much less velocity and no gravity for the first 3 seconds, pierces infinitely, and does 5 extra damage
        public override void AI()
        {
            if (Projectile.ai[1] == 0 )
            {
                Projectile.velocity *= 0.5f;
            }
            if (Main.rand.NextBool())
            {
                int i = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType);
                Main.dust[i].velocity.X *= 0.4f;
                Main.dust[i].velocity.Y += Fired ? 0f : Projectile.velocity.Y * 0.5f;
            }

            Projectile.ai[1]++;
            Projectile.rotation += 0.1f;
            if (Projectile.ai[1] >= 180f)
            {
                Projectile.ai[1] = 180f;
                Projectile.velocity.Y += 0.2f;
            }
            if (Projectile.velocity.Y > 10f)
                Projectile.velocity.Y = 10f;

            Point p = Projectile.Center.ToTileCoordinates();
            // Don't check out of bounds
            if (p.X < 0 || p.X >= Main.maxTilesX || p.Y < 0 || p.Y >= Main.maxTilesY)
                return;
            Tile placer = Main.tile[p.X, p.Y + 1];
            if (placer.HasTile && TileID.Sets.Platforms[placer.TileType] && Projectile.ai[1] >= 60f)
                Projectile.Kill();

        }
    }
    public class PolypSandBallGun : SandBall
    {
        public override string Texture => "CalamityMod/Projectiles/Typeless/SandBallPolyp";
        public override int TileType => ModContent.TileType<Tiles.SunkenSea.PolypSand>();
        public override int ItemType => ModContent.ItemType<Items.Placeables.SunkenSea.PolypSand>();
        public override int DustType => DustID.Sand;

        //Doze: I'm giving our sandgun sands actual special effects because it's more interesting
        //This one has a small homing range and can go through platforms
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = true;
            return true;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.penetrate = 1;
            Projectile.Calamity().conditionalHomingRange = 160;
        }
    }
    public class VolcanicSandBallGun : SandBall
    {
        public override string Texture => "CalamityMod/Projectiles/Typeless/SandBallVolcanic";
        public override int TileType => ModContent.TileType<Tiles.SunkenSea.VolcanicSand>();
        public override int ItemType => ModContent.ItemType<Items.Placeables.SunkenSea.VolcanicSand>();
        public override int DustType => -1;

        //Doze: I'm giving our sandgun sands actual special effects because it's more interesting
        //This one fires a shotgun of mini sands, but every sand deals less damage

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.usesIDStaticNPCImmunity = false;
        }
        public override bool PreAI()
        {
            if (Projectile.ai[2] == 1)
            {
                if (Projectile.ai[1] == 0)
                {
                    Projectile.scale = 0.75f;
                    Projectile.penetrate = 1;
                    Projectile.timeLeft = 30;
                }
            } else
            {
                if (Projectile.ai[1] == 0)
                {
                    if (Main.myPlayer == Projectile.owner)
                        for (var i = 0; i < 5; i++)
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.RotatedByRandom(0.5f) * 0.75f, ModContent.ProjectileType<VolcanicSandBallGun>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack, Projectile.owner, ai2: 1);
                    Projectile.damage = (int)(Projectile.damage * 0.5f);
                    
                }
            }
            return base.PreAI();
        }
        public override bool PreKill(int timeLeft)
        {
            if (Projectile.ai[2] == 1) //Baby sands should always vanish without placing blocks
                Projectile.active = false;
            return Projectile.active;
        }
    }

    // All the setups go here to prevent mass blocks of copypasting
    public abstract class SandBall : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";

        // Whether or not it is a fired projectile
        public virtual bool Fired => true;
        // Associated tile type
        public virtual int TileType => TileID.Sand;
        // Associated item type
        public virtual int ItemType => ItemID.SandBlock;
        // Associated dust type
        public virtual int DustType => DustID.Sand;

        /// <summary>
        /// Whether the sandball should drop as an item on death instead of placing a tile. This is for non-falling sands in sandgun.
        /// </summary>
        public virtual bool DropAsItem => false;

        public override void SetDefaults()
        {
            Projectile.knockBack = 6f;
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;

            if (Fired)
            {
                Projectile.MaxUpdates = 2;
                Projectile.DamageType = DamageClass.Ranged;
            }
            else
                Projectile.hostile = true;
        }

        // Using clones will not allow for custom dust types sadly
        public override void AI()
        {
            if (Main.rand.NextBool() && DustType >= 0)
            {
                int i = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType);
                Main.dust[i].velocity.X *= 0.4f;
                Main.dust[i].velocity.Y += Fired ? 0f : Projectile.velocity.Y * 0.5f;
            }

            Projectile.ai[1]++;
            Projectile.rotation += 0.1f;
            if (Projectile.ai[1] >= 60f || !Fired)
            {
                Projectile.ai[1] = 60f;
                Projectile.velocity.Y += 0.2f;
            }
            if (Projectile.velocity.Y > 10f)
                Projectile.velocity.Y = 10f;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override void OnKill(int timeLeft)
        {
            Point p = Projectile.Center.ToTileCoordinates();
            // If the sand is dying outside the world border, or having used all of it's pierce, cancel placing sand.
            if ( p.X < 0 || p.X >= Main.maxTilesX || p.Y < 0 || p.Y >= Main.maxTilesY)
                return;

            if (DropAsItem)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Item.NewItem(Projectile.GetSource_DropAsItem(), Projectile.position, Projectile.width, Projectile.height, ItemType);
                return;
            }
            Tile placer = Main.tile[p.X, p.Y];

            // If the sand hit a half brick, but was mostly going downwards (at a lower than 45 degree angle), then stack atop the half brick.
            if (placer.IsHalfBlock && Projectile.velocity.Y > 0f && Math.Abs(Projectile.velocity.Y) > Math.Abs(Projectile.velocity.X))
                placer = Main.tile[p.X, --p.Y];

            bool ValidTileBelow = true;
            bool SlopeTileBelow = false;

            // Attempt to place sand and unslope tile below if available
            // Under no circumstances can falling sand destroy minecart tracks.
            if (!placer.HasTile && placer.TileType != TileID.MinecartTrack)
            {
                if (p.Y + 1 < Main.maxTilesY)
                {
                    Tile under = Main.tile[p.X, p.Y + 1];
                    if (under.HasTile)
                    {
                        if (under.TileType == TileID.MinecartTrack)
                            ValidTileBelow = false;
                        else if (under.IsHalfBlock || under.Slope != 0)
                            SlopeTileBelow = true;
                    }
                }

                if (ValidTileBelow)
                {
                    bool PlacedBlock = WorldGen.PlaceTile(p.X, p.Y, TileType, false, true);
                    WorldGen.SquareTileFrame(p.X, p.Y);

                    if (PlacedBlock && SlopeTileBelow)
                    {
                        WorldGen.SlopeTile(p.X, p.Y + 1);
                        if (Main.netMode != NetmodeID.SinglePlayer)
                            NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 14, p.X, p.Y + 1);
                    }
                    if (PlacedBlock && Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 1, p.X, p.Y, TileType);
                }
            }
            // Give the block back if you literally can't place it
            else
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Item.NewItem(Projectile.GetSource_DropAsItem(), Projectile.position, Projectile.width, Projectile.height, ItemType);
        }
    }
}
