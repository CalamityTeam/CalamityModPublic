using Terraria.DataStructures;
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
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Thrust;
            Item.useTurn = false;
            Item.useAnimation = 25;
            Item.useTime = 25;
            Item.width = 58;
            Item.height = 58;
            Item.damage = 90;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 8.25f;
            Item.UseSound = SoundID.Item1;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<DNA>();
            Item.shootSpeed = 32f;
            Item.value = Item.buyPrice(0, 80, 0, 0);
            Item.rare = ItemRarityID.Yellow;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position.X, position.Y, Item.shootSpeed * player.direction, 0f, type, damage, knockback, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 234);
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
                player.immuneTime = Item.useTime / 5;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CoreofCalamity>().
                AddIngredient<LifeAlloy>(5).
                AddIngredient(ItemID.SoulofLight, 5).
                AddIngredient(ItemID.SoulofNight, 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
