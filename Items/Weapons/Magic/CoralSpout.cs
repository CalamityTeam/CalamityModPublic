using System.Linq;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class CoralSpout : ModItem
    {
        public static readonly SoundStyle ChargeSound = SoundID.LiquidsHoneyWater with { Type = SoundType.Sound };

        public static int FullChargeExtraDamage = 3; //Extra damage dealt by each coral chunkits b when fully charging a shot

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Coral Spout");
            Tooltip.SetDefault("Casts a shotgun-like blast of coral shards\n" +
                               "Keep the attack button held down to narrow the spread\n" +
                               "Fully charging the attack releases a single, bigger chunk of coral that sticks to enemies\n" +
                               "Grabbing the chunk of coral after it falls from the enemy replenishes 100 mana\n" +
                               //Lore tooltip time. Sekiro.
                               "[c/5C95A1:Knowledge is important, and the Old Sea Kingdom’s many scribes knew it had to be preserved at any cost.]\n" +
                               "[c/5C95A1:Their libraries were much larger than most, because of the water-proof stone tablets occupying them.]"
                );
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 11;
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
