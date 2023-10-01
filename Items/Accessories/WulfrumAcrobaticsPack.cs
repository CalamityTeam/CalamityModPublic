using CalamityMod.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Back)]
    public class WulfrumAcrobaticsPack : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public static readonly SoundStyle ShootSound = new("CalamityMod/Sounds/Custom/WulfrumHookShoot") { Volume = 0.7f,  MaxInstances = 1, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest};
        public static readonly SoundStyle GrabSound = new("CalamityMod/Sounds/Custom/WulfrumHookGrapple") { Volume = 0.7f, MaxInstances = 1, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest };
        public static readonly SoundStyle ReleaseSound = new("CalamityMod/Sounds/Custom/WulfrumHookDisengage") { Volume = 0.7f, MaxInstances = 1, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest };


        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 22;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.moveSpeed += 0.08f;
            player.GetModPlayer<WulfrumPackPlayer>().WulfrumPackEquipped = true;
            player.GetModPlayer<WulfrumPackPlayer>().PackItem = Item;

            Lighting.AddLight(player.Center, Color.Lerp(Color.DeepSkyBlue,Color.GreenYellow, (float)Math.Sin( Main.GlobalTimeWrappedHourly * 2f) * 0.5f + 0.5f).ToVector3());
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Chain, 2).
                AddIngredient<WulfrumMetalScrap>(6).
                AddIngredient<EnergyCore>().
                AddTile(TileID.Anvils).
                Register();
        }
    }

    public class WulfrumPackProjectile : GlobalProjectile
    {
        public override bool? CanUseGrapple(int type, Player player)
        {
            //Player can shoot up to 2 wulfrum hooks, but only 2 is allowed to stay grappled.
            if (player.GetModPlayer<WulfrumPackPlayer>().WulfrumPackEquipped)
            {
                if (Main.projectile.Count(n => n.active && n.owner == player.whoAmI && n.type == ProjectileType<WulfrumHook>()) > 1)
                    return false;
            }

            //This should never happen. This is for the case in which the player shoots a hook without having the wulfrum pack equipped but somehow having a wulfrum hook out.
            //Under no real circumstances should this happen, given wulfrum hooks instantly get killed if the player doesn't have the wulfrum pack.
            else if (Main.projectile.Any(n => n.active && n.owner == player.whoAmI && n.type == ProjectileType<WulfrumHook>()))
                return false;

            return base.CanUseGrapple(type, player);
        }

        //Prevent players with a wulfrum pack to spawn any non-wulfrum hooks.
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            Player owner = Main.player[projectile.owner];
            if (projectile.aiStyle == 7 && projectile.type != ProjectileType<WulfrumHook>() && owner.GetModPlayer<WulfrumPackPlayer>().WulfrumPackEquipped && projectile.type != ProjectileID.TrackHook)
            {
                projectile.active = false;
            }
        }
    }

    public class WulfrumPackPlayer : ModPlayer
    {
        public bool WulfrumPackEquipped = false;
        public Item PackItem = null;
        public bool AutoGrappleActivated
        {
            get
            {
                if (!WulfrumPackEquipped || Grappled || //Ignore if player isnt wearing the grapple pack, or is already grappled.
                    Player.noFallDmg || Player.equippedWings != null || //Ignore if player can't take fall damage
                    Player.controlDown || //Ignore if player disables the auto grapple by holding down
                    Player.velocity.Y * Player.gravDir < 0 || //Ignore if not falling *down*
                    (Player.fallStart >= (int)(Player.position.Y / 16f) && Player.gravDir > 0) || (!(Player.fallStart <= (int)(Player.position.Y / 16f)) && Player.gravDir < 0) || //Ignore if the player is not falling below their last fall point
                    Player.mount.Active || //ignore if player is on a mount
                    Player.webbed || Player.stoned || Player.frozen || Player.vortexDebuff //Ignore if players movement is compromised
                    )
                    return false;

                return true;
            }
        }
        /// <summary>
        /// The index of the grapple projectile currently grappled.
        /// </summary>
        public int Grapple = 0;
        /// <summary>
        /// The length of the current rope. Determined when the grapple lands.
        /// </summary>
        public float SwingLength = 0f;
        /// <summary>
        /// Used when we need to store the hook between instructions.
        /// </summary>
        public int hookCache = -1;
        /// <summary>
        /// The cooldown is only set when firing a hook straight downwards
        /// </summary>
        public int hookCooldown = 0;
        /// <summary>
        /// Is the player grappled?
        /// </summary>
        public bool Grappled => WulfrumPackEquipped && Main.projectile[Grapple].active && Main.projectile[Grapple].ModProjectile is WulfrumHook hook && hook.State == WulfrumHook.HookState.Grappling;
        public bool GrappleMovementDisabled
        {
            get
            {
                if (!Grappled)
                    return false;

                if (!PlayerOnGround)
                    return false;

                if ((Player.Center - Main.projectile[Grapple].Center).Length() > SwingLength)
                    return false;

                return true;
            }
        }

        public Vector2 CurrentPosition;
        public Vector2 OldPosition;
        public List<VerletSimulatedSegment> Segments;

        public static int SimulationResolution = 3;
        public static int HookUpdates = 3;
        public static float GrappleVelocity = 17f;
        public static float ReturnVelocity = 5f;
        public static float MaxHopVelocity = 4f; //The maximum velocity at which the player gets any amount of vertical boost from hopping out of the hook
        public static int SafetySteps = 3;
        public static float SafetyHookAngle = MathHelper.PiOver2 * 1.2f;
        public static float SafetyHookAngleResolution = 50f;

        public bool PlayerOnGround => Collision.SolidCollision(Player.position + Vector2.UnitY* 2f * Player.gravDir, Player.width, Player.height, false);

        public override void ResetEffects()
        {
            WulfrumPackEquipped = false;
            PackItem = null;

            if (hookCooldown > 0)
                hookCooldown--;
        }

        //Initialize the segments between the player and the hook's end point.
        public void SetSegments(Vector2 endPoint)
        {
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

        public override void PostUpdateRunSpeeds()
        {
            if (hookCache != -1)
            {
                Player.grappling[0] = hookCache;
                Player.grapCount = 1;
            }

            hookCache = -1;
        }

        public override void PreUpdateMovement()
        {
            //If the hook cache was set from -1 in the part before the player stepped up,reset it.
            //Should this be detoured before the PlayerLoaders PreUpdateMovement call? In case another mod uses hooks in this call and their modplayer gets called before this one?
            if (hookCache > -1)
            {
                Player.grappling[0] = hookCache;
                Player.grapCount = 1;
            }

            hookCache = -1;

            if (Grappled)
            {
                if ((Main.projectile[Grapple].Center - Player.Center).Length() > SwingLength + 80f)
                {
                    SoundEngine.PlaySound(WulfrumAcrobaticsPack.ReleaseSound, Main.projectile[Grapple].Center);
                    Main.projectile[Grapple].Kill();
                }
                else
                    SimulateMovement(Main.projectile[Grapple]);
            }
        }

        public void SimulateMovement(Projectile grapple)
        {
            Segments = VerletSimulatedSegment.SimpleSimulation(Segments, SwingLength / SimulationResolution, 50, 0.3f * Player.gravDir);

            Vector2 CurrentPosition;

            foreach (VerletSimulatedSegment position in Segments)
            {
                CurrentPosition = position.position;

                //Control point markers
                //Dust doost = Dust.NewDustPerfect(CurrentPosition, 1, Vector2.Zero);
                //doost.noGravity = true;
            }


            if (!GrappleMovementDisabled)
            {
                CurrentPosition = Segments[SimulationResolution].position;
                Player.velocity = CurrentPosition - Player.Center;

                //let the player swing themselves around if they are under the hook.
                if (Player.gravDir * (Player.Center.Y - Segments[0].position.Y) > 0)
                {
                    float swing = 0;

                    if (Math.Sign(Player.velocity.X) < 0)
                    {
                        if (Player.controlLeft)
                            swing -= 0.1f;

                        else if (Player.controlRight)
                            swing += 0.1f;
                    }

                    else if (Math.Sign(Player.velocity.X) > 0)
                    {
                        if (Player.controlRight)
                            swing += 0.1f;

                        else if (Player.controlLeft)
                            swing -= 0.1f;
                    }

                    Player.velocity.X += swing;
                }

                else if (Math.Abs(Player.Center.X - Segments[0].position.X) < 30f && Math.Abs(Player.velocity.X) < 1)
                {
                    Player.velocity.X = Player.velocity.X == 0 ? 1.5f : 1.5f * Math.Sign(Player.velocity.X);
                }

            }

            if (Grappled)
            {
                for (int i = 1; i < Segments.Count; i++)
                {
                    Lighting.AddLight(Segments[i].position, Color.Lerp(Color.DeepSkyBlue, Color.GreenYellow, i / (float)SimulationResolution).ToVector3());
                }
            }

            //Set the old position of the simulation's segments to be the players current center (before the velocity gets applied
            //We can't set the new position here by simply adding the velocity to the players current position because it leads to.. funny bugs if you collide with tiles.
            Segments[SimulationResolution].oldPosition = Player.Center;
        }

        public override void PostUpdate()
        {
            //After the player's movements are finished being calculated, set the current position of the hook chain to be at their new center.
            if (Grappled)
            {
                Segments[SimulationResolution].position = Player.Center;


                if (!GrappleMovementDisabled)
                {
                    //Play a swoosh sound if the player changed sides and moved fast
                    bool playerCrossedSides = Math.Sign(Segments[SimulationResolution].oldPosition.X - Segments[0].position.X) != Math.Sign(Segments[SimulationResolution].position.X - Segments[0].position.X);
                    float swingSpeed = (Segments[SimulationResolution].oldPosition - Segments[SimulationResolution].position).Length();
                    if (swingSpeed > 6f && playerCrossedSides)
                        SoundEngine.PlaySound(CommonCalamitySounds.LouderSwingWoosh with { Volume = CommonCalamitySounds.LouderSwingWoosh.Volume * (Math.Clamp((swingSpeed - 6f) / 12f, 0, 1)) }, Player.Center);
                }
            }

            else if (AutoGrappleActivated)
            {
                Vector2 checkedPlayerPosition = Player.position;
                bool imminentDanger = false;

                for (int i = 0; i < SafetySteps; i++)
                {
                    Vector2 collisionVector = Collision.TileCollision(checkedPlayerPosition, Player.velocity, Player.width, Player.height, gravDir: (int)Player.gravDir);
                    if (collisionVector.Y < Player.velocity.Y)
                    {
                        imminentDanger = true;
                        checkedPlayerPosition += collisionVector;
                        break;
                    }

                    checkedPlayerPosition += collisionVector;
                }

                if (!imminentDanger)
                    return;

                int fallDistance = (int)(checkedPlayerPosition.Y / 16f) - Player.fallStart;
                int fallDmgThreshold = 25 + Player.extraFall;

                //Technically doesn't ignore clouds but oh well.
                if (fallDistance * Player.gravDir > fallDmgThreshold)
                {
                    float halfSpread = SafetyHookAngle / 2f;
                    Point bestGrapplePos = Point.Zero;
                    float bestGrappleScore = 0;

                    for (float angle = -halfSpread; angle < halfSpread; angle += SafetyHookAngle / SafetyHookAngleResolution)
                    {
                        for (int i = 0; i < (int)(WulfrumHook.MaxReach / 16f); i++ )
                        {
                            Vector2 checkSpot = Player.Center + (-Vector2.UnitY * Player.gravDir * i * 16f).RotatedBy(angle);
                            Point tilePos = checkSpot.ToSafeTileCoordinates();
                            Tile tile = Main.tile[tilePos];
                            if (tile.HasUnactuatedTile && tile.CanTileBeLatchedOnTo() && !Player.IsBlacklistedForGrappling(tilePos))
                            {
                                if (bestGrappleScore < EvaluatePotentialSafetyHookPos((checkSpot - Player.Center).Length(), angle))
                                {
                                    bestGrapplePos = tilePos;
                                    bestGrappleScore = EvaluatePotentialSafetyHookPos((checkSpot - Player.Center).Length(), angle);
                                }
                                break;
                            }
                        }
                    }

                    if (bestGrapplePos != Point.Zero)
                    {
                        //Clear any hooks that might have been flying before then.
                        for (int i = 0; i < Main.maxProjectiles; ++i)
                        {
                            Projectile p = Main.projectile[i];
                            if (!p.active || p.owner != Player.whoAmI || p.type != ProjectileType<WulfrumHook>())
                                continue;

                            if (p.ModProjectile is WulfrumHook)
                            {
                                SoundEngine.PlaySound(WulfrumAcrobaticsPack.ReleaseSound, p.Center);
                                p.Kill();

                            }
                        }

                        //Reset the players fall height, because if they take fall dmg in teh frame right after this one it may have a chance of still killing the player due to the
                        //code where the grapple resets the players fall speed hasnt been called yet
                        Player.fallStart = (int)(Player.position.Y / 16);

                        if (Player.whoAmI == Main.myPlayer)
                        {
                            Projectile.NewProjectile(Player.GetSource_ItemUse(PackItem), bestGrapplePos.ToWorldCoordinates(), Vector2.Zero, ProjectileType<WulfrumHook>(), 3, 0, Player.whoAmI);
                        }
                    }
                }
            }
        }

        public float EvaluatePotentialSafetyHookPos(float distance, float angle)
        {
            float score = 0.0001f;

            if (distance < 2 * WulfrumHook.MaxReach / 3f)
                score += distance / (2 * WulfrumHook.MaxReach / 3f);

            else
                score += 1 - (distance - (2 * WulfrumHook.MaxReach / 3f)) / (WulfrumHook.MaxReach / 3f);

            score += (1 - Math.Abs(angle) / (SafetyHookAngle / 2f)) * 0.5f;

            return score;
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (!WulfrumPackEquipped)
                return;

            //Shoot a new hook 
            if (triggersSet.Grapple && Player.releaseHook)
            {
                //Clear any previous non-wulfrum hooks / Any hooks that just got shot (should already be handled by the global proj
                for (int i = 0; i < Main.maxProjectiles; ++i)
                {
                    Projectile p = Main.projectile[i];
                    if (!p.active || p.owner != Player.whoAmI || p.aiStyle != 7 || p.type == ProjectileType<WulfrumHook>())
                        continue;

                    p.Kill();
                }


                if (hookCooldown <= 0 && Main.projectile.Count(n => n.active && n.owner == Player.whoAmI && n.type == ProjectileType<WulfrumHook>()) <= 1)
                {
                    SoundEngine.PlaySound(WulfrumAcrobaticsPack.ShootSound, Player.Center);
                    if (Player.whoAmI == Main.myPlayer)
                    {
                        Vector2 velocity = (Main.MouseWorld - Player.Center).SafeNormalize(Vector2.One) * GrappleVelocity;
                        Projectile.NewProjectile(Player.GetSource_ItemUse(PackItem), Player.Center, velocity, ProjectileType<WulfrumHook>(), 0, 0, Player.whoAmI);

                        float angleToRightBelow = velocity.AngleBetween(Vector2.UnitY);
                        if (angleToRightBelow < MathHelper.PiOver2) //Put a cooldown on hooking down below. 
                        {
                            int extraCooldown = (int)(Utils.GetLerpValue(MathHelper.PiOver2, 0f, angleToRightBelow) * 15); //Get more cooldown the straightest down youre aiming
                            hookCooldown = 15 + extraCooldown;
                        }
                    }
                }
            }

            //Jumping out of the hook
            if (triggersSet.Jump && Player.releaseJump)
            {
                for (int i = 0; i < Main.maxProjectiles; ++i)
                {
                    Projectile p = Main.projectile[i];
                    if (!p.active || p.owner != Player.whoAmI || p.type != ProjectileType<WulfrumHook>())
                        continue;

                    //Only clear hooks that are attached to stuff
                    if (p.ModProjectile is WulfrumHook claw && claw.State == WulfrumHook.HookState.Grappling)
                    {

                        float angleToUpright = (Player.Center - p.Center).AngleBetween(-Vector2.UnitY);
                        bool canJumpOffHook = angleToUpright > MathHelper.PiOver2 || Player.Distance(p.Center) < 38;// Don't do any jump stuff if the player is jumping from above the hook.

                        if (canJumpOffHook) 
                        {
                            Vector2 velocityBoost = Vector2.Zero;

                            //Additionally, accelerate the player a lil' if they were holding down the buttons in the direction of their swing.
                            if ((Math.Sign(Player.velocity.X) < 0 && Player.controlLeft) || (Math.Sign(Player.velocity.X) > 0 && Player.controlRight))
                            {
                                velocityBoost += Player.velocity * 0.15f;
                            }
                            //Additionally^2, if the player isnt moving very fast, make them do a straight up hop.
                            //Don't do the hop if the player isnt moving at all though because thats handled by vanilla.
                            if (Player.velocity.Length() < MaxHopVelocity && Player.velocity.Length() > 0.0001f || PlayerOnGround)
                            {
                                velocityBoost -= Vector2.UnitY * Player.jumpSpeed * (1 - (float)Math.Pow(Player.velocity.Length() / MaxHopVelocity, 5f));
                            }


                            Player.velocity += velocityBoost;
                            Player.jump = Player.jumpHeight / 2;
                        }

                        else
                        {
                            //Prevents double jumps from getting activated
                            Player.releaseJump = false;
                        }

                        SoundEngine.PlaySound(WulfrumAcrobaticsPack.ReleaseSound, p.Center);
                        p.Kill();

                    }
                }

            }
        }


        public override void FrameEffects()
        {
            //Cache the hook. This is done so that the player may walk around animated if the grapple's movement is disabled.
            //The cache is retrieved in the postFrame detour.
            if (Player.grappling[0] >= 0 && GrappleMovementDisabled && Main.projectile[Player.grappling[0]].type == ProjectileType<WulfrumHook>())
            {
                hookCache = Player.grappling[0];
                Player.grappling[0] = -1;
                Player.grapCount = 0;
            }
        }
    }
}
