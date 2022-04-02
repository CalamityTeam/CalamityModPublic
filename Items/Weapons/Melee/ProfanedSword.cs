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
            item.damage = 100;
            item.melee = true;
            item.width = item.height = 52;
            item.scale = 1.5f;
            item.useTime = 23;
            item.useAnimation = 23;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 7.5f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.shootSpeed = 20f;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.noMelee = true;
                item.noUseGraphic = true;
                item.shoot = ModContent.ProjectileType<ProfanedSwordProj>();
            }
            else
            {
                item.noMelee = false;
                item.noUseGraphic = false;
                item.shoot = ProjectileID.None;
            }
            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<ProfanedSwordProj>(), damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (crit)
                damage /= 2;

            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 300);
            Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<Brimblast>(), damage, knockback, Main.myPlayer);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            if (crit)
                damage /= 2;

            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 300);
            Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<Brimblast>(), damage, item.knockBack, Main.myPlayer);
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(4))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, (int)CalamityDusts.Brimstone);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<UnholyCore>(), 6);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
