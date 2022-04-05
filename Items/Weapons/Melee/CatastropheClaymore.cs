using Terraria.DataStructures;
using Terraria.DataStructures;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class CatastropheClaymore : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Catastrophe Claymore");
            Tooltip.SetDefault("Fires explosive energy bolts");
        }

        public override void SetDefaults()
        {
            Item.width = 56;
            Item.damage = 98;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 23;
            Item.useTime = 23;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6.25f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 56;
            Item.value = Item.buyPrice(0, 48, 0, 0);
            Item.rare = ItemRarityID.LightPurple;
            Item.shoot = ModContent.ProjectileType<CalamityAura>();
            Item.shootSpeed = 11f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            type = Utils.SelectRandom(Main.rand, new int[]
            {
                ModContent.ProjectileType<CalamityAura>(),
                ModContent.ProjectileType<CalamityAuraType2>(),
                ModContent.ProjectileType<CalamityAuraType3>()
            });

            Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, Main.myPlayer);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.HallowedBar, 10).AddIngredient(ItemID.CrystalShard, 7).AddIngredient(ItemID.SoulofNight, 5).AddRecipeGroup("CursedFlameIchor", 5).AddIngredient(ItemID.SoulofMight, 3).AddIngredient(ItemID.SoulofSight, 3).AddIngredient(ItemID.SoulofFright, 3).AddTile(TileID.MythrilAnvil).Register();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 73);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (Main.rand.NextBool(3))
            {
                target.AddBuff(BuffID.Ichor, 60);
                target.AddBuff(BuffID.OnFire, 180);
                target.AddBuff(BuffID.Frostburn, 120);
            }
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            if (Main.rand.NextBool(3))
            {
                target.AddBuff(BuffID.Ichor, 60);
                target.AddBuff(BuffID.OnFire, 180);
                target.AddBuff(BuffID.Frostburn, 120);
            }
        }
    }
}
