using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria.Audio;

namespace CalamityMod.Items.Weapons.Magic
{
    public class CoralSpout : ModItem
    {
        public static readonly SoundStyle ChargeSound = SoundID.LiquidsHoneyWater with { Type = SoundType.Sound };

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Coral Spout");
            Tooltip.SetDefault("Casts coral water spouts that lay on the ground and damage enemies");
            //Lore about stone tablets being used since paper couldn't be brought underwater.
            //Maybe something something, the sea kingdom had to have a large amount of scribes to rewrite books into tablets
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 7;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 7;
            Item.width = 28;
            Item.height = 30;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.channel = true;
            Item.knockBack = 2f;
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item17;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<CoralSpoutHoldout>();
            Item.shootSpeed = 16f;
        }

        public override void HoldItem(Player player)
        {
            player.Calamity().mouseWorldListener = true;
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.projectile.Any(n => n.active && n.owner == player.whoAmI && n.type == ModContent.ProjectileType<CoralSpoutHoldout>());
        }

        public override void UseItemFrame(Player player)
        {
            //Calculate the dirction in which the players arms should be pointing at.
            Vector2 playerToCursor = (player.Calamity().mouseWorld - player.Center).SafeNormalize(Vector2.UnitX);
            float armPointingDirection = (playerToCursor.ToRotation());

            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, armPointingDirection - MathHelper.PiOver2);
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armPointingDirection - MathHelper.PiOver2);
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.Calamity().mouseWorld.X > player.Center.X)
            {
                player.ChangeDir(1);
            }
            else
            {
                player.ChangeDir(-1);
            }

            CalamityUtils.CleanHoldStyle(player, player.compositeFrontArm.rotation + MathHelper.PiOver2, player.GetFrontHandPosition(player.compositeFrontArm.stretch, player.compositeFrontArm.rotation).Floor(), new Vector2(32, 0), new Vector2(-10, 8));
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SeaRemains>(2).
                AddIngredient(ItemID.Coral, 5).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
