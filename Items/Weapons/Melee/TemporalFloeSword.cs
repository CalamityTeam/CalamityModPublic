using Terraria.DataStructures;
using Terraria.DataStructures;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TemporalFloeSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Temporal Floe Sword");
            Tooltip.SetDefault("The iceman cometh...\n" +
                "Fires a frozen sword beam that splits into two more beams\n" +
                "The direction of the beams depends on in game time");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 52;
            Item.damage = 95;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 16;
            Item.useTurn = true;
            Item.knockBack = 6;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 60;
            Item.value = Item.buyPrice(0, 80, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ModContent.ProjectileType<TemporalFloeSwordProjectile>();
            Item.shootSpeed = 16f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, (int)(damage * 0.65), knockback, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 34);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GlacialState>(), 60);
            target.AddBuff(BuffID.Frostburn, 300);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GlacialState>(), 60);
            target.AddBuff(BuffID.Frostburn, 300);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<VerstaltiteBar>(15).
                AddIngredient<SeaPrism>(15).
                AddIngredient(ItemID.Ectoplasm, 5).
                AddTile(TileID.IceMachine).
                Register();
        }
    }
}
