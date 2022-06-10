using System.Linq;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Cnidarian : ModItem
    {
        public override string Texture => "CalamityMod/Items/Weapons/Melee/CnidarianFishingRod";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cnidarian");
            Tooltip.SetDefault("Fishes up an electric jellyfish\n" +
            "Periodically sends out sparks to electrocute nearby enemies.\n" +
            //Lore tooltip time. Elden Ring.
            "[c/5C95A1:From looking at their less sapient brethren, the Old Kingdom’s inhabitants adapted tools and weapons designed for underwater efficiency.]\n" +
            "[c/5C95A1:The electric properties of ghost bells remain useful even after death, having been commonly used as conduits.]"
            );
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 26;
            Item.DamageType = DamageClass.Melee;
            Item.damage = 11;
            Item.knockBack = 3f;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.autoReuse = true;

            Item.holdStyle = 16; //Custom hold style
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item1;
            Item.channel = true;
            //Item.noUseGraphic = true;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<CnidarianJellyfishOnTheString>();
            Item.shootSpeed = 10f;

            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 2);
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.projectile.Any(n => n.active && n.owner == player.whoAmI && n.type == ProjectileType<CnidarianJellyfishOnTheString>());
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SeaRemains>(2).
                AddTile(TileID.Anvils).
                Register();
        }

        public override void HoldItem(Player player)
        {
            player.Calamity().mouseWorldListener = true;
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

            CalamityUtils.CleanHoldStyle(player, player.compositeFrontArm.rotation + MathHelper.PiOver2, player.GetFrontHandPosition(player.compositeFrontArm.stretch, player.compositeFrontArm.rotation).Floor(), new Vector2(42, 34), new Vector2(-15, 11));
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
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armPointingDirection - MathHelper.PiOver2);
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

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D properSprite = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Melee/Cnidarian").Value;

            //Scale the jellyfish sprite properly, since its larger than the fishing rod (Largest dimension of the jellyfish sprite : 52. Largest dimension of the fishing rod : 42)
            float scaleRatio = 42 / 52f;
            //Offset the jellyfish sprite properly, since the fishing rod is larger than the jellyfish (Jellyfish width : 28px, Fishing rod width : 42)
            Vector2 positionOffset = new Vector2(21 - 14, 0) * scale;

            spriteBatch.Draw(properSprite, position + positionOffset, null, drawColor, 0f, origin, scale * scaleRatio, 0, 0);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D properSprite = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Melee/Cnidarian").Value;

            spriteBatch.Draw(properSprite, Item.position - Main.screenPosition, null, lightColor, rotation, properSprite.Size() / 2f, scale, 0, 0);
            return false;
        }
        #endregion
    }
}
