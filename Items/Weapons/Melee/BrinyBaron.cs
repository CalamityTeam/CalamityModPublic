using Terraria.DataStructures;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class BrinyBaron : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Briny Baron");
            Tooltip.SetDefault("Striking an enemy with the blade causes a briny typhoon to appear\n" +
                "Right click to fire a razorwind aqua blade");
            SacrificeTotal = 1;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 124;
            Item.knockBack = 4f;
            Item.useAnimation = Item.useTime = 15;
            Item.DamageType = DamageClass.Melee;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.shootSpeed = 4f;

            Item.shoot = ModContent.ProjectileType<Razorwind>();

            Item.width = 100;
            Item.height = 102;
            Item.scale = 1.5f;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;

            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.noMelee = true;
            }
            else
            {
                Item.noMelee = false;
            }

            return base.UseItem(player);
        }

        public override float UseSpeedMultiplier(Player player)
        {
            if (player.altFunctionUse == 2)
                return 1f;
            return 1.33f;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                damage = (int)(damage * 0.43);
                type = ModContent.ProjectileType<Razorwind>();
            }

            else 
                type = ProjectileID.None;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 187, 0f, 0f, 100, new Color(53, Main.DiscoG, 255));
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            var source = player.GetSource_ItemUse(Item);
            if (crit)
                damage /= 2;

            if (player.ownedProjectileCounts[ModContent.ProjectileType<BrinySpout>()] == 0)
                Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<BrinyTyphoonBubble>(), damage, knockback, player.whoAmI);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            var source = player.GetSource_ItemUse(Item);
            if (crit)
                damage /= 2;

            if (player.ownedProjectileCounts[ModContent.ProjectileType<BrinySpout>()] == 0)
                Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<BrinyTyphoonBubble>(), damage, Item.knockBack, player.whoAmI);
        }

        public override void UseAnimation(Player player)
        {
            Item.noUseGraphic = false;
            Item.UseSound = SoundID.Item1;

            if (player.altFunctionUse == 2)
            {
                Item.noUseGraphic = true;
                Item.UseSound = SoundID.Item84;
            }
        }
    }
}
