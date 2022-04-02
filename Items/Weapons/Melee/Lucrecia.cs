using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Lucrecia : ModItem
    {
        public const int OnHitIFrames = 5;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lucrecia");
            Tooltip.SetDefault("Finesse\n" +
                "Striking an enemy makes you immune for a short time\n" +
                "Fires a DNA chain");
        }

        public override void SetDefaults()
        {
            item.useStyle = ItemUseStyleID.Stabbing;
            item.useTurn = false;
            item.useAnimation = 25;
            item.useTime = 25;
            item.width = 58;
            item.height = 58;
            item.damage = 90;
            item.melee = true;
            item.knockBack = 8.25f;
            item.UseSound = SoundID.Item1;
            item.useTurn = true;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<DNA>();
            item.shootSpeed = 32f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = ItemRarityID.Yellow;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, item.shootSpeed * player.direction, 0f, type, damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CoreofCalamity>());
            recipe.AddIngredient(ModContent.ItemType<BarofLife>(), 5);
            recipe.AddIngredient(ItemID.SoulofLight, 5);
            recipe.AddIngredient(ItemID.SoulofNight, 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 234);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            player.GiveIFrames(OnHitIFrames, false);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            bool isImmune = false;
            for (int j = 0; j < player.hurtCooldowns.Length; j++)
            {
                if (player.hurtCooldowns[j] > 0)
                    isImmune = true;
            }
            if (!isImmune)
            {
                player.immune = true;
                player.immuneTime = item.useTime / 5;
            }
        }
    }
}
