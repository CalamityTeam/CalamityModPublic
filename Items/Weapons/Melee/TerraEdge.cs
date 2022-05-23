using Terraria.DataStructures;
using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TerraEdge : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Terra Edge");
            Tooltip.SetDefault("Heals the player on enemy hits\n" +
                "Fires a beam that inflicts ichor for a short time");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 58;
            Item.scale = 1.5f;
            Item.damage = 112;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 17;
            Item.useTurn = true;
            Item.knockBack = 6.25f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.buyPrice(0, 80, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ModContent.ProjectileType<TerraEdgeBeam>();
            Item.shootSpeed = 12f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, (int)(damage * 0.75), knockback, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 107);
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Ichor, 90);

            if (!target.canGhostHeal || player.moonLeech)
                return;

            int healAmount = Main.rand.Next(2) + 2;
            player.statLife += healAmount;
            player.HealEffect(healAmount);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Ichor, 90);

            if (player.moonLeech)
                return;

            int healAmount = Main.rand.Next(2) + 2;
            player.statLife += healAmount;
            player.HealEffect(healAmount);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<TrueBloodyEdge>().
                AddIngredient(ItemID.TrueExcalibur).
                AddIngredient<LivingShard>(7).
                AddTile(TileID.MythrilAnvil).
                Register();
            CreateRecipe().
                AddIngredient(ItemID.TrueNightsEdge).
                AddIngredient(ItemID.TrueExcalibur).
                AddIngredient<LivingShard>(7).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
