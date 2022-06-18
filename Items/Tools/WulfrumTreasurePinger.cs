using CalamityMod.Items.Materials;
using CalamityMod.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CalamityMod.Items.Tools
{
    public class WulfrumTreasurePinger : ModItem
    {
        public static readonly SoundStyle ScanBeepSound = new("CalamityMod/Sounds/Item/WulfrumPing") { PitchVariance = 0.1f };
        public static readonly SoundStyle ScanBeepBreakSound = new("CalamityMod/Sounds/Item/WulfrumPingBreak");
        public static readonly SoundStyle RechargeBeepSound = new("CalamityMod/Sounds/Item/WulfrumPingReady") { PitchVariance = 0.1f };

        public int usesLeft = maxUses;
        public const int maxUses = 20;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Treasure Pinger");
            Tooltip.SetDefault("Helps you find metal that's hopefully more valuable than wulfrum\n" +
            "This contraption seems incredibly shoddy. [c/fc4903: It'll break sooner than later for sure]"
            );
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 42;
            Item.useTime = Item.useAnimation = 25;
            Item.autoReuse = false;
            Item.holdStyle = 16; //Custom hold style
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = null;
            Item.noMelee = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(silver: 50);
            usesLeft = maxUses;
        }

        public override void HoldItem(Player player)
        {
            player.Calamity().mouseWorldListener = true;
        }

        public override bool CanUseItem(Player player)
        {
            return !(TilePingerSystem.tileEffects["WulfrumPing"].Active);
        }

        #region saving the durability
        public override ModItem Clone(Item item)
        {
            ModItem clone = base.Clone(item);
            if (clone is WulfrumTreasurePinger a && item.ModItem is WulfrumTreasurePinger a2)
                a.usesLeft = a2.usesLeft;

            return clone;
        }

        public override void SaveData(TagCompound tag)
        {
            tag["usesLeft"] = usesLeft;
        }

        public override void LoadData(TagCompound tag)
        {
            usesLeft = tag.GetInt("usesLeft");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(usesLeft);
        }

        public override void NetReceive(BinaryReader reader)
        {
            usesLeft = reader.ReadInt32();
        }
        #endregion

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<WulfrumShard>(6).
                Register();
        }

        public override bool? UseItem(Player player)
        {
            if (TilePingerSystem.AddPing("WulfrumPing", player.Center, player))
            {
                if (player.name != "John Wulfrum")
                    usesLeft--;

                if (usesLeft <= 0)
                {
                    if (player.whoAmI == Main.myPlayer)
                        SoundEngine.PlaySound(ScanBeepBreakSound);
                    Item.TurnToAir();
                }

                else if (player.whoAmI == Main.myPlayer)
                    SoundEngine.PlaySound(ScanBeepSound);

                return true;
            }

            return false;
        }

        #region drawing stuff
        public void SetItemInHand(Player player, Rectangle heldItemFrame)
        {
            //Make the player face where they're aiming.
            if (player.Calamity().mouseWorld.X > player.Center.X)
            {
                player.ChangeDir(1);
            }
            else
            {
                player.ChangeDir(-1);
            }

            float itemRotation = player.compositeBackArm.rotation + MathHelper.PiOver2;
            Vector2 itemPosition = player.GetBackHandPosition(player.compositeBackArm.stretch, player.compositeBackArm.rotation).Floor();
            Vector2 itemSize = new Vector2(52, 42);
            Vector2 itemOrigin = new Vector2(-20, -13);

            CalamityUtils.CleanHoldStyle(player, itemRotation, itemPosition, itemSize, itemOrigin);
        }

        public void SetPlayerArms(Player player)
        {
            //Calculate the dirction in which the players arms should be pointing at.
            Vector2 playerToCursor = (player.Calamity().mouseWorld - player.Center).SafeNormalize(Vector2.UnitX);
            float armPointingDirection = (playerToCursor.ToRotation() + MathHelper.PiOver2).Modulo(MathHelper.TwoPi);

            //"crop" the rotation so the player only tilts the fishing rod slightly up and slightly down.
            if (armPointingDirection < MathHelper.Pi)
            {
                armPointingDirection = armPointingDirection / MathHelper.Pi * MathHelper.PiOver4 * 0.5f - MathHelper.PiOver4 * 0.3f;
            }

            //It gets a bit harder if its pointing left; ouch
            else
            {
                armPointingDirection -= MathHelper.Pi;

                armPointingDirection = armPointingDirection / MathHelper.Pi * MathHelper.PiOver4 * 0.5f - MathHelper.PiOver4 * 0.3f + MathHelper.Pi;
            }

            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, armPointingDirection - MathHelper.PiOver2);
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armPointingDirection - MathHelper.PiOver2);
        }

        public override void HoldStyle(Player player, Rectangle heldItemFrame) => SetItemInHand(player, heldItemFrame);
        public override void UseStyle(Player player, Rectangle heldItemFrame) => SetItemInHand(player, heldItemFrame);
        public override void HoldItemFrame(Player player) => SetPlayerArms(player);
        public override void UseItemFrame(Player player) => SetPlayerArms(player);

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (usesLeft == maxUses)
                return;

            float barScale = 1.3f;

            var barBG = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GenericBarBack").Value;
            var barFG = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GenericBarFront").Value;

            Vector2 drawPos = position + Vector2.UnitY * (frame.Height - 2) * scale + Vector2.UnitX * (frame.Width - barBG.Width * barScale) * scale * 0.5f;
            Rectangle frameCrop = new Rectangle(0, 0, (int)(usesLeft / (float)maxUses * barFG.Width), barFG.Height);
            Color colorBG = Color.RoyalBlue;
            Color colorFG = Color.Lerp(Color.Teal, Color.YellowGreen, usesLeft / (float)maxUses);

            spriteBatch.Draw(barBG, drawPos, null, colorBG, 0f, origin, scale * barScale, 0f, 0f);
            spriteBatch.Draw(barFG, drawPos, frameCrop, colorFG * 0.8f, 0f, origin, scale * barScale, 0f, 0f);
        }
        #endregion
    }
}
