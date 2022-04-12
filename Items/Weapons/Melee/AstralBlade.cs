using Terraria.DataStructures;
using Terraria.DataStructures;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Placeables;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Items.Weapons.Melee
{
    public class AstralBlade : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Blade");
            Tooltip.SetDefault("Deals more damage the more life an enemy has left");
        }

        public override void SetDefaults()
        {
            Item.damage = 85;
            Item.DamageType = DamageClass.Melee;
            Item.width = 80;
            Item.height = 80;
            Item.scale = 1.5f;
            Item.useTime = 9;
            Item.useAnimation = 9;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4f;
            Item.value = Item.buyPrice(0, 95, 0, 0);
            Item.rare = ItemRarityID.Cyan;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 25;

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 300);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 300);
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            for (int i = 0; i < 3; i++)
            {
                Dust d = CalamityUtils.MeleeDustHelper(player, Main.rand.NextBool(2) ? ModContent.DustType<AstralOrange>() : ModContent.DustType<AstralBlue>(), 0.7f, 55, 110, -0.07f, 0.07f);
                if (d != null)
                {
                    d.customData = 0.03f;
                }
            }
        }

        public override void ModifyHitNPC(Player player, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            float lifeRatio = target.life / (float)target.lifeMax;
            float multiplier = MathHelper.Lerp(1f, 2f, lifeRatio);

            damage = (int)(damage * multiplier);
            knockback *= multiplier;

            if (!crit)
                crit = Main.rand.NextBool((int)MathHelper.Clamp((Item.crit + player.GetCritChance(DamageClass.Melee)) * multiplier, 0f, 99f), 100);

            if (multiplier > 1.5f)
            {
                SoundEngine.PlaySound(SoundID.Item105, Main.player[Main.myPlayer].position);
                bool blue = Main.rand.NextBool();
                float angleStart = Main.rand.NextFloat(0f, MathHelper.TwoPi);
                float var = 0.05f + (2f - multiplier);
                for (float angle = 0f; angle < MathHelper.TwoPi; angle += var)
                {
                    blue = !blue;
                    Vector2 velocity = angle.ToRotationVector2() * (2f + (float)(Math.Sin(angleStart + angle * 3f) + 1) * 2.5f) * Main.rand.NextFloat(0.95f, 1.05f);
                    Dust d = Dust.NewDustPerfect(target.Center, blue ? ModContent.DustType<AstralBlue>() : ModContent.DustType<AstralOrange>(), velocity);
                    d.customData = 0.025f;
                    d.scale = multiplier - 0.75f;
                    d.noLight = false;
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AstralBar>(8).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
