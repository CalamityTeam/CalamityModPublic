using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class ElementalExcalibur : ModItem
    {
        private static int BaseDamage = 2000;
        private int BeamType = 0;
        private const int alpha = 50;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Elemental Excalibur");
            Tooltip.SetDefault("Freezes enemies and heals the player on hit\n" +
                "Fires rainbow beams that change their behavior based on their color\n" +
                "Right click for true melee");
        }

        public override void SetDefaults()
        {
            item.damage = BaseDamage;
            item.useAnimation = 14;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 14;
            item.useTurn = true;
            item.melee = true;
            item.knockBack = 8f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.width = 112;
            item.height = 112;
            item.value = Item.buyPrice(5, 0, 0, 0);
            item.rare = ItemRarityID.Purple;
            item.shoot = ModContent.ProjectileType<ElementalExcaliburBeam>();
            item.shootSpeed = 6f;
            item.Calamity().customRarity = CalamityRarity.Rainbow;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 10;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, (float)BeamType, 0f);

            BeamType++;
            if (BeamType > 11)
                BeamType = 0;

            return false;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.shoot = ProjectileID.None;
                item.shootSpeed = 0f;
            }
            else
            {
                item.shoot = ModContent.ProjectileType<ElementalExcaliburBeam>();
                item.shootSpeed = 12f;
            }

            return base.CanUseItem(player);
        }

        public override void ModifyHitNPC(Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
        {
            if (player.altFunctionUse == 2)
                damage *= 2;
        }

        public override void ModifyHitPvp(Player player, Player target, ref int damage, ref bool crit)
        {
            if (player.altFunctionUse == 2)
                damage *= 2;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(4))
            {
                Color color = new Color(255, 0, 0, alpha);
                switch (BeamType)
                {
                    case 0: // Red
                        break;
                    case 1: // Orange
                        color = new Color(255, 128, 0, alpha);
                        break;
                    case 2: // Yellow
                        color = new Color(255, 255, 0, alpha);
                        break;
                    case 3: // Lime
                        color = new Color(128, 255, 0, alpha);
                        break;
                    case 4: // Green
                        color = new Color(0, 255, 0, alpha);
                        break;
                    case 5: // Turquoise
                        color = new Color(0, 255, 128, alpha);
                        break;
                    case 6: // Cyan
                        color = new Color(0, 255, 255, alpha);
                        break;
                    case 7: // Light Blue
                        color = new Color(0, 128, 255, alpha);
                        break;
                    case 8: // Blue
                        color = new Color(0, 0, 255, alpha);
                        break;
                    case 9: // Purple
                        color = new Color(128, 0, 255, alpha);
                        break;
                    case 10: // Fuschia
                        color = new Color(255, 0, 255, alpha);
                        break;
                    case 11: // Hot Pink
                        color = new Color(255, 0, 128, alpha);
                        break;
                    default:
                        break;
                }

                Dust dust24 = Main.dust[Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 267, 0f, 0f, alpha, color, 1.2f)];
                dust24.noGravity = true;
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<ExoFreeze>(), 60);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 240);
            target.AddBuff(BuffID.Frostburn, 300);
            target.AddBuff(BuffID.OnFire, 360);

            if (!target.canGhostHeal || player.moonLeech)
                return;

            int healAmount = Main.rand.Next(3) + 10;
            player.statLife += healAmount;
            player.HealEffect(healAmount);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<ExoFreeze>(), 60);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 240);
            target.AddBuff(BuffID.Frostburn, 300);
            target.AddBuff(BuffID.OnFire, 360);

            if (player.moonLeech)
                return;

            int healAmount = Main.rand.Next(3) + 10;
            player.statLife += healAmount;
            player.HealEffect(healAmount);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<GreatswordofBlah>());
            recipe.AddIngredient(ItemID.TrueExcalibur);
            recipe.AddIngredient(ItemID.LargeDiamond);
            recipe.AddIngredient(ItemID.LightShard, 3);
            recipe.AddIngredient(ItemID.DarkShard, 3);
            recipe.AddIngredient(ModContent.ItemType<LivingShard>(), 10);
            recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 10);
            recipe.AddIngredient(ItemID.SoulofLight, 20);
            recipe.AddIngredient(ItemID.SoulofNight, 20);
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
