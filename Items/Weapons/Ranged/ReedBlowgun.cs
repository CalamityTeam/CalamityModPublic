using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    [LegacyName("Seabow")]
    public class ReedBlowgun : ModItem
    {
        public static readonly SoundStyle BubbleBurstSound = new ("CalamityMod/Sounds/Custom/PistolShrimpBubbleBurst") { PitchVariance = 0.15f, Volume = 0.2f};

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reed Blowgun");
            Tooltip.SetDefault("Fires a high-pressure stream of bubbles\n" +
                //Lore tooltip time. King's field
                "[c/5C95A1:The Old Sea Kingdom never truly sought to expand beyond its initial borders.]\n" +
                "[c/5C95A1:However, they had a perfect track record of repelling any invasions, thanks to their insurmountable advantage in the water.]"
                );

            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 19;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 22;
            Item.height = 46;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.holdStyle = 16;
            Item.noMelee = true;
            Item.knockBack = 13.5f;
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<PressurizedBubbleStream>();
            Item.shootSpeed = 16f;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override void HoldItem(Player player)
        {
            player.Calamity().mouseWorldListener = true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedByRandom(0.01f);
        }

        public void SetItemInHand(Player player, Rectangle heldItemFrame)
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

            float blowpipeRotation = player.compositeBackArm.rotation + MathHelper.PiOver2 + player.direction * MathHelper.PiOver4 / 3f;

            Vector2 blowpipePosition = player.GetBackHandPosition(player.compositeBackArm.stretch, player.compositeBackArm.rotation).Floor() + Vector2.UnitY * 7f;
            if (player.direction > 0)
                blowpipePosition += Vector2.UnitX * -9f;

            CalamityUtils.CleanHoldStyle(player, blowpipeRotation, blowpipePosition, new Vector2(50, 18), new Vector2(-17, 6));
        }


        public void SetPlayerArms(Player player, bool frontArm = false)
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

            //Calculate the dirction in which the players arms should be pointing at.
            Vector2 playerToCursor = (player.Calamity().mouseWorld - player.Center).SafeNormalize(Vector2.UnitX);
            float pointingDirection = (playerToCursor.ToRotation());

            if (frontArm)
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, pointingDirection - MathHelper.PiOver2);
            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, pointingDirection - MathHelper.PiOver2);
            player.headRotation = pointingDirection;
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
            SetPlayerArms(player, true);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SeaRemains>(2).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
