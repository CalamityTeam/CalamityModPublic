using Terraria.DataStructures;
using Terraria.DataStructures;
using CalamityMod.Dusts;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class ProfanedSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimstone Sword");
            Tooltip.SetDefault("Summons brimstone geysers on hit\n" +
                "Right click to throw like a javelin that explodes on hit");
        }

        public override void SetDefaults()
        {
            Item.damage = 100;
            Item.DamageType = DamageClass.Melee;
            Item.width = Item.height = 52;
            Item.scale = 1.5f;
            Item.useTime = 23;
            Item.useAnimation = 23;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 7.5f;
            Item.value = Item.buyPrice(0, 36, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shootSpeed = 20f;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.noMelee = true;
                Item.noUseGraphic = true;
                Item.shoot = ModContent.ProjectileType<ProfanedSwordProj>();
            }
            else
            {
                Item.noMelee = false;
                Item.noUseGraphic = false;
                Item.shoot = ProjectileID.None;
            }
            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<ProfanedSwordProj>(), damage, knockback, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            var source = player.GetProjectileSource_Item(Item);
            if (crit)
                damage /= 2;

            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 300);
            Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<Brimblast>(), damage, knockback, Main.myPlayer);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            var source = player.GetProjectileSource_Item(Item);
            if (crit)
                damage /= 2;

            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 300);
            Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<Brimblast>(), damage, Item.knockBack, Main.myPlayer);
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(4))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, (int)CalamityDusts.Brimstone);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ModContent.ItemType<UnholyCore>(6).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
