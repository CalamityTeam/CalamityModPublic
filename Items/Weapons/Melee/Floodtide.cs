using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Floodtide : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.damage = 75;
            Item.DamageType = DamageClass.Melee;
            Item.width = 80;
            Item.height = 92;
            Item.useTime = 21;
            Item.useAnimation = 21;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6f;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<MiniSharkron>();
            Item.shootSpeed = 18f;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemLocation += new Vector2(-4f * player.direction, 2f * player.gravDir).RotatedBy(player.itemRotation);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 2; i++)
            {
                float SpeedX = velocity.X + (float)Main.rand.Next(-20, 21) * 0.05f;
                float SpeedY = velocity.Y + (float)Main.rand.Next(-20, 21) * 0.05f;
                int proj = Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, type, damage, knockback, player.whoAmI, 0f, 0f);
                if (proj.WithinBounds(Main.maxProjectiles))
                    Main.projectile[proj].DamageType = DamageClass.Melee;
            }
            return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 217);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SeaRemains>(12).
                AddIngredient<DepthCells>(10).
                AddIngredient(ItemID.SharkFin, 2).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
