using CalamityMod.Buffs.Mounts;
using CalamityMod.CalPlayer;
using CalamityMod.Cooldowns;
using CalamityMod.Items.Accessories.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Mounts;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameInput;
using static Microsoft.Xna.Framework.Input.Keys;
using static Terraria.ModLoader.ModContent;


namespace CalamityMod.Items.Armor.MarniteArchitect
{
    [AutoloadEquip(EquipType.Head)]
    public class MarniteArchitectHeadgear : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PreHardmode";

        public static readonly SoundStyle LiftSpawnSound = new("CalamityMod/Sounds/Item/MarniteLiftSummon");
        public static readonly SoundStyle LiftGoAwaySound = new("CalamityMod/Sounds/Item/MarniteLiftUnsummon");
        public static readonly SoundStyle LiftHummSound = new("CalamityMod/Sounds/Item/MarniteLiftHumm") { IsLooped = true};

        public static float LiftRaiseSpeed = 2f;
        public static float MaxLiftHeight = 138f;

        public override void Load()
        {
            Terraria.On_Player.QuickMount += ActivateLift;
        }

        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
            ArmorIDs.Head.Sets.DrawFullHair[equipSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 0;
        }

        private void ActivateLift(Terraria.On_Player.orig_QuickMount orig, Player self)
        {
            //Spoof the mount thing
            if (!self.mount.Active && HasArmorSet(self) && self.miscEquips[Player.miscSlotMount].IsAir)
            {
                if (self.frozen || self.tongued || self.webbed || self.stoned || self.gravDir == -1f || self.dead || self.noItems)
                    return;

                int liftMountType = MountType<MarniteLift>();

                if (self.mount.CanMount(liftMountType, self))
                {
                    self.mount.SetMount(liftMountType, self);
                    SoundEngine.PlaySound(LiftSpawnSound, self.Center);
                }
            }

            else
                orig(self);
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == ModContent.ItemType<MarniteArchitectToga>();
        public static bool HasArmorSet(Player player) => player.armor[0].type == ItemType<MarniteArchitectHeadgear>() && player.armor[1].type == ItemType<MarniteArchitectToga>();
        public bool IsPartOfSet(Item item) => item.type == ItemType<MarniteArchitectHeadgear>() ||
                item.type == ItemType<MarniteArchitectToga>();

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Marnite Lift"; //Replaced below
            player.GetModPlayer<MarniteArchitectPlayer>().setEquipped = true;
        }

        public override void UpdateEquip(Player player)
        {
            //Tile range is a static variable did you know that? That's quite funny. I assume it's just because its not like other players would need to know about the players reach
            if (Main.myPlayer == player.whoAmI)
            {
                Player.tileRangeX += 5;
                Player.tileRangeY += 5; //Extendo grip also increases vertical tile range by one less than horizontal <-- This is silly lmao
            }
        }

