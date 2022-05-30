using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee.Yoyos;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Cnidarian : ModItem
    {
        public override string Texture => "CalamityMod/Items/Weapons/Melee/CnidarianFishingRod";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cnidarian");
            Tooltip.SetDefault("Fires a seashell when enemies are near\n" +
            "A very agile yoyo");
            ItemID.Sets.Yoyo[Item.type] = true;
            ItemID.Sets.GamepadExtraRange[Item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Item.type] = true;
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

            Item.shoot = ModContent.ProjectileType<CnidarianYoyo>();
            Item.shootSpeed = 10f;

            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 2);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SeaRemains>(2).
                AddTile(TileID.Anvils).
                Register();
        }

        #region drawing stuff
        public override void HoldItemFrame(Player player)
        {
            //Make the player face where they're aiming.
            if (Main.MouseWorld.X > player.Center.X)
            {
                player.ChangeDir(1);
            }
            else
            {
                player.ChangeDir(-1);
            }

            //Calculate the dirction in which the players arms should be pointing at.
            Vector2 playerToCursor = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX);
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

             player.itemLocation = player.MountedCenter + player.compositeFrontArm.rotation.ToRotationVector2() * 10f * player.direction;
             player.itemRotation = player.compositeFrontArm.rotation + MathHelper.PiOver2 * player.direction;

        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D properSprite = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Melee/Cnidarian").Value;

            spriteBatch.Draw(properSprite, position, null, drawColor, 0f, origin, scale, 0, 0);
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
