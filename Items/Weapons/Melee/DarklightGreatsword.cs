using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class DarklightGreatsword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Darklight Greatsword");
            Tooltip.SetDefault("Fires darklight blades that split on death");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 92;
            Item.damage = 123;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 36;
            Item.useTurn = true;
            Item.knockBack = 5;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 100;
            Item.scale = 1.5f;
            Item.value = CalamityGlobalItem.Rarity6BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<DarkBeam>();
            Item.shootSpeed = 25f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            type = Main.rand.NextBool(2) ? type : ModContent.ProjectileType<LightBeam>();
            Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, (int)(damage * 0.8), knockback, player.whoAmI);
            return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 29);
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 180);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 180);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CryonicBar>(12).
                AddIngredient(ItemID.FallenStar, 5).
                AddIngredient(ItemID.SoulofNight).
                AddIngredient(ItemID.SoulofLight).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