        public static void ModifySetTooltips(ModItem item, List<TooltipLine> tooltips)
        {
            if (HasArmorSet(Main.LocalPlayer))
            {
                int setBonusIndex = tooltips.FindIndex(x => x.Name == "SetBonus" && x.Mod == "Terraria");

                if (setBonusIndex != -1)
                {
                    tooltips[setBonusIndex].Text = CalamityUtils.GetTextValueFromModItem<MarniteArchitectHeadgear>("AbilityBrief");
                    tooltips[setBonusIndex].OverrideColor = Color.Lerp(new Color(255, 243, 161), new Color(137, 162, 255), 0.5f + 0.5f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3f));

                    TooltipLine setBonus1 = new TooltipLine(item.Mod, "CalamityMod:SetBonus1", CalamityUtils.GetTextValueFromModItem<MarniteArchitectHeadgear>("AbilityDescription"));
                    setBonus1.OverrideColor = new Color(145, 197, 239);
                    tooltips.Insert(setBonusIndex + 1, setBonus1);
                }

            }
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips) => ModifySetTooltips(this, tooltips);


        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnyGoldCrown").
                AddIngredient(ItemID.Granite, 5).
                AddIngredient(ItemID.Marble, 5).
                AddTile(TileID.Anvils).
                Register();
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class MarniteArchitectToga : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PreHardmode";
        public override void Load()
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Armor/MarniteArchitect/MarniteArchitectToga_Legs", EquipType.Legs, this);
        }

        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 1;
        }

        public override void UpdateEquip(Player player)
        {
            player.tileSpeed += 0.5f;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => MarniteArchitectHeadgear.ModifySetTooltips(this, tooltips);


        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnyGoldBar", 2).
                AddIngredient(ItemID.Silk, 15).
                AddIngredient(ItemID.Granite, 15).
                AddIngredient(ItemID.Marble, 15).
                AddTile(TileID.Anvils).
                Register();
        }

        public override void SetMatch(bool male, ref int equipSlot, ref bool robes)
        {
            robes = true;
            equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);
        }
    }

    public class MarniteArchitectPlayer : ModPlayer
    {
        public bool setEquipped = false;
        public bool mounted = false;
        public SlotId liftDroningSoundSlot;

        public override void ResetEffects()
        {
            setEquipped = false;
            mounted = false;
        }

        public override void UpdateDead()
        {
            setEquipped = false;
            mounted = false;
        }

        public override void PostUpdateMiscEffects()
        {
            if (!setEquipped && Player.mount.Type == ModContent.MountType<MarniteLift>() && Player.mount.Active)
                Player.mount.Dismount(Player);

            if (mounted)
            {
                if (!SoundEngine.TryGetActiveSound(liftDroningSoundSlot, out var soundPlaying))
                    liftDroningSoundSlot = SoundEngine.PlaySound(MarniteArchitectHeadgear.LiftHummSound, Player.Center);

                else
                {
                    soundPlaying.Position = Player.Center;
                    float distanceToGround = RaycastGround(true).Y;
                    soundPlaying.Volume = 1 - (distanceToGround / MarniteArchitectHeadgear.MaxLiftHeight) * 0.3f;
                }



                //Do something to prevent the player from having only one feet showing??
            }

            else
            {
                if (SoundEngine.TryGetActiveSound(liftDroningSoundSlot, out var soundPlaying))
                {
                    soundPlaying.Stop();
                    liftDroningSoundSlot = SlotId.Invalid;
                }
            }
        }

        public Vector2 RaycastGround(bool centerOnly = false)
        {
            float closestDistance = (MarniteArchitectHeadgear.MaxLiftHeight + 1);

            if (centerOnly)
            {
                Vector2 basePosition = Player.Bottom;

                for (float j = 0; j < closestDistance; j += 8f)
                {
                    Point tileToCheck = (basePosition + Vector2.UnitY * j).ToSafeTileCoordinates();

                    if (Main.tile[tileToCheck].IsTileSolidGround())
                    {
                        closestDistance = j;
                        break;
                    }
                }

                return Vector2.UnitY * closestDistance;
            }

            for (float i = 0; i < Player.width; i += Player.width / 6f)
            {
                Vector2 basePosition = Player.BottomLeft + Vector2.UnitX * i;

                for (float j = 0; j < closestDistance; j += 8f)
                {
                    Point tileToCheck = (basePosition + Vector2.UnitY * j).ToSafeTileCoordinates();

                    if (Main.tile[tileToCheck].IsTileSolidGround())
                    {
                        closestDistance = j;
                        break;
                    }
                }
            }

            return Vector2.UnitY * closestDistance;
        }

        public float BestLiftDistanceFromGround()
        {
            Vector2 closestGround = RaycastGround();
            if (closestGround.Y > MarniteArchitectHeadgear.MaxLiftHeight)
                return -1;

            return closestGround.Y;
        }

        public override void PreUpdateMovement()
        {
            if (mounted)
            {
                float distanceToGround = BestLiftDistanceFromGround();

                if (distanceToGround >= 0)
                {
                    float newVelocity;  

                    if (Player.controlUp || Player.controlJump)
                    {
                        newVelocity = -MarniteArchitectHeadgear.LiftRaiseSpeed;

                        if (distanceToGround + MarniteArchitectHeadgear.LiftRaiseSpeed > MarniteArchitectHeadgear.MaxLiftHeight - 3)
                        {
                            newVelocity = ((MarniteArchitectHeadgear.MaxLiftHeight - 3) - distanceToGround) * -1;
                        }

                        Player.fallStart = (int)(Player.Center.Y / 16);
                    }

                    else if (Player.controlDown)
                    {
                        newVelocity = MarniteArchitectHeadgear.LiftRaiseSpeed;

                        if (Collision.TileCollision(Player.position, new Vector2(Player.velocity.X, newVelocity), Player.width, Player.height, true, false, (int)Player.gravDir).Y == 0f)
                            newVelocity = 0.5f;
                    }
                    else
                        newVelocity = 0f;


                    if (Math.Abs(Player.velocity.Y) < 4f)
                    {
                        Player.velocity.Y = newVelocity;
                        Player.fallStart = (int)(Player.Center.Y / 16);
                    }

                    else
                        Player.velocity.Y = MathHelper.Lerp(Player.velocity.Y, newVelocity, 0.12f);
                }

            }
        }
    }

    public class MarniteLift : ModMount
    {

        public override void SetStaticDefaults()
        {
            // Movement
            MountData.jumpHeight = 0; // How high the mount can jump.
            MountData.acceleration = 0.2f; // The rate at which the mount speeds up.
            MountData.jumpSpeed = 0f; // The rate at which the player and mount ascend towards (negative y velocity) the jump height when the jump button is presssed.
            MountData.blockExtraJumps = true; // Determines whether or not you can use a double jump (like cloud in a bottle) while in the mount.
            MountData.constantJump = false; // Allows you to hold the jump button down.
            MountData.fallDamage = 1f; // Fall damage multiplier.
            MountData.runSpeed = 2f; // The speed of the mount
            MountData.dashSpeed = 2f; // The speed the mount moves when in the state of dashing.
            MountData.flightTimeMax = 0; // The amount of time in frames a mount can be in the state of flying.
            MountData.fatigueMax = 0;
            MountData.usesHover = false;

            // Misc
            MountData.buff = BuffType<MarniteLiftBuff>(); // The ID number of the buff assigned to the mount.

            // Effects
            MountData.spawnDust = 6; // The ID of the dust spawned when mounted or dismounted.

            // Frame data and player offsets
            MountData.totalFrames = 1; // Amount of animation frames for the mount
            MountData.heightBoost = 0; // Height between the mount and the ground
            MountData.playerYOffsets = Enumerable.Repeat(33, 1).ToArray(); // Fills an array with values for less repeating code
            MountData.xOffset = 0;
            MountData.yOffset = 0;
            MountData.bodyFrame = 0;
            MountData.playerHeadOffset = 4;

            //Sprites
            if (Main.netMode != NetmodeID.Server)
            {
                MountData.frontTextureGlow = ModContent.Request<Texture2D>("CalamityMod/Items/Armor/MarniteArchitect/MarniteLiftFire");
            }
        }

        public override void UpdateEffects(Player player)
        {
            Lighting.AddLight(player.Bottom + Vector2.UnitY * 10f, Color.DeepSkyBlue.ToVector3());

            float centerDistance = player.GetModPlayer<MarniteArchitectPlayer>().RaycastGround(true).Y;

            if (centerDistance <= MarniteArchitectHeadgear.MaxLiftHeight)
            {
                if (Main.rand.NextFloat() > (centerDistance / MarniteArchitectHeadgear.MaxLiftHeight) * 0.6f && Main.rand.NextBool())
                {
                    float scale = 1.2f - (centerDistance / MarniteArchitectHeadgear.MaxLiftHeight) * 0.7f;
                    Dust dust = Dust.NewDustPerfect(player.Bottom + Vector2.UnitY * centerDistance + Vector2.UnitX * Main.rand.Next(-16, 16), 31, new Vector2((Main.rand.NextFloat(-8, 8) * scale) - player.velocity.X, Main.rand.NextFloat(-1, 1)), 120, Scale : scale * 1.5f);
                }
            }

            if (Main.rand.NextBool(3))
            {
                float speedRotation = player.velocity.X * 0.03f;

                Dust dust = Dust.NewDustPerfect(player.Bottom + Vector2.UnitX.RotatedBy(speedRotation) * Main.rand.Next(-6, 6) + Vector2.UnitY.RotatedBy(speedRotation) * -2f, 229, Vector2.UnitY.RotatedBy(speedRotation) * Main.rand.NextFloat(1f, 3f), 120, Scale: Main.rand.NextFloat(0.6f, 1f));
                dust.noGravity = true;
            }
        }

        public void DustEffects(Player player)
        {
            for (int j = 0; j < 17; j++)
            {
                Vector2 direction = Main.rand.NextVector2CircularEdge(1f, 1f);
                int dustType = Main.rand.NextBool() ? 240 : 236;
                Dust.NewDustPerfect(player.Bottom + (direction * 3) + Vector2.UnitY * 10f, dustType, direction.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f) - 1.57f) * Main.rand.Next(1, 3), 0, new Color(255, 255, 60) * 0.8f, 0.6f);
            }
        }

        public override void SetMount(Player player, ref bool skipDust)
        {
            DustEffects(player);
            skipDust = true;
        }

        public override void Dismount(Player player, ref bool skipDust)
        {
            SoundEngine.PlaySound(MarniteArchitectHeadgear.LiftGoAwaySound, player.Center);
            DustEffects(player);
            skipDust = true;
        }

        public static Vector2 GetFireSquish(float timeOffset, float timeSpeed)
        {
            return new Vector2((float)Math.Sin(Main.GlobalTimeWrappedHourly * timeSpeed + timeOffset) * 0.1f + 1f, 1.2f - 0.3f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * timeSpeed + timeOffset));
        }

        public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
        {
            rotation = MathHelper.Clamp(drawPlayer.velocity.X * 0.03f, - MathHelper.ToRadians(7f), MathHelper.ToRadians(7f));
            drawPlayer.fullRotation = rotation;

            // Draw is called for each mount texture we provide, so we check drawType to avoid duplicate draws.
            if (drawType == 0)
            {
                Texture2D platformTex = MountData.frontTexture.Value;
                Texture2D fireTex = MountData.frontTextureGlow.Value;

                Vector2 fireOrigin = new Vector2(fireTex.Width / 2f, 0f);
                Color fireColor = Color.White;

                for (int i = 0; i < 3; i++)
                {
                    fireColor = Color.White;
                    Vector2 fireScale = GetFireSquish(i * 0.6f, 14f);
                    fireColor.A = (byte)(210 - i * 70);
                    fireColor *= (0.3f + 0.35f * i);
                    fireScale *= drawScale * (1.5f - 0.25f * i);

                    playerDrawData.Add(new DrawData(fireTex, drawPosition + new Vector2(0, 8), new Rectangle(0, 0, fireTex.Width, fireTex.Height), fireColor, drawPlayer.fullRotation, fireOrigin, fireScale * drawScale, SpriteEffects.None, 0));
                }
                    
                playerDrawData.Add(new DrawData(platformTex, drawPosition, new Rectangle(0, 0, platformTex.Width, platformTex.Height), drawColor, drawPlayer.fullRotation, platformTex.Size() / 2f, drawScale, SpriteEffects.None, 0));
            }

            return false;
        }
    }
}
