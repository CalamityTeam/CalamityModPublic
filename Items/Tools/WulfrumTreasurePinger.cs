using System.Linq;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Systems;
using Terraria.Audio;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Tools
{
    public class WulfrumTreasurePinger : ModItem
    {
        public static readonly SoundStyle BeepSound = new("CalamityMod/Sounds/Item/WulfrumPing") { PitchVariance = 0.1f };

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
        }

        public override bool CanUseItem(Player player)
        {
            return !(TilePingerSystem.tileEffects["WulfrumPing"].Active);
        }

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
                SoundEngine.PlaySound(BeepSound);
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

            CalamityUtils.CleanHoldStyle(player, player.compositeBackArm.rotation + MathHelper.PiOver2, player.GetBackHandPosition(player.compositeBackArm.stretch, player.compositeBackArm.rotation).Floor(), new Vector2(52, 42), new Vector2(-20, -13));
        }


        public void SetPlayerArms(Player player)
        {
            //Calculate the dirction in which the players arms should be pointing at.
            Vector2 playerToCursor = (player.Calamity().mouseWorld - player.Center).SafeNormalize(Vector2.UnitX);
            float armPointingDirection = (playerToCursor.ToRotation());


            //"crop" the rotation so the player only tilts the fishing rod slightly up and slightly down.
            if (armPointingDirection < MathHelper.PiOver2 && armPointingDirection >= -MathHelper.PiOver2)
            {
                armPointingDirection = -MathHelper.PiOver2 + MathHelper.PiOver4 + MathHelper.PiOver2 * Utils.GetLerpValue(0f, MathHelper.Pi, armPointingDirection + MathHelper.PiOver2, true);
            }

            //It gets a bit harder if its pointing left; ouch
            else
            {
                if (armPointingDirection > 0)
                    armPointingDirection = MathHelper.PiOver2 + MathHelper.PiOver4 + MathHelper.PiOver4 * Utils.GetLerpValue(0f, MathHelper.PiOver2, armPointingDirection - MathHelper.PiOver2, true);
                else
                    armPointingDirection = -MathHelper.Pi + MathHelper.PiOver4 * Utils.GetLerpValue(-MathHelper.Pi, -MathHelper.PiOver4, armPointingDirection, true);
            }

            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, armPointingDirection - MathHelper.PiOver2);
        }

        public override void HoldStyle(Player player, Rectangle heldItemFrame)
        {
            SetItemInHand(player, heldItemFrame);
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            SetItemInHand(player, heldItemFrame);
        }

        public override void HoldItemFrame(Player player)
        {
            SetPlayerArms(player);
        }

        public override void UseItemFrame(Player player)
        {
            SetPlayerArms(player);
        }
        #endregion
    }
}
