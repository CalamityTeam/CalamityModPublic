using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameInput;
using Terraria.DataStructures;
using CalamityMod.Projectiles.Typeless;
using static Terraria.ModLoader.ModContent;
using System.Linq;
using Microsoft.Xna.Framework;
using CalamityMod.DataStructures;
using System.Collections.Generic;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Back)]
    public class WulfrumAcrobaticsPack : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Wulfrum Acrobatics Pack");
            Tooltip.SetDefault("Rebrands a broken winch into a feature\n" +
                "8% increased movement speed");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 22;
            Item.value = Item.buyPrice(0, 1, 0, 0);
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.moveSpeed += 0.8f;
            player.GetModPlayer<WulfrumPackPlayer>().WulfrumPackEquipped = true;
            player.GetModPlayer<WulfrumPackPlayer>().PackItem = Item;
        }
    }

    public class WulfrumPackProjectile : GlobalProjectile
    {
        public override bool? CanUseGrapple(int type, Player player)
        {
            if (Main.projectile.Any(n => n.active && n.owner == player.whoAmI && n.type == ProjectileType<WulfrumHook>()))
                return false;

            return base.CanUseGrapple(type, player);
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            Player owner = Main.player[projectile.owner];

            if (projectile.aiStyle == 7 && owner.GetModPlayer<WulfrumPackPlayer>().WulfrumPackEquipped)
            {
                if (Main.myPlayer == projectile.owner)
                    Projectile.NewProjectile(source, owner.Center, projectile.velocity, ProjectileType<WulfrumHook>(), 0, 0, projectile.owner);

                projectile.active = false;
            }
        }
    }

    public class WulfrumPackPlayer : ModPlayer
    {
        public bool WulfrumPackEquipped = false;
        public Item PackItem = null;
        public bool AutoGrappleActivated => !Player.noFallDmg;
        public int Grapple = 0;
        public float SwingLenght = 0f;

        public Vector2 CurrentPosition;
        public Vector2 OldPosition;
        public List<VerletSimulatedSegment> Segments;

        public static int SimulationResolution = 10;

        public override void ResetEffects()
        {
            WulfrumPackEquipped = false;
            PackItem = null;
        }

        public override void PreUpdateMovement()
        {
            if (WulfrumPackEquipped && Main.projectile[Grapple].active && Main.projectile[Grapple].ModProjectile is WulfrumHook hook && hook.State == WulfrumHook.HookState.Grappling)
                SimulateMovement(Main.projectile[Grapple]);
        }

        public void SetSegments(Vector2 endPoint)
        {
            SimulationResolution = 3;

            if (Segments == null)
                Segments = new List<VerletSimulatedSegment>();

            Segments.Clear();

            for (int i = 0; i <= SimulationResolution; i++)
            {
                float progress = i / (float)SimulationResolution;
                VerletSimulatedSegment segment = new VerletSimulatedSegment(Vector2.Lerp(endPoint, Player.Center, progress));
                if (i == 0)
                    segment.locked = true;

                if (i == SimulationResolution)
                    segment.oldPosition = Player.oldPosition + new Vector2(Player.width, Player.height) * 0.5f;

                Segments.Add(segment);
            }
        }


        public void SimulateMovement(Projectile grapple)
        {
            Segments = VerletSimulatedSegment.SimpleSimulation(Segments, SwingLenght / SimulationResolution, 50);

            /*
            //Manually apply gravity to the player.
            CurrentPosition += CurrentPosition - OldPosition;
            CurrentPosition += Vector2.UnitY * 1.3f;

            CurrentPosition = grapple.Center + (CurrentPosition - grapple.Center).SafeNormalize(Vector2.Zero) * SwingLenght;
            */

            Vector2 CurrentPosition;

            foreach (VerletSimulatedSegment position in Segments)
            {
                CurrentPosition = position.position;
                Dust doost = Dust.NewDustPerfect(CurrentPosition, 1, Vector2.Zero);
                doost.noGravity = true;
            }

            CurrentPosition = Segments[SimulationResolution].position;

            Player.velocity = CurrentPosition - Player.Center;

            Segments[SimulationResolution].oldPosition = Player.Center;
            Segments[SimulationResolution].position = Player.Center + Player.velocity;
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (triggersSet.Grapple)
            {
                bool grappleFound = false;

                for (int i = 0; i < Main.maxProjectiles; ++i)
                {
                    Projectile p = Main.projectile[i];
                    if (!p.active || p.owner != Player.whoAmI || p.aiStyle != 7)
                        continue;

                    p.Kill();
                    grappleFound = true;
                }

                if (!grappleFound && !Main.projectile.Any(n => n.active && n.owner == Player.whoAmI && n.type == ProjectileType<WulfrumHook>()))
                {
                    Vector2 velocity = (Main.MouseWorld - Player.Center).SafeNormalize(Vector2.One) * 30f;
                    Projectile.NewProjectile(Player.GetSource_ItemUse(PackItem), Player.Center, velocity, ProjectileType<WulfrumHook>(), 0, 0, Player.whoAmI);
                    //Launch grapple
                }
            }

            if (triggersSet.Jump && Player.releaseJump)
            {
                for (int i = 0; i < Main.maxProjectiles; ++i)
                {
                    Projectile p = Main.projectile[i];
                    if (!p.active || p.owner != Player.whoAmI || p.type != ProjectileType<WulfrumHook>())
                        continue;

                    p.Kill();
                }

                Player.jump = 0;
            }
        }
    }
}
